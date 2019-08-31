using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Extensions;
using BotWithAPI.Util;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;

namespace BotWithAPI.Handlers
{
	internal class UpdateMembersList : IUpdateHandler
	{
		public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
		{
			var me = await context.Bot.Client.GetMeAsync(cancellationToken);
			var mes = context.Update.Message;

			switch(mes.Type)
				{
					case MessageType.ChatMembersAdded:
						NewUser();
						break;
					case MessageType.ChatMemberLeft:
						if(mes.LeftChatMember == me) return;
						LostUser();
						break;
				}

			async void NewUser()
			{
				await context.Bot.Client.SendTextMessageAsync(mes.Chat, NewUserMsg(), ParseMode.Html,
																			 cancellationToken: cancellationToken,
																			 replyToMessageId: mes.MessageId);
			}

			async void LostUser()
			{
				try
					{
						await context.Bot.Client.SendTextMessageAsync(mes.Chat, LostUserMsg(), ParseMode.Html,
																					 cancellationToken: cancellationToken,
																					 replyToMessageId: mes.MessageId);
					} catch(Exception e)
					{
						Console.WriteLine(e);
						throw;
					}
			}

			string NewUserMsg()
			{
				var writer = new StringBuilder();

				if(mes.NewChatMembers.Length > 1)
					writer.Append("Приветствуем новых участников!".ToHTMLTagCode() + "\n\r");
				else
					writer.Append("Приветствуем нового участника!".ToHTMLTagCode() + "\n\r");
				foreach(var member in mes.NewChatMembers)
					writer.Append(member.Mention());
				writer.Append("\n\r");
				writer.Append("Пожалуйста, ознакомьтесь с правилами.".ToCharArray());
				writer.Append(" /rules");
				return writer.ToString();
			}

			string LostUserMsg()
			{
				var writer = new StringBuilder();
				writer.Append("Нас покидает:\n\r".ToHTMLTagCode());
				writer.Append(mes.LeftChatMember.Mention());
				writer.Append("\n\r");
				writer.Append("Возвращайся скорее!".ToHTMLTagCode());
				return writer.ToString();
			}

			await next(context, cancellationToken);
		}
	}
}
