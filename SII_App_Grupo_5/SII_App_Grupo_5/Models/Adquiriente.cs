using System.ComponentModel.DataAnnotations;

namespace SII_App_Grupo_5.Models
{
    public class Adquiriente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int InscripcionId { get; set; }
        virtual public Inscripcion Inscripcion { get; set; }
        public int Rut { get; set; }
        public int PorcentajeDerecho { get; set; }
        public bool Acreditado { get; set; }
    }
}
