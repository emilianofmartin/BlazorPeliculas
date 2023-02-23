using AutoMapper;
using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace BlazorPeliculas.Server.Controllers {
    [Route("api/movies"), ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class MoviesController : ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IFileSaver fileSaver;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly string container = "movies";

        public MoviesController(ApplicationDbContext context,
            IFileSaver fileSaver,
            IMapper mapper,
            UserManager<IdentityUser> userManager) {
            this.context = context;
            this.fileSaver = fileSaver;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<HomePageDTO>> Get() {
            var limit = 6;
            var onBoardMovies = await context.Movies
                .Where(movie => movie.OnBillboard)
                .Take(limit)
                .OrderByDescending(movie => movie.ReleaseDate)
                .ToListAsync();
            var today = DateTime.Today;
            var nextReleases = await context.Movies
                .Where(movie => movie.ReleaseDate > today)
                .Take(limit)
                .OrderBy(movie => movie.ReleaseDate)
                .ToListAsync();

            var result = new HomePageDTO {
                OnBoard = onBoardMovies,
                NextReleases = nextReleases
            };
            return result;
        }

        [HttpGet("{id:int}"), AllowAnonymous]
        public async Task<ActionResult<MovieViewDTO>> Get(int id) {
            var movie = await context.Movies
               .Where(movie => movie.ID == id)
               .Include(movie => movie.GenresMovie)
                .ThenInclude(gm => gm.Genre)
               .Include(movie => movie.MovieActor.OrderBy(ma => ma.Orden))
                .ThenInclude(ma => ma.Actor)
               .FirstOrDefaultAsync();

            if (movie is null) {
                //No se encontró la película
                return NotFound();
            }

            var votesMedia = 0.0;
            var userVote = 0;

            if(await context.VotesMovies.AnyAsync(x => x.MovieID == id)) {
                //Alguien ha votado por la película
                votesMedia = await context.VotesMovies
                    .Where(x => x.MovieID == id)
                    .AverageAsync(x => x.Voto);

                if(HttpContext.User.Identity!.IsAuthenticated) {
                    var user = await userManager.FindByEmailAsync(HttpContext.User.Identity!.Name!);

                    if(user is null) {
                        //No debería pasar nunca, pero por las dudas...
                        return BadRequest("User was not found");
                    }

                    var userID = user.Id;

                    var userVoteDB = await context.VotesMovies
                        .FirstOrDefaultAsync(x => x.MovieID == id && x.UserID == userID);

                    if(userVoteDB is not null)
                        userVote = userVoteDB.Voto;
                }
            }

            var model = new MovieViewDTO();
            model.Movie = movie;
            model.Genres = movie.GenresMovie.Select(gm => gm.Genre!).ToList();
            model.Actors = movie.MovieActor.Select(ma => new Actor {
                Name = ma.Actor!.Name,
                Photo = ma.Actor.Photo,
                Character = ma.Character,
                ID = ma.Actor.ID
            }).ToList();

            model.VotesMedia = votesMedia;
            model.UserVote = userVote;

            return model;
        }

        [HttpGet("filter"), AllowAnonymous]
        public async Task<ActionResult<List<Movie>>> Get([FromQuery] SearchMoviesParametersDTO model) {
            var queryableMovies = context.Movies.AsQueryable();

            if(!string.IsNullOrWhiteSpace(model.Title))
                queryableMovies = queryableMovies
                    .Where(x => x.Title.Contains(model.Title));

            if(model.Onbillboard)
                queryableMovies = queryableMovies
                    .Where(x => x.OnBillboard);

            if(model.Releases) {
                var today = DateTime.Today;

                queryableMovies = queryableMovies
                    .Where(x => x.ReleaseDate >= today);
            }

            if (model.GenreID != 0)
                queryableMovies = queryableMovies
                    .Where(x => x.GenresMovie
                        .Select(y => y.GenreID)
                        .Contains(model.GenreID));

            if(model.MostVoted) {
                queryableMovies = queryableMovies.OrderByDescending(m => m.VotesMovies.Average(vm => vm.Voto));
            }

            await HttpContext.InserPaginationtParametersInResponse(queryableMovies, model.RecordCount);

            //Recién acá materealizo el query y lo ejecuto en la BD -> ejecución diferida
            var movies = await queryableMovies.ToPage(model.pagination).ToListAsync();
            return movies;
        }

        [HttpGet("edit/{id:int}")]
        public async Task<ActionResult<MovieUpdateDTO>> PutGet(int id) {
            //Re-utilizamos el GET para traer el ActionResult con la info de la película.
            var movieActionResult = await Get(id);

            if (movieActionResult.Result is NotFoundResult)
                return NotFound();

            var movieViewDTO = movieActionResult.Value;        //Será el DTO
            var selectedGenresIDs = movieViewDTO!.Genres.Select(x => x.ID).ToList();
            var unselectedGenres = await context.Genres
                .Where(x => !selectedGenresIDs.Contains(x.ID))
                .ToListAsync();

            var model = new MovieUpdateDTO();
            model.Movie = movieViewDTO.Movie;
            model.UnselectedGenres = unselectedGenres;
            model.SelectedGenres = movieViewDTO.Genres;
            model.Actors = movieViewDTO.Actors;

            return model;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Movie movie) {
            if (!string.IsNullOrWhiteSpace(movie.Poster)) {
                var poster = Convert.FromBase64String(movie.Poster);
                movie.Poster = await fileSaver.SaveFile(poster, ".jpg", container);
            }

            WriteActorsOrder(movie);

            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.ID;
        }

        private static void WriteActorsOrder(Movie movie) {
            if (movie.MovieActor is not null) {
                for (int i = 0; i < movie.MovieActor.Count; i++) {
                    movie.MovieActor[i].Orden = i + 1;
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Movie movie) {
            var movieDB = await context.Movies
                .Include(x => x.GenresMovie)
                .Include(x => x.MovieActor)
                .FirstOrDefaultAsync(x => x.ID == movie.ID);

            if (movieDB is null)
                return NotFound();

            //Tomá las propiedades de movie y pasalas a movieDB
            movieDB = mapper.Map(movie, movieDB);

            if (!string.IsNullOrWhiteSpace(movie.Poster)) {
                //Nos mandaron una foto desde el frontend
                var Poster = Convert.FromBase64String(movie.Poster);
                movieDB.Poster = await fileSaver.EditFile(Poster, ".jpg", container, movieDB.Poster!);
            }

            WriteActorsOrder(movieDB);

            await context.SaveChangesAsync();   //Se hace el UPDATE
            return NoContent();            //Todo se hizo correctamente
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult<int>> Delete(int id) {
            var movie = await context.Movies.FirstOrDefaultAsync(x => x.ID == id);

            if (movie is null)
                return NotFound();

            context.Remove(movie);      //Marcamos para borrar el actor
            await context.SaveChangesAsync();
            if (!string.IsNullOrWhiteSpace(movie.Poster))
                await fileSaver.DeleteFile(movie.Poster!, container);

            return NoContent();            //Todo se hizo correctamente
        }
    }
}