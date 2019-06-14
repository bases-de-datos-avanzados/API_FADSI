using API_FADSI.Models;
using Google.Maps;
using Google.Maps.DistanceMatrix;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Services
{
    public class PlaceService
    {
        // Holds the collection "Place" of the database
        private readonly IMongoCollection<Place> _places;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="config"></param>
        public PlaceService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString(config.GetSection("DataBases")["Mongo"]));
            var database = client.GetDatabase(config.GetSection("DataBases")["Mongo"]);
            _places = database.GetCollection<Place>(CONSTANTS_PLACE.COLLECTION_PLACE);
        }

        /// <summary>
        /// Method to determine the id of the next place to insert in the database.
        /// </summary>
        /// <returns></returns>
        private string GetPlaceId()
        {
            int nextId = 0;
            var project = new BsonDocument { { "$project", new BsonDocument { { CONSTANTS_PLACE.ID, 1 } } } };
            var pipeline = new[] { project };

            List<Place> ids = _places.Aggregate<Place>(pipeline).ToList();
            if (ids.Count == 0) return CONSTANTS_PLACE.ID_PREFIX + 0;
           
            else
            {
                List<int> intIds = new List<int>();
                for (int i = 0; i < ids.Count; ++i)
                {
                    intIds.Add(Int32.Parse(ids[i].Id.Substring(CONSTANTS_PLACE.ID_PREFIX.Length,
                        ids[i].Id.Length - CONSTANTS_PLACE.ID_PREFIX.Length)));
                }
                intIds.Sort();

                for (int i = 0, j=0; i<=intIds.Count; ++i)
                {
                    if (i == intIds.Count) { nextId = j; break; }
                    else if (intIds[i] == j) j++;
                    else
                    {
                        nextId = j;
                        break;
                    }
                }
            }
            return CONSTANTS_PLACE.ID_PREFIX + nextId;
        }


        /// <summary>
        /// Obtains all the documents in the collection "Place"
        /// </summary>
        /// <returns></returns>
        public List<Place> Get()
        {
            return _places.Find(place => true).ToList();
        }


        /// <summary>
        /// Obtains a single document of the collection "Place" that matches the given Id (_id)
        /// </summary>
        /// <param name="pId">Id of the document</param>
        /// <returns></returns>
        public Place Get(string pId)
        {
            var filter = Builders<Place>.Filter.Eq(CONSTANTS_PLACE.ID, pId);
            return _places.Find<Place>(filter).FirstOrDefault();
        }


        /// <summary>
        /// Create a new document inside the collection "Place"
        /// </summary>
        /// <param name="pPlace">New place to be created</param>
        /// <returns>0 if successful, -1 if there is an error</returns>
        public int Create(Place pPlace)
        {
            try
            {
                pPlace.Id = GetPlaceId();
                _places.InsertOne(pPlace);
            }
            catch (Exception e)
            {
                return -1;
            }

            return 0;
        }


        /// <summary>
        /// Update a document of the collection "Place", searching it by its Id (_id) 
        /// </summary>
        /// <param name="pId">Id of the place</param>
        /// <param name="pPlace">New place with updated data</param>
        /// <returns>0 if successful, -1 if there is an error</returns>
        public int Update(string pId, Place pPlace)
        {
            var filter = Builders<Place>.Filter.Eq(CONSTANTS_PLACE.ID, pId);
            Place oldPlace = _places.Find<Place>(filter).FirstOrDefault();
            try
            {
                _places.DeleteOne(place => place.Id == pId);
                _places.InsertOne(pPlace);
            }
            catch (Exception e)
            {
                if (oldPlace == null) return -1;
                else _places.InsertOne(oldPlace);
            }
            return 0;
        }


        /// <summary>
        /// Deletes a document in the collection "Place" that has the specified Id (_id)
        /// </summary>
        /// <param name="pId">Id of the place to be deleted</param>
        public void Remove(string pId)
        {
            _places.DeleteOne(place => place.Id == pId);
        }


        /// <summary>
        /// Obtains all the documents in the collection "Place" with a matching "type" field
        /// </summary>
        /// <param name="pType">Integer that represents the type of place</param>
        /// <returns></returns>
        public List<Place> PlacesByType(int pType)
        {
            var filter = Builders<Place>.Filter.Eq(CONSTANTS_PLACE.TYPE, CONSTANTS_PLACE.PLACE_TYPE[pType]);
            return _places.Find(filter).ToList();
        }


        public List<dynamic> NearbyPlaces(string pPlaceId)
        {
            List<dynamic> nearbyPlaces = new List<dynamic>();
            var queryCheck = Builders<Place>.Filter.Eq(CONSTANTS_PLACE.ID, pPlaceId);
            Place origin = _places.Find(queryCheck).FirstOrDefault();
            if (origin != null)
            {
                List<Place> registeredPlaces;
                var query = new BsonDocument { { CONSTANTS_PLACE.ID, new BsonDocument { { "$ne", pPlaceId } } } };
                registeredPlaces = _places.Find(query).ToList();

                foreach (Place place in registeredPlaces)
                {
                    dynamic data = new ExpandoObject();
                    dynamic distance = DistanceByCoordinates(origin.Latitude, origin.Longitude, 
                        place.Latitude, place.Longitude);
                    data.id = place.Id;
                    data.name = place.Name;
                    data.meters = distance.meters;
                    data.kilometers = distance.kilometers;
                    nearbyPlaces.Add(data);
                }
                return nearbyPlaces;
            }
            else return null;
            
        }



        private ExpandoObject DistanceByCoordinates(double pLatitude1, double pLongitude1, double pLatitude2, double pLongitude2)
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
