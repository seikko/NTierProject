using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.WEBUI.Models
{
    public class AppUserVM
    {
        public AppUser AppUser { get; set; }

        public  UserProfile Profile { get; set; }


    }
}