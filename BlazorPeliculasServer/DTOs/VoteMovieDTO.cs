using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculasServer.DTOs {
    public class VoteMovieDTO {
        public int MovieID {  get; set; }
        public int Voto { get; set; }
    }
}
