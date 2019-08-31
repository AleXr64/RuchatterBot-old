using System;
using System.Collections.Generic;

namespace BotWithAPI.Data.Model
{
	public class ChatParams
	{
		public ChatParams(long c)
		{
			Chat = c;
			Rules = "Установите новые правила, ответив на сообщение /setrules";
			Settings = new ChatSettings();
		}

		public ChatParams() { }

		public string Rules { get; set; }
		public long Chat { get; set; }
		public string Username { get; set; }
		public string Description { get; set; }
		public DateTime? LastUpDateTime { get; set; }
		public string Title { get; set; }
		public virtual List<UserParams> Administrators { get; set; }
		public int Id { get; set; }
		public virtual ChatSettings Settings { get; set; }
		public bool IsValid { get; set; }
	}
}
