using BotWithAPI.Data.Model;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace BotWithAPI.Data
{
	public class MysqlDB : DbContext
	{
		public MysqlDB(DbContextOptions<MysqlDB> options)
			: base(options) { }

		public virtual DbSet<BotSettings> BotSettings { get; set; }
		public virtual DbSet<UserParams> UserParams { get; set; }
		public virtual DbSet<ChatSettings> ChatSettings { get; set; }
        public virtual DbSet<ChatParams> ChatParams { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ChatParams>().HasKey(x => x.Id);
			modelBuilder.Entity<UserParams>().HasIndex(x => new {x.User, x.Chat});

		}
	}
}
