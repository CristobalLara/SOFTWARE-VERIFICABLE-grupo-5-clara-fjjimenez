using SII_App_Grupo_5.Controllers;
using SII_App_Grupo_5.Data;
using SII_App_Grupo_5.Models;

namespace SII_App_Grupo_5.UnitTests
{
    [TestClass]
    public class InscripcionesControllerTests
    {
        public InscriptionsGrupo5DbContext? _contexto;
        public InscripcionesController? _controller;

        [TestInitialize]
        public void Initialize()
        {
            _contexto = new InscriptionsGrupo5DbContext();
            _controller = new InscripcionesController(_contexto);
        }

        [TestMethod]
        [TestCategory("RUT")]
        public void IsValidRut_ValidRutReturnsTrue()
        {
            string rutTestValid = "2-7";
            bool result = _controller!.ValidaRut(rutTestValid);

            Assert.IsTrue(result, "Resultado debiese ser verdadero");
        }
        [TestMethod]
        [TestCategory("RUT")]
        public void IsValidRut_ValidRutReturnsFalse()
        {
            string rutTestInvalid = "78-9";
            bool result = _controller!.ValidaRut(rutTestInvalid);

            Assert.IsFalse(result, "Resultado debiese ser falso");
        }

        [TestMethod]
        [TestCategory("Regularizacion de Patrimonio con un adquireinte")]
        public void RegularizacionPatrimonio_ReturnsPorcentaje()
        {
            bool[] adquirientesAcreditado = new bool[] { true };
            float totalPorcentajeDerecho = 100;
            int adquirientesNoAcreditados = 0;
            List<float> adquirientesPorcentajeDerechoFloat = new(){ 100.0f };

            float result = _controller!.RegularizacionPatrimonio(adquirientesAcreditado, totalPorcentajeDerecho, adquirientesNoAcreditados, adquirientesPorcentajeDerechoFloat);
           
            Assert.AreEqual(result, 100.0f);
        }
        [TestMethod]
        [TestCategory("Regularizacion de Patrimonio con dos adquireintes")]
        public void RegularizacionPatrimonio_ReturnsPorcentajeSegundoAdquiriente()
        {
            bool[] adquirientesAcreditado = new bool[] { true, true };
            float totalPorcentajeDerecho = 100;
            int adquirientesNoAcreditados = 0;
            List<float> adquirientesPorcentajeDerechoFloat = new(){ 60.0f, 40.0f };

            float result = _controller!.RegularizacionPatrimonio(adquirientesAcreditado, totalPorcentajeDerecho, adquirientesNoAcreditados, adquirientesPorcentajeDerechoFloat);

            Assert.AreEqual(result, 40.0f);
        }

        [TestMethod]
        [TestCategory("Creacion Enajenantes")]
        public void CreacionEnajenantes_ReturnsEnajenante()
        {
            // Arrange
            bool[] enajenantesAcreditado = new bool[] { true };
            List<float> enajenantesPorcentajeDerechoFloat = new(){ 100.0f };
            string[] enajenantesRut = new string[] { "19189435-9" };
            List<Enajenante> listaEnajenantes = new();
            Inscripcion inscripcion = new()
            {
                NaturalezaEscritura = "Compraventa",
                Comuna = "Macul",
                Manzana = 1,
                Predio = 2,
                Fojas = 0,
                NumeroInscripcion = 1,
                FechaInscripcion = DateTime.Now
            };
            List<Enajenante> listaEnajenantesMock = new();
            Enajenante enajenanteEsperado = new()
            {
                Rut = enajenantesRut[0],
                PorcentajeDerecho = enajenantesPorcentajeDerechoFloat[0],
                Acreditado = enajenantesAcreditado[0],
                InscripcionId = inscripcion.Folio
        };
            
            listaEnajenantesMock.Add(enajenanteEsperado);

            // Act
            List<Enajenante> result = _controller!.CreacionEnajenantes(inscripcion, listaEnajenantes, enajenantesRut, enajenantesPorcentajeDerechoFloat, enajenantesAcreditado);
            Assert.AreEqual(result[0].Rut, listaEnajenantesMock[0].Rut);
            Assert.AreEqual(result[0].PorcentajeDerecho, listaEnajenantesMock[0].PorcentajeDerecho);
            Assert.AreEqual(result[0].Acreditado, listaEnajenantesMock[0].Acreditado);
            Assert.AreEqual(result[0].InscripcionId, listaEnajenantesMock[0].InscripcionId);

        }
        [TestMethod]
        [TestCategory("Creacion Adquirientes")]
        public void CreacionAdquirientes_ReturnsAdquiriente()
        {
            bool[] adquirientesAcreditado = new bool[] { true };
            List<float> adquirientesPorcentajeDerechoFloat = new() { 100.0f };
            string[] adquirientesRut = new string[] { "19189435-9" };
            List<Adquiriente> listaAdquirientes = new();
            Inscripcion inscripcion = new()
            {
                NaturalezaEscritura = "Compraventa",
                Comuna = "Macul",
                Manzana = 1,
                Predio = 2,
                Fojas = 0,
                NumeroInscripcion = 1,
                FechaInscripcion = DateTime.Now
            };

            List<Adquiriente> listaAdquirientesMock = new();
            Adquiriente adquirienteEsperado = new()
            {
                Rut = adquirientesRut[0],
                PorcentajeDerecho = adquirientesPorcentajeDerechoFloat[0],
                Acreditado = adquirientesAcreditado[0],
                InscripcionId = inscripcion.Folio
        };
            listaAdquirientesMock.Add(adquirienteEsperado);

            List<Adquiriente> result = _controller!.CreacionAdquirientes(inscripcion, listaAdquirientes, adquirientesRut, adquirientesPorcentajeDerechoFloat, adquirientesAcreditado);
            Assert.AreEqual(result[0].Rut, listaAdquirientesMock[0].Rut);
            Assert.AreEqual(result[0].PorcentajeDerecho, listaAdquirientesMock[0].PorcentajeDerecho);
            Assert.AreEqual(result[0].Acreditado, listaAdquirientesMock[0].Acreditado);
            Assert.AreEqual(result[0].InscripcionId, listaAdquirientesMock[0].InscripcionId);
        }

