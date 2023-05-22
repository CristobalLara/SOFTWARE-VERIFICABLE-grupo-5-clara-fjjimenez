using Microsoft.AspNetCore.Mvc;
using SII_App_Grupo_5.Data;
using SII_App_Grupo_5.Models;

namespace SII_App_Grupo_5.Controllers
{
    public class MultiPropietarioController : Controller
    {
        public InscriptionsGrupo5DbContext contexto;
        public MultiPropietarioController(InscriptionsGrupo5DbContext Contexto)
        {
            contexto = Contexto;
        }

        public IActionResult Index()
        {
            ViewBag.Comunas = contexto.Comunas;

            IEnumerable<MultiPropietario> multiPropietarios = contexto.MultiPropietarios.OrderBy(mp => mp.FechaInscripcion).ToList();
            return View(multiPropietarios);
        }
        [HttpPost]
        public IActionResult Index(string searchComuna, string searchManzana, string searchPredio, 
            string searchAnoVigenciaInicial, string searchAnoVigenciaFinal)
        {
            int searchComunaInt, searchManzanaInt, searchPredioInt, searchAnoVigenciaInicialInt, searchAnoVigenciaFinalInt;

            bool isIntManzana = int.TryParse(searchManzana, out searchManzanaInt);
            bool isIntPredio = int.TryParse(searchPredio, out searchPredioInt);
            bool isIntAVI = int.TryParse(searchAnoVigenciaInicial, out searchAnoVigenciaInicialInt);
            bool isIntAVF = int.TryParse(searchAnoVigenciaFinal, out searchAnoVigenciaFinalInt);

            var multiPropietarios = contexto.MultiPropietarios.ToList();
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
               .Where(i => (i.AnoVigenciaFinal.ToString().Contains(searchAnoVigenciaFinal))).ToList();
            }
            ViewBag.Comunas = contexto.Comunas;
            return View(multiPropietarios);
        }
    }
}
