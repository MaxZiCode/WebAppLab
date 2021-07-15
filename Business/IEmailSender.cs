using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppLab
{
	public interface IEmailSender
	{
		public Task SendAsync(string toAddress, string subject, string body);
	}
}
