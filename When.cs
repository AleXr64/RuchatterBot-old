using System.Linq;
using Microsoft.AspNetCore.Http;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;

namespace BotWithAPI
{
	public static class When
	{
		public static bool OnGroup(IUpdateContext context)
		{
			bool checkMessage()
			{
				return context.Update.Message?.Chat?.Type == ChatType.Group ||
						 context.Update.Message?.Chat?.Type == ChatType.Supergroup;
			}

			bool checkCallback()
			{
				return context.Update.CallbackQuery?.Message?.Chat?.Type == ChatType.Group ||
						 context.Update.CallbackQuery?.Message?.Chat?.Type == ChatType.Supergroup;
			}

			return checkMessage() || checkCallback();
		}

		public static bool OnPrivate(IUpdateContext context)
		{
			bool checkMessage() { return context.Update.Message?.Chat?.Type == ChatType.Private; }

			bool checkCallback() { return context.Update.CallbackQuery?.Message?.Chat?.Type == ChatType.Private; }

			return checkMessage() || checkCallback();
		}

		public static bool Webhook(IUpdateContext context) { return context.Items.ContainsKey(nameof(HttpContext)); }

		public static bool NewMessage(IUpdateContext context) { return context.Update.Message != null; }

		public static bool NewTextMessage(IUpdateContext context) { return context.Update.Message?.Text != null; }

		public static bool NewCommand(IUpdateContext context)
		{
			return context.Update.Message?.Entities?.FirstOrDefault()?.Type == MessageEntityType.BotCommand;
		}

		public static bool MembersChanged(IUpdateContext context)
		{
			return context.Update.Message?.NewChatMembers != null ||
					 context.Update.Message?.LeftChatMember != null ||
					 context.Update.ChannelPost?.NewChatMembers != null ||
					 context.Update.ChannelPost?.LeftChatMember != null;
		}

		public static bool LocationMessage(IUpdateContext context) { return context.Update.Message?.Location != null; }

		public static bool StickerMessage(IUpdateContext context) { return context.Update.Message?.Sticker != null; }

		public static bool CallbackQuery(IUpdateContext context) { return context.Update.CallbackQuery != null; }
	}
}
