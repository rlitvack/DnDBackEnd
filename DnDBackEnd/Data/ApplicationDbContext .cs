using DnDBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace DnDBackEnd.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<Character> Characters { get; set; }
	}
}
