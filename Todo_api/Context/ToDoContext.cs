using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Todo_api.Models;

namespace Todo_api.Context
{
    public class ToDoContext : IdentityDbContext<AplicationUser>
    {
        public ToDoContext(DbContextOptions options) : base(options) { }
        public DbSet<TodoTask<string>> Tasks { get; set; }
       
    }
}
