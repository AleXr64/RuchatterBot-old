using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Data;
using BotWithAPI.Extensions;
using BotWithAPI.Util;
using Telegram.Bot.Framework.Abstractions;

namespace BotWithAPI.Handlers
{
	internal class StickerHandler : IUpdateHandler
	{
		private readonly MysqlDB db;
		private readonly RestrictionCache restrictions;

		public StickerHandler(MysqlDB db, RestrictionCache restrictions)
		{
			this.db = db;
			this.restrictions = restrictions;
		}

		public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
		{
			var msg = context.Update.Message;
			var senderAdmin = msg.From.IsAdminAsync(msg.Chat, context.Bot);

			var now = DateTime.Now;
			var settings = db.ChatParams.First(x => x.Chat == msg.Chat.Id).Settings;
			if(!settings.StickerFloodControl) return;
			var bantime = TimeSpan.FromSeconds(settings.StickerFloodDelay);
			restrictions.StickerFloodRestrictions.RemoveAll(x => x.EndTime < now);
			if(restrictions.StickerFloodRestrictions.Any(x => x.Chat == msg.Chat.Id))
				{
					if(settings.StickerFloodIgnoreAdmins && await senderAdmin) return;
					await context.Bot.Client.DeleteMessageAsync(msg.Chat, msg.MessageId, cancellationToken);
				}
			else
				{
					var restrict = new StickerFloodRestriction(msg.Chat.Id, now + bantime);
					restrictions.StickerFloodRestrictions.Add(restrict);
				}
		}
	}
}
