using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Data;
using BotWithAPI.Extensions;
using BotWithAPI.Util;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotWithAPI.Handlers.Commands
{
	public class UserWithRep
	{
		public int Rep;
		public User User;
	}

	public class ReputationPlus : IUpdateHandler
	{
		private readonly MysqlDB db;
		public ReputationPlus(MysqlDB db) { this.db = db; }

		public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
		{
			if(context.Update.Message.ReplyToMessage == null) return;

			var originMessage = context.Update.Message;
	if (!db.ChatParams.First(x=>x.Chat == originMessage.Chat.Id).Settings.ReputationEnabled) return;
	
	 if (originMessage.From == originMessage.ReplyToMessage.From ||
				originMessage.ReplyToMessage.From == context.Bot.Client.GetMeAsync(cancellationToken).Result) return;
			var From = new UserWithRep {User = originMessage.From};
			var To = new UserWithRep {User = originMessage.ReplyToMessage.From};
			From.Rep = db.UserParams.First(x => x.User == From.User.Id && x.Chat == originMessage.Chat.Id).Reputation;

			var nrep = db.UserParams.First(x => x.User == To.User.Id && x.Chat == originMessage.Chat.Id);
			nrep.Reputation++;
			To.Rep = nrep.Reputation;
			db.Update(nrep);
			await db.SaveChangesAsync(cancellationToken);
            try
                {
                    await context.Bot.Client.SendTextMessageAsync(context.Update.Message.Chat,
                                                                  ReputationLang
                                                                     .ReputationPlus(From, To),
                                                                  ParseMode.Html,
                                                                  cancellationToken: cancellationToken,
                                                                  replyToMessageId: originMessage.MessageId);
                } catch(ApiRequestException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
		}
	}

	public class ReputationMinus : IUpdateHandler
	{
		private readonly MysqlDB db;
		public ReputationMinus(MysqlDB db) { this.db = db; }

		public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
		{
			if(context.Update.Message.ReplyToMessage == null) return;

			var originMessage = context.Update.Message;
	if (!db.ChatParams.First(x => x.Chat == originMessage.Chat.Id).Settings.ReputationEnabled) return;
	if (originMessage.From == originMessage.ReplyToMessage.From ||
				originMessage.ReplyToMessage.From == context.Bot.Client.GetMeAsync(cancellationToken).Result) return;
			var From = new UserWithRep {User = originMessage.From};
			var To = new UserWithRep {User = originMessage.ReplyToMessage.From};
			From.Rep = db.UserParams.First(x => x.User == From.User.Id && x.Chat == originMessage.Chat.Id).Reputation;

			var nrep = db.UserParams.First(x => x.User == To.User.Id && x.Chat == originMessage.Chat.Id);
			nrep.Reputation--;
			To.Rep = nrep.Reputation;
			db.Update(nrep);
			await db.SaveChangesAsync(cancellationToken);
            try
                {
                    await context.Bot.Client.SendTextMessageAsync(context.Update.Message.Chat,
                                                                  ReputationLang
                                                                     .ReputationMinus(From, To),
                                                                  ParseMode.Html,
                                                                  cancellationToken: cancellationToken,
                                                                  replyToMessageId: originMessage.MessageId);
                } catch(ApiRequestException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
		}
	}

	public class MyReputation : CommandBase
	{
		private readonly MysqlDB db;
		public MyReputation(MysqlDB db) { this.db = db; }

		public override async Task HandleAsync(
			IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
		{
			var originMessage = context.Update.Message;
	if (!db.ChatParams.First(x => x.Chat == originMessage.Chat.Id).Settings.ReputationEnabled) return;
	var chat = originMessage.Chat;
			var from = originMessage.From;
			var user = db.UserParams.First(x => x.Chat == chat.Id && x.User == from.Id);
			var Rep = new UserWithRep {Rep = user.Reputation, User = from};
			await context.Bot.Client.SendTextMessageAsync(chat, ReputationLang.MyReputation(Rep),
																		 ParseMode.Html, replyToMessageId: originMessage.MessageId,
																		 cancellationToken: cancellationToken);
		}
	}

	public static class ReputationLang
	{
		private static string RepMsg(string str, UserWithRep UserFrom, UserWithRep UserTo)
		{
			var writer = new StringWriter();
			writer.Write(UserFrom.User.Mention());
			writer.Write(str.ToHTMLTagCode());
			writer.Write(UserTo.User.Mention());
			writer.Write($"({UserTo.Rep})".ToHTMLTagCode());
			return writer.ToString();
		}

		public static string ReputationPlus(UserWithRep UserFrom, UserWithRep UserTo)
		{
			var str = $"({UserFrom.Rep}) увеличил репутацию ";
			return RepMsg(str, UserFrom, UserTo);
		}

		public static string ReputationMinus(UserWithRep UserFrom, UserWithRep UserTo)
		{
			var str = $"({UserFrom.Rep}) уменьшил репутацию ";
			return RepMsg(str, UserFrom, UserTo);
		}

		public static string MyReputation(UserWithRep from)
		{
			var str = $", Ваша репутация: {from.Rep}";
			var writer = new StringWriter();
			writer.Write(from.User.Mention());
			writer.Write(str.ToHTMLTagCode());
			return writer.ToString();
		}
	}
}
