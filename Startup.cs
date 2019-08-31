using System;
using BotWithAPI.Data;
using BotWithAPI.Extensions;
using BotWithAPI.Handlers;
using BotWithAPI.Handlers.AntiFlood;
using BotWithAPI.Handlers.Commands;
using BotWithAPI.Handlers.Commands.AdminCommands;
using BotWithAPI.Options;
using BotWithAPI.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;

namespace BotWithAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration) { Configuration = configuration; }

		private IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
		#region MySQL Context

			services.AddMemoryCache();
			services.AddEntityFrameworkMySql().AddEntityFrameworkProxies();
			services
			  .AddDbContext<MysqlDB>(options =>
					{
						var MysqlVer = new Version(5, 5, 60);
						var mysqlString = Configuration.GetConnectionString("DefaultConnection");
						options.UseLazyLoadingProxies();

						options.UseMySql(mysqlString,
											  mysqlOptions =>
												  {
													  mysqlOptions.ServerVersion(MysqlVer, ServerType.MariaDb);
													  // replace with your Server Version and Type
													  mysqlOptions.UnicodeCharSet(CharSet.Utf8mb4);
												  });
					});

		#endregion

		#region BotServices

			services.AddTransient<EchoBot>()
					  .Configure<BotOptions<EchoBot>>(Configuration.GetSection("EchoBot"))
					  .Configure<CustomBotOptions<EchoBot>>(Configuration.GetSection("EchoBot"))
					  .AddScoped<ChatsHandler>() //отслеживание добавления в чаты и кеш параметров
					  .AddScoped<WebhookLogger>()
					  .AddScoped<StickerHandler>() //антифлуд стикерами
					  .AddScoped<ExceptionHandler>()
					  .AddScoped<UpdateMembersList>() //приветствие/прощание с пользователями при входе/выходе
					  .AddScoped<CallbackQueryHandler>()
					  .AddScoped<RulesPrint>()
					  .AddScoped<RulesSet>()
					  .AddScoped<ReputationPlus>()
					  .AddScoped<UsersHandler>() //создание пользователей в базе 
					  .AddScoped<ReputationMinus>()
					  .AddScoped<MyReputation>()
					  .AddScoped<WarnCommand>()
					  .AddScoped<UnWarnCommand>()
					  .AddScoped<BanCommand>()
					  .AddScoped<BayanCommand>()
					  .AddScoped<BayanSetCommand>()
					  .AddScoped<PerUserAntiFlood>() //отслеживание флуда командами
					  .AddSingleton<RestrictionCache>()
				;

		#endregion

			services.AddMvc();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			//if(env.IsDevelopment())
			//    {
			//        app.UseDeveloperExceptionPage();

			//        // get bot updates from Telegram via long-polling approach during development
			//        // this will disable Telegram webhooks
			//        app.UseTelegramBotLongPolling<EchoBot>(ConfigureBot(),
			//                                               TimeSpan.FromSeconds(2));
			//    }
			//else
			//    {
			//        // use Telegram bot webhook middleware in higher environments
			//        app.UseTelegramBotWebhook<EchoBot>(ConfigureBot());
			//        // and make sure webhook is enabled
			//        app.EnsureWebhookSet<EchoBot>();
			//    }
			app.UseTelegramBotLongPolling<EchoBot>(ConfigureBot(), TimeSpan.FromSeconds(2));
			app.UseMvcWithDefaultRoute();
		}

		private IBotBuilder ConfigureBot()
		{
			IBotBuilder PublicBranch(IBotBuilder groups)
			{
				return groups.UseWhen<UpdateMembersList>(When.MembersChanged)
								 .UseWhen(When.NewMessage,
											 msgBranch => msgBranch
															 .Use<ChatsHandler>()
															 .Use<UsersHandler>()
															 .UseWhen<StickerHandler>(When.StickerMessage)
															 .UseWhen(When.NewTextMessage,
																		 txtBranch => TextBranchInChats(txtBranch)))
								 .UseWhen<CallbackQueryHandler>(When.CallbackQuery);
			}

			IBotBuilder TextBranchInChats(IBotBuilder botBuilder)
			{
				return botBuilder
						.UseWhen(BotCommands.IsCustomCommand,
									customCommands => customCommands
														  .Use<PerUserAntiFlood>()
														  .UseWhen<ReputationPlus>(BotCommands.ReputationPlus)
														  .UseWhen<ReputationMinus>(BotCommands.ReputationMinus)
														  .UseWhen<BayanCommand>(BotCommands.Bayan))
						.UseWhen(When.NewCommand,
									cmdBranch => cmdBranch
													.UseCommand<RulesSet>("setrules")
													.UseCommand<WarnCommand>("warn")
													.UseCommand<UnWarnCommand>("unwarn")
													.UseCommand<BanCommand>("ban")
													.UseCommand<BayanSetCommand>("setbayan")
													.UseWhen(BotCommands.IsFloodableCommand,
																floodable => floodable
																				.Use<PerUserAntiFlood>()
																				.UseCommand<RulesPrint>("rules")
																				.UseCommand<MyReputation>("myrep")));
			}



			return new BotBuilder()
					.Use<ExceptionHandler>()
					.UseWhen(When.OnGroup, groups =>
									PublicBranch(groups))
					;
		}
	}
}
