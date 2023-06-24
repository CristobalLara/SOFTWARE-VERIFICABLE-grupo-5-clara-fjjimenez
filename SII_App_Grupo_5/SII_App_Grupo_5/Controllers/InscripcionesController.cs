﻿using Microsoft.AspNetCore.Mvc;
using SII_App_Grupo_5.Models;
using SII_App_Grupo_5.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        public IActionResult Create(    Inscripcion inscripcion,
                                        string[] adquirientesRut, 
                                        string[] adquirientesPorcentajeDerecho, 
                                        bool[] adquirientesAcreditado,
                                        string[] enajenantesRut, 
                                        string[] enajenantesPorcentajeDerecho, 
                                        bool[] enajenantesAcreditado)
        {

            List<float> adquirientesPorcentajeDerechoFloat = PorcentajeStringAFloat(adquirientesPorcentajeDerecho);
            List<float> enajenantesPorcentajeDerechoFloat = PorcentajeStringAFloat(enajenantesPorcentajeDerecho);
            List<Adquiriente> listaAdquirientes = new List<Adquiriente>();
            List<Enajenante> listaEnajenantes = new List<Enajenante>();

            _contexto.Inscripciones.AddRange(inscripcion);
            _contexto.SaveChanges();

            foreach (string adquirienteRut in adquirientesRut)
            {
                bool valido = ValidaRut(adquirienteRut);
                if (!valido)
                {
                    ModelState.AddModelError(string.Empty, "Alguno de los ruts ingresados no es válido");
                    ViewBag.Comunas = _contexto.Comunas;
                    return View();
                }
            }

            //CREACION DE LA INSCRIPCION
            CreacionAdquirientes(inscripcion, listaAdquirientes, adquirientesRut, adquirientesPorcentajeDerechoFloat, adquirientesAcreditado);
            CreacionEnajenantes(inscripcion, listaEnajenantes, enajenantesRut, enajenantesPorcentajeDerechoFloat, enajenantesAcreditado);

            float totalPorcentajeDerecho = 100;
            int adquirientesNoAcreditados = 0;

            //PROCESANDO REGULARIZACION DE PATRIMONIO
            if (inscripcion.NaturalezaEscritura == "RegularizacionPatrimonio")
            {
                RegularizacionPatrimonio(adquirientesAcreditado, totalPorcentajeDerecho, adquirientesNoAcreditados, adquirientesPorcentajeDerechoFloat);
            }
            //PROCESANDO CASOS DE COMPRAVENTA
            if (inscripcion.NaturalezaEscritura == "Compraventa")
            {
                //COMPRAVENTA DE TRANSFERENCIA TOTAL
                if (adquirientesPorcentajeDerechoFloat.Sum() == 100)
                {
                    CompraventaTransferenciaTotal(inscripcion, adquirientesPorcentajeDerechoFloat, enajenantesRut);
                }
                //COMPRAVENTA DE DERECHOS
                else if (adquirientesPorcentajeDerechoFloat.Sum() < 100 && enajenantesRut.Count() == 1 && adquirientesRut.Count() == 1)
                {
                    CompraventaDerechos(inscripcion, enajenantesRut, adquirientesPorcentajeDerechoFloat, enajenantesPorcentajeDerechoFloat);
                }
                //COMPRAVENTA DE DOMINIOS
                else
                {
                    CompraventaDominios(inscripcion, enajenantesRut, enajenantesPorcentajeDerechoFloat );
                }
            }
            //CREACION DE MULTIPROPIETARIO

            for (int i = 0; i < adquirientesRut.Count(); i++)
            {
                MultiPropietario multipropietario = new MultiPropietario();
                multipropietario.RutPropietario = adquirientesRut[i];
                multipropietario.PorcentajeDerecho = adquirientesPorcentajeDerechoFloat[i];
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
                        else if (multipropietario.NumeroInscripcion == multipropietariosDuplicados[j].NumeroInscripcion && 
                                 multipropietariosDuplicados[j].AnoInscripcion == multipropietario.AnoInscripcion)
                        {
                            if (multipropietario.Id > multipropietariosDuplicados[j].Id)
                                multipropietariosDuplicados[j].AnoVigenciaFinal=multipropietario.AnoVigenciaInicial;
                            else if (multipropietario.Id < multipropietariosDuplicados[j].Id)
                                multipropietario.AnoVigenciaFinal = multipropietariosDuplicados[j].AnoVigenciaInicial - 1;
                        }
                        else if (multipropietario.NumeroInscripcion < multipropietariosDuplicados[j].NumeroInscripcion)
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

        // Funciones Encapsuladoras
        private List<float> PorcentajeStringAFloat(string[] stringList)
        {
            List<float> floatList = new List<float>();
            foreach (string percentage in stringList)
            {
                floatList.Add(float.Parse(percentage.Replace(".", ",")));
            }
            return floatList;
        }

        private void CreacionAdquirientes(   Inscripcion inscripcion, 
                                            List<Adquiriente> listaAdquirientes, 
                                            string[] adquirientesRut, 
                                            List<float> adquirientesPorcentajeDerechoFloat, 
                                            bool[] adquirientesAcreditado)
        {
            for (int i = 0; i < adquirientesRut.Count(); i++)
            {
                Adquiriente adquiriente = new Adquiriente();
                adquiriente.Rut = adquirientesRut[i];
                adquiriente.PorcentajeDerecho = adquirientesPorcentajeDerechoFloat[i];
                adquiriente.Acreditado = adquirientesAcreditado[i];
                adquiriente.InscripcionId = inscripcion.Folio;
                if (!adquiriente.Acreditado)
                {
                    adquiriente.PorcentajeDerecho = 0;
                }
                listaAdquirientes.Add(adquiriente);
                _contexto.Adquirientes.AddRange(adquiriente);
            }
        }

        private void CreacionEnajenantes(   Inscripcion inscripcion,
                                            List<Enajenante> listaEnajenantes,
                                            string[] enajenantesRut,
                                            List<float> enajenantesPorcentajeDerechoFloat,
                                            bool[] enajenantesAcreditado)
        {
            for (int i = 0; i < enajenantesRut.Count(); i++)
            {
                Enajenante enajenante = new Enajenante();
                enajenante.Rut = enajenantesRut[i];
                enajenante.PorcentajeDerecho = enajenantesPorcentajeDerechoFloat[i];
                enajenante.Acreditado = enajenantesAcreditado[i];
                enajenante.InscripcionId = inscripcion.Folio;
                listaEnajenantes.Add(enajenante);
                _contexto.Enajenantes.AddRange(enajenante);
            }
        }
        private void RegularizacionPatrimonio(  bool[] adquirientesAcreditado,
                                                float totalPorcentajeDerecho,
                                                int adquirientesNoAcreditados,
                                                List<float> adquirientesPorcentajeDerechoFloat)
        {
            for (int i = 0; i < adquirientesAcreditado.Count(); i++)
            {
                if (adquirientesAcreditado[i])
                {
                    totalPorcentajeDerecho =- adquirientesPorcentajeDerechoFloat[i];
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
                    adquirientesPorcentajeDerechoFloat[j] = parcialPorcentajeDerecho;
                }
            }
        }

        
        private int ConfirmarFantasma(Inscripcion inscripcion,
                                                    string[] enajenantesRut)
        {
            bool enajenanteEncontrado = false;
            List<MultiPropietario> multipropietariosFantasmas = _contexto.MultiPropietarios.
                OrderBy(mp => mp.AnoInscripcion).
                ThenBy(mp => mp.NumeroInscripcion).
                Where(mp => mp.Comuna == inscripcion.Comuna &&
                            mp.Manzana == inscripcion.Manzana &&
                            mp.Predio == inscripcion.Predio &&
                            mp.AnoVigenciaFinal == null).ToList();

            int numerofantasmas = 0;
            for (int i = 0; i < enajenantesRut.Count(); i++)
            {
                enajenanteEncontrado = false;
                for (int j = 0; j < multipropietariosFantasmas.Count(); j++)
                {
                    if (enajenantesRut[i] == multipropietariosFantasmas[j].RutPropietario)
                    {
                        enajenanteEncontrado = true;
                        break;
                    }
                }
                if (!enajenanteEncontrado)
                {
                    numerofantasmas++;
                }
            }
            return numerofantasmas;
        }
        private void CompraventaTransferenciaTotalFantasmaCasoA(Inscripcion inscripcion,
                                                    List<float> adquirientesPorcentajeDerechoFloat)
        {
            float transferenciaTotal = 0;
            float divisor;

            List<MultiPropietario> multipropietariosEnajenantes = _contexto.MultiPropietarios.
            OrderBy(mp => mp.AnoInscripcion).
            ThenBy(mp => mp.NumeroInscripcion).
            Where(mp => mp.Comuna == inscripcion.Comuna &&
                        mp.Manzana == inscripcion.Manzana &&
                        mp.Predio == inscripcion.Predio &&
                        mp.AnoVigenciaFinal == null).ToList();

            if (multipropietariosEnajenantes[0].AnoVigenciaInicial == inscripcion.FechaInscripcion.Year)
            {
                for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                {
                    transferenciaTotal =+ multipropietariosEnajenantes[j].PorcentajeDerecho;
                }
                for (int k = 0; k < adquirientesPorcentajeDerechoFloat.Count(); k++)
                {
                    transferenciaTotal =+ adquirientesPorcentajeDerechoFloat[k];
                }

                divisor = transferenciaTotal / 100;

                List<MultiPropietario> multipropietariosAfectados = _contexto.MultiPropietarios.
                OrderBy(mp => mp.AnoInscripcion).
                ThenBy(mp => mp.NumeroInscripcion).
                Where(mp => mp.Comuna == inscripcion.Comuna &&
                            mp.Manzana == inscripcion.Manzana &&
                            mp.Predio == inscripcion.Predio &&
                            mp.AnoVigenciaFinal == null).ToList();
                for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                {
                    multipropietariosEnajenantes[j].PorcentajeDerecho = multipropietariosEnajenantes[j].PorcentajeDerecho / divisor;
                }
                for (int k = 0; k < adquirientesPorcentajeDerechoFloat.Count(); k++)
                {
                    adquirientesPorcentajeDerechoFloat[k] = adquirientesPorcentajeDerechoFloat[k] / divisor;
                }
            }
            else
            {
                List<MultiPropietario> multipropietariosAfectados = new List<MultiPropietario>(); ;
                MultiPropietario multiPropietarioNuevaVigencia = new MultiPropietario();
                for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                {
                    transferenciaTotal =+ multipropietariosEnajenantes[j].PorcentajeDerecho;
                    multiPropietarioNuevaVigencia = CrearMultiPropietario(inscripcion, multipropietariosEnajenantes, j);
                    multipropietariosAfectados.Add(multiPropietarioNuevaVigencia);
                }
                for (int k = 0; k < adquirientesPorcentajeDerechoFloat.Count(); k++)
                {
                    transferenciaTotal =+ adquirientesPorcentajeDerechoFloat[k];
                }

                divisor = transferenciaTotal / 100;

                for (int j = 0; j < multipropietariosAfectados.Count(); j++)
                {
                    multipropietariosAfectados[j].PorcentajeDerecho = multipropietariosAfectados[j].PorcentajeDerecho / divisor;
                    _contexto.MultiPropietarios.AddRange(multipropietariosAfectados[j]);
                }
                for (int k = 0; k < adquirientesPorcentajeDerechoFloat.Count(); k++)
                {
                    adquirientesPorcentajeDerechoFloat[k] = adquirientesPorcentajeDerechoFloat[k] / divisor;
                }
            }
        }
        private void CompraventaTransferenciaTotalFantasmaCasoB(Inscripcion inscripcion,
                                                    List<float> adquirientesPorcentajeDerechoFloat,
                                                    string[] enajenantesRut)
        {
            float transferenciaTotal = 0;
            float divisor;

            List<MultiPropietario> multipropietariosEnajenantes = _contexto.MultiPropietarios.
            OrderBy(mp => mp.AnoInscripcion).
            ThenBy(mp => mp.NumeroInscripcion).
            Where(mp => mp.Comuna == inscripcion.Comuna &&
                        mp.Manzana == inscripcion.Manzana &&
                        mp.Predio == inscripcion.Predio &&
                        mp.AnoVigenciaFinal == null).ToList();

            if (multipropietariosEnajenantes[0].AnoVigenciaInicial == inscripcion.FechaInscripcion.Year)
            {
                for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                {
                    transferenciaTotal =+ multipropietariosEnajenantes[j].PorcentajeDerecho;
                }
                for (int k = 0; k < adquirientesPorcentajeDerechoFloat.Count(); k++)
                {
                    transferenciaTotal =+ adquirientesPorcentajeDerechoFloat[k];
                }

                divisor = transferenciaTotal / 100;

                List<MultiPropietario> multipropietariosAfectados = _contexto.MultiPropietarios.
                OrderBy(mp => mp.AnoInscripcion).
                ThenBy(mp => mp.NumeroInscripcion).
                Where(mp => mp.Comuna == inscripcion.Comuna &&
                            mp.Manzana == inscripcion.Manzana &&
                            mp.Predio == inscripcion.Predio &&
                            mp.AnoVigenciaFinal == null).ToList();
                for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                {
                    multipropietariosEnajenantes[j].PorcentajeDerecho = multipropietariosEnajenantes[j].PorcentajeDerecho / divisor;
                }
                for (int k = 0; k < adquirientesPorcentajeDerechoFloat.Count(); k++)
                {
                    adquirientesPorcentajeDerechoFloat[k] = adquirientesPorcentajeDerechoFloat[k] / divisor;
                }
            }
            else
            {
                List<MultiPropietario> multipropietariosAfectados = new List<MultiPropietario>(); ;
                MultiPropietario multiPropietarioNuevaVigencia = new MultiPropietario();
                for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
                {
                    transferenciaTotal =+ multipropietariosEnajenantes[j].PorcentajeDerecho;
                    multiPropietarioNuevaVigencia = CrearMultiPropietario(inscripcion, multipropietariosEnajenantes, j);
                    for (int k = 0; k < enajenantesRut.Count(); k++)
                    {
                        if (multipropietariosEnajenantes[j].RutPropietario == enajenantesRut[k])
                        {
                            multipropietariosAfectados.Add(multiPropietarioNuevaVigencia);
                            break;
                        }
                    }
                }
                for (int k = 0; k < adquirientesPorcentajeDerechoFloat.Count(); k++)
                {
                    transferenciaTotal =+ adquirientesPorcentajeDerechoFloat[k];
                }

                divisor = transferenciaTotal / 100;

                for (int j = 0; j < multipropietariosAfectados.Count(); j++)
                {
                    _contexto.MultiPropietarios.AddRange(multipropietariosAfectados[j]);
                }
                for (int k = 0; k < adquirientesPorcentajeDerechoFloat.Count(); k++)
                {
                    adquirientesPorcentajeDerechoFloat[k] = adquirientesPorcentajeDerechoFloat[k] / divisor;
                }
            }
        }

        private void CompraventaTransferenciaTotal( Inscripcion inscripcion,
                                                    List<float> adquirientesPorcentajeDerechoFloat,
                                                    string[] enajenantesRut)
        {
            int numerofantasmas = ConfirmarFantasma(inscripcion, enajenantesRut);

            if (numerofantasmas>0 && numerofantasmas < enajenantesRut.Count())
            {
                CompraventaTransferenciaTotalFantasmaCasoB(inscripcion, adquirientesPorcentajeDerechoFloat,enajenantesRut);
            }
            else if (numerofantasmas == enajenantesRut.Count())
            {
                CompraventaTransferenciaTotalFantasmaCasoA(inscripcion, adquirientesPorcentajeDerechoFloat);
            }
            else if(numerofantasmas==0)
            {
                List<MultiPropietario> multipropietariosEnajenantes = _contexto.MultiPropietarios.
            OrderBy(mp => mp.AnoInscripcion).
            ThenBy(mp => mp.NumeroInscripcion).
            Where(mp => mp.Comuna == inscripcion.Comuna &&
                        mp.Manzana == inscripcion.Manzana &&
                        mp.Predio == inscripcion.Predio &&
                        mp.AnoVigenciaFinal == null).ToList();

                float transferenciaTotal = 0;
                bool porBorrar = false;
                MultiPropietario multiPropietarioNuevaVigencia = new MultiPropietario();
                for (int i = 0; i < multipropietariosEnajenantes.Count(); i++)
                {
                    for (int j = 0; j < enajenantesRut.Count(); j++)
                    {
                        if (multipropietariosEnajenantes[i].AnoVigenciaInicial == inscripcion.FechaInscripcion.Year)
                        {
                            if (multipropietariosEnajenantes[i].RutPropietario == enajenantesRut[j])
                            {
                                transferenciaTotal =+ multipropietariosEnajenantes[i].PorcentajeDerecho;
                                _contexto.MultiPropietarios.Remove(multipropietariosEnajenantes[i]);
                                break;
                            }
                            else
                            {
                                multipropietariosEnajenantes[i].AnoVigenciaFinal = inscripcion.FechaInscripcion.Year;
                                break;
                            }
                        }
                        else
                        {
                            multiPropietarioNuevaVigencia = CrearMultiPropietario(inscripcion, multipropietariosEnajenantes, i);

                            if (enajenantesRut[j] == multipropietariosEnajenantes[i].RutPropietario)
                            {
                                porBorrar = true;
                            }
                        }
                    }
                    _contexto.MultiPropietarios.AddRange(multiPropietarioNuevaVigencia);
                    if (porBorrar)
                    {
                        transferenciaTotal =+ multiPropietarioNuevaVigencia.PorcentajeDerecho;
                        _contexto.MultiPropietarios.Remove(multiPropietarioNuevaVigencia);
                        porBorrar = false;
                    }
                }
                for (int k = 0; k < adquirientesPorcentajeDerechoFloat.Count(); k++)
                {
                    adquirientesPorcentajeDerechoFloat[k] = (float)(transferenciaTotal * (adquirientesPorcentajeDerechoFloat[k] * 0.01));
                }
            }
        }

        private void CompraventaDerechosFantasma(Inscripcion inscripcion,
                                            string[] enajenantesRut,
                                            List<float> adquirientesPorcentajeDerechoFloat
                                            )
        {

            List<MultiPropietario> multipropietariosEnajenantes = _contexto.MultiPropietarios.
                    OrderBy(mp => mp.AnoInscripcion).
                    ThenBy(mp => mp.NumeroInscripcion).
                    Where(mp => mp.Comuna == inscripcion.Comuna &&
                                mp.Manzana == inscripcion.Manzana &&
                                mp.Predio == inscripcion.Predio &&
                                mp.AnoVigenciaFinal == null).ToList();


            MultiPropietario multiPropietarioNuevaVigencia = new MultiPropietario();

            multiPropietarioNuevaVigencia.RutPropietario = enajenantesRut[0];
            multiPropietarioNuevaVigencia.Fojas = inscripcion.Fojas;
            multiPropietarioNuevaVigencia.NumeroInscripcion = null;
            multiPropietarioNuevaVigencia.PorcentajeDerecho = 100 - adquirientesPorcentajeDerechoFloat[0];
            multiPropietarioNuevaVigencia.FechaInscripcion = inscripcion.FechaInscripcion;
            multiPropietarioNuevaVigencia.AnoInscripcion = null;
            if (inscripcion.FechaInscripcion <= new DateTime(2019, 1, 1))
            {
                multiPropietarioNuevaVigencia.AnoVigenciaInicial = new DateTime(2019, 1, 1).Year;
            }
            else
            {
                multiPropietarioNuevaVigencia.AnoVigenciaInicial = inscripcion.FechaInscripcion.Year;
            }
            multiPropietarioNuevaVigencia.AnoVigenciaFinal = null;
            multiPropietarioNuevaVigencia.Comuna = inscripcion.Comuna;
            multiPropietarioNuevaVigencia.Manzana = inscripcion.Manzana;
            multiPropietarioNuevaVigencia.Predio = inscripcion.Predio;
            _contexto.MultiPropietarios.AddRange(multiPropietarioNuevaVigencia);

            //MANEJANDO MULTIPROPIETARIOS
            for (int j = 0; j < multipropietariosEnajenantes.Count(); j++)
            {
                if (multipropietariosEnajenantes[j].AnoVigenciaInicial == inscripcion.FechaInscripcion.Year)
                {
                    for (int i = 0; i < multipropietariosEnajenantes.Count(); i++)
                    {
                        _contexto.MultiPropietarios.Remove(multipropietariosEnajenantes[i]);
                    }
                    break;
                }
                else
                {
                    for (int i = 0; i < multipropietariosEnajenantes.Count(); i++)
                    {
                        multipropietariosEnajenantes[i].AnoVigenciaFinal = inscripcion.FechaInscripcion.Year-1;
                    }
                    break;
                }
            }
        }
        private void CompraventaDerechos(   Inscripcion inscripcion,
                                            string[] enajenantesRut,
                                            List<float> adquirientesPorcentajeDerechoFloat,
                                            List<float> enajenantesPorcentajeDerechoFloat
                                            ) 
        {
            int numerofantasmas = ConfirmarFantasma(inscripcion, enajenantesRut);
            if (numerofantasmas > 0)
            {
                CompraventaDerechosFantasma(inscripcion, enajenantesRut, adquirientesPorcentajeDerechoFloat);
            }
            else
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
                    if (multipropietariosEnajenantes[j].AnoVigenciaInicial == inscripcion.FechaInscripcion.Year)
                    {
                        if (multipropietariosEnajenantes[j].RutPropietario == enajenantesRut[0])
                        {
                            float Derechos = multipropietariosEnajenantes[j].PorcentajeDerecho * (enajenantesPorcentajeDerechoFloat[0] / 100);
                            multipropietariosEnajenantes[j].PorcentajeDerecho = multipropietariosEnajenantes[j].PorcentajeDerecho - Derechos;
                            adquirientesPorcentajeDerechoFloat[0] = Derechos;
                            break;
                        }
                    }
                    else
                    {
                        MultiPropietario multiPropietarioNuevaVigencia = CrearMultiPropietario(inscripcion, multipropietariosEnajenantes, j);

                        if (multipropietariosEnajenantes[j].RutPropietario != enajenantesRut[0])
                        {
                            _contexto.MultiPropietarios.AddRange(multiPropietarioNuevaVigencia);
                        }
                        else
                        {
                            float Derechos = multiPropietarioNuevaVigencia.PorcentajeDerecho * (enajenantesPorcentajeDerechoFloat[0] / 100);
                            multiPropietarioNuevaVigencia.PorcentajeDerecho =- Derechos;
                            _contexto.MultiPropietarios.AddRange(multiPropietarioNuevaVigencia);
                            adquirientesPorcentajeDerechoFloat[0] = Derechos;
                        }
                    }
                }
            }
        }
        private void CompraventaDominios(   Inscripcion inscripcion,
                                            string[] enajenantesRut,
                                            List<float> enajenantesPorcentajeDerechoFloat
                                            )
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
                    float Dominios = enajenantesPorcentajeDerechoFloat[i];
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
                        MultiPropietario multiPropietarioNuevaVigencia = CrearMultiPropietario(inscripcion, multipropietariosEnajenantes, j);

                        if (multipropietariosEnajenantes[j].RutPropietario != enajenantesRut[i])
                        {
                            _contexto.MultiPropietarios.AddRange(multiPropietarioNuevaVigencia);
                        }
                        else
                        {
                            multiPropietarioNuevaVigencia.PorcentajeDerecho =- Dominios;
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
        private MultiPropietario CrearMultiPropietario(Inscripcion inscripcion ,List<MultiPropietario> multipropietariosEnajenantes, int posicion)
        {
            MultiPropietario multiPropietarioNuevaVigencia = new MultiPropietario();

            multiPropietarioNuevaVigencia.RutPropietario = multipropietariosEnajenantes[posicion].RutPropietario;
            multiPropietarioNuevaVigencia.PorcentajeDerecho = multipropietariosEnajenantes[posicion].PorcentajeDerecho;
            multiPropietarioNuevaVigencia.Fojas = multipropietariosEnajenantes[posicion].Fojas;
            multiPropietarioNuevaVigencia.NumeroInscripcion = multipropietariosEnajenantes[posicion].NumeroInscripcion;
            multiPropietarioNuevaVigencia.FechaInscripcion = multipropietariosEnajenantes[posicion].FechaInscripcion;
            multiPropietarioNuevaVigencia.AnoInscripcion = multipropietariosEnajenantes[posicion].FechaInscripcion.Year;
            multiPropietarioNuevaVigencia.AnoVigenciaInicial = inscripcion.FechaInscripcion.Year;
            multiPropietarioNuevaVigencia.AnoVigenciaFinal = null;
            multiPropietarioNuevaVigencia.Comuna = inscripcion.Comuna;
            multiPropietarioNuevaVigencia.Manzana = inscripcion.Manzana;
            multiPropietarioNuevaVigencia.Predio = inscripcion.Predio;
            multipropietariosEnajenantes[posicion].AnoVigenciaFinal = inscripcion.FechaInscripcion.Year - 1;
            return multiPropietarioNuevaVigencia;
        }
        private bool ValidaRut(string rut)
        {
            //Basado en: https://gist.github.com/donpandix/045f865c3bf800893036
            string rutTemporal = rut;
            rutTemporal = rutTemporal.Replace(".", "").ToUpper();

            Regex expresion = new Regex("^([0-9]+-[0-9K])$");
            if (!expresion.IsMatch(rutTemporal))
            {
                return false;
            }
            rutTemporal = rutTemporal.Replace("-", "");

            List<string> rutEntero = new List<string>();
            foreach (char digito in rutTemporal)
            {
                rutEntero.Insert(0, digito.ToString());
            }
            string digitoVerificador = rutEntero[0];
            rutEntero.RemoveAt(0);

            int sumaTotal = 0;
            int multiplicador = 2;
            foreach (string digito in rutEntero)
            {
                int digitoInt = Int32.Parse(digito);
                sumaTotal += digitoInt * multiplicador;
                multiplicador++;
                if (multiplicador > 7) { multiplicador = 2; }
            }

            string digitoVerificadorEsperado = (11 - (sumaTotal % 11)).ToString();
            if (digitoVerificadorEsperado == "10") { digitoVerificadorEsperado = "K"; }
            else if (digitoVerificadorEsperado == "11") { digitoVerificadorEsperado = "0"; }

            if (digitoVerificadorEsperado == digitoVerificador) { return true; }
            else { return false; }
        }
    }
}

