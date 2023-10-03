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

    [Command("memer")]
    public async Task GetMemer(CommandContext ctx)
    {
        var events = await GetEventsAsync();

        // Create a dictionary to hold the counts of reactions a user received
        var userReactionsCounts = new Dictionary<string, int>();

        // Loop through each event
        foreach (var ev in events)
        {
            string jsonString = Encoding.UTF8.GetString(ev.Event.Data.ToArray());
            // Deserialize the event data to a dynamic object
            ReactionAdded eventData = JsonSerializer.Deserialize<ReactionAdded>(jsonString);

            // Get the user id of the user who was reacted on
            string reactedOnUserId = eventData.ReactedOnId;

            // If the user id is not already in the dictionary, add it
            if (!userReactionsCounts.ContainsKey(reactedOnUserId))
            {
                userReactionsCounts[reactedOnUserId] = 0;
            }

            // Increment the count for this user
            userReactionsCounts[reactedOnUserId]++;
        }

        // Get the top 10 users with the most reactions
        var topUsers = userReactionsCounts.OrderByDescending(x => x.Value).Take(10);

        // Build a message to display the top users
        var topUsersString = new StringBuilder("Top 10 users reacted on are:\n");
        int counter = 0;

        foreach (var kvp in topUsers)
        {
            try
            {
                var user = await DiscordClient.GetUserAsync(ulong.Parse(kvp.Key));
                topUsersString.AppendLine($"{counter + 1}. {user.Username} - {kvp.Value}");
                counter++;
            }
            catch (Exception)
            {

            }
        }

        await DiscordClient.SendMessageAsync(ctx.Channel, topUsersString.ToString());
    }


    [Command("copespammer")]
    public async Task GetCopeSpammer(CommandContext ctx)
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

            if (eventData is null || eventData.ReactionName.Equals("copium") == false)
                continue;

            // Get the user id of the user who was reacted on
            string reactorId = eventData.ReactorId;

            // If the user id is not already in the dictionary, add it
            if (!userReactionsCount.ContainsKey(reactorId))
            {
                userReactionsCount[reactorId] = 0;
            }

            // Increment the count for this user
            userReactionsCount[reactorId]++;
        }

        var topUsers = userReactionsCount.OrderByDescending(x => x.Value).Take(10);

        // Build a message to display the top users
        var copiumEmoji = await GetEmoji(ctx.Guild.Id, "copium");
        var topUsersString = new StringBuilder($"Top 10 <:{"copium"}:{copiumEmoji.ToString()}>spammers are:\n");
        int counter = 0;

        foreach (var kvp in topUsers)
        {
            try
            {
                var user = await DiscordClient.GetUserAsync(ulong.Parse(kvp.Key));
                topUsersString.AppendLine($"{counter + 1}. {user.Username} - {kvp.Value}");
                counter++;
            }
            catch (Exception)
            {
            }
        }

        await DiscordClient.SendMessageAsync(ctx.Channel, topUsersString.ToString());

    }

    [Command("copelord")]
    public async Task GetCopelord(CommandContext ctx)
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

            if (eventData is null || eventData.ReactionName.Equals("copium") == false)
                continue;

            // Get the user id of the user who was reacted on
            string reactedOnId = eventData.ReactedOnId;

            // If the user id is not already in the dictionary, add it
            if (!userReactionsCount.ContainsKey(reactedOnId))
            {
                userReactionsCount[reactedOnId] = 0;
            }

            // Increment the count for this user
            userReactionsCount[reactedOnId]++;
        }

        var topUsers = userReactionsCount.OrderByDescending(x => x.Value).Take(10);

        // Build a message to display the top users
        var copiumEmoji = await GetEmoji(ctx.Guild.Id, "copium");
        var topUsersString = new StringBuilder($"Top 10 <:{"copium"}:{copiumEmoji.ToString()}>lords are:\n");
        int counter = 0;

        foreach (var kvp in topUsers)
        {
            try
            {
                var user = await DiscordClient.GetUserAsync(ulong.Parse(kvp.Key));
                topUsersString.AppendLine($"{counter + 1}. {user.Username} - {kvp.Value}");
                counter++;
            }
            catch (Exception)
            {
            }
        }

        await DiscordClient.SendMessageAsync(ctx.Channel, topUsersString.ToString());
    }

    [Command("reactor")]
    public async Task GetReactor(CommandContext ctx)
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

            // Get the user id of the user who was reacted on
            string reactorId = eventData.ReactorId;

            // If the user id is not already in the dictionary, add it
            if (!userReactionsCount.ContainsKey(reactorId))
            {
                userReactionsCount[reactorId] = 0;
            }

            // Increment the count for this user
            userReactionsCount[reactorId]++;
        }

        // Get the top 10 users with the most reactions
        var topUsers = userReactionsCount.OrderByDescending(x => x.Value).Take(10);

        // Build a message to display the top users
        var topUsersString = new StringBuilder("Top 10 users who reacted are:\n");
        int counter = 0;

        foreach (var kvp in topUsers)
        {
            try
            {
                var user = await DiscordClient.GetUserAsync(ulong.Parse(kvp.Key));
                topUsersString.AppendLine($"{counter + 1}. {user.Username} - {kvp.Value}");
                counter++;
            }
            catch (Exception)
            {
            }
        }

        await DiscordClient.SendMessageAsync(ctx.Channel, topUsersString.ToString());
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