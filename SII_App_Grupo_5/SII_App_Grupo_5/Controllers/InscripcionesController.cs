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
            ViewBag.Comunas = contexto.Comunas;
            //cargar run , cruzo por id
            return View();
        }


        public IActionResult Details(int Id)
        {
            var inscripcion = contexto.Inscripciones.FirstOrDefault(i => i.Folio == Id);
            return View(inscripcion);
        }


        [HttpPost]
        public IActionResult Create(Inscripcion inscripcion, string[] AdquirientesRut, int[] AdquirientesPorcentajeDerecho, bool[] AdquirientesAcreditado,
        string[] EnajenantesRut, int[] EnajenantesPorcentajeDerecho, bool[] EnajenantesAcreditado)
        {
            contexto.Inscripciones.AddRange(inscripcion);
            contexto.SaveChanges();

            List<Inscripcion> inscripciones = contexto.Inscripciones.ToList();
            List<Enajenante> enajenantes = contexto.Enajenantes.ToList();
            List<Adquiriente> adquirientes = contexto.Adquirientes.ToList();


            for (int i = 0; i < AdquirientesRut.Count(); i++)
            {
                Adquiriente adquiriente = new Adquiriente();
                adquiriente.Rut = AdquirientesRut[i];
                adquiriente.PorcentajeDerecho = AdquirientesPorcentajeDerecho[i];
                adquiriente.Acreditado = AdquirientesAcreditado[i];
                adquiriente.InscripcionId = inscripcion.Folio;
                contexto.Adquirientes.AddRange(adquiriente);

                MultiPropietario multipropietario = new MultiPropietario();
                multipropietario.RutPropietario = AdquirientesRut[i];
                multipropietario.PorcentajeDerecho = AdquirientesPorcentajeDerecho[i];
                multipropietario.Fojas = inscripcion.Fojas;
                multipropietario.NumeroInscripcion = inscripcion.NumeroInscripcion;
                multipropietario.FechaInscripcion = inscripcion.FechaInscripcion;
                multipropietario.AnoInscripcion = inscripcion.FechaInscripcion.Year;
                if (inscripcion.FechaInscripcion <= new DateTime(2019, 1, 1))
                {
                    multipropietario.AnoVigenciaInicial = new DateTime(2019, 1, 1).Year;
                }
                else
                {
                    multipropietario.AnoVigenciaInicial = inscripcion.FechaInscripcion.Year;
                }

                multipropietario.AnoVigenciaFinal = null;
                multipropietario.Comuna = inscripcion.Comuna;
                multipropietario.Manzana = inscripcion.Manzana;
                multipropietario.Predio = inscripcion.Predio;
                contexto.MultiPropietarios.AddRange(multipropietario);
            }

            for (int i = 0; i < EnajenantesRut.Count(); i++)
            {
                Enajenante enajenante = new Enajenante();
                enajenante.Rut = EnajenantesRut[i];
                enajenante.PorcentajeDerecho = EnajenantesPorcentajeDerecho[i];
                enajenante.Acreditado = EnajenantesAcreditado[i];
                enajenante.InscripcionId = inscripcion.Folio;
                contexto.Enajenantes.AddRange(enajenante);
            }

            List<MultiPropietario> multipropietarios = contexto.MultiPropietarios.OrderBy(mp => mp.AnoInscripcion).ThenBy(mp => mp.NumeroInscripcion).ToList();
            for (int i = 0; i < multipropietarios.Count(); i++)
            {

            }
            contexto.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
