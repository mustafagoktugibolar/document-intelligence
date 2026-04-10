using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Application.Documents.Upload;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Documents;

internal sealed class Upload : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("documents", async (
            [FromForm] Guid organizationId,
            IFormFile file,
            IFileStorageService storageService,
            ICommandHandler<UploadDocumentCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            await using Stream stream = file.OpenReadStream();
            string storageKey = await storageService.UploadAsync(
                stream, file.FileName, organizationId.ToString(), cancellationToken);

            UploadDocumentCommand command = new()
            {
                OrganizationId = organizationId,
                FileName = file.FileName,
                StorageKey = storageKey,
                FileSizeBytes = file.Length
            };

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(id => Results.Created($"documents/{id}", id), CustomResults.Problem);
        })
        .HasPermission(Permissions.DocumentsWrite)
        .WithTags(Tags.Documents)
        .DisableAntiforgery();
    }
}
