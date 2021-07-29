using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

		[Column(TypeName = "Money")]
		public decimal Price { get; set; }
		public int Count { get; set; }

		[DataType(DataType.Date)]
		[Column(TypeName = "Date")]
		public DateTime? DeletedAt { get; set; }
	}
}
