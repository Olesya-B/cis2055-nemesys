using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace NemesysZ2.Models
{
    public class ApplicationUser:IdentityUser
    {
        [PersonalData]
        public string FullName { get; set; }
        public byte[] ProfilePic { get; set; }
        public int UsernameChangeLimit { get; set; } = 5;
    }
}
