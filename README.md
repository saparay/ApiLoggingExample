# API Logging Example (ASP.NET Core)

This project demonstrates a **custom-built API logging framework** using **ASP.NET Core**, **Entity Framework Core**, and **SQL Server**.

The implementation focuses on **capturing complete request lifecycle data** (request, response, exceptions) and **persisting logs asynchronously** using an in-memory queue and a background worker — ensuring **high performance and zero impact on request execution**.

---

## ✨ What This Project Solves

Typical API logging problems:

* ❌ Blocking requests with database calls
* ❌ Logging directly inside `ILogger`
* ❌ Losing correlation between logs
* ❌ Missing request / response payloads

✅ This project solves all of the above using **middleware + background processing**.

---

## 🏗️ Overall Flow

```
HTTP Request
   ↓
CorrelationIdMiddleware
   ↓
RequestResponseLoggingMiddleware
   ↓
Controller / Business Logic
   ↓
ExceptionMiddleware
   ↓
FinalRequestLoggingMiddleware
   ↓
ILogQueue (ConcurrentQueue)
   ↓
LogWorker (BackgroundService)
   ↓
SQL Server (ApiLogs table)
```

---

## 🧩 Project Components

### 📦 Entities

#### `ApiLog`

Represents a single API execution log.

Captured fields include:

* TraceId (Correlation ID)
* HTTP Method & Path
* Status Code
* Request Body
* Response Body
* Exception Details
* Client Info (IP, Browser, Device)
* Timestamp

---

#### `RequestLogContext`

A **scoped context object** used to collect log messages throughout a single request lifecycle.

```csharp
public class RequestLogContext
{
    public List<string> Logs { get; } = new();
}
```

---

## 🗄️ Data Layer

### `LoggingDbContext`

* EF Core DbContext
* Maps `ApiLog` entity to `dbo.ApiLogs`

```csharp
public DbSet<ApiLog> ApiLogs { get; set; }
```

---

### Logging Queue

#### `ILogQueue`

Defines a contract for enqueuing and dequeuing logs.

```csharp
void Enqueue(ApiLog log);
bool TryDequeue(out ApiLog log);
```

#### `LogQueue`

* Uses `ConcurrentQueue<ApiLog>`
* Thread-safe
* Non-blocking

---

### `LogWorker` (BackgroundService)

* Continuously dequeues logs
* Writes logs to SQL Server using EF Core
* Uses `IServiceScopeFactory` for safe DbContext usage
* Swallows exceptions to prevent application crashes

---

## 🧱 Middleware

### 🔗 CorrelationIdMiddleware

* Reads `X-Correlation-Id` header (if present)
* Generates a new ID when missing
* Assigns it to `HttpContext.TraceIdentifier`
* Adds it to response headers

---

### 📥📤 RequestResponseLoggingMiddleware

* Captures request body using buffering
* Captures response body using memory stream
* Stores bodies in `HttpContext.Items`
* Adds readable messages to `RequestLogContext`

---

### ❗ ExceptionMiddleware

* Catches unhandled exceptions
* Adds exception details to `RequestLogContext`
* Returns a standardized error response

```json
{
  "traceId": "<correlation-id>",
  "message": "Internal Server Error"
}
```

---

### 🧾 FinalRequestLoggingMiddleware

* Executes after request completion
* Builds a complete `ApiLog` object
* Enqueues the log into `ILogQueue`

This ensures **no database access happens inside the request thread**.

---

## ⚙️ Application Configuration

### Service Registration (`Program.cs`)

Key registrations:

* `LoggingDbContext`
* `ILogQueue` as Singleton
* `RequestLogContext` as Scoped
* `LogWorker` as Hosted Service

Middleware order:

```csharp
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<FinalRequestLoggingMiddleware>();
```

---

## 🗄️ Database Schema

```sql
CREATE TABLE ApiLogs (
    Id INT IDENTITY PRIMARY KEY,
    TraceId NVARCHAR(100),
    LogLevel NVARCHAR(50),
    Message NVARCHAR(MAX),
    Exception NVARCHAR(MAX),
    Path NVARCHAR(200),
    Method NVARCHAR(20),
    StatusCode INT,
    RequestBody NVARCHAR(MAX),
    ResponseBody NVARCHAR(MAX),
    Device NVARCHAR(200),
    Browser NVARCHAR(500),
    IpAddress NVARCHAR(50),
    CreatedAt DATETIME2
);
```

---

## ▶️ Running the Project

1. Configure SQL Server connection string in `appsettings.json`
2. Create database and table
3. Run the application

```bash
dotnet run
```

---

## 🧪 How to Verify Logging

* Call any API endpoint
* Trigger an exception
* Check `ApiLogs` table
* Validate:

  * Correlation ID
  * Request & response bodies
  * Exception information

---

## ✅ Best Practices Demonstrated

* Asynchronous logging
* No EF Core usage in request pipeline
* Thread-safe queue processing
* Centralized exception handling
* Clean separation of concerns

---

## 📈 Possible Improvements

* Batch inserts for high traffic
* Sensitive data masking
* Log level filtering
* Integration with ELK / Azure App Insights

---

## 👨‍💻 Author

**Mani Chandra Saparay**

---

## 📜 License

This project is intended for learning and demonstration purposes.
