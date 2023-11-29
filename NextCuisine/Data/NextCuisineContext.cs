using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NextCuisine.Models;

namespace NextCuisine.Data
{
    public class NextCuisineContext : DbContext
    {
        public NextCuisineContext (DbContextOptions<NextCuisineContext> options)
            : base(options)
        {
        }

        public DbSet<NextCuisine.Models.Guest> Guest { get; set; } = default!;
    }
}
