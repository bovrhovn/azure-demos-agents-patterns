namespace AP.AzureSearchAsVectorStore;

public class MovieFactory
{
    public static List<Movie> GetMovieVectorList()
    {
        return
        [
            new Movie
            {
                Key = "1",
                Title = "The Shawshank Redemption",
                Year = 1994,
                Category = "Drama",
                Description =
                    "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency."
            },

            new Movie
            {
                Key = "2",
                Title = "Dracula: A Love Tale",
                Year = 2025,
                Category = "Horror",
                Description =
                    "When a 15th-century prince denounces God after the loss of his wife he inherits an eternal curse: he becomes Dracula. Condemned to wander the centuries, he defies fate and death, guided by a single hope - to be reunited with his lost love.."
            },

            new Movie
            {
                Key = "3",
                Title = "Back to the future",
                Year = 1985,
                Category = "Teen Comedy",
                Description =
                    "Marty McFly, a 17-year-old high school student, is accidentally sent 30 years into the past in a time-traveling DeLorean invented by his close friend, the maverick scientist Doc Brown."
            },

            new Movie
            {
                Key = "4",
                Title = "Terminator",
                Year = 1984,
                Category = "Action",
                Description =
                    "A cyborg assassin from the future attempts to find and kill a young woman who is destined to give birth to a warrior that will lead a resistance to save humankind from extinction."
            }

        ];
    }
}

public class Movie
{
    public required string Key { get; set; }
    public required   string Title { get; set; }
    public int Year { get; set; }
    public required string Category { get; set; }
    public string Description { get; set; } = "";
    public ReadOnlyMemory<float> Vector { get; set; }
}