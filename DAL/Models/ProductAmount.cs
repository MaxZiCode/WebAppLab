using System;

namespace WebAppLab.Data.Models
{
	public class ProductAmount
	{
		public Guid OrderId { get; set; }
		public Guid ProductId { get; set; }
		public int Amount { get; set; }
	}
}
