(function ($) {
    $.alert = function (msg, callback) {
        bootbox.alert(msg, function () {
            if (callback) {
                callback();
            }
        });
    };

    $.confirm = function (msg, callback, options) {
        options = options || {};
        bootbox.dialog({
            message: msg,
            buttons: {
                confirm: {
                    label: options.confirmLabel || "确认",
                    className: "btn-primary",
                    callback: function () {
                        if (callback) {
                            callback(true);
                        }
                    }
                },
                cancel: {
                    label: options.cancelLabel || "取消",
                    className: "btn-default",
                    callback: function () {
                        if (callback) {
                            callback(false);
                        }
                    }
                }
            }
        });
    };

    function blockUI(el, centerY) {
        var el = jQuery(el);
        el.block({
            message: '<img src="/Content/AuthCenter/adminlet/img/loading.gif" align="" style="width:40px;">',
            centerY: centerY != undefined ? centerY : true,
            css: {
                top: '40%',
                border: 'none',
                padding: '2px',
                backgroundColor: 'none',
                position: 'fixed'
            },
            overlayCSS: {
                backgroundColor: '#000',
                opacity: 0.7,
                cursor: 'wait'

            }
        });
    }

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

})(jQuery);

/*
 ajax请求统一处理 start 
 */
$(document).ajaxSend(onAjaxSend)
.ajaxComplete(onComplete)
.ajaxError(onError);

function onAjaxSend(event, xhr, settings) {
    if (settings.type.toUpperCase() == "GET") {
        if (settings.url.indexOf("?") > 0) {
            settings.url += "&t=" + new Date().getTime();
        }
        else {
            settings.url += "?t=" + new Date().getTime();
        }
    }
    $.showLoading();
}
function onComplete(event, xhr, settings) {
    $.hideLoading();
    var result = xhr.responseText;
    if (result && result.Success != undefined && !result.Success) {
        $.alert(result.Message);
    }
}
function onError(event, xhr, settings) {
    $.hideLoading();
    $.alert('网络连接异常。');
}
/*
 ajax请求统一处理 end 
 */