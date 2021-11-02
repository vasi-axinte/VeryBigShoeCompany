using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using VeryBigShoesCompany.Server.Repositories;
using VeryBigShoesCompany.Shared;

namespace VeryBigShoesCompany.Server.Services
{
    public class OrdersService
    {
        public OrdersService(InMemoryOrdersRepository repository)
        {
            Repository = repository;
        }
        private InMemoryOrdersRepository Repository { get; }

        public void AddOrders(List<Order> orders)
        {
            var validationResult = ValidateOrders(orders);
            if (!string.IsNullOrEmpty(validationResult))
            {
                throw new Exception(validationResult);
            }
            Repository.Orders.AddRange(orders);
        }
        
        public List<Order> GetOrders()
        {
            return Repository.Orders.ToList();
        }

        private string ValidateOrders(List<Order> orders)
        {
            var errors = new StringBuilder();

            foreach (var order in orders)
            {
                var orderErrors = new StringBuilder();
                string error;
                
                if (!IsEmailValid(order, out error))
                {
                    orderErrors.AppendLine(error);
                };
                if (!IsDateValid(order, out error))
                {
                    orderErrors.AppendLine(error);
                }
                if (!IsSizeValid(order, out error))
                {
                    orderErrors.AppendLine(error);
                }
                if (!IsQuantityValid(order, out error))
                {
                    orderErrors.AppendLine(error);
                }

                var errorsString = orderErrors.ToString();
                if (!string.IsNullOrEmpty(errorsString))
                {
                    errors.AppendLine(errorsString);
                }
            }

            return errors.ToString();
        }
        
        private bool IsEmailValid(Order order, out string error)
        {
            if (MailAddress.TryCreate(order.CustomerEmail, out _))
            {
                error = null;
                return true;
            }
            error = $"Email address: {order.CustomerEmail} is invalid";
            return false;
        }

        private bool IsDateValid(Order order, out string error)
        {
            if ((order.DateRequired - DateTime.Now).Days >= 11)
            {
                error = null;
                return true;
            }
            error = $"Date {order.DateRequired.ToShortDateString()} is invalid. It must be at least 10 days later than today";
            return false;
        }

        private bool IsSizeValid(Order order, out string error)
        {
            if (order.Size < 11.5)
            {
                error = $"Size {order.Size} < 11.5";
                return false;
            }

            if (order.Size > 15)
            {
                error = $"Size {order.Size} > 15";
                return false;
            }

            if (Math.Abs(Math.Ceiling(order.Size) - order.Size) > 0.5)
            {
                error = $"Size {order.Size} is not well formatted. Must be between 11.5 and 15, and only increments of .5 are accepted";
                return false;
            }

            error = null;
            return true;
        }

        private bool IsQuantityValid(Order order, out string error)
        {
            if (order.Quantity % 1000 == 0)
            {
                error = null;
                return true;
            }
            error = $"Quantity {order.Quantity} invalid. Must be a multiple of 1000";
            return false;
        }
    }
}
