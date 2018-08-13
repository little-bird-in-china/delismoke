
(function ($) {
    $.fn.html5uploader = function (opts) {

        var defaults = {
            fileTypeExts: '',//鍏佽涓婁紶鐨勬枃浠剁被鍨嬶紝濉啓mime绫诲瀷
            url: '',//鏂囦欢鎻愪氦鐨勫湴鍧€
            auto: false,//选择文件后自动上传
            multi: true,//榛樿鍏佽閫夋嫨澶氫釜鏂囦欢
            showItemUI: true, //是否单个文件上传界面
            buttonText: '选择文件',//涓婁紶鎸夐挳涓婄殑鏂囧瓧
            removeTimeout: 1000,//涓婁紶瀹屾垚鍚庤繘搴︽潯鐨勬秷澶辨椂闂�
            itemTemplate: '<li id="${fileID}file"><div class="progress"><div class="progressbar"></div></div><span class="filename">${fileName}</span><span class="progressnum">0/${fileSize}</span><a class="uploadbtn">上传</a><a class="delfilebtn">删除</a></li>',//涓婁紶闃熷垪鏄剧ず鐨勬ā鏉�,鏈€澶栧眰鏍囩浣跨敤<li>
            onUploadStart: function () { },//涓婁紶寮€濮嬫椂鐨勫姩浣�
            onUploadSuccess: function () { },//涓婁紶鎴愬姛鐨勫姩浣�
            onUploadComplete: function () { },//涓婁紶瀹屾垚鐨勫姩浣�
            onUploadError: function () { }, //涓婁紶澶辫触鐨勫姩浣�
            onInit: function () { },//鍒濆鍖栨椂鐨勫姩浣�
        }

        var option = $.extend(defaults, opts);

        //灏嗘枃浠剁殑鍗曚綅鐢眀ytes杞崲涓篕B鎴朚B
        var formatFileSize = function (size) {
            if (size > 1024 * 1024) {
                size = (Math.round(size * 100 / (1024 * 1024)) / 100).toString() + 'MB';
            }
            else {
                size = (Math.round(size * 100 / 1024) / 100).toString() + 'KB';
            }
            return size;
        }
        //鏍规嵁鏂囦欢搴忓彿鑾峰彇鏂囦欢
        var getFile = function (index, files) {
            for (var i = 0; i < files.length; i++) {
                if (files[i].index == index) {
                    return files[i];
                }
            }
            return false;
        }
        //灏嗘枃浠剁被鍨嬫牸寮忓寲涓烘暟缁�
        var formatFileType = function (str) {
            if (str) {
                return str.split(",");
            }
            return false;
        }

        //鏂囦欢涓婁紶
        var funUploadFile = function (file) {
            var self = this;
            (function (file) {
                var xhr = new XMLHttpRequest();
                if (xhr.upload) {
                    // 涓婁紶涓�
                    xhr.upload.addEventListener("progress", function (e) {
                        self.onProgress(file, e.loaded, e.total);
                    }, false);

                    // 鏂囦欢涓婁紶鎴愬姛鎴栨槸澶辫触
                    xhr.onreadystatechange = function (e) {
                        if (xhr.readyState == 4) {
                            if (xhr.status == 200) {
                                self.onUploadSuccess(file, xhr.responseText);
                                setTimeout(function () { ZXXFILE.onDelete(file.index); }, option.removeTimeout);

                                self.onUploadComplete(xhr.response);

                            } else {
                                self.onUploadError(file, xhr.responseText);
                            }
                        }
                    };

                    var formData = new FormData();
                    formData.append("myfile", file);

                    option.onUploadStart();
                    // 寮€濮嬩笂浼�
                    xhr.open("POST", self.url, true);
                    //xhr.setRequestHeader("X_FILENAME", file.name);
                    //xhr.send(file);
                    xhr.send(formData);
                }
            })(file);


        }


        this.each(function () {
            var _this = $(this);
            //鍏堟坊鍔犱笂file鎸夐挳鍜屼笂浼犲垪琛�
            var inputstr = '<input class="uploadfile" style="visibility:hidden;" type="file" name="fileselect[]"';
            if (option.multi) {
                inputstr += 'multiple';
            }
            inputstr += '/>';
            inputstr += '<a href="javascript:void(0)" class="uploadfilebtn">';
            inputstr += option.buttonText;
            inputstr += '</a>';
            var fileInputButton = $(inputstr);
            var uploadFileList = $('<ul class="filelist"></ul>');
            _this.append(fileInputButton, uploadFileList);
            //鍒涘缓鏂囦欢瀵硅薄
            var ZXXFILE = {
                fileInput: fileInputButton.get(0),				//html file鎺т欢
                upButton: null,					//鎻愪氦鎸夐挳
                url: option.url,						//ajax鍦板潃
                fileFilter: [],					//杩囨护鍚庣殑鏂囦欢鏁扮粍
                filter: function (files) {		//閫夋嫨鏂囦欢缁勭殑杩囨护鏂规硶
                    var arr = [];
                    var typeArray = formatFileType(option.fileTypeExts);
                    if (!typeArray) {
                        for (var i in files) {
                            if (files[i].constructor == File) {
                                arr.push(files[i]);
                            }
                        }
                    }
                    else {
                        for (var i in files) {
                            if (files[i].constructor == File) {
                                if ($.inArray(files[i].type, typeArray) >= 0) {
                                    arr.push(files[i]);
                                }
                                else {
                                    alert('鏂囦欢绫诲瀷涓嶅厑璁革紒');
                                    fileInputButton.val('');
                                }
                            }
                        }
                    }
                    return arr;
                },
                //鏂囦欢閫夋嫨鍚�
                onSelect: option.onSelect || function (files) {

                    for (var i = 0; i < files.length; i++) {

                        var file = files[i];

                        if (option.showItemUI) {
                            var html = option.itemTemplate;
                            //澶勭悊妯℃澘涓娇鐢ㄧ殑鍙橀噺
                            html = html.replace(/\${fileID}/g, file.index).replace(/\${fileName}/g, file.name).replace(/\${fileSize}/g, formatFileSize(file.size));
                            uploadFileList.append(html);
                        }
                        
                        //鍒ゆ柇鏄惁鏄嚜鍔ㄤ笂浼�
                        if (option.auto) {
                            ZXXFILE.funUploadFile(file);
                        }
                    }

                    //濡傛灉閰嶇疆闈炶嚜鍔ㄤ笂浼狅紝缁戝畾涓婁紶浜嬩欢
                    if (!option.auto) {
                        _this.find('.uploadbtn').die().live('click', function () {
                            var index = parseInt($(this).parents('li').attr('id'));
                            ZXXFILE.funUploadFile(getFile(index, files));
                        });
                    }
                    //涓哄垹闄ゆ枃浠舵寜閽粦瀹氬垹闄ゆ枃浠朵簨浠�
                    _this.find('.delfilebtn').live('click', function () {
                        var index = parseInt($(this).parents('li').attr('id'));
                        ZXXFILE.funDeleteFile(index);
                    });

                },
                //鏂囦欢鍒犻櫎鍚�
                onDelete: function (index) {
                    _this.find('#' + index + 'file').fadeOut();
                },
                onProgress: function (file, loaded, total) {
                    var eleProgress = _this.find('#' + file.index + 'file .progress'), percent = (loaded / total * 100).toFixed(2) + '%';
                    eleProgress.find('.progressbar').css('width', percent);
                    if (total - loaded < 500000) { loaded = total; }//瑙ｅ喅鍥涜垗浜斿叆璇樊
                    eleProgress.parents('li').find('.progressnum').html(formatFileSize(loaded) + '/' + formatFileSize(total));
                },		//鏂囦欢涓婁紶杩涘害
                onUploadSuccess: option.onUploadSuccess,		//鏂囦欢涓婁紶鎴愬姛鏃�
                onUploadError: option.onUploadError,		//鏂囦欢涓婁紶澶辫触鏃�,
                onUploadComplete: option.onUploadComplete,		//鏂囦欢鍏ㄩ儴涓婁紶瀹屾瘯鏃�

                /* 寮€鍙戝弬鏁板拰鍐呯疆鏂规硶鍒嗙晫绾� */

                //鑾峰彇閫夋嫨鏂囦欢锛宖ile鎺т欢鎴栨嫋鏀�
                funGetFiles: function (e) {

                    // 鑾峰彇鏂囦欢鍒楄〃瀵硅薄
                    var files = e.target.files || e.dataTransfer.files;
                    //缁х画娣诲姞鏂囦欢
                    files = this.filter(files)
                    this.fileFilter.push(files);
                    this.funDealFiles(files);
                    return this;
                },

                //閫変腑鏂囦欢鐨勫鐞嗕笌鍥炶皟
                funDealFiles: function (files) {
                    var fileCount = _this.find('.filelist li').length;//闃熷垪涓凡缁忔湁鐨勬枃浠朵釜鏁�
                    for (var i = 0; i < this.fileFilter.length; i++) {
                        for (var j = 0; j < this.fileFilter[i].length; j++) {
                            var file = this.fileFilter[i][j];
                            //澧炲姞鍞竴绱㈠紩鍊�
                            file.index = ++fileCount;
                        }
                    }
                    //鎵ц閫夋嫨鍥炶皟
                    this.onSelect(files);

                    return this;
                },

                //鍒犻櫎瀵瑰簲鐨勬枃浠�
                funDeleteFile: function (index) {

                    for (var i = 0; i < this.fileFilter.length; i++) {
                        for (var j = 0; j < this.fileFilter[i].length; j++) {
                            var file = this.fileFilter[i][j];
                            if (file.index == index) {
                                this.fileFilter[i].splice(j, 1);
                                this.onDelete(index);
                            }
                        }
                    }
                    return this;
                },

                //鏂囦欢涓婁紶
                funUploadFile: function (file) {
                    var self = this;
                    (function (file) {
                        var xhr = new XMLHttpRequest();
                        if (xhr.upload) {
                            // 涓婁紶涓�
                            xhr.upload.addEventListener("progress", function (e) {
                                self.onProgress(file, e.loaded, e.total);
                            }, false);

                            // 鏂囦欢涓婁紶鎴愬姛鎴栨槸澶辫触
                            xhr.onreadystatechange = function (e) {
                                if (xhr.readyState == 4) {
                                    if (xhr.status == 200) {
                                        self.onUploadSuccess(file, xhr.responseText);
                                        setTimeout(function () { ZXXFILE.onDelete(file.index); }, option.removeTimeout);

                                        self.onUploadComplete(xhr.response);

                                    } else {
                                        self.onUploadError(file, xhr.responseText);
                                    }
                                }
                            };

                            var formData = new FormData();
                            formData.append("myfile", file );

                            option.onUploadStart();
                            // 寮€濮嬩笂浼�
                            xhr.open("POST", self.url, true);
                            //xhr.setRequestHeader("X_FILENAME", file.name);
                            //xhr.send(file);
                            xhr.send(formData);
                        }
                    })(file);


                },

                init: function () {
                    var self = this;

                    //鏂囦欢閫夋嫨鎺т欢閫夋嫨
                    if (this.fileInput) {
                        this.fileInput.addEventListener("change", function (e) { self.funGetFiles(e); }, false);
                    }

                    //_this.find('.uploadfilebtn').live('click', function () {
                    //    _this.find('.uploadfile').trigger('click');
                    //});

                    _this.find('.uploadfilebtn').click(function () {
                        _this.find('.uploadfile').trigger('click');
                    });

                    option.onInit();
                }
            };
            //鍒濆鍖栨枃浠跺璞�
            ZXXFILE.init();


        });
    }







    $.fn.easyuploader = function (opts) {

        var defaults = {
            fileTypeExts: '',//鍏佽涓婁紶鐨勬枃浠剁被鍨嬶紝濉啓mime绫诲瀷
            url: '',//鏂囦欢鎻愪氦鐨勫湴鍧€
            auto: false,//选择文件后自动上传
            multi: true,//榛樿鍏佽閫夋嫨澶氫釜鏂囦欢
            showItemUI: true, //是否单个文件上传界面
            buttonText: '选择文件',//涓婁紶鎸夐挳涓婄殑鏂囧瓧
            removeTimeout: 1000,//涓婁紶瀹屾垚鍚庤繘搴︽潯鐨勬秷澶辨椂闂�
            itemTemplate: '<li id="${fileID}file"><div class="progress"><div class="progressbar"></div></div><span class="filename">${fileName}</span><span class="progressnum">0/${fileSize}</span><a class="uploadbtn">上传</a><a class="delfilebtn">删除</a></li>',//涓婁紶闃熷垪鏄剧ず鐨勬ā鏉�,鏈€澶栧眰鏍囩浣跨敤<li>
            onUploadStart: function () { },//涓婁紶寮€濮嬫椂鐨勫姩浣�
            onUploadSuccess: function () { },//涓婁紶鎴愬姛鐨勫姩浣�
            onUploadComplete: function () { },//涓婁紶瀹屾垚鐨勫姩浣�
            onUploadError: function () { }, //涓婁紶澶辫触鐨勫姩浣�
            onInit: function () { },//鍒濆鍖栨椂鐨勫姩浣�
        }

        var option = $.extend(defaults, opts);

        //灏嗘枃浠剁殑鍗曚綅鐢眀ytes杞崲涓篕B鎴朚B
        var formatFileSize = function (size) {
            if (size > 1024 * 1024) {
                size = (Math.round(size * 100 / (1024 * 1024)) / 100).toString() + 'MB';
            }
            else {
                size = (Math.round(size * 100 / 1024) / 100).toString() + 'KB';
            }
            return size;
        }



        //鏍规嵁鏂囦欢搴忓彿鑾峰彇鏂囦欢
        var getFile = function (index, files) {
            for (var i = 0; i < files.length; i++) {
                if (files[i].index == index) {
                    return files[i];
                }
            }
            return false;
        }
        //灏嗘枃浠剁被鍨嬫牸寮忓寲涓烘暟缁�
        var formatFileType = function (str) {
            if (str) {
                return str.split(",");
            }
            return false;
        }

        var checkFile = function (file) {		//閫夋嫨鏂囦欢缁勭殑杩囨护鏂规硶
            debugger;
            var typeArray = formatFileType(option.fileTypeExts);
            if (!typeArray) {
                if (file.constructor == File) {
                    return true;
                }
            }
            else {
                if (file.constructor == File) {
                    if ($.inArray(files.type, typeArray) >= 0) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
        }


        //鏂囦欢涓婁紶
        var funUploadFile = function (file) {
            var self = this;
            (function (file) {
                var xhr = new XMLHttpRequest();
                if (xhr.upload) {
                    // 涓婁紶涓�
                    xhr.upload.addEventListener("progress", function (e) {
                        self.onProgress(file, e.loaded, e.total);
                    }, false);

                    // 鏂囦欢涓婁紶鎴愬姛鎴栨槸澶辫触
                    xhr.onreadystatechange = function (e) {
                        if (xhr.readyState == 4) {
                            if (xhr.status == 200) {
                                self.onUploadSuccess(file, xhr.responseText);
                                setTimeout(function () { ZXXFILE.onDelete(file.index); }, option.removeTimeout);

                                self.onUploadComplete(xhr.response);

                            } else {
                                self.onUploadError(file, xhr.responseText);
                            }
                        }
                    };

                    var formData = new FormData();
                    formData.append("myfile", file);

                    option.onUploadStart();
                    // 寮€濮嬩笂浼�
                    xhr.open("POST", self.url, true);
                    //xhr.setRequestHeader("X_FILENAME", file.name);
                    //xhr.send(file);
                    xhr.send(formData);
                }
            })(file);


        }

        $(this).change(function () {
            var filepath = $(this).val();
            var file = $(this).files[0];
            console.log( checkFile(file));

            console.log($(this).val());
        });
    }

})(jQuery)












