using MessagePack;
using Microsoft.AspNetCore.Mvc;
using SII_App_Grupo_5.Data;
using SII_App_Grupo_5.Models;
using System.Data;

namespace SII_App_Grupo_5.Controllers
{
    public class MultiPropietarioController : Controller
    {
        public InscripcionesGrupo5DbContext _contexto;
        public MultiPropietarioController(InscripcionesGrupo5DbContext contexto)
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
            string searchAnoVigencia)
        {
            bool isIntManzana = int.TryParse(searchManzana, out _);
            bool isIntPredio = int.TryParse(searchPredio, out _);
            bool isIntAV = int.TryParse(searchAnoVigencia, out _);
            int anoVigencia;
            if (isIntAV) { anoVigencia = int.Parse(searchAnoVigencia); }
            else                { anoVigencia = -1;}

            List<MultiPropietario> multiPropietarios = IntIsValid(  isIntManzana,
                                                                    isIntPredio,
                                                                    isIntAV,
                                                                    searchComuna,
                                                                    searchManzana,
                                                                    searchPredio,
                                                                    anoVigencia);
            ViewBag.Comunas = _contexto.Comunas;
            return View(multiPropietarios);
        }

        public List<MultiPropietario> IntIsValid(bool isIntManzana,
                                           bool isIntPredio,
                                           bool isIntAV,
                                           string searchComuna,
                                           string searchManzana,
                                           string searchPredio,
                                           int annoVigencia)
        {
            int thisYear = DateTime.Now.Year;
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
            if (isIntAV)
            {
                multiPropietarios = multiPropietarios
               .Where(i => ((i.AnoVigenciaInicial <= annoVigencia) && ((i.AnoVigenciaFinal >= annoVigencia) || (thisYear >= annoVigencia)))).ToList();
            }
            return multiPropietarios;
        }
    }
}
