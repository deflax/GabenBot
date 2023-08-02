using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Simple.Discord.Bot.Commands;

public static class Program
{
    static async Task Main(string[] args)
    {
        await using var fs = File.OpenRead("./appsettings.json");
        using var sr = new StreamReader(fs, new UTF8Encoding(false));
        var json = await sr.ReadToEndAsync();

        var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
        var config = new DiscordConfiguration()
        {
            Token = configJson.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
        };
        var discord = new DiscordClient(config);
        var discordRestClient = new DiscordRestClient(config);
        DiscordActivity status = new("The sound of $$$", ActivityType.ListeningTo);

        var settings = EventStoreClientSettings.Create(configJson.EventStoreConnectionString);
        EventStoreClient client = new EventStoreClient(settings);

        var gaben = new GabenCommandHandlers(client, discordRestClient);

        var services = new ServiceCollection()
                .AddSingleton<EventStoreClient>(client)
                .AddSingleton<DiscordClient>(discord)
                .AddSingleton<DiscordRestClient>(discordRestClient)
                .BuildServiceProvider();

        var commandsConfig = new CommandsNextConfiguration()
        {
            StringPrefixes = new string[] { "!" },
            Services = services
        };

        discord.MessageCreated += gaben.MessageCreatedHandler;
        discord.MessageReactionAdded += gaben.MessageReactionAddedHandler;

        var commands = discord.UseCommandsNext(commandsConfig);
        commands.RegisterCommands<GabenCommandModule>();

        await discord.ConnectAsync(status, UserStatus.Online);
        await Task.Delay(-1);
    }
}