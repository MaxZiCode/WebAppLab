using System;

namespace WebAppLab.Models
{
	public class User
	{
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string AddressDelivery { get; set; }
		public string PhoneNumber { get; set; }
	}
}
