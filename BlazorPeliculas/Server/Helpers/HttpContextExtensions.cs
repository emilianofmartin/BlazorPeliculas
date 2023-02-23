using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Helpers {
    public static class HttpContextExtensions {
        public async static Task InserPaginationtParametersInResponse<T>(this HttpContext context, IQueryable<T> queryable, int amountRecordsToShow) {
            if(context is null)
                throw new ArgumentNullException(nameof(context));

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / amountRecordsToShow);

            context.Response.Headers.Add("count", count.ToString());
            context.Response.Headers.Add("totalPages", totalPages.ToString());
        }
    }
}
