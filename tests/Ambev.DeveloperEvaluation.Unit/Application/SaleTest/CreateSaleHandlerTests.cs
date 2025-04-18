using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Messaging.Common;
using Ambev.DeveloperEvaluation.Messaging.Events;
using Ambev.DeveloperEvaluation.Messaging.Interfaces;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.SaleTest.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Serilog.Core;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.SaleTest;

/// <summary>
/// Unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IDomainEventPublisher _eventPublisher = Substitute.For<IDomainEventPublisher>();
    private readonly ILogger<CreateSaleHandler> _logger = Substitute.For<ILogger<CreateSaleHandler>>();
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _logger, _eventPublisher);
    }

    [Fact(DisplayName = "Given valid command When handling Then sale is created successfully")]
    public async Task Handle_ValidRequest_ReturnsResult()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale(command.SaleNumber, command.SaleDate, command.CustomerId, command.CustomerName, command.Branch,
            command.Items.Select(i => new SaleItem(i.ProductId, i.Quantity, i.ProductDetails)));

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        var resultMock = new CreateSaleResult { Id = sale.Id };
        _mapper.Map<CreateSaleResult>(sale).Returns(resultMock);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);

        await _saleRepository.Received(1).CreateAsync(
        Arg.Is<Sale>(s =>
            s.SaleNumber == command.SaleNumber &&
            s.CustomerId == command.CustomerId &&
            s.Branch == command.Branch &&
            s.Items.Count == command.Items.Count
        ),
        Arg.Any<CancellationToken>()
    );

        await _eventPublisher.Received(1).PublishAsync(
            Arg.Is<CreatedEvent>(e => e.AggregateId == sale.Id)
        );
    }

    [Fact(DisplayName = "Given valid command When handled Then logs creation info")]
    public async Task Handle_ValidRequest_LogsCreationInfo()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        //var handler = CreateHandler(out var repository, out var mapper, out var logger, out var publisher);

        var mappedResult = new CreateSaleResult { Id = Guid.NewGuid() };
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(mappedResult);

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.Arg<Sale>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Handling CreateSaleCommand")),
            null,
            Arg.Any<Func<object, Exception?, string>>()
        );
    }

    [Fact(DisplayName = "Given valid sale When handled Then returns expected result")]
    public async Task Handle_ValidSale_ReturnsExpectedResult()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale(command.SaleNumber, command.SaleDate, command.CustomerId, command.CustomerName, command.Branch,
            command.Items.Select(i => new SaleItem(i.ProductId, i.Quantity, i.ProductDetails)).ToList());

        var result = new CreateSaleResult { Id = sale.Id };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);
        _eventPublisher.PublishAsync(Arg.Any<CreatedEvent>()).Returns(Task.CompletedTask);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(sale.Id);
    }

    [Fact(DisplayName = "Given valid sale When handled Then publishes SaleCreatedEvent")]
    public async Task Handle_ValidSale_PublishesDomainEvent()
    {

        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale(command.SaleNumber, command.SaleDate, command.CustomerId, command.CustomerName, command.Branch,
            command.Items.Select(i => new SaleItem(i.ProductId, i.Quantity, i.ProductDetails)).ToList());

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(new CreateSaleResult { Id = sale.Id });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _eventPublisher.Received(1).PublishAsync(
            Arg.Is<CreatedEvent>(e => e.AggregateId == sale.Id)
        );
    }

    [Fact(DisplayName = "Given invalid CreateSaleCommand When handled Then throws argument exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var invalidCommand = new CreateSaleCommand(); // Faltam campos obrigatórios

        // Act
        var act = () => _handler.Handle(invalidCommand, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact(DisplayName = "Given repository throws exception When handling Then propagates exception")]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.CreateAsync(
            Arg.Is<Sale>(s => s.SaleNumber == command.SaleNumber),
            Arg.Any<CancellationToken>())
            .Throws(new Exception("Database error"));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Database error");
    }

    [Fact(DisplayName = "Given event publisher throws exception When handling Then propagates exception")]
    public async Task Handle_EventPublisherThrows_PropagatesException()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();

        var savedSale = new Sale(
            command.SaleNumber,
            command.SaleDate,
            command.CustomerId,
            command.CustomerName,
            command.Branch,
            command.Items.Select(i => new SaleItem(i.ProductId, i.Quantity, i.ProductDetails)).ToList()
        );

        _saleRepository.CreateAsync(
            Arg.Is<Sale>(s => s.SaleNumber == command.SaleNumber),
            Arg.Any<CancellationToken>())
            .Returns(savedSale);

        _eventPublisher.PublishAsync(
            Arg.Is<CreatedEvent>(e => e.AggregateId == savedSale.Id))
            .Throws(new InvalidOperationException("Event publish failed"));

        var resultMock = new CreateSaleResult { Id = savedSale.Id };
        _mapper.Map<CreateSaleResult>(savedSale).Returns(resultMock);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Event publish failed");
    }
}