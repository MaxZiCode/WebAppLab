using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppLab.Models
{
	public class AuthResult
	{
		public bool Succeeded { get; private set; }
		public string FailMessage { get; private set; }

		public static AuthResult Success => new() { Succeeded = true };

		public static AuthResult FromFail(string message) => new() { Succeeded = false, FailMessage = message };
	}
}
