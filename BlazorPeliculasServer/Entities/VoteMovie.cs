using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculasServer.Entities {
    public class VoteMovie {
        public int ID { get; set; }
        public int Voto { get; set; }
        public DateTime VoteWhen { get; set; }
        public int MovieID { get; set; }
        public Movie? Movie { get; set; }

        public string UserID { get; set; } = null!;
    }
}
