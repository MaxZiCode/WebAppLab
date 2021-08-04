using System;
using System.Collections.Generic;

using WebAppLab.Data.Constants;

namespace WebAppLab.Data.Models
{
	public class Product
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public Platforms Platform { get; set; }
		public DateTime DateCreated { get; set; }
		public int TotalRating { get; set; }
		public string Genre { get; set; }
		public Rating Rating { get; set; }
		public string Logo { get; set; }
		public string Background { get; set; }
		public decimal Price { get; set; }
		public int Count { get; set; }
		public DateTime? DeletedAt { get; set; }

		public ICollection<ProductRating> ProductRatings { get; set; }
	}
}
