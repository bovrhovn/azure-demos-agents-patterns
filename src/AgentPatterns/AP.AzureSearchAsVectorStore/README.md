# AP.AzureSearchAsVectorStore — Azure AI Search as Vector Store

## What This Pattern Does

The **AzureSearchAsVectorStore** pattern demonstrates how to use [Azure AI Search](https://learn.microsoft.com/azure/search/) as a vector database for semantic / similarity search. The demo:

1. Creates (or updates) an Azure AI Search index with a vector field.
2. Generates embeddings for a small movie catalogue using Azure OpenAI embeddings.
3. Uploads the documents and their vectors to the index.
4. Runs two natural-language queries using k-nearest-neighbour (kNN) vector search.

## When to Use It

- Retrieval-Augmented Generation (RAG): store knowledge base documents and retrieve relevant chunks before calling an LLM.
- Semantic search over unstructured text where keyword matching is insufficient.
- Recommendation systems based on content similarity.

## Prerequisites

| Requirement | Details |
|-------------|---------|
| .NET 10 SDK | <https://dot.net> |
| Azure OpenAI resource | An endpoint and an **embeddings** deployment (e.g. `text-embedding-ada-002`) |
| Azure AI Search resource | A search service in the same region (or any region) |
| `Endpoint` env var | Your Azure OpenAI endpoint URL |
| `SearchEndpoint` env var | Your Azure AI Search endpoint URL |
| `DeploymentName` env var | Name of the embeddings model deployment |

## Running the Demo

```bash
export Endpoint="https://<openai-resource>.openai.azure.com/"
export SearchEndpoint="https://<search-resource>.search.windows.net"
export DeploymentName="text-embedding-ada-002"

cd src/AgentPatterns/AP.AzureSearchAsVectorStore
dotnet run
```

On Windows (PowerShell):

```powershell
$env:Endpoint       = "https://<openai-resource>.openai.azure.com/"
$env:SearchEndpoint = "https://<search-resource>.search.windows.net"
$env:DeploymentName = "text-embedding-ada-002"

dotnet run
```

## Code Walkthrough

### `MovieFactory.cs`

`MovieFactory` is a simple in-process data source that returns a list of `Movie` objects:

```csharp
public static List<Movie> GetMovieVectorList()
{
    return [
        new Movie { Key = "1", Title = "The Shawshank Redemption", Year = 1994, ... },
        new Movie { Key = "2", Title = "Dracula: A Love Tale",     Year = 2025, ... },
        // ...
    ];
}
```

The `Movie` class holds structured fields plus a `Vector` field (`ReadOnlyMemory<float>`) populated at runtime:

```csharp
public class Movie
{
    public required string Key  { get; set; }
    public required string Title { get; set; }
    public int Year { get; set; }
    public required string Category { get; set; }
    public string Description { get; set; } = "";
    public ReadOnlyMemory<float> Vector { get; set; }
}
```

### Index Creation

The index is created with an HNSW algorithm configuration and a 1 536-dimension vector field:

```csharp
new SearchField("Vector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
{
    IsSearchable = true,
    VectorSearchDimensions = 1536,
    VectorSearchProfileName = "vector-profile"
}
```

### Generating Embeddings

Embeddings are generated using `IEmbeddingGenerator<string, Embedding<float>>`:

```csharp
IEmbeddingGenerator<string, Embedding<float>> generator =
    new AzureOpenAIClient(...)
        .GetEmbeddingClient(deploymentName)
        .AsIEmbeddingGenerator();

movie.Vector = await generator.GenerateVectorAsync(movie.Description);
```

### Vector Search

Queries use the `VectorizedQuery` with kNN to find the most similar documents:

```csharp
new VectorizedQuery(queryEmbedding.ToArray())
{
    KNearestNeighborsCount = resultCount,
    Fields = { "Vector" }
}
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `MovieFactory.GetMovieVectorList` | Returns the seed movie catalogue |
| `Movie` | Data model with structured fields and a float vector |
| `SearchIndexClient.CreateOrUpdateIndexAsync` | Creates the vector-enabled search index |
| `IEmbeddingGenerator.GenerateVectorAsync` | Generates a float vector from a text description |
| `SearchClient.IndexDocumentsAsync` | Uploads documents (with their vectors) to Azure AI Search |
| `VectorizedQuery` | Performs kNN vector similarity search |

## Learn More

| Resource | Link |
|----------|------|
| Azure AI Search overview | <https://learn.microsoft.com/azure/search/search-what-is-azure-search> |
| Vector search in Azure AI Search | <https://learn.microsoft.com/azure/search/vector-search-overview> |
| Create a vector index | <https://learn.microsoft.com/azure/search/vector-search-how-to-create-index> |
| Azure OpenAI embeddings | <https://learn.microsoft.com/azure/ai-services/openai/concepts/understand-embeddings> |
| IEmbeddingGenerator in Microsoft.Extensions.AI | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| RAG with Azure AI Search | <https://learn.microsoft.com/azure/search/retrieval-augmented-generation-overview> |
