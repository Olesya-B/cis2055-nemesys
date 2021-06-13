using NemesysZ2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NemesysZ2.ViewModels
{
    public class ReportsViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string ImageUrl { get; set; }
        public CategoryViewModel Category { get; set; }
        public AuthorViewModel Author { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}", ApplyFormatInEditMode = true)] 
        public DateTime DateSpotted { get; set; }

        public string Location { get; set; }

        public string Status { get; set; }
        public int NoOfLikes { get; set; }
        public List<Comments> Comments { get; set; }


    }

}
