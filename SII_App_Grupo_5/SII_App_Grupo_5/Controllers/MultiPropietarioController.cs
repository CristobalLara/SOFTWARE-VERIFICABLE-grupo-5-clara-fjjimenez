using Microsoft.AspNetCore.Mvc;
using SII_App_Grupo_5.Models;
using SII_App_Grupo_5.Data;

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
    }
}
