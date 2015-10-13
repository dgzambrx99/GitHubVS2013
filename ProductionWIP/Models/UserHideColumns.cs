using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductionWIP.Models
{
    public class UserHideColumns
    {

        public int UserHideColumnsId { get; set; }

        public int OperatorsId { get; set; }

        public  Operators Operators { get; set; }

        public int ColumnNamesId { get; set; }

        public ColumnNames ColumnNames { get; set; }

    }
}