using LaunchPad.Contracts.Events;
using System.Security.Cryptography;
using System.Text;

namespace LaunchPad.Ingestion.Domain;

public static class ContentHasher
{
    public static byte[] Compute(LaunchUpserted l)
    {
        var sb = new StringBuilder()
            .Append(l.Name).Append('|')
            .Append(l.Status).Append('|')
            .Append(l.NetUtc.ToUnixTimeSeconds()).Append('|')
            .Append(l.WindowStartUtc?.ToUnixTimeSeconds()).Append('|')
            .Append(l.WindowEndUtc?.ToUnixTimeSeconds()).Append('|')
            .Append(l.Agency.Ll2Id).Append('|')
            .Append(l.Rocket.Ll2Id).Append('|')
            .Append(l.Pad.Ll2Id).Append('|')
            .Append(l.Orbit).Append('|')
            .Append(l.MissionName);

        return SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}