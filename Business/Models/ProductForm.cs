using Microsoft.AspNetCore.Http;

using System;
using System.IO;

using WebAppLab.Data.Constants;

namespace WebAppLab.Models
{
	public class ProductForm
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public Platforms Platform { get; set; }
		public DateTime DateCreated { get; set; }
		public string Genre { get; set; }
		public Rating Rating { get; set; }
		public IFormFile Logo { get; set; }
		public IFormFile Background { get; set; }
		public decimal Price { get; set; }
		public int Count { get; set; }
	}
}
