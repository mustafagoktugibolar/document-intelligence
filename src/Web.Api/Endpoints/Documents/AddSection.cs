using Application.Abstractions.Messaging;
using Application.Documents.AddSection;
using Domain.Documents;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Documents;

internal sealed class AddSection : IEndpoint
{
    public sealed class Request
    {
        public string Name { get; set; } = string.Empty;
        public int StartChunkIndex { get; set; }
        public int EndChunkIndex { get; set; }
        public SectionVisibility Visibility { get; set; }
        public string? SourceTag { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("documents/{id:guid}/sections", async (
            Guid id,
            Request request,
            ICommandHandler<AddDocumentSectionCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddDocumentSectionCommand
            {
                DocumentId = id,
                Name = request.Name,
                StartChunkIndex = request.StartChunkIndex,
                EndChunkIndex = request.EndChunkIndex,
                Visibility = request.Visibility,
                SourceTag = request.SourceTag
            };

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                sectionId => Results.Created($"sections/{sectionId}", sectionId),
                CustomResults.Problem);
        })
        .HasPermission(Permissions.DocumentsWrite)
        .WithTags(Tags.Documents);
    }
}
