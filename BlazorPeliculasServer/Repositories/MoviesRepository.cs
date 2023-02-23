using AutoMapper;
using BlazorPeliculasServer.Data;
using BlazorPeliculasServer.DTOs;
using BlazorPeliculasServer.Entities;
using BlazorPeliculasServer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BlazorPeliculasServer.Repositories {
    public class MoviesRepository {
        private readonly ApplicationDbContext context;
        private readonly IFileSaver fileSaver;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly AuthenticationStateService authStateService;
        private readonly string container = "movies";

        public MoviesRepository(ApplicationDbContext context,
            IFileSaver fileSaver,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            AuthenticationStateService authStateService) {
            this.context = context;
            this.fileSaver = fileSaver;
            this.mapper = mapper;
            this.userManager = userManager;
            this.authStateService = authStateService;
        }

        public async Task<HomePageDTO> Get() {
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

        public async Task<MovieViewDTO> Get(int id) {
            var movie = await context.Movies
               .Where(movie => movie.ID == id)
               .Include(movie => movie.GenresMovie)
                .ThenInclude(gm => gm.Genre)
               .Include(movie => movie.MovieActor.OrderBy(ma => ma.Orden))
                .ThenInclude(ma => ma.Actor)
               .FirstOrDefaultAsync();

            if(movie is null) {
                //No se encontró la película
                return null;
            }

            var votesMedia = 0.0;
            var userVote = 0;

            if(await context.VotesMovies.AnyAsync(x => x.MovieID == id)) {
                //Alguien ha votado por la película
                votesMedia = await context.VotesMovies
                    .Where(x => x.MovieID == id)
                    .AverageAsync(x => x.Voto);

                var userID = await authStateService.GetCurrentUserID();

                if(userID != null) {
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

        public async Task<PaginatedResponseDTO<Movie>> Get(SearchMoviesParametersDTO model) {
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

            if(model.GenreID != 0)
                queryableMovies = queryableMovies
                    .Where(x => x.GenresMovie
                        .Select(y => y.GenreID)
                        .Contains(model.GenreID));

            if(model.MostVoted) {
                queryableMovies = queryableMovies.OrderByDescending(m => m.VotesMovies.Average(vm => vm.Voto));
            }

            var paginatedResponse = new PaginatedResponseDTO<Movie>();
            paginatedResponse.totalPages = await queryableMovies.CalculateTotalPages(model.RecordCount);

            //Recién acá materealizo el query y lo ejecuto en la BD -> ejecución diferida
            paginatedResponse.Records = await queryableMovies.ToPage(model.pagination).ToListAsync();
            return paginatedResponse;
        }

        public async Task<MovieUpdateDTO> PutGet(int id) {
            //Re-utilizamos el GET para traer el ActionResult con la info de la película.
            var movieActionResult = await Get(id);

            if(movieActionResult == null) return null;

            var movieViewDTO = movieActionResult;        //Será el DTO
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

        public async Task<int> Post(Movie movie) {
            if(!string.IsNullOrWhiteSpace(movie.Poster)) {
                var poster = Convert.FromBase64String(movie.Poster);
                movie.Poster = await fileSaver.SaveFile(poster, ".jpg", container);
            }

            WriteActorsOrder(movie);

            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.ID;
        }

        private static void WriteActorsOrder(Movie movie) {
            if(movie.MovieActor is not null) {
                for(int i = 0; i < movie.MovieActor.Count; i++) {
                    movie.MovieActor[i].Orden = i + 1;
                }
            }
        }

        public async Task Put(Movie movie) {
            var movieDB = await context.Movies
                .Include(x => x.GenresMovie)
                .Include(x => x.MovieActor)
                .FirstOrDefaultAsync(x => x.ID == movie.ID);

            if(movieDB is null) throw new ApplicationException($"Movie {movie.ID} was not found!");

            //Tomá las propiedades de movie y pasalas a movieDB
            movieDB = mapper.Map(movie, movieDB);

            if(!string.IsNullOrWhiteSpace(movie.Poster)) {
                //Nos mandaron una foto desde el frontend
                var Poster = Convert.FromBase64String(movie.Poster);
                movieDB.Poster = await fileSaver.EditFile(Poster, ".jpg", container, movieDB.Poster!);
            }

            WriteActorsOrder(movieDB);

            await context.SaveChangesAsync();   //Se hace el UPDATE
        }


        public async Task Delete(int id) {
            var movie = await context.Movies.FirstOrDefaultAsync(x => x.ID == id);

            if(movie is null) throw new ApplicationException($"Movie {id} was not found!");

            context.Remove(movie);      //Marcamos para borrar el actor
            await context.SaveChangesAsync();
            if(!string.IsNullOrWhiteSpace(movie.Poster))
                await fileSaver.DeleteFile(movie.Poster!, container);

        }
    }
}
