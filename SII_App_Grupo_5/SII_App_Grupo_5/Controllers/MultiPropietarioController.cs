using Microsoft.AspNetCore.Mvc;
using SII_App_Grupo_5.Data;
using SII_App_Grupo_5.Models;

namespace SII_App_Grupo_5.Controllers
{
    public class MultiPropietarioController : Controller
    {
        public InscriptionsGrupo5DbContext _contexto;
        public MultiPropietarioController(InscriptionsGrupo5DbContext contexto)
        {
            _contexto = contexto;
        }

        public IActionResult Index()
        {
            ViewBag.Comunas = _contexto.Comunas;

            IEnumerable<MultiPropietario> multiPropietarios = _contexto.MultiPropietarios.OrderBy(mp => mp.FechaInscripcion).ToList();
            return View(multiPropietarios);
        }
        [HttpPost]
        public IActionResult Index(string searchComuna, string searchManzana, string searchPredio, 
            string searchAnoVigenciaInicial, string searchAnoVigenciaFinal)
        {
            bool isIntManzana = int.TryParse(searchManzana, out int searchManzanaInt);
            bool isIntPredio = int.TryParse(searchPredio, out int searchPredioInt);
            bool isIntAVI = int.TryParse(searchAnoVigenciaInicial, out int searchAnoVigenciaInicialInt);
            bool isIntAVF = int.TryParse(searchAnoVigenciaFinal, out int searchAnoVigenciaFinalInt);

            var multiPropietarios = _contexto.MultiPropietarios.ToList();
            if (searchComuna != null)
            {
                 multiPropietarios = multiPropietarios
                .Where(i => (i.Comuna.Contains(searchComuna))).ToList();
            }
            if (isIntManzana)
            {
                multiPropietarios = multiPropietarios
               .Where(i => (isIntManzana && i.Manzana.ToString().Contains(searchManzana))).ToList();
            }
            if (isIntPredio)
            {
                multiPropietarios = multiPropietarios
               .Where(i => (isIntPredio && i.Predio.ToString().Contains(searchPredio))).ToList();
            }
            if (isIntAVI)
            {
                multiPropietarios = multiPropietarios
               .Where(i => (i.AnoVigenciaInicial.ToString().Contains(searchAnoVigenciaInicial))).ToList();
            }
            if (isIntAVF)
            {
                multiPropietarios = multiPropietarios
               .Where(i => (i.AnoVigenciaFinal?.ToString().Contains(searchAnoVigenciaFinal) ?? false)).ToList();
            }
            ViewBag.Comunas = _contexto.Comunas;
            return View(multiPropietarios);
        }
    }
}
