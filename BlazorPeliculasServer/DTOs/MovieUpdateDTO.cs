using BlazorPeliculasServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculasServer.DTOs {
    public class MovieUpdateDTO {
        public Movie Movie { get; set; } = null!;
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public List<Genre> SelectedGenres { get; set; } = new List<Genre>();
        public List<Genre> UnselectedGenres { get; set; } = new List<Genre>();
    }
}
