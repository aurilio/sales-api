using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.SaleTest;

/// <summary>
/// Unit tests for the <see cref="Sale"/> entity using Bogus and FluentAssertions.
/// </summary>
public class SaleTests
{
    private readonly Faker _faker = new();

    [Theory(DisplayName = "Given invalid sale number When creating sale Then throws exception")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidSaleNumber_ThrowsException(string saleNumber)
    {
        var act = () => new Sale(saleNumber, DateTime.UtcNow, Guid.NewGuid(), _faker.Name.FullName(), _faker.Company.CompanyName());
        act.Should().Throw<ArgumentException>().WithMessage("*Sale number*");
    }

    [Fact(DisplayName = "Given empty customerId When creating sale Then throws exception")]
    public void Constructor_InvalidCustomerId_ThrowsException()
    {
        var act = () => new Sale(_faker.Random.Hash(), DateTime.UtcNow, Guid.Empty, _faker.Name.FullName(), _faker.Company.CompanyName());
        act.Should().Throw<ArgumentException>().WithMessage("*CustomerId*");
    }

    [Theory(DisplayName = "Given null or empty branch When creating sale Then throws exception")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_InvalidBranch_ThrowsException(string? branch)
    {
        var act = () => new Sale(_faker.Random.Hash(), DateTime.UtcNow, Guid.NewGuid(), _faker.Name.FullName(), branch);
        act.Should().Throw<ArgumentException>().WithMessage("*Branch*");
    }

    [Fact(DisplayName = "Given valid update When updating sale Then values are updated")]
    public void UpdateSale_ValidInput_ShouldUpdateCorrectly()
    {
        // Arrange
        var sale = CreateValidSale();
        var updatedItems = GetValidItems();

        // Act
        var newNumber = _faker.Random.Hash();
        var newCustomer = _faker.Name.FullName();
        var newBranch = _faker.Company.CompanyName();
        var newCustomerId = Guid.NewGuid();
        sale.UpdateSale(newNumber, DateTime.UtcNow.AddDays(1), newCustomerId, newCustomer, newBranch, true);

        // Assert
        sale.SaleNumber.Should().Be(newNumber);
        sale.CustomerName.Should().Be(newCustomer);
        sale.Branch.Should().Be(newBranch);
        sale.CustomerId.Should().Be(newCustomerId);
        sale.IsCancelled.Should().BeTrue();
        sale.UpdatedAt.Should().NotBeNull();
    }

    private Sale CreateValidSale()
    {
        return new Sale(
            _faker.Random.Hash(),
            DateTime.UtcNow,
            Guid.NewGuid(),
            _faker.Name.FullName(),
            _faker.Company.CompanyName());
    }

    private List<SaleItem> GetValidItems()
    {
        return new List<SaleItem>
        {
            new SaleItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10, new ProductDetails(
                _faker.Commerce.ProductName(),
                _faker.Commerce.Categories(1).First(),
                _faker.Random.Decimal(100, 5000),
                _faker.Image.PicsumUrl()))
        };
    }
}