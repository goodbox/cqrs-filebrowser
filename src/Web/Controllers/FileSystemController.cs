using Application.Features.FileSystem.Commands.DeleteEntry;
using Application.Features.FileSystem.Commands.MoveEntry;
using Application.Features.FileSystem.Commands.UploadFile;
using Application.Features.FileSystem.DTOs;
using Application.Features.FileSystem.Queries.GetDirectoryListing;
using Application.Features.FileSystem.Queries.GetFileDownload;
using Application.Features.FileSystem.Queries.SearchFileSystem;
using Application.Common.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/fs")]
public sealed class FileSystemController : ControllerBase
{
    private readonly ISender _sender;

    public FileSystemController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("browse")]
    public async Task<IActionResult> Browse([FromQuery] string path = "", CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetDirectoryListingQuery(path), ct);
        return Ok(DirectoryListingDto.From(result));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] string path = "",
        [FromQuery] bool recursive = false,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { title = "Bad Request", detail = "Search query 'q' is required." });

        var result = await _sender.Send(new SearchFileSystemQuery(q, path, recursive), ct);
        return Ok(SearchResultDto.From(result));
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download([FromQuery] string path, CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetFileDownloadQuery(path), ct);
        return File(result.Content, result.ContentType, result.FileName, enableRangeProcessing: true);
    }

    [HttpPost("upload")]
    [RequestSizeLimit(104_857_600)]
    public async Task<IActionResult> Upload([FromQuery] string path = "", CancellationToken ct = default)
    {
        if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
            return BadRequest(new { title = "Bad Request", detail = "No files provided." });

        var results = new List<UploadFileResult>();

        foreach (var file in Request.Form.Files)
        {
            var command = new UploadFileCommand(
                path,
                file.FileName,
                file.OpenReadStream(),
                file.Length);

            var result = await _sender.Send(command, ct);
            results.Add(result);
        }

        return Ok(results.Select(r => new { r.RelativePath, r.FileName, r.SizeBytes }));
    }

    [HttpDelete("entry")]
    public async Task<IActionResult> Delete([FromQuery] string path, CancellationToken ct = default)
    {
        await _sender.Send(new DeleteEntryCommand(path), ct);
        return NoContent();
    }

    [HttpPost("move")]
    public async Task<IActionResult> Move([FromBody] MoveRequest body, CancellationToken ct = default)
    {
        await _sender.Send(new MoveEntryCommand(body.From, body.To), ct);
        return NoContent();
    }
}

public sealed record MoveRequest(string From, string To);
