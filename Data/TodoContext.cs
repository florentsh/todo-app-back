using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BackTodoApi.Models;

namespace BackTodoApi.Data
{
    public class TodoContext : IdentityDbContext<IdentityUser>
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

        public DbSet<Todo> Todos { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Todo>()
                .HasOne(t => t.User)
                .WithMany()                 
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.SetNull); 
        }
    }
}