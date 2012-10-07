using System;
using System.Collections.Generic;
using System.Linq;
using CoolCode;
using CoolCode.Data.Entity;
using CoolCode.ServiceModel.Services;
using Linkknil.Entities;
using Linkknil.Models;
using NReadability;

namespace Linkknil.Services {
    public class LinkService : ServiceBase {
        protected new LinkknilContext db = new LinkknilContext();//{ get { return (LinkknilContext)base.db; } }
        private PullService pullService = new PullService();

        public LinkService() {
            LinkknilContext.SetInitializer();
            var pf = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            db.AppCategories.Add(new AppCategory { Id = pf + "1", Name = "Bruce" });
            //db.AppCategories.Add(new AppCategory { Id = pf + "2", Name = "Hohn" });
            //db.AppCategories.Add(new AppCategory { Id = pf + "3", Name = "Rose" });
            db.SaveChanges();
        }

        public void DigLink() {
            //查找未进行Dig的链接
            var links = GetUnhandledLinks(10, DateTime.Now.AddMinutes(1));//半小时

            //遍历每个链接
            foreach (var link in links) {
                var dig = new DigLink {
                    Id = Guid.NewGuid().ToString(),
                    LinkId = link.Id,
                    BeginTime = DateTime.Now,
                    Status = (int)LinkStatus.Diging,
                    DuplicateNum = 0,
                    DigNum = 0,
                    FailNum = 0
                };
                try {
                    //链接状态设置成处理中
                    UpdateLinkStatusToHandle(link.Id);

                    //读取链接里匹配的内容源url
                    var digLinks = pullService.PullLink(link.Url, link.XPath);

                    foreach (var digLink in digLinks) {
                        dig.DigNum++;
                        //从网络抓取内容放入Content表
                        var pullStatus = PullStatus.Fail;
                        try {
                            pullStatus = PullContent(link, digLink);
                        }
                        catch (Exception ex) {
                            Logger.Error("抓取Html内容到数据表发生错误！url:" + digLink.Url, ex);
                        }

                        switch (pullStatus) {
                            case PullStatus.Fail:
                                dig.FailNum++;
                                break;
                            case PullStatus.Duplicate:
                                dig.DuplicateNum++;
                                break;
                        }

                        if (pullStatus == PullStatus.Success) {
                            //内容源url、title根据PushTarget确认目标放入Push表
                            SaveLinkToPush(digLink);
                        }
                    }
                }
                catch (Exception ex) {
                    //Log错误
                    LogError(link, ex);
                }
                finally {
                    //设置链接状态处理完成
                    dig.EndTime = DateTime.Now;
                    dig.TimeSpan = (int)(dig.EndTime - dig.BeginTime).Value.TotalSeconds;
                    dig.Status = (int)LinkStatus.Enabled;
                    UpdateLinkStatusToDone(link.Id, dig);
                }
            }
        }

