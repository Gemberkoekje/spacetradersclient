using Qowaiv.Validation.Abstractions;
using SpaceTraders.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Helpers;

/// <summary>
/// Helper methods for pagination.
/// </summary>
public static class ListHelper
{
    /// <summary>
    /// Fetches all pages of data from a paginated API endpoint.
    /// </summary>
    /// <typeparam name="TItem">The type of items to fetch.</typeparam>
    /// <typeparam name="TPage">The type of the page response.</typeparam>
    /// <param name="service">The SpaceTraders service.</param>
    /// <param name="fetchPage">Function to fetch a single page.</param>
    /// <param name="getData">Function to extract items from a page.</param>
    /// <param name="startPage">The starting page number.</param>
    /// <param name="limit">The number of items per page.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A result containing all items.</returns>
    public static async Task<Result<List<TItem>>> GetAllPagesAsync<TItem, TPage>(
        this SpaceTradersService service,
        Func<SpaceTradersClient, int /*page*/, int /*limit*/, CancellationToken, Task<TPage>> fetchPage,
        Func<TPage, ICollection<TItem>> getData,
        int startPage = 1,
        int limit = 20,
        CancellationToken ct = default)
    {
        var all = new List<TItem>();
        var page = startPage;

        while (true)
        {
            var result = await service.EnqueueAsync(
                (client, token) => fetchPage(client, page, limit, token),
                priority: false,
                cancellationToken: ct);

            if (result is null)
            {
                return Result.WithMessages<List<TItem>>(ValidationMessage.Error("Result is null"));
            }

            if (!result.IsValid)
            {
                return Result.WithMessages<List<TItem>>(result.Messages);
            }

            var data = getData(result.Value);
            if (data is null || data.Count == 0)
            {
                break;
            }

            all.AddRange(data);
            page++;
        }

        return all;
    }
}
