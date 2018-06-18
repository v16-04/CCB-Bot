namespace DiscordSupportBot.Modules
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
 //   using DiscordSupportBot.Models.General;
    using DiscordSupportBot.Common;
    using DiscordSupportBot.Models.Exchanges;
    using DiscordSupportBot.Models.Github;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class GeneralModule : ModuleBase<SocketCommandContext>
    {
        private string[] VoteOptions = { "1⃣", "2⃣", "3⃣", "4⃣", "5⃣", "6⃣", "7⃣", "8⃣", "9⃣", "🔟" };
        private static HttpClient client = new HttpClient();

        [Command("help")]
        public async Task Help()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Ipsum Bot Help")
                .WithColor(Discord.Color.Blue)
                .WithThumbnailUrl("https://masternodes.online/coin_image/IPS.png")
                .WithFooter("https://github.com/CryptoCashBack-Hub")

                .AddField("//help", "shows available commands")
                .AddField("//ipsum or //ips", "shows coin info")
                .AddField("//guides or //guide", "replies with current installation guides")
                .AddField("//mnstatus <pubkey> or //masternode <pubkey>", "checks the status of your masternode")

                .AddField("//price <ticker> or //checkprice <ticker>", "replies with cmc price")
                .AddField("//donate or //donations", "replies with Dev IPS and BTC donation address")
                .AddField("//build or //version", "replies with current wallet realse link");

                


            var isBotChannel = this.Context.Channel.Id.Equals(DiscordSupportBot.Common.DiscordData.BotChannel);

            await this.Context.Guild.GetTextChannel(DiscordSupportBot.Common.DiscordData.BotChannel)
                .SendMessageAsync(isBotChannel ? string.Empty : this.Context.Message.Author.Mention, false, builder.Build());
        }

        [Command("guide")]
        [Alias("guides")]
        public async Task guide()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Master List of Guides").WithColor(Discord.Color.Blue)
                .WithDescription("\u200b")
                .WithUrl("https://github.com/CryptoCashBack-Hub/CCB_Guides")
                .WithThumbnailUrl("https://masternodes.online/coin_image/CCB.png")

                .AddField("The current wallet download", "https://github.com/CryptoCashBack-Hub/CCB/releases")
                .AddField("Complete install script for vps", "https://github.com/CryptoCashBack-Hub/CCB_Sripts")
                .AddField("Configuration Seed List", "https://github.com/CryptoCashBack-Hub/CCB_Guides/blob/master/Seeds");

            await this.ReplyAsync(string.Empty, false, builder.Build());
        }


        /*
        [Command("createpoll")]
        [Alias("poll")]
        public async Task Poll(string question, params string[] options)
        {
            var user = this.Context.Message.Author as SocketGuildUser;
            var permissiveRole = user.Roles.FirstOrDefault(r => r.Name.Equals("devs") || r.Name.Equals("admins"));

            if (permissiveRole != null)
            {
                var builder = new EmbedBuilder();
                var optionsList = this.GetVoteOptions(options);

                builder.WithTitle($"{question.Trim('?')}?")
                    .WithDescription(optionsList)
                    .WithColor(Discord.Color.Blue);

                var message = await this.Context.Guild.GetTextChannel(DiscordSupportBot.Common.DiscordData.BotPollChannel)
                    .SendMessageAsync(string.Empty, false, builder.Build());

                for (int i = 0; i < options.Length; i++)
                {
                    await message.AddReactionAsync(new Emoji(this.VoteOptions[i]));
                }
            }
            else
            {
                await this.Context.Message.Author.SendMessageAsync($"not enough permissions for the usage of poll command!");
            }
        }
        
        [Command("ipsum")]
        [Alias("ips")]
        public async Task Ipsum()
        {
            var data = this.GetGraviexData();

            var builder = new EmbedBuilder();

            builder.WithTitle("Ipsum [IPS]")
                .WithDescription("\u200b")
                .WithCurrentTimestamp()
                .WithFooter("https://ipsum.network/")
                .WithThumbnailUrl("https://masternodes.online/coin_image/IPS.png")
                .WithColor(Discord.Color.Blue);

            if (data.Result.Success)
            {
                builder
                    .AddInlineField("Time", $"{data.Result.TimeOfUpdate.ParseEpochToDateTime().ToString()}")
                    .AddInlineField("Price", $"{data.Result.Ticker.Last.ToString()}")
                    .AddInlineField("Volume BTC", $"{data.Result.Ticker.VolumeBtc.ToString()}");
            }
            else
            {
                builder
                    .AddField("", "could not retrieve data from exchange");
            }


            var isBotChannel = this.Context.Channel.Id.Equals(DiscordSupportBot.Common.DiscordData.BotChannel);

            await this.Context.Guild.GetTextChannel(DiscordSupportBot.Common.DiscordData.BotChannel)
                .SendMessageAsync(isBotChannel ? string.Empty : this.Context.Message.Author.Mention, false, builder.Build());
        }
        
        [Command("guide")]
        [Alias("guides")]
        public async Task Guide()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Master List of Guides")
                .WithColor(Discord.Color.Blue)
                .WithDescription("\u200b")
                .WithUrl("https://github.com/ipsum-network/guides")
                .WithThumbnailUrl("https://masternodes.online/coin_image/IPS.png")

                .AddField("PLEASE UPGRADE TO NEW WALLET VERSION ASAP", "https://github.com/ipsum-network/guides/blob/master/v3.1-UPDATE.md")
                .AddField("Full list of all guides:", "https://github.com/ipsum-network/guides")
                .AddField("Configuration Seed List:", "https://github.com/ipsum-network/seeds");


            var isBotChannel = this.Context.Channel.Id.Equals(DiscordSupportBot.Common.DiscordData.BotChannel);

            await this.Context.Guild.GetTextChannel(DiscordSupportBot.Common.DiscordData.BotChannel)
                .SendMessageAsync(isBotChannel ? string.Empty : this.Context.Message.Author.Mention, false, builder.Build());
        }

        [Command("donate")]
        [Alias("donations")]
        public async Task Donation()
        {
            var builder = new EmbedBuilder();

            var dataBtc = this.GetBtcDonationAddressBalance("1592K4xS5QkXDStELPk9nHBEqZ5vLNAyrm");
            var dataIps = this.GetIpsDonationAddressBalance("iSv6vXhSbb7WH8D3dVHuWecZ7pGj4AJMmt");

            builder.WithTitle("")
                .WithColor(Discord.Color.Blue)
                .WithThumbnailUrl("https://masternodes.online/coin_image/IPS.png")
                .AddField("Donations will be used for:", "Exchange Listings, Development, and Infrastructure")
                .AddField("IPS Donation Address:", "iSv6vXhSbb7WH8D3dVHuWecZ7pGj4AJMmt")
                .AddField("BTC Donation Address:", "1592K4xS5QkXDStELPk9nHBEqZ5vLNAyrm")
                .AddField("\u200b", "\u200b")
                .AddField("Current BTC donation balance:", $"{dataBtc.Result} BTC")
                .AddField("Current IPS donation balance:", $"{dataIps.Result} IPS");

            var isBotChannel = this.Context.Channel.Id.Equals(DiscordSupportBot.Common.DiscordData.BotChannel);

            await this.Context.Guild.GetTextChannel(DiscordSupportBot.Common.DiscordData.BotChannel)
                .SendMessageAsync(isBotChannel ? string.Empty : this.Context.Message.Author.Mention, false, builder.Build());
        }
        */
        [Command("build")]
        [Alias("version")]
        public async Task CurrentBuild()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("The current build is on v1.0.0.1").WithColor(Discord.Color.Blue)
                .WithThumbnailUrl("https://masternodes.online/coin_image/CCB.png")
                .WithDescription("\u200b")

                .AddField("Current wallet build!", "https://github.com/CryptoCashBack/CryptoCashBack/releases/");

            await this.ReplyAsync(string.Empty, false, builder.Build());
        }
        /*
        private string GetVoteOptions(string[] options)
        {
            var result = string.Empty;

            for (int i = 0; i < options.Length; i++)
            {
                result += $"\n{this.VoteOptions[i]} - {options[i]}";
            }

            return result;
        }
        
        private async Task<Graviex> GetGraviexData()
        {
            var response = await client.GetStringAsync($"https://graviex.net:443//api/v2/tickers/ipsbtc.json");
            var result = JsonConvert.DeserializeObject<Graviex>(response.ToString());

            return result;
        }
        */
        private async Task<GithubRelease> GetGithubReleaseData()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "request");

            var response = await client.GetStringAsync($"https://github.com/CryptoCashBack/CryptoCashBack/releases/");
            var result = JsonConvert.DeserializeObject<GithubRelease>(response.ToString());

            return result;
        }

        private async Task<decimal> GetBtcDonationAddressBalance(string address)
        {
            var response = await client.GetStringAsync($"https://blockchain.info/q/addressbalance/{address}");

            return decimal.Parse(response, System.Globalization.NumberStyles.AllowDecimalPoint) / 100000000;
        }

        private async Task<decimal> GetIpsDonationAddressBalance(string address)
        {
            var response = await client.GetStringAsync($"https://explorer.ipsum.network/ext/getbalance/{address}");

            return decimal.Parse(response, System.Globalization.NumberStyles.AllowDecimalPoint);
        }
    }
}
