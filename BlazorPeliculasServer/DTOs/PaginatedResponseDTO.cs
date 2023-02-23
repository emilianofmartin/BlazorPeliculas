namespace BlazorPeliculasServer.DTOs {
    public class PaginatedResponseDTO<T> {
        public int totalPages { get; set; }
        public List<T> Records { get; set; }
    }
}
