using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.Entities {
    public class GenresMovie {
        public int MovieID { get; set; }
        public int GenreID { get; set; }
        public Genre? Genre { get; set; }
        public Movie? Movie { get; set; }
    }
}
