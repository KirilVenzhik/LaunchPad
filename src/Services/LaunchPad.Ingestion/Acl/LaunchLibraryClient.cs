using LaunchPad.Ingestion.Acl.DTOs;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace LaunchPad.Ingestion.Acl;

public sealed class LaunchLibraryClient(HttpClient http) : ILaunchLibraryClient
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public async IAsyncEnumerable<Ll2Launch> GetUpcomingAsync(
        [EnumeratorCancellation] CancellationToken ct)
    {
        string? url = "launch/upcoming/?mode=normal&limit=50";
        while (url is not null)
        {
            using var resp = await http.GetAsync(url, ct);
            resp.EnsureSuccessStatusCode();
            var page = await resp.Content.ReadFromJsonAsync<Ll2LaunchPage>(Json, ct)
                       ?? throw new InvalidOperationException("LL2 returned empty page");

            foreach (var launch in page.Results)
                yield return launch;

            url = page.Next;
        }
    }
}
