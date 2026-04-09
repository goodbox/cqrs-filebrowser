# FileBrowser — CLAUDE.md

## Project Overview

File & Directory Browsing Single Page App. Clean architecture with a hand-rolled CQRS implementation, vanilla JS SPA, no server-side rendering.

## Solution Structure

```
src/
├── FileBrowser.sln
├── Domain/                     ← Pure domain records, no dependencies
├── Application/                ← CQRS interfaces + handlers, DTOs, common utilities
├── Infrastructure/             ← System.IO implementation, settings
└── Web/                        ← ASP.NET host, controllers, SPA assets
```

## Build & Run

```bash
dotnet build FileBrowser.sln
dotnet run --project Web
```

Default dev URL: `http://localhost:5000`

## Configuration

Root directory is set in `Web/appsettings.json` under `FileSystem.RootPath`.  
Development override: `Web/appsettings.Development.json` (defaults to `C:\Users\Public`).

## Architecture Rules

- **No server-side HTML rendering.** All UI is in `Web/wwwroot/`.
- **Custom CQRS — no MediatR.** The CQRS infrastructure lives in `Application/Common/Cqrs/`. Queries and commands live in `Application/Features/FileSystem/`. Controllers dispatch via `ISender` and do nothing else.
- **Path traversal prevention** is enforced in `Application/Common/PathValidator.cs` — every repository call validates the resolved path stays within `RootPath`.
- **DTOs** use static `From()` factory methods. No AutoMapper.
- **No bundler.** The SPA uses native ES modules (`type="module"`). No Node.js toolchain needed.

## Custom CQRS

MediatR was removed. The CQRS implementation is in `Application/Common/Cqrs/`:

| File | Purpose |
|------|---------|
| `IRequest.cs` | `IRequest<TResponse>` and `IRequest` marker interfaces |
| `IRequestHandler.cs` | `IRequestHandler<TRequest, TResponse>` and `IRequestHandler<TRequest>` |
| `ISender.cs` | Dispatcher interface used by controllers |
| `Mediator.cs` | Resolves handlers from DI via reflection and invokes them |
| `CqrsServiceExtensions.cs` | `AddCqrs()` — scans Application assembly, registers all handlers and `ISender` |

Register with `builder.Services.AddCqrs()` in `Program.cs`. To add a new feature, create a request record implementing `IRequest<T>` and a handler implementing `IRequestHandler<,>` — no other registration needed.

## Key Files

| File | Purpose |
|------|---------|
| `Application/Common/Cqrs/` | CQRS infrastructure (see above) |
| `Application/Common/IFileSystemRepository.cs` | Central contract all layers depend on |
| `Application/Common/PathValidator.cs` | Security — prevents `../` traversal |
| `Infrastructure/FileSystem/PhysicalFileSystemRepository.cs` | All System.IO operations |
| `Web/Controllers/FileSystemController.cs` | API surface: browse, search, download, upload, delete, move |
| `Web/Program.cs` | Host bootstrap — registration order matters |
| `Web/wwwroot/js/app.js` | SPA entry point |
| `Web/wwwroot/js/router.js` | Query-string routing with `history.pushState` |
| `Web/wwwroot/js/api.js` | All fetch/XHR calls to the API |

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/fs/browse?path=` | Directory listing |
| GET | `/api/fs/search?q=&path=&recursive=` | Search |
| GET | `/api/fs/download?path=` | Download file |
| POST | `/api/fs/upload?path=` | Upload (multipart/form-data) |
| DELETE | `/api/fs/entry?path=` | Delete file or folder |
| POST | `/api/fs/move` | Move/rename (JSON body `{from, to}`) |

## SPA URL Scheme

State is encoded in query strings for deep-linkable URLs:

```
/                         → root directory
/?path=Documents          → browse subdirectory
/?path=Documents&q=report → search within dir
/?q=report&recursive=1    → recursive search from root
```
