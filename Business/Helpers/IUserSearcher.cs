using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppLab.Models;

namespace WebAppLab.Helpers
{
	public interface IUserSearcher
	{
		public Task<User> FindAsync(HttpContext context);
	}
}
