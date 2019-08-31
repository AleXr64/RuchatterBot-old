using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Data;
using BotWithAPI.Extensions;
using Telegram.Bot.Framework.Abstractions;

namespace BotWithAPI.Handlers.Commands
{
	public class RulesPrint : CommandBase
	{
		private readonly MysqlDB db;
		public RulesPrint(MysqlDB db) { this.db = db; }

		public override async Task HandleAsync(
			IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
		{
			var originMessage = context.Update.Message;
			var chat = context.Update.Message.Chat;
			await context.Bot.Client.SendTextMessageAsync(chat, db.ChatParams.First(z => z.Chat == chat.Id).Rules,
																		 cancellationToken: cancellationToken,
																		 replyToMessageId: originMessage.MessageId);
		}
	}

	public class RulesSet : CommandBase
	{
		private readonly MysqlDB db;

		public RulesSet(MysqlDB db, ChatsHandler chatsHandler) { this.db = db; }

		public override async Task HandleAsync(
			IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
		{
			var chat = context.Update.Message.Chat;
			var from = context.Update.Message.From;
			if(!await from.IsAdminAsync(chat, context.Bot)) return;
			if(context.Update.Message.ReplyToMessage == null ||
				context.Update.Message.ReplyToMessage.Text == string.Empty) return;
			var rulesreply = context.Update.Message.ReplyToMessage.Text;

			var target = db.ChatParams.First(x => x.Chat == chat.Id);
			target.Rules = rulesreply;
			await db.SaveChangesAsync(cancellationToken);
		}
	}
}
