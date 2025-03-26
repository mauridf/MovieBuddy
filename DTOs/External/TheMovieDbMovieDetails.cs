namespace MovieBuddy.DTOs.External
{
    public class TheMovieDbMovieDetails
    {
        public int Id { get; set; }
        public string Title { get; set; } // For movies
        public string Name { get; set; } // For TV shows
        public string Overview { get; set; }
        public string ReleaseDate { get; set; } // For movies
        public string FirstAirDate { get; set; } // For TV shows
        public int? Runtime { get; set; } // For movies
        public List<TheMovieDbGenre> Genres { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }
        public TheMovieDbVideos Videos { get; set; }
        public TheMovieDbWatchProviders WatchProviders { get; set; }
    }

    public class TheMovieDbGenre
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TheMovieDbVideos
    {
        public List<TheMovieDbVideo> Results { get; set; }
    }

    public class TheMovieDbVideo
    {
        public string Key { get; set; }
        public string Site { get; set; }
        public string Type { get; set; }
    }

    public class TheMovieDbWatchProviders
    {
        public Dictionary<string, TheMovieDbWatchProviderCountry> Results { get; set; }
    }

    public class TheMovieDbWatchProviderCountry
    {
        public string Link { get; set; }
        public List<TheMovieDbWatchProviderInfo> Flatrate { get; set; }
    }

    public class TheMovieDbWatchProviderInfo
    {
        public string ProviderName { get; set; }
        public string LogoPath { get; set; }
    }
}
