using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using RestSharp;
using Newtonsoft.Json;
using System.Text.Json;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var client = new RestClient("https://getinvoices.azurewebsites.net/api");
            var request = new RestRequest("/Customers", Method.Get);
            var response = client.Execute(request);
            var model = JsonConvert.DeserializeObject<List<CustomerModel>>(response.Content);
            return View(model);
        }
        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult DeleteCustomer(string id = "0")
        {
            var client = new RestClient("https://getinvoices.azurewebsites.net/api/");
            var request = new RestRequest("/Customer/"+id, Method.Delete);
            RestResponse response = client.Execute(request);
            return RedirectToAction("Index");
        }
        [Route("create")]
        [Route("edit/{id}")]
        [HttpGet]
        public IActionResult CreateCustomer(string id = "0")
        {
            if (id == "0")
            {
                var customerModel = new CustomerModel();
                return View(customerModel);
            }
            else
            {
                var client = new RestClient("https://getinvoices.azurewebsites.net/api");
                var request = new RestRequest("Customer/" + id);
                var response = client.ExecuteGet(request);
                // deserialize json string response to JsonNode object
                var data = JsonConvert.DeserializeObject<CustomerModel>(response.Content);
                return View(data);
            }

        }
        [HttpPost]
        [Route("create")]
        [Route("edit/{id}")]
        public IActionResult CreateCustomer(CustomerModel customerModel)
        {
            if (customerModel.id == null)
            {
                var client = new RestClient("https://getinvoices.azurewebsites.net/api");
                var request = new RestRequest("Customer/", Method.Post);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(customerModel);
                var response = client.ExecutePost(request);
                return RedirectToAction("Index");
            }
            else
            {
                var client = new RestClient("https://getinvoices.azurewebsites.net/api");
                var request = new RestRequest("/Customer?id="+customerModel.id, Method.Post);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(customerModel); 
                var response = client.ExecutePost(request);
                var data = JsonConvert.DeserializeObject<CustomerModel>(response.Content);
                // deserialize json string response to JsonNode object
                return RedirectToAction("Index");
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
