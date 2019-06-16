using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Models
{
    public class RepOrderHistory
    {
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string OrderId { get; set; }
        public int Total { get; set; }
        public string PlaceName { get; set; }
        public List<ProductHistory> Products { get; set; }
    }

    public class ProductHistory
    {
        public string name { get; set; }
        public int quantity { get; set; }
    }
}
