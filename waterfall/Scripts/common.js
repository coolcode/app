function cb_CodeHighlight() {
    SyntaxHighlighter.config.strings.viewSource = "view my source!!!!";
    SyntaxHighlighter.highlight();

    psComments();
}

/*美化评论*/
function psComments() {
    var w1 = '<div class="list">' +
        '<table class="out" border="0" cellspacing="0" cellpadding="0"> ' +
			'<tr>' +
				'<td class="icontd" align="left" valign="bottom">' +
                    '<img src="http://pic.cnblogs.com/avatar/simple_avatar.gif" width="39px" height="39px"/>'
    '</td>' +
				'<td align="left" valign="bottom" class="q">' +
					'<table border="0" cellpadding="0" cellspacing="0" style=""> ' +
						'<tr><td class="topleft"></td><td class="top"></td><td class="topright"></td></tr> ' +
						'<tr><td class="left"></td> <td align="left" class="conmts"><p>';


    var w2 = '</p> </td> <td class="right"></td></tr> ' +
						'<tr><td class="bottomleft"></td><td class="bottom"></td><td class="bottomright"></td></tr> ' +
					'</table>' +
				'</td> ' +
			'</tr> ' +
		'</table> ' +
    '</div>';


    var louzhu1 = '<div class="list">' +
        '<table class="inc" border="0" cellspacing="0" cellpadding="0"> ' +
			'<tr>' +
				'<td align="right" valign="bottom" class="q">' +
					'<table border="0" cellpadding="0" cellspacing="0" style=""> ' +
						'<tr><td class="topleft"></td><td class="top"></td><td class="topright"></td></tr> ' +
						'<tr><td class="left"></td> <td align="left" class="conmts"><p>';


    var louzhu2 = '</p> </td> <td class="right"></td></tr> ' +
						'<tr><td class="bottomleft"></td><td class="bottom"></td><td class="bottomright"></td></tr> ' +
					'</table>' +
				'</td> ' +
				'<td class="icontd" align="left" valign="bottom">' +
                    '<img src="http://pic.cnblogs.com/avatar/simple_avatar.gif" width="39px" height="39px"/>' +
				'</td>' +
			'</tr> ' +
		'</table> ' +
    '</div>';

    $.each($(".feedbackItem"), function (i, feedbackItem) {
        var fbi = $(feedbackItem);
        var lz = fbi.find(".louzhu");

        var avatar = "";
        $.each(fbi.find("a"), function (i, link) {
            var href = $(link).attr("href");
            if (/\/u\/(\d+)/.test(href)) {
                avatar = p + "a" + RegExp.$1 + ".jpg";
            }
        });

        var comment_body = fbi.find(".blog_comment_body");
        var html = "";

        if (lz.length > 0) {
            html = w1 + comment_body.html() + w2;
        } else {
            html = louzhu1 + comment_body.html() + louzhu2;
        }

        if (avatar) {
            html.replace("http://pic.cnblogs.com/avatar/simple_avatar.gif", avatar)
        }

        comment_body.html(html);

    });
}

function psComments() {
    var w1 = '<div class="list">' +
        '<table class="out" border="0" cellspacing="0" cellpadding="0"> ' +
			'<tr>' +
				'<td align="left" valign="bottom" class="q">' +
					'<table border="0" cellpadding="0" cellspacing="0" style=""> ' +
						'<tr><td class="topleft"></td><td class="top"></td><td class="topright"></td></tr> ' +
						'<tr><td class="left"></td> <td align="left" class="conmts"><p>';


    var w2 = '</p> </td> <td class="right"></td></tr> ' +
						'<tr><td class="bottomleft"></td><td class="bottom"></td><td class="bottomright"></td></tr> ' +
					'</table>' +
				'</td> ' +
				'<td class="icontd" align="left" valign="bottom">' +
                    '<img src="http://pic.cnblogs.com/avatar/simple_avatar.gif" width="39px" height="39px"/>'
                '</td>' +
			'</tr> ' +
		'</table> ' +
    '</div>';

    $.each($(".blog_comment_body"), function (i, t) {        
        $(t).html(w1 + $(t).html() + w2);
    });

    $(".louzhu").parent().find('.out')
                .removeClass("out").addClass("inc")
		.find(".q").attr("align", "right");

    /* 20120927 Freewind 同学提供的获取头像技巧*/
    var p = "http://pic.cnblogs.com/avatar/";
    var c = document.getElementById("blog-comments-placeholder");
    var d = c.childNodes; 
    for (i = 0; i < d.length; i++) {
        if (d[i].className == "feedbackItem") {
            var t = d[i].getElementsByTagName("table")[0];
            var t0 = t.rows[0];
            t0.cells[1].style.display = "block";
            if (/\/u\/(\d+)/.test(d[i].innerHTML)) {
                img = t0.cells[1].getElementsByTagName("img")[0];
                img.src = p + "a" + RegExp.$1 + ".jpg";
                img.onerror = function () {
                    this.src = p + 'simple_avatar.gif';
                };
            }

            if (t.className == "out") {
                t0.appendChild(t0.cells[0]);
            }
        }
    };

}

$(document).ready(function () {

   
}); 