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

    //数据创建后修改当前Url
    $.createChangeUrl = function (addUrlParam) {
        var currentUrl = document.URL;
        var currentParamArray = $.getQueryString();

        var newUrl = currentUrl;
        if (currentParamArray != null && currentParamArray > 0) {
            newUrl += "&";
        } else
        {
            newUrl += "?";
        }
        newUrl += addUrlParam;
        history.pushState(null, null, newUrl);
    }

    //判断对象中是否存在html标签
    //返回值为true 即存在html标签
    $.HasHtmlTag = function (obj) {
        return CheckHtmlTag(obj);
    }


    //去掉小数点后的.00  或.*0
    $.DeletePriceZero = function (price) {
        if (price == null)
            return price;
        var tempPrice = price.toFixed(2);
        if (tempPrice.toString().indexOf('.') > 0) {
            var array = tempPrice.toString().split('.');
            var partOfPrice = parseInt(array[1]);
            if (array[1] == '00') {
                return price.toFixed(0);
            } else if (array[1].substring(array[1].length - 1, array[1].length) == '0') {
                return price.toFixed(1);
            }
        }

        return price.toFixed(2);
    }

    //去掉html标签
    $.RemoveHtml = function (text) {
        if (Object.prototype.toString.call(text) !== "[object String]"||text == "") {
            return text
        }
        return text.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/'/g, '&apos;').replace(/"/g, '&quot;');
    }
    //对数组中元素进行组合
    $.getCombination = function (data, index = 0, group = []) {
        var need_apply = new Array();
        need_apply.push(data[index]);
        for (var i = 0; i < group.length; i++) {
            need_apply.push(group[i] + data[index]);
        }
        group.push.apply(group, need_apply);

        if (index + 1 >= data.length) return group;
        else return $.getCombination(data, index + 1, group);
    } 

    //拷贝对象
    $.copyObject = function (obj) {
        return JSON.parse(JSON.stringify(obj));
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
function CheckHtmlTag(obj) {
    if (typeof (obj) == "object") {
        for (var a in obj) {
            if (typeof (obj[a]) == "object") {
                if (CheckHtmlTag(obj[a])) {
                    return true;
                }
            }
            else {
                if (typeof (obj[a]) == "string" && obj[a] != null) {
                    if (obj[a].indexOf('<') > -1) {
                        return true;
                    }
                }
            }
        }
    }
    else {
        if (typeof (obj) == "string" && obj != null) {
            if (obj.indexOf('<') > -1) {
                return true;
            }
        }
    }
    return false;
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