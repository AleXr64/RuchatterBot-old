using System;
using System.Collections.Generic;
using BotWithAPI.Data.Model;

namespace BotWithAPI.Util
{
	public class RestrictionCache
	{
		public RestrictionCache()
		{
			CommandFloodRestrictions = new List<FloodRestrictions>();
			StickerFloodRestrictions = new List<StickerFloodRestriction>();
		}

		public List<FloodRestrictions> CommandFloodRestrictions { get; }
		public List<StickerFloodRestriction> StickerFloodRestrictions { get; }
	}

	public class FloodRestrictions
	{
		public FloodRestrictions(DateTime endTime, UserParams user)
		{
			EndTime = endTime;
			User = user;
			IsSecondTry = false;
			IsInformed = IsSecondTry;
		}

		public UserParams User { get; }
		public DateTime EndTime { get; }
		public bool IsInformed { get; set; }
		public bool IsSecondTry { get; set; }
	}

	public class StickerFloodRestriction
	{
		public StickerFloodRestriction(long chat, DateTime endTime)
		{
			Chat = chat;
			EndTime = endTime;
		}

		public long Chat { get; }
		public DateTime EndTime { get; }
	}
}
