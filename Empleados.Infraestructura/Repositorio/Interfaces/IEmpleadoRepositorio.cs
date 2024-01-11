using Empleados.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleados.Infraestructura.Repositorio.Interfaces
{
    public interface IEmpleadoRepositorio : IRepositorio<Empleado>
    {
        void Actualizar(Empleado empleado);
    }
}
