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
            if (context.Inscripciones.Any())
            {
                return;   // DB has been seeded
            }

            var propiedad = new Inscripcion
            {
                NaturalezaEscritura = "Compraventa",
                Comuna = "Macul",
                Manzana = 1,
                Predio = 2,
                Fojas = 0,
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