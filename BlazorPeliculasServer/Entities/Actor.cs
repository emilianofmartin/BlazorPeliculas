using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculasServer.Entities {
    public class Actor {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Photo { get; set; }
        public DateTime? BirthDate { get; set; }
        [NotMapped]
        public string? Character { get; set; }
        public List<MovieActor> MovieActor { get; set; } = new List<MovieActor>();

        public override bool Equals(object? obj) {
            if(obj is Actor a2) {
                //El objeto recibido es un actor
                return ID == a2.ID;
            }

            return false;
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
