using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TimeZoneBot
{
    public class CommandHandlingService
    {
        private static CommandHandlingService instance = null;
        public static CommandHandlingService Instance
        {
            get
            {
                return instance;
            }
        }

        private readonly CommandService commands;
        private readonly DiscordSocketClient client;
        private readonly IServiceProvider services;

        public CommandService CommandsInstance { get { return commands; } }

        public CommandHandlingService(IServiceProvider services)
        {
            if(instance != null)
            {
                return;
            }
            instance = this;

            commands = services.GetRequiredService<CommandService>();
            client = services.GetRequiredService<DiscordSocketClient>();
            this.services = services;

            // Hook CommandExecuted to handle post-command-execution logic.
            commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }
            if (message.Source != MessageSource.User)
            {
                return;
            }
            
            var argPos = 0;
            if (!message.HasCharPrefix('~', ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(client, message);
            await commands.ExecuteAsync(context, argPos, services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command unspecified when command not found
            if (!command.IsSpecified)
                return;

            // command was successful
            if (result.IsSuccess)
                return;

            // command failed. Notify the user
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}