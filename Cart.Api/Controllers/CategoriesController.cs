using Cart.Domain;
using Cart.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RS2.Core;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Cart.Api.Controllers
{
    [Route("api/[controller]")]
 	[Authorize]
    public class CategoriesController : ControllerBase
    {

		private ICategoryService _categoryService;
		private IUnitOfWork _unitOfWork;
        private readonly IHostingEnvironment env;

        public CategoriesController(ICategoryService categoryService , 
			IUnitOfWork unitOfWork,
            IHostingEnvironment env)
		{
			_categoryService = categoryService;
			_unitOfWork = unitOfWork;
            this.env = env;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok( await _categoryService.GetAllCategories());
        }

		// GET: api/Categories/5
		[HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
			try
			{
				var category = await _categoryService.GetCategoryById(id);
				if (category != null)				
					return Ok(category);				
				else
					throw new Exception("Not Found");
			}
			catch(Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
			
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Category category)
        {
            int id = await _categoryService.SaveCategory(category);

            if (id > 0)
                return Ok(id);
            else
                return StatusCode(500, "Data Not saved to database. Please ");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
			using (var tran = _unitOfWork.DataContext.Database.BeginTransaction())
			{
				
				var result =  await _categoryService.DeleteCategory(id); // Set Deleted flag.

				if (result)
				{
					return Ok();
				}
				else
				{					
					return StatusCode(500,"Unable to delete the project.");
				}
			}
        }

	}
}
