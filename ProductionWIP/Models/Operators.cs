using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProductionWIP.Models
{
    [Table("Operators")]
    public class Operators
    {
        [Key]
        public int OperatorsId { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public string Name { get; set; }
    }
}