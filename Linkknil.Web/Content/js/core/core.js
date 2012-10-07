/// <reference path="../jquery/jquery-vsdoc.js" />

$(document).ready(function() {
	$.ajaxSetup({
		cache: false //data: { _date: new Date() }
	});
	$("#loading").ajaxStart(function() {
		$(this).show('slow');
		//$(this).slideDown('slow', function() { el.show(); });
	}).ajaxComplete(function() {
		$(this).hide('slow');
	}).ajaxStop(function() {
		$(this).hide('slow');
	}).slideUp('slow');
});

(function($) {
	$.extend({
		getClientHeight: function() {
			if ($.browser.msie) {
				return document.compatMode == "CSS1Compat" ? document.documentElement.clientHeight : document.body.clientHeight;
			} else {
				return self.innerHeight;
			}
		},
		getClientWidth: function() {
			if ($.browser.msie) {
				return document.compatMode == "CSS1Compat" ? document.documentElement.clientWidth : document.body.clientWidth;
			} else {
				return self.innerWidth;
			}
		}
	});
})(jQuery);
