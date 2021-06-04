using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NemesysZ2.Models;

namespace NemesysZ2.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }

        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public int ReadCount { get; set; }
        //Ref for navigation 
        public ApplicationUser User {get; set;}
        public string UserId { get; set; }

        public DateTime UpdatedDate { get; internal set; }

    }
}
