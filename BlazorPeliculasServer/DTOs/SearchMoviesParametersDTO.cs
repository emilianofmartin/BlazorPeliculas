using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculasServer.DTOs {
    public class SearchMoviesParametersDTO {
        public int Page { get; set; } = 1;
        public int RecordCount { get; set; } = 10;
        public PaginationDTO pagination {
            get {
                return new PaginationDTO {
                    Page = Page,
                    RecordCount = RecordCount
                };
            }
        }

        public string? Title { get; set; }
        public int GenreID {  get; set; }
        public bool Onbillboard { get; set; }
        public bool Releases { get; set; }
        public bool MostVoted { get; set; }
    }
}
