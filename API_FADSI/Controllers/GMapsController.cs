using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using API_FADSI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_FADSI.Controllers
{
    [ApiController]
    public class GMapsController : ControllerBase
    {
        private const string GMAPS_URL = "api/gmaps";
        private readonly GMapsService _gMapsService;


        /// <summary>
        /// Class constructor 
        /// </summary>
        /// <param name="pGMapsService"></param>
        public GMapsController(GMapsService pGMapsService)
        {
            _gMapsService = pGMapsService;
        }


        /// <summary>
        /// Returns a list of places nearby the specified latitude and longitude
        /// </summary>
        /// <param name="pLatitude">Latitude</param>
        /// <param name="pLongitude">Longitude</param>
        /// <returns></returns>
        [Route(GMAPS_URL + "/coordinates/{pLatitude}/{pLongitude}")]
        [HttpGet]
        public List<dynamic> PlacesByCoordinates([FromRoute] double pLatitude, [FromRoute] double pLongitude)
        {
            var result = _gMapsService.PlacesByCoordinates(pLatitude, pLongitude);
            return result;
        }


        /// <summary>
        /// Returns a list of places nearby the specified address
        /// </summary>
        /// <param name="pAddress">Address of search</param>
        /// <returns></returns>
        [Route(GMAPS_URL + "/address/{pAddress}")]
        [HttpGet]
        public List<dynamic> PlacesByAddress(string pAddress)
        {
            return _gMapsService.PlacesByAddress(pAddress);
        }


        /// <summary>
        /// Returns all the details of a specific place
        /// </summary>
        /// <param name="pPlaceId">Place id given by Google Maps API</param>
        /// <returns></returns>
        [Route(GMAPS_URL + "/details/{pPlaceId}")]
        [HttpGet]
        public ExpandoObject PlaceDetails(string pPlaceId)
        {
            return _gMapsService.PlaceDetails(pPlaceId);
        }


        /// <summary>
        /// Returns the distance between two sets of coordinates
        /// </summary>
        /// <param name="pLatitude1">Latitude of first point</param>
        /// <param name="pLongitude1">Longitude of first point</param>
        /// <param name="pLatitude2">Latitude of second point</param>
        /// <param name="pLongitude2">Longitude of second point</param>
        /// <returns></returns>
        [Route(GMAPS_URL + "/distance/{pLatitude1}/{pLongitude1}/{pLatitude2}/{pLongitude2}")]
        [HttpGet]
        public ExpandoObject DistanceByCoordinates([FromRoute] double pLatitude1, [FromRoute] double pLongitude1,
            [FromRoute] double pLatitude2, [FromRoute] double pLongitude2)
        {
            return _gMapsService.DistanceByCoordinates(pLatitude1, pLongitude1, pLatitude2, pLongitude2);
        }

    }
}