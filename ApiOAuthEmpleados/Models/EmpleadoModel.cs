using System.ComponentModel.DataAnnotations.Schema;

namespace ApiOAuthEmpleados.Models
{
    public class EmpleadoModel
    {
        public int idEmpleado { get; set; }
        public string Apellido { get; set; }
        public string Oficio { get; set; }
        public int Salario { get; set; }
        public int idDepartamento { get; set; }
    }
}
