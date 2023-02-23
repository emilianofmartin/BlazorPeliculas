using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Controllers {
    [ApiController, Route("api/users"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class UsersController: ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public UsersController(ApplicationDbContext context, 
            UserManager<IdentityUser> userManager) {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO pagination) {
            var queryable = context.Users.AsQueryable();
            await HttpContext.InserPaginationtParametersInResponse(queryable,
                pagination.RecordCount);
            return await queryable.ToPage(pagination)
                .Select(x => new UserDTO { ID = x.Id, Email = x.Email! })
                .ToListAsync();
        }

        [HttpGet("roles")]
        public async Task<ActionResult<List<RoleDTO>>> Get() {
            return await context.Roles.Select(x => new RoleDTO { Name = x.Name }).ToListAsync();
        }

        [HttpPost("assignRole")]
        public async Task<ActionResult> AssignRoleToUser(EditRolDTO editRolDTO) {
            var user = await userManager.FindByIdAsync(editRolDTO.UserID);
            if(user is null)
                return BadRequest("User was not found!");

            await userManager.AddToRoleAsync(user, editRolDTO.Role);
            return NoContent();
        }

        [HttpPost("removeRole")]
        public async Task<ActionResult> RemovenRoleFromUser(EditRolDTO editRolDTO) {
            var user = await userManager.FindByIdAsync(editRolDTO.UserID);
            if(user is null)
                return BadRequest("User was not found!");

            await userManager.RemoveFromRoleAsync(user, editRolDTO.Role);
            return NoContent();
        }
    }
}
