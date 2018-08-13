(function ($) {

    // wrapper function to  block element(indicate loading)
    function blockUI(el, centerY) {
        var el = jQuery(el); 
        el.block({
            message: '<img src="/Content/assets/img/loading.gif" align="" style="width:30px;">',
            centerY: centerY != undefined ? centerY : true,
            css: {
                top: '50%',
                border: 'none',
                padding: '2px',
                backgroundColor: 'none',
                position: 'fixed',
                zIndex:2010
            },
            overlayCSS: {
                backgroundColor: '#ccc',
                opacity: 0.1,
                cursor: 'wait',
                zIndex:2000
            }
        });
    }

    // wrapper function to  un-block element(finish loading)
    function unblockUI(el) {
        jQuery(el).unblock({
            onUnblock: function () {
                jQuery(el).removeAttr("style");
            }
        });
    }

    $.showLoading = function (target) {
        blockUI(target ? target : $('body'), false);
    };

    $.hideLoading = function (target) {
        if (target) {
            unblockUI(target);
        }
        else {
            unblockUI($('body'));
        }
    };

    //form : Jquery Object
    function buildEntity(form) {
        var obj = {};
        form.find("[data-model]").each(function () {
            var name = $(this).attr("data-model");
            var names = name.split(".");
            var temp;
            var val;
            if ($(this).attr("type") == "checkbox"
                || $(this).attr("type") == "radio") {
                if ($(this).prop("checked")) {
                    val = $(this).val();
                }
            }
            else {
                val = $.trim($(this).val());
            }
            if (names.length > 1) {
                for (var i = 0; i < names.length; i++) {
                    if (temp) {
                        if (!temp[names[i]]) {
                            if (i == (names.length - 1)) temp[names[i]] = val;
                            else temp[names[i]] = {};
                        }
                        temp = temp[names[i]];
                    } else {
                        if (!obj[names[i]]) {
                            if (i == (names.length - 1)) obj[names[i]] = val;
                            else obj[names[i]] = {};
                        }
                        temp = obj[names[i]];
                    }
                }
            } else {
                obj[name] = val;
            }

        });
        return obj;
    }

    $.buildEntity = buildEntity;

    //$.alert = function (msg, callback) {
    //    bootbox.alert(msg, function () {
    //        if (callback) {
    //            callback();
    //        }

    //    });
    //};

    $.confirm = function (msg, callback) {
        bootbox.confirm(msg, function (result) {
            if (callback) {
                callback(result);
            }
        });
    };

    $.showSuccess = function (msg,callback) {
        if (!msg) {
            msg = "操作成功。";
        }
        Notify_(msg, 'bottom-right', '5000', 'success', 'fa-check', true);
        if (callback && typeof callback == 'function') {
            setTimeout(function () {
                callback.call(this);
            }, 5000);
        }
    };

    $.showWarning = function (msg, callback) {
        Notify_(msg, 'bottom-right', '5000', 'warning', 'fa-warning', true);
        if (callback && typeof callback == 'function') {
            setTimeout(function () {
                callback.call(this);
            }, 5000);
        }
    };

    $.showError = function (msg, callback) {
        Notify_(msg, 'bottom-right', '5000', 'danger', 'fa-times', true);
        if (callback && typeof callback == 'function') {
            setTimeout(function () {
                callback.call(this);
            }, 5000);
        }
    };

    $.prompt = function (msg, okCallback, cancelCallback) {
        bootbox.prompt(msg, function (result) {
            if (result === null) {
                if (cancelCallback) {
                    cancelCallback();
                }
                cancelCallback();
            } else {
                if (okCallback) {
                    okCallback(result);
                }
            }
        });
    };

    $.getQueryString = function () {
        var result = location.search.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));
        if (result == null) {
            return "";
        }
        for (var i = 0; i < result.length; i++) {
            result[i] = result[i].substring(1);
        }
        return result;
    }

    //根据QueryString参数名称获取值
    $.getQueryStringByName = function(name) {
        var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    }

    //根据QueryString参数索引获取值
    $.getQueryStringByIndex = function(index) {
        if (index == null) {
            return "";
        }
        var queryStringList = $.getQueryString();
        if (index >= queryStringList.length) {
            return "";
        }
        var result = queryStringList[index];
        var startIndex = result.indexOf("=") + 1;
        result = result.substring(startIndex);
        return result;
    }

    //绑定下拉选择框数据//parm = { id: "", showAll: true, data: [], value: "" }
    $.bindSelecter = function (param) {
        var $select = $("#" + param.id);
        $select.empty();
        if (param.showAll == undefined || param.showAll) {
            $select.append("<option value=''>-所有-</option>");
        }
        if (param.data) {
            $.each(param.data, function (i, item) {
                $select.append("<option value='" + item.Code + "'" + (param.value == item.Code ? " selected='selected'" : "") + ">" + item.Name + "</option>");
            });
        }
        if (typeof (param.callback) == "function") {
            param.callback();
        }
    };

    //Ajax获取下拉选择框数据并绑定到指定select标签上面//parm = { id: "", showAll: true, value: "", url: "" }
    $.selecter = function (param) {
        if (param.id == undefined || param.id.length <= 0) {
            return;
        }
        if (param.url == undefined || param.url.length <= 0) {
            return;
        }
        $.ajax({
            url: param.url,
            type: "POST",
            dataType: "json",
            data: {},
            success: function (data) {
                if (data) {
                    param.data = data;
                    $.bindSelecter(param);
                } else {
                    if (typeof (param.callback) == "function") {
                        param.callback();
                    }
                }
            }
        });
    }

    $.removeHtml = function (text) {
        if (Object.prototype.toString.call(text) !== "[object String]" || text == "") {
            return text
        }
        return text.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/'/g, '&apos;').replace(/"/g, '&quot;');
    }
    $.joinArraryStr =  function (joinArrary, type) {
        if (joinArrary.length > 0) {
            let temparrary = joinArrary.filter( function(value, index) {
                return value !== null && value !== "" && typeof value !== "undefined";
            });
            return temparrary.join(type);
        } else {
            return;
        }
    }
})(jQuery);
$(function () {
    $('select.select2').select2();
    $('body').on('keyup', '.num-only', function () {
        var reg = $(this).val().match(/\d+\.?\d{0,2}/);
        var txt = '';
        if (reg != null) {
            txt = reg[0];
        }
        $(this).val(txt);
    }).change(function () {
        $(this).keypress();
        var v = $(this).val();
        if (/\.$/.test(v)) {
            $(this).val(v.substr(0, v.length - 1));
        }
    });
});
$.ajaxSetup({
    error: onError,
    complete: onComplete,
    beforeSend: onAjaxSend
})
function onAjaxSend(event, xhr) {
    if (xhr.type.toUpperCase() == "GET") {
        if (xhr.url.indexOf("?") > 0) {
            xhr.url += "&t=" + new Date().getTime();
        }
        else {
            xhr.url += "?t=" + new Date().getTime();
        }
    }
    $.showLoading();
}
function onComplete(event, xhr) {
    $.hideLoading();
    var result = event.responseJSON;
    if (result && result.Success != undefined && !result.Success) {
        $.showError(result.Message);
    }
}
function onError(event, xhr,errorMsg) {
    $.hideLoading();
    if (errorMsg != '') {
        $.showError('网络连接异常。');
    }
}

function ReplaceObjHtmlTag(obj) {
    if (typeof (obj) == "object") {
        for (var a in obj) {
            if (typeof (obj[a]) == "object") {
                if (ReplaceObjHtmlTag(obj[a])) {
                    return;
                }
            }
            else {
                if (typeof (obj[a]) == "string" && obj[a] != null) {
                    obj[a]= $.removeHtml(obj[a]);
                }
            }
        }
    }
    else {
        if (typeof (obj) == "string" && obj != null) {
            obj=$.removeHtml(obj);
        }
    }
    return obj;
}



//string 扩展类似C# string.Format()方法
String.prototype.format = function (args) {
    if (arguments.length > 0) {
        var result = this;
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                var reg = new RegExp("({" + key + "})", "g");
                result = result.replace(reg, args[key]);
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] == undefined) {
                    return "";
                }
                else {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
        return result;
    }
    else {
        return this;
    }
}


// 对Date的扩展，将 Date 转化为指定格式的String   
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符，   
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)   
// 例子：   
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423   
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18   
Date.prototype.Format = function (fmt) { //author: meizz   
    var o = {
        "M+": this.getMonth() + 1,                 //月份   
        "d+": this.getDate(),                    //日   
        "h+": this.getHours(),                   //小时   
        "m+": this.getMinutes(),                 //分   
        "s+": this.getSeconds(),                 //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
} 