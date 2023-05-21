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
        public IActionResult Create(Inscripcion inscripcion, int[] AdquirientesRut, int[] AdquirientesPorcentajeDerecho, bool[] AdquirientesAcreditado,
        int[] EnajenantesRut, int[] EnajenantesPorcentajeDerecho, bool[] EnajenantesAcreditado)
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
            var a = EnajenantesRut.Count();

            for (int i = 0; i < EnajenantesRut.Count(); i++)
            {
                Enajenante enajenante = new Enajenante();
                enajenante.Rut = EnajenantesRut[i];
                enajenante.PorcentajeDerecho = EnajenantesPorcentajeDerecho[i];
                enajenante.Acreditado = EnajenantesAcreditado[i];
                enajenante.InscripcionId = inscripcion.Folio;
                contexto.Enajenantes.AddRange(enajenante);
            }
            contexto.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
