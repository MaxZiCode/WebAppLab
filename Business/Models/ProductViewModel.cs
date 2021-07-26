using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data.Models;

namespace WebAppLab.Models
{
	public class ProductViewModel
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
	}
}
