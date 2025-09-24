using AutoMapper;
using CategoriesWithProducts.CustomActionFilters;
using CategoriesWithProducts.Models.DTO;
using CategoriesWithProducts.Models.Entities;
using CategoriesWithProducts.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CategoriesWithProducts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IProductRepository productRepository;

        public ProductsController(IMapper mapper, IProductRepository productRepository)
        {
            this.mapper = mapper;
            this.productRepository = productRepository;
        }

        [Authorize(Roles = "Writer")]
        [HttpPost]
        [Route("{categoryId:guid}")]
        [ValidateModel]
        public async Task<IActionResult> CreateProduct([FromBody] AddProductDto addProductDto, [FromRoute] Guid categoryId)
        {

            var productDomainModel = mapper.Map<Product>(addProductDto);

            var createdProduct = await productRepository.CreateProductAsync(productDomainModel, categoryId);

            return Ok(mapper.Map<ProductDto>(createdProduct));
        }

        // Get: /api/category?filterOn=Name&filterQuery=a&sortBy=name&isAscending=true&pageNumber=1&pageSize=10
        
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] string filterOn, [FromQuery] string? filterQuery,
            [FromQuery] decimal? minValue, [FromQuery] decimal? maxValue, [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            filterOn = "Name";
            var productsDomainModel = await productRepository.GetAllProductsAsync(filterOn, filterQuery, minValue, maxValue, sortBy,
                isAscending ?? true, pageNumber, pageSize);

            var totalCount = await productRepository.GetFilteredProductsCountAsync(filterQuery = null,
            sortBy = null,
            isAscending ?? true,
            minValue = null,
            maxValue = null);

            return Ok(new PagedProductResultDto
            {
                Products = mapper.Map<List<ProductDto>>(productsDomainModel),
                TotalCount = totalCount
            });

        }

        
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetProductById([FromRoute] Guid id)
        {
            var productDomainModel = await productRepository.GetProductByIdAsync(id);

            if (productDomainModel == null)
            {
                NotFound();
            }
            return Ok(mapper.Map<ProductDto>(productDomainModel));
        }

        
        [HttpGet]
        [Route("{categoryId}/products")]
        public async Task<IActionResult> GetProductsByCategory([FromRoute] Guid categoryId, [FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] decimal? minValue, [FromQuery] decimal? maxValue, [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var productDomainModel = await productRepository.GetProductsByCategoryAsync(categoryId, filterQuery, minValue, maxValue, sortBy,
                isAscending ?? true, pageNumber, pageSize);

            if (productDomainModel == null)
            {
                NotFound();
            }
            return Ok(mapper.Map<List<ProductDto>>(productDomainModel));
        }

        [Authorize(Roles = "Writer")]
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, UpdateProductDto updateProductDto)
        {

            var productDomainModel = mapper.Map<Product>(updateProductDto);

            productDomainModel = await productRepository.UpdateProductAsync(id, productDomainModel);

            if (productDomainModel == null)
            {
                NotFound();
            }
            return Ok(mapper.Map<ProductDto>(productDomainModel));

        }

        [Authorize(Roles = "Writer")]
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteProduct([FromRoute,] Guid id)
        {
            var productDomainModel = await productRepository.DeleteProductAsync(id);
            if (productDomainModel == null)
            {
                NotFound();
            }

            return Ok(mapper.Map<ProductDto>(productDomainModel));
        }
        [HttpGet("search")]
        public async Task<IActionResult> LiveSearch([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<ProductDto>());

            var matchedProducts = await productRepository.GetAllProductsAsync(
                filterOn: "Name",
                filterQuery: query,
                pageSize: 5
            );

            var result = mapper.Map<List<ProductDto>>(matchedProducts);

            return Ok(result);
        }
    }
}
