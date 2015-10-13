using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProductionWIP.Models
{
    [Table("ColumnNames")]
    public class ColumnNames
    {
        [Key]
        public int ColumnNamesId { get; set; }
       
        public int Order { get; set; }
        [Required]
        public string Label { get; set; }
    }
}