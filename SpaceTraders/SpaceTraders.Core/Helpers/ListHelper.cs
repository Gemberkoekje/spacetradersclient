using Qowaiv.Validation.Abstractions;
using SpaceTraders.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceTraders.Core.Helpers;

public static class ListHelper
{
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
