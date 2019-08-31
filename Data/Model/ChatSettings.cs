using Telegram.Bot.Types;

namespace BotWithAPI.Data.Model
{
	public class ChatSettings
	{
		public ChatSettings()
		{
			StickerFloodControl = true;
			ReputationEnabled = true;
			StickerFloodIgnoreAdmins = true;
			StickerFloodDelay = 30;
		}

		public int Id { get; set; }
		public bool StickerFloodControl { get; set; }
		public bool StickerFloodIgnoreAdmins { get; set; }
		public int StickerFloodDelay { get; set; }
		public bool ReputationEnabled { get; set; }
		public string  BayanAnimation { get; set; }
	}
}
