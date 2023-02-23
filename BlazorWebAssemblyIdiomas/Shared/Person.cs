using BlazorWebAssemblyIdiomas.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAssemblyIdiomas.Shared {
    public class Person {
        [Required(
            ErrorMessageResourceType = typeof(Resource), 
            ErrorMessageResourceName = nameof(Resource.required))]
        public string Name { get; set; } = null!;
    }
}
