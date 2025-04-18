using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.SaleTest;

/// <summary>
/// Unit tests for the <see cref="GetSaleHandler"/> class.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ILogger<GetSaleHandler> _logger = Substitute.For<ILogger<GetSaleHandler>>();
    private readonly GetSaleHandler _handler;

    private readonly Faker _faker = new();

    public GetSaleHandlerTests()
    {
        _handler = new GetSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid sale ID When handling Then returns expected result")]
    public async Task Handle_ValidId_ReturnsMappedResult()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var query = new GetSaleQuery(saleId);

        var sale = new Sale(
            _faker.Commerce.Ean8(),
            _faker.Date.Past(),
            Guid.NewGuid(),
            _faker.Name.FullName(),
            _faker.Company.CompanyName(),
            new List<SaleItem>
            {
            new SaleItem(Guid.NewGuid(), 10,
                new ProductDetails(
                    _faker.Commerce.ProductName(),
                    _faker.Commerce.Categories(1)[0],
                    _faker.Random.Decimal(100, 1000),
                    _faker.Image.PicsumUrl()))
            }
        );

        var expectedResult = new GetSaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact(DisplayName = "Given non-existent sale ID When handling Then throws NotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var query = new GetSaleQuery(Guid.NewGuid());
        _saleRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Sale with identifier {query.Id} was not found.");
    }
}