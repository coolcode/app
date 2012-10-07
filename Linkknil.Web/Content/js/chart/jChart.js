/*
说明：jChart是基于Visifire报表和jQuery的js组件
作者：Bruce Lee
创建日期：2010-04-19
创建日期：2011-02-08
*/

/*  Sample: 

	$("#id").jChart({ 
		title: "报表标题",
		yTitle: "Y轴标题", 
		xTitle: "X轴标题",  
		renderAs: "Column",
		url: "foo",
		view3D: true
	})
	.loadChart();
*/

(function ($) {

	//visifire 的xml数据模板
	var chartXmlString = ''
	+ '<vc:Chart xmlns:vc="clr-namespace:Visifire.Charts;assembly=SLVisifire.Charts" Width="{0}" Height="{1}" View3D="{2}" BorderThickness="0" Theme="Theme1"  Watermark="False">'
		+ '<vc:Chart.Titles>'
			+ '<vc:Title Text="{3}" />'
		+ '</vc:Chart.Titles>'
		+ '<vc:Chart.AxesX>'
			+ '<vc:Axis Title="{4}" />'
		+ '</vc:Chart.AxesX>'
		+ '<vc:Chart.AxesY>'
			+ '<vc:Axis Title="{5}" AxisType="Primary" />'
		+ '</vc:Chart.AxesY>'
		+ '<vc:Chart.Series>'
	//+ '<vc:DataSeries RenderAs="{6}" ShowInLegend="True"  AxisYType="Primary" >'
				+ '{7}'
	//+ '</vc:DataSeries>'
		+ '</vc:Chart.Series>'
	+ '</vc:Chart>'
	;

	//格式化字符串
	$.fn.format = function (source, params) {
		if (arguments.length == 1)
			return function () {
				var args = $.makeArray(arguments);
				args.unshift(source);
				return _ajax.format.apply(this, args);
			};
		if (arguments.length > 2 && params.constructor != Array) {
			params = $.makeArray(arguments).slice(1);
		}
		if (params.constructor != Array) {
			params = [params];
		}
		$.each(params, function (i, n) {
			source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
		});
		return source;
	};

	//配置Chart
	$.fn.jChart = function (settings) {
		var thisObject = this;

		//默认配置
		var s = {
			xapPath: "/Content/js/chart/SL.Visifire.Charts.xap",
			renderAs: "Column",
			width: 650,
			height: 400,
			view3D: false,
			title: "",
			xTitle: "",
			yTitle: "",
			autoLoad: true,
			param: {},
			callback: undefined
		};

		$.extend(s, settings);

		//visifire 报表组件实例
		var vChart = new Visifire(s.xapPath, s.width, s.height);

		//ajax 加载数据
		this.loadChart = function (param, callback) {
			$.get(s.url, param, function (data) {
				if (data) {
					thisObject.each(function () {
						var id = $(this).attr("id");
						data = data.replace(new RegExp("\\{RenderAs\\}", "g"), s.renderAs);
						var xml = $.format(chartXmlString, s.width, s.height, s.view3D, s.title, s.xTitle, s.yTitle, s.renderAs, data);
						//alert("xml:" + xml);
						vChart.setDataXml(xml);
						vChart.render(id);

						if (typeof callback == "function") {
							callback(data);
						}
					});
				}
				else {
					$(thisObject).html("没有数据！");
				}
			});
		};

		if (s.autoLoad) {
			$(function () {
				thisObject.loadChart(s.param, s.callback);
			});
		}

		return this;
	}
})(jQuery);