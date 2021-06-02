 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NemesysZ2.Models;

namespace NemesysZ2.Models
{
    public class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                context.AddRange
                (
                    new Category()
                    {
                        Name = "Uncategorised"
                    },
                    new Category()
                    {
                        Name = "Comedy"
                    },
                    new Category()
                    {
                        Name = "News"
                    }
                );
                context.SaveChanges();
            }


            if (!context.BlogPosts.Any())
            {
                context.AddRange
                (

                    new BlogPost()
                    {
                        Title = "AGA Today",
                        Content = "Today's AGA is characterized by a series of discussions and debates around ...",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        ImageUrl = "/images/seed1.jpg",
                        CategoryId = 1
                    },
                    new BlogPost()
                    {
                        Title = "Traffic is incredible",
                        Content = "Today's traffic can't be described using words. Only an image can do that ...",
                        CreatedDate = DateTime.UtcNow.AddDays(-1),
                        UpdatedDate = DateTime.UtcNow.AddDays(-1),
                        ImageUrl = "/images/seed2.jpg",
                        CategoryId = 2
                    },
                    new BlogPost()
                    {
                        Title = "When is Spring really starting?",
                        Content = "Clouds clouds all around us. I thought spring started already, but ...",
                        CreatedDate = DateTime.UtcNow.AddDays(-2),
                        UpdatedDate = DateTime.UtcNow.AddDays(-2),
                        ImageUrl = "/images/seed3.jpg",
                        CategoryId = 2
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
