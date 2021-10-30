using System;

namespace VeryBigShoesCompany.Shared
{
    public class Order
    {
        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public short Quantity { get; set; }

        public string Notes { get; set; }

        public float Size { get; set; }

        public DateTime DateRequired { get; set; }
    }
}
