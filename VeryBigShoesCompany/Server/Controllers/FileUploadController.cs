using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using VeryBigShoesCompany.Shared;

namespace VeryBigShoesCompany.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;

        public FileUploadController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        [HttpPost]
        public void Post(UploadedFile uploadedFile)
        {
            var path = $"{environment.WebRootPath}\\{uploadedFile.FileName}";

            var memoryStream = new MemoryStream(uploadedFile.FileContent);
            XmlReader xmlReader = XmlReader.Create(memoryStream);
            XDocument doc = XDocument.Load(xmlReader);

            ValidateXml(doc);

            BigShoeDataImport data = null;
            XmlSerializer serializer = new XmlSerializer(typeof(BigShoeDataImport));

            data = (BigShoeDataImport)serializer.Deserialize(doc.Root.CreateReader());
        }

        void ValidateXml(XDocument doc)
        {
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add("", $"{environment.WebRootPath}\\Resources\\OrderImport.xsd");
            doc.Validate(schema, ValidationEventHandler);
        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlSeverityType type = XmlSeverityType.Warning;
            if (Enum.TryParse<XmlSeverityType>("Error", out type))
            {
                if (type == XmlSeverityType.Error) throw new Exception(e.Message);
            }
        }
    }
}
