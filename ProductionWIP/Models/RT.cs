using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductionWIP.Models
{
    public class RT
    {
    
        public int RTId { get; set; }
        public DateTime LastModified { get; set; }
        public int? OperatorsId { get; set; }
        public Operators Operators { get; set; }
        public string AdminId { get; set; }  
        public ApplicationUser Admin { get; set; }
        public bool? bitDevice { get; set; }



    }
}