using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System;
using System.Linq;

using WebAppLab.Data.Constants;
using WebAppLab.Models;

namespace WebAppLab.Attributes
{
	public class ValidationFilterSoftActionFilter : IActionFilter
	{
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

			var modelState = context.ModelState;
			AddErrorIfInvalidEnum<Rating>(modelState, nameof(data.Age), data.Age);
			AddErrorIfInvalidEnum<SortBy>(modelState, nameof(data.SortBy), data.SortBy);

			if (!modelState.IsValid)
			{
				var validationDetails = new ValidationProblemDetails(modelState);
				context.Result = new BadRequestObjectResult(validationDetails);
			}
		}

		private void AddErrorIfInvalidEnum<T>(ModelStateDictionary modelstate, string key, string value) where T: struct
		{
			if (value == null)
				return;

			var isParsed = Enum.TryParse(value, ignoreCase: true, out T parsedValue);
			if (isParsed && Enum.IsDefined(typeof(T), parsedValue))
				return;
			
			modelstate.AddModelError(key, $"The value '{value}' is invalid.");
		}
	}
}
