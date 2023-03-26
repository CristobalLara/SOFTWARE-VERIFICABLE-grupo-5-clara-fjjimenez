using Microsoft.AspNetCore.Mvc;
using SII_App_Grupo_5.Models;
using SII_App_Grupo_5.Data;

namespace SII_App_Grupo_5.Controllers
{
    public class InscripcionesController : Controller
    {
        public InscriptionsGrupo5DbContext contexto;
        public InscripcionesController(InscriptionsGrupo5DbContext Contexto)
        {
            contexto = Contexto;
        }
        public IActionResult Index()
        {
            IEnumerable<Inscripcion> inscripciones = contexto.Inscripciones.ToList();
            return View(inscripciones);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Inscripcion inscripcion)
        {
            return View();
        }
    }
}
