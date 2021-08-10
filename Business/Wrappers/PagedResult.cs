using System;

namespace WebAppLab.Wrappers
{
	public class PagedResult<T>
	{
		public int PageNumber { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalRecords { get; set; }
		public T Data { get; set; }

		public PagedResult(T pagedProducts, int totalProducts, int pageSize, int pageNumber)
		{
			var totalPages = Convert.ToInt32(Math.Ceiling((double)totalProducts / pageSize));

			Data = pagedProducts;
			PageSize = pageSize;
			PageNumber = pageNumber;
			TotalPages = totalPages;
			TotalRecords = totalProducts;
		}
	}
}
