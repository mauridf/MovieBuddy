using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBuddy.Data;
using MovieBuddy.DTOs;
using MovieBuddy.Mappings;
using MovieBuddy.Models;
using MovieBuddy.Service;

namespace MovieBuddy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITheMovieDbService _movieDbService;
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext context, ITheMovieDbService movieDbService, IMapper mapper)
        {
            _context = context;
            _movieDbService = movieDbService;
            _mapper = mapper;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetUserByName(string name)
        {
            var user = await _context.Users
                .Include(u => u.Preferences)
                .FirstOrDefaultAsync(u => u.Name.ToLower() == name.ToLower());

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            try
            {
                // Validação básica
                if (string.IsNullOrWhiteSpace(userDto.Name))
                {
                    return BadRequest(new { message = "O nome do usuário é obrigatório." });
                }

                // Verifica se o usuário já existe
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Name.ToLower() == userDto.Name.ToLower());

                if (existingUser != null)
                {
                    return Conflict(new { message = "Já existe um usuário com este nome." });
                }

                // Valida os gêneros
                if (userDto.GenreIds != null && userDto.GenreIds.Any())
                {
                    var allGenres = await _movieDbService.GetAllGenresAsync();
                    var invalidGenres = userDto.GenreIds.Except(allGenres.Select(g => g.Id));

                    if (invalidGenres.Any())
                    {
                        return BadRequest(new
                        {
                            message = $"IDs de gênero inválidos: {string.Join(", ", invalidGenres)}"
                        });
                    }
                }

                // Cria o novo usuário
                var user = new User
                {
                    Name = userDto.Name.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                // Adiciona as preferências se houver
                if (userDto.GenreIds != null && userDto.GenreIds.Any())
                {
                    foreach (var genreId in userDto.GenreIds)
                    {
                        _context.UserPreferences.Add(new UserPreference
                        {
                            UserId = user.Id,
                            GenreId = genreId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                // Retorna o usuário criado
                var createdUser = await _context.Users
                    .Include(u => u.Preferences)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                var response = new ApiResponseDto<UserResponseDto>
                {
                    Success = true,
                    Message = "Usuário criado com sucesso.",
                    Data = _mapper.Map<UserResponseDto>(createdUser)
                };

                return CreatedAtAction(nameof(GetUserByName), new { name = user.Name }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<UserResponseDto>
                {
                    Success = false,
                    Message = $"Erro ao criar usuário: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}/preferences")]
        public async Task<IActionResult> UpdatePreferences(int id, [FromBody] UserPreferenceDto preferencesDto)
        {
            try
            {
                // Validação básica
                if (preferencesDto == null || preferencesDto.GenreIds == null)
                {
                    return BadRequest(new { message = "Dados de preferências inválidos." });
                }

                // Verifica se o usuário existe
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado." });
                }

                // Valida os gêneros
                var allGenres = await _movieDbService.GetAllGenresAsync();
                var invalidGenres = preferencesDto.GenreIds.Except(allGenres.Select(g => g.Id));

                if (invalidGenres.Any())
                {
                    return BadRequest(new
                    {
                        message = $"IDs de gênero inválidos: {string.Join(", ", invalidGenres)}"
                    });
                }

                // Remove preferências existentes
                var existingPreferences = _context.UserPreferences
                    .Where(up => up.UserId == id);
                _context.UserPreferences.RemoveRange(existingPreferences);

                // Adiciona as novas preferências
                foreach (var genreId in preferencesDto.GenreIds)
                {
                    _context.UserPreferences.Add(new UserPreference
                    {
                        UserId = id,
                        GenreId = genreId
                    });
                }

                await _context.SaveChangesAsync();

                // Retorna o usuário atualizado
                var updatedUser = await _context.Users
                    .Include(u => u.Preferences)
                    .FirstOrDefaultAsync(u => u.Id == id);

                var response = new ApiResponseDto<UserResponseDto>
                {
                    Success = true,
                    Message = "Preferências atualizadas com sucesso.",
                    Data = _mapper.Map<UserResponseDto>(updatedUser)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<UserResponseDto>
                {
                    Success = false,
                    Message = $"Erro ao atualizar preferências: {ex.Message}"
                });
            }
        }

        [HttpPost("{userId}/ratings")]
        public async Task<IActionResult> AddRating(int userId, [FromBody] RatingDto ratingDto)
        {
            if (ratingDto.UserId != userId)
                return BadRequest("User ID mismatch");

            var existingRating = await _context.UserRatings
                .FirstOrDefaultAsync(r => r.UserId == userId &&
                                        r.MovieId == ratingDto.MovieId &&
                                        r.IsTvShow == ratingDto.IsTvShow);

            if (existingRating != null)
            {
                existingRating.Rating = ratingDto.Rating;
                existingRating.RatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.UserRatings.Add(new UserRating
                {
                    UserId = userId,
                    MovieId = ratingDto.MovieId,
                    IsTvShow = ratingDto.IsTvShow,
                    Rating = ratingDto.Rating,
                    RatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{userId}/ratings")]
        public async Task<IActionResult> GetUserRatings(int userId)
        {
            try
            {
                // Verifica se o usuário existe
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    return NotFound(new ApiResponseDto<List<RatingDto>>
                    {
                        Success = false,
                        Message = "Usuário não encontrado."
                    });
                }

                // Busca as avaliações do usuário
                var ratings = await _context.UserRatings
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.RatedAt) // Ordena por data mais recente
                    .Select(r => new RatingDto
                    {
                        UserId = r.UserId,
                        MovieId = r.MovieId,
                        IsTvShow = r.IsTvShow,
                        Rating = r.Rating
                    })
                    .ToListAsync();

                // Se não houver avaliações, retorna array vazio
                if (ratings == null || !ratings.Any())
                {
                    return Ok(new ApiResponseDto<List<RatingDto>>
                    {
                        Success = true,
                        Message = "Nenhuma avaliação encontrada para este usuário.",
                        Data = new List<RatingDto>()
                    });
                }

                // Retorna as avaliações encontradas
                return Ok(new ApiResponseDto<List<RatingDto>>
                {
                    Success = true,
                    Data = ratings
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<List<RatingDto>>
                {
                    Success = false,
                    Message = $"Erro ao buscar avaliações: {ex.Message}"
                });
            }
        }
    }
}
