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
                Comuna = "Santiago",
                Manzana = 1,
                Predio = 1,
                RutPropietario = "1",
                PorcentajeDerecho = 100,
                Fojas = 1,
                AnoInscripcion = 2023,
                FechaInscripcion = DateTime.Now,
                NumeroInscripcion = 1,
                AnoVigenciaInicial = 2023
            };
            context.MultiPropietarios.AddRange(multiPropietario);

            List<String> ListaComunas = new List<String>()
                    {
                        //Región Metropolitana
                        "Santiago", "Cerrillos", "Cerro Navia", "Conchalí", "El Bosque", "Estación Central", "Huechuraba", "Independencia",
                        "La Cisterna", "La Florida", "La Granja", "La Pintana", "La Reina", "Las Condes", "Lo Barnechea", "Lo Espejo","Lo Prado",
                        "Macul", "Maipú", "Ñuñoa", "Pedro Aguirre Cerda", "Peñalolén", "Providencia", "Pudahuel", "Quilicura", "Quinta Normal",
                        "Recoleta", "Renca", "San Joaquín", "San Miguel", "San Ramón", "Vitacura",

                        //Región de Valparaíso
                        "Valparaíso", "Casablanca", "Concón", "Juan Fernández", "Puchuncaví", "Quintero", "Viña del Mar", "Isla de Pascua",
                        "Los Andes", "Calle Larga", "Rinconada", "San Esteban", "La Ligua", "Cabildo", "Papudo", "Petorca", "Zapallar",
                        "Quillota", "Calera", "Hijuelas", "La Cruz","Nogales", "San Antonio", "Algarrobo", "Cartagena", "El Quisco", "El Tabo",
                        "Santo Domingo", "San Felipe", "Catemu", "Llaillay", "Panquehue", "Putaendo", "Santa María", "Quilpué", "Limache",
                        "Olmué",

                        //Resto de comunas no se agregan de momento
                    };

            foreach (String comunaNombre in ListaComunas)
            {
                var comuna = new Comuna
                {
                    Nombre = comunaNombre,
                };
                context.Comunas.AddRange(comuna);
            };


            context.SaveChanges();
        }
    }
}