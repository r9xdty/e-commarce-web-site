using System.Text.Json;
using AutoMapper;
using CategoriesWithProducts.CustomActionFilters;
using CategoriesWithProducts.Data;
using CategoriesWithProducts.Models.DTO;
using CategoriesWithProducts.Models.Entities;
using CategoriesWithProducts.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CategoriesWithProducts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ListDBContext dBContext;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;
        private readonly ILogger<CategoryController> logger;

        public CategoryController(ListDBContext dBContext, ICategoryRepository categoryRepository, IMapper mapper,
            ILogger<CategoryController> logger)
        {
            this.dBContext = dBContext;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            this.logger = logger;
        }


        //Bütün verileri çekme
        [HttpGet]
        
        public async Task<IActionResult> GetAllCategories()
        {
            var categoriesDomain = await categoryRepository.GetAllCategoriesAsync();

            logger.LogInformation($"Finished GetAllCategories request with data: {JsonSerializer.Serialize(categoriesDomain)}");

            return Ok(mapper.Map<List<CategoryDto>>(categoriesDomain));
        }



        //Belli bir id ye göre veri çekme
        [HttpGet]
        [Route("{id:guid}")]
        
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            var categoryDomain = await categoryRepository.GetCategoryByIdAsync(id); //Find sadece id için kullanılabilir.

            if (categoryDomain == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CategoryDto>(categoryDomain));
        }
        //Veri yayınlama
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryDto addCategoryDto)
        {
            var categoryDomainModel = mapper.Map<Category>(addCategoryDto);

            categoryDomainModel = await categoryRepository.AddCategoryAsync(categoryDomainModel);

            var categoryDto = mapper.Map<CategoryDto>(categoryDomainModel);

            return CreatedAtAction(nameof(AddCategory), new { id = categoryDomainModel.Id });

        }



        //Veri güncelleme
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            var categoryDomainModel = mapper.Map<Category>(updateCategoryDto);

            categoryDomainModel = await categoryRepository.UpdateCategoryAsync(id, categoryDomainModel);

            if (categoryDomainModel == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CategoryDto>(categoryDomainModel));
        }



        //Veri silme
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var categoryDomainModel = await categoryRepository.DeleteCategoryAsync(id);

            if (categoryDomainModel == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CategoryDto>(categoryDomainModel));    
        }

    }
}
