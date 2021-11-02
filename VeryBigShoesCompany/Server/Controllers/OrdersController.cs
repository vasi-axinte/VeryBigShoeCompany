using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using VeryBigShoesCompany.Server.Services;
using VeryBigShoesCompany.Shared;

namespace VeryBigShoesCompany.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public OrdersController(IWebHostEnvironment environment, OrdersService ordersService)
        {
            Environment = environment;
            OrdersService = ordersService;
        }

        private  IWebHostEnvironment Environment { get; }

        private OrdersService OrdersService { get; }

        [HttpGet]
        public IEnumerable<Shared.Order> Get()
        {
            return OrdersService.GetOrders();
        }

        [HttpPost]
        public IActionResult Post(UploadedFile uploadedFile)
        {
            var memoryStream = new MemoryStream(uploadedFile.FileContent);
            var xmlReader = XmlReader.Create(memoryStream);
            var doc = XDocument.Load(xmlReader);

            try
            {
                ValidateXml(doc);
            }
            catch (Exception ex)
            {
                return new UnprocessableEntityObjectResult(ex.Message);
            }

            var orders = GetOrdersFromXml(doc);

            try
            {
                OrdersService.AddOrders(orders);

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new UnprocessableEntityObjectResult(ex.Message);
            }
        }

        private List<Shared.Order> GetOrdersFromXml(XDocument doc)
        {
            BigShoeDataImport data = null;
            var serializer = new XmlSerializer(typeof(BigShoeDataImport));

            data = (BigShoeDataImport)serializer.Deserialize(doc.Root.CreateReader());
            return data.Orders.Select(d => new Shared.Order
            {
                CustomerName = d.CustomerName,
                CustomerEmail = d.CustomerEmail,
                Quantity = d.Quantity,
                Notes = d.Notes,
                Size = d.Size,
                DateRequired = d.DateRequired,
            }).ToList();
        }

        void ValidateXml(XDocument doc)
        {
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add("", $"{Environment.WebRootPath}\\Resources\\OrderImport.xsd");
            doc.Validate(schema, ValidationEventHandler);
        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlSeverityType type;
            if (Enum.TryParse("Error", out type))
            {
                if (type == XmlSeverityType.Error) throw new Exception(e.Message);
            }
        }
    }
}
