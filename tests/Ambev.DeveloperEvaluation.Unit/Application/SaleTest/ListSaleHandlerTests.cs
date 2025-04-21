using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using MockQueryable.NSubstitute;
using Xunit;
using MockQueryable;

namespace Ambev.DeveloperEvaluation.Unit.Application.SaleTest;

/// <summary>
/// Unit tests for the <see cref="ListSaleHandler"/>.
/// </summary>
public class ListSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ILogger<ListSaleHandler> _logger = Substitute.For<ILogger<ListSaleHandler>>();
    private readonly ListSaleHandler _handler;

    private readonly Faker _faker = new();

    public ListSaleHandlerTests()
    {
        _handler = new ListSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid query When handling Then returns paginated list")]
    public async Task Handle_ValidRequest_ReturnsPaginatedList()
    {
        var query = new ListSaleQuery(page: 1, size: 10, orderBy: "SaleDate", filters: new Dictionary<string, string>());

        var fakeSales = Enumerable.Range(1, 10).Select(_ => new Sale(
            _faker.Commerce.Ean8(),
            _faker.Date.Recent(),
            Guid.NewGuid(),
            _faker.Name.FullName(),
            _faker.Company.CompanyName(),
            new List<SaleItem>
            {
            new SaleItem(Guid.NewGuid(), 10,
                new ProductDetails(
                    _faker.Commerce.ProductName(),
                    _faker.Commerce.Categories(1)[0],
                    _faker.Random.Decimal(10, 100),
                    _faker.Image.PicsumUrl()))
            }
        )).ToList();

        var paginated = new PaginatedList<Sale>(
            items: fakeSales,
            count: 100,
            pageNumber: 1,
            pageSize: 10
        );

        _saleRepository
            .ListAsync(1, 10, "SaleDate", Arg.Any<Dictionary<string, string>>(), Arg.Any<CancellationToken>())
            .Returns(paginated);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(100);
        result.Should().HaveCount(10);
        result.CurrentPage.Should().Be(1);
    }

    [Fact(DisplayName = "Given query with filters When handling Then filters are applied")]
    public async Task Handle_WithFilters_AppliesFiltersAndReturnsList()
    {
        // Arrange
        var query = new ListSaleQuery
        (
            page: 1,
            size: 5,
            orderBy: "SaleDate",
            filters: new Dictionary<string, string>
            {
            { "Branch", "*Filial*" }
            }
        );

        var matchingSale = new Sale(
            saleNumber: "A-2025-001",
            saleDate: _faker.Date.Recent(),
            customerId: Guid.NewGuid(),
            customerName: _faker.Name.FullName(),
            branch: "Filial São Paulo",
            items: new List<SaleItem>
            {
            new SaleItem(Guid.NewGuid(), 4,
                new ProductDetails("Produto", "Categoria", 100m, "img.png"))
            });

        var paginated = new PaginatedList<Sale>(
            items: new List<Sale> { matchingSale },
            count: 1,
            pageNumber: 1,
            pageSize: 5
        );

        _saleRepository
            .ListAsync(1, 5, "SaleDate", query.Filters, Arg.Any<CancellationToken>())
            .Returns(paginated);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result.First().Branch.Should().Contain("Filial");
    }


    [Fact(DisplayName = "Given no order by When handling Then defaults to SaleDate descending")]
    public async Task Handle_NoOrderBy_UsesDefaultOrder()
    {
        // Arrange
        var query = new ListSaleQuery(
            page: 1,
            size: 5,
            orderBy: null,
            filters: null
        );

        var sales = Enumerable.Range(0, 3).Select(_ => new Sale(
            _faker.Commerce.Ean8(),
            _faker.Date.Past(),
            Guid.NewGuid(),
            _faker.Name.FullName(),
            _faker.Company.CompanyName(),
            new List<SaleItem>
            {
            new SaleItem(Guid.NewGuid(), 3,
                new ProductDetails("Test", "Cat", 50m, "img.png"))
            }
        )).ToList();

        var paginated = new PaginatedList<Sale>(
            items: sales,
            count: sales.Count,
            pageNumber: 1,
            pageSize: 5
        );

        _saleRepository
            .ListAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string?>(), Arg.Any<Dictionary<string, string>?>(), Arg.Any<CancellationToken>())
            .Returns(paginated);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(sales.Count);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(5);
    }

}