using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Messaging.Events;
using Ambev.DeveloperEvaluation.Messaging.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.SaleTest
{
    public class DeleteSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
        private readonly ILogger<DeleteSaleHandler> _logger = Substitute.For<ILogger<DeleteSaleHandler>>();
        private readonly IDomainEventPublisher _eventPublisher = Substitute.For<IDomainEventPublisher>();
        private readonly DeleteSaleHandler _handler;

        public DeleteSaleHandlerTests()
        {
            _handler = new DeleteSaleHandler(_saleRepository, _logger, _eventPublisher);
        }

        [Fact(DisplayName = "Given existing sale ID When deleting Then returns success response and logs")]
        public async Task Handle_ExistingSale_DeletesSuccessfully()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var command = new DeleteSaleCommand(saleId);

            _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Message.Should().Contain(saleId.ToString());

            await _saleRepository.Received(1).DeleteAsync(saleId, Arg.Any<CancellationToken>());
            await _eventPublisher.Received(1).PublishAsync(Arg.Is<CancelledEvent>(e => e.AggregateId == saleId));
        }

        [Fact(DisplayName = "Given non-existing sale ID When deleting Then throws NotFoundException")]
        public async Task Handle_NonExistingSale_ThrowsNotFound()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var command = new DeleteSaleCommand(saleId);

            _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Sale with identifier {saleId} was not found.");

            await _saleRepository.Received(1).DeleteAsync(saleId, Arg.Any<CancellationToken>());
            await _eventPublisher.DidNotReceive().PublishAsync(Arg.Any<CancelledEvent>());
        }
    }

}
