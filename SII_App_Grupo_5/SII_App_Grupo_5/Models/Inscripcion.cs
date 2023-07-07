using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SII_App_Grupo_5.Models

{
    public class Inscripcion
    {
        [Key]
        public int Folio { get; set; }
        
        [StringLength(30)]
        public string NaturalezaEscritura { get; set; }

        [Required]
        [StringLength(40)]
        public string Comuna { get; set; }
        [Required]
        public int Manzana { get; set; }
        [Required]
        public int Predio { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public int Fojas { get; set; }
        public int NumeroInscripcion { get; set; }
        virtual public List<Enajenante> Enajenantes { get; set; }
        virtual public List<Adquiriente> Adquirientes { get; set; }
    }
}
