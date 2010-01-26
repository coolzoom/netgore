using System.Linq;

namespace NetGore.Features.Guilds
{
    /// <summary>
    /// Handles when a guild member invokes an event.
    /// </summary>
    /// <param name="invoker">The guild member that invoked the event.</param>
    /// <param name="guildEvent">The event.</param>
    public delegate void GuildInvokeEventHandler(IGuildMember invoker, GuildEvents guildEvent);
}