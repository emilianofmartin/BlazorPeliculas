using BlazorPeliculasServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculasServer.DTOs {
    public class HomePageDTO {
        public List<Movie>? OnBoard { get; set; }
        public List<Movie>? NextReleases { get; set; }

    }
}
