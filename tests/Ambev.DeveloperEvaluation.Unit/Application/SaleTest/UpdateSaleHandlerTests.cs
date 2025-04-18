using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Messaging.Events;
using Ambev.DeveloperEvaluation.Messaging.Interfaces;
using Ambev.DeveloperEvaluation.Unit.Application.SaleTest.TestData;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.SaleTest;

/// <summary>
/// Unit tests for the <see cref="UpdateSaleHandler"/> class.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IDomainEventPublisher _eventPublisher = Substitute.For<IDomainEventPublisher>();
    private readonly ILogger<UpdateSaleHandler> _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
    private readonly UpdateSaleHandler _handler;

    private readonly Faker _faker = new();

    public UpdateSaleHandlerTests()
    {
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _logger, _eventPublisher);
    }

    [Fact(DisplayName = "Given valid update command When handling Then sale is updated successfully")]
    public async Task Handle_ValidCommand_ReturnsUpdatedResult()
    {
        var command = UpdateSaleTestData.GenerateValidCommand();
        var sale = UpdateSaleTestData.GenerateValidSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleItem>(Arg.Any<UpdateSaleItemCommand>())
            .Returns(call => new SaleItem(Guid.NewGuid(), 2, new ProductDetails("Test", "Cat", 100m, "img")));
        _mapper.Map<UpdateSaleResult>(sale).Returns(new UpdateSaleResult { Id = sale.Id });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<ModifiedEvent>());
    }

    [Fact(DisplayName = "Given non-existent sale When handling Then throws NotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsNotFoundException()
    {
        var command = UpdateSaleTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Sale with identifier {command.Id} was not found.");
    }

    [Fact(DisplayName = "Given cancellation request When handling Then emits CancelledEvent")]
    public async Task Handle_CancelledCommand_EmitsCancelledEvent()
    {
        var command = UpdateSaleTestData.GenerateValidCommand();
        command.IsCancelled = true;

        var sale = UpdateSaleTestData.GenerateValidSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);

        _mapper.Map<SaleItem>(Arg.Any<UpdateSaleItemCommand>())
            .Returns(new SaleItem(Guid.NewGuid(), 2, new ProductDetails("Test", "Cat", 100m, "img")));
        _mapper.Map<UpdateSaleResult>(sale).Returns(new UpdateSaleResult { Id = sale.Id });

        await _handler.Handle(command, CancellationToken.None);

        await _eventPublisher.Received(1).PublishAsync(Arg.Any<CancelledEvent>());
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<ModifiedEvent>());
    }

    [Fact(DisplayName = "Given update fails When saving Then propagates exception")]
    public async Task Handle_UpdateFails_ThrowsException()
    {
        // Arrange
        var command = UpdateSaleTestData.GenerateValidCommand();
        var existingSale = UpdateSaleTestData.GenerateValidSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.UpdateAsync(existingSale, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Failed to update sale"));

        _mapper.Map<SaleItem>(Arg.Any<UpdateSaleItemCommand>())
            .Returns(new SaleItem(Guid.NewGuid(), 2, new ProductDetails("Test", "Cat", 100m, "img")));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to update sale");
    }

    [Fact(DisplayName = "Given valid update When event publisher fails Then throws exception")]
    public async Task Handle_EventPublishFails_ThrowsException()
    {
        // Arrange
        var command = UpdateSaleTestData.GenerateValidCommand();
        var sale = UpdateSaleTestData.GenerateValidSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);

        _mapper.Map<SaleItem>(Arg.Any<UpdateSaleItemCommand>())
            .Returns(new SaleItem(Guid.NewGuid(), 2, new ProductDetails("Test", "Cat", 100m, "img")));

        _mapper.Map<UpdateSaleResult>(sale).Returns(new UpdateSaleResult { Id = sale.Id });

        _eventPublisher.PublishAsync(Arg.Any<ModifiedEvent>())
            .Throws(new ApplicationException("Event publish failure"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Event publish failure");
    }
}