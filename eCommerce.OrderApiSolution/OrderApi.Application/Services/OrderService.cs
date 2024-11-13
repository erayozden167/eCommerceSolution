using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface,HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        public async Task<ProductDTO> GetProduct(int productId)
        {
            //call productApi
            //pass in gateway
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;

            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }
        public async Task<AppUserDTO> GetUser(int userId)
        {
            var getUser = await httpClient.GetAsync($"/api/users/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;

            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }
        public async Task<OrderDetailDTO> GetOrderDetails(int orderId)
        {
            var order = await orderInterface.GetAsync(orderId);
            if(order is null || order!.Id <= 0)
                return null!;

            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            // Product prepare
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            //User prepare
            var userDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            return new OrderDetailDTO(order.Id, productDTO.Id, userDTO.Id,userDTO.Name, userDTO.Email,userDTO.Address, userDTO.TelephoneNumber, productDTO.Name, order.PurchaseQuantity, productDTO.Price, (productDTO.Price * order.PurchaseQuantity), order.OrderDate);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            var orders = await orderInterface.GetOrdersAsync(x => x.ClientId == clientId);
            if(!orders.Any()) return null!;

            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }
    }
}
