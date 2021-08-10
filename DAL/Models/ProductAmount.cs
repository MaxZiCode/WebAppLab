using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppLab.Data.Models
{
	public class ProductAmount
	{
		public Guid OrderId { get; set; }
		public Guid ProductId { get; set; }
		public int Amount { get; set; }
	}
}
