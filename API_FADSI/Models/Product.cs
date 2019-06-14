using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Models
{
    static class CONSTANTS_PRODUCT
    {
        public const string ID_PREFIX = "PROD-";
        public const string COLLECTION_PRODUCT = "Product";
        public const string ID = "_id";
        public const string NAME = "name";
        public const string DESCRIPTION = "description";
        public const string PRICE = "price";
        public const string PHOTO = "photo";
        public const string PLACE_ID = "Place_id";
    }


    public class Product
    {
        [BsonElement(CONSTANTS_PRODUCT.ID)]
        public string Id { get; set; } = "";

        [BsonElement(CONSTANTS_PRODUCT.NAME)]
        public string Name { get; set; }

        [BsonElement(CONSTANTS_PRODUCT.DESCRIPTION)]
        public string Description { get; set; }

        [BsonElement(CONSTANTS_PRODUCT.PRICE)]
        public int Price { get; set; }

        [BsonElement(CONSTANTS_PRODUCT.PHOTO)]
        public string Photo { get; set; }

        [BsonElement(CONSTANTS_PRODUCT.PLACE_ID)]
        public string PlaceId { get; set; }
    }


}
