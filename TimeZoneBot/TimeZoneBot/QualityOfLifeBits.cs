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
        private static readonly ulong VoiceRole = 917238745579806740U;

        public async Task VoiceStatusUpdate(SocketUser socketUser, SocketVoiceState oldState,
            SocketVoiceState newState)
        {

            if (socketUser is IGuildUser user &&
                user.Guild.Roles.FirstOrDefault(x => x.Id == VoiceRole) != null)
            {
                if (oldState.VoiceChannel == null)
                {
                    await user.AddRoleAsync(VoiceRole);
                }
                if (newState.VoiceChannel == null)
                {
                    await user.RemoveRoleAsync(VoiceRole);
                }
            }
        }
    }
}
