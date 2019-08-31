using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Data;
using BotWithAPI.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotWithAPI.Handlers.Commands.AdminCommands
{
	public class WarnCommand : CommandBase
	{
		private readonly BanCommand ban;
		private readonly MysqlDB db;

		public WarnCommand(MysqlDB db, BanCommand ban)
		{
			this.db = db;
			this.ban = ban;
		}

		public override async Task HandleAsync(
			IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
		{
			var originMessage = context.Update.Message;
			var me = await context.Bot.Client.GetMeAsync(cancellationToken);
			if(context.Update.Message.ReplyToMessage == null) return;
			var from = context.Update.Message.From;
			var to = context.Update.Message.ReplyToMessage.From;

			var chat = context.Update.Message.Chat;
			if(from.Id == to.Id || to == me || await to.IsAdminAsync(chat, context.Bot)) return;
			if(await from.IsAdminAsync(chat, context.Bot))
				{
					var user = db.UserParams.First(x => x.User == to.Id && x.Chat == chat.Id);
					user.Warns++;
					if(user.Warns > 3)
						{
							user.Warns = 3;
							db.Update(user);
							return;
						}

					db.Update(user);
					await db.SaveChangesAsync(cancellationToken);
					await context.Bot.Client.SendTextMessageAsync(chat, inform(to, user.Warns), ParseMode.Html,
																				 cancellationToken: cancellationToken,
																				 replyToMessageId: originMessage.ReplyToMessage.MessageId);
					if(user.Warns >= 3) await ban.HandleAsync(context, next, args, cancellationToken);
				}
		}

		private static string inform(User from, int count)
		{
			var writer = new StringWriter();
			writer.Write(from.Mention());
			var code = new TagBuilder("code");
			code.InnerHtml.Append(", Вам выдано предупреждение! ");
			code.InnerHtml.Append($"Текущее кол-во: {count}/3");
			code.WriteTo(writer, HtmlEncoder.Default);
			return writer.ToString();
		}
	}
}
