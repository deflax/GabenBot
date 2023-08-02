using System.Text;
using System.Text.Json;
using System.Threading;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using EventStore.Client;

namespace Simple.Discord.Bot.Commands;

public class GabenCommandHandlers
{
    private readonly Gaben gaben;
    private readonly EventStoreClient _client;
    private readonly DiscordRestClient _discordRestClient;

    public GabenCommandHandlers(EventStoreClient client, DiscordRestClient discordRestClient)
    {
        gaben = new Gaben();
        _client = client;
        _discordRestClient = discordRestClient;
    }

    public async Task MessageReactionAddedHandler(DiscordClient discordClient, MessageReactionAddEventArgs args)
    {
        var reactedOn = args.Message.Author is null ? "no-user-id" : args.Message.Author.Id.ToString();

        if (args.Message.Author is null)
        {
            var message = await _discordRestClient.GetMessageAsync(args.Channel.Id, args.Message.Id);

            reactedOn = message.Author.Id.ToString();
        }
        var @event = new ReactionAdded(args.Emoji.Name, args.User.Id.ToString(), reactedOn);

        var eventData = new EventData(
            Uuid.NewUuid(),
            "ReactionAdded",
            JsonSerializer.SerializeToUtf8Bytes(@event)
        );

        await _client.AppendToStreamAsync("reactions", StreamState.Any, new[] { eventData });
    }

    public async Task MessageCreatedHandler(DiscordClient s, MessageCreateEventArgs e)
    {
        if (e.Author.Username.Equals("Gaben", StringComparison.OrdinalIgnoreCase))
            return;

        var messageContent = e.Message.Content.ToLower();

        if (messageContent.Length < 3 || messageContent.Equals("!gaben", StringComparison.OrdinalIgnoreCase))
            return;

        var ratio = FuzzySharp.Fuzz.PartialRatio("gaben", messageContent);

        if (messageContent.Contains("габен"))
            ratio += 100;

        if (ratio < 76)
            return;

        var response = gaben.GetResponse();

        if (response.IsFile)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(response.Content);
            MemoryStream stream = new MemoryStream(byteArray);

            var msg2 = await new DiscordMessageBuilder()
                .WithContent("")
                .AddFile("gaben.txt", stream)
                .SendAsync(e.Channel);
        }
        else
        {
            var msg = await new DiscordMessageBuilder()
                .WithContent(response.Content)
                .SendAsync(e.Channel);
        }
    }
}

public record ReactionAdded(string ReactionName, string ReactorId, string ReactedOnId);