using BlazorPeliculas.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.DTOs {
    public class MovieViewDTO {
        public Movie Movie { get; set; } = null!;
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public int UserVote { get; set; }
        public double VotesMedia { get; set; }
    }
}
