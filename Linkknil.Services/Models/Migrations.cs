using System;
using System.Data.Entity.Migrations;

namespace Linkknil.Models {
    //refer:http://msdn.microsoft.com/zh-cn/data/jj591621
    public partial class DemoMigration : DbMigration {
        public override void Up() {
            AddColumn("Blogs", "Url", c => c.String());
        }

        public override void Down() {
            DropColumn("Blogs", "Url");
        }
    }

}
