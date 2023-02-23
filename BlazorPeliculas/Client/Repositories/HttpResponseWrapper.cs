using System.Net;

namespace BlazorPeliculas.Client.Repositories {
    public class HttpResponseWrapper<T> {
        public HttpResponseWrapper(T? response, bool Error, HttpResponseMessage httpResponseMessage) {
            Response = response;
            this.Error = Error;
            this.httpResponseMessage = httpResponseMessage;
        }
        public bool Error { get; set; }
        public T? Response { get; set; }
        public HttpResponseMessage httpResponseMessage { get; set; }

        public async Task<string?> GetErrMessage() {
            if (!Error)
                return null;

            var codStatus = httpResponseMessage.StatusCode;

            if (codStatus == HttpStatusCode.NotFound)
                return "Resource not found";
            else if (codStatus == HttpStatusCode.BadRequest)
                return await httpResponseMessage.Content.ReadAsStringAsync();
            else if (codStatus == HttpStatusCode.Unauthorized)
                return "You need to be logged to do this";
            else if (codStatus == HttpStatusCode.Forbidden)
                return "You don't own the grants needed to perfom this";
            else
                return "An unexpected error has ocurred";
        }
    }
}
