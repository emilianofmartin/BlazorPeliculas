using AutoMapper;
using BlazorPeliculasServer.Data;
using BlazorPeliculasServer.DTOs;
using BlazorPeliculasServer.Entities;
using BlazorPeliculasServer.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculasServer.Repositories {
    public class VotesRepository {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;
        private readonly AuthenticationStateService authenticationStateService;

        public VotesRepository(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IMapper mapper,
            AuthenticationStateService authenticationStateService) {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
            this.authenticationStateService = authenticationStateService;
        }

        public async Task Vote(VoteMovieDTO voteMovieDTO) {
            //Tomo el nombre (email) del usuario y con eso busco y obtengo el registro de dicho usuario.
            var userID = await authenticationStateService.GetCurrentUserID();

            if(userID is null) {
                //No debería pasar nunca, pero por las dudas...
                return;
            }

            var currentVote = await context.VotesMovies
                .FirstOrDefaultAsync(x => x.MovieID == voteMovieDTO.MovieID && x.UserID == userID);

            if(currentVote is null) {
                //Aún no voto. Crear uno nuevo.
                var voteMovie = mapper.Map<VoteMovie>(voteMovieDTO);
                voteMovie.UserID = userID;
                voteMovie.VoteWhen = DateTime.Now;
                context.Add(voteMovie);     //Marcar para grabar
            }
            else {
                //Ya votó. Actualizar la fecha y el voto
                currentVote.VoteWhen = DateTime.Now;
                currentVote.Voto = voteMovieDTO.Voto;
            }

            await context.SaveChangesAsync();
        }
    }
}
