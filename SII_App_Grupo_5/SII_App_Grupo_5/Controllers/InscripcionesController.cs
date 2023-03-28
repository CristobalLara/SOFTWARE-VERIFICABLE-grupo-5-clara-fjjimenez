using Microsoft.AspNetCore.Mvc;
using SII_App_Grupo_5.Models;
using SII_App_Grupo_5.Data;
using System.Diagnostics.Metrics;

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
            //cargar run , cruzo por id
            return View();
        }


        public IActionResult Details(int Id)
        {
            return View(contexto.Inscripciones.Find(Id));
        }


        [HttpPost]
        public IActionResult Create(Inscripcion inscripcion)
        {
            contexto.Inscripciones.AddRange(inscripcion);
            contexto.SaveChanges();
            return View();
        }
    }
}
