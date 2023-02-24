using BlazorPeliculas.Shared.Entities;
using System.Text;
using System.Text.Json;

namespace BlazorPeliculas.Client.Repositories {
    public class Repository : IRepository {
        public HttpClient HttpClient { get; }

        public Repository(HttpClient httpClient) {
            HttpClient = httpClient;
        }
        private JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        };

        //Lo haremos con T porque podríamos recibir un listado de géneros, actores, películas, etc.
        public async Task<HttpResponseWrapper<T>> Get<T>(string url) {
            var responseHTTP = await HttpClient.GetAsync(url);
            if (responseHTTP.IsSuccessStatusCode) {
                var response = await DeserializeResponse<T>(responseHTTP, DefaultJsonOptions);
                return new HttpResponseWrapper<T>(response, Error: false, responseHTTP);
            }
            else {
                return new HttpResponseWrapper<T>(default, Error:true, responseHTTP);
            }
        }

        public async Task<HttpResponseWrapper<object>> Post<T>(string url, T send) {
            var sendJSON = JsonSerializer.Serialize(send);
            var sendContent = new StringContent(sendJSON, Encoding.UTF8, "application/json");
            var responseHttp = await HttpClient.PostAsync(url, sendContent);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public async Task<HttpResponseWrapper<object>> Put<T>(string url, T send) {
            var sendJSON = JsonSerializer.Serialize(send);
            var sendContent = new StringContent(sendJSON, Encoding.UTF8, "application/json");
            var responseHttp = await HttpClient.PutAsync(url, sendContent);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public async Task<HttpResponseWrapper<object>> Delete(string url) {
            var responseHTTP = await HttpClient.DeleteAsync(url);

            return new HttpResponseWrapper<object>(null, !responseHTTP.IsSuccessStatusCode, responseHTTP);
        }

        public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T send) {
            var sendJSON = JsonSerializer.Serialize(send);
            var sendContent = new StringContent(sendJSON, Encoding.UTF8, "application/json");
            var responseHttp = await HttpClient.PostAsync(url, sendContent);

            if(responseHttp.IsSuccessStatusCode) {
                var response = await DeserializeResponse<TResponse>(responseHttp, DefaultJsonOptions);
                return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
            }
            return new HttpResponseWrapper<TResponse>(default, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        private async Task<T> DeserializeResponse<T>(HttpResponseMessage httpResponse, JsonSerializerOptions jsonSerializerOptions) {
            var responseStting = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseStting, jsonSerializerOptions);
        }

        public List<Movie> GetMovies() {
            return new List<Movie>() {
            new Movie {Title = "Wakanda forever", 
                ReleaseDate = new DateTime(2022, 11, 11),
                Poster = "https://upload.wikimedia.org/wikipedia/en/thumb/3/3b/Black_Panther_Wakanda_Forever_poster.jpg/220px-Black_Panther_Wakanda_Forever_poster.jpg"},
            new Movie {Title = "Moana", 
                ReleaseDate = new DateTime(2016, 11, 23),
                Poster = "https://upload.wikimedia.org/wikipedia/en/thumb/2/26/Moana_Teaser_Poster.jpg/220px-Moana_Teaser_Poster.jpg" },
            new Movie {Title = "Inception", 
                ReleaseDate = new DateTime(2010, 7, 16),
                Poster = "https://upload.wikimedia.org/wikipedia/en/2/2e/Inception_%282010%29_theatrical_poster.jpg" }
        };
        }
    }
}
