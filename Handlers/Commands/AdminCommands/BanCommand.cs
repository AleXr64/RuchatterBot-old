using System.IO;
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
	public class BanCommand : CommandBase
	{
		private MysqlDB db;
		public BanCommand(MysqlDB db) { this.db = db; }

		public override async Task HandleAsync(
			IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
		{
			var originMessage = context.Update.Message;
			var me = await context.Bot.Client.GetMeAsync(cancellationToken);
			if(originMessage.ReplyToMessage == null) return;
			var from = originMessage.From;
			var to = originMessage.ReplyToMessage.From;
			var chat = originMessage.Chat;
			if(from.Id == to.Id || to == me || await to.IsAdminAsync(chat, context.Bot)) return;
			if(await from.IsAdminAsync(chat, context.Bot))
				{
					await context.Bot.Client.SendTextMessageAsync(chat, inform(from, to), ParseMode.Html,
																				 cancellationToken: cancellationToken,
																				 replyToMessageId: originMessage.ReplyToMessage.MessageId);
					await context.Bot.Client.KickChatMemberAsync(chat, to.Id, cancellationToken: cancellationToken);
				}
		}

		private string inform(User from, User to)
		{
			var writer = new StringWriter();
			writer.Write(from.Mention());
			writer.Write(" заблокировал пользователя ".ToHTMLTagCode());
			writer.Write(to.Mention());
			return writer.ToString();
		}
	}
}
