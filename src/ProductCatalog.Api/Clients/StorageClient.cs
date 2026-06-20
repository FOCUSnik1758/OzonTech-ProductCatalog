using System.Net;
using System.Net.Http.Json;
using ProductCatalog.Api.Contracts.Storage;
using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Clients;

public sealed class StorageClient : IStorageClient
{
    private readonly HttpClient _httpClient;

    public StorageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task NotifyProductCreatedAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        var request = new ProductCreatedStorageRequest
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/Storage/product-created",
            request,
            cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var responseBody = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new InvalidOperationException(
                $"Storage Service вернул код {(int)response.StatusCode}. " +
                $"Ответ: {responseBody}");
        }
    }
}