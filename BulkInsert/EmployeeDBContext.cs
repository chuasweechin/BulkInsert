using Microsoft.EntityFrameworkCore;

namespace BulkInsert
{
	public class EmployeeDbContext : DbContext
	{
		public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) {}
		public DbSet<Employee> Employees { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder) {}
	}
}