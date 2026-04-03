using Application.Abstractions.Messaging;
using Application.Documents.Upload;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Documents;

internal sealed class Upload : IEndpoint
{
    public sealed class Request
    {
        public Guid OrganizationId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("documents", async (
            Request request,
            ICommandHandler<UploadDocumentCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UploadDocumentCommand
            {
                OrganizationId = request.OrganizationId,
                FileName = request.FileName,
                StorageKey = request.StorageKey,
                FileSizeBytes = request.FileSizeBytes
            };

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(id => Results.Created($"documents/{id}", id), CustomResults.Problem);
        })
        .HasPermission(Permissions.DocumentsWrite)
        .WithTags(Tags.Documents);
    }
}
