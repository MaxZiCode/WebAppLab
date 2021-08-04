using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Filters;

using WebAppLab.Constants;
using WebAppLab.Data;
using WebAppLab.Models;
using WebAppLab.Data.Constants;

namespace WebAppLab.Attributes
{
	public class ValidationSortFilterAttribute : IActionFilter
	{
		private static readonly string[] allSorts;

		private readonly ApplicationDbContext context;

		static ValidationSortFilterAttribute()
		{
			allSorts = typeof(SortBy).GetFields().Select(p => (string)p.GetValue(obj: null)).ToArray();
		}

		public ValidationSortFilterAttribute(ApplicationDbContext context)
		{
			this.context = context;
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var param = context.ActionArguments.SingleOrDefault(a => a.Value is ProductListData);

			if (param.Value == null)
			{
				context.Result = new BadRequestObjectResult(new { message = "Object is null" });
				return;
			}

			var data = (ProductListData)param.Value;

			if (data.SortBy != null && !IsSortExists(data.SortBy))
			{
				context.Result = new BadRequestObjectResult(new { message = $"Sort {data.SortBy} not found" });
				return;
			}

			if (data.Genre != null && !IsGenreExists(data.Genre))
			{
				context.Result = new BadRequestObjectResult(new { message = $"Genre {data.Genre} not found" });
				return;
			}
		}

		private bool IsGenreExists(string genre)
		{
			var foundProduct = context.Products.FirstOrDefault(p => p.Genre == genre);
			return foundProduct != null;
		}

		private bool IsSortExists(string sort)
		{
			var foundSort = allSorts.SingleOrDefault(s => s.Equals(sort, StringComparison.OrdinalIgnoreCase));
			return foundSort != null;
		}
	}
}
