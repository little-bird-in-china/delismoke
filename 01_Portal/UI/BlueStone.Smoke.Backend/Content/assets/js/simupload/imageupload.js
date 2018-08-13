
(function ($) {
    $.fn.imguploader = function (opts) {

        var defaults = {
            allowedFileExtensions: ['jpg', 'png', 'jpeg', 'gif'],//文件格式
            url: '',//文件上传地址
            imgServier: '', //图片服务器地址(用于拼接图片的url地址)
            maxImgSize: 1000000,//文件大小(KB)
            maxImgCount: 65535,  //最大图片数
            defaultImages: [],  //初始化加载图片
            extraBtns: [],  //其他自定义button
            extraBtnHandlers: [],  //其他自定义button click事件
            multi: true,//
            onUploadStart: function () { },//
            onUploadSuccess: function () { },//
            beforeDelete: function () { }, //删除图片前事件
            //onUploadComplete: function () { },//
            onUploadError: function () { }, //
            onInit: function () { },//
        }

        var option = $.extend(defaults, opts);

        //
        var formatFileSize = function (size) {
            if (size > 1024 * 1024) {
                size = (Math.round(size * 100 / (1024 * 1024)) / 100).toString() + 'MB';
            }
            else {
                size = (Math.round(size * 100 / 1024) / 100).toString() + 'KB';
            }
            return size;
        }



        //
        var getFile = function (index, files) {
            for (var i = 0; i < files.length; i++) {
                if (files[i].index == index) {
                    return files[i];
                }
            }
            return false;
        }
        //
        //var formatFileType = function (str) {
        //    if (str) {
        //        return str.split(",");
        //    }
        //    return false;
        //}



        new ImgUploader(this, option);


    }

    var ImgUploader = function (element, options) {
        var self = this;
        self._init(element, options);
    };

    ImgUploader.prototype = {
        constructor: ImgUploader,
        //_imgs:[],
        _init: function (p, options) {
            this._imgs = [];
            this.options = options;
            this._initHtml(p);
            this._initEvent();
            if (options.defaultImages && options.defaultImages.length > 0) {
                this._setImgs(options.defaultImages);
                if (options.defaultImages.length >= options.maxImgCount) {
                    $(".add-img", self.ele).parent().hide();
                }
            }
        },
        _initHtml: function (p) {

            //debugger;
            var html = $('<div class="img-box full"></div>');
            html.append(
                '<section class=" img-section">' +
                //'<p class="up-p">作品图片：' +
                //    '<span class="up-span">最多可以上传5张图片，马上上传</span>' +
                //'</p>' +
                '<div class="z_photo upimg-div clear" style="padding: 18px; border: 2px dashed #E7E6E6;">' +
                '<section class="z_file fl">' +
                '<img src="/Content/assets/js/simupload/a11.png" class="add-img">' +
                '<input type="file" name="imgupload" id="imgupload" class="file" style="display:none" value="" accept="image/jpg,image/jpeg,image/png,image/bmp" multiple="">' +
                '</section>' +
                '</div>' +
                '</section>');

            this.ele = html;
            var imgBox = $(p).closest(".img-box");
            if (imgBox.length == 0) {
                $(p).replaceWith(this.ele);
            } else {
                imgBox.replaceWith(this.ele);
            }


        },
        _initEvent: function () {

            var self = this;

            $("img.add-img", self.ele).click(function () {
                $("input[name='imgupload']", self.ele).trigger('click');
            });

            $("input[name='imgupload']", self.ele).change(function (e) {
                
                var imgContainer = $(".z_photo", self.ele);
                var files = e.target.files;

                if (!self._fileCheck(files)) {
                    return false;
                }


                $.each(files, function (i, n) {

                    var file = n;
                    var img = { SysNo: 0, Url: window.URL.createObjectURL(file)};
                    var $section = self._createSection(0, img);
                    imgContainer.prepend($section);

                    self._doUpload(file, $section, function (sectionObj, response) {
                        var imgData = JSON.parse(response);
                        var url = self._getUploadImgUrl(imgData.url);
                        $(".up-section", sectionObj).removeClass("loading");
                        $(".up-img", sectionObj).removeClass("up-opcity");
                        $(".up-img", sectionObj).attr("src", url);

                        img.ImagePath = imgData.url;
                        img.Url = url;
                        img.FileName = file.name;
                        img.FileSize = file.size;
                        self._imgs.push(img);

                        if (self._imgs.length >= self.options.maxImgCount) {
                            $(".add-img", self.ele).parent().hide();
                        }

                        if (self.options.onUploadSuccess) {
                            self.options.onUploadSuccess(img);
                        }

                    });


                });

                $(this).val('');
            });
        },

        _doUpload: function (file, backData, successHandler) {
            var self = this;
            var xhr = new XMLHttpRequest();
            if (xhr.upload) {
                // 涓婁紶涓�
                //xhr.upload.addEventListener("progress", function (e) {
                //    self.onProgress(file, e.loaded, e.total);
                //}, false);

                // 鏂囦欢涓婁紶鎴愬姛鎴栨槸澶辫触
                xhr.onreadystatechange = function (e) {
                    if (xhr.readyState == 4) {
                        if (xhr.status == 200) {


                            //self.onUploadSuccess(file, xhr.responseText);
                            //setTimeout(function () { ZXXFILE.onDelete(file.index); }, option.removeTimeout);

                            //self.onUploadComplete(xhr.response);
                            if (successHandler) {
                                successHandler(backData, xhr.response);
                            }
                        } else {
                            //self.onUploadError(file, xhr.responseText);
                            console.log('uploaderror' + xhr.responseText);
                        }
                    }
                };

                var formData = new FormData();
                formData.append("myfile", file);

                //option.onUploadStart();
                // 寮€濮嬩笂浼�
                xhr.open("POST", self.options.url, true);
                //xhr.setRequestHeader("X_FILENAME", file.name);
                //xhr.send(file);
                xhr.send(formData);
            }
        },

        _getUploadImgUrl: function (uploadurl) {
            return this.options.imgServier + uploadurl;
        },

        _createSection: function (sysno, img) {
            var self = this;
            var $section = $("<section class='up-section fl loading'>");
            //imgContainer.prepend($section);
            var $span = $("<span class='up-span'>");
            $span.appendTo($section);

            var $img0 = $("<img class='close-upimg'>").on("click", function (event) {
                self._delete($section, img);
            });
            $img0.attr("src", "/Content/assets/js/simupload/img/a7.png").appendTo($section);
            var $img = $("<img class='up-img up-opcity'>");
            $img.attr("src", img.Url);
            $img.appendTo($section);

            self._addExtraBtn($section, img);

            var $p = $("<p class='img-name-p'>");
            $p.html("").appendTo($section);
            var $input = $("<input id='taglocation' name='taglocation' value='' type='hidden'>");
            $input.appendTo($section);
            var $input2 = $("<input id='tags' name='tags' value='' type='hidden'/>");
            $input2.appendTo($section);

            return $section;
        },

        _delete: function ($section, img) {
            var self = this;
            if (self.options.beforeDelete) {
                self.options.beforeDelete(img);
            }

            var index = self._imgs.indexOf(img);
            if (index >= 0) {
                self._imgs.splice(index, 1);
            }
            $section.remove();

            //event.preventDefault();
            //event.stopPropagation();
            $(".works-mask").show();
            //delParent = $(this).parent();


            if (self._imgs.length < self.options.maxImgCount) {
                $(".add-img", self.ele).parent().show();
            }

        },

        _setImgs: function (imgs) {
            var self = this;
            self._imgs = imgs;
            $(".up-section", self.ele).remove();

            var imgContainer = $(".z_photo", self.ele);
            $.each(imgs, function (i, img) {
                var $section = self._createSection(0, img);
                imgContainer.prepend($section);
            });

            $(".up-section", self.ele).removeClass("loading");
            $(".up-img", self.ele).removeClass("up-opcity");
        },

        _addExtraBtn: function (section, img) {
            var self = this;
            var extraBtns = self.options.extraBtns;
            var extraBtnHandlers = self.options.extraBtnHandlers;
            if (extraBtns && extraBtnHandlers && extraBtns.length > 0 && extraBtns.length == extraBtnHandlers.length) {
                $.each(extraBtns, function (i, btnStr) {
                    var extraBtn = $(btnStr);
                    extraBtn.addClass('extra-btn');
                    $(extraBtn).click(function () {
                        extraBtnHandlers[0](img);
                    });
                    extraBtn.appendTo(section);
                });
            }
        },

        _fileCheck: function (files) {
            var self = this;
            //var typeArray = self._formatFileType(self.options.fileTypeExts);

            if (files.length + self._imgs.length > self.options.maxImgCount) {
                alert("超过允许上传的最大数目");
                return false;
            }
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                if ($.inArray(self._getExtenstion(file.name), self.options.allowedFileExtensions) >= 0) { }
                else {
                    alert("图片格式不正确");
                    return false;
                }

                if (self.options.maxImgSize) {
                    if (file.size > self.options.maxImgSize * 1000) {
                        alert("图片大小过大");
                        return false
                    }
                }
            }

            return true;
        },

        _formatFileType: function (str) {
            if (str) {
                return str.split("|");
            }
            return false;
        },
        _getExtenstion: function (fileName) {
            if (fileName) {
                var array = fileName.split(".");
                return array[array.length - 1];
            }
            return "";
        },
    };

})(jQuery)












