using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using CoolCode.ServiceModel.Services;
using Linkknil.Entities;

namespace Linkknil.Models {
    public class LinkknilContext : DefaultDbContext {
        public DbSet<AppCategory> AppCategories { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<DailyVisit> DailyVisits { get; set; }

        public DbSet<LinkItem> Links { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<DigLink> Digs { get; set; }
        public DbSet<PushItem> PushItems { get; set; }
        public DbSet<PushTarget> PushTargets { get; set; }


        public static void SetInitializer() {
            Database.SetInitializer(new LinkknilContextInitializer());
        }

        class AutoMigrationsConfiguration : DbMigrationsConfiguration<LinkknilContext> {

            public AutoMigrationsConfiguration() {
                AutomaticMigrationDataLossAllowed = true;
                AutomaticMigrationsEnabled = true;
            }

            protected override void Seed(LinkknilContext context) {
                var dict = new Dictionary<string, Dictionary<string, string>>{
{"PullType",new Dictionary<string, string>{{"rss","RSS"},{"xpath","Html + xPath"}}},
{"ItemStatus",new Dictionary<string, string>{{"1","启用"},{"0","停用"}}},
{"AppStatus",new Dictionary<string, string>{{"0","未审核"},{"1","审核中"},{"100","发布"},{"-2","下线"},{"-1","审核不通过"}}}
			    };

                foreach (var items in dict) {
                    int index = 1;
                    foreach (var item in items.Value) {
                        context.DictionaryItems.AddOrUpdate(c => new{c.Name,c.Value}, new DictionaryItem {
                            Name = items.Key,
                            Value = item.Key,
                            Text = item.Value,
                            SortIndex = index++,
                            Enabled = true,
                            IsInner = true
                        });
                    }
                }

                int i = 1;
                var appCategories = new[] { "新闻", "IT资讯", "娱乐" };
                foreach (var appCategory in appCategories) {
                    context.AppCategories.AddOrUpdate(c => c.Name, new AppCategory {
                        Id = Guid.NewGuid().ToString(),
                        Name = appCategory,
                        Sort = string.Format("{0:d4}", i++),
                        Status = (int)ItemStatus.Enabled
                    });
                }

                base.Seed(context);

            }
        }

        class LinkknilContextInitializer : MigrateDatabaseToLatestVersion<LinkknilContext, AutoMigrationsConfiguration> {
            // DropCreateDatabaseIfModelChanges<LinkknilContext> 
            
        }
    }
}
