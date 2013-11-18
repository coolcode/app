using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EaseEasy.Data.Entity;
using EaseEasy.ServiceModel.Services;
using Linkknil.Entities;
using Linkknil.Models;
using Linkknil.Services;

namespace Linkknil.Jobs {
    public class CrawlJob : ServiceBase<LinkknilContext> {

        public void DigLinks() {
            //查找未进行Dig的链接
            var links = GetUnhandledLinks(10, DateTime.Now.AddMinutes(30));//半小时

            //遍历每个链接
            Parallel.ForEach(links, link => {
                                            var linkService = new LinkService();
                                            linkService.DigLink(link);
                                        });

        }

        private IEnumerable<LinkItem> GetUnhandledLinks(int count, DateTime handleTime) {
            return
                db.Query<LinkItem>(
                   string.Format(@"select top {0} l.* from Lnk_Link l
where l.Status = 1 --启用
and ISNULL(l.HandleTime,'1999-9-9') < @HandleTime -- 上次处理时间区间未超出
and exists(select null from PF_App a where a.Id = l.AppId and a.Status = @Status) --App状态是启用
order by ISNULL(l.HandleTime,'1999-9-9')", count),
                    new { HandleTime = handleTime, Status = (int)AppStatus.Publish });
        }

        public void PushLinks() {
            //查找Push表未处理的url
            var count = 10;
            var pushItems = db.Query<PushItem>(string.Format(@"select top {0} p.* from lnk_push p where Status = 0 order by CreateTime", count));

            Parallel.ForEach(pushItems, link => {
                var linkService = new LinkService();
                linkService.PushLink(link);
            });
        }
    }
}
