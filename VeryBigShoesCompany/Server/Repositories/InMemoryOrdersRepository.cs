using System;
using System.Collections.Generic;
using System.Linq;
using VeryBigShoesCompany.Shared;

namespace VeryBigShoesCompany.Server.Repositories
{
    public class InMemoryOrdersRepository
    {
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
