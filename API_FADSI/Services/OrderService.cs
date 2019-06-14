using API_FADSI.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Services
{
    public class OrderService
    {
        // Holds the collection "Order" of the database
        private readonly IMongoCollection<Order> _orders;


        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="config"></param>
        public OrderService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString(config.GetSection("DataBases")["Mongo"]));
            var database = client.GetDatabase(config.GetSection("DataBases")["Mongo"]);
            _orders = database.GetCollection<Order>(CONSTANTS_ORDER.COLLECTION_ORDER);
        }


        /// <summary>
        /// Method to determine the id of the next order to insert in the database.
        /// </summary>
        /// <returns></returns>
        private string GetOrderId()
        {
            int nextId = 0;
            var project = new BsonDocument { { "$project", new BsonDocument { { CONSTANTS_ORDER.ID, 1 } } } };
            var pipeline = new[] { project };

            List<Order> ids = _orders.Aggregate<Order>(pipeline).ToList();
            if (ids.Count == 0) return CONSTANTS_ORDER.ID_PREFIX + 0;

            else
            {
                List<int> intIds = new List<int>();
                for (int i = 0; i < ids.Count; ++i)
                {
                    intIds.Add(Int32.Parse(ids[i].Id.Substring(CONSTANTS_ORDER.ID_PREFIX.Length,
                        ids[i].Id.Length - CONSTANTS_ORDER.ID_PREFIX.Length)));
                }
                intIds.Sort();

                for (int i = 0, j = 0; i <= intIds.Count; ++i)
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
            return CONSTANTS_ORDER.ID_PREFIX + nextId;
        }


        /// <summary>
        /// Obtains all the documents in the collection "Order"
        /// </summary>
        /// <returns></returns>
        public List<Order> Get()
        {
            return _orders.Find(order => true).ToList();
        }


        /// <summary>
        /// Obtains a single document of the collection "Order" that matches the given Id (_id)
        /// </summary>
        /// <param name="pId">Id of the document</param>
        /// <returns></returns>
        public Order Get(string pId)
        {
            var filter = Builders<Order>.Filter.Eq(CONSTANTS_ORDER.ID, pId);
            return _orders.Find<Order>(filter).FirstOrDefault();
        }


        /// <summary>
        /// Obtains all the documents of the collection "Order" with a matching User_id field
        /// </summary>
        /// <param name="pUserId">Id of the user who made the order</param>
        /// <returns></returns>
        public List<Order> GetUserOrders(string pUserId)
        {
            var filter = Builders<Order>.Filter.Eq(CONSTANTS_ORDER.USER_ID, pUserId);
            return _orders.Find(filter).ToList<Order>();
        }


        /// <summary>
        /// Create a new document inside the collection "Order"
        /// </summary>
        /// <param name="pOrder">New order to be created</param>
        /// <returns>0 if successful, -1 if there is an error</returns>
        public int Create(Order pOrder)
        {
            try
            {
                pOrder.Id = GetOrderId();
                _orders.InsertOne(pOrder);
            }
            catch (Exception e)
            {
                return -1;
            }

            return 0;
        }


        /// <summary>
        /// Update a document of the collection "Order", searching it by its Id (_id) 
        /// </summary>
        /// <param name="pId">Id of the order</param>
        /// <param name="pOrder">New order with updated data</param>
        /// <returns>0 if successful, -1 if there is an error</returns>
        public int Update(string pId, Order pOrder)
        {
            var filter = Builders<Order>.Filter.Eq(CONSTANTS_ORDER.ID, pId);
            Order oldOrder = _orders.Find<Order>(filter).FirstOrDefault();
            try
            {
                _orders.DeleteOne(order => order.Id == pId);
                _orders.InsertOne(pOrder);
            }
            catch (Exception e)
            {
                if (oldOrder == null) return -1;
                else _orders.InsertOne(oldOrder);
            }
            return 0;
        }


        /// <summary>
        /// Deletes a document in the collection "Order" that has the specified Id (_id)
        /// </summary>
        /// <param name="pId">Id of the order to be deleted</param>
        public void Remove(string pId)
        {
            _orders.DeleteOne(order => order.Id == pId);
        }


    }
}
