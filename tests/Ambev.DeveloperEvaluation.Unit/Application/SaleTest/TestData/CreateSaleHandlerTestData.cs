using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.SaleTest.TestData;

public static class CreateSaleHandlerTestData
{
    private static readonly Faker<ProductDetails> productDetailsFaker = new Faker<ProductDetails>()
        .CustomInstantiator(f => new ProductDetails(
            f.Commerce.ProductName(),
            f.Commerce.Categories(1)[0],
            f.Random.Decimal(100, 1000),
            f.Image.PicsumUrl()
        ));

    private static readonly Faker<CreateSaleItemCommand> itemFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.ProductDetails, f => productDetailsFaker.Generate());

    private static readonly Faker<CreateSaleCommand> saleFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.SaleNumber, f => $"SN-{f.Random.Number(1000, 9999)}")
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.Branch, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, f => itemFaker.Generate(f.Random.Int(1, 5)));

    public static CreateSaleCommand GenerateValidCommand() => saleFaker.Generate();
}