/// <reference path="../jquery/jquery-vsdoc.js" />

$(document).ready(function () {

	//Sidebar Accordion Menu:

	$("#main-nav li ul").hide(); // Hide all sub menus
	$("#main-nav li a.current").parent().parent().prev().addClass('current');
	$("#main-nav li a.current").parent().find("ul").slideToggle("slow"); // Slide down the current menu item's sub menu

	$("#main-nav li a.nav-top-item").click( // When a top menu item is clicked...
		function () {
			$(this).parent().siblings().find("ul").slideUp("normal"); // Slide up all sub menus except the one clicked
			$(this).next().slideToggle("normal"); // Slide down the clicked sub menu
			return false;
		}
	);

	$("#main-nav li a.no-submenu").click( // When a menu item with no sub menu is clicked...
		function () {
			window.location.href = (this.href); // Just open the link instead of a sub menu
			return false;
		}
	);

	// Sidebar Accordion Menu Hover Effect:

	$("#main-nav li .nav-top-item").hover(
		function () {
			$(this).stop().animate({ paddingRight: "25px" }, 200);
		},
		function () {
			$(this).stop().animate({ paddingRight: "15px" });
		}
	);

	//Minimize Content Box

	$(".content-box-header h3").css({ "cursor": "s-resize" }); // Give the h3 in Content Box Header a different cursor
	$(".closed-box .content-box-content").hide(); // Hide the content of the header if it has the class "closed"
	$(".closed-box .content-box-tabs").hide(); // Hide the tabs in the header if it has the class "closed"

	$(".content-box-header h3").click( // When the h3 is clicked...
		function () {
			$(this).parent().next().toggle(); // Toggle the Content Box
			$(this).parent().parent().toggleClass("closed-box"); // Toggle the class "closed-box" on the content box
			$(this).parent().find(".content-box-tabs").toggle(); // Toggle the tabs
		}
	);

	// Content box tabs:

	$('.content-box .content-box-content div.tab-content').hide(); // Hide the content divs
	$('ul.content-box-tabs li a.default-tab').addClass('current'); // Add the class "current" to the default tab
	$('.content-box-content div.default-tab').show(); // Show the div with class "default-tab"

	$('.content-box ul.content-box-tabs li a').click( // When a tab is clicked...
		function () {
			$(this).parent().siblings().find("a").removeClass('current'); // Remove "current" class from all tabs
			$(this).addClass('current'); // Add class "current" to clicked tab
			var currentTab = $(this).attr('href'); // Set variable "currentTab" to the value of href of clicked tab
			$(currentTab).siblings().hide(); // Hide all content divs
			$(currentTab).show(); // Show the content div with the id equal to the id of clicked tab
			return false;
		}
	);

	//Close button:

	$(".close").click(
		function () {
			$(this).parent().fadeTo(400, 0, function () { // Links with the class "close" will close parent
				$(this).slideUp(400);
			});
			return false;
		}
	);

	// Alternating table rows:

	$('tbody tr:even').addClass("alt-row"); // Add class "alt-row" to even table rows


	// Initialise Facebox Modal window:

	if ($.fancybox) {
		$('a[rel*=modal]').fancybox({
			showNavArrows: false,
			width: '80%',
			margin: 0,
			padding: 0,
			height: 510,
			//scrolling: 'no',
			hideOnOverlayClick: false

			//			'autoScale': false
		}); // Applies modal window to any link with attribute rel="modal"
	}

	$('a[rel*=ajax]').live('click', function () {
		var confirm_msg = $(this).attr("confirm");
		if (confirm_msg && !confirm(confirm_msg)) { return false; }

		var url = $(this).attr("href");
		$.ajax({
			url: url,
			data: $('form').serialize(),
			dataType: 'json',
			type: 'GET',
			success: function (r) {
				if (r.success.toString().toLowerCase() == "false") {
					alert("提交数据时发生错误：" + r.message);
					return;
				}
				alert("处理成功！");
				document.location.href = document.location.href;
			},
			error: function (r) {
				alert("提交数据时发生错误：" + r);
			}
		});
		return false;
	});

	$('a[rel=batch-delete]').live('click', function () {
		var checked_items = $(".gridview input[type='checkbox'].check-item");


		var ids = new Array();
		checked_items.each(function (i, e) {
			if ($(e).is(":checked")) {
				ids.push($(e).val());
			}
		});

		if (ids.length == 0) {
			alert("请选择要删除的数据");
			return false;
		}

		if (!confirm("确认要删除选择的数据吗？")) { return false; }

		var data = { id: ids.join(",") };

		//$.extend(data, $('form').serialize());
		//debugger;
		var url = $(this).attr("href");
		$.ajax({
			url: url,
			data: data,
			dataType: 'json',
			type: 'GET',
			success: function (r) {
				if (r.success.toString().toLowerCase() == "false") {
					alert("提交数据时发生错误：" + r.message);
					return;
				}
				alert("处理成功！");
				document.location.href = document.location.href;
			},
			error: function (r) {
				alert("提交数据时发生错误：" + r);
			}
		});
		return false;
	});

	$("input[rel*=search]").click(function () {
		var t = $(this).attr("target");
		$("#" + t).reload($('form').serialize());
	});

	/*
	$("input.datepicker").live('click',function () {
	WdatePicker(); 
	}); 
	*/
});


(function ($) {
	$.extend({
		ajaxSave: function (options) {
			var s = {
				data: $('form').serialize(),
				dataType: 'json',
				type: 'POST',
				success: function (r) {
					if (r.success.toString().toLowerCase() == "false") {
						alert("提交数据时发生错误：" + r.message);
						return;
					}
					parent.location.href = parent.location.href;
					//alert("保存成功！");
					parent.$.fancybox.close();
				},
				error: function (r) {
					alert("提交数据时发生错误：" + r);
				}
			};
			$.extend(s, options);
			$.ajax(s);
		}
	});
})(jQuery);


  
  
  