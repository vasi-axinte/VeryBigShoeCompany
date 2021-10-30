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
        public short Quantity { get; set; }
        
        [XmlAttribute(nameof(Notes))]
        public string Notes { get; set; }
        
        [XmlAttribute(nameof(Size))]
        public float Size { get; set; }
        
        [XmlAttribute(nameof(DateRequired))]
        public DateTime DateRequired { get; set; }
    }

    [Serializable]
    [XmlRoot("BigShoeDataImport")]
    public class BigShoeDataImport
    {
        [XmlElement("Order")]
        public Order[] Orders { get; set; }
    }
}