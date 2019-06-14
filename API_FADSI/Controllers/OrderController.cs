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
    public class OrderController : ControllerBase
    {
        private const string ORDER_URL = "api/orders";
        private readonly OrderService _orderService;

        /// <summary>
        /// Class constructor 
        /// </summary>
        /// <param name="pOrderService"></param>
        public OrderController(OrderService pOrderService)
        {
            _orderService = pOrderService;
        }


        /// <summary>
        /// Return a list with the data of all the orders inside the database
        /// </summary>
        /// <returns></returns>
        [Route(ORDER_URL + "/all")]
        [HttpGet]
        public ActionResult<List<Order>> Get()
        {
            return _orderService.Get();
        }


        /// <summary>
        /// Returns the data of a single order by its Id
        /// </summary>
        /// <param name="pId">Id of the order</param>
        /// <returns></returns>
        [Route(ORDER_URL + "/get/{pId}")]
        [HttpGet]
        public ActionResult<Order> Get(string pId)
        {
            var order = _orderService.Get(pId);

            if (order == null)
                return NotFound();

            return order;
        }


        /// <summary>
        /// Returns a list with all the orders made by an user
        /// </summary>
        /// <param name="pUserId">Id of the user</param>
        /// <returns></returns>
        [Route(ORDER_URL + "/user/{pUserId}")]
        [HttpGet]
        public ActionResult<List<Order>> GetUserOrders(string pUserId)
        {
            return _orderService.GetUserOrders(pUserId);

        }


        /// <summary>
        /// Receives the data of a new order, to insert it in the database
        /// </summary>
        /// <param name="pOrder">Model class with the data of the new order</param>
        /// <returns>Http status code: 201 if successful, 409 if there is an error</returns>
        [Route(ORDER_URL + "/create")]
        [HttpPost]
        public IActionResult Create(Order pOrder)
        {
            int result = _orderService.Create(pOrder);

            if (result < 0)
                return StatusCode(StatusCodes.Status409Conflict);

            return StatusCode(StatusCodes.Status201Created);
        }


        /// <summary>
        /// Receives the updated data of a order, to update it in the database
        /// </summary>
        /// <param name="pId">Id of the order</param>
        /// <param name="pOrder">Model class with the updated data</param>
        /// <returns>Http status code: 200 if successfull,
        /// 409 if there is an error during the updating process, 
        /// 404 if the order is not found the database</returns>
        [Route(ORDER_URL + "/update/{pId}")]
        [HttpPost]
        public IActionResult Update(string pId, Order pOrder)
        {
            if (_orderService.Get(pId) == null)
                return NotFound();

            if (_orderService.Update(pId, pOrder) < 0)
                return StatusCode(StatusCodes.Status409Conflict);

            return StatusCode(StatusCodes.Status200OK);
        }


        /// <summary>
        /// Deletes a order from the database, searching it by its Id(_id)
        /// </summary>
        /// <param name="pId">Id of the order</param>
        /// <returns>Http status code: 200 if successfull, 
        /// 404 if the order is not found the database</returns>
        [Route(ORDER_URL + "/delete/{pId}")]
        [HttpGet]
        public IActionResult Delete(string pId)
        {
            if (_orderService.Get(pId) == null)
                return NotFound();

            _orderService.Remove(pId);
            return StatusCode(StatusCodes.Status200OK);
        }


    }


}