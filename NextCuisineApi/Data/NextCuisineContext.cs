using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NextCuisineApi.Models;

namespace NextCuisine.Data
{
    public class NextCuisineContext : DbContext
    {
        public NextCuisineContext (DbContextOptions<NextCuisineContext> options)
            : base(options)
        {
        }

        public DbSet<Guest> Guest { get; set; } = default!;
    }
}
