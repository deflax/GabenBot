# GabenBot

GabenDiscordBot is a Discord bot that tracks reactions and messages containing some version of the word "Gaben". It's a fun and interactive way to engage with your Discord community!

## Requirements

- .NET 7.0 or above
- Discord Bot Token
- EventStoreDb instance

## Setup

Clone the repository:

```bash
git clone https://github.com/yourusername/GabenDiscordBot.git
```

Navigate into the project directory:
```bash
cd GabenDiscordBot
```

## Appsettings.json

In the root directory of the GabenDiscordBot project, create a new file named appsettings.json.

Here's a template for what the appsettings.json file should look like:
```json
{
  "token": "<Your Discord Bot Token>",
  "prefix": "!",
  "eventStoreConnectionString": "esdb://<username>:<password>@<host>?tls=<tlsvalue>"
}
```
Replace <Your Discord Bot Token> with your actual Discord bot token, and <Command Prefix for your Bot> with the prefix you want to use for bot commands. This prefix will be used to trigger your bot's commands in Discord.
Running the Bot. Lastly put the connection string to your EventStoreDb

Once you've set up your appsettings.json file, you can run the bot with the following command:
```bash
dotnet run
```

## Contribution

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
