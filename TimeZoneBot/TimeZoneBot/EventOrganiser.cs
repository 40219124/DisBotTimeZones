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
    class EventOrganiser
    {
        public enum eDateDisplay
        {
            DDMMYYYY = 'd', DMonthYYYY = 'D',                       // Date
            HH_MM = 't', HH_MM_SS = 'T',                            // Time
            DMonthYYYY_HH_MM = 'f', Day_DMonthYYYY_HH_MM = 'F',     // Date & Time
            TimeAgo = 'R'                                           // The Past
        }

        public enum eWaitingFor { none = -1, Year, Month, Day, Hour, Minute, Second, Title, Description, Roles }

        public class EventDetails
        {
            public DateTime Time = new DateTime(2021, 12, 25, 0, 0, 1);
            public string Title = "";
            public string Description = "";
            public List<ulong> Roles = new List<ulong>();

        }

        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        eWaitingFor PlanningStage = eWaitingFor.none;
        EventDetails TheEvent = new EventDetails();
        ulong PlanningMessageID = 0;
        ulong PlanningPromptMessageId = 0;
        // ~~~ event planner and channel id's for the messages without commands

        public async Task MessageReceivedAsync(SocketMessage message)
        {
            if (!(message.Channel is IGuildChannel channel && channel.Guild != null) || message.Author.IsBot)
            {
                return;
            }
            Console.WriteLine($"message recieved from: {((IGuildChannel)message.Channel).Guild.Name}");

            if (message.Content.StartsWith("~plan"))// || (!message.Content.StartsWith("~") && PlanningStage != eWaitingFor.none))
            {
                await PlanEvent(message);
            }
        }

        public async Task MessageEditedAsync(Cacheable<IMessage, ulong> oldMessage, SocketMessage message, IMessageChannel channel)
        {
            Console.WriteLine($"message edited in: {((IGuildChannel)message.Channel).Guild.Name}");
            if (message.Id != PlanningMessageID)
            {
                return;
            }
            await PlanEvent(message);
        }

        public async Task PlanEvent(SocketMessage message)
        {
            if (message.Content.ToLower().Equals("cancel"))
            {
                PlanningStage = eWaitingFor.none;
                PlanningMessageID = 0;
                await message.Channel.ModifyMessageAsync(PlanningPromptMessageId, x => x.Content = "Event cleared");
                return;
            }
            switch (PlanningStage)
            {
                case eWaitingFor.Year:
                    TheEvent.Time.AddYears(int.Parse(message.Content) - TheEvent.Time.Year);
                    await message.Channel.ModifyMessageAsync(PlanningPromptMessageId, x => x.Content = "What month? (MM)");
                    PlanningStage = eWaitingFor.Month;
                    return;
                case eWaitingFor.Month:
                    TheEvent.Time.AddMonths(int.Parse(message.Content) - TheEvent.Time.Month);
                    await message.Channel.ModifyMessageAsync(PlanningPromptMessageId, x => x.Content = "What day? (DD)");
                    PlanningStage = eWaitingFor.Day;
                    return;
                case eWaitingFor.Day:
                    TheEvent.Time.AddDays(int.Parse(message.Content) - TheEvent.Time.Day);
                    await message.Channel.ModifyMessageAsync(PlanningPromptMessageId, x => x.Content = "What hour? (hh [0,24])");
                    PlanningStage = eWaitingFor.Hour;
                    return;
                case eWaitingFor.Hour:
                    TheEvent.Time.AddHours(int.Parse(message.Content) - TheEvent.Time.Hour);
                    await message.Channel.ModifyMessageAsync(PlanningPromptMessageId, x => x.Content = "What minute? (mm)");
                    PlanningStage = eWaitingFor.Minute;
                    return;
                case eWaitingFor.Minute:
                    TheEvent.Time.AddMinutes(int.Parse(message.Content) - TheEvent.Time.Minute);
                    await message.Channel.ModifyMessageAsync(PlanningPromptMessageId, x => x.Content = "What second? (mm)");
                    PlanningStage = eWaitingFor.Second;
                    return;
                case eWaitingFor.Second:
                    TheEvent.Time.AddHours(int.Parse(message.Content) - TheEvent.Time.Second);
                    await message.Channel.ModifyMessageAsync(PlanningPromptMessageId, x => x.Content = "What will the title be?");
                    PlanningStage = eWaitingFor.Title;
                    return;
                case eWaitingFor.Title:
                    TheEvent.Title = message.Content;
                    await message.Channel.ModifyMessageAsync(PlanningPromptMessageId, x => x.Content = "What is the description?");
                    PlanningStage = eWaitingFor.Description;
                    return;
                case eWaitingFor.Description:
                    TheEvent.Description = message.Content;
                    await message.Channel.ModifyMessageAsync(PlanningPromptMessageId, x => x.Content = "Who should be notified?");
                    PlanningStage = eWaitingFor.Roles;
                    return;
                case eWaitingFor.Roles:
                    // ~~~ break up roles and add them.
                    await message.Channel.ModifyMessageAsync(PlanningPromptMessageId, x => x.Content = "Planning complete.");
                    PlanningStage = eWaitingFor.none;
                    PlanningPromptMessageId = 0;
                    PlanningMessageID = 0;

                    await message.Channel.SendMessageAsync($"**{TheEvent.Title}**\n{TheEvent.Description}\n<t:{new TimeSpan(TheEvent.Time.Ticks - UnixEpoch.Ticks).TotalSeconds}:{(char)eDateDisplay.Day_DMonthYYYY_HH_MM}>");
                    return;
                default:
                    Task<Discord.Rest.RestUserMessage> t = message.Channel.SendMessageAsync("Time to plan your event.\nWhat year is it? (YYYY)");
                    await t;
                    PlanningPromptMessageId = t.Result.Id;
                    PlanningMessageID = message.Id;
                    PlanningStage = eWaitingFor.Year;
                    return;
            }
        }
    }
}
