using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_FADSI.Models;
using API_FADSI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_FADSI.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private const string PRODUCT_URL = "api/products";
        private readonly ProductService _productService;

        /// <summary>
        /// Class constructor 
        /// </summary>
        /// <param name="pProductService"></param>
        public ProductController(ProductService pProductService)
        {
            _productService = pProductService;
        }


        /// <summary>
        /// Return a list with the data of all the products inside the database
        /// </summary>
        /// <returns></returns>
        [Route(PRODUCT_URL + "/all")]
        [HttpGet]
        public ActionResult<List<Product>> Get()
        {
            return _productService.Get();
        }



        /// <summary>
        /// Return the data of a single product by its Id
        /// </summary>
        /// <param name="pId">Id of the product</param>
        /// <returns></returns>
        [Route(PRODUCT_URL + "/get/{pId}")]
        [HttpGet]
        public ActionResult<Product> Get(string pId)
        {
            var product = _productService.Get(pId);

            if (product == null)
                return NotFound();

            return product;
        }


        /// <summary>
        /// Returns the list of products of a place
        /// </summary>
        /// <param name="pPlaceId">Id of the place</param>
        /// <returns></returns>
        [Route(PRODUCT_URL + "/place/{pPlaceId}")]
        [HttpGet]
        public ActionResult<List<Product>> GetPlaceProducts(string pPlaceId)
        {
            return _productService.GetPlaceProducts(pPlaceId);
        }


        /// <summary>
        /// Receives the data of a new product, to insert it in the database
        /// </summary>
        /// <param name="pProduct">Model class with the data of the new product</param>
        /// <returns>Http status code: 201 if successful, 409 if there is an error</returns>
        [Route(PRODUCT_URL + "/create")]
        [HttpPost]
        public IActionResult Create(Product pProduct)
        {
            int result = _productService.Create(pProduct);

            if (result < 0)
                return StatusCode(StatusCodes.Status409Conflict);

            return StatusCode(StatusCodes.Status201Created);
        }


        /// <summary>
        /// Receives the updated data of a product, to update it in the database
        /// </summary>
        /// <param name="pId">Id of the place</param>
        /// <param name="pProduct">Model class with the updated data</param>
        /// <returns>Http status code: 200 if successfull,
        /// 409 if there is an error during the updating process, 
        /// 404 if the product is not found the database</returns>
        [Route(PRODUCT_URL + "/update/{pId}")]
        [HttpPost]
        public IActionResult Update(string pId, Product pProduct)
        {
            if (_productService.Get(pId) == null)
                return NotFound();

            if (_productService.Update(pId, pProduct) < 0)
                return StatusCode(StatusCodes.Status409Conflict);

            return StatusCode(StatusCodes.Status200OK);
        }


        /// <summary>
        /// Deletes a product from the database, searching it by its Id(_id)
        /// </summary>
        /// <param name="pId">Id of the product</param>
        /// <returns>Http status code: 200 if successfull, 
        /// 404 if the product is not found the database</returns>
        [Route(PRODUCT_URL + "/delete/{pId}")]
        [HttpGet]
        public IActionResult Delete(string pId)
        {
            if (_productService.Get(pId) == null)
                return NotFound();

            _productService.Remove(pId);
            return StatusCode(StatusCodes.Status200OK);
        }

    }
}