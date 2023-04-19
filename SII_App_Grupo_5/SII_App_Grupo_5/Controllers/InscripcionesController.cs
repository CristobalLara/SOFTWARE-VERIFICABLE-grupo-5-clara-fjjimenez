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
            var inscripcion = contexto.Inscripciones.FirstOrDefault(i => i.Folio == Id);
            return View(inscripcion);
        }


        [HttpPost]
        public IActionResult Create(Inscripcion inscripcion, int[] AdquirientesRut, int[] AdquirientesPorcentajeDerecho, bool[] AdquirientesAcreditado)
        {
            contexto.Inscripciones.AddRange(inscripcion);
            contexto.SaveChanges();

            for (int i = 0; i < AdquirientesRut.Count(); i++)
            {
                Adquiriente adquiriente = new Adquiriente();
                adquiriente.Rut = AdquirientesRut[i];
                adquiriente.PorcentajeDerecho = AdquirientesPorcentajeDerecho[i];
                adquiriente.Acreditado = AdquirientesAcreditado[i];
                adquiriente.InscripcionId = inscripcion.Folio;
                contexto.Adquirientes.AddRange(adquiriente);
            }
            contexto.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
