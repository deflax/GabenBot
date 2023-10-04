using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.Client;
using Simple.Discord.Bot.Commands;
using System.Text.Json;
using DSharpPlus;

public class GabenCommandModule : BaseCommandModule
{
    private const string Spammer = "spammers";
    private const string Lord = "lords";
    private const string CopiumEmoji = "copium";

    public EventStoreClient EventStoreClient { private get; set; }
    public DiscordClient DiscordClient { private get; set; }
    public DiscordRestClient DiscordRestClient { private get; set; }

    [Command("gaben")]
    public async Task GetLeaderBoard(CommandContext ctx)
    {
        // Get the events from the event store
        var events = await GetEventsAsync();

        // Create a dictionary to hold the reaction counts
        var reactionCounts = new Dictionary<string, int>();

        // Loop through each event
        foreach (var ev in events)
        {

            string jsonString = Encoding.UTF8.GetString(ev.Event.Data.ToArray());
            // Deserialize the event data to a dynamic object
            ReactionAdded eventData = JsonSerializer.Deserialize<ReactionAdded>(jsonString);

            // Get the reaction name from the event data
            string reactionName = eventData.ReactionName;

            // If the reaction name is not already in the dictionary, add it
            if (!reactionCounts.ContainsKey(reactionName))
            {
                reactionCounts[reactionName] = 0;
            }

            // Increment the count for this reaction
            reactionCounts[reactionName]++;
        }


        var reactions = reactionCounts.OrderByDescending(x => x.Value).Take(10);
        // Print out the reaction counts

        var topReactionsString = new StringBuilder("Top reactions are:\n");
        var zz = await GetEmojis(ctx.Guild.Id, reactions.Select(x => x.Key).ToList());
        int counter = 0;
        foreach (var kvp in reactions)
        {
            if (zz.ContainsKey(kvp.Key))
                topReactionsString.AppendLine($"{counter + 1}. <:{kvp.Key}:{zz[kvp.Key].ToString()}> - {kvp.Value}");
            else
                topReactionsString.AppendLine($"{counter + 1}. :{kvp.Key}: - {kvp.Value}");

            counter++;
        }

        await DiscordClient.SendMessageAsync(ctx.Channel, topReactionsString.ToString());
    }

    [Command("memespammer")]
    public async Task GetMemeSpammer(CommandContext ctx)
    {
        var messageBuilder = new StringBuilder("Top 10 users who reacted are:\n");

        await GetTopUsersByReaction(ctx, Spammer, messageBuilder);
    }

    [Command("memelord")]
    public async Task GetMemeLord(CommandContext ctx)
    {
        var messageBuilder = new StringBuilder("Top 10 users reacted on are:\n");

        await GetTopUsersByReaction(ctx, Lord, messageBuilder);
    }

    [Command("copespammer")]
    public async Task GetCopeSpammer(CommandContext ctx)
    {
        var copiumEmoji = await GetEmoji(ctx.Guild.Id, CopiumEmoji);

        var messageBuilder = new StringBuilder($"Top 10 <:{CopiumEmoji}:{copiumEmoji}>{Spammer} are:\n");

        await GetTopUsersByReaction(ctx, Spammer, messageBuilder, CopiumEmoji);
    }

    [Command("copelord")]
    public async Task GetCopelord(CommandContext ctx)
    {
        var copiumEmoji = await GetEmoji(ctx.Guild.Id, CopiumEmoji);

        var messageBuilder = new StringBuilder($"Top 10 <:{CopiumEmoji}:{copiumEmoji}>{Lord} are:\n");

        await GetTopUsersByReaction(ctx, Lord, messageBuilder, CopiumEmoji);
    }

