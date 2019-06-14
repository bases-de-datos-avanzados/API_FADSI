using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Models
{
    static class CONSTANTS_PLACE
    {
        public const string ID_PREFIX = "PLACE-";
        public const string COLLECTION_PLACE = "Place";
        public const string ID = "_id";
        public const string NAME = "name";
        public const string DESCRIPTION = "description";
        public const string LATITUDE = "latitude";
        public const string LONGITUDE = "longitude";
        public const string ADDRESS = "address";
        public const string TYPE = "type";
        public const string PHONE = "phone";
        public const string RATING = "rating";
        public const string SCHEDULE = "schedule";
        public const string WEBSITE = "website";
        public const string PHOTO = "photo";
        public const string STAFF_AMOUNT = "staffAmount";

        public static readonly Dictionary<int, string> PLACE_TYPE = new Dictionary<int, string>() {
            {1, "Restaurant" },
            {2, "Supermarket" },
            {3, "Drugstore" },
            {4, "Mechanic" },
            {5, "Bar" }
        }; 
    }

    public class Place
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; } = "";

        [BsonElement(CONSTANTS_PLACE.NAME)]
        public string Name { get; set; }

        [BsonElement(CONSTANTS_PLACE.DESCRIPTION)]
        public string Description { get; set; }

        [BsonElement(CONSTANTS_PLACE.LATITUDE)]
        public string Latitude { get; set; }

        [BsonElement(CONSTANTS_PLACE.LONGITUDE)]
        public string Longitude { get; set; }

        [BsonElement(CONSTANTS_PLACE.ADDRESS)]
        public string Address { get; set; }

        [BsonElement(CONSTANTS_PLACE.TYPE)]
        public string Type { get; set; }

        [BsonElement(CONSTANTS_PLACE.PHONE)]
        public string Phone { get; set; } = "";

        [BsonElement(CONSTANTS_PLACE.RATING)]
        public float Rating { get; set; } = 0.0f;

        [BsonElement(CONSTANTS_PLACE.SCHEDULE)]
        public string Schedule { get; set; } = "";

        [BsonElement(CONSTANTS_PLACE.WEBSITE)]
        public string Website { get; set; } = "";

        [BsonElement(CONSTANTS_PLACE.PHOTO)]
        public string Photo { get; set; } = "";

        [BsonElement(CONSTANTS_PLACE.STAFF_AMOUNT)]
        public int StaffAmount { get; set; }
    }
}
