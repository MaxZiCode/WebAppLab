using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppLab.Data.Models
{
	[Index(nameof(Name), nameof(Platform), nameof(DateCreated), nameof(TotalRating))]
	public class Product
	{
		public Guid Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public Platforms Platform { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[Column(TypeName = "Date")]
		public DateTime DateCreated { get; set; }

		[Required]
		[Range(0, 100)]
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
