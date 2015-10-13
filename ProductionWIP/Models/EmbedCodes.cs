using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProductionWIP.Models
{
    [Table("EmbedCodes")]
    public class EmbedCodes
    {
        [Key]
        public int EmbedCodesId { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string Token { get; set; }
    }
}