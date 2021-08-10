using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;

namespace WebAppLab.Data.Models
{
	public class User : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string AddressDelivery { get; set; }
		public Order Order { get; set; }

		public ICollection<ProductRating> ProductRatings { get; set; }
	}
}
