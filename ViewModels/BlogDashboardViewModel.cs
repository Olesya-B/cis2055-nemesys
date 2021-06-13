using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace NemesysZ2.ViewModels
{
    public class BlogDashboardViewModel
    {
        [DisplayName("Total Blog Posts")]
        public int TotalEntries { get; set; }

        [DisplayName("Total Registered Users")]
        public int TotalRegisteredUsers { get; set; }

    }

}
