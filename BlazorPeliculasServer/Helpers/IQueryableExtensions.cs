using BlazorPeliculasServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculasServer.Helpers {
    public static class IQueryableExtensions {
        public async static Task<int> CalculateTotalPages<T> (this IQueryable<T> queryable, int recordsToShowCount) {
            double account = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(account / recordsToShowCount);
            return totalPages;
        }

        public static IQueryable<T> ToPage<T>(this IQueryable<T> queryable, PaginationDTO pagination) {
            return queryable
                .Skip((pagination.Page - 1) * pagination.RecordCount)
                .Take(pagination.RecordCount);
        }
    }
}
