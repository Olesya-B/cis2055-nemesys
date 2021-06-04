using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NemesysZ2.Models;

namespace NemesysZ2.Models.Interfaces
{
    public interface INemesysRepository
    {
        IEnumerable<BlogPost> GetAllBlogPosts();
        BlogPost GetBlogPostById(int blogPostId);

        void CreateBlogPost(BlogPost newBlogPost);

        void UpdateBlogPost(BlogPost updatedBlogPost);

        IEnumerable<Category> GetAllCategories();
        Category GetCategoryById(int categoryId);
    }
}
