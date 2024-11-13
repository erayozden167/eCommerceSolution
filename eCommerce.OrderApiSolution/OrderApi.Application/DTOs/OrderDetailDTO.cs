﻿using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
    public record OrderDetailDTO
        (
        [Required] int OrderId,
        [Required] int ProductId,
        [Required] int ClientId,
        [Required] string ClientName,
        [Required, EmailAddress] string Email,
        [Required] string Address,
        [Required] string Phone,
        [Required] string ProductName,
        [Required] int PurchaseQuantity,
        [Required, DataType(DataType.Currency)] decimal UnitPrice, 
        [Required, DataType(DataType.Currency)] decimal TotalPrice,
        [Required] DateTime OrderDate
        );
}
