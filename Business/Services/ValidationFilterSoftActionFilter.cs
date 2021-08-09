using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Linq;

using WebAppLab.Data;
using WebAppLab.Helpers;
using WebAppLab.Models;

namespace WebAppLab.Attributes
{
	public class ValidationFilterSoftActionFilter : IActionFilter
	{ 
		private readonly ApplicationDbContext dBcontext;

		public ValidationFilterSoftActionFilter(ApplicationDbContext context)
		{
			dBcontext = context;
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var param = context.ActionArguments.SingleOrDefault(a => a.Value is ProductListData);

			if (param.Value == null)
			{
				context.ModelState.AddModelError(nameof(ProductListData), $"'{nameof(ProductListData)}' is null.");
				return;
			}

			var data = (ProductListData)param.Value;
		}
	}
}
