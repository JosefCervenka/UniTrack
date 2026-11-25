namespace Application.Services;

using Pgvector;
using SmartComponents.LocalEmbeddings;

public class EmbeddingService(LocalEmbedder embedder)
{
    private Task<float[]> EmbedTextAsync(string text, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ct.ThrowIfCancellationRequested();

        EmbeddingF32 embedding = embedder.Embed(text);
        return Task.FromResult(embedding.Values.ToArray());
    }

    public async Task<Vector> EmbedToPgVectorAsync(string text, CancellationToken ct = default)
    {
        float[] values = await EmbedTextAsync(text, ct);
        return new Vector(values);
    }
}