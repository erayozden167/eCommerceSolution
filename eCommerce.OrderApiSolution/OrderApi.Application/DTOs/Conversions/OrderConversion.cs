using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO dto) => new Order()
        {
            Id = dto.Id,
            ClientId = dto.ClientId,
            ProductId = dto.ProductId,
            OrderDate = dto.OrderDate,
            PurchaseQuantity = dto.Quantity
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? Orders)
        {
            if (order is not null || Orders is null)
            {
                var singleOrder = new OrderDTO(order!.Id, order.ProductId, order.ClientId, order.PurchaseQuantity, order.OrderDate);
                return (singleOrder, null);
            }
            if (Orders is not null || order is null)
            {
                var _orders = Orders!.Select(o => new OrderDTO(o.Id, o.ProductId, o.ClientId, o.PurchaseQuantity, o.OrderDate));
                return (null, _orders);
            }
        }
    }
}
