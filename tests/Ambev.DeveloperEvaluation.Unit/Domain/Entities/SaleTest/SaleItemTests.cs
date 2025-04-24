using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.SaleTests;

public class SaleItemUpdateTests
{
    private readonly Faker faker = new();

    private ProductDetails FakeProductDetails(decimal price = 100) =>
        new ProductDetails(
            title: faker.Commerce.ProductName(),
            category: faker.Commerce.Categories(1)[0],
            price: price,
            image: faker.Image.PicsumUrl()
        );

    [Fact(DisplayName = "Should update item with new quantity and product details")]
    public void Update_ShouldUpdateFieldsCorrectly()
    {
        // Arrange
        var initialProduct = FakeProductDetails(100);
        var updatedProduct = FakeProductDetails(150); // Novo preço
        var item = new SaleItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, initialProduct);
        var newQuantity = 10;

        // Act
        item.Update(item.ProductId, newQuantity, updatedProduct);

        // Assert
        var expectedDiscount = 0.20m; // Com 10 unidades, o desconto deve ser 20%
        var expectedUnitPrice = updatedProduct.Price * (1 - expectedDiscount); // 150 * 0.8 = 120
        var expectedTotal = newQuantity * expectedUnitPrice; // 10 * 120 = 1200

        item.Quantity.Should().Be(newQuantity);
        item.Discount.Should().Be(expectedDiscount);
        item.UnitPrice.Should().Be(expectedUnitPrice);
        item.TotalAmount.Should().Be(expectedTotal);
        item.ProductDetails.Should().BeEquivalentTo(updatedProduct);
        item.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "Should throw exception if updated quantity exceeds limit")]
    public void Update_ShouldThrow_WhenQuantityGreaterThan20()
    {
        // Arrange
        var product = FakeProductDetails(100);
        var item = new SaleItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10, product);

        // Act
        Action act = () => item.Update(item.ProductId, 21, product);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot sell more than 20 identical items.");
    }

    [Fact(DisplayName = "Should throw exception if updated quantity is zero")]
    public void Update_ShouldThrow_WhenQuantityIsZero()
    {
        // Arrange
        var product = FakeProductDetails(100);
        var item = new SaleItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10, product);

        // Act
        Action act = () => item.Update(item.ProductId, 0, product);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be greater than zero.");
    }

    [Fact(DisplayName = "Should throw exception if updated product is null")]
    public void Update_ShouldThrow_WhenProductIsNull()
    {
        // Arrange
        var product = FakeProductDetails(100);
        var item = new SaleItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, product);

        // Act
        Action act = () => item.Update(item.ProductId, 5, null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Value cannot be null. (Parameter 'productDetails')");
    }

    [Fact(DisplayName = "Should update discount when quantity changes tiers")]
    public void Update_ShouldRecalculateDiscount_WhenQuantityChanges()
    {
        // Arrange
        var product = FakeProductDetails(200);
        var item = new SaleItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 3, product); // Sem desconto

        // Act
        item.Update(item.ProductId, 10, product); // Desconto de 20%

        // Assert
        item.Discount.Should().Be(0.20m);
        item.UnitPrice.Should().Be(160); // 200 - 20%
        item.TotalAmount.Should().Be(1600);
    }
}