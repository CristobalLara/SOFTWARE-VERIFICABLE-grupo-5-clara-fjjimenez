using NuGet.Protocol.Plugins;
using SII_App_Grupo_5.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SII_App_Grupo_5.Data
{
    public static class DbInitializer
    {
        public static void Initialize(InscriptionsGrupo5DbContext context)
        {
            if (context.Personas.Any())
            {
                return;   // DB has been seeded
            }
            else if (context.Inscripciones.Any())
            {
                return;   // DB has been seeded
            }


            var cristo = new Persona
            {
                RUN = 191894359,
                Nombre = "Cristo"
            };
            context.Personas.AddRange(cristo);

            var francisco = new Persona
            {
                RUN = 191341171,
                Nombre = "Francisco"
            };
            context.Personas.AddRange(francisco);

            var propiedad = new Inscripcion
            {
                Folio = 1,
                NaturalezaEscritura = "Compraventa",
                Comuna = "Macul",
                Manzana = 1,
                Predio = "CuadroVerde",
                Personas = new List<Persona> { cristo },
                Fojas = "Foja 1",
                NumeroInscripcion = 1,
                FechaInscripcion = DateTime.Now
            };
            context.Inscripciones.AddRange(propiedad);

            var multiPropietario = new MultiPropietario
            {
                Comuna = "Vitacura",
                Manzana = 131,
                Predio = 051,
                Propietario = "19134117-1",
                PorcentajeDerecho = 20,
                Fojas = "Foja 1",
                AnoInscripcion = 2023,
                FechaInscripcion = DateTime.Now,
                NumeroInscripcion = 1,
                AnoVigenciaInicial = 2023
            };
            context.MultiPropietarios.AddRange(multiPropietario);

            context.SaveChanges();
        }
    }
}