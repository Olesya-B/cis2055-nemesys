using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NemesysZ2.Models;

//using Microsoft.Exchange.WebServices.Data;
using NemesysZ2.Models.Interfaces;

namespace NemesysZ2.Models.Repositories
{
    public class NemesysReporisoty : INemesysRepository
    {
        private readonly AppDbContext _appDbContext;

        public NemesysReporisoty(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<BlogPost> GetAllBlogPosts()
        {
            //Using Eager loading with Include
            return _appDbContext.BlogPosts.Include(b => b.Category).OrderBy(b => b.CreatedDate);
        }

        public BlogPost GetBlogPostById(int blogPostId)
        {
            //Using Eager loading with Include
            return _appDbContext.BlogPosts.Include(b => b.Category).FirstOrDefault(p => p.Id == blogPostId);
        }

        public void CreateBlogPost(BlogPost blogPost)
        {
            _appDbContext.BlogPosts.Add(blogPost);
            _appDbContext.SaveChanges();
        }

        public void UpdateBlogPost(BlogPost blogPost)
        {
            var existingBlogPost = _appDbContext.BlogPosts.SingleOrDefault(bp => bp.Id == blogPost.Id);
            if (existingBlogPost != null)
            {
                existingBlogPost.Title = blogPost.Title;
                existingBlogPost.Content = blogPost.Content;
                existingBlogPost.UpdatedDate = blogPost.UpdatedDate;
                existingBlogPost.ImageUrl = blogPost.ImageUrl;
                existingBlogPost.CategoryId = blogPost.CategoryId;
                existingBlogPost.UserId = blogPost.UserId;

                _appDbContext.Entry(existingBlogPost).State = EntityState.Modified;
                _appDbContext.SaveChanges();
            }
        }


        public IEnumerable<Category> GetAllCategories()
        {
            //Not loading related blog posts
            return _appDbContext.Categories;
        }

        public Category GetCategoryById(int categoryId)
        {
            //Not loading related blog posts
            return _appDbContext.Categories.FirstOrDefault(c => c.Id == categoryId);
        }





    }


}