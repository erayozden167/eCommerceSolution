using OrderApi.Application.DTOs;
using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public interface IOrderService
    {
        Task<OrderDetailDTO> GetOrderDetails(int orderId);
    }
}
