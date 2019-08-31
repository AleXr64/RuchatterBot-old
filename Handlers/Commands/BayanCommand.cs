using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Data;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Framework.Abstractions;

namespace BotWithAPI.Handlers.Commands
{
	public class BayanCommand : IUpdateHandler
	{
		private readonly MysqlDB db;
		public BayanCommand(MysqlDB db) { this.db = db; }

		public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
		{
			var originMessage = context.Update.Message;
			if(originMessage.ReplyToMessage == null) return;
			var chat = context.Update.Message.Chat;
			var dbChat = db.ChatParams.First(x => x.Chat == chat.Id);
			if(dbChat.Settings.BayanAnimation != null)
				{
					var url = dbChat.Settings.BayanAnimation;
                    try
                        {
                            await context.Bot.Client.DeleteMessageAsync(originMessage.Chat, originMessage.MessageId,
                                                                        cancellationToken);
                        } catch(Exception e)
                        {
                            Console.WriteLine(e);
                           
                        }

                    try
                        {
                            await context.Bot.Client.SendAnimationAsync(
                                chat, url, replyToMessageId: originMessage.ReplyToMessage.MessageId,
                                cancellationToken: cancellationToken);
                        } catch(ApiRequestException e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
				}
		}
	}
}
