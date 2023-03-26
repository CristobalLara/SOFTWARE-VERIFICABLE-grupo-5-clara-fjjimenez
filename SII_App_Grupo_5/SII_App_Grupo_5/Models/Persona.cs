﻿using System.ComponentModel.DataAnnotations;

namespace SII_App_Grupo_5.Models
{
    public class Persona
    {
        [Key]
        public int RUN { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        public ICollection<Inscripcion> Inscripciones { get; set; }
    }
}
