namespace WebAppLab.Wrappers
{
	public class PagedResult<T>
	{
		public int PageNumber { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalRecords { get; set; }
		public string FirstPage { get; set; }
		public string LastPage { get; set; }
		public string NextPage { get; set; }
		public string PreviousPage { get; set; }
		public T Data { get; set; }

		public PagedResult(T data, int pageNumber, int pageSize)
		{
			PageNumber = pageNumber;
			PageSize = pageSize;
			Data = data;
		}
	}
}
