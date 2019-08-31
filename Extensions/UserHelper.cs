using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotWithAPI.Extensions
{
	public static class UserHelper
	{
		public static string UserFullName(this User user)
		{
			var result = new StringBuilder();
			if(!string.IsNullOrEmpty(user.FirstName)) result.Append(user.FirstName);
			if(!string.IsNullOrEmpty(user.LastName))
				{
					result.Append(' ');
					result.Append(user.LastName);
				}

			return result.ToString();
		}

		public static async Task<bool> IsAdminAsync(this User user, Chat chat, IBot bot)
		{
			var usr = await bot.Client.GetChatMemberAsync(chat, user.Id);
			return usr.Status == ChatMemberStatus.Administrator || usr.Status == ChatMemberStatus.Creator;
		}

		public static string Mention(this User user)
		{
			var writer = new StringWriter();
			var link = new TagBuilder("a");
			link.Attributes.Add("href", "tg://user?id=" + user.Id);
			link.InnerHtml.Append(user.UserFullName());
			link.WriteTo(writer, HtmlEncoder.Default);
			return writer.ToString();
		}
	}
}
