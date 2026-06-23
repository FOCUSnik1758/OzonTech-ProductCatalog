using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Clients;

public interface IStorageClient
{
    Task NotifyProductCreatedAsync(
        Product product,
        CancellationToken cancellationToken);
}