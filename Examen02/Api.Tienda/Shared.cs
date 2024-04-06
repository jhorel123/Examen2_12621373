using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Tienda
{
    public class Shared
    {
        [Key]
        public int IdCliente { get; set; }
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
    }
}
