using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Collections.Generic;

namespace TimeZoneBot
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        [Command("Set")]
        [Summary("Set someone's role")]
        public async Task SetRole(string role, IUser user = null)
        {
            user = user ?? Context.User;
            await ReplyAsync("Command: greeting");

            IRole roleI = Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals(role));
            if (Context.Guild.CurrentUser.GuildPermissions.ManageRoles)
            {
                await ReplyAsync("Permission for roles");
            }
            await (user as IGuildUser).AddRoleAsync(roleI);
            await ReplyAsync("Role set");
        }


        [Command("Help")]
        public async Task Help()
        {
            List<CommandInfo> commands = CommandHandlingService.Instance.CommandsInstance.Commands.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (CommandInfo command in commands)
            {
                string embedFieldText = (command.Summary ?? "No description") + "\n";
                //embedFieldText += command.Parameters.ToString() ?? "No parameters\n";
                embedBuilder.AddField(command.Name, embedFieldText);
            }

            await ReplyAsync("The instructions I listen to: ", false, embedBuilder.Build());
        }
    }
}
