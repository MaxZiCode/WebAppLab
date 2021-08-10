using AutoMapper;

using WebAppLab.Data.Constants;

namespace WebAppLab.Models
{
	public class ProductListData
	{
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public string Genre { get; set; }
		public Rating? Age { get; set; }
		public SortBy? SortBy { get; set; }
		public bool? IsSortAsc { get; set; }
	}
}