        private IEnumerable<LinkItem> GetUnhandledLinks(int count, DateTime handleTime) {
            return
                db.Query<LinkItem>(
                   string.Format(@"select top {0} l.* from Lnk_Link l
where l.Status = 1 --启用
and ISNULL(l.HandleTime,'1999-9-9') < @HandleTime -- 上次处理时间区间未超出
and exists(select null from PF_App a where a.Id = l.AppId and a.Status = 1) --App状态是启用
order by ISNULL(l.HandleTime,'1999-9-9')", count),
                    new { HandleTime = handleTime });
        }

        private void UpdateLinkStatusToHandle(string linkId) {
            db.Execute(@"update Lnk_Link set Status = 50 where Id = @Id", new { Id = linkId });
        }

        private PullStatus PullContent(LinkItem link, Link digLink) {
            DateTime beginTime = DateTime.Now;
            //判断是否已经抓取过
            var isPull = db.Exists(@"select top 1 1 from Lnk_Content c where c.Url = @Url", new { Url = digLink.Url });
            if (isPull) {
                return PullStatus.Duplicate;
            }
            try {
                //读取内容
                var html = pullService.PullHtml(digLink.Url);

                var nReadabilityTranscoder = new NReadabilityWebTranscoder();
                var transResult = nReadabilityTranscoder.Transcode(new WebTranscodingInput(digLink.Url));

                //保存内容
                var content = new Content {
                    Id = Guid.NewGuid().ToString(),
                    LinkId = link.Id,
                    AppId = link.AppId,
                    DeveloperId = db.Get<string>(@"select top 1 DeveloperId from PF_App a where a.Id = @AppId ", new { link.AppId }),
                    BeginTime = beginTime,
                    EndTime = DateTime.Now,
                    TimeSpan = (int)(DateTime.Now - beginTime).TotalSeconds,
                    Url = digLink.Url,
                    Title = html.GetHtmlTitle().Cut(100),
                    Text = html.HtmlToText(),
                    Html = html,
                    Tag = "",
                    Response = "",
                    FriendlyTitle = transResult.ExtractedTitle.Cut(100),
                    FriendlyHtml = transResult.ExtractedContent
                };
                db.Contents.Add(content);
                db.SaveChanges();
            }
            catch (Exception ex) {
                LogError(link, ex);
                return PullStatus.Fail;
            }

            return PullStatus.Success;
        }

        private void SaveLinkToPush(Link digLink) {
            var isPush = db.Exists(@"select top 1 1 from Lnk_Push where Url = @Url", new{digLink.Url});

            if (isPush) {
                return;
            }

            var targets = db.PushTargets.Select(c => c.Id).ToList();
            foreach (var pushTarget in targets) {
                var pushItem = new PushItem {
                    Id = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Target = pushTarget,
                    Url = digLink.Url,
                    Title = digLink.Title,
                    Status = (int)PullStatus.None
                };
                db.PushItems.Add(pushItem);
            }
            db.SaveChanges();
        }

        private void LogError(LinkItem link, Exception ex) {
            Logger.Error(string.Format("Link(url:{0},xpath:{1},Id:{2}) 发生异常！", link.Url, link.XPath, link.Id), ex);
        }

        private void UpdateLinkStatusToDone(string linkId, DigLink digLink) {
            db.Execute(@"update Lnk_Link set Status = 1, HandleTime = @HandleTime where Id = @Id", new { Id = linkId, HandleTime = DateTime.Now });
            db.Digs.Add(digLink);
            db.SaveChanges();
        }

        public void PushLink() {
            //查找Push表未处理的url
            var count = 10;
            var pushItems = db.Query<PushItem>(string.Format(@"select top {0} p.* from lnk_push p where Status = 0 order by CreateTime", count));

            foreach (var pushItem in pushItems) {
                var beginTime = DateTime.Now;

                var pushTarget = db.Get<PushTarget>("select top 1 t.* from Lnk_PushTarget t where Id = @Id",
                                                    new { Id = pushItem.Target });

                var response = string.Empty;
                var status = PullStatus.None;

                if (pushTarget == null) {
                    response = string.Format("无法找到对应的推送目标(Target:{0})", pushItem.Target);
                    status = PullStatus.Fail;
                }
                else {
                    //推送数据
                    switch (pushItem.Target) {
                        case "BF58D5E6-9ED6-48F9-BB4D-5F52426DD620": //Readability
                            try {
                                var readabilityService = new ReadabilityService(pushTarget.Account, pushTarget.Password);
                                readabilityService.Bookmark(pushItem.Url);
                                status = PullStatus.Success;
                            }
                            catch (Exception ex) {
                                status = PullStatus.Fail;
                                if (ex.Message.Contains("(404)")) {
                                    response += "（404）找不到网页" + pushItem.Url;
                                }
                                else if (ex.Message.Contains("(409)")) {
                                    response += "（409）重复推送" + pushItem.Url;
                                    status = PullStatus.Duplicate;
                                }
                                else {
                                    response += "未知错误！";
                                }
                                response += " " + ex.Message + " StakTrace:" + ex.StackTrace;
                            }
                            break;
                    }
                }
                //更新推送状态
                db.Execute(@"update lnk_push set Response = @Response, Status = @Status, PushTime = @PushTime, TimeSpan = @TimeSpan where Id = @Id ", new { Id = pushItem.Id, Response = response, Status = (int)status, PushTime = DateTime.Now, TimeSpan = (DateTime.Now - beginTime).TotalSeconds });
            }
        }


    }

}
