using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Models;

using AutoMapper;

namespace WebAppLab.Helpers
{
	public class ClaimsUserSearcher : IUserSearcher
	{
		private readonly ApplicationDbContext dbContext;
		private readonly IMapper mapper;

		public ClaimsUserSearcher(ApplicationDbContext dbContext, IMapper mapper)
		{
			this.dbContext = dbContext;
			this.mapper = mapper;
		}

		public async Task<User> FindAsync(HttpContext context)
		{
			var id = context.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			if (id == null)
				return null;

			var userFromDb = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
			if (userFromDb == null)
				return null;

			var user = mapper.Map<UserDbo, User>(userFromDb);
			return user;
		}
	}
}
