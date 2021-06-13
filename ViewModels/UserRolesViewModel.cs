using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NemesysZ2.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
       
    }
}
