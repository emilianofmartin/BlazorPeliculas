using BlazorPeliculas.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using BlazorPeliculas.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BlazorPeliculas.Server.Controllers {
    [Route("api/genres"), ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class GenresController : ControllerBase {
        private readonly ApplicationDbContext context;
        public GenresController(ApplicationDbContext context) {
            this.context = context;
        }

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Genre>>> Get() {
            return await context.Genres.ToListAsync();
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genre>> Get(int id) {
            var genre = await context.Genres.FirstOrDefaultAsync(genre => genre.ID == id);

            if (genre is null)
                return NotFound();

            return genre;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Genre genre) {
            context.Add(genre);
            await context.SaveChangesAsync();
            return genre.ID;
        }

        [HttpPut]
        public async Task<ActionResult<int>> Put(Genre genre) {
            context.Update(genre);     //Marco el género para ser actualizado.
            await context.SaveChangesAsync();   //Se hace el UPDATE
            return NoContent();            //Todo se hizo correctamente
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<int>> Delete(int id) {
            var affectedFiles = await context.Genres
                .Where(x => x.ID == id)
                .ExecuteDeleteAsync();

            if(affectedFiles == 0)
                return NotFound();

            return NoContent();
        }
    }
}
