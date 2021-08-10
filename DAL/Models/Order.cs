using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppLab.Data.Models
{
	public class Order
	{
		public Guid Id { get; set; }
		public string UserId { get; set; }
		public ICollection<ProductAmount> ProductAmounts { get; set; }
		public bool IsCompleted { get; set; }
	}
}
