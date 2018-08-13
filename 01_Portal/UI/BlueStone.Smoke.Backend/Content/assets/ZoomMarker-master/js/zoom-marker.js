
(function($) {

    var options, that, dialog;
    var isInit = false;
    var markerList = [];     // 数组，存放marker的DOM对象
    var markerId = 0;           // marker的唯一ID，只能增加
    var imgDefaultWidth = 0;    //图片第一次加载时的宽度
    var imgDefaultHeight = 0;
    var imgDefaultOffset = {};


    $.fn.extend({
        "zoomMarker": function (_options) {
            if(isInit)
                return;
            isInit = true;
            document.ondragstart=function() {return false;}
            that = $(this);
            var offset;
            // 初始化配置
            if(typeof(_options)==='undefined')
                options = defaults;
            else
                options = $.extend({}, defaults, _options);

            that.context = {};
            that.context.options = options;
            // 配置图片资源
            if (options.src === null) {
                that.hide();
                console.log('Image resources is not defined.');
            }
            else {
                loadImage(options.src);
            }

            if (options.imgzoom) {
                // 初始化鼠标滚动监听器
                that.bind('mousewheel', function (event, delta) {
                    that.zoomMarker_Zoom({ x: event.pageX, y: event.pageY }, delta > 0 ? (1 + options.rate) : (1 - options.rate), that);
                    return false;
                });
            }

            if (options.imgdrag) {
                // 图片拖动监听器
                var picHamer = new Hammer(document.getElementById($(this).attr('id')));
                picHamer.on("pan", function (e) {
                    if (!(options.pinchlock || false)) {
                        dialog.hide();

                        that.zoomMarker_Move(e.deltaX + offset.left, e.deltaY + offset.top);
                    }
                });
                picHamer.on("panstart", function (e) {
                    offset = that.offset();
                });
                picHamer.on("panend", function (e) {
                    // 解除pinch事件，使能pan
                    options.pinchlock = false;

                    setImageArea();

                });
                // 触屏移动事件
                picHamer.get('pinch').set({ enable: true });
                picHamer.on("pinchmove", function (e) {
                    dialog.hide();
                    that.zoomMarker_Zoom({ x: e.center.x, y: e.center.y }, e.scale / options.pinchscale, that);
                    options.pinchscale = e.scale;
                });
                picHamer.on("pinchstart", function (e) {
                    // 为了防止缩放手势结束后，两个手指抬起的速度不一致导致误判为pan事件，这里锁定pinch并在pan释放后解除
                    options.pinchlock = true;
                    options.pinchscale = 1.0;
                });
            }


            // 添加用于显示marker悬浮内容窗的div标签
            dialog = $("<div id='zoom-marker-hover-dialog' class='zoom-marker-hover-dialog'></div>");
            that.parent().append(dialog);

            // 添加图像点击监听器，发送消息
            new Hammer(document.getElementById(that.attr('id'))).on('tap', function (e) {
                if(typeof(e.pointers[0].x)==='undefined'){
                    var offset = that.offset();
                    that.trigger("zoom_marker_mouse_click", {
                        x: (e.center.x-offset.left) / that.width() * options.imgNaturalSize.width,
                        y: (e.center.y-offset.top) / that.height() * options.imgNaturalSize.height
                    });
                }
                else {
                    that.trigger("zoom_marker_mouse_click", {
                        pageX: e.pointers[0].offsetX,
                        pageY: e.pointers[0].offsetY,
                        x: e.pointers[0].offsetX / that.width() * options.imgNaturalSize.width,
                        y: e.pointers[0].offsetY / that.height() * options.imgNaturalSize.height
                    });
                }
            });

            that.mouseup(function (event) {//鼠标释放
                var marker=operateMarker[0];
                var p = getMousePos(event);
                if (marker != null) {
                    //marker.PAtive = true;   //位置有效
                    $("[SAMEDELETE='SAMEDELETE']").remove();

                    var option = {
                        src: "/Content/assets/ZoomMarker-master/img/marker.svg",
                        x: event.offsetX / that.width() * options.imgNaturalSize.width,
                        y: event.offsetY / that.height() * options.imgNaturalSize.height,
                        size: 30,
                        dialog: {
                            value: marker.Text ,
                            offsetX: 20,
                            style: {
                                "border-color": "#bfbfbf"
                            }
                        },
                        hint: { value: 1, style: { color: "#ffffff", left: "10px" } }
                    };

                    that.zoomMarker_AddMarkerV2(option, marker);

                    if (options.afterReaseMarker) {
                        var markerData = null;
                        for (var i = 0; i < markerList.length; i++) {
                            if (markerList[i].marker == marker) {
                                markerData = markerList[i];
                                break;
                            }
                        }

                        if (markerData) {
                            options.afterReaseMarker(markerData);
                        }                        
                    }


                }

                if (options.showdialogtype == 'click') {
                    dialog.hide();
                }
            });

            


        },
        // 加载图片
        "zoomMarker_LoadImage": function (src,hander) {
            if (src) {
                that.show();
                loadImage(src, hander);
            } else {
                that.hide();
            }  
        },
        /**
         * 图片缩放
         * @param center    缩放中心坐标，{x:1, y:2}
         * @param scale     缩放比例，>0为放大，<0为缩小
         */
        "zoomMarker_Zoom": function (center, scale, that){
            //var that = $(this);
            var options = that.context.options;
            var offset = that.offset();
            var h0 = that.height();
            var w0 = that.width();
            var tarWidth = w0*scale;
            // 宽度限制
            if(options.max!=null && (tarWidth>options.max||tarWidth<options.min)){
                return;
            }
            that.height(h0*scale);
            that.width(tarWidth);
            if(typeof(center)==='undefined' || center===null){
                center = {};
                center.x = offset.left+w0/2;
                center.y = offset.top+h0/2;
            }
            that.offset({
                top: (center.y-that.height()*(center.y-offset.top)/h0),
                left: (center.x-that.width()*(center.x-offset.left)/w0)})
            reloadMarkers();

            setImageArea();
        },
        // 图片拖动
        // x>0向右移动，y>0向下移动
        "zoomMarker_Move": function(x, y){

            $(this).offset({top:y, left:x});
            reloadMarkers();
        },
        // 添加标记点
        // marker {src:"marker.png", x:100, y:100, size:20}
        "zoomMarker_AddMarker": function(marker){
            return addMarker(marker);
        },
        // 添加标记点
        // marker {src:"marker.png", x:100, y:100, size:20}
        "zoomMarker_AddMarkerV2": function (markerOption,markerObj) {
            return addMarkerV2(markerOption, markerObj);
        },

        // 删除标记点
        "zoomMarker_RemoveMarker": function(markerId){
            removeMarker(markerId);
        },
        // 清空标记点
        "zoomMarker_CleanMarker": function(){
            cleanMarkers();
        },
        // 获取图像真实尺寸
        "zoomMarker_GetPicSize": function(){
            return options.imgNaturalSize;
        },
        // 图像居中显示
        "zoomMarker_ImageCenterAlign" : function(){
            imageCenterAlign();
        },
        "zoomMarker_GetData": function () {
            return markerList;
        },
        "zoomMarker_LoadMarker": function (markerData) {
            loadMarkersV2(markerData);
        },
        "zoomMarker_HideImg": function (markerData) {
            that.hide();
        },
        "zoomMarker_ImgSizeReset": function () {
            var styleStr = that.attr("style");

            styleStr = removeStyleItem(styleStr, "width:|height:|top:|left:");

            that.attr("style", styleStr);
        },
        "zoomMarker_SetImageArea": function (setDefault) {
            setImageArea(setDefault);
        },
    });

    /**
     * 异步获取图片大小，需要等待图片加载完后才能判断图片实际大小
     * @param img
     * @param fn        {width:rw, height:rh}
     */
    var getImageSize = function(img, fn){
        img.onload = null;
        if(img.complete){
            fn(_getImageSize(img));
        }
        else{
            img.onload = function(){
                fn(_getImageSize(img));
            }
        }
    };

    /**
     * 获取图片大小的子方法，参考getImageSize()
     * @param img
     * @returns {{width: (Number|number), height: (Number|number)}}
     * @private
     */
    var _getImageSize = function(img){
        if (typeof img.naturalWidth === "undefined") {
            // IE 6/7/8
            var i = new Image();
            i.src = img.src;
            var rw = i.width;
            var rh = i.height;
        }
        else {
            // HTML5 browsers
            var rw = img.naturalWidth;
            var rh = img.naturalHeight;
        }
        return ({width:rw, height:rh});
    }

    /**
     * 加载图片
     * @param src
     * @param noResize  是否调整图片的位置
     */
    var loadImage = function (src, handler) {
        that.show();
        //that.trigger("zoom_marker_img_load", src);
        that.attr("src", src);
        //if(src)
        getImageSize(document.getElementsByName(that.attr('name'))[0], function (size) {
            options.imgNaturalSize = size;
            //if (typeof (noResize) === 'undefined' || !noResize) {
            //    // 调整图片宽高
            //    //var originWidth = that.width();
            //    //var originHeight = that.height();
            //    //if (options.width != null) {
            //    //    that.width(options.width);
            //    //    that.height(that.width() / originWidth * originHeight);
            //    //}

            //    var parentWidth = that.parent().width();
            //    var width = parentWidth * 0.8;
            //    var originWidth = that.width();
            //    var originHeight = that.height();
            //    if (options.width != null) {
            //        that.width(width);
            //        that.height(width/options.imgNaturalSize.width   * options.imgNaturalSize.height);
            //    }
            //}
            that.trigger("zoom_marker_img_loaded", size);

            if (options.resetimgafterload) {
                that.zoomMarker_ImgSizeReset();
            }



            //if(typeof(noResize)==='undefined' || !noResize) {
            //    imageCenterAlign();
            //}
            //options.imgNaturalSize = size;
            //loadMarkers(options.markers);


            imgDefaultWidth = that.outerWidth();
            imgDefaultHeight = that.outerHeight();
            imgDefaultOffset = that.offset();

            setImageArea();

            if (handler) {
                handler();
            }

        });
    }

    /**
     * 图像居中
     */
    var imageCenterAlign = function () {
        // 图像居中
        var offset = that.offset();
        var pDiv = that.parent();
        var top = offset.top + (pDiv.height() - that.height()) / 2;
        var left = offset.left + (pDiv.width() - that.width()) / 2;
        that.offset({top: top > 0 ? top : 0, left: left > 0 ? left : 0});
    }

    /**
     * 加载marker
     * @param markers
     */
    var loadMarkers = function(markers){
        $(markers).each(function(index, marker){
            addMarker(marker);
        });
    };

    var loadMarkersV2 = function (markersData) {
        $(markersData).each(function (index, marker) {
            addMarkerV2(marker.option, marker.markerObj);
        });
    };


    /**
     * 添加marker
     * @param marker
     */
    var addMarker = function(marker){
        var _marker = $("<div class='zoom-marker'><img draggable='false'><span></span></div>");
        var __marker = _marker.find("img");
        var size = marker.size||options.marker_size;
        marker.size = size;
        __marker.attr('src', marker.src);
        // 标记点内容文本
        if(typeof(marker.hint)!='undefined'){
            var span = _marker.find("span");
            //span.empty().append(marker.hint.value||"");
            span.css(marker.hint.style||{});
        }
        __marker.height(size);
        __marker.width(size);
        var markerObj = {id:markerId++, marker:_marker, param:marker};
        // 按键监听器
        _marker.click(function(){
            if(typeof(marker.click)!="undefined"){
                marker.click(markerObj);
            }
            that.trigger("zoom_marker_click", markerObj);
        });
        // 悬浮监听器
        if(typeof(marker.dialog)!='undefined'){
            _marker.mousemove(function(e){
                options.hover_marker_id = markerObj.id;
                dialog.empty().append(marker.dialog.value||'').css(marker.dialog.style||{}).show().offset({
                    left: (marker.dialog.offsetX||0) + e.pageX,
                    top: (marker.dialog.offsetY||0) + e.pageY
                });
            });
            _marker.mouseout(function(e){
                options.hover_marker_id = null;
                dialog.hide();
            });
        };
        that.parent().append(_marker);
        markerList.push(markerObj);
        setMarkerOffset(_marker, marker, that.offset());

        return markerObj;
    }



    var addMarkerV2 = function (marker, _marker) {
        //var _marker = $("<div class='zoom-marker'><img draggable='false'><span></span></div>");
        var size = marker.size || options.marker_size;
        marker.size = size;
        // 标记点内容文本
        if (typeof (marker.hint) != 'undefined') {
            var span = _marker.find("span");
            //span.empty().append(marker.hint.value || "");
            span.css(marker.hint.style || {});
        }
        var markerObj = { id: marker.SysNo, marker: _marker, param: marker };

        // 悬浮监听器
        if (typeof (marker.dialog) != 'undefined' && marker.dialog != null) {
            var type = options.showdialogtype;
            if (typeof (type) === 'undefined' || type == null || type == "mousemove") {
                _marker.mousemove(function (e) {
                    options.hover_marker_id = markerObj.id;
                    dialog.empty().append(marker.dialog.value || '').css(marker.dialog.style || {}).show().offset({
                        left: (marker.dialog.offsetX || 0) + e.pageX,
                        top: (marker.dialog.offsetY || 0) + e.pageY
                    });
                });
                _marker.mouseout(function (e) {
                    options.hover_marker_id = null;
                    dialog.hide();
                });
            }
            else if (type == 'click') {
                _marker.click(function (e) {
                    options.hover_marker_id = markerObj.id;
                    dialog.empty().append(marker.dialog.value || '').css(marker.dialog.style || {}).show().offset(getDialogOffset(e, marker.dialog.value));
                });

                
            }

        };
        markerList.push(markerObj);



        setMarkerOffset(_marker, marker, that.offset());

        return markerObj;
    }



    /**
     * 在拖动或缩放后重新加载图标
     */
    var reloadMarkers = function(){
        var offset = that.offset();
        $(markerList).each(function (index, element) {
            setMarkerOffset(element.marker, element.param, offset);
        });
    };

    /**
     * 清空标记
     */
    var cleanMarkers = function(){
        $(markerList).each(function(index, element){
            element.marker.unbind();
            element.marker.remove();
        });
        markerList = [];
        options.markers = [];
        dialog.hide();
    };

    /**
     * 删除标记
     * @param markerId 标记点的唯一ID
     */
    var removeMarker = function(markerId){
        $(markerList).each(function(index, element){
            if(element.id===markerId) {
                element.marker.unbind();
                element.marker.remove();
                // 如果当前悬浮窗在该marker上显示，需要隐藏该悬浮窗
                if(((options.hover_marker_id||null)!=null) && options.hover_marker_id===markerId){
                    dialog.hide();
                }
                return false;
            }
        });
    }

    /**
     * 配置marker的offset
     * @param marker        需要配置的marker对象
     * @param position      marker的位置信息，包含{x: , y: }
     * @param offset        图片的offset信息
     */
    var setMarkerOffset = function (marker, position, offset) {
        marker.offset({
            left: that.width() * position.x / options.imgNaturalSize.width + offset.left - position.size/2,
            top: that.height() * position.y / options.imgNaturalSize.height + offset.top - position.size
        });
    }


    var setImageArea = function (setDeault) {
        if (that.css("display") == "none" || setDeault) {
            imageArea.minx = 0;
            imageArea.miny = 0;
            imageArea.maxx = 0;
            imageArea.maxy = 0;
        } else {
            //debugger;
            var parent = that.parent();
            var parentOffset = parent.offset();
            var parentMinX = parentOffset.left;
            var parentMinY = parentOffset.top;
            var parentMaxX = parentOffset.left + parent.outerWidth();
            var parentMaxY = parentOffset.top + parent.outerHeight();


            var offset = that.offset();
            imageArea.minx = offset.left > parentMinX ? offset.left : parentMinX;
            imageArea.miny = offset.top > parentMinY ? offset.top : parentMinY;
            imageArea.maxx = offset.left + that.width();
            imageArea.maxy = offset.top + that.height();

            imageArea.maxx = imageArea.maxx < parentMaxX ? imageArea.maxx : parentMaxX;
            imageArea.maxy = imageArea.maxy < parentMaxY ? imageArea.maxy : parentMaxY;
        }
    }

    var removeStyleItem = function (styleStr,removeStr) {
        var array = [];
        if (styleStr.indexOf(":") > -1) {
            array = styleStr.split(";");
        } else {
            array.push(styleStr);
        }
        
        var checkArray = [];
        if (removeStr.indexOf("|") > -1) {
            checkArray = removeStr.split("|");
        } else {
            checkArray.push(removeStr);
        }

        var indexArray = [];
        $.each(array, function (i, n) {
            $.each(checkArray, function (j, m) {
                if (n.indexOf(m) > -1) {
                    indexArray.push(i);
                    return false;
                }
            });
        });

        while (indexArray.length > 0) {
            var maxIndex = Math.max.apply(null, indexArray); //取数组中最大值
            array.splice(maxIndex, 1);

            var p = indexArray.indexOf(maxIndex);
            indexArray.splice(p, 1);
        }

        return array.join(';');
    }

    var getMousePos= function (event) {
        var e = event || window.event;
        var scrollX = document.documentElement.scrollLeft || document.body.scrollLeft;
        var scrollY = document.documentElement.scrollTop || document.body.scrollTop;
        var x = e.pageX || e.clientX + scrollX;
        var y = e.pageY || e.clientY + scrollY;
        //alert('x: ' + x + '\ny: ' + y);
        return { x: x, y: y };
    }

    var getDialogOffset = function (event, dialog) {
        var offset = {};

        var marker = $(event.target);
        var markerwidth = marker.outerWidth();
        var markerheight = marker.outerHeight();
        var markeroffset = marker.offset();
        var markercenter = { x: markeroffset.left + markerwidth / 2, y: markeroffset.top + markerheight / 2 };

        var p = getMousePos(event);
        var imgoffset = imgDefaultOffset;
        var imgwidth = imgDefaultWidth;
        var imgheight = imgDefaultHeight;

        var digwidth = dialog.outerWidth();
        var digheight = dialog.outerHeight();
        console.log(p.y);
        if (p.x < imgoffset.left + imgwidth / 2) {
            offset.left = markercenter.x + markerwidth / 2;
        } else {
            offset.left = markercenter.x - digwidth - markerwidth / 2;
        }

        if (p.y < imgoffset.top + imgheight / 2) {
            offset.top = markercenter.y + markerheight/2;
        } else {
            offset.top = markercenter.y - digheight - markerheight / 2;
        }

        return offset;

    }

    var defaults = {
        rate: 0.2,              // 鼠标滚动的缩放速率
        src: null,             // 图片资源
        width: 500,           // 指定图片宽度
        min: 300,               // 图片最小宽度
        max: null,              // 图片最大宽度
        markers: [],             // marker数组，[{src:"marker.png", x:100, y:100, size:20, click:fn()}]
        marker_size: 20,        // 默认marker尺寸
        imgdrag: true,         //图片能否拖动
        imgzoom: true,         //图片能否放大缩小
        //newdialog: true,        //新dialog
        showdialogtype: null,    //dialog展示方式
        resetimgafterload: false,   //加载完成后 重置图片大小

    }

})(window.jQuery);
