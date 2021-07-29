using System;

namespace WebAppLab.Data.Models
{
	public class ProductRating
	{
		public Guid ProductId { get; set; }
		public string UserId { get; set; }
		public int Rating { get; set; }
	}
}
