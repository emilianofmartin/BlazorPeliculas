using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.Entities {
    public class Movie {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public string? Summary { get; set; }
        public bool OnBillboard { get; set; }
        public string? Trailer { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? Poster { get; set; }
        public List<GenresMovie> GenresMovie { get; set; } = new List<GenresMovie>();
        public List<MovieActor> MovieActor { get; set; } = new List<MovieActor>();
        public List<VoteMovie> VotesMovies{ get; set; } = new List<VoteMovie>();
        public string? TrimmedTitle {
            get {
                if(string.IsNullOrWhiteSpace(Title)) {
                    return null;
                }

                if(Title.Length > 60) {
                    return Title.Substring(0, 60) + "...";
                }
                else {
                    return Title;
                }
            }
        }

        public string? urlTitle() { 
            return Title.Replace(" ", "-");
        }
    }
}
