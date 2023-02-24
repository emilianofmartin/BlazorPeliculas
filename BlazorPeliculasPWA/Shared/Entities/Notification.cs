using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.Entities {
    public class Notification {
        public int Id { get; set; }
        public string URL { get; set; }
        public string P256h { get; set; }
        public string Auth { get; set; }
    }
}
