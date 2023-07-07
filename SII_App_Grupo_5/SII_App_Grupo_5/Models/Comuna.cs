using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SII_App_Grupo_5.Models
{
    public class Comuna
    {
        [Key]
        public int Id { get; set; }
        public String Nombre { get; set; }

    }
}
