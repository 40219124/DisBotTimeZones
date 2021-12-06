using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using System.Collections.Generic;

namespace TimeZoneBot
{
    class QualityOfLifeBits
    {
        private Dictionary<ulong, bool> GuildVoiceRoleActive = new Dictionary<ulong, bool>();
        private Dictionary<ulong, ulong> GuildVoiceRoleID = new Dictionary<ulong, ulong>();

        public async Task VoiceStatusUpdate(SocketUser socketUser,
            SocketVoiceState oldState, SocketVoiceState newState)
        {
            if (socketUser is IGuildUser user &&
                GuildVoiceRoleActive.ContainsKey(user.GuildId) &&
                GuildVoiceRoleActive[user.GuildId])
            {
                if (oldState.VoiceChannel == null)
                {
                    await ManageUserRole(user, true);
                }
                if (newState.VoiceChannel == null)
                {
                    await ManageUserRole(user, false);
                }
            }
        }

        private async Task ManageUserRole(IGuildUser user, bool addRole)
        {
            if (GuildVoiceRoleID.ContainsKey(user.GuildId) &&
                GuildVoiceRoleID[user.GuildId] != 0 &&
                user.Guild.Roles.FirstOrDefault(x => x.Id == GuildVoiceRoleID[user.GuildId]) != null)
            {
                if (addRole)
                {
                    await user.AddRoleAsync(GuildVoiceRoleID[user.GuildId]);
                }
                else
                {
                    await user.RemoveRoleAsync(GuildVoiceRoleID[user.GuildId]);
                }
            }
        }

        private readonly List<string> Affirmatives = new List<string>() { "yes", "active", "true", "on" };
        private readonly List<string> Negatives = new List<string>() { "no", "inactive", "false", "off" };

        public async Task MessageReceivedAsync(SocketMessage message)
        {
            if (!(message.Channel is IGuildChannel channel && channel.Guild != null))
            {
                return;
            }
            ulong guildID = channel.GuildId;
            // Add guild to data
            if (!GuildVoiceRoleActive.ContainsKey(guildID))
            {
                GuildVoiceRoleActive.Add(guildID, false);
                GuildVoiceRoleID.Add(guildID, 0);
            }
            string content = message.Content.ToLower();
            // Toggle state
            if (content == "~voicerole")
            {
                await ActivateVoiceRole(channel.Guild, !GuildVoiceRoleActive[guildID]);
            }
            else if (content.StartsWith("~voicerole "))
            {
                string remainder = content.Substring("~voicerole ".Length);
                // Set on
                if (Affirmatives.Contains(remainder))
                {
                    await ActivateVoiceRole(channel.Guild, true);
                }
                // Set off
                else if (Negatives.Contains(remainder))
                {
                    await ActivateVoiceRole(channel.Guild, false);
                }
                // Set voice role for server
                else if (remainder.StartsWith("<@&"))
                {
                    bool priorState = GuildVoiceRoleActive[guildID];
                    await ActivateVoiceRole(channel.Guild, false);
                    string roleID = remainder.Substring("<@&".Length).Split('>')[0];
                    GuildVoiceRoleID[guildID] = ulong.Parse(roleID);
                    await message.Channel.SendMessageAsync(roleID);
                    await ActivateVoiceRole(channel.Guild, priorState);
                }
            }
        }

        private async Task ActivateVoiceRole(IGuild guild, bool activate)
        {
            if (GuildVoiceRoleActive[guild.Id] == activate)
            {
                return;
            }
            IReadOnlyCollection<IVoiceChannel> voiceChannels = await guild.GetVoiceChannelsAsync();
            foreach (IVoiceChannel vc in voiceChannels)
            {
                IEnumerable<IGuildUser> guildUsers = await Discord.AsyncEnumerableExtensions.FlattenAsync(vc.GetUsersAsync());
                foreach (IGuildUser gu in guildUsers)
                {
                    await ManageUserRole(gu, activate);
                }
            }
            GuildVoiceRoleActive[guild.Id] = activate;
        }
    }
}
