using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NemesysZ2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NemesysZ2.Models.Interfaces;
using System.IO;
using NemesysZ2.Models.Repositories;
using NemesysZ2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace NemesysZ2.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly INemesysRepository _bloggyRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public BlogPostController(INemesysRepository blogRepository, UserManager<IdentityUser> userManager)
        {
            _bloggyRepository = blogRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var model = new ReportsListViewModel()
            {
                TotalEntries = _bloggyRepository.GetAllBlogPosts().Count(),
                BlogPosts = _bloggyRepository
                .GetAllBlogPosts()
                .OrderByDescending(b => b.CreatedDate)
                .Select(b => new ReportsViewModel
                {
                    Id = b.Id,
                    CreatedDate = b.CreatedDate,
                    Content = b.Content,
                    ImageUrl = b.ImageUrl,
                    ReadCount = b.ReadCount,
                    Title = b.Title,
                    Category = new CategoryViewModel()
                    {
                        Id = b.Category.Id,
                        Name = b.Category.Name
                    },
                    Author = new AuthorViewModel()
                    {
                        Id = b.UserId,
                        Name = (_userManager.FindByIdAsync(b.UserId).Result != null) ? _userManager.FindByIdAsync(b.UserId).Result.UserName : "Anonymous"
                    }


                })
            };

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var post = _bloggyRepository.GetBlogPostById(id);
            if (post == null)
                return NotFound();
            else
            {
                var model = new ReportsViewModel()
                {
                    Id = post.Id,
                    CreatedDate = post.CreatedDate,
                    ImageUrl = post.ImageUrl,
                    ReadCount = post.ReadCount,
                    Title = post.Title,
                    Content = post.Content,
                    Category = new CategoryViewModel()
                    {
                        Id = post.Category.Id,
                        Name = post.Category.Name
                    },
                    Author = new AuthorViewModel()
                    {
                        Id = post.UserId,
                        Name = (_userManager.FindByIdAsync(post.UserId).Result != null) ? _userManager.FindByIdAsync(post.UserId).Result.UserName : "Anonymous"
                    }
                };

                return View(model);
            }

        }

        [HttpGet]
 
        public IActionResult Create()
        {
            //Load all categories and create a list of CategoryViewModel
            var categoryList = _bloggyRepository.GetAllCategories().Select(c => new CategoryViewModel()
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            //Pass the list into an EditBlogPostViewModel, which is used by the View (all other properties may be left blank, unless you want to add other default values
            var model = new EditBlogPostViewModel()
            {
                CategoryList = categoryList
            };

            //Pass model to View
            return View(model);
        }

        [HttpPost]
   
        public IActionResult Create([Bind("Title, Content, ImageToUpload, CategoryId")] EditBlogPostViewModel newBlogPost)
        {
            if (ModelState.IsValid)
            {
                string fileName = "";
                if (newBlogPost.ImageToUpload != null)
                {
                    //At this point you should check size, extension etc...
                    //Then persist using a new name for consistency (e.g. new Guid)
                    var extension = "." + newBlogPost.ImageToUpload.FileName.Split('.')[newBlogPost.ImageToUpload.FileName.Split('.').Length - 1];
                    fileName = Guid.NewGuid().ToString() + extension;
                    var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\blogposts\\" + fileName;
                    using (var bits = new FileStream(path, FileMode.Create))
                    {
                        newBlogPost.ImageToUpload.CopyTo(bits);
                    }
                }

                BlogPost blogPost = new BlogPost()
                {
                    Title = newBlogPost.Title,
                    Content = newBlogPost.Content,
                    CreatedDate = DateTime.UtcNow,
                    ImageUrl = "/images/blogposts/" + fileName,
                    ReadCount = 0,
                    CategoryId = newBlogPost.CategoryId,
                    UserId = _userManager.GetUserId(User)
                };

                _bloggyRepository.CreateBlogPost(blogPost);
                return RedirectToAction("Index");
            }
            else
            {
                //Load all categories and create a list of CategoryViewModel
                var categoryList = _bloggyRepository.GetAllCategories().Select(c => new CategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();

                //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the CategoryId
                newBlogPost.CategoryList = categoryList;

                return View(newBlogPost);
            }
        }

        [HttpGet]
 
        public IActionResult Edit(int id)
        {
            var existingBlogPost = _bloggyRepository.GetBlogPostById(id);
            if (existingBlogPost != null)
            {
                EditBlogPostViewModel model = new EditBlogPostViewModel()
                {
                    Id = existingBlogPost.Id,
                    Title = existingBlogPost.Title,
                    Content = existingBlogPost.Content,
                    ImageUrl = existingBlogPost.ImageUrl,
                    CategoryId = existingBlogPost.CategoryId
                };

                //Load all categories and create a list of CategoryViewModel
                var categoryList = _bloggyRepository.GetAllCategories().Select(c => new CategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();

                //Attach to view model - view will pre-select according to the value in CategoryId
                model.CategoryList = categoryList;

                return View(model);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
    
        public IActionResult Edit([FromRoute] int id, [Bind("Id, Title, Content, ImageToUpload, CategoryId")] EditBlogPostViewModel updatedBlogPost)
        {
            var modelToUpdate = _bloggyRepository.GetBlogPostById(id);
            if (modelToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string imageUrl = "";

                if (updatedBlogPost.ImageToUpload != null)
                {
                    string fileName = "";
                    //At this point you should check size, extension etc...
                    //Then persist using a new name for consistency (e.g. new Guid)
                    var extension = "." + updatedBlogPost.ImageToUpload.FileName.Split('.')[updatedBlogPost.ImageToUpload.FileName.Split('.').Length - 1];
                    fileName = Guid.NewGuid().ToString() + extension;
                    var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\blogposts\\" + fileName;
                    using (var bits = new FileStream(path, FileMode.Create))
                    {
                        updatedBlogPost.ImageToUpload.CopyTo(bits);
                    }
                    imageUrl = "/images/blogposts/" + fileName;
                }
                else
                    imageUrl = modelToUpdate.ImageUrl;

                modelToUpdate.Title = updatedBlogPost.Title;
                modelToUpdate.Content = updatedBlogPost.Content;
                modelToUpdate.ImageUrl = imageUrl;
                modelToUpdate.UpdatedDate = DateTime.Now;
                modelToUpdate.CategoryId = updatedBlogPost.CategoryId;
                modelToUpdate.UserId = _userManager.GetUserId(User);

                _bloggyRepository.UpdateBlogPost(modelToUpdate);
                return RedirectToAction("Index");
            }
            else
            {
                //Load all categories and create a list of CategoryViewModel
                var categoryList = _bloggyRepository.GetAllCategories().Select(c => new CategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();

                //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the CategoryId
                updatedBlogPost.CategoryList = categoryList;

                return View(updatedBlogPost);
            }


        }
    }
}

