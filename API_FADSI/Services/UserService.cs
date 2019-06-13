using API_FADSI.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Services
{
    public class UserService
    {
        // Holds the collection "Users" of the database
        private readonly IMongoCollection<User> _users;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="config"></param>
        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString(config.GetSection("DataBases")["Mongo"]));
            var database = client.GetDatabase(config.GetSection("DataBases")["Mongo"]);
            _users = database.GetCollection<User>(CONSTANTS_USER.COLLECTION_USER);
        }


        /// <summary>
        /// Obtains all the documents in the collection "Users"
        /// </summary>
        /// <returns></returns>
        public List<User> Get()
        {
            return _users.Find(user => true).ToList();
        }


        /// <summary>
        /// Obtains a single document of the collection "Users" that matches the given Id (_id)
        /// </summary>
        /// <param name="pId">Id of the document</param>
        /// <returns></returns>
        public User Get(string pId)
        {
            var filter = Builders<User>.Filter.Eq(CONSTANTS_USER.ID, pId);
            return _users.Find<User>(filter).FirstOrDefault();
        }


        /// <summary>
        /// Create a new document inside the collection "User"
        /// </summary>
        /// <param name="pUser">New user to be created</param>
        /// <returns>0 if successful, -1 if there is an error</returns>
        public int Create(User pUser)
        {
            try
            {
                _users.InsertOne(pUser);
            }
            catch (Exception e)
            {
                return -1;
            }

            return 0;
        }


        /// <summary>
        /// Update a document of the collection "Users", searching it by its Id (_id) 
        /// </summary>
        /// <param name="pId">Id of the user</param>
        /// <param name="pUser">New user with updated data</param>
        /// <returns>0 if successful, -1 if there is an error</returns>
        public int Update(string pId, User pUser)
        {
            var filter = Builders<User>.Filter.Eq(CONSTANTS_USER.ID, pId);
            User oldUser = _users.Find<User>(filter).FirstOrDefault();
            try
            {
                _users.DeleteOne(user => user.Id == pId);
                _users.InsertOne(pUser);
            }
            catch (Exception e)
            {
                if(oldUser==null) return -1;
                else _users.InsertOne(oldUser);
            }
            return 0;
        }


        /// <summary>
        /// Deletes a document in the collection "Users" that has the specified Id (_id)
        /// </summary>
        /// <param name="pId">Id of the user to be deleted</param>
        public void Remove(string pId)
        {
            _users.DeleteOne(user => user.Id == pId);
        }


        /// <summary>
        /// Return the data of a document with matching username
        /// </summary>
        /// <param name="pUserName">User name</param>
        /// <returns></returns>
        private ExpandoObject GetByUserName(string pUserName)
        {
            var match = new BsonDocument("$match", new BsonDocument(CONSTANTS_USER.USER_NAME, pUserName));
            var project = new BsonDocument { { "$project",
                    new BsonDocument { { CONSTANTS_USER.ID, 1}, { CONSTANTS_USER.FIRSTNAME, 1},
                        { CONSTANTS_USER.LASTNAME, 1}, { CONSTANTS_USER.USER_NAME, 1},
                        { CONSTANTS_USER.PASS, 1}, { CONSTANTS_USER.USER_TYPE, 1} } } };
            var pipeline = new[] { match, project };
            return _users.Aggregate<ExpandoObject>(pipeline).FirstOrDefault();
        }


        /// <summary>
        /// Return the data of a user
        /// </summary>
        /// <param name="pUserName"></param>
        /// <returns></returns>
        public ExpandoObject Login(string pUserName)
        {
            dynamic user = this.GetByUserName(pUserName);
            if (user == null) return null;

            return user;
        }

    }
}
