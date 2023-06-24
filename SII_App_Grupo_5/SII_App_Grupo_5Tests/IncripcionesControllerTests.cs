using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SII_App_Grupo_5.Models;
using SII_App_Grupo_5.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using SII_App_Grupo_5.Controllers;
using System.Web.Mvc;


namespace SII_App_Grupo_5Tests
{
    [TestClass]
    public class IncripcionesControllerTests
    {
        private readonly InscriptionsGrupo5DbContext _contexto;
        private readonly InscripcionesController _controller;

        public IncripcionesControllerTests(InscripcionesController controller, InscriptionsGrupo5DbContext contexto)
        {
            _contexto = contexto;
            _controller = controller;
        }
        [TestMethod]
        public void IsValidRut_ValidRutReturnsTrue(object Inscripciones)
        {
            string rutTest = "2-7";

            bool result = _controller.ValidaRut(rutTest);

            Assert.IsTrue(result, "Resultado debiese ser verdadero");
        }
        [TestMethod]
        public void IsValidRut_ValidRutReturnsFalse(object Inscripciones)
        {
            string rutTest = "78-9";

            bool result = _controller.ValidaRut(rutTest);

            Assert.IsTrue(result, "Resultado debiese ser falso");
        }
    }

}