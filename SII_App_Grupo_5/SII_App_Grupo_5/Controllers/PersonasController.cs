using Microsoft.AspNetCore.Mvc;
using SII_App_Grupo_5.Data;
using SII_App_Grupo_5.Models;
using System.Diagnostics;

namespace SII_App_Grupo_5.Controllers
{

    public class PersonasController : Controller
    {
        public InscriptionsGrupo5DbContext contexto;
        public PersonasController(InscriptionsGrupo5DbContext Contexto)
        {
            contexto = Contexto;
        }
        public IActionResult Index()
        {
            IEnumerable<Persona> personas = contexto.Personas.ToList();
            return View(personas);
        }
        public IActionResult Create()
        {
            //cargar run , cruzo por id
            return View();
        }

        public IActionResult Details(int Id)
        {
            return View(contexto.Personas.Find(Id));
        }

        [HttpPost]
        public IActionResult Create(Persona persona)
        {
            contexto.Personas.AddRange(persona);
            contexto.SaveChanges();
            return View();
        }
    }
}
