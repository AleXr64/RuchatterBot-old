using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotWithAPI.Data;
using BotWithAPI.Data.Model;
using Telegram.Bot.Framework.Abstractions;

namespace BotWithAPI.Handlers
{
	public class UsersHandler : IUpdateHandler
	{
		private readonly MysqlDB db;
		public UsersHandler(MysqlDB db) { this.db = db; }

		public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
		{
			var user = context.Update.Message.From;
			var chat = context.Update.Message.Chat;
			if(!db.UserParams.Any(x => x.User == user.Id && x.Chat == chat.Id))
				{
					var adduser = new UserParams(user.Id, chat.Id);
					db.UserParams.Add(adduser);
					_ = db.SaveChangesAsync(cancellationToken);
				}

			await next(context, cancellationToken);
		}
	}
}
