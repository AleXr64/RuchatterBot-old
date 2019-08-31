using Microsoft.Extensions.Options;
using Telegram.Bot.Framework;

namespace BotWithAPI
{
	public class EchoBot : BotBase
	{
		public EchoBot(IOptions<BotOptions<EchoBot>> options)
			: base(options.Value) { }
	}
}
