
using System;

namespace WebAppLab.Data.Models
{
	public class Product
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public Platforms Platform { get; set; }
		public DateTime DateCreated { get; set; }
		public int TotalRating { get; set; }
	}
}
