using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using API_FADSI.Models;
using API_FADSI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_FADSI.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private const string USER_URL = "api/users";
        private readonly UserService _userService;

        /// <summary>
        /// Class constructor 
        /// </summary>
        /// <param name="pUserService"></param>
        public UserController(UserService pUserService)
        {
            _userService = pUserService;
        }


        /// <summary>
        /// Return a list with the data of all the users inside the database
        /// </summary>
        /// <returns></returns>
        [Route(USER_URL + "/all")]
        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            return _userService.Get();
        }


        /// <summary>
        /// Return the data of a single user by its Id
        /// </summary>
        /// <param name="pId">Id of the user</param>
        /// <returns></returns>
        [Route(USER_URL + "/get/{pId}")]
        [HttpGet]
        public ActionResult<User> Get(string pId)
        {
            var user = _userService.Get(pId);

            if (user == null)
                return NotFound();

            return user;
        }


        /// <summary>
        /// Receives the data of a new user, to insert it in the database
        /// </summary>
        /// <param name="pUser">Model class with the data of the new user</param>
        /// <returns>Http status code: 201 if successful, 409 if there is an error</returns>
        [Route(USER_URL + "/create")]
        [HttpPost]
        public IActionResult Create(User pUser)
        {
            int result = _userService.Create(pUser);

            if (result < 0)
                return StatusCode(StatusCodes.Status409Conflict);

            return StatusCode(StatusCodes.Status201Created);
        }


        /// <summary>
        /// Receives the updated data of a user, to update it in the database
        /// </summary>
        /// <param name="pId">Id of the user</param>
        /// <param name="pUser">Model class with the updated data</param>
        /// <returns>Http status code: 200 if successfull,
        /// 409 if there is an error during the updating process, 
        /// 404 if the user is not found the database</returns>
        [Route(USER_URL + "/update/{pId}")]
        [HttpPost]
        public IActionResult Update(string pId, User pUser)
        {
            if (_userService.Get(pId) == null)
                return NotFound();

            if (_userService.Update(pId, pUser) < 0)
                return StatusCode(StatusCodes.Status409Conflict);

            return StatusCode(StatusCodes.Status200OK);
        }


        /// <summary>
        /// Deletes a user from the database, searching it by its Id(_id)
        /// </summary>
        /// <param name="pId">Id of the user</param>
        /// <returns>Http status code: 200 if successfull, 
        /// 404 if the user is not found the database</returns>
        [Route(USER_URL + "/delete/{pId}")]
        [HttpGet]
        public IActionResult Delete(string pId)
        {
            if (_userService.Get(pId) == null)
                return NotFound();

            _userService.Remove(pId);
            return StatusCode(StatusCodes.Status200OK);
        }


        /// <summary>
        /// Return the data of a user searching it by its user name
        /// </summary>
        /// <param name="pUserName">Username</param>
        /// <returns></returns>
        [Route(USER_URL + "/login/{pUserName}")]
        [HttpGet]
        public ActionResult<ExpandoObject> Login(string pUserName)
        {
            var user = _userService.Login(pUserName);
            if (user == null)
                return NotFound();

            return user;
        }

    }
}