/// <reference path="../jquery/jquery-vsdoc.js" />

(function ($) {
	$.fn.reload = function (param) {
		var _grid = this;
		$.post(this.attr("url"), param, function (data) {
			try {
				var r = $.parseJSON(data);
				if (r.success == false) {
					alert(r.message);
					return;
				}
			}
			catch (e) {
			} 
			_grid.html(data); 
		});
	};

	$.fn.refresh = function () {
		this.find("div.pReload").parent().click();
	};

	$.fn.getSelectItem = function () {
		var selectrow = this.find("table tbody tr.selectrow td input");
		var selectitem = {};
		selectrow.each(function (i) {
			var item = $(this);
			selectitem[item.attr("class")] = item.val();
		});
		return selectitem;
	};

	//表头或分页点击
	$(".gridview a.ajax-headerLink,.gridview a.ajax-pagerLink").live("click", function () {//.grid-header
		var _t = $(this);
		$.post(_t.attr("href"), $('form').serialize(), function (data) {
			try {
				var r = $.parseJSON(data);
				if (r.success == false) {
					alert(r.message);
					return;
				}
			}
			catch (e) {
			}
            var gridID =  _t.attr("UpdateTargetId")
			var grid = $("#" + gridID);
			grid.html(data);
		});
		return false;
	});

	// Select row when clicked.
	$(".gridview tbody tr").live("click", function () {
		var _t = this;
		$(".gridview tbody tr").filter(function (i) {
			return this != _t;
		})
		.removeClass("selectrow");

		$(this).toggleClass("selectrow");
	});

	// Check all checkboxes when the one in a table head is checked.
	$(".gridview input[type='checkbox'].check-all").live("click",
		function () {
			$(this).parent().parent().parent().parent().find("input[type='checkbox'].check-item").attr('checked', $(this).is(':checked'));
		}
	);

	$(".gridview input[type='checkbox'].check-item").live("click",
		function () {
			if (!$(this).is(':checked')) {
				$(this).parent().parent().parent().parent().find("input[type='checkbox'].check-all").attr('checked', false);
			}
			else {
				var len = $(".gridview input[type='checkbox'][checked!='true'].check-item").length;
				if (len == 0) {
					$(this).parent().parent().parent().parent().find("input[type='checkbox'].check-all").attr('checked', true);
				}
			}
		}
	);

})(jQuery);