using System.ComponentModel.DataAnnotations;

namespace SII_App_Grupo_5.Models
{
    public class MultiPropietario
    {
        [Key]
        public int Id { get; set; }
        public String Comuna { get; set; }
        public int Manzana { get; set; }
        public int Predio { get; set; }
        public string RutPropietario { get; set; }
        public float PorcentajeDerecho { get; set; }
        public int Fojas { get; set; }
        public int AnoInscripcion { get; set; }
        public int NumeroInscripcion { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public int AnoVigenciaInicial { get; set; }
        [DisplayFormat(NullDisplayText = "no tiene")]
        public int? AnoVigenciaFinal { get; set; }
    }
}
