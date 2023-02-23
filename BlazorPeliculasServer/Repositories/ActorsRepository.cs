using AutoMapper;
using BlazorPeliculasServer.Data;
using BlazorPeliculasServer.DTOs;
using BlazorPeliculasServer.Entities;
using BlazorPeliculasServer.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculasServer.Repositories {
    public class ActorsRepository {
        private readonly ApplicationDbContext context;
        private readonly IFileSaver fileSaver;
        private readonly IMapper mapper;
        private readonly string container = "people";

        public ActorsRepository(ApplicationDbContext context, IFileSaver fileSaver, IMapper mapper) {
            this.context = context;
            this.fileSaver = fileSaver;
            this.mapper = mapper;
        }

        public async Task<PaginatedResponseDTO<Actor>> Get(PaginationDTO pagination) {
            //return await context.Actors.ToListAsync();
            var queryable = context.Actors.AsQueryable();
            var paginatedResponse = new PaginatedResponseDTO<Actor>();
            paginatedResponse.totalPages = await  queryable.CalculateTotalPages(pagination.RecordCount);
            paginatedResponse.Records = await queryable.OrderBy(x => x.Name).ToPage(pagination).ToListAsync();

            return paginatedResponse;
        }

        public async Task<List<Actor>> Get(string searchText) {
            if(string.IsNullOrWhiteSpace(searchText))
                return new List<Actor>();

            searchText = searchText.ToLower();
            return await context.Actors
                           .Where(x => x.Name.ToLower().Contains(searchText))
                           .Take(5)
                           .ToListAsync();
        }

        public async Task<Actor> Get(int id) {
            return await context.Actors.FirstOrDefaultAsync(actor => actor.ID == id);       //Si no lo encuentra, devuelve NULL
        }

        public async Task<int> Post(Actor actor) {
            if(!string.IsNullOrWhiteSpace(actor.Photo)) {
                //Nos mandaron una foto desde el frontend
                var photoActor = Convert.FromBase64String(actor.Photo);
                actor.Photo = await fileSaver.SaveFile(photoActor, ".jpg", container);
            }

            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.ID;
        }

        public async Task Put(Actor actor) {
            var actorDB = await context.Actors.FirstOrDefaultAsync(a => a.ID == actor.ID);

            if(actorDB is null)
                throw new ApplicationException($"Actor {actor.ID} was not found!");

            //Tomá las propiedades de actor y pasalas a actorDB
            actorDB = mapper.Map(actor, actorDB);

            if(!string.IsNullOrWhiteSpace(actor.Photo)) {
                //Nos mandaron una foto desde el frontend
                var photoActor = Convert.FromBase64String(actor.Photo);
                actorDB.Photo = await fileSaver.EditFile(photoActor, ".jpg", container, actorDB.Photo!);
            }

            await context.SaveChangesAsync();   //Se hace el UPDATE
        }


        public async Task Delete(int id) {
            var actor = await context.Actors.FirstOrDefaultAsync(x => x.ID == id);

            if(actor is null)
                throw new ApplicationException($"Actor {id} was not found!");

            context.Remove(actor);      //Marcamos para borrar el actor
            await context.SaveChangesAsync();
            if(!string.IsNullOrWhiteSpace(actor.Photo))
                await fileSaver.DeleteFile(actor.Photo!, container);
        }
    }
}
