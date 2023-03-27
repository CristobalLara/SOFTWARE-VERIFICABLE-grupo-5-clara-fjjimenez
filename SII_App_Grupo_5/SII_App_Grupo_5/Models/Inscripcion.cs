﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SII_App_Grupo_5.Models

{
    public class Inscripcion
    {
        [Key]
        public int Id { get; set; }
        public int Folio { get; set; }
        
        [StringLength(30)]
        public string NaturalezaEscritura { get; set; }

        [Required]
        [StringLength(40)]
        public string Comuna { get; set; }
        [Required]
        public int Manzana { get; set; }
        [Required]
        [StringLength(50)]
        public string Predio { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string Fojas { get; set; }
        public int NumeroInscripcion { get; set; }
        public ICollection<Persona> Personas { get; set; }
    }
}
