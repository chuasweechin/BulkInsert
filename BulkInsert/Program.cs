using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BulkInsert
{
  class Program
  {
		static void Main(string[] args)
		{
			
			Stopwatch stopwatch = new Stopwatch();
			string connectionString = "Host=localhost;Port=5432;Database=employeedb;Username=chuasweechin";

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddDbContext<EmployeeDbContext>(options => {
				options.UseNpgsql(connectionString);
			});

			var serviceProvider = serviceCollection.BuildServiceProvider();
			EmployeeDbContext context = serviceProvider.GetService<EmployeeDbContext>();

			stopwatch.Start();
			int recordToInsert = 50000;
			BulkInsertSaveOneByOne(context, recordToInsert);
			//BulkInsertSaveOnce(context, recordToInsert);
			//BulkInsertSaveOneExtension(context, recordToInsert);
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

		// Average Speed: 10k records in 210 seconds
		// Inserted Record has 4 string fields
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

		// Average Speed: 10k records in 2 seconds
		// Average Speed: 50k records in 8.5 seconds
		// Inserted Record has 4 string fields
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

		// Supported on ly on SQL
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
