using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProductionWIP.Models
{
    [Table("HeatTreat")]
    public class HeatTreat
    {
        [Key]
        public int HeatTreatId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]

        public DateTime Date { get; set; }
        //[Required(ErrorMessage="Please enter the Job Card Number")]
        [StringLength(50, ErrorMessage = "The maxlength is 50")]

        public string JobCardNumber { get; set; }
        [Required(ErrorMessage = "Please enter the First Name")]

        public string FirstName { get; set; }
        //[Required(ErrorMessage = "Please enter the Station Number")]


        public string StationNumber { get; set; }
        //[Required(ErrorMessage = "Please enter the Temperature")]

        public string Temperature { get; set; }
        //[Required(ErrorMessage = "Please enter the Load")]

        public string Load { get; set; }

        public bool Image { get; set; }
        public bool Image2 { get; set; }
        public bool Image3 { get; set; }
        public bool Image4 { get; set; }
        public bool Image5 { get; set; }


        public int? OperatorsId { get; set; }

        public Operators Operators { get; set; }
    }
}