# File & Directory Browsing Single Page App

## Overview

A web service API and single-page application that allows users to query, search, and browse directories on the web server. The server-side home directory is configurable via a variable. All rendering is client-side via JavaScript — no server-side HTML rendering.

The goal is a functional proof-of-concept with enough code to evaluate design decisions, logic, and coding style.

---

## Constraints

- Build must work in Visual Studio 2022+, Rider, VS Code, or CLI SDK tools
- UI must be built with **vanilla JavaScript or TypeScript** — no React, Angular, or other UI library
- No server-side HTML rendering; all UI is rendered client-side
- Styling is secondary — functionality is the priority
- put everything in the src folder
- use this file structure, only create folders as needed 
src/
 ├── Web/                         <-- Single host project
 │    ├── Controllers/            (API endpoints only)
 │    ├── Pages/ or Views/        (Razor Pages or MVC Views)
 │    ├── Components/             (Blazor, if applicable)
 │    ├── wwwroot/                (static assets: js, css, images)
 │    │    ├── js/
 │    │    ├── css/
 │    │    └── lib/
 │    │
 │    ├── Filters/
 │    ├── Middleware/
 │    ├── ViewModels/             (UI-specific shaping only)
 │    └── Program.cs
 │
 ├── Application/                 <-- CQRS lives here
 │    ├── Common/
 │    └── Features/
 │         └── Orders/
 │              ├── Commands/
 │              ├── Queries/
 │              └── DTOs/
 │
 ├── Domain/
 ├── Infrastructure/
 - in the wwwroot folder, put a single index.html file, and use that as the source of the single page application

---

## Requirements

### API
- use cqrs forthe actual implementation
- do not use mediatr, write your own cqrs implementation
- Browse directory contents (files and folders) — returns JSON
- Search files and folders — returns JSON
- Upload files
- Download files
- Configurable server-side home/root directory

### Frontend (SPA)

- Single Page App — JavaScript renders all HTML
- Deep-linkable URLs — UI state (current directory, search query, etc.) is reflected in and restored from the URL
- Display file and folder counts and sizes for the current view
- File upload and download from the browser

---

## Bonus / Nice to Have

- File/folder operations: delete, move, copy
- Entire component contained in a dialog widget with a trigger element (button, etc.)
- Performance optimizations (highly valued)
- Any other cool additions

---

## Acceptance Criteria

- [ ] Project builds via `dotnet build` or Visual Studio
- [ ] API returns JSON for browse and search operations
- [ ] Navigating to a deep link restores the correct UI state
- [ ] File and folder counts and sizes are shown for the current directory view
- [ ] Files can be uploaded and downloaded from the browser
- [ ] No server-side HTML rendering — all UI is JavaScript-driven
