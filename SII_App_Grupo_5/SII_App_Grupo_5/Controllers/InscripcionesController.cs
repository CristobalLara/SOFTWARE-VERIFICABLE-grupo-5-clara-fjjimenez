using Microsoft.AspNetCore.Mvc;
using SII_App_Grupo_5.Models;
using SII_App_Grupo_5.Data;
using System.Diagnostics.Metrics;
using System.Linq;

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
            //SE OBTIENEN LAS COMUNAS PARA EL COMONBOX
            ViewBag.Comunas = contexto.Comunas;
            return View();
        }


        public IActionResult Details(int Id)
        {
            var inscripcion = contexto.Inscripciones.FirstOrDefault(i => i.Folio == Id);
            return View(inscripcion);
        }


        [HttpPost]
        public IActionResult Create(Inscripcion inscripcion, string[] AdquirientesRut, float[] AdquirientesPorcentajeDerecho, bool[] AdquirientesAcreditado,
        string[] EnajenantesRut, float[] EnajenantesPorcentajeDerecho, bool[] EnajenantesAcreditado)
        {
            List<Adquiriente> listaAdquirientes = new List<Adquiriente>();
            List<Enajenante> listaEnajenantes = new List<Enajenante>();

            contexto.Inscripciones.AddRange(inscripcion);
            contexto.SaveChanges();

            //CREACION DE ADQUIRIENTES DE LA INSCRIPCION
            for (int i = 0; i < AdquirientesRut.Count(); i++)
            {
                Adquiriente adquiriente = new Adquiriente();
                adquiriente.Rut = AdquirientesRut[i];
                adquiriente.PorcentajeDerecho = AdquirientesPorcentajeDerecho[i];
                adquiriente.Acreditado = AdquirientesAcreditado[i];
                adquiriente.InscripcionId = inscripcion.Folio;
                if (!adquiriente.Acreditado)
                {
                    adquiriente.PorcentajeDerecho = 0;
                }
                listaAdquirientes.Add(adquiriente);
                contexto.Adquirientes.AddRange(adquiriente);
            }
            //CREACION DE ENAJENANTES DE LA INSCRIPCION
            for (int i = 0; i < EnajenantesRut.Count(); i++)
            {
                Enajenante enajenante = new Enajenante();
                enajenante.Rut = EnajenantesRut[i];
                enajenante.PorcentajeDerecho = EnajenantesPorcentajeDerecho[i];
                enajenante.Acreditado = EnajenantesAcreditado[i];
                enajenante.InscripcionId = inscripcion.Folio;
                listaEnajenantes.Add(enajenante);
                contexto.Enajenantes.AddRange(enajenante);
            }

            float TotalPorcentajeDerecho = 100;
            int AdquirientesNoAcreditados = 0;

            //PROCESANDO REGULARIZACION DE PATRIMONIO
            if (inscripcion.NaturalezaEscritura == "RegularizacionPatrimonio")
            {
                for (int i = 0; i < AdquirientesAcreditado.Count(); i++)
                {
                    if (AdquirientesAcreditado[i])
                    {
                        TotalPorcentajeDerecho = TotalPorcentajeDerecho - AdquirientesPorcentajeDerecho[i];
                    }
                    else
                    {
                        AdquirientesNoAcreditados++;
                    }
                }

                float ParcialPorcentajeDerecho = TotalPorcentajeDerecho / AdquirientesNoAcreditados;
                
                for (int j = 0; j < AdquirientesAcreditado.Count(); j++)
                {
                    if (!AdquirientesAcreditado[j])
                    {
                        AdquirientesPorcentajeDerecho[j] = ParcialPorcentajeDerecho;
                    }
                }
            }
            //PROCESANDO CASOS DE COMPRAVENTA
            if (inscripcion.NaturalezaEscritura == "Compraventa")
            {
                //COMPRAVENTA DE TRANSFERENCIA TOTAL
                if (AdquirientesPorcentajeDerecho.Sum() == 100)
                {
                    float TransferenciaTotal = 0;
                    for (int i = 0; i < EnajenantesRut.Count(); i++)
                    {
                        List<MultiPropietario> multipropietariosEnajenantes = contexto.MultiPropietarios.
                        OrderBy(mp => mp.AnoInscripcion).
                        ThenBy(mp => mp.NumeroInscripcion).
                        Where(mp => mp.RutPropietario == EnajenantesRut[i] && mp.AnoVigenciaFinal==null).ToList();
                        for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                        {
                            if (multipropietariosEnajenantes[j].Comuna == inscripcion.Comuna &&
                                multipropietariosEnajenantes[j].Manzana == inscripcion.Manzana &&
                                multipropietariosEnajenantes[j].Predio == inscripcion.Predio)
                            {
                                TransferenciaTotal = TransferenciaTotal + multipropietariosEnajenantes[j].PorcentajeDerecho;
                            }
                            if (multipropietariosEnajenantes[j].AnoVigenciaInicial == inscripcion.FechaInscripcion.Year)
                            {
                                contexto.MultiPropietarios.Remove(multipropietariosEnajenantes[j]);
                                break;
                            }
                            else
                            {
                                multipropietariosEnajenantes[j].AnoVigenciaFinal = inscripcion.FechaInscripcion.Year;
                                break;
                            }
                        }
                        
                        for (int k = 0; k < AdquirientesPorcentajeDerecho.Count(); k++)
                        {
                            AdquirientesPorcentajeDerecho[k] = (float)(TransferenciaTotal * (AdquirientesPorcentajeDerecho[k] * 0.01));
                        }
                    }
                }
                //COMPRAVENTA DE DERECHOS
                else if (AdquirientesPorcentajeDerecho.Sum() < 100 && EnajenantesRut.Count() == 1 && AdquirientesRut.Count() == 1)
                {
                    List<MultiPropietario> multipropietariosEnajenantes = contexto.MultiPropietarios.
                    OrderBy(mp => mp.AnoInscripcion).
                    ThenBy(mp => mp.NumeroInscripcion).
                    Where(mp => mp.RutPropietario == EnajenantesRut[0] && mp.AnoVigenciaFinal == null).ToList();
                    for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                    {
                        if (multipropietariosEnajenantes[j].Comuna == inscripcion.Comuna &&
                            multipropietariosEnajenantes[j].Manzana == inscripcion.Manzana &&
                            multipropietariosEnajenantes[j].Predio == inscripcion.Predio)
                        {
                            float Derechos = multipropietariosEnajenantes[j].PorcentajeDerecho * 
                                               (EnajenantesPorcentajeDerecho[0] / 100) * 
                                               (AdquirientesPorcentajeDerecho[0] / 100);
                            AdquirientesPorcentajeDerecho[0] = Derechos;
                            multipropietariosEnajenantes[j].PorcentajeDerecho = multipropietariosEnajenantes[j].PorcentajeDerecho - Derechos;
                            break;
                        }
                    }
                }
                //COMPRAVENTA DE DOMINIOS
                else
                {
                    for (int i = 0; i < EnajenantesRut.Count(); i++)
                    {
                        List<MultiPropietario> multipropietariosEnajenantes = contexto.MultiPropietarios.
                        OrderBy(mp => mp.AnoInscripcion).
                        ThenBy(mp => mp.NumeroInscripcion).
                        Where(mp => mp.RutPropietario == EnajenantesRut[i]).ToList();
                        float Dominios = EnajenantesPorcentajeDerecho[i];
                        for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                        {
                            if (multipropietariosEnajenantes[j].Comuna == inscripcion.Comuna &&
                                multipropietariosEnajenantes[j].Manzana == inscripcion.Manzana &&
                                multipropietariosEnajenantes[j].Predio == inscripcion.Predio)
                            {
                                multipropietariosEnajenantes[j].PorcentajeDerecho = multipropietariosEnajenantes[j].PorcentajeDerecho - Dominios;
                                //MANEJANDO MULTIPROPIETARIOS NEGATIVOS
                                if (multipropietariosEnajenantes[j].PorcentajeDerecho < 0)
                                {
                                    multipropietariosEnajenantes[j].PorcentajeDerecho = 0;
                                }
                                break;
                            }

                        }
                    }
                }
            }
            //CREACION DE MULTIPROPIETARIO
            for (int i = 0; i < AdquirientesRut.Count(); i++)
            {
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

                //MANEJANDO MULTIPROPIETARIOS DUPLICADOS (MERGE)
                List<MultiPropietario> multipropietariosDuplicados = contexto.MultiPropietarios.
                        OrderBy(mp => mp.AnoInscripcion).
                        ThenBy(mp => mp.NumeroInscripcion).ToList();

                foreach (var duplicado in multipropietariosDuplicados)
                {
                    if (duplicado.Manzana == multipropietario.Manzana &&
                        duplicado.Comuna == multipropietario.Comuna &&
                        duplicado.Predio == multipropietario.Predio &&
                        duplicado.RutPropietario == multipropietario.RutPropietario &&
                        duplicado.AnoVigenciaFinal==null
                        )
                    {
                        if(multipropietario.NumeroInscripcion > duplicado.NumeroInscripcion)
                        {
                            multipropietario.PorcentajeDerecho += duplicado.PorcentajeDerecho;
                            contexto.MultiPropietarios.Remove(duplicado);
                        }
                        else
                        {
                            duplicado.PorcentajeDerecho += multipropietario.PorcentajeDerecho;
                            contexto.MultiPropietarios.Remove(multipropietario);
                        }
                    }
                }
                //MANEJANDO MULTIPROPIETARIOS ERRONEOS
                List<MultiPropietario> multipropietariosActivos = contexto.MultiPropietarios.
                        OrderBy(mp => mp.AnoInscripcion).
                        ThenBy(mp => mp.NumeroInscripcion).
                        Where(mp => mp.Manzana == multipropietario.Manzana &&
                              mp.Comuna == multipropietario.Comuna &&
                              mp.Predio == multipropietario.Predio &&
                              mp.AnoVigenciaFinal == null).ToList();

                float PorcentajeDerechoTotal = 0;
                
                foreach (var activos in multipropietariosActivos)
                {
                    {
                        PorcentajeDerechoTotal += activos.PorcentajeDerecho;
                    }
                }

                if (PorcentajeDerechoTotal>100)
                {
                    float Ponderado = 100 / PorcentajeDerechoTotal;

                    foreach (var activos in multipropietariosActivos)
                    {
                        {
                            activos.PorcentajeDerecho  = activos.PorcentajeDerecho*Ponderado;
                        }
                    }
                }
                else if (PorcentajeDerechoTotal < 100)
                {
                    int PropietariosNulos = multipropietariosActivos.Where(mp => mp.PorcentajeDerecho == 0).Count();

                    float Diferencia = (100 - PorcentajeDerechoTotal) / PropietariosNulos;

                    List<MultiPropietario> multipropietariosNulos = multipropietariosActivos.
                        OrderBy(mp => mp.AnoInscripcion).
                        ThenBy(mp => mp.NumeroInscripcion).
                        Where(mp => mp.PorcentajeDerecho == 0).ToList();

                    foreach (var nulos in multipropietariosNulos)
                    {   
                        {
                            nulos.PorcentajeDerecho = Diferencia;
                        }
                    }
                }
            }

            //MANEJANDO CASOS DE ERROR Y EXCEPCION

            float sumaPorcAdquirientes = listaAdquirientes.Sum(p => p.PorcentajeDerecho);
            float sumaPorcEnajenantes = listaEnajenantes.Sum(p => p.PorcentajeDerecho);
            
            if (sumaPorcAdquirientes > 100)
            {
                ModelState.AddModelError(string.Empty, "La Suma de los porcentajes de derecho para los Adquirientes no puede pasar el 100%");
                ViewBag.Comunas = contexto.Comunas;
                return View();
            }

            if (sumaPorcEnajenantes > 100)
            {
                ModelState.AddModelError(string.Empty, "La Suma de los porcentajes de derecho para los Enajenantes no puede pasar el 100%");
                //SE OBTIENEN LAS COMUNAS PARA EL COMONBOX
                ViewBag.Comunas = contexto.Comunas;
                return View();
            }

            if (inscripcion.NaturalezaEscritura == "RegularizacionPatrimonio" && listaEnajenantes.Count() > 0)
            {
                ModelState.AddModelError(string.Empty, "Regularizacion de Patrimonio no usa Enajenantes");
                //SE OBTIENEN LAS COMUNAS PARA EL COMONBOX
                ViewBag.Comunas = contexto.Comunas;
                return View();
            }

            contexto.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
