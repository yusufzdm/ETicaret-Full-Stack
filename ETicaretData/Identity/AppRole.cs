﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretData.Identity
{
    public class AppRole:IdentityRole<int>
    {
        public AppRole():base()
        {
            
        }

        public AppRole(string roleName) : base(roleName)
        {
        }

    }
}
