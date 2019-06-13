using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Models
{
    static class CONSTANTS_USER
    {
        public const string COLLECTION_USER = "User";
        public const string ID = "_id";
        public const string FIRSTNAME = "firstName";
        public const string LASTNAME = "lastName";
        public const string BIRTHDAY = "birthday";
        public const string PHONE = "phone";
        public const string EMAIL = "email";
        public const string USER_NAME = "userName";
        public const string PASS = "pass";
        public const string USER_TYPE = "userType";
        public const string ORDERS = "orders";
    }

    public class User
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        [BsonElement(CONSTANTS_USER.FIRSTNAME)]
        public string Firstname { get; set; }

        [BsonElement(CONSTANTS_USER.LASTNAME)]
        public string Lastname { get; set; }

        [BsonElement(CONSTANTS_USER.BIRTHDAY)]
        public DateTime Birthday { get; set; }

        [BsonElement(CONSTANTS_USER.PHONE)]
        public string Phone { get; set; }

        [BsonElement(CONSTANTS_USER.EMAIL)]
        public string Email { get; set; }

        [BsonElement(CONSTANTS_USER.USER_NAME)]
        public string UserName { get; set; }

        [BsonElement(CONSTANTS_USER.PASS)]
        public string Pass { get; set; }

        [BsonElement(CONSTANTS_USER.USER_TYPE)]
        public string UserType { get; set; }

        //[BsonElement(CONSTANTS_USER.ORDERS)]
        //public List<Order> Orders { get; set; }
    }
}
