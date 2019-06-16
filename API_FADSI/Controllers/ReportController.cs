using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_FADSI.Models;
using API_FADSI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_FADSI.Controllers
{
    [ApiController]
    public class ReportController : ControllerBase
    {
        private const string REPORT_URL = "api/reports";
        private readonly ReportService _reportService;

        /// <summary>
        /// Class constructor 
        /// </summary>
        /// <param name="pReportService"></param>
        public ReportController(ReportService pReportService)
        {
            _reportService = pReportService;
        }


        [Route(REPORT_URL + "/user/{pUserId}")]
        [HttpGet]
        public ActionResult<dynamic> SearchUser(string pUserId)
        {
            return _reportService.SearchUser(pUserId);
        }


        [Route(REPORT_URL + "/orders/user/{pUserId}")]
        [HttpGet]
        public ActionResult<List<RepOrderHistory>> UserOrderHistory(string pUserId)
        {
            return _reportService.UserOrderHistory(pUserId);
        }


        /// <summary>
        /// Obtains a list of places where user have made orders
        /// </summary>
        /// <returns></returns>
        [Route(REPORT_URL + "/orders/places")]
        [HttpGet]
        public ActionResult<List<Place>> PlacesWithOrders()
        {
            return _reportService.PlacesWithOrders();
        }


        [Route(REPORT_URL + "/places/top5")]
        [HttpGet]
        public ActionResult<List<dynamic>> Top5Places()
        {
            return _reportService.Top5Places();
        }


        [Route(REPORT_URL + "/relatedUsers/{pUserId}")]
        [HttpGet]
        public ActionResult<RepRelatedClients> RelatedUsers(string pUserId)
        {
            return _reportService.RelatedUserOrders(pUserId);
        }


    }



}