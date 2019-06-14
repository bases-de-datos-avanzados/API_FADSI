using API_FADSI.Models;
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
            return _places.Find(filter).ToList<Place>();
        }

    }
}
