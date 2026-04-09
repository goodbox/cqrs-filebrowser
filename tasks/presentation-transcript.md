# File Browser SPA — Video Presentation Transcript
### ~6 minutes

---

**[0:00 — Intro]**

Hey everyone. I'm going to walk through a file browser web application I built as a proof-of-concept. It's a full-stack .NET 8 app with a clean architecture backend and a vanilla JavaScript single-page application on the front end. No React, no Angular — just the platform. Let's dig in.

---

**[0:20 — What it does]**

The app lets you browse and search directories on the server, upload and download files, and move or delete entries — all from the browser. The UI state is encoded in the URL, so every view is deep-linkable. You can paste a URL to a specific folder or search query and the app restores exactly that state.

The server-side root directory is configurable, so you point it at whatever folder you want to expose. Everything is rendered client-side — the server only serves JSON from API endpoints and static files. There's no server-rendered HTML at all.

---

**[1:00 — Solution structure]**

The solution has four projects, each with a distinct responsibility.

`Domain` is the innermost layer. It has no dependencies on anything — just plain C# records representing the core concepts: `FileEntry`, `DirectoryEntry`, `DirectoryListing`, and `SearchResult`. Domain exceptions like `PathTraversalException` also live here.

`Application` is the CQRS layer. It holds the interfaces the rest of the system depends on — `IFileSystemRepository` and `IFileSystemSettings` — and all the query and command handlers. It also contains our custom CQRS implementation, which I'll come back to.

`Infrastructure` is where the real I/O happens. It implements `IFileSystemRepository` using `System.IO`, and binds configuration from `appsettings.json`.

And `Web` is the host — ASP.NET Core controllers, middleware, and all the static frontend assets.

The dependency arrows only point inward. Web depends on Application and Infrastructure. Infrastructure depends on Application. Application depends only on Domain. Domain has no dependencies at all.

---

**[2:00 — Custom CQRS]**

The project uses CQRS — Command Query Responsibility Segregation — to separate read operations from write operations. Rather than pulling in a third-party library, we rolled our own implementation. It lives in four files under `Application/Common/Cqrs`.

We define three interfaces: `IRequest<TResponse>` marks a query or command that returns a value. `IRequest` marks a command that returns nothing — void, essentially. And `IRequestHandler` has two variants to match.

The `ISender` interface has two `Send` overloads — one returning `Task<TResponse>`, one returning `Task`.

The `Mediator` class implements `ISender`. It takes `IServiceProvider`, and when `Send` is called, it uses reflection to find and invoke the right `IRequestHandler` from the DI container. It builds the open generic handler type, resolves it via `GetRequiredService`, and calls `Handle`.

Registration is done through a single `AddCqrs()` extension method that scans the Application assembly, finds every class implementing a handler interface, and registers it. One call in `Program.cs` wires everything up.

The result is that the controllers know nothing about handlers, and handlers know nothing about each other. Adding a new feature means adding a query or command class, a handler class, and nothing else.

---

**[3:10 — API layer]**

The API surface is six endpoints on a single controller at `/api/fs`.

`GET /browse` returns a directory listing as JSON — files, subdirectories, counts, and total size.

`GET /search` accepts a query string and an optional recursive flag. It searches file and folder names within a given path and returns matches.

`GET /download` streams a file back to the browser using ASP.NET's `FileStreamResult` with range processing enabled, so large files and media playback work correctly.

`POST /upload` accepts multipart form data. Multiple files can be uploaded in one request.

`DELETE /entry` and `POST /move` handle the bonus operations — deletion and move or rename.

All error handling flows through a `GlobalExceptionMiddleware`. Domain exceptions map to specific HTTP status codes: path traversal attempts return 400, not-found cases return 404. Controllers themselves are completely clean — they receive a request, dispatch it via `ISender`, map the result to a DTO, and return it.

---

**[4:10 — Path security]**

One thing worth calling out explicitly is path traversal prevention. The server exposes a configured root directory, but a malicious request could send a path like `../../etc/passwd` to try to escape that root.

`PathValidator` in the Application layer handles this. It normalizes both the root path and the combined path using `Path.GetFullPath`, which resolves all `..` segments without touching the filesystem. It then asserts the result starts with the normalized root. If it doesn't, it throws `PathTraversalException`, which the middleware converts to a 400.

Every single repository method calls `PathValidator` before doing any I/O. It's enforced at the Application boundary, not just the controller.

---

**[4:50 — Frontend SPA]**

The frontend is a vanilla JavaScript single-page application served from `wwwroot/index.html`. It uses native ES modules — `type="module"` on the script tag — so there's no build step, no bundler, no Node toolchain required. `dotnet build` is all you need.

The architecture is straightforward. `state.js` is a minimal pub/sub store — a plain object with an `Object.assign`-based `setState` function and a subscriber list. `router.js` reads and writes URL query strings, using `history.pushState` for navigation and listening to `popstate` for browser back/forward. `api.js` contains all fetch calls, with one exception: uploads use XHR instead, because the Fetch API doesn't expose upload progress events.

Components are plain functions that accept a container element and render HTML into it. `fileBrowser.js` orchestrates the main view — it calls the browse API, then renders the breadcrumb, file list, and status bar. `searchView.js` handles search results. `uploadDialog.js` is a modal with drag-and-drop, a file picker, and a live progress bar.

Because all state lives in the URL, hitting refresh or pasting a link always restores the exact view. There's no client-side state that can go stale.

---

**[5:45 — Wrap-up]**

To summarize: clean architecture with strict dependency rules, a hand-rolled CQRS dispatcher that keeps controllers and handlers completely decoupled, explicit path traversal guards at the Application boundary, and a zero-dependency frontend that's fully deep-linkable.

The whole thing builds with `dotnet build` and runs with `dotnet run --project Web`. No separate frontend build step.

Thanks for watching.

---

*Total estimated reading time at ~130 words/min: ~6 minutes*
