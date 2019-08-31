using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Data;
using BotWithAPI.Extensions;
using BotWithAPI.Util;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotWithAPI.Handlers.Commands.AdminCommands
{
	public class UnWarnCommand : CommandBase
	{
		private readonly MysqlDB db;
		public UnWarnCommand(MysqlDB db) { this.db = db; }

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
					if(user.Warns <= 0)
						{
							user.Warns = 0;
							return;
						}

					user.Warns--;
					db.Update(user);
					await db.SaveChangesAsync(cancellationToken);
					await context.Bot.Client.SendTextMessageAsync(chat, inform(to, user.Warns), ParseMode.Html,
																				 cancellationToken: cancellationToken,
																				 replyToMessageId: originMessage.ReplyToMessage.MessageId);
				}
		}

		private static string inform(User from, int count)
		{
			var writer = new StringWriter();
			writer.Write(from.Mention());
			var text = $", с Вас снято предупреждение! Текущее кол-во: {count}/3".ToHTMLTagCode();
			writer.Write(text);
			return writer.ToString();
		}
	}
}
