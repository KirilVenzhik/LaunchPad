using LaunchPad.Ingestion.Acl.DTOs;

namespace LaunchPad.Ingestion.Acl;

public interface ILaunchLibraryClient
{
    IAsyncEnumerable<Ll2Launch> GetUpcomingAsync(CancellationToken ct);
}
