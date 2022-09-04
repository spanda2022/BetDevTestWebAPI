using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BetDevTestClassLibrary.Model;
using BetDevTestAPI.Services;

namespace BetDevTestAPI.Data
{


    public partial class ApplicationDBContext : IdentityDbContext<Users>

    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {

        }
       
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<CartProduct> CartProducts { get; set; }
        public virtual DbSet<OrderHistory> OrderHistory { get; set; }
        public virtual DbSet<Product> Products { get; set; }

    }
}
