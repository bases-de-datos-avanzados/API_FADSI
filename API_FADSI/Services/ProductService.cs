using API_FADSI.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Services
{
    public class ProductService
    {
        // Holds the collection "Product" of the database
        private readonly IMongoCollection<Product> _products;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="config"></param>
        public ProductService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString(config.GetSection("DataBases")["Mongo"]));
            var database = client.GetDatabase(config.GetSection("DataBases")["Mongo"]);
            _products = database.GetCollection<Product>(CONSTANTS_PRODUCT.COLLECTION_PRODUCT);
        }


        /// <summary>
        /// Method to determine the id of the next product to insert in the database.
        /// </summary>
        /// <returns></returns>
        private string GetProductId()
        {
            int nextId = 0;
            var project = new BsonDocument { { "$project", new BsonDocument { { CONSTANTS_PRODUCT.ID, 1 } } } };
            var pipeline = new[] { project };

            List<Product> ids = _products.Aggregate<Product>(pipeline).ToList();
            if (ids.Count == 0) return CONSTANTS_PRODUCT.ID_PREFIX + 0;

            else
            {
                List<int> intIds = new List<int>();
                for (int i = 0; i < ids.Count; ++i)
                {
                    intIds.Add(Int32.Parse(ids[i].Id.Substring(CONSTANTS_PRODUCT.ID_PREFIX.Length,
                        ids[i].Id.Length - CONSTANTS_PRODUCT.ID_PREFIX.Length)));
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
            return CONSTANTS_PRODUCT.ID_PREFIX + nextId;
        }


        /// <summary>
        /// Obtains all the documents in the collection "Product"
        /// </summary>
        /// <returns></returns>
        public List<Product> Get()
        {
            return _products.Find(product => true).ToList();
        }


        /// <summary>
        /// Obtains a single document of the collection "Product" that matches the given Id (_id)
        /// </summary>
        /// <param name="pId">Id of the document</param>
        /// <returns></returns>
        public Product Get(string pId)
        {
            var filter = Builders<Product>.Filter.Eq(CONSTANTS_PRODUCT.ID, pId);
            return _products.Find(filter).FirstOrDefault();
        }


        /// <summary>
        /// Obtains all the documents of the collection "Product" with a matching Place_id field
        /// </summary>
        /// <param name="pPlaceId">Id of the place where the product is sold</param>
        /// <returns></returns>
        public List<Product> GetPlaceProducts(string pPlaceId)
        {
            var filter = Builders<Product>.Filter.Eq(CONSTANTS_PRODUCT.PLACE_ID, pPlaceId);
            return _products.Find(filter).ToList<Product>();
        }


        /// <summary>
        /// Create a new document inside the collection "Product"
        /// </summary>
        /// <param name="pProduct">New product to be created</param>
        /// <returns>0 if successful, -1 if there is an error</returns>
        public int Create(Product pProduct)
        {
            try
            {
                pProduct.Id = GetProductId();
                Debug.WriteLine("ID GENERADO: " + pProduct.Id);
                _products.InsertOne(pProduct);
            }
            catch (Exception e)
            {
                return -1;
            }

            return 0;
        }


        /// <summary>
        /// Update a document of the collection "Product", searching it by its Id (_id) 
        /// </summary>
        /// <param name="pId">Id of the product</param>
        /// <param name="pProduct">New product with updated data</param>
        /// <returns>0 if successful, -1 if there is an error</returns>
        public int Update(string pId, Product pProduct)
        {
            var filter = Builders<Product>.Filter.Eq(CONSTANTS_PRODUCT.ID, pId);
            Product oldProduct = _products.Find<Product>(filter).FirstOrDefault();
            try
            {
                _products.DeleteOne(product => product.Id == pId);
                _products.InsertOne(pProduct);
            }
            catch (Exception e)
            {
                if (oldProduct == null) return -1;
                else _products.InsertOne(oldProduct);
            }
            return 0;
        }


        /// <summary>
        /// Deletes a document in the collection "Product" that has the specified Id (_id)
        /// </summary>
        /// <param name="pId">Id of the product to be deleted</param>
        public void Remove(string pId)
        {
            _products.DeleteOne(product => product.Id == pId);
        }


    }
}
