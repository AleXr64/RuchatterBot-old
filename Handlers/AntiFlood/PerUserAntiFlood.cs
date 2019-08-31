using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Data.Model;
using BotWithAPI.Extensions;
using BotWithAPI.Util;
using Microsoft.AspNetCore.Mvc.Rendering;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotWithAPI.Handlers.AntiFlood
{
	public class PerUserAntiFlood : IUpdateHandler
	{
		private readonly RestrictionCache restrictions;
		public PerUserAntiFlood(RestrictionCache restrictions) { this.restrictions = restrictions; }

		public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
		{
			var mess = context.Update.Message;
			var from = context.Update.Message.From;
			if(await from.IsAdminAsync(context.Update.Message.Chat, context.Bot))
				{
					await next(context, cancellationToken);
					return;
				}

			restrictions.CommandFloodRestrictions.RemoveAll(x => x.EndTime < DateTime.Now);
			var now = DateTime.Now;
			var user = new UserParams(context.Update.Message.From.Id, context.Update.Message.Chat.Id);
			var endTime = now + TimeSpan.FromMinutes(3);
			var restrict = new FloodRestrictions(endTime, user);
			var current = restrictions.CommandFloodRestrictions.Find(x => x.User == user);
			if(current == null) //is allow?
				{
					restrictions.CommandFloodRestrictions.Add(restrict);
					await next(context, cancellationToken);
					return;
				}

			if(!current.IsSecondTry)
				{
					restrictions.CommandFloodRestrictions.Remove(current);

					current.IsSecondTry = true;
					restrictions.CommandFloodRestrictions.Add(current);
					await next(context, cancellationToken);
				}
			else
				{
					if(!current.IsInformed)
						{

                            try
                                {
                                    await context.Bot.Client.SendTextMessageAsync(current.User.Chat, FloodInformer(from),
                                                                                  cancellationToken:
                                                                                  cancellationToken, parseMode: ParseMode.Html,
                                                                                  replyToMessageId: mess.MessageId);
                                } catch(Exception e)
                                {
                                    Console.WriteLine(e);
                                    
                                }
							restrictions.CommandFloodRestrictions.Remove(current);

							current.IsInformed = true;
							restrictions.CommandFloodRestrictions.Add(current);
						}
				}
		}

		private static string FloodInformer(User from)
		{
			var code = new TagBuilder("code");
			code.InnerHtml.Append($"Антифлуд: игнорирование команд от {from.UserFullName()} в течении 3-ех минут");
			var writer = new StringWriter();
			code.WriteTo(writer, HtmlEncoder.Default);
			return writer.ToString();
		}
	}
}
