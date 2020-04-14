using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// Test Setup
// Local Machine Specs: Macbook Air with 1.6 GHz Dual-Core Intel Core i5, 8GB 2133 MHz LPDDR3
// MS SQL 2019 on Docker on the same local machine executing this program
// Postgres 11 installed on the same local machine executing this program
// Employee Record has 4 string fields
namespace BulkInsert
{
  class Program
  {
		static void Main(string[] args)
		{
			Stopwatch stopwatch = new Stopwatch();
			ServiceCollection serviceCollection = new ServiceCollection();

			//serviceCollection.AddDbContext<EmployeeDbContext>(options => {
			//	options.UseNpgsql("Host=localhost;Port=5432;Database=employeedb;Username=chuasweechin");
			//});

			serviceCollection.AddDbContextPool<EmployeeDbContext>(options =>
				options.UseSqlServer("Server=localhost,1433\\Catalog=Employee;Database=EmployeeDB;User=sa;Password=Winter2019")
			);

			var serviceProvider = serviceCollection.BuildServiceProvider();
			EmployeeDbContext context = serviceProvider.GetService<EmployeeDbContext>();

			stopwatch.Start();
			int recordToInsert = 10000;
			//BulkInsertSaveOneByOne(context, recordToInsert);
			//BulkInsertSaveOnce(context, recordToInsert);
			BulkInsertSaveOneExtension(context, recordToInsert);
			stopwatch.Stop();

			Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);
		}

		internal static void Get(EmployeeDbContext context)
		{
			foreach (var i in context.Employees.ToList())
			{
				Console.WriteLine(i.Name);
			}
		}

		// Postgres Results
		// Average Speed: 10k records in 210 seconds
		// Average Speed: 50k records in more than 20 minutes (Too long to wait....)
		//
		// MS SQL Results
		// Average Speed: 10k records in 330 seconds
		// Average Speed: 50k records in ??? seconds
		internal static void BulkInsertSaveOneByOne(EmployeeDbContext context, int recordToInsert)
		{
			for (int i = 0; i < recordToInsert; i++)
			{
				Employee employee = new Employee()
				{
					Id = Guid.NewGuid().ToString(),
					Name = "Apple",
					Department = "OGP",
					Email = "apple@email.com"
				};

				context.Employees.Add(employee);
				context.SaveChanges();
			}
		}

		// Postgres Results
		// Average Speed: 10k records in 2 seconds
		// Average Speed: 50k records in 8.5 seconds
		//
		// MS SQL Results
		// Average Speed: 10k records in 3 seconds
		// Average Speed: 50k records in 12 seconds
		internal static void BulkInsertSaveOnce(EmployeeDbContext context, int recordToInsert)
		{
			for (int i = 0; i < recordToInsert; i++)
			{
				Employee employee = new Employee()
				{
					Id = Guid.NewGuid().ToString(),
					Name = "Apple",
					Department = "OGP",
					Email = "apple@email.com"
				};

				context.Employees.Add(employee);
			}

			context.SaveChanges();
		}

		// Supported only on SQL
		// MS SQL Results
		// Average Speed: 10k records in 1 seconds
		// Average Speed: 50k records in 3 seconds
		internal static void BulkInsertSaveOneExtension(EmployeeDbContext context, int recordToInsert)
		{
			List<Employee> list = new List<Employee>();

			for (int i = 0; i < recordToInsert; i++)
			{
				list.Add(new Employee()
				{
					Id = Guid.NewGuid().ToString(),
					Name = "Apple",
					Department = "OGP",
					Email = "apple@email.com"
				});
			}

			context.BulkInsert(list);
		}
	}
}
