using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Til_IOT.Database
{
    /// <summary>
    /// Database contact for LoginModel
    /// </summary>
    public class LoginDBContext : IdentityDbContext
    {
        

        private static string GetConnectionString()
        {
            return ConnectionString;
        }

        private static string ConnectionString { get; set; }

        public LoginDBContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
            //            ConnectionString = connectionString;
        }
    }
}
