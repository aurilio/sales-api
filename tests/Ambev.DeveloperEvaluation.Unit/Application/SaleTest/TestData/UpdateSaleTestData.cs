using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.SaleTest.TestData;

public static class UpdateSaleTestData
{
    private static readonly Faker _faker = new();

    public static UpdateSaleCommand GenerateValidCommand()
    {
        return new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = _faker.Commerce.Ean8(),
            SaleDate = _faker.Date.Recent(),
            CustomerId = Guid.NewGuid(),
            CustomerName = _faker.Name.FullName(),
            Branch = _faker.Company.CompanyName(),
            IsCancelled = false,
            Items = new List<UpdateSaleItemCommand>
        {
            new UpdateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                Quantity = 2,
                ProductDetails = new ProductDetails(
                    _faker.Commerce.ProductName(),
                    _faker.Commerce.Categories(1).First(),
                    _faker.Random.Decimal(10, 100),
                    _faker.Image.PicsumUrl()
                )
            }
        }
        };
    }

    public static Sale GenerateValidSale()
    {
        return new Sale(
            _faker.Commerce.Ean8(),
            _faker.Date.Recent(),
            Guid.NewGuid(),
            _faker.Name.FullName(),
            _faker.Company.CompanyName()
        );
    }
}