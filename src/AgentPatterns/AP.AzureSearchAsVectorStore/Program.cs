using AP.AzureSearchAsVectorStore;
using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.AI;
using Spectre.Console;

AnsiConsole.MarkupLine("[bold green]Azure Search as Vector Store[/]");

#region Environment variables

var endpoint = Environment.GetEnvironmentVariable("Endpoint");
ArgumentException.ThrowIfNullOrEmpty(endpoint, "Endpoint environment variable is not set.");
var searchEndpoint = Environment.GetEnvironmentVariable("SearchEndpoint");
ArgumentException.ThrowIfNullOrEmpty(searchEndpoint, "searchEndpoint environment variable is not set.");
var deploymentName = Environment.GetEnvironmentVariable("DeploymentName");
ArgumentException.ThrowIfNullOrEmpty(deploymentName, "DeploymentName environment variable is not set.");

AnsiConsole.MarkupLine($"[green]Using Endpoint:[/] {endpoint}");
AnsiConsole.MarkupLine($"[green]Using Search Endpoint:[/] {searchEndpoint}");
AnsiConsole.MarkupLine($"[green]Using DeploymentName:[/] {deploymentName}");

#endregion

const string indexName = "movies";
const int vectorDimensions = 1536;

var indexClient = GetSearchIndexClient(searchEndpoint);

// create or update the search index with vector field
await CreateOrUpdateIndexAsync(indexClient);

var searchClient = indexClient.GetSearchClient(indexName);

// get movie list
// var movieData = MovieFactory.GetMovieVectorList();
var credentials = new DefaultAzureCredential();
IEmbeddingGenerator<string, Embedding<float>> generator =
    new AzureOpenAIClient(new Uri(endpoint), credentials)
    .GetEmbeddingClient(deploymentName)
    .AsIEmbeddingGenerator();
//
// // generate embeddings and upload documents to Azure AI Search
// AnsiConsole.MarkupLine("[blue]Processing movies to Azure AI search[/]");
// var documents = new List<SearchDocument>();
// foreach (var movie in movieData)
// {
//     movie.Vector = await generator.GenerateVectorAsync(movie.Description);
//     var doc = new SearchDocument
//     {
//         ["Key"] = movie.Key,
//         ["Title"] = movie.Title,
//         ["Year"] = movie.Year,
//         ["Category"] = movie.Category,
//         ["Description"] = movie.Description,
//         ["Vector"] = movie.Vector.ToArray()
//     };
//     documents.Add(doc);
//     AnsiConsole.MarkupLine($"[gray]Processed[/] {movie.Title}");
// }
// await searchClient.IndexDocumentsAsync(IndexDocumentsBatch.Upload(documents));
//
// // wait briefly for indexing to complete
// await Task.Delay(2000);

// creates a list of questions
var questions = new List<(string Question, int ResultCount)>
{
    ("A family friendly movie that includes travel to future", 1),
    ("Movie released in year 1980 and 1990", 2)
};

foreach (var question in questions)
{
    await SearchMovieAsync(question.Question, question.ResultCount);
}

async Task SearchMovieAsync(string question, int resultCount)
{
    AnsiConsole.WriteLine($"====================================================");
    AnsiConsole.MarkupLine($"[blue]Searching for:[/] {question}");
    AnsiConsole.WriteLine();

    // generate query embedding and perform vector search
    var queryEmbedding = await generator.GenerateVectorAsync(question);

    var searchOptions = new SearchOptions
    {
        VectorSearch = new VectorSearchOptions
        {
            Queries =
            {
                new VectorizedQuery(queryEmbedding.ToArray())
                {
                    KNearestNeighborsCount = resultCount,
                    Fields = { "Vector" }
                }
            }
        },
        Size = resultCount
    };

    var response = await searchClient.SearchAsync<SearchDocument>(null, searchOptions);

    await foreach (var result in response.Value.GetResultsAsync())
    {
        AnsiConsole.WriteLine($">> Title: {result.Document["Title"]}");
        AnsiConsole.WriteLine($">> Year: {result.Document["Year"]}");
        AnsiConsole.WriteLine($">> Description: {result.Document["Description"]}");
        AnsiConsole.WriteLine($">> Score: {result.Score}");
        AnsiConsole.WriteLine();
    }
    AnsiConsole.WriteLine($"====================================================");
    AnsiConsole.WriteLine();
}

async Task CreateOrUpdateIndexAsync(SearchIndexClient client)
{
    var vectorSearch = new VectorSearch();
    vectorSearch.Algorithms.Add(new HnswAlgorithmConfiguration("hnsw-config"));
    vectorSearch.Profiles.Add(new VectorSearchProfile("vector-profile", "hnsw-config"));

    var index = new SearchIndex(indexName)
    {
        VectorSearch = vectorSearch,
        Fields =
        {
            new SimpleField("Key", SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
            new SearchableField("Title") { IsFilterable = true },
            new SimpleField("Year", SearchFieldDataType.Int32) { IsFilterable = true, IsSortable = true },
            new SearchableField("Category") { IsFilterable = true },
            new SearchableField("Description"),
            new SearchField("Vector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
            {
                IsSearchable = true,
                VectorSearchDimensions = vectorDimensions,
                VectorSearchProfileName = "vector-profile"
            }
        }
    };

    await client.CreateOrUpdateIndexAsync(index);
}

SearchIndexClient GetSearchIndexClient(string azureAISearchUri)
{
    var credential = new DefaultAzureCredential();
    var client = new SearchIndexClient(new Uri(azureAISearchUri), credential);
    return client;
}