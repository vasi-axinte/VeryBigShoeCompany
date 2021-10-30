using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using VeryBigShoesCompany.Server.Repositories;
using VeryBigShoesCompany.Shared;

namespace VeryBigShoesCompany.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public OrdersController(IWebHostEnvironment environment, InMemoryOrdersRepository repository)
        {
            Environment = environment;
            Repository = repository;
        }

        private  IWebHostEnvironment Environment { get; }

        private InMemoryOrdersRepository Repository { get; }

        [HttpGet]
        public IEnumerable<Shared.Order> Get()
        {
            return Repository.Orders.ToArray();
        }

        [HttpPost]
        public void Post(UploadedFile uploadedFile)
        {
            var path = $"{Environment.WebRootPath}\\{uploadedFile.FileName}";

            var memoryStream = new MemoryStream(uploadedFile.FileContent);
            XmlReader xmlReader = XmlReader.Create(memoryStream);
            XDocument doc = XDocument.Load(xmlReader);

            ValidateXml(doc);

            BigShoeDataImport data = null;
            XmlSerializer serializer = new XmlSerializer(typeof(BigShoeDataImport));

            data = (BigShoeDataImport)serializer.Deserialize(doc.Root.CreateReader());

            Repository.Orders.AddRange(data.Orders.Select(d => new Shared.Order
            {
                CustomerName = d.CustomerName,
                CustomerEmail = d.CustomerEmail,
                Quantity = d.Quantity,
                Notes = d.Notes,
                Size = d.Size,
                DateRequired = d.DateRequired,
            }).ToList());
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
