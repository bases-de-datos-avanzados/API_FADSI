using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Models
{
    public class RepRelatedClients
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientLName { get; set; }
        public List<Stores> Stores { get; set; }
        public List<RelatedUsers> OtherUsers { get; set; }

        
    }

    public class Stores
    {
        public string Place_Id { get; set; }
        public string Place_Name { get; set; }
        public string Place_Desc { get; set; }
    }


    public class RelatedUsers
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserLastName { get; set; }
        public string PlaceId { get; set; }
        public string PlaceName { get; set; }
        public DateTime Date { get; set; }
        public string OrderId { get; set; }
    }
}
