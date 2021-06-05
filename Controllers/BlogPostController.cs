using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NemesysZ2.Models;
using NemesysZ2.Models.Interfaces;
using NemesysZ2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace NemesysZ2.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly INemesysRepository _bloggyRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public BlogPostController(INemesysRepository blogRepository, UserManager<ApplicationUser> userManager)
        {
            _bloggyRepository = blogRepository;
            _userManager = userManager;
        }

        [Authorize]
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
                    Status = b.Status,

                    Category = new CategoryViewModel()
                    {
                        Id = b.Category.Id,
                        Name = b.Category.Name
                    },
                    Author = new AuthorViewModel()
                    {
                        Id = b.UserId,
                        Email = (_userManager.FindByIdAsync(b.UserId).Result != null) ? _userManager.FindByIdAsync(b.UserId).Result.Email : "Anonymous",
                        Name = (_userManager.FindByIdAsync(b.UserId).Result != null) ? _userManager.FindByIdAsync(b.UserId).Result.UserName : "Anonymous"
                    },
                    DateSpotted = b.DateSpotted,
                    Location = b.Location


                })
            };

            return View(model);
        }

        [Authorize]
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
                        Email = (_userManager.FindByIdAsync(post.UserId).Result != null) ? _userManager.FindByIdAsync(post.UserId).Result.Email : "Anonymous",
                        Name = (_userManager.FindByIdAsync(post.UserId).Result != null) ? _userManager.FindByIdAsync(post.UserId).Result.UserName : "Anonymous"
                    },
                    DateSpotted = post.DateSpotted,
                    Location = post.Location,

                };

                return View(model);
            }

        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
        public IActionResult Create([Bind("Title, Content, ImageToUpload, ReadCount, CategoryId, DateSpotted, Location")] EditBlogPostViewModel newBlogPost)
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
                    UserId = _userManager.GetUserId(User),
                    DateSpotted = newBlogPost.DateSpotted,
                    Location = newBlogPost.Location

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
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var existingBlogPost = _bloggyRepository.GetBlogPostById(id);
            if (existingBlogPost != null)
            {

                var currentUser = await _userManager.GetUserAsync(User);
                bool currRole = await _userManager.IsInRoleAsync(currentUser, "Administrator");

                if (existingBlogPost.User.Id == currentUser.Id || currRole == true)
                {
                    if (currRole == false)
                    {
                        return Unauthorized();
                    }
                    EditBlogPostViewModel model = new EditBlogPostViewModel()
                    {
                        Id = existingBlogPost.Id,
                        Title = existingBlogPost.Title,
                        Content = existingBlogPost.Content,
                        ImageUrl = existingBlogPost.ImageUrl,
                        CategoryId = existingBlogPost.CategoryId,
                        DateSpotted = existingBlogPost.DateSpotted,
                        Location = existingBlogPost.Location

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
                    return Unauthorized();
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("Id, Title, Content, ImageToUpload, CategoryId, DateSpotted, Location")] EditBlogPostViewModel updatedBlogPost)
        {
            var modelToUpdate = _bloggyRepository.GetBlogPostById(id);
            if (modelToUpdate == null)
            {
                return NotFound();
            }





            //Check if the current user has access to this resource
            var currentUser = await _userManager.GetUserAsync(User);
            bool currRole = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            if (modelToUpdate.User.Id == currentUser.Id || currRole == true)
            {
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
                    modelToUpdate.DateSpotted = updatedBlogPost.DateSpotted;
                    modelToUpdate.Location = updatedBlogPost.Location;


                    _bloggyRepository.UpdateBlogPost(modelToUpdate);

                    return RedirectToAction("Index");
                }
                else
                    return Unauthorized(); //or redirect to error controller with 401/403 actions
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



        [HttpGet]

        public async Task<IActionResult> EditStatus(int id)
        {
            var existingBlogPost = _bloggyRepository.GetBlogPostById(id);
            if (existingBlogPost != null)
            {

                var currentUser = await _userManager.GetUserAsync(User);
                bool currRole = await _userManager.IsInRoleAsync(currentUser, "Administrator");

                if (currRole == true)
                {
                    if (currRole == false)
                    {
                        return Unauthorized();
                    }
                    EditBlogPostViewModel model = new EditBlogPostViewModel()
                    {
                        Status = existingBlogPost.Status
                    };

                    return View(model);
                }              
                     
            
                else
                    return Unauthorized();

                //Load all categories and create a list of CategoryViewModel

            }

            else
                return RedirectToAction("Index");
            
          
        }


        [HttpPost]
        public async Task<IActionResult> EditStatus([FromRoute] int id, [Bind("Id, Title, Content, ReadCount, ImageToUpload, CategoryId, DateSpotted, Location, Status")] EditBlogPostViewModel updatedBlogPost)
        {
            var modelToUpdate = _bloggyRepository.GetBlogPostById(id);
            if (modelToUpdate == null)
            {
                return NotFound();
            }

            //Check if the current user has access to this resource
            var currentUser = await _userManager.GetUserAsync(User);
            bool currRole = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            if (currRole == true)
            {
               
                    modelToUpdate.Status = updatedBlogPost.Status;
                    _bloggyRepository.UpdateBlogPost(modelToUpdate);

                    return RedirectToAction("Index");
              
            }
            else
                return Unauthorized();
        }


        //Like Button

       

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> Likes(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var existingBlogPost =  _bloggyRepository.GetBlogPostById(id);
            if (existingBlogPost != null)
            {
                EditBlogPostViewModel _MyVoteModel = new EditBlogPostViewModel();
                _MyVoteModel.ReadCount++;


            };
               
                return View();
                       
        }



        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Dashboard()
        {

                ViewBag.Title = "Bloggy Dashboard";
                
                var model = new BlogDashboardViewModel();
                model.TotalRegisteredUsers = _userManager.Users.Count();
                model.TotalEntries = _bloggyRepository.GetAllBlogPosts().Count();
              
                return View(model);

        }





    }

}

