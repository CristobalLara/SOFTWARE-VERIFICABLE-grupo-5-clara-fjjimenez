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
        public string Propietario { get; set; }
        public int PorcentajeDerecho { get; set; }
        public string Fojas { get; set; }
        public int AnoInscripcion { get; set; }
        public int NumeroInscripcion { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public int AnoVigenciaInicial { get; set; }
        [DisplayFormat(NullDisplayText = "no tiene")]
        public int? AnoVigenciaFinal { get; set; }
    }
}
