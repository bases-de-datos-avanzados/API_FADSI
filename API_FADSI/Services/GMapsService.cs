using Google.Maps;
using Google.Maps.DistanceMatrix;
using Google.Maps.Places;
using Google.Maps.Places.Details;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace API_FADSI.Services
{
    public class GMapsService
    {
        /// <summary>
        /// Obtains a list of places within a 10 meters radius of the specified latitude and longitude
        /// </summary>
        /// <param name="pLatitude">Latitude</param>
        /// <param name="pLongitude">Longitude</param>
        /// <returns></returns>
        public List<dynamic> PlacesByCoordinates(double pLatitude, double pLongitude)
        {
            PlacesRequest request = new NearbySearchRequest()
            {
                Location = new LatLng(pLatitude, pLongitude),
                Radius = 10
            };

            PlacesResponse response = new PlacesService().GetResponse(request);
            List<dynamic> foundPlaces = new List<dynamic>();

            if (response.Status == ServiceResponseStatus.Ok)
            {
                foreach(PlacesResult result in response.Results)
                {
                    dynamic place = new ExpandoObject();
                    place.placeID = result.PlaceId;
                    place.name = result.Name;
                    place.address = result.FormattedAddress;
                    place.type = new List<string>();
                    foreach (PlaceType type in result.Types)
                    {
                        place.type.Add(type.ToString());
                    }
                    place.rating = result.Rating;
                    place.photo = result.Icon;
                    foundPlaces.Add(place);
                }

                return foundPlaces;
            }
            else return null;
        }


        /// <summary>
        /// Obtains a list of places within a 10 meters radius of the specified address
        /// </summary>
        /// <param name="pAddress">Address of search</param>
        /// <returns></returns>
        public List<dynamic> PlacesByAddress(string pAddress)
        {
            PlacesRequest request = new TextSearchRequest()
            {
                Query = pAddress,
                Radius = 10
            };

            PlacesResponse response = new PlacesService().GetResponse(request);
            List<dynamic> foundPlaces = new List<dynamic>();

            if (response.Status == ServiceResponseStatus.Ok)
            {
                foreach (PlacesResult result in response.Results)
                {
                    dynamic place = new ExpandoObject();
                    place.Id = result.PlaceId;
                    place.name = result.Name;
                    place.address = result.FormattedAddress;
                    place.type = new List<string>();
                    foreach (PlaceType type in result.Types)
                    {
                        place.type.Add(type.ToString());
                    }
                    place.rating = result.Rating;
                    place.photo = result.Icon;
                    foundPlaces.Add(place);
                }
                return foundPlaces;
            }
            else return null;
        }


        /// <summary>
        /// Obtains all the details of a specific place
        /// </summary>
        /// <param name="pPlaceId">Place id given by Google Maps API</param>
        /// <returns></returns>
        public dynamic PlaceDetails(string pPlaceId)
        {
            PlaceDetailsRequest request = new PlaceDetailsRequest();
            request.PlaceID = pPlaceId;

            var response = new PlaceDetailsService().GetResponse(request);

            dynamic place = new ExpandoObject();
            if (response.Status == ServiceResponseStatus.Ok)
            {
                PlaceDetailsResult result = response.Result;
                place.name = result.Name;
                place.latitude = result.Geometry.Location.Latitude;
                place.longitude = result.Geometry.Location.Longitude;
                place.address = result.FormattedAddress;
                place.phone = result.InternationalPhoneNumber;
                place.rating = result.Rating;
                place.schedule = GetOpeningHours(result);
                place.website = result.Website;
                place.photo = "";
                //place.photo = RequestPlacePhoto(result.Photos[0].PhotoReference);
                return place;
            }
            else return null;
        }


        /// <summary>
        /// Obtains the schedule of a place, which could be not returned by the API
        /// </summary>
        /// <param name="pResult"></param>
        /// <returns></returns>
        private string[] GetOpeningHours(PlaceDetailsResult pResult)
        {
            string[] schedule = { };
            try
            {
                schedule = pResult.OpeningHours.WeekdayText;
                return schedule;
            }
            catch (Exception) { return schedule; }
        }


        /// <summary>
        /// Obtains a photo related to place by requesting it to Google Maps API.
        /// </summary>
        /// <param name="pReference">Reference of the photo given by the API</param>
        /// <returns>Base 64 string representation of the photo</returns>
        public string RequestPlacePhoto(string pReference)
        {
            string apiURL = @"https://maps.googleapis.com/maps/api/place/photo?maxwidth=150&photoreference=";
            string url = apiURL + pReference + "&key="+ "AIzaSyBrQ-Pb3As0dKkt1iPsxL9IOr-Nfk3E1Cc";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (Stream stream = httpWebReponse.GetResponseStream())
                {
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }

                    string base64 = Convert.ToBase64String(bytes);
                    return base64;
                }
            }
        }


        /// <summary>
        /// Obtains the distance in meters and kilometers between two sets of coordinates
        /// </summary>
        /// <param name="pLatitude1">Latitude of first point</param>
        /// <param name="pLongitude1">Longitude of first point</param>
        /// <param name="pLatitude2">Latitude of second point</param>
        /// <param name="pLongitude2">Longitude of second point</param>
        /// <returns></returns>
        public ExpandoObject DistanceByCoordinates(double pLatitude1, double pLongitude1, double pLatitude2, double pLongitude2)
        {
            DistanceMatrixRequest request = new DistanceMatrixRequest();
            request.AddDestination(new LatLng(latitude: pLatitude2, longitude: pLongitude2));
            request.AddOrigin(new LatLng(latitude: pLatitude1, longitude: pLongitude1));

            request.Mode = TravelMode.driving;

            DistanceMatrixResponse response = new DistanceMatrixService().GetResponse(request);
            dynamic data = new ExpandoObject();
            if (response.Status == ServiceResponseStatus.Ok)
            {
                data.meters = response.Rows[0].Elements[0].distance.Value;
                data.kilometers = response.Rows[0].Elements[0].distance.Text;
                return data;
            }
            else return null;
        }


    }
}
