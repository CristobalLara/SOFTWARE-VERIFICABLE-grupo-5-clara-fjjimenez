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

            context.SaveChanges();
        }
    }
}