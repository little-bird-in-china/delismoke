(function ($) {
    $.WdatePicker = function (options) {
        options = $.extend(true, {
            src: '',
            minDate: "1900-01-01 00:00:00",
            maxDate: "9999-12-31 23:59:59",
            startDate: '',
            dateFmt: 'yyyy-MM-dd',
            isShowClear: true,
            onpicked: function () { },
            oncleared: function () { }
        }, options);

        var el = document.getElementById(options.src);
        var a = function () {
            WdatePicker({
                minDate: options.minDate,
                maxDate: options.maxDate,
                startDate: options.startDate,
                dateFmt: options.dateFmt,
                isShowClear:options.isShowClear,
                onpicked: function (d) {
                    options.onpicked.call(this, d);
                },
                oncleared: options.oncleared
            });
        };
        if(el.addEventListener)
            el.addEventListener("click",a,false);
        if(el.attachEvent)
            el.attachEvent("onclick", a);
    };
    //日期区间：限制条件 开始时间不能小于结束时间且不能大于当前时间
    $.WdatePickerSection = function (options) {
        var options = $.extend(true, {
            begin: '',
            end:'',
            dateFmt: 'yyyy-MM-dd',
            commMaxDate: ''
        }, options);
        $.WdatePicker({
            src: options.begin,
            maxDate: "#F{$dp.$D(\'" + options.end + "\')||\'" + options.commMaxDate + "\'}",
            dateFmt: options.dateFmt,
            onpicked: function () {

            }
        });
        $.WdatePicker({
            src: options.end,
            minDate: "#F{$dp.$D(\'" + options.begin + "\')}",
            maxDate: options.commMaxDate,
            dateFmt: options.dateFmt,
            onpicked: function () {
              
            }
        });
    };
})(jQuery);