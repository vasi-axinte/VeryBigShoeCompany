using System;
using System.Xml.Serialization;

namespace VeryBigShoesCompany.Server.Controllers
{
    [Serializable()]
    public class Order
    {
        [XmlAttribute(nameof(CustomerName))]
        public string CustomerName { get; set; }
        
        [XmlAttribute(nameof(CustomerEmail))]
        public string CustomerEmail { get; set; }
        
        [XmlAttribute(nameof(Quantity))]
        public string Quantity { get; set; }
        
        [XmlAttribute(nameof(Notes))]
        public string Notes { get; set; }
        
        [XmlAttribute(nameof(Size))]
        public string Size { get; set; }
        
        [XmlAttribute(nameof(DateRequired))]
        public string DateRequired { get; set; }
    }

    [Serializable]
    [XmlRoot("BigShoeDataImport")]
    public class BigShoeDataImport
    {
        [XmlElement("Order")]
        public Order[] Orders { get; set; }
    }
}