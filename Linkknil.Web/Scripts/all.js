function initLayout() {
    var window_width = $(window).width();
    wrapper_width = Math.floor(window_width / 236) * 236;
    if (window_width > 960) {
        $('.wrapper').css("width", wrapper_width + "px");
    } else {
        $('.wrapper').css("width", "944px");
    }
}

function resort() {
    $('#masonry').masonry({
        itemSelector: '.item',
        isResizable: false
    });
}

$(function () {
    initLayout();
    resort();
});

$(window).resize(function () {
    initLayout();
    resort();
});

var __lastIndex__ = "!@#";
$(window).scroll(function () {
    var o = $('#masonry');

    // 并不是所有页面都要执行加载操作。
    // 你也可以选择别的识别条件。
    if (o != null && o.length != 0) {

        //获取网页的完整高度(fix)
        var hght = window.scrollHeight;

        var srollPos = $(window).scrollTop();    //滚动条距顶部距离(页面超出窗口的高度)
        var dbHiht = $("body").height();          //页面(约等于窗体)高度/单位px
        var mainHiht = o.height();               //主体元素高度/单位px

        if (($(document).height() - $(window).height() - srollPos) > 50) {
            return;
        }

        //避免重复提交
        if (__lastIndex__ == o.attr("index")) {
            return;
        }

        __lastIndex__ = o.attr("index");
        show();

        return;

    }
});

function show() {
    var target = $('#masonry');

    if (!target) { return false; }

    if (target.attr("nomore")==1) {
        return false;
    }

    var data = {};
    data.next = target.attr("index");


    var currentHref = window.location;

    // 有的时候可能还要从url中读取参数，我使用正则式
    /*
    var tcaStr = new RegExp("(^|)tca=([^\&]*)(\&|$)", "gi").exec(currentHref);
    if (tcaStr) data.tca = tcaStr[2];

    var scaStr = new RegExp("(^|)sca=([^\&]*)(\&|$)", "gi").exec(currentHref);
    if (scaStr) data.sca = scaStr[2];

    var tagStr = new RegExp("(^|)tag=([^\&]*)(\&|$)", "gi").exec(currentHref);
    if (tagStr) data.tag = tagStr[2];
    */
    // ajax请求数据
    jQuery.ajax({
        type: "POST",
        url: "/Archive/Lazyload",
        data: data,
        dataType: "json",
        cache:false,
        beforeSend: function (XMLHttpRequest) {
            $("#loading").css('display', '');
        },
        success: function (r) {
        var data = r.Items;
        var html="";
            for (var i = 0, length = data.length; i < length; i++) {
                var item = data[i];
                html += "<div class='item'>" +
                        "<h2 class='title'>"+
                           "<a href='/p/"+ item.Id + "' target='_blank'>" + item.Title+ "</a>" + 
                        "</h2>"+
                        "<div class='comments'>"+
                            item.Summary+
                        "</div>" +
                        "<div class='chat'>"+
                            "<div class='chatbox'>" +
                            "<img src='/Image/AppIcon/"+ item.IconPath+"' width='50px' height='50px' alt='' />"+
                            "<b>"+item.Author + "</b><p>发布于"+ item.PublishTimeText + "</p>"+
                            "</div>"+
                        "</div>"+
                    "</div>";
            }

            
            var box = $(html);
            target.append(box).masonry('appended',box,true);

            target.attr('index', r.LastIndex);
            target.attr('nomore', r.Count==0?1:0);

            $("#loading").css('display', 'none');
        },
        error: function () {
           // alert("加载失败");
        }
    });

    return false;
}
