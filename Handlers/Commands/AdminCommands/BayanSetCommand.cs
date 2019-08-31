using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Data;
using BotWithAPI.Extensions;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;

namespace BotWithAPI.Handlers.Commands.AdminCommands
{
	public class BayanSetCommand : CommandBase
	{
		private readonly MysqlDB db;
		public BayanSetCommand(MysqlDB db) { this.db = db; }

		public override async Task HandleAsync(
			IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
		{
			var from = context.Update.Message.From;

			var currentChat = context.Update.Message.Chat;
			if(!await from.IsAdminAsync(currentChat, context.Bot)) return;
			
			if (context.Update.Message.ReplyToMessage.Document == null)
				return;
			var file = context.Update.Message.ReplyToMessage.Document;
	


			
			var chat = db.ChatParams.First(x => x.Chat == currentChat.Id);
			if(db.ChatSettings.Any(x => x.BayanAnimation == file.FileId))
				chat.Settings.BayanAnimation =
					db.ChatSettings.First(x => x.BayanAnimation == file.FileId).BayanAnimation;

			else
				chat.Settings.BayanAnimation = file.FileId;

			db.Update(chat);
			db.SaveChanges();
		}
	}
}
