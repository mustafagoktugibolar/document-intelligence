Scaffold a new feature for the Document Intelligence platform following Clean Architecture patterns.

If not provided in args, ask the user:
1. Feature name (e.g., "UploadDocument", "GetDocumentById")
2. Type: Command (write) or Query (read)
3. Which entity it belongs to (Document, ProcessingJob, etc.)

Then create these files, following the exact patterns of existing features:

**For a Command** — reference `src/Application/Todos/Create/` pattern:
- `src/Application/{Entity}/{Feature}/{Feature}Command.cs`
- `src/Application/{Entity}/{Feature}/{Feature}CommandHandler.cs`
- `src/Application/{Entity}/{Feature}/{Feature}CommandValidator.cs` (if validation needed)
- `src/Web.Api/Endpoints/{Entity}/{Feature}.cs` (Minimal API endpoint)

**For a Query** — reference `src/Application/Todos/GetById/` pattern:
- `src/Application/{Entity}/{Feature}/{Feature}Query.cs`
- `src/Application/{Entity}/{Feature}/{Feature}QueryHandler.cs`
- `src/Application/{Entity}/{Feature}/{Entity}Response.cs`
- `src/Web.Api/Endpoints/{Entity}/{Feature}.cs` (Minimal API endpoint)

Always:
- Use `Result<T>` return type
- Follow ICommand<T> / IQuery<T> interfaces from SharedKernel
- Register endpoint in the existing DI pattern (endpoints are auto-discovered)
- Use `HasPermission` attribute where auth is needed
