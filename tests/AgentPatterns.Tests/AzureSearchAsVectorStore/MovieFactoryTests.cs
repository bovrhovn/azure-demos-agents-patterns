using AP.AzureSearchAsVectorStore;

namespace AgentPatterns.Tests.AzureSearchAsVectorStore;

public class MovieFactoryTests
{
    [Fact]
    public void GetMovieVectorList_ReturnsFourMovies()
    {
        var movies = MovieFactory.GetMovieVectorList();
        Assert.Equal(4, movies.Count);
    }

    [Fact]
    public void GetMovieVectorList_AllMoviesHaveKeys()
    {
        var movies = MovieFactory.GetMovieVectorList();
        Assert.All(movies, m => Assert.False(string.IsNullOrEmpty(m.Key)));
    }

    [Fact]
    public void GetMovieVectorList_AllMoviesHaveTitles()
    {
        var movies = MovieFactory.GetMovieVectorList();
        Assert.All(movies, m => Assert.False(string.IsNullOrEmpty(m.Title)));
    }

    [Fact]
    public void GetMovieVectorList_AllMoviesHaveCategories()
    {
        var movies = MovieFactory.GetMovieVectorList();
        Assert.All(movies, m => Assert.False(string.IsNullOrEmpty(m.Category)));
    }

    [Fact]
    public void GetMovieVectorList_AllMoviesHaveDescriptions()
    {
        var movies = MovieFactory.GetMovieVectorList();
        Assert.All(movies, m => Assert.False(string.IsNullOrEmpty(m.Description)));
    }

    [Fact]
    public void GetMovieVectorList_AllMoviesHavePositiveYear()
    {
        var movies = MovieFactory.GetMovieVectorList();
        Assert.All(movies, m => Assert.True(m.Year > 0));
    }

    [Fact]
    public void GetMovieVectorList_AllKeysAreUnique()
    {
        var movies = MovieFactory.GetMovieVectorList();
        var keys = movies.Select(m => m.Key).ToList();
        Assert.Equal(keys.Count, keys.Distinct().Count());
    }

    [Fact]
    public void GetMovieVectorList_ContainsShawshankRedemption()
    {
        var movies = MovieFactory.GetMovieVectorList();
        Assert.Contains(movies, m => m.Title.Contains("Shawshank", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void GetMovieVectorList_ContainsBackToTheFuture()
    {
        var movies = MovieFactory.GetMovieVectorList();
        Assert.Contains(movies, m => m.Title.Contains("future", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void GetMovieVectorList_ContainsTerminator()
    {
        var movies = MovieFactory.GetMovieVectorList();
        Assert.Contains(movies, m => m.Title.Contains("Terminator", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void GetMovieVectorList_ReturnsDifferentCallEachTime()
    {
        var first = MovieFactory.GetMovieVectorList();
        var second = MovieFactory.GetMovieVectorList();
        // Independent lists (not same reference)
        Assert.NotSame(first, second);
        Assert.Equal(first.Count, second.Count);
    }

    [Fact]
    public void GetMovieVectorList_VectorsDefaultToEmpty()
    {
        var movies = MovieFactory.GetMovieVectorList();
        // Vectors are not pre-populated; they default to empty ReadOnlyMemory<float>
        Assert.All(movies, m => Assert.Equal(0, m.Vector.Length));
    }
}

public class MovieTests
{
    [Fact]
    public void Movie_CanBeConstructedWithRequiredProperties()
    {
        var movie = new Movie
        {
            Key = "99",
            Title = "Test Movie",
            Category = "Test",
            Year = 2024
        };

        Assert.Equal("99", movie.Key);
        Assert.Equal("Test Movie", movie.Title);
        Assert.Equal("Test", movie.Category);
        Assert.Equal(2024, movie.Year);
    }

    [Fact]
    public void Movie_DescriptionDefaultsToEmptyString()
    {
        var movie = new Movie { Key = "1", Title = "Title", Category = "Cat" };
        Assert.Equal(string.Empty, movie.Description);
    }

    [Fact]
    public void Movie_VectorDefaultsToEmpty()
    {
        var movie = new Movie { Key = "1", Title = "Title", Category = "Cat" };
        Assert.Equal(0, movie.Vector.Length);
    }

    [Fact]
    public void Movie_CanSetDescription()
    {
        var movie = new Movie { Key = "1", Title = "Title", Category = "Cat", Description = "A great film." };
        Assert.Equal("A great film.", movie.Description);
    }

    [Fact]
    public void Movie_CanSetVector()
    {
        var vector = new float[] { 0.1f, 0.2f, 0.3f };
        var movie = new Movie { Key = "1", Title = "Title", Category = "Cat" };
        movie.Vector = new ReadOnlyMemory<float>(vector);
        Assert.Equal(3, movie.Vector.Length);
    }
}
