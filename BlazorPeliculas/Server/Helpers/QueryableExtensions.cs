using BlazorPeliculas.Shared.DTOs;

namespace BlazorPeliculas.Server.Helpers {
    public static class QueryableExtensions {
        public static IQueryable<T> ToPage<T>(this IQueryable<T> queryable, PaginationDTO pagination) {
            return queryable
                .Skip((pagination.Page - 1) * pagination.RecordCount)
                .Take(pagination.RecordCount);
        }
    }
}
