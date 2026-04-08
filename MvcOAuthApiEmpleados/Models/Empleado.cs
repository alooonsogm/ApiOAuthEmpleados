using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcOAuthApiEmpleados.Models
{
    public class Empleado
    {
        public int idEmpleado { get; set; }
        public string Apellido { get; set; }
        public string Oficio { get; set; }
        public int Salario { get; set; }
        public int idDepartamento { get; set; }
    }
}
