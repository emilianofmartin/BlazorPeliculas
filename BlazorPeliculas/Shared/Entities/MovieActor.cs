using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.Entities {
    public class MovieActor {
        public int ActorID { get; set; }
        public int MovieID { get; set; }
        public Actor? Actor { get; set; }
        public Movie? Movie { get; set; }
        public string? Character { get; set; }
        public int Orden { get; set; }          //El orden de un actor en una película
    }
}
