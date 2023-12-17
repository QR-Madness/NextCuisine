using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NextCuisineApi.Models;

namespace NextCuisineApi.Data
{
    public class NextCuisineApiContext : DbContext
    {
        public NextCuisineApiContext (DbContextOptions<NextCuisineApiContext> options)
            : base(options)
        {
        }

        public DbSet<Guest> Guest { get; set; } = default!;
    }
}
