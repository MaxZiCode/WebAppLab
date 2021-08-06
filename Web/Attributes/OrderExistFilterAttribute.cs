using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Constants;
using WebAppLab.Data;
using WebAppLab.Extensions;
using WebAppLab.Models;
using WebAppLab.Services;

namespace WebAppLab.Attributes
{
	public class OrderExistFilterAttribute : IAsyncActionFilter
	{
		private readonly IOrderManager orderManager;

		public OrderExistFilterAttribute(IOrderManager orderManager)
		{
			this.orderManager = orderManager;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var userId = context.HttpContext.User.GetId();
			var hasUserOrder = await orderManager.HasUserOrderAsync(userId);
			if (!hasUserOrder)
			{
				context.Result = new NotFoundResult();
				return;
			}

			await next();
		}
	}
}
