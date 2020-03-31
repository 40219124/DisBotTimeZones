using System;
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
        [Command("Dictionary")]
        public async Task MakeDictionary()
        {
            using (StreamWriter file = new StreamWriter(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%/Desktop/DictionaryContent.txt")))
            {
                foreach (TimeZoneInfo tz in TimeZoneInfo.GetSystemTimeZones())
                {
                    string name = tz.StandardName;
                    string initials = "";
                    foreach (char c in name.ToCharArray())
                    {
                        if (c >= 'A' && c <= 'Z')
                        {
                            initials += c;
                        }
                    }
                    string minutes = (tz.BaseUtcOffset.Minutes / 60.0f).ToString();
                    if (minutes.Equals("0"))
                    {
                        minutes = "";
                    }
                    else if (minutes.Contains("-"))
                    {
                        minutes = minutes.Substring(2);
                    }
                    else
                    {
                        minutes = minutes.Substring(1);
                    }
                    string tag = initials + (tz.BaseUtcOffset.Hours >= 0 ? "+" : "") + tz.BaseUtcOffset.Hours.ToString() + minutes;
                    await file.WriteAsync($"{{\"{tag}\", \"{name}\"}},\n");
                }
            }
            await ReplyAsync("Dictionary finished.");
        }

        [Command("Offsets")]
        [Summary("Get selection of time zone names with given offset from UTC.")]
        public async Task GetOffsets(float offset)
        {
            string zones = "";
            foreach (TimeZoneInfo tz in TimeZoneInfo.GetSystemTimeZones())
            {
                if (tz.BaseUtcOffset.Hours + (tz.BaseUtcOffset.Minutes / 60.0f) == offset)
                {
                    zones += tz.StandardName + "\n";
                }
            }
            await ReplyAsync(zones);
        }

        [Command("Set")]
        [Summary("Set someone's role")]
        public async Task SetRole(string timeZone, IUser user = null)
        {
            user = user ?? Context.User;
            string role = "";
            string zone = TimeZoneConversion.shortToName["CUT+0"];
            foreach (KeyValuePair<string, string> pair in TimeZoneConversion.shortToName)
            {
                if (pair.Value.Equals(timeZone))
                {
                    role = pair.Key;
                    break;
                }
            }
            if (role.Equals(""))
            {
                await ReplyAsync("Not a valid time zone.  Try using \"~Offset [hours]\" to find your time zone name.");
                return;
            }

            IRole roleI = Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals(role));

            if (roleI == null)
            {
                roleI = await Context.Guild.CreateRoleAsync(role, null, Color.DarkBlue, false, null);
            }
            await (user as IGuildUser).AddRoleAsync(roleI);
            await ReplyAsync("Role set");
        }

        [Command("Echo")]
        public async Task EchoText([Remainder]string rem)
        {
            await ReplyAsync(rem);
        }

        [Command("Swap")]
        [Summary("Convert from one timezone to another.")]
        public async Task ConvertTime(string time, [Remainder]string rem)
        {
            DateTime fromTime = OriginTime(time);
            if (fromTime == new DateTime())
            {
                await ReplyAsync("No valid time parsed.");
                return;
            }

            string[] remSplit = rem.Split(',');
            if (remSplit.Length == 0)
            {
                await ReplyAsync("No timezones to convert to.");
                return;
            }

            IGuildUser fromUser = null;
            string[] start = remSplit[0].Split(' ');
            int toIndex = -1;
            for (int i = 0; i < start.Length; ++i)
            {
                if (i == 2)
                {
                    await ReplyAsync("Too many timezone origins.");
                    return;
                }
                if (start[i].ToLower().Equals("to"))
                {
                    toIndex = i;
                    break;
                }
            }

            fromUser = (toIndex < 1 ? (Context.User as IGuildUser) : (FindUser(start[0]) ?? (Context.User as IGuildUser)));
            if (toIndex + 1 == start.Length)
            {
                await ReplyAsync("No timezones to convert to.");
                return;
            }
            
            TimeZoneInfo fromZone = GetUserZone(fromUser).FirstOrDefault();

            List<IGuildUser> timeTos = new List<IGuildUser>();
            remSplit[0] = start[toIndex + 1];
            for (int i = 0; i < remSplit.Length; ++i)
            {
                IGuildUser u = FindUser(remSplit[i]);
                if (u != null)
                {
                    timeTos.Add(u);
                }
            }

            List<TimeZoneInfo> zones = new List<TimeZoneInfo>();
            DateTime rootTime = TimeZoneInfo.ConvertTimeToUtc(fromTime, fromZone);
            foreach (IGuildUser u in timeTos)
            {
                zones.Add(GetUserZone(u).FirstOrDefault());
                //await ReplyAsync(zones.Last().StandardName);
            }

            foreach (TimeZoneInfo tz in zones)
            {
                await ReplyAsync($"{fromTime.TimeOfDay} in {fromZone.StandardName} is {TimeZoneInfo.ConvertTimeFromUtc(rootTime, tz).TimeOfDay} in {tz.StandardName}\n");
            }
        }

        private DateTime OriginTime(string time)
        {
            int hours = -1, minutes = -1;

            DecypherTime(time, ref hours, ref minutes);

            if (hours == -1 || minutes == -1)
            {
                return new DateTime();
            }

            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, 0);
        }

        private void DecypherTime(string time, ref int hours, ref int minutes)
        {
            string[] timeSplit = time.ToLower().Split(new char[] { '.', ':' });
            string[] cleanSplit = new string[2];

            if (timeSplit.Length == 2)
            {
                cleanSplit[0] = timeSplit[0].Trim(' ');
                cleanSplit[1] = timeSplit[1].Split(new char[] { 'a', 'p' })[0].Trim(' ');
            }
            else if (timeSplit.Length == 1)
            {
                string nums = time.Split(new char[] { 'a', 'p' })[0].Trim(' ');
                switch (nums.Length)
                {
                    case 1:
                        cleanSplit[0] = $"0{nums}";
                        cleanSplit[1] = "00";
                        break;
                    case 2:
                        cleanSplit[0] = nums;
                        cleanSplit[1] = "00";
                        break;
                    case 3:
                        cleanSplit[0] = $"0{nums.Substring(0, 1)}";
                        cleanSplit[1] = nums.Substring(1, 2);
                        break;
                    case 4:
                        cleanSplit[0] = nums.Substring(0, 2);
                        cleanSplit[1] = nums.Substring(2, 2);
                        break;
                    default:
                        break;
                }
                // ~~~~~~~~    ^ old ^     ||    v new v   ~~~~~~~~~~~~~
                //if (nums.Length > 4)
                //{
                //    return;
                //}
                //cleanSplit[0] = nums.Substring(0, 2 - (nums.Length % 2));
                //cleanSplit[1] = nums.Substring(nums.Length - 2 * (nums.Length / 3), 2 * (nums.Length / 3));
            }
            else
            {
                return;
            }
            if (cleanSplit[0].Length == 1)
            {
                cleanSplit[0] = $"0{cleanSplit[0]}";
            }
            if (cleanSplit[1].Length != 2)
            {
                cleanSplit[1] = "00";
            }

            if (!int.TryParse(cleanSplit[0], out hours) || !int.TryParse(cleanSplit[1], out minutes))
            {
                return;
            }

            if ((hours < 12 && timeSplit.Last().Contains("p")) || (hours == 12 && timeSplit.Last().Contains("a")))
            {
                hours += 12;
                hours %= 24;
            }
        }

        private List<TimeZoneInfo> GetUserZone(IGuildUser user)
        {
            List<TimeZoneInfo> outZones = new List<TimeZoneInfo>();

            foreach (ulong id in user.RoleIds)
            {
                IRole role = Context.Guild.GetRole(id);
                if (role != null)
                {
                    if (TimeZoneConversion.shortToName.ContainsKey(role.Name)){
                        outZones.Add(TimeZoneInfo.FindSystemTimeZoneById(TimeZoneConversion.shortToName[role.Name]));
                    }
                }
            }

            return outZones;
        }

        private IGuildUser FindUser(string name)
        {
            name = name.ToLower().Trim(' ');
            foreach (IGuildUser u in Context.Guild.Users)
            {
                if ((u.Nickname != null && u.Nickname.ToLower().Equals(name))
                    || u.Mention.Equals(name))
                {
                    return u;
                }
                else
                {
                    string username = u.Username.ToLower();
                    if (username.Equals(name)
                        || (username + "#" + u.Discriminator).Equals(name))
                    {
                        return u;
                    }

                }
            }

            return null;
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
