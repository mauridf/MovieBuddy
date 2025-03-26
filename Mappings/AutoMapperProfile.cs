using AutoMapper;
using MovieBuddy.DTOs.External;
using MovieBuddy.DTOs;
using MovieBuddy.Models;

namespace MovieBuddy.Mappings
{
    using AutoMapper;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Mapeamento para pesquisa
            CreateMap<TheMovieDbSearchResult, MovieDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.MediaType == "tv" ? src.Name : src.Title))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.MediaType == "tv" ? src.FirstAirDate : src.ReleaseDate))
                .ForMember(dest => dest.IsTvShow, opt => opt.MapFrom(src => src.MediaType == "tv"))
                .ForMember(dest => dest.Genres, opt => opt.Ignore());

            // Mapeamento para detalhes
            CreateMap<TheMovieDbMovieDetails, MovieDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name ?? src.Title))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.FirstAirDate ?? src.ReleaseDate))
                .ForMember(dest => dest.IsTvShow, opt => opt.MapFrom(src => src.Name != null))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.Name).ToList()));

            // Mapeamento para gêneros
            CreateMap<TheMovieDbGenre, GenreDto>();

            // Mapeamento para usuário
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.GenrePreferences,
                           opt => opt.MapFrom(src => src.Preferences.Select(p => p.GenreId).ToList()));
        }
    }
}
