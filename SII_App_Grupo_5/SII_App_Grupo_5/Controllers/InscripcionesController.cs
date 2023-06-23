using Microsoft.AspNetCore.Mvc;
using SII_App_Grupo_5.Models;
using SII_App_Grupo_5.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System;

namespace SII_App_Grupo_5.Controllers
{
    public class InscripcionesController : Controller
    {
        public InscriptionsGrupo5DbContext _contexto;
        public InscripcionesController(InscriptionsGrupo5DbContext contexto)
        {
            _contexto = contexto;
        }
        public IActionResult Index()
        {
            IEnumerable<Inscripcion> inscripciones = _contexto.Inscripciones.ToList();
            return View(inscripciones);
        }

        public IActionResult Create()
        {
            //SE OBTIENEN LAS COMUNAS PARA EL COMONBOX
            ViewBag.Comunas = _contexto.Comunas;
            return View();
        }


        public IActionResult Details(int Id)
        {
            var inscripcion = _contexto.Inscripciones.FirstOrDefault(i => i.Folio == Id);
            return View(inscripcion);
        }


        [HttpPost]
        public IActionResult Create(Inscripcion inscripcion, string[] adquirientesRut, float[] adquirientesPorcentajeDerecho, bool[] adquirientesAcreditado,
        string[] enajenantesRut, float[] enajenantesPorcentajeDerecho, bool[] enajenantesAcreditado)
        {
            List<Adquiriente> listaAdquirientes = new List<Adquiriente>();
            List<Enajenante> listaEnajenantes = new List<Enajenante>();

            _contexto.Inscripciones.AddRange(inscripcion);
            _contexto.SaveChanges();

            //CREACION DE ADQUIRIENTES DE LA INSCRIPCION
            for (int i = 0; i < adquirientesRut.Count(); i++)
            {
                Adquiriente adquiriente = new Adquiriente();
                adquiriente.Rut = adquirientesRut[i];
                adquiriente.PorcentajeDerecho = adquirientesPorcentajeDerecho[i];
                adquiriente.Acreditado = adquirientesAcreditado[i];
                adquiriente.InscripcionId = inscripcion.Folio;
                if (!adquiriente.Acreditado)
                {
                    adquiriente.PorcentajeDerecho = 0;
                }
                listaAdquirientes.Add(adquiriente);
                _contexto.Adquirientes.AddRange(adquiriente);
            }
            //CREACION DE ENAJENANTES DE LA INSCRIPCION
            for (int i = 0; i < enajenantesRut.Count(); i++)
            {
                Enajenante enajenante = new Enajenante();
                enajenante.Rut = enajenantesRut[i];
                enajenante.PorcentajeDerecho = enajenantesPorcentajeDerecho[i];
                enajenante.Acreditado = enajenantesAcreditado[i];
                enajenante.InscripcionId = inscripcion.Folio;
                listaEnajenantes.Add(enajenante);
                _contexto.Enajenantes.AddRange(enajenante);
            }

            float totalPorcentajeDerecho = 100;
            int adquirientesNoAcreditados = 0;

            //PROCESANDO REGULARIZACION DE PATRIMONIO
            if (inscripcion.NaturalezaEscritura == "RegularizacionPatrimonio")
            {
                for (int i = 0; i < adquirientesAcreditado.Count(); i++)
                {
                    if (adquirientesAcreditado[i])
                    {
                        totalPorcentajeDerecho = totalPorcentajeDerecho - adquirientesPorcentajeDerecho[i];
                    }
                    else
                    {
                        adquirientesNoAcreditados++;
                    }
                }

                float parcialPorcentajeDerecho = totalPorcentajeDerecho / adquirientesNoAcreditados;
                
                for (int j = 0; j < adquirientesAcreditado.Count(); j++)
                {
                    if (!adquirientesAcreditado[j])
                    {
                        adquirientesPorcentajeDerecho[j] = parcialPorcentajeDerecho;
                    }
                }
            }
            //PROCESANDO CASOS DE COMPRAVENTA
            if (inscripcion.NaturalezaEscritura == "Compraventa")
            {
                //COMPRAVENTA DE TRANSFERENCIA TOTAL
                if (adquirientesPorcentajeDerecho.Sum() == 100)
                {
                    float transferenciaTotal = 0;
                    for (int i = 0; i < enajenantesRut.Count(); i++)
                    {
                        List<MultiPropietario> multipropietariosEnajenantes = _contexto.MultiPropietarios.
                        OrderBy(mp => mp.AnoInscripcion).
                        ThenBy(mp => mp.NumeroInscripcion).
                        Where(mp => mp.RutPropietario == enajenantesRut[i] && mp.AnoVigenciaFinal==null).ToList();
                        for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                        {
                            if (multipropietariosEnajenantes[j].Comuna == inscripcion.Comuna &&
                                multipropietariosEnajenantes[j].Manzana == inscripcion.Manzana &&
                                multipropietariosEnajenantes[j].Predio == inscripcion.Predio)
                            {
                                transferenciaTotal = transferenciaTotal + multipropietariosEnajenantes[j].PorcentajeDerecho;
                            }
                            if (multipropietariosEnajenantes[j].AnoVigenciaInicial == inscripcion.FechaInscripcion.Year)
                            {
                                _contexto.MultiPropietarios.Remove(multipropietariosEnajenantes[j]);
                                break;
                            }
                            else
                            {
                                multipropietariosEnajenantes[j].AnoVigenciaFinal = inscripcion.FechaInscripcion.Year;
                                break;
                            }
                        }
                        
                        for (int k = 0; k < adquirientesPorcentajeDerecho.Count(); k++)
                        {
                            adquirientesPorcentajeDerecho[k] = (float)(transferenciaTotal * (adquirientesPorcentajeDerecho[k] * 0.01));
                        }
                    }
                }
                //COMPRAVENTA DE DERECHOS
                else if (adquirientesPorcentajeDerecho.Sum() < 100 && enajenantesRut.Count() == 1 && adquirientesRut.Count() == 1)
                {
                    List<MultiPropietario> multipropietariosEnajenantes = _contexto.MultiPropietarios.
                    OrderBy(mp => mp.AnoInscripcion).
                    ThenBy(mp => mp.NumeroInscripcion).
                    Where(mp => mp.Comuna == inscripcion.Comuna &&
                                mp.Manzana == inscripcion.Manzana &&
                                mp.Predio == inscripcion.Predio &&
                                mp.AnoVigenciaFinal == null).ToList();

                    //MANEJANDO MULTIPROPIETARIOS
                    for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                    {
                        if(multipropietariosEnajenantes[j].AnoVigenciaInicial == inscripcion.FechaInscripcion.Year)
                        {
                            if (multipropietariosEnajenantes[j].RutPropietario == enajenantesRut[0])
                            {
                                float Derechos = multipropietariosEnajenantes[j].PorcentajeDerecho * (enajenantesPorcentajeDerecho[0] / 100);
                                multipropietariosEnajenantes[j].PorcentajeDerecho = multipropietariosEnajenantes[j].PorcentajeDerecho - Derechos;
                                adquirientesPorcentajeDerecho[0] = Derechos;
                                break;
                            }   
                        }
                        else
                        {
                            MultiPropietario multiPropietarioNuevaVigencia = new MultiPropietario();

                            multiPropietarioNuevaVigencia.RutPropietario = multipropietariosEnajenantes[j].RutPropietario;
                            multiPropietarioNuevaVigencia.PorcentajeDerecho = multipropietariosEnajenantes[j].PorcentajeDerecho;
                            multiPropietarioNuevaVigencia.Fojas = multipropietariosEnajenantes[j].Fojas;
                            multiPropietarioNuevaVigencia.NumeroInscripcion = multipropietariosEnajenantes[j].NumeroInscripcion;
                            multiPropietarioNuevaVigencia.FechaInscripcion = multipropietariosEnajenantes[j].FechaInscripcion;
                            multiPropietarioNuevaVigencia.AnoInscripcion = multipropietariosEnajenantes[j].FechaInscripcion.Year;
                            multiPropietarioNuevaVigencia.AnoVigenciaInicial = inscripcion.FechaInscripcion.Year;
                            multiPropietarioNuevaVigencia.AnoVigenciaFinal = null;
                            multiPropietarioNuevaVigencia.Comuna = inscripcion.Comuna;
                            multiPropietarioNuevaVigencia.Manzana = inscripcion.Manzana;
                            multiPropietarioNuevaVigencia.Predio = inscripcion.Predio;

                            multipropietariosEnajenantes[j].AnoVigenciaFinal = inscripcion.FechaInscripcion.Year - 1;

                            if (multipropietariosEnajenantes[j].RutPropietario != enajenantesRut[0])
                            {
                                _contexto.MultiPropietarios.AddRange(multiPropietarioNuevaVigencia);
                            }
                            else
                            {
                                float Derechos = multiPropietarioNuevaVigencia.PorcentajeDerecho * (enajenantesPorcentajeDerecho[0] / 100);
                                multiPropietarioNuevaVigencia.PorcentajeDerecho = multiPropietarioNuevaVigencia.PorcentajeDerecho - Derechos;
                                _contexto.MultiPropietarios.AddRange(multiPropietarioNuevaVigencia);
                                adquirientesPorcentajeDerecho[0] = Derechos;
                            }
                        }
                    }
                }
                //COMPRAVENTA DE DOMINIOS
                else
                {
                    List<MultiPropietario> multipropietariosEnajenantes = _contexto.MultiPropietarios.
                    OrderBy(mp => mp.AnoInscripcion).
                    ThenBy(mp => mp.NumeroInscripcion).
                    Where(mp => mp.Comuna == inscripcion.Comuna &&
                                mp.Manzana == inscripcion.Manzana &&
                                mp.Predio == inscripcion.Predio &&
                                mp.AnoVigenciaFinal == null).ToList();
                    
                    for (int i = 0; i < enajenantesRut.Count(); i++)
                    {
                        for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                        {
                            float Dominios = enajenantesPorcentajeDerecho[i];
                            if (multipropietariosEnajenantes[j].AnoVigenciaInicial == inscripcion.FechaInscripcion.Year)
                            {

                                if (multipropietariosEnajenantes[j].RutPropietario == enajenantesRut[i])
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
                            else
                            {

                                MultiPropietario multiPropietarioNuevaVigencia = new MultiPropietario();

                                multiPropietarioNuevaVigencia.RutPropietario = multipropietariosEnajenantes[j].RutPropietario;
                                multiPropietarioNuevaVigencia.PorcentajeDerecho = multipropietariosEnajenantes[j].PorcentajeDerecho;
                                multiPropietarioNuevaVigencia.Fojas = multipropietariosEnajenantes[j].Fojas;
                                multiPropietarioNuevaVigencia.NumeroInscripcion = multipropietariosEnajenantes[j].NumeroInscripcion;
                                multiPropietarioNuevaVigencia.FechaInscripcion = multipropietariosEnajenantes[j].FechaInscripcion;
                                multiPropietarioNuevaVigencia.AnoInscripcion = multipropietariosEnajenantes[j].FechaInscripcion.Year;
                                multiPropietarioNuevaVigencia.AnoVigenciaInicial = inscripcion.FechaInscripcion.Year;
                                multiPropietarioNuevaVigencia.AnoVigenciaFinal = null;
                                multiPropietarioNuevaVigencia.Comuna = inscripcion.Comuna;
                                multiPropietarioNuevaVigencia.Manzana = inscripcion.Manzana;
                                multiPropietarioNuevaVigencia.Predio = inscripcion.Predio;

                                multipropietariosEnajenantes[j].AnoVigenciaFinal = inscripcion.FechaInscripcion.Year - 1;

                                if (multipropietariosEnajenantes[j].RutPropietario != enajenantesRut[i])
                                {
                                    _contexto.MultiPropietarios.AddRange(multiPropietarioNuevaVigencia);
                                }
                                else
                                {
                                    multiPropietarioNuevaVigencia.PorcentajeDerecho = multiPropietarioNuevaVigencia.PorcentajeDerecho - Dominios;
                                    //MANEJANDO MULTIPROPIETARIOS NEGATIVOS
                                    if (multiPropietarioNuevaVigencia.PorcentajeDerecho < 0)
                                    {
                                        _contexto.MultiPropietarios.AddRange(multiPropietarioNuevaVigencia);
                                        _contexto.MultiPropietarios.Remove(multiPropietarioNuevaVigencia);
                                        continue;
                                    }
                                    _contexto.MultiPropietarios.AddRange(multiPropietarioNuevaVigencia);
                                }
                                continue;

                            }
                        }
                    }
                }
            }
            //CREACION DE MULTIPROPIETARIO

            for (int i = 0; i < adquirientesRut.Count(); i++)
            {
                MultiPropietario multipropietario = new MultiPropietario();
                multipropietario.RutPropietario = adquirientesRut[i];
                multipropietario.PorcentajeDerecho = adquirientesPorcentajeDerecho[i];
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
                _contexto.MultiPropietarios.AddRange(multipropietario);

                //MANEJANDO MULTIPROPIETARIOS DUPLICADOS (MERGE)
                List<MultiPropietario> multipropietariosDuplicados = _contexto.MultiPropietarios.
                        OrderBy(mp => mp.AnoInscripcion).
                        ThenBy(mp => mp.NumeroInscripcion).ToList();

                for (int j = 0; j < multipropietariosDuplicados.Count(); j++)
                {
                    if (multipropietariosDuplicados[j].Manzana == multipropietario.Manzana &&
                        multipropietariosDuplicados[j].Comuna == multipropietario.Comuna &&
                        multipropietariosDuplicados[j].Predio == multipropietario.Predio &&
                        multipropietariosDuplicados[j].RutPropietario == multipropietario.RutPropietario &&
                        multipropietariosDuplicados[j].AnoVigenciaFinal == null
                        )
                    {
                        if (multipropietario.NumeroInscripcion > multipropietariosDuplicados[j].NumeroInscripcion)
                        {
                            multipropietario.PorcentajeDerecho += multipropietariosDuplicados[j].PorcentajeDerecho;
                            _contexto.MultiPropietarios.Remove(multipropietariosDuplicados[j]);
                        }
                        else
                        {
                            multipropietariosDuplicados[j].PorcentajeDerecho += multipropietario.PorcentajeDerecho;
                            _contexto.MultiPropietarios.Remove(multipropietario);
                        }
                    }
                }
            }
            _contexto.SaveChanges();
            //MANEJANDO MULTIPROPIETARIOS ERRONEOS
            List<MultiPropietario> multipropietariosActivos = _contexto.MultiPropietarios.
                        Where(mp => mp.Manzana == inscripcion.Manzana &&
                                mp.Comuna == inscripcion.Comuna &&
                                mp.Predio == inscripcion.Predio &&
                                mp.AnoVigenciaFinal == null).ToList();

            float porcentajeDerechoTotal = 0;

            for (int i = 0; i < multipropietariosActivos.Count(); i++)
            {
                porcentajeDerechoTotal += multipropietariosActivos[i].PorcentajeDerecho;
            }

            if (porcentajeDerechoTotal > 100)
            {
                float Ponderado = 100 / porcentajeDerechoTotal;

                for (int i = 0; i < multipropietariosActivos.Count(); i++)
                {
                    multipropietariosActivos[i].PorcentajeDerecho = multipropietariosActivos[i].PorcentajeDerecho * Ponderado;
                }
            }
            else if (porcentajeDerechoTotal < 100)
            {
                int PropietariosNulos = multipropietariosActivos.Where(mp => mp.PorcentajeDerecho == 0).Count();

                float Diferencia = (100 - porcentajeDerechoTotal) / PropietariosNulos;

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


            float sumaPorcAdquirientes = listaAdquirientes.Sum(p => p.PorcentajeDerecho);
            float sumaPorcEnajenantes = listaEnajenantes.Sum(p => p.PorcentajeDerecho);

            if (sumaPorcAdquirientes > 100)
            {
                ModelState.AddModelError(string.Empty, "La Suma de los porcentajes de derecho para los Adquirientes no puede pasar el 100%");
                ViewBag.Comunas = _contexto.Comunas;
                return View();
            }

            if (sumaPorcEnajenantes > 100)
            {
                ModelState.AddModelError(string.Empty, "La Suma de los porcentajes de derecho para los Enajenantes no puede pasar el 100%");
                //SE OBTIENEN LAS COMUNAS PARA EL COMONBOX
                ViewBag.Comunas = _contexto.Comunas;
                return View();
            }

            if (inscripcion.NaturalezaEscritura == "RegularizacionPatrimonio" && listaEnajenantes.Count() > 0)
            {
                ModelState.AddModelError(string.Empty, "Regularizacion de Patrimonio no usa Enajenantes");
                //SE OBTIENEN LAS COMUNAS PARA EL COMONBOX
                ViewBag.Comunas = _contexto.Comunas;
                return View();
            }

            _contexto.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
