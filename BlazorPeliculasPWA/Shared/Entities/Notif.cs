using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.Entities {
    public class Notif {
        public int Id { get; set; }
        public string URL { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
    }
}
