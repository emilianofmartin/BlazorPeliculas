using AutoMapper;
using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Controllers {
    [Route("api/actors"), ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]

    public class ActorsController: ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IFileSaver fileSaver;
        private readonly IMapper mapper;
        private readonly string container = "people";

        public ActorsController(ApplicationDbContext context, IFileSaver fileSaver, IMapper mapper) {
            this.context = context;
            this.fileSaver = fileSaver;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> Get([FromQuery] PaginationDTO pagination) {
            //return await context.Actors.ToListAsync();
            var queryable = context.Actors.AsQueryable();
            await HttpContext.InserPaginationtParametersInResponse(queryable, pagination.RecordCount);
            return await queryable.OrderBy(x => x.Name).ToPage(pagination).ToListAsync();
        }

        [HttpGet("search/{searchText}")]
        public async Task<ActionResult<IEnumerable<Actor>>> Get(string searchText) {
            if(string.IsNullOrWhiteSpace(searchText)) 
                return new List<Actor>();

            searchText = searchText.ToLower();
            return await context.Actors
                           .Where(x => x.Name.ToLower().Contains(searchText))
                           .Take(5)
                           .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Actor>> Get(int id) {
            var actor = await context.Actors.FirstOrDefaultAsync(actor => actor.ID == id);

            if(actor is null)
                return NotFound();

            return actor;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Actor actor) {
            if(!string.IsNullOrWhiteSpace(actor.Photo)) {
                //Nos mandaron una foto desde el frontend
                var photoActor = Convert.FromBase64String(actor.Photo);
                actor.Photo = await fileSaver.SaveFile(photoActor, ".jpg", container);
            }

            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.ID;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Actor actor) {
            var actorDB = await context.Actors.FirstOrDefaultAsync(a => a.ID == actor.ID);

            if(actorDB is null) 
                return NotFound();
            //Tomá las propiedades de actor y pasalas a actorDB
            actorDB = mapper.Map(actor, actorDB);

            if(!string.IsNullOrWhiteSpace(actor.Photo)) {
                //Nos mandaron una foto desde el frontend
                var photoActor = Convert.FromBase64String(actor.Photo);
                actorDB.Photo = await fileSaver.EditFile(photoActor, ".jpg", container, actorDB.Photo!);
            }

            await context.SaveChangesAsync();   //Se hace el UPDATE
            return NoContent();            //Todo se hizo correctamente
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult<int>> Delete(int id) {
            var actor = await context.Actors.FirstOrDefaultAsync(x => x.ID == id);

            if(actor is null)
                return NotFound();

            context.Remove(actor);      //Marcamos para borrar el actor
            await context.SaveChangesAsync();
            if(!string.IsNullOrWhiteSpace(actor.Photo))
                await fileSaver.DeleteFile(actor.Photo!, container);

            return NoContent();
        }
    }
}
