using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using API_FADSI.Models;
using API_FADSI.Services;
using Google.Maps;
using Google.Maps.Geocoding;
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


        //TEST ONLY!!! REMOVE LATER**********************************
        [Route(USER_URL + "/testmaps")]
        [HttpGet]
        public ActionResult<ExpandoObject> TestMaps()
        {
            //always need to use YOUR_API_KEY for requests.  Do this in App_Start.
            //GoogleSigned.AssignAllServices(new GoogleSigned("AIzaSyBrQ-Pb3As0dKkt1iPsxL9IOr-Nfk3E1Cc"));

            var request = new GeocodingRequest();
            request.Address = "San Isidro de El Guarco, Cartago";
            var response = new GeocodingService().GetResponse(request);

            //The GeocodingService class submits the request to the API web service, and returns the
            //response strongly typed as a GeocodeResponse object which may contain zero, one or more results.

            dynamic data = new ExpandoObject();
            //Assuming we received at least one result, let's get some of its properties:
            if (response.Status == ServiceResponseStatus.Ok && response.Results.Count() > 0)
            {
                var result = response.Results.First();

                //Console.WriteLine("Full Address: " + result.FormattedAddress);         // "1600 Pennsylvania Ave NW, Washington, DC 20500, USA"
                data.fullAddress = result.FormattedAddress;
                //Console.WriteLine("Latitude: " + result.Geometry.Location.Latitude);   // 38.8976633
                data.latitude = result.Geometry.Location.Latitude;
                //Console.WriteLine("Longitude: " + result.Geometry.Location.Longitude); // -77.0365739
                data.longitude = result.Geometry.Location.Longitude;
                //Console.WriteLine();
                return data;
            }
            else
            {
                return StatusCode(StatusCodes.Status409Conflict);
                //Console.WriteLine("Unable to geocode.  Status={0} and ErrorMessage={1}", response.Status, response.ErrorMessage);
            }
        }

    }
}