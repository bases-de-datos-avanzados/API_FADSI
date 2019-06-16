using API_FADSI.Models;
using Microsoft.Extensions.Configuration;
using Neo4jClient;
using Neo4jClient.Cypher;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace API_FADSI.Services
{
    public class ReportService
    {
        ///Connection with the Neo4j Graph
        private readonly GraphClient _client;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="config"></param>
        public ReportService(IConfiguration config)
        {
            _client = new GraphClient(new Uri(config.GetConnectionString("NEO4J")));
            _client.Connect();
        }


        public dynamic SearchUser(string pUserId)
        {
            var query = _client.Cypher
               .Match("(user:User)")
               .Where("user._id = {userId}")
               .WithParam("userId", pUserId)
               .Return((user) => new { User = user.As<Node<string>>() });

            var results = query.Results.ToList();

            return JsonConvert.DeserializeObject<dynamic>(results.FirstOrDefault().User.Data);
        }


        /// <summary>
        /// REPORT 1 ADMIN 
        /// Obtains the history of orders of one client
        /// </summary>
        /// <param name="pUserId">Client id</param>
        /// <returns></returns>
        public List<RepOrderHistory> UserOrderHistory(string pUserId)
        {
            //dynamic data = new ExpandoObject();
            dynamic test = new ExpandoObject();

            var query = _client.Cypher
                .Match("(user:User)<--(ord:Order)-->(n)-->(prod:Product)")
                .Where("user._id = {userId}")
                .OptionalMatch("(ord)-[rel:Order_Place]-(place:Place)")
                .WithParam("userId", pUserId)
                .Return((user, ord, place, prod, n) => new {
                    UserId = Return.As<string>("user._id"),
                    UserFirstName = Return.As<string>("user.name"),
                    UserLastName = Return.As<string>("user.lastName"),
                    OrderId = Return.As<string>("ord._id"),
                    OrderTotal = Return.As<int>("ord.total"),
                    OrderPlace = Return.As<string>("place.name"),
                    Products = Return.As<IEnumerable<ProductHistory>>("collect({name:prod.name, quantity:n.quantity})")
                });

            var results = query.Results.ToList();
            List<RepOrderHistory> orders = new List<RepOrderHistory>();

            for(int i=0; i<results.Count; ++i)
            {
                RepOrderHistory order = new RepOrderHistory();
                order.UserId = results[i].UserId;
                order.UserFirstName = results[i].UserFirstName;
                order.UserLastName = results[i].UserLastName;
                order.OrderId = results[i].OrderId;
                order.Total = results[i].OrderTotal;
                order.PlaceName = results[i].OrderPlace;
                order.Products = results[i].Products.ToList();
                orders.Add(order);
            }

            return orders;
        }


        /// <summary>
        /// Obtains a list of places where user have made orders
        /// </summary>
        /// <returns></returns>
        public List<Place> PlacesWithOrders()
        {
            var query = _client.Cypher
              .Match("(user:User)<--(ord:Order)-[r:Order_Place]-(p:Place)")
              .With("distinct p.`_id` as id, p.name as name, p.description as description, " +
              "p.address as address, p.type as type, p.phone as phone, p.rating as rating, " +
              "p.schedule as schedule, p.website as website, p.photo as photo, p.staffAmount as staffAmount")
              .Return((p) => new {
                  Id = Return.As<string>("id"),
                  Name = Return.As<string>("name"),
                  Descrption = Return.As<string>("description"),
                  Address = Return.As<string>("address"),
                  Type = Return.As<string>("type"),
                  Phone = Return.As<string>("phone"),
                  Rating = Return.As<float>("rating"),
                  Schedule = Return.As<string>("schedule"),
                  Website = Return.As<string>("website"),
                  Photo = Return.As<string>("photo"),
                  StaffAmount = Return.As<int>("staffAmount")
              });

            var results = query.Results.ToList();
            List<Place> places = new List<Place>();
            for(int i=0; i<results.Count; ++i)
            {
                Place place = new Place();
                place.Id = results[i].Id;
                place.Name = results[i].Name;
                place.Description = results[i].Descrption;
                place.Address = results[i].Address;
                place.Type = results[i].Type;
                place.Phone = results[i].Phone;
                place.Rating = results[i].Rating;
                place.Schedule = results[i].Schedule;
                place.Website = results[i].Website;
                place.Photo = results[i].Photo;
                place.StaffAmount = results[i].StaffAmount;
                places.Add(place);
            }
            return places;

        }

        /// <summary>
        /// Obtains the top 5 places with most orders
        /// </summary>
        /// <returns></returns>
        public List<dynamic> Top5Places()
        {
            var query = _client.Cypher
              .Match("(user:User)<--(ord:Order)-[r:Order_Place]-(p:Place)")
              .With("distinct p.`_id` as id, p.name as name, p.description as description, " +
              "p.address as address, p.type as type, p.phone as phone, p.rating as rating, " +
              "p.schedule as schedule, p.website as website, p.photo as photo, p.staffAmount as staffAmount," +
              "count(*) as ordersAmount")
              .Return((p) => new {
                  Id = Return.As<string>("id"),
                  Name = Return.As<string>("name"),
                  Descrption = Return.As<string>("description"),
                  Address = Return.As<string>("address"),
                  Type = Return.As<string>("type"),
                  Phone = Return.As<string>("phone"),
                  Rating = Return.As<float>("rating"),
                  Schedule = Return.As<string>("schedule"),
                  Website = Return.As<string>("website"),
                  Photo = Return.As<string>("photo"),
                  StaffAmount = Return.As<int>("staffAmount"),
                  OrdersAmount = Return.As<int>("ordersAmount")
              })
              .OrderByDescending("ordersAmount")
              .Limit(5);

            var results = query.Results.ToList();
            List<dynamic> places = new List<dynamic>();
            for (int i = 0; i < results.Count; ++i)
            {
                dynamic place = new ExpandoObject();
                place.Id = results[i].Id;
                place.Name = results[i].Name;
                place.Description = results[i].Descrption;
                place.Address = results[i].Address;
                place.Type = results[i].Type;
                place.Phone = results[i].Phone;
                place.Rating = results[i].Rating;
                place.Schedule = results[i].Schedule;
                place.Website = results[i].Website;
                place.Photo = results[i].Photo;
                place.StaffAmount = results[i].StaffAmount;
                place.OrdersAmount = results[i].OrdersAmount;
                places.Add(place);
            }
            return places;

        }


        public RepRelatedClients RelatedUserOrders(string pUserId)
        {
            var query = _client.Cypher
                .Match("(user:User)<--(ord:Order)-->(p:Place)")
                .Where("user._id={userId}")
                .Match("(user2:User)<--(ord2:Order)-->(p2:Place)")
                .Where("not user2._id={userId} and p2.`_id`=p.`_id`")
                .WithParam("userId", pUserId)
                .Return((user, p, user2, ord2, p2) => new {
                    ClientId = Return.As<string>("user._id"),
                    ClientName = Return.As<string>("user.name"),
                    ClientLName = Return.As<string>("user.lastName"),
                    Stores = Return.As<IEnumerable<Stores>>("collect(distinct {Place_Id : p.`_id`, " +
                        "Place_Name : p.name, Place_Desc : p.description})"),
                    OtherUsers = Return.As<IEnumerable<RelatedUsers>>("collect(distinct {UserId:user2.`_id`, UserName:user2.name, " +
                        "UserLastName:user2.lastName, PlaceId:p2.`_id`, PlaceName:p2.name, " +
                        "Date:ord2.dateTime, OrderId:ord2.`_id`})")
                });

            var results = query.Results.ToList();
            List<RepRelatedClients> orders = new List<RepRelatedClients>();

            
            RepRelatedClients data = new RepRelatedClients();
            data.ClientId = results[0].ClientId;
            data.ClientName = results[0].ClientName;
            data.ClientLName = results[0].ClientLName;
            data.Stores = results[0].Stores.ToList();
            data.OtherUsers = results[0].OtherUsers.ToList();

            return data;
        }




    }



}
