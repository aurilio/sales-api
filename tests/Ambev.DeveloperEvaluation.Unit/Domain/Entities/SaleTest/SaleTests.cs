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

    [Fact(DisplayName = "Given valid input When creating sale Then initializes correctly")]
    public void Constructor_ValidInput_ShouldCreateSale()
    {
        // Arrange
        var items = GetValidItems();

        // Act
        var sale = new Sale(_faker.Random.Hash(), DateTime.UtcNow, Guid.NewGuid(), _faker.Name.FullName(), _faker.Company.CompanyName(), items);

        // Assert
        sale.Should().NotBeNull();
        sale.TotalAmount.Should().BeGreaterThan(0);
        sale.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        sale.Items.Should().HaveCount(1);
    }

    [Theory(DisplayName = "Given invalid sale number When creating sale Then throws exception")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidSaleNumber_ThrowsException(string saleNumber)
    {
        var act = () => new Sale(saleNumber, DateTime.UtcNow, Guid.NewGuid(), _faker.Name.FullName(), _faker.Company.CompanyName(), GetValidItems());
        act.Should().Throw<ArgumentException>().WithMessage("*Sale number*");
    }

    [Fact(DisplayName = "Given empty customerId When creating sale Then throws exception")]
    public void Constructor_InvalidCustomerId_ThrowsException()
    {
        var act = () => new Sale(_faker.Random.Hash(), DateTime.UtcNow, Guid.Empty, _faker.Name.FullName(), _faker.Company.CompanyName(), GetValidItems());
        act.Should().Throw<ArgumentException>().WithMessage("*CustomerId*");
    }

    [Theory(DisplayName = "Given null or empty branch When creating sale Then throws exception")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_InvalidBranch_ThrowsException(string? branch)
    {
        var act = () => new Sale(_faker.Random.Hash(), DateTime.UtcNow, Guid.NewGuid(), _faker.Name.FullName(), branch, GetValidItems());
        act.Should().Throw<ArgumentException>().WithMessage("*Branch*");
    }

    [Fact(DisplayName = "Given empty item list When creating sale Then throws domain exception")]
    public void Constructor_EmptyItemList_ThrowsDomainException()
    {
        var act = () => new Sale(_faker.Random.Hash(), DateTime.UtcNow, Guid.NewGuid(), _faker.Name.FullName(), _faker.Company.CompanyName(), []);
        act.Should().Throw<Exception>().WithMessage("A sale must have at least one item.");
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
        sale.UpdateSale(newNumber, DateTime.UtcNow.AddDays(1), newCustomerId, newCustomer, newBranch, true, updatedItems);

        // Assert
        sale.SaleNumber.Should().Be(newNumber);
        sale.CustomerName.Should().Be(newCustomer);
        sale.Branch.Should().Be(newBranch);
        sale.CustomerId.Should().Be(newCustomerId);
        sale.Items.Should().HaveCount(1);
        sale.IsCancelled.Should().BeTrue();
        sale.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "Given empty updated items When updating sale Then throws domain exception")]
    public void UpdateSale_EmptyItems_ThrowsDomainException()
    {
        var sale = CreateValidSale();
        var act = () => sale.UpdateSale("SALE002", DateTime.UtcNow, Guid.NewGuid(), _faker.Name.FullName(), _faker.Company.CompanyName(), false, []);
        act.Should().Throw<Exception>().WithMessage("A sale must have at least one item.");
    }

    private Sale CreateValidSale()
    {
        return new Sale(
            _faker.Random.Hash(),
            DateTime.UtcNow,
            Guid.NewGuid(),
            _faker.Name.FullName(),
            _faker.Company.CompanyName(),
            GetValidItems());
    }

    private List<SaleItem> GetValidItems()
    {
        return new List<SaleItem>
        {
            new SaleItem(Guid.NewGuid(), 10, new ProductDetails(
                _faker.Commerce.ProductName(),
                _faker.Commerce.Categories(1).First(),
                _faker.Random.Decimal(100, 5000),
                _faker.Image.PicsumUrl()))
        };
    }
}