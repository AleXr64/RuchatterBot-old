using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Data;
using BotWithAPI.Data.Model;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotWithAPI.Handlers
{
	public class ChatsHandler : IUpdateHandler
	{
		private readonly MysqlDB db;

		public ChatsHandler(MysqlDB db) { this.db = db; }

		public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
		{
			if(!db.BotSettings.Any())
				{
					var settings = new BotSettings();
					db.BotSettings.Add(settings);
				}
			else
				{
					var settings = db.BotSettings.First();
					if(settings.LastDdCheck != null && settings.LastDdCheck.Value.Date != DateTime.Today)
						await CleanDb(context);
				}

			var chat = context.Update?.Message?.Chat;
			if(chat != null && chat.Type != ChatType.Private)
				{
					if(!db.ChatParams.Any(x => x.Chat == chat.Id))
						{
							StoreChat(chat);
						}
					else
						{
							var cached = db.ChatParams.First(x => x.Chat == chat.Id);
							//				CleanDB(context.Bot);
							//
							//HACK
							//Если Setting не загружены - принудительно обновить инфу
							//settings нет в базе
							try
								{
									if(cached?.LastUpDateTime?.Date != DateTime.Today || cached.Settings == null)
										await RefreshChatInfo(context, cancellationToken, chat);
								} catch(Exception)
								{
									await RefreshChatInfo(context, cancellationToken, chat);
								}

							//
							//HACK
							//
						}
				}

			await next(context, cancellationToken);
		}

		private async Task CleanDb(IUpdateContext context)
		{
			var settings = db.BotSettings.First();
			settings.LastDdCheck = DateTime.Now;
			db.Update(settings);
			db.SaveChanges();
			var chats = db.ChatParams.ToList();

			foreach(var chat_test in chats)
				{
					ChatMember[] test;
					try
						{
							test = await context.Bot.Client.GetChatAdministratorsAsync(chat_test.Chat);
						} catch(ApiRequestException)
						{
							chat_test.IsValid = false;
							db.Update(chat_test);
							db.SaveChanges();
							test = null;
						}

					if(test?.Length > 0)
						{
							chat_test.IsValid = true;
							db.Update(chat_test);
							db.SaveChanges();
						}
				}
		}

		private async Task RefreshChatInfo(IUpdateContext context, CancellationToken cancellationToken, Chat chat)
		{
			var info = await context.Bot.Client.GetChatAsync(chat, cancellationToken);
			var admins = await context.Bot.Client.GetChatAdministratorsAsync(chat, cancellationToken);
			var adminParams = new List<UserParams>();
			foreach(var admin in admins)
				{
					UserParams tempAdmin;
					try
						{
							tempAdmin = db.UserParams.First(x => x.Chat == chat.Id && x.User == admin.User.Id);
						} catch(InvalidOperationException)
						{
							tempAdmin = new UserParams(admin.User.Id, chat.Id);
							db.UserParams.Add(tempAdmin);
						}

					if(tempAdmin != null) adminParams.Add(tempAdmin);
				}

			var updatechat = db.ChatParams.First(x => x.Chat == chat.Id);
			updatechat.Description = info.Description;
			updatechat.Title = info.Title;
			updatechat.Username = info.Username;
			updatechat.LastUpDateTime = DateTime.Now;
			updatechat.Administrators = adminParams;
			//
			//HACK
			//
			try
				{
					if(updatechat.Settings == null) updatechat.Settings = new ChatSettings();
				} catch(Exception)
				{
					updatechat.Settings = new ChatSettings();
				}

			//
			//HACK
			//
			db.Update(updatechat);
			db.SaveChanges();
		}

		private void StoreChat(Chat chat)
		{
			var param = new ChatParams(chat.Id);
			db.ChatParams.Add(param);
			db.SaveChanges();
		}
	}
}
