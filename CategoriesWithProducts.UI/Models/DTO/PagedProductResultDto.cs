namespace CategoriesWithProducts.UI.Models.DTO
{
    public class PagedProductResultDto
    {
        public List<ProductDto> Products { get; set; }
        public int TotalCount { get; set; }
    }
}
