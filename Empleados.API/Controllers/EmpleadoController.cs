using Empleados.Core.Modelos;
using Empleados.Infraestructura.Repositorio.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Empleados.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly IEmpleadoRepositorio _repo;
        private readonly ILogger<EmpleadoController> _logger;

        public EmpleadoController(IEmpleadoRepositorio repo, ILogger<EmpleadoController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
        {
            _logger.LogInformation("Listado de Empleados");
            var lista = await _repo.ObtenerTodos(incluirPropiedades: "Compania");

            return Ok(lista);
        }

        [HttpGet("{id}", Name = "GetEmpleado")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Empleado>> GetEmpleado(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Debe de Enviar el ID");
                return BadRequest();
            }

            var emp = await _repo.ObtenerPrimero(e => e.Id == id, incluirPropiedades: "Compania");

            if (emp == null)
            {
                return NotFound();
            }

            return Ok(emp);  // Status code = 200
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Empleado>> PostEmpleado([FromBody] Empleado empleado)
        {

            if (empleado == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var empleadoExiste = await _repo.ObtenerPrimero(
                                         c => c.Apellidos.ToLower() == empleado.Apellidos.ToLower() &&
                                              c.Nombres.ToLower() == empleado.Nombres.ToLower()
                                        );

            if (empleadoExiste != null)
            {

                return BadRequest("Nombre del Empleado ya existe!");
            }

            if(empleado.CompaniaId == 0)
            {
                return BadRequest("Compania ID Es Obligatorio");
            }

            await _repo.Agregar(empleado);
            await _repo.Guardar();

            return CreatedAtRoute("GetEmpleado", new { id = empleado.Id }, empleado); // Status Code= 201
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteEmpleado(int id)
        {
            var empleado = await _repo.ObtenerPrimero(e => e.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }
            _repo.Remover(empleado);
            await _repo.Guardar();
            return NoContent();
        }
    }
}
