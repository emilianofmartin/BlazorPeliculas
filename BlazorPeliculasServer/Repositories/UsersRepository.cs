using BlazorPeliculasServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlazorPeliculasServer.DTOs;
using Microsoft.EntityFrameworkCore;
using BlazorPeliculasServer.Helpers;
using System.Security.Claims;

namespace BlazorPeliculasServer.Repositories {
    public class UsersRepository {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public UsersRepository(ApplicationDbContext context,
            UserManager<IdentityUser> userManager) {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<PaginatedResponseDTO<UserDTO>> Get(PaginationDTO pagination) {
            var queryable = context.Users.AsQueryable();
            var response = new PaginatedResponseDTO<UserDTO>();

            response.totalPages = await queryable.CalculateTotalPages(pagination.RecordCount);
            response.Records = await queryable.ToPage(pagination)
                .Select(x => new UserDTO { ID = x.Id, Email = x.Email! })
                .ToListAsync();
            return response;
        }

        public async Task<List<RoleDTO>> GetRoles() {
            return await context.Roles.Select(x => new RoleDTO { Name = x.Name! }).ToListAsync();
        }

        public async Task AssignRoleToUser(EditRolDTO editRolDTO) {
            var user = await userManager.FindByIdAsync(editRolDTO.UserID);
            if(user is null)
                throw new Exception("User was not found!");

            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRolDTO.Role));
            await userManager.AddToRoleAsync(user, editRolDTO.Role);
        }

        public async Task RemovenRoleFromUser(EditRolDTO editRolDTO) {
            var user = await userManager.FindByIdAsync(editRolDTO.UserID);
            if(user is null)
                throw new Exception("User was not found!");

            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRolDTO.Role));
            await userManager.RemoveFromRoleAsync(user, editRolDTO.Role);
        }
    }
}
