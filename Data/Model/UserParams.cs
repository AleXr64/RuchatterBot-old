using System.ComponentModel.DataAnnotations;

namespace BotWithAPI.Data.Model

{
	public class UserParams
	{
		public UserParams() { }

		public UserParams(long u, long c)
		{
			Chat = c;
			User = u;
		}

		[Key]
		public int Id { get; set; }

		public short Warns { get; set; }
		public int Reputation { get; set; }
		public long Chat { get; set; }
		public long User { get; set; }

		

		protected bool Equals(UserParams other) { return Chat == other.Chat && User == other.User; }

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != GetType()) return false;
			return Equals((UserParams) obj);
		}

		public override int GetHashCode()
		{
			unchecked
				{
					return (Chat.GetHashCode() * 397) ^ User.GetHashCode();
				}
		}

		public static bool operator ==(UserParams one, UserParams other)
		{
			if((object) one == null && (object) other == null)
				return true;
			if((object) one == null || (object) other == null) return false;

			return other.Chat == one.Chat && one.User == other.User;
		}

		public static bool operator !=(UserParams one, UserParams other) { return !(one == other); }
	}
}
