using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleados.Core.Modelos
{
    public class Compania
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la Compania es Requerido")]
        [MaxLength(100, ErrorMessage = "No sea mayor a 100")]
        public string NombreCompania { get; set; }

        [Required(ErrorMessage = "El direccion de la Compania es Requerido")]
        [MaxLength(150, ErrorMessage = "No sea mayor a 150")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El telefono de la Compania es Requerido")]
        [MaxLength(40, ErrorMessage = "No sea mayor a 40")]
        public string Telefono { get; set; }

        [MaxLength(40, ErrorMessage = "No sea mayor a 40")]
        public string Telefono2 { get; set; }

    }
}
