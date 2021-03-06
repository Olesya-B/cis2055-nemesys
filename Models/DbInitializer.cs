 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NemesysZ2.Models;

namespace NemesysZ2.Models
{
    public class DbInitializer
    {
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                //Seed Roles
                roleManager.CreateAsync(new IdentityRole("User")).Wait();
                roleManager.CreateAsync(new IdentityRole("Administrator")).Wait();
                roleManager.CreateAsync(new IdentityRole("SuperAdmin")).Wait();
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    Email = "testuser@testmail.com",
                    NormalizedEmail = "TESTUSER@TESTMAIL.COM",
                    UserName = "testuser@testmail.com",
                    NormalizedUserName = "TESTUSER@TESTMAIL.COM",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D") //to track important profile updates (e.g. password change)
                };

                //Add to store
                IdentityResult result = userManager.CreateAsync(user, "Bl0ggyRules!").Result;
                if (result.Succeeded)
                {
                    //Add to role
                    userManager.AddToRoleAsync(user, "User").Wait();
                }


                var admin = new ApplicationUser()
                {
                    Email = "Admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    UserName = "Admin",
                    NormalizedUserName = "ADMIN",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D") //to track important profile updates (e.g. password change)
                };

                //Add to store
                result = userManager.CreateAsync(admin, "Bl0ggyRules!").Result;
                if (result.Succeeded)
                {
                    //Add to role
                    userManager.AddToRoleAsync(admin, "Administrator").Wait();
                }
            }
        }

        public static void SeedData(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                context.AddRange
                (
                    new Category()
                    {
                        Name = "Unsafe Act"
                    },
                    new Category()
                    {
                        Name = "Environmental Dangers"
                    },
                    new Category()
                    {
                        Name = "Exposed Equipement"
                    }, new Category()
                    {
                        Name = "Unsafe Structures"
                    },
                    new Category()
                    {
                        Name = "Other"
                    }
                    
                );
                context.SaveChanges();
            }


            if (!context.BlogPosts.Any())
            {
                var user = userManager.GetUsersInRoleAsync("User").Result.FirstOrDefault();

                context.AddRange
                (


                    new BlogPost()
                    {
                        Title = "AGA Today",
                        Content = "Today's AGA is characterized by a series of discussions and debates around ...",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        ImageUrl = "/images/seed1.jpg",
                        CategoryId = 1,
                        UserId = user.Id
                    },
                    new BlogPost()
                    {
                        Title = "Traffic is incredible",
                        Content = "Today's traffic can't be described using words. Only an image can do that ...",
                        CreatedDate = DateTime.UtcNow.AddDays(-1),
                        UpdatedDate = DateTime.UtcNow.AddDays(-1),
                        ImageUrl = "/images/seed2.jpg",
                        CategoryId = 2,
                        UserId = user.Id
                    },
                    new BlogPost()
                    {
                        Title = "When is Spring really starting?",
                        Content = "Clouds clouds all around us. I thought spring started already, but ...",
                        CreatedDate = DateTime.UtcNow.AddDays(-2),
                        UpdatedDate = DateTime.UtcNow.AddDays(-2),
                        ImageUrl = "/images/seed3.jpg",
                        CategoryId = 2,
                        UserId = user.Id
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
