using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotWithAPI.Extensions
{
	public static class BotCommands
	{
		private static readonly List<string> FloodControList = new List<string> {"/rules", "/myrep"};

		private static readonly List<string> CustomCommandList = new List<string>
			{
				"+",
				"👍",
				"👍🏼",
				"👍🏽",
				"👍🏾",
				"👍🏿",
				"👍🏻",
				"-",
				"👎🏼",
				"👎🏼",
				"👎🏽",
				"👎🏾",
				"👎🏿",
				"👎🏻",
				"баян"
			};

		public static bool ReputationPlus(IUpdateContext context)
		{
			var txt = context.Update.Message?.Text;
			return txt != null &&
					 (txt == "+" ||
					  txt == "👍🏼" ||
					  txt == "👍" ||
					  txt == "👍🏽" ||
					  txt == "👍🏾" ||
					  txt == "👍🏿" ||
					  txt == "👍🏻");
		}

		public static bool ReputationMinus(IUpdateContext context)
		{
			var txt = context.Update.Message?.Text;
			return txt != null &&
					 (txt == "-" ||
					  txt == "👎🏼" ||
					  txt == "👎🏼" ||
					  txt == "👎🏽" ||
					  txt == "👎🏾" ||
					  txt == "👎🏻" ||
					  txt == "👎🏿");
		}

		public static bool Bayan(IUpdateContext ctx)
		{
			return ctx.Update.Message?.Text != null && ctx.Update.Message?.Text.ToLower() == "баян";
		}

		public static bool IsFloodableCommand(IUpdateContext ctx)
		{
			var text = ctx.Update.Message.Text.ToLower();

			foreach(var floodable in FloodControList)
				if(Regex.IsMatch(text,
									  $@"^{floodable}(?:@{ctx.Bot.Username})?$",
									  RegexOptions.IgnoreCase))
					return true;
			return false;
		}

		public static bool IsCustomCommand(IUpdateContext ctx)
		{
			var text = ctx.Update.Message.Text.ToLower();
			foreach(var knowCommand in CustomCommandList)
				if(text == knowCommand.ToLower())
					return true;

			return false;
		}
	}

	public static class BotHelper
	{
		/// <summary>
		///   Отправляет в текущий чат текстовое сообщение с разметкой в HTML
		/// </summary>
		/// <param name="Text">Текст для отправки</param>
		public static async Task<Message> SendHTMLCodeMsg(this IUpdateContext context, string Text)
		{
			var msg = context.Update.Message;
			return await context.Bot.Client.SendTextMessageAsync(msg.Chat, Text, ParseMode.Html);
		}

		/// <summary>
		///   Отправляет в текущий чат текстовое сообщение без разметки
		/// </summary>
		/// <param name="Text">Текст для отправки</param>
		public static async Task<Message> SendSimpleText(this IUpdateContext context, string Text)
		{
			var msg = context.Update.Message;
			return await context.Bot.Client.SendTextMessageAsync(msg.Chat, Text, ParseMode.Html);
		}
	}
}
