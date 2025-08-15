using E_Commerce.Common.Infrastructure.Messaging;
using E_Commerce.ProductCatalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_Commerce.ProductCatalog.Infrastructure.EventHandlers;

public class ProductInventoryUpdatedEventHandler : INotificationHandler<ProductInventoryUpdatedEvent>
{
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<ProductInventoryUpdatedEventHandler> _logger;

    public ProductInventoryUpdatedEventHandler(IMessageBroker messageBroker, ILogger<ProductInventoryUpdatedEventHandler> logger)
    {
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task Handle(ProductInventoryUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing inventory update event for product {ProductId}", notification.ProductId);

        var integrationEvent = new ProductInventoryUpdatedIntegrationEvent(
            notification.ProductId.Value,
            notification.TenantId.Value,
            notification.OldQuantity,
            notification.NewQuantity);

        await _messageBroker.PublishAsync(integrationEvent, "integration.events", "product.inventory.updated", cancellationToken);
    }
}

public record ProductInventoryUpdatedIntegrationEvent(
    Guid ProductId,
    Guid TenantId,
    int OldQuantity,
    int NewQuantity);
