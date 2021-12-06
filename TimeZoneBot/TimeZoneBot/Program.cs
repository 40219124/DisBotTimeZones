using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;


namespace TimeZoneBot
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            using (IDisposable services = (IDisposable)ConfigureServices())
            {
                DiscordSocketClient client = ((IServiceProvider)services).GetRequiredService<DiscordSocketClient>();
                client.Log += Log;
                ((IServiceProvider)services).GetRequiredService<CommandService>().Log += Log;
                QualityOfLifeBits qol = new QualityOfLifeBits();
                client.UserVoiceStateUpdated += qol.VoiceStatusUpdate;
                client.MessageReceived += qol.MessageReceivedAsync;
                
                string token = File.ReadAllText(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\OneDrive\Documents\TimeZonerToken.txt"));

                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                await ((IServiceProvider)services).GetRequiredService<CommandHandlingService>().InitializeAsync();
                
                await Task.Delay(-1);
            }
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine($"~~ {msg.ToString()}");
            return Task.CompletedTask;
        }
    }
}
