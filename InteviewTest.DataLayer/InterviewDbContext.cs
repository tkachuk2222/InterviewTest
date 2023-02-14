using InterviewTest.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewTest.DataLayer
{
    public class InterviewDbContext : DbContext
    {
        public InterviewDbContext(DbContextOptions<InterviewDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}