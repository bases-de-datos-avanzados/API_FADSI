using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Models
{
    static class CONSTANTS_ORDER
    {
        public const string COLLECTION_ORDER = "Order";
        public const string ID_PREFIX = "ORDER-";
        public const string ID = "_id";
        public const string USER_ID = "User_id";
        public const string DATE_TIME = "dateTime";
        public const string STATUS = "status";
        public const string EXTRAS = "extras";
        public const string TOTAL = "total";
        public const string PLACE_ID = "Place_id";
        public const string PRODUCTS = "products";
        public const string RELATED = "related";
        public const string SUB_PROD_ID = "Product_id";
        public const string SUB_PROD_QUANTITY = "quantity";
        public const string SUB_RELATED_ID = "Place_id";

        public static readonly Dictionary<int, string> STATES = new Dictionary<int, string>() {
            {1, "Registered" },
            {2, "Assigned" },
            {3, "On route" },
            {4, "Delivered" }
        };
    }


    public class Order
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; } = "";

        [BsonElement(CONSTANTS_ORDER.USER_ID)]
        public string UserId { get; set; }

        [BsonElement(CONSTANTS_ORDER.DATE_TIME)]
        public DateTime Date_Time { get; set; }

        [BsonElement(CONSTANTS_ORDER.STATUS)]
        public string Status { get; set; }

        [BsonElement(CONSTANTS_ORDER.EXTRAS)]
        public string Extras { get; set; }

        [BsonElement(CONSTANTS_ORDER.TOTAL)]
        public int Total { get; set; }

        [BsonElement(CONSTANTS_ORDER.PLACE_ID)]
        public string PlaceId { get; set; }

        [BsonElement(CONSTANTS_ORDER.PRODUCTS)]
        public List<OrderProduct> Products {get; set; }

        [BsonElement(CONSTANTS_ORDER.RELATED)]
        public List<RelatedPlace> Related { get; set; }
    }


    public class OrderProduct
    {   
        [BsonElement(CONSTANTS_ORDER.SUB_PROD_ID)]
        public string ProductId { get; set; }

        [BsonElement(CONSTANTS_ORDER.SUB_PROD_QUANTITY)]
        public int Quantity { get; set; }
    }

    public class RelatedPlace
    {
        [BsonElement(CONSTANTS_ORDER.SUB_RELATED_ID)]
        public string Place_Id { get; set; }
    }


}
