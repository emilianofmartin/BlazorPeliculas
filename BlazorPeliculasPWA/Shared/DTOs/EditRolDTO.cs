using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.DTOs {
    public class EditRolDTO {
        public string UserID { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
