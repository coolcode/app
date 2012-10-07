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

$(window).scroll(function () {
    var o = $('#masonry');

    // 并不是所有页面都要执行加载操作。
    // 你也可以选择别的识别条件。
    if (o != null && o.length != 0) {

        //获取网页的完整高度(fix)
        var hght = window.scrollHeight;
        //alert(hght);

        var srollPos = $(window).scrollTop();    //滚动条距顶部距离(页面超出窗口的高度)
        var dbHiht = $("body").height();          //页面(约等于窗体)高度/单位px
        var mainHiht = o.height();               //主体元素高度/单位px

        var text = "Top:" + ($(window).scrollTop());
        text += ",Body:" + dbHiht;
        text += ",Main:" + mainHiht;
        text += ",Document:" + $(document).height();
        text += ",Window:" + $(window).height();
        text += ",Diff:" + ($(document).height() - $(window).height() - srollPos);

        $(".displayText").text(text);

        if (($(document).height() - $(window).height() - srollPos) > 50) {
            return;
        }

        show();

        return;
        //获取浏览器高度(fix)
        var clientHeight = window.clientHeight;
        //alert(clientHeight);

        //获取网页滚过的高度(dynamic)
        var top = window.pageYOffset ||
                        (document.compatMode == 'CSS1Compat' ?
                        document.documentElement.scrollTop :
                        document.body.scrollTop);



        //当 top+clientHeight = scrollHeight的时候就说明到底儿了
        if (top >= (parseInt(hght) - clientHeight)) {
            show();
        }

    }
});

//我所要执行的操作是把ajax取得的数据塞到目标div中
function show() {
    //alert(1);
    var target = $('#masonry');

    if (!target) { return false; }

    //一般你都要记录一下你的数据的录入状态，比如到当前显示页码
    var current_page = parseInt(target.attr('index'));

    if(!current_page){
        current_page = 1;
    }

    //还要记录一个最大显示页码，以阻止请求溢出
    var max_page = parseInt(target.attr('maxPages'))|| 100;
    if (current_page >= max_page) {
         alert("超出最大页限制！");
        return false;
    }

    var data = {};
    data.nextPage = parseInt(current_page) + 1;


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
        url: "/Home/Lazyload",
        data: data,
        dataType: "json",
        beforeSend: function (XMLHttpRequest) {
            $("#loading").css('display', '');
        },
        success: function (data) {
        var html="";
            for (var i = 0, length = data.length; i < length; i++) {
                var item = data[i];
                html += "<div class='item'>" +
              "<img height='" + item.height + "px' src='" + item.src + "' width='" + item.width + "px' alt='' />" +
              "<h2 class='lipsum'></h2>" +
              "<div class='comments'>ladsflasdf" +
              "</div>" +
              "<div class='chat'>" +
                "<div class='chatbox'>" +
                  "<img height='30px' src='/image/format/30x30/798dc3/15237a_text=voluptatem' width='30px' alt='' />" +
                  "<b>inn Lloyd</b>" +
                  "via" +
                  "<b>Kerry Beasley</b>" +
                  "onto" +
                "</div>" +
              "</div>" +
            "</div>";

            }

            
            var box = $(html);
            target.append(box).masonry('appended',box,true);
            //$(html).appendTo(target);

           // alert(123);

            target.attr('index', parseInt(current_page) + 1);

            $("#loading").css('display', 'none');
        },
        error: function () {
            alert("加载失败");
        }
    });

    return false;
}
