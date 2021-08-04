using AutoMapper;

using WebAppLab.Data.Constants;

namespace WebAppLab.Models
{
	public class ProductListData
	{
		private static readonly IMapper mapper = new MapperConfiguration(config => config.CreateMap<ProductListData, ProductListData>()).CreateMapper();

		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public string Genre { get; set; }
		public Rating? Age { get; set; }
		public string SortBy { get; set; }
		public bool? IsSortAsc { get; set; }

		public ProductListData Clone() => mapper.Map<ProductListData>(this);
	}
}
