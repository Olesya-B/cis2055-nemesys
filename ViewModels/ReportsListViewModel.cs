using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NemesysZ2.ViewModels;
using NemesysZ2.Models;


namespace NemesysZ2.ViewModels
{
    public class ReportsListViewModel
    {
        public int TotalEntries {get; set;}
        public IEnumerable<ReportsViewModel> BlogPosts { get; set; }

    }
}
