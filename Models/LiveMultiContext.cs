
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    public class LiveMultiContext:DbContext
    {
        public LiveMultiContext(DbContextOptions<LiveMultiContext> options) :base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<RecordVideo> RecordVideos { get; set; }
        public DbSet<Comment> Comments { get; set; }

    }
}
