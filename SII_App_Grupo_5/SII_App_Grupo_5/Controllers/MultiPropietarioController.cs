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
            IEnumerable<MultiPropietario> multiPropietarios = contexto.MultiPropietarios.ToList();
            return View(multiPropietarios);
        }
        [HttpPost]
        public IActionResult Index(string search)
        {
            int searchInt;
            bool isInt = int.TryParse(search, out searchInt);

            var multiPropietarios = contexto.MultiPropietarios
                .Where(i => i.Comuna.Contains(search)
                        || (isInt && i.Manzana.ToString().Contains(search))
                        || (isInt && i.Predio.ToString().Contains(search))
                        || (isInt && i.AnoVigenciaInicial.ToString().Contains(search))
                        || (isInt && i.AnoVigenciaFinal.ToString().Contains(search)))
                .ToList();

            return View(multiPropietarios);
        }
    }
}
