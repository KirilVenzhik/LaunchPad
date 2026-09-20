using LaunchPad.Catalog.Domain.Entities;
using LaunchPad.Catalog.Infrastructure;
using LaunchPad.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LaunchPad.Catalog.Application.Consumers;

public class LaunchUpsertedConsumer(CatalogDbContext db) : IConsumer<LaunchUpserted>
{
    public async Task Consume(ConsumeContext<LaunchUpserted> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;
        var now = DateTimeOffset.UtcNow;

        var agency = await GetOrCreateAgency(message.Agency, cancellationToken);
        var rocket = await GetOrCreateRocket(message.Rocket, cancellationToken);
        var pad = await GetOrCreatePad(message.Pad, cancellationToken);

        var status = EventMapping.ToStatus(message.Status);
        var orbit = EventMapping.ToOrbit(message.Orbit);

        var launch = await db.Launches.FirstOrDefaultAsync(x => x.Ll2Id == message.LaunchLl2Id, cancellationToken);

        if (launch is null)
        {
            launch = Launch.Create(
                message.LaunchLl2Id, 
                message.Name, status, 
                message.NetUtc,
                message.WindowStartUtc, 
                message.WindowEndUtc, 
                agency, 
                rocket, 
                pad,
                message.MissionName, 
                orbit, 
                now);
            db.Launches.Add(launch);
        }
        else
        {
            launch.UpdateDetails(
                message.Name, 
                message.WindowStartUtc, 
                message.WindowEndUtc, 
                message.MissionName, 
                orbit, 
                now);
            launch.Reschedule(message.NetUtc, now);
            launch.ChangeStatus(status, now);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Agency> GetOrCreateAgency(AgencyPayload agencyPayload, CancellationToken cancellationToken)
    {
        var agency = await db.Agencies.FirstOrDefaultAsync(x => x.Ll2Id == agencyPayload.Ll2Id, cancellationToken);
        if (agency is null)
        {
            agency = Agency.Create(
                agencyPayload.Ll2Id,
                agencyPayload.Name,
                agencyPayload.Abbrev,
                EventMapping.ToAgencyType(agencyPayload.Type),
                agencyPayload.CountryCode);
            db.Agencies.Add(agency);
        }
        return agency;
    }

    private async Task<RocketConfig> GetOrCreateRocket(RocketConfigPayload rocketConfigPayload, CancellationToken cancellationToken)
    {
        var rocket = await db.RocketConfigs.FirstOrDefaultAsync(x => x.Ll2Id == rocketConfigPayload.Ll2Id, cancellationToken);
        if (rocket is null) 
        { 
            rocket = RocketConfig.Create(
                rocketConfigPayload.Ll2Id, 
                rocketConfigPayload.FullName, 
                rocketConfigPayload.Family, 
                rocketConfigPayload.ReusableStages); 
            db.RocketConfigs.Add(rocket); 
        }
        return rocket;
    }

    private async Task<Pad> GetOrCreatePad(PadPayload padPayload, CancellationToken cancellationToken)
    {
        var pad = await db.Pads.FirstOrDefaultAsync(x => x.Ll2Id == padPayload.Ll2Id, cancellationToken);
        if (pad is null) 
        { 
            pad = Pad.Create(padPayload.Ll2Id, 
                padPayload.Name, 
                padPayload.CountryCode, 
                padPayload.Latitude, 
                padPayload.Longitude); 
            db.Pads.Add(pad); 
        }
        return pad;
    }
}
