using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ProductionWIP.Models
{
    public partial class WIPDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<HeatTreat> HeatTreat { get; set; }
        public DbSet<Operators> Operators { get; set; }
        public DbSet<EmbedCodes> EmbedCodes { get; set; }
        public DbSet<ColumnNames> ColumnNames { get; set; }

        public DbSet<UserHideColumns> UserHideColumns { get; set; }
        public DbSet<RT> RT { get; set; }



    }

}