        [TestMethod]
        [TestCategory("Transferencia Total")]
        public void CompraventaTransferenciaTotal_ReturnsPorcentaje()
        {
            List<float> adquirientesPorcentajeDerechoFloat = new(){ 100.0f };
            string[] enajenantesRut = new string[] { "19189435-9" };
            Inscripcion inscripcion = new()
            {
                NaturalezaEscritura = "Compraventa",
                Comuna = "Macul",
                Manzana = 1,
                Predio = 2,
                Fojas = 0,
                NumeroInscripcion = 1,
                FechaInscripcion = DateTime.Now
            };

            float result = _controller!.CompraventaTransferenciaTotal(inscripcion, adquirientesPorcentajeDerechoFloat, enajenantesRut);
            Assert.AreEqual(result, 1);
        }
        [TestMethod]
        [TestCategory("Transferencia Dominio")]
        public void CompraventaDominios_ReturnsPorcentaje()
        {
            List<float> enajenantesPorcentajeDerechoFloat = new(){ 30.0f, 20.0f };
            string[] enajenantesRut = new string[] { "19189435-9", "10725958-9" };
            Inscripcion inscripcion = new()
            {
                NaturalezaEscritura = "Compraventa",
                Comuna = "Macul",
                Manzana = 1,
                Predio = 2,
                Fojas = 0,
                NumeroInscripcion = 1,
                FechaInscripcion = DateTime.Now
            };

            float result = _controller!.CompraventaDominios(inscripcion, enajenantesRut, enajenantesPorcentajeDerechoFloat);
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void CrearMultiPropietario_ReturnsMultipropietarion ()
        {
            DateTime FechaInscripcion = DateTime.Now;
            Inscripcion inscripcionAdquiriente = new()
            { 
                NaturalezaEscritura = "Compraventa",
                Comuna = "Santiago",
                Manzana = 1,
                Predio = 1,
                Fojas = 1,
                NumeroInscripcion = 1,
                FechaInscripcion = FechaInscripcion
            };

            MultiPropietario multiPropietario = new()
            {
                Comuna = "Santiago",
                Manzana = 1,
                Predio = 1,
                RutPropietario = "1",
                PorcentajeDerecho = 100,
                Fojas = 1,
                AnoInscripcion = 2023,
                FechaInscripcion = FechaInscripcion,
                NumeroInscripcion = 1,
                AnoVigenciaInicial = 2023
            };

            List<MultiPropietario> multipropietarios = new() { multiPropietario! };
            MultiPropietario result = _controller!.CrearMultiPropietario(inscripcionAdquiriente!,
                                                                    multipropietarios,
                                                                    0);
            Assert.AreEqual(result.Comuna, inscripcionAdquiriente!.Comuna);
            Assert.AreEqual(result.Manzana, inscripcionAdquiriente!.Manzana);
            Assert.AreEqual(result.Predio, inscripcionAdquiriente!.Predio);
            Assert.AreEqual(result.Fojas, inscripcionAdquiriente!.Fojas);
            Assert.AreEqual(result.NumeroInscripcion, inscripcionAdquiriente!.NumeroInscripcion);
            Assert.AreEqual(result.FechaInscripcion, inscripcionAdquiriente!.FechaInscripcion);
        }
    }

}