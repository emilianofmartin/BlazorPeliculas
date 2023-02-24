using AutoMapper;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Controllers {
    [ApiController, Route("api/votes"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class VotesController: ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public VotesController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IMapper mapper) {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Vote(VoteMovieDTO voteMovieDTO) {
            //Tomo el nombre (email) del usuario y con eso busco y obtengo el registro de dicho usuario.
            var user = await userManager.FindByEmailAsync(HttpContext.User.Identity!.Name!);

            if(user is null) {
                //No debería pasar nunca, pero por las dudas...
                return BadRequest("User was not found");
            }

            var userID = user.Id;

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
            return NoContent();
        }
    }
}
