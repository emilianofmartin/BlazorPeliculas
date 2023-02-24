using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.Entities {
    public class Genre {
        public int ID { get; set; }

        [Required(ErrorMessage = "The field '{0}' is required.")]
        public string Name { get; set; } = null!;
        public List<GenresMovie> Genres { get; set; } = new List<GenresMovie>();
    }
}
