﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();

        private string roleList;

        public string RoleList
        {
            get 
            {
                return string.Join(", ", Roles.Select(x=> x.Value)); 
            }
        }

    }
}