    [Command("mmr")]
    public async Task GetMmr(CommandContext ctx)
    {
        // Get the events from the event store
        var events = await GetEventsAsync();

        // Create a dictionary to hold the reaction counts
        var reactionCounts = new Dictionary<string, int>();

        // Loop through each event
        foreach (var ev in events)
        {

            string jsonString = Encoding.UTF8.GetString(ev.Event.Data.ToArray());
            // Deserialize the event data to a dynamic object
            ReactionAdded eventData = JsonSerializer.Deserialize<ReactionAdded>(jsonString);

            // Get the reaction name from the event data
            string reactionName = eventData.ReactionName;

            // If the reaction name is not already in the dictionary, add it
            if (!reactionCounts.ContainsKey(eventData.ReactedOnId))
            {
                reactionCounts[eventData.ReactedOnId] = 0;
            }

            if (reactionName == "minusmmr")
                reactionCounts[eventData.ReactedOnId] -= 30;

            if (reactionName == "plusmmr")
                reactionCounts[eventData.ReactedOnId] += 30;
        }

        var reactionsToRemove = reactionCounts.Where(x => x.Value == 0);


        var reactions = reactionCounts.Except(reactionsToRemove).OrderByDescending(x => x.Value);
        // Print out the reaction counts

        var UserMmrs = new StringBuilder("Current mmr ladder is :\n");
        int counter = 0;
        foreach (var kvp in reactions)
        {
            try
            {
                var user = await DiscordClient.GetUserAsync(ulong.Parse(kvp.Key));
                UserMmrs.AppendLine($"{counter + 1}. {user.Username} == {kvp.Value}");
                counter++;
            }
            catch (Exception)
            {
                UserMmrs.AppendLine($"could not load for user with id: {kvp.Key} mmr value {kvp.Value}");
            }
        }

        await DiscordClient.SendMessageAsync(ctx.Channel, UserMmrs.ToString());
    }

    private async Task GetTopUsersByReaction(CommandContext ctx, string userDescriptor, StringBuilder stringBuilder, string? reactionName = null)
    {
        var events = await GetEventsAsync();

        // Create a dictionary to hold the counts of reactions a user received
        var userReactionsCount = new Dictionary<string, int>();

        // Loop through each event
        foreach (var ev in events)
        {
            string jsonString = Encoding.UTF8.GetString(ev.Event.Data.ToArray());
            // Deserialize the event data to a dynamic object
            ReactionAdded eventData = JsonSerializer.Deserialize<ReactionAdded>(jsonString);

            if (eventData is null || (reactionName is not null && eventData.ReactionName.Equals(reactionName) == false))
                continue;

            // Decide which Id to use based on the descriptor
            string userId = userDescriptor == Spammer ? eventData.ReactorId : eventData.ReactedOnId;

            // If the user id is not already in the dictionary, add it
            if (!userReactionsCount.ContainsKey(userId))
            {
                userReactionsCount[userId] = 0;
            }

            // Increment the count for this user
            userReactionsCount[userId]++;
        }

        var topUsers = userReactionsCount.OrderByDescending(x => x.Value).Take(10);

        // Build a message to display the top users
        int counter = 0;

        foreach (var kvp in topUsers)
        {
            try
            {
                var user = await DiscordClient.GetUserAsync(ulong.Parse(kvp.Key));
                stringBuilder.AppendLine($"{counter + 1}. {user.Username} - {kvp.Value}");
                counter++;
            }
            catch (Exception)
            {
            }
        }

        await DiscordClient.SendMessageAsync(ctx.Channel, stringBuilder.ToString());
    }


    private async Task<List<ResolvedEvent>> GetEventsAsync()
    {
        var result = EventStoreClient.ReadStreamAsync(
                            Direction.Forwards,
                            "reactions",
                            StreamPosition.Start);

        List<ResolvedEvent> events = await result.ToListAsync();

        return events;
    }

    private async Task<Dictionary<string, ulong>> GetEmojis(ulong guildId, List<string> emojisToGet)
    {
        Dictionary<string, ulong> emojiIds = new Dictionary<string, ulong>();
        var guild = await DiscordRestClient.GetGuildAsync(guildId);
        foreach (var emoji in emojisToGet)
        {
            try
            {
                KeyValuePair<ulong, DSharpPlus.Entities.DiscordEmoji> emojiMetadata = guild.Emojis.SingleOrDefault(x => x.Value.Name.ToString().Equals(emoji, StringComparison.OrdinalIgnoreCase));
                if (emojiMetadata.Value is null == false)
                    emojiIds.Add(emojiMetadata.Value.Name, emojiMetadata.Value.Id);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }

        return emojiIds;
    }

    private async Task<ulong> GetEmoji(ulong guildId, string emojiToGet)
    {
        var guild = await DiscordRestClient.GetGuildAsync(guildId);
        KeyValuePair<ulong, DSharpPlus.Entities.DiscordEmoji> emojiMetadata = guild.Emojis.SingleOrDefault(x => x.Value.Name.ToString().Equals(emojiToGet, StringComparison.OrdinalIgnoreCase));
        if (emojiMetadata.Value is null == false)
            return emojiMetadata.Value.Id;

        return 0;
    }
}