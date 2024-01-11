using Castle.Core.Logging;
using Empleados.API.Controllers;
using Empleados.Core.Modelos;
using Empleados.Infraestructura.Data;
using Empleados.Infraestructura.Repositorio;
using Empleados.Infraestructura.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Empleados.Test
{
    [TestFixture]
    public class EmpleadoTests
    {
        public Empleado empleadoTest1;
        public Empleado empleadoTest2;
        public Empleado empleadoFail;
        private DbContextOptions<ApplicationDbContext> options;

        [SetUp]
        public void SetUp()
        {
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
                          .UseInMemoryDatabase(databaseName: "temp_empleadoDB").Options;

            empleadoTest1 = new Empleado()
            {
                Id = 1,
                Apellidos = "Paredes 1",
                Nombres = "Adrian 1",
                Cargo = "Desarrollador",
                CompaniaId = 1
            };
            empleadoTest2 = new Empleado()
            {
                Id = 2,
                Apellidos = "Paredes 2",
                Nombres = "Adrian 2",
                Cargo = "Desarrollador",
                CompaniaId = 1
            };
            empleadoFail = new Empleado()
            {
                Id = 2,
                Apellidos = "Paredes 2",
                Nombres = "Adrian 2",
                Cargo = "Desarrollador"
            };
        }

        [Test]
        [Order(1)]
        public async Task EmpleadoRepositorio_AgregarEmpleado_GrabadoExitoso()
        {
            //Arrange
            var context = new ApplicationDbContext(options);
            var empleadoRepositorio= new EmpleadoRepositorio(context);

            //Act
            await empleadoRepositorio.Agregar(empleadoTest1);
            await empleadoRepositorio.Guardar();
            var empleadoDB = await empleadoRepositorio.ObtenerPrimero();

            //Assert 
            Assert.AreEqual(empleadoTest1.Id, empleadoDB.Id);
            Assert.AreEqual(empleadoTest1.Apellidos, empleadoDB.Apellidos);
        }

        [Test]
        [Order(2)]
        public async Task EmpleadoRepositorio_ObtenerTodos_ObtenerListaEmpleados()
        {
            //Arrange
            var expectedResult = new List<Empleado> { empleadoTest1,empleadoTest2};
            var context = new ApplicationDbContext(options);
            var empleadoRepositorio = new EmpleadoRepositorio(context);
            context.Database.EnsureDeleted();

            //Act
            await empleadoRepositorio.Agregar(empleadoTest1);
            await empleadoRepositorio.Agregar(empleadoTest2);
            await empleadoRepositorio.Guardar();
            var empleadoList = await empleadoRepositorio.ObtenerTodos();

            CollectionAssert.AreEqual(expectedResult, empleadoList);
        }

        [Test]
        [Order(3)]
        public async Task EmpleadoController_GetEmpleados_ObtenerListaEmpleados()
        {
            //Arrange
            var empleados = new List<Empleado>()
            {
                new Empleado { Id = 1, Apellidos = "Apellido Test1", Nombres = "Nombres Test1", Cargo = "Cargo 1", CompaniaId =1},
                new Empleado { Id = 2, Apellidos = "Apellido Test2", Nombres = "Nombres Test2", Cargo = "Cargo 2", CompaniaId =1}
            };

            var mockEmpleadoRepositorio = new Mock<IEmpleadoRepositorio>();
            mockEmpleadoRepositorio.Setup(x => x.ObtenerTodos(null, null, "Compania")).ReturnsAsync(empleados);
            var mockLogger = new Mock<ILogger<EmpleadoController>>();

            var empleadoController = new EmpleadoController(mockEmpleadoRepositorio.Object, mockLogger.Object);
            var actionResult = await empleadoController.GetEmpleados();
            var resultado = actionResult.Result as OkObjectResult;
            var empleadosDB = resultado.Value as IEnumerable<Empleado>;

            //Assert
            CollectionAssert.AreEqual(empleados, empleadosDB);
            Assert.AreEqual(empleados.Count(), empleadosDB.Count());
        }

        [Test]
        [Order(4)]
        public async Task EmpleadoController_GetEmpleados_ObtenerEmpleado()
        {
            //Arrange

            var mockEmpleadoRepositorio = new Mock<IEmpleadoRepositorio>();
            mockEmpleadoRepositorio.Setup(x => x.ObtenerPrimero(e=>e.Id==1, "Compania")).ReturnsAsync(empleadoTest1);
            var mockLogger = new Mock<ILogger<EmpleadoController>>();

            var empleadoController = new EmpleadoController(mockEmpleadoRepositorio.Object, mockLogger.Object);
            var actionResult = await empleadoController.GetEmpleado(1);
            var resultado = actionResult.Result as OkObjectResult;
            var empleadoDB = resultado.Value as Empleado;

            //Assert
            Assert.AreEqual(empleadoTest1, empleadoDB);
        }
        [Test]
        [Order(5)]
        public async Task EmpleadoController_GetEmpleados_ObtenerNotFound()
        {
            //Arrange

            var mockEmpleadoRepositorio = new Mock<IEmpleadoRepositorio>();
            mockEmpleadoRepositorio.Setup(x => x.ObtenerPrimero(e => e.Id == 1, "Compania")).ReturnsAsync(empleadoTest1);
            var mockLogger = new Mock<ILogger<EmpleadoController>>();

            var empleadoController = new EmpleadoController(mockEmpleadoRepositorio.Object, mockLogger.Object);
            var actionResult = await empleadoController.GetEmpleado(-1);
            var resultado = actionResult.Result as OkObjectResult;
            

            //Assert
            Assert.IsNull(resultado);
        }
        [Test]
        [Order(6)]
        public async Task EmpleadoController_PostEmpleados_GrabadoExitoso()
        {
            //Arrange

            var mockEmpleadoRepositorio = new Mock<IEmpleadoRepositorio>();
            mockEmpleadoRepositorio.Setup(x => x.Agregar(empleadoTest1));
            var mockLogger = new Mock<ILogger<EmpleadoController>>();

            var empleadoController = new EmpleadoController(mockEmpleadoRepositorio.Object, mockLogger.Object);
            var actionResult = await empleadoController.PostEmpleado(empleadoTest1);
            var resultado = actionResult.Result as CreatedAtRouteResult;
            var empleadoDB = resultado.Value as Empleado;


            //Assert
            Assert.AreEqual(empleadoTest1, empleadoDB);
        }
        [Test]
        [Order(7)]
        public async Task EmpleadoController_PostEmpleados_ErrorAlGrabar()
        {
            //Arrange

            var mockEmpleadoRepositorio = new Mock<IEmpleadoRepositorio>();
            mockEmpleadoRepositorio.Setup(x => x.Agregar(empleadoFail));
            var mockLogger = new Mock<ILogger<EmpleadoController>>();

            var empleadoController = new EmpleadoController(mockEmpleadoRepositorio.Object, mockLogger.Object);
            var actionResult = await empleadoController.PostEmpleado(empleadoFail);
            var resultado = actionResult.Result as CreatedAtRouteResult;


            //Assert
            Assert.IsNull(resultado);
        }
    }
}
