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
    public class PlaceController : ControllerBase
    {
        private const string PLACE_URL = "api/places";
        private readonly PlaceService _placeService;

        /// <summary>
        /// Class constructor 
        /// </summary>
        /// <param name="pPlaceService"></param>
        public PlaceController(PlaceService pPlaceService)
        {
            _placeService = pPlaceService;
        }


        /// <summary>
        /// Return a list with the data of all the places inside the database
        /// </summary>
        /// <returns></returns>
        [Route(PLACE_URL + "/all")]
        [HttpGet]
        public ActionResult<List<Place>> Get()
        {
            return _placeService.Get();
        }


        /// <summary>
        /// Return the data of a single place by its Id
        /// </summary>
        /// <param name="pId">Id of the place</param>
        /// <returns></returns>
        [Route(PLACE_URL + "/get/{pId}")]
        [HttpGet]
        public ActionResult<Place> Get(string pId)
        {
            var place = _placeService.Get(pId);

            if (place == null)
                return NotFound();

            return place;
        }


        /// <summary>
        /// Receives the data of a new place, to insert it in the database
        /// </summary>
        /// <param name="pPlace">Model class with the data of the new place</param>
        /// <returns>Http status code: 201 if successful, 409 if there is an error</returns>
        [Route(PLACE_URL + "/create")]
        [HttpPost]
        public IActionResult Create(Place pPlace)
        {
            int result = _placeService.Create(pPlace);

            if (result < 0)
                return StatusCode(StatusCodes.Status409Conflict);

            return StatusCode(StatusCodes.Status201Created);
        }


        /// <summary>
        /// Receives the updated data of a place, to update it in the database
        /// </summary>
        /// <param name="pId">Id of the place</param>
        /// <param name="pPlace">Model class with the updated data</param>
        /// <returns>Http status code: 200 if successfull,
        /// 409 if there is an error during the updating process, 
        /// 404 if the place is not found the database</returns>
        [Route(PLACE_URL + "/update/{pId}")]
        [HttpPost]
        public IActionResult Update(string pId, Place pPlace)
        {
            if (_placeService.Get(pId) == null)
                return NotFound();

            if (_placeService.Update(pId, pPlace) < 0)
                return StatusCode(StatusCodes.Status409Conflict);

            return StatusCode(StatusCodes.Status200OK);
        }


        /// <summary>
        /// Deletes a place from the database, searching it by its Id(_id)
        /// </summary>
        /// <param name="pId">Id of the place</param>
        /// <returns>Http status code: 200 if successfull, 
        /// 404 if the place is not found the database</returns>
        [Route(PLACE_URL + "/delete/{pId}")]
        [HttpGet]
        public IActionResult Delete(string pId)
        {
            if (_placeService.Get(pId) == null)
                return NotFound();

            _placeService.Remove(pId);
            return StatusCode(StatusCodes.Status200OK);
        }


        /// <summary>
        /// Returns the list of all places of the specified type
        /// </summary>
        /// <param name="pType">Integer that represents the type of place</param>
        /// <returns></returns>
        [Route(PLACE_URL + "/type/{pType}")]
        [HttpGet]
        public ActionResult<List<Place>> PlacesByType(int pType)
        {
            return _placeService.PlacesByType(pType);
        }


        [Route(PLACE_URL + "/nearby/{pPlaceId}")]
        [HttpGet]
        public ActionResult<List<dynamic>> NearbyPlaces(string pPlaceId)
        {
            return _placeService.NearbyPlaces(pPlaceId);
        }


    }
}