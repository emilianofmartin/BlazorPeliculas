using BlazorPeliculasServer.Data;
using BlazorPeliculasServer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculasServer.Repositories {
    public class GenresRepository {
        private readonly ApplicationDbContext context;
        public GenresRepository(ApplicationDbContext context) {
            this.context = context;
        }

        public async Task<List<Genre>> Get() {
            return await context.Genres.ToListAsync();
        }


        public async Task<Genre> Get(int id) {
            var genre = await context.Genres.FirstOrDefaultAsync(genre => genre.ID == id);

            if(genre is null)
                throw new ApplicationException($" Genre {id} was not found");

            return genre;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Genre genre) {
            context.Add(genre);
            await context.SaveChangesAsync();
            return genre.ID;
        }

        public async Task Put(Genre genre) {
            context.Update(genre);     //Marco el género para ser actualizado.
            await context.SaveChangesAsync();   //Se hace el UPDATE
        }

        public async Task Delete(int id) {
            var affectedFiles = await context.Genres
                .Where(x => x.ID == id)
                .ExecuteDeleteAsync();

            if(affectedFiles == 0)
                throw new ApplicationException($" Genre {id} was not found");

        }
    }
}
