using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.SaleTest;

public class SaleItemTests
{
    private readonly Faker faker = new();

    private ProductDetails FakeProductDetails(decimal price = 100) =>
        new ProductDetails(
            title: faker.Commerce.ProductName(),
            category: faker.Commerce.Categories(1)[0],
            price: price,
            image: faker.Image.PicsumUrl()
        );

    [Theory(DisplayName = "Should apply correct discount based on quantity")]
    [InlineData(2, 0.0)]
    [InlineData(4, 0.10)]
    [InlineData(10, 0.20)]
    public void SaleItem_ShouldApplyCorrectDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var price = 100m;
        var product = FakeProductDetails(price);

        // Act
        var item = new SaleItem(Guid.NewGuid(), quantity, product);

        // Assert
        item.Discount.Should().Be(expectedDiscount);
    }

    [Theory(DisplayName = "Should calculate correct unit price with discount")]
    [InlineData(4, 100, 90)] // 10% discount
    [InlineData(10, 100, 80)] // 20% discount
    [InlineData(2, 100, 100)] // no discount
    public void SaleItem_ShouldCalculateCorrectUnitPrice(int quantity, decimal price, decimal expectedUnitPrice)
    {
        var product = FakeProductDetails(price);

        var item = new SaleItem(Guid.NewGuid(), quantity, product);

        item.UnitPrice.Should().Be(expectedUnitPrice);
    }

    [Theory(DisplayName = "Should calculate correct total amount")]
    [InlineData(4, 100, 360)] // 4 * 90
    [InlineData(10, 100, 800)] // 10 * 80
    [InlineData(2, 100, 200)] // 2 * 100
    public void SaleItem_ShouldCalculateCorrectTotalAmount(int quantity, decimal price, decimal expectedTotal)
    {
        var product = FakeProductDetails(price);

        var item = new SaleItem(Guid.NewGuid(), quantity, product);

        item.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact(DisplayName = "Should throw exception if quantity is greater than 20")]
    public void SaleItem_ShouldThrowException_WhenQuantityGreaterThanAllowed()
    {
        var product = FakeProductDetails(100);

        Action act = () => new SaleItem(Guid.NewGuid(), 21, product);

        act.Should().Throw<DomainException>()
            .WithMessage("Cannot sell more than 20 identical items.");
    }

    [Fact(DisplayName = "Should throw exception if quantity is zero or negative")]
    public void SaleItem_ShouldThrowException_WhenQuantityIsInvalid()
    {
        var product = FakeProductDetails(100);

        Action act = () => new SaleItem(Guid.NewGuid(), 0, product);

        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be greater than zero.");
    }

    [Fact(DisplayName = "Should throw exception if ProductDetails is null")]
    public void SaleItem_ShouldThrowException_WhenProductDetailsIsNull()
    {
        var product = FakeProductDetails(100);
        Action act = () => new SaleItem(Guid.NewGuid(), 0, product);

        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be greater than zero.");
    }
}