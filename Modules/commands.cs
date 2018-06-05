using Discord.Commands;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using System.Collections.Generic;
using System.Diagnostics;

namespace new_bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        sqlFunctionsHandler handler = new sqlFunctionsHandler();

        decimal bot_version = 1.2m;

        string bot_uptime = Convert.ToString(DateTime.Now - Process.GetCurrentProcess().StartTime.AddTicks(-(Process.GetCurrentProcess().StartTime.Ticks % TimeSpan.TicksPerSecond)));

        [Command("!about")]
        public async Task AboutAsync()
        {
            try
            {
                await Context.User.SendMessageAsync($":bookmark: Bot developer:{Environment.NewLine}{Environment.NewLine}:flag_gb: **{Context.Guild.GetUser(265637738370433035).Username}#{Context.Guild.GetUser(265637738370433035).Discriminator}** " + Environment.NewLine + Environment.NewLine +
                    $"Version: **{bot_version}**" + Environment.NewLine +
                    $"Language: **C#**" + Environment.NewLine +
                    $"Uptime: **{bot_uptime}**"
                    );
            }
            catch (Exception)
            {
                throw;
            }
            finally { await Context.Message.DeleteAsync(); }
        }

        [Command("!warns")]
        public async Task warn(IGuildUser targetUser)
        {
            try
            {
                var builder = new EmbedBuilder()
                    .WithTitle("User Warns")
                    .WithDescription($"User {targetUser.Mention} has **{handler.numberOfWarns(targetUser.Id)}**/3 warns.")
                    .WithColor(new Color(0xFF0000))
                    .WithTimestamp(DateTime.UtcNow);

                var embed = builder.Build();
                await Context.User.SendMessageAsync("", false, embed);
            }
            catch (Exception)
            {
                throw;
            } finally { await Context.Message.DeleteAsync(); }
        }

        [Command("!warn")]
        public async Task warn(IGuildUser warnedUser, [Remainder] string reason)
        {
            try
            {
                if (handler.isStaff(Context.Guild.GetUser(Context.User.Id)) == false)
                {
                    await Context.User.SendMessageAsync(":exclamation: Insufficient permissions for this command");
                    return;
                }

                int previousWarnsTotal = handler.numberOfWarns(Context.User.Id);
                handler.warnUser(warnedUser.Id, Context.User.Id, reason);

                var builder = new EmbedBuilder()
                    .WithTitle("User Warned")
                    .WithDescription($":exclamation: User {warnedUser.Mention} has just been warned by {Context.User.Mention}\t\t\t" + Environment.NewLine + $"**Reason:** {reason}" + Environment.NewLine +
                    $"**Total Warnings:** {handler.numberOfWarns(warnedUser.Id)}/3" + Environment.NewLine + $"**Location:** #{Context.Channel}")
                    .WithColor(new Color(0xFF0000))
                    .WithTimestamp(DateTime.UtcNow);
                var embed = builder.Build();

                var pmBuilder = new EmbedBuilder()
                    .WithTitle("You've been warned")
                    .WithDescription($":exclamation: You've been warned by {Context.User.Mention}\t\t\t" + Environment.NewLine + $"**Reason:** {reason}" + Environment.NewLine +
                    $"**Total Warnings:** {handler.numberOfWarns(warnedUser.Id)}/3" + Environment.NewLine + $"**Location:** #{Context.Channel}")
                    .WithColor(new Color(0xFF0000))
                    .WithTimestamp(DateTime.UtcNow);
                var pmEmbed = pmBuilder.Build();


                var pmSuccBuilder = new EmbedBuilder()
                    .WithTitle("User Warned")
                    .WithDescription($":exclamation: You've just warned {warnedUser.Mention}\t\t\t" + Environment.NewLine + $"**Reason:** {reason}" + Environment.NewLine +
                    $"**Total Warnings:** {handler.numberOfWarns(warnedUser.Id)}/3" + Environment.NewLine + $"**Location:** #{Context.Channel}")
                    .WithColor(new Color(0x40E0D0))
                    .WithTimestamp(DateTime.UtcNow);
                var pmSuccEmbed = pmSuccBuilder.Build();

                await Context.User.SendMessageAsync("", false, pmSuccEmbed);
                await Context.Guild.GetTextChannel(451423996303376384).SendMessageAsync("", false, embed);
                await warnedUser.SendMessageAsync("", false, pmEmbed);

                if (handler.numberOfWarns(warnedUser.Id) >= 3) { await Context.Guild.AddBanAsync(warnedUser, 3, "Warnings exceeded 3"); handler.removeWarns(warnedUser.Id); }

            } catch (Exception)
            {
                throw;
            } finally { await Context.Message.DeleteAsync(); }
        }
    }

    public class commandExceptionHandler
    {
        public async Task HandleCommandException(SocketUserMessage message, IResult ex)
        {
            try
            {
                var builder = new EmbedBuilder()
                    .WithTitle($":exclamation: Invalid Syntax - You entered something incorrectly.")
                    //.WithDescription("A full list of updated commands and their roles.")
                    .WithColor(new Color(0x9E2CCC))
                    .WithTimestamp(DateTime.UtcNow)
                    .WithFooter(footer =>
                    {
                        footer
                            .WithText("Fallout 76: Helper Bot");
                            //.WithIconUrl("https://media.discordapp.net/attachments/325286562386673665/453660638607048704/FO76_Bot_pfp.png?width=521&height=521");
                    })
                    .WithThumbnailUrl("https://media.discordapp.net/attachments/325286562386673665/453660638607048704/FO76_Bot_pfp.png?width=521&height=521")
                    .WithAuthor(author =>
                    {
                        author
                            .WithName("Fallout 76: Helper Bot")
                            //.WithUrl("https://discord.gg/CJXCVaW")
                            .WithIconUrl("https://media.discordapp.net/attachments/325286562386673665/453660638607048704/FO76_Bot_pfp.png?width=521&height=521");
                    })
                    .AddField("To warn a user", "``!warn <user> <reason>`` _(optional)_");
                var embed = builder.Build();
                await message.Author.SendMessageAsync("", false, embed);
            }
            catch
            {
                throw;
            }
            finally
            {
                await message.DeleteAsync();
            }
        }
    }
}
