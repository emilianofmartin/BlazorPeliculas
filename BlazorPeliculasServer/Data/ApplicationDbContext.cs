using BlazorPeliculasServer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculasServer.Data {
    public class ApplicationDbContext : IdentityDbContext {
        public ApplicationDbContext(DbContextOptions options) : base(options) {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);     //No se puede eliminar esta línea.

            modelBuilder.Entity<GenresMovie>().HasKey(x => new { x.GenreID, x.MovieID });
            modelBuilder.Entity<MovieActor>().HasKey(x => new { x.ActorID, x.MovieID });
        }

        public DbSet<Genre> Genres => Set<Genre>();  //Creamos la tabla Genres a partir de la clase Genre.
        public DbSet<Actor> Actors => Set<Actor>();  //Creamos la tabla Actors a partir de la clase Actor.
        public DbSet<Movie> Movies => Set<Movie>();  //Creamos la tabla Movies a partir de la clase Movie.
        public DbSet<GenresMovie> GenresMovie => Set<GenresMovie>();  //Creamos la tabla GenresMovie a partir de la clase GenresMovie.
        public DbSet<MovieActor> MoviesActors => Set<MovieActor>();  //Creamos la tabla MoviesActors a partir de la clase MovieActor.
        public DbSet<VoteMovie> VotesMovies => Set<VoteMovie>();  //Creamos la tabla VotesMovies a partir de la clase VoteMovie.
    }
}
