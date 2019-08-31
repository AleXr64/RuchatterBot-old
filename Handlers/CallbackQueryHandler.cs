using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;

namespace BotWithAPI.Handlers
{
	public class CallbackQueryHandler : IUpdateHandler
	{
		public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
		{
			var cq = context.Update.CallbackQuery;

			await context.Bot.Client.AnswerCallbackQueryAsync(cq.Id, "PONG", true, cancellationToken: cancellationToken);

			await next(context, cancellationToken);
		}
	}
}
