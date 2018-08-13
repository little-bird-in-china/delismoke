

var TopicMaintainHandler = {
    SysNo: 0,
    TopicSysNo: 0,
    TopicContentEditor: null,
    //初始化html编辑器
    InitEditor: function () {
        UEDITOR_CONFIG.zIndex = 1;
        TopicMaintainHandler.TopicContentEditor = new UE.ui.Editor();
        TopicMaintainHandler.TopicContentEditor.render('TopicContentEditor');
    },
    //保存文章信息
    Save: function () {
        var topicInfo = $.buildEntity($("#TopicMaintainForm"));
        topicInfo.Content = TopicMaintainHandler.TopicContentEditor.getContent();
        //topicInfo.DefaultImage = $("#DefaultImage").val();
        var newsTitle = $("#Title").val().trim();
        topicInfo.TopicCategorySysNo = $("#TopicCategorySysNo").val()
        if (newsTitle.length <= 0) {
            $.showError('标题不能为空！');
            return false;
        }
        //var subTitle = $("#SubTitle").val().trim();
        //if (subTitle.length <= 0) {
        //    $.showError('副标题不能为空！');
        //    return false;
        //}
        var topicCategorySysNo = $("#TopicCategorySysNo").val().trim();
        if (parseInt(topicCategorySysNo) <= 0) {
            $.showError('类别不能为空！');
            return false;
        }
        var content = topicInfo.Content.replace("<p>", "").replace("</p>", "").trim();
        if (_htmlContent == topicInfo.TopicContentType && content.length <= 0) {
            $.showError('内容不能为空！');
            return false;
        }
        if ($("#CbxIsRed").prop("checked")) {
            topicInfo.IsRed = 1;
        }
        if ($("#CbxIsTop").prop("checked")) {
            topicInfo.IsTop = 1;
        }
        //var startDate = $("#txtStartTime").val();
        //var endDate = $("#txtEndTime").val();
        //if (startDate != null && startDate != "" && endDate != null && endDate != "") {
        //    if (startDate > endDate) {
        //        $.showError('开始时间不能大于结束时间！');
        //        return false;
        //    }
        //}
        //if (endDate != null && endDate != "") {
        //    endDate = endDate + ' 23:59:59';
        //    topicInfo.EndTime = endDate;
        //}
        //$.confirm("您确定要保存文章信息吗？", function (result) {
        //    if (!result)
        //        return;
        $.ajax({
            url: "/Topic/AjaxSaveTopicInfo",
            //type: "POST",
            //dataType: "json",
            //data: { data: encodeURI(JSON.stringify(topicInfo)) },
            type: "POST",
            dataType: "json",
            data: topicInfo,
            beforeSend: function () {
                $("#btnSave").hide();
                $("#btnSaving").show();
            },
            error: function () {
                $("#btnSave").show();
                $("#btnSaving").hide();
            },
            success: function (data) { 
                if (!data.Message) {
                    $.showSuccess('保存文章信息成功！', function () {
                        if ($("#SysNo").val() == 0) {
                            //TopicMaintainHandler.updateFileMasterID(data)
                            //setTimeout(function () {
                                location.href = 'Maintain?sysno=' + data;
                            //}, 100);

                        } else {
                            $("#btnSaving").hide();
                            $("#btnSave").show();
                            location.reload();
                        }
                    });
                }
                else {
                    $("#btnSave").show();
                    $("#btnSaving").hide();
                }
            }
        });
    },
    PublishTopic: function () {
        $.confirm("您确定要发布文章吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "/Topic/AjaxPublishTopic?SysNo=" + $("input[data-model=SysNo]").val(),
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (!data.Message) {
                        $.showSuccess('发布文章成功！', function () {
                            location.reload();
                        });
                    }
                }
            });
        });
    },
    //撤下文章
    OfflineTopic: function () {
        $.confirm("您确定要撤下文章吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "/Topic/AjaxOfflineTopic?SysNo=" + $("input[data-model=SysNo]").val(),
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (!data.Message) {
                        $.showSuccess('撤下文章成功！', function () {
                            location.reload();
                        });
                    }
                }
            });
        });
    },
    //作废文章
    AbondonTopic: function () {
        $.confirm("您确定要作废文章吗？", function (result) {
            if (!result)
                return;

            $.ajax({
                url: "/Topic/AjaxAbondonTopic?SysNo=" + $("input[data-model=SysNo]").val(),
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (!data.Message) {
                        $.showSuccess('作废文章成功！', function () {
                            location.reload();
                        });
                    }
                }
            });
        });
    },
    getContentFileOption: function () {
        var type = 4;//parseInt($("#selectContentType").val());
        var extensions = ['jpg', 'png', 'gif', 'bmp'];
        var dropZoneTitle = '拖拽文件到这里 &hellip;<br />支持多文件同时上传<br />上传的图片的像素比为：2048*1372<br />单文件大小不要超过4096kb';
        var maxFileSize = 4096;
        var maxFileCount = 40;
        switch (type) {
            case 2:
                extensions = ['doc', 'docx', 'xls', 'xlsx', 'ppt', 'pptx', 'pdf', 'txt', 'rar', 'zip'];
                dropZoneTitle = "拖拽文件到这里 &hellip;<br />单文件大小不要超过20480kb=20M<br />支持后缀为.doc/.docx/.xls/.xlsx/.ppt/.pptx/.txt/.rar/.zip/.pdf的文件"
                maxFileSize = 20480;
                maxFileCount = 1;
                break;
            case 3:
                extensions = ['mp4', 'webm', 'ogg'];
                dropZoneTitle = "拖拽视频到这里 &hellip;<br />单视频大小不要超过500M<br />仅支持.MP4格式"
                maxFileSize = 530000;// 512M
                maxFileCount = 1;
                break;
            default: break;
        }
        return {
            allowedFileExtensions: extensions,
            maxFileSize: maxFileSize,
            maxFileCount: maxFileCount,
            dropZoneTitle: dropZoneTitle
        };
    },
    contentFileConfigList: [],
    TotalSelectNum: 0,//选择的总数
    iniDefaultImageUploader: function () {
        var url = GlobalDefine.ImageStorageServerDomain
        var viewUrls = [];
        var initialPreviewConfig = [];
        if (TopicMaintainHandler.topicDefaultImage != "") {
            url = url + TopicMaintainHandler.topicDefaultImage;
            viewUrls = [url];
            initialPreviewConfig.push({ downloadUrl: url, width: "120px", key: -1 })
        }
        var DefaultImageOption = {
            language: 'zh',
            uploadUrl: GlobalDefine.GetImageUploadServerDomain("topic"), // you must set a valid URL here else you will get an error
            deleteUrl: TopicMaintainHandler.deleteFileUrl,
            allowedFileExtensions: ['jpg', 'png', 'gif', 'bmp'],
            uploadAsync: true,
            overwriteInitial: true,
            //showCaption: false,
            showClose: false,
            initialPreview: viewUrls,
            initialPreviewAsData: true,
            // zoomModalHeight: 300,
            initialPreviewConfig: initialPreviewConfig,
            maxFileSize: 1024,
            maxFileCount: 1,
            dropZoneTitle: '拖拽文件到这里 &hellip;<br />上传的图片宽高比为3:1<br />单文件大小不要超过1MB',
            fileActionSettings: {
                showZoom: true,
                showUpload: false,
                showDrag: false
            },
            slugCallback: function (filename) {
                return filename.replace('(', '_').replace(']', '_');
            },
        };
        $("#topicFile_DefaultImage").fileinput(DefaultImageOption);
        $('#topicFile_DefaultImage').on('fileuploaded', function (event, data, previewId, index) {
            var ele = $(this);
            var result = data.response;
            if (result.state != "SUCCESS") {
                $.showError(result.message);
            } else {
                $("#txtDefaultImage").val(result.url);
                var initConfig = [];
                initConfig.push({ downloadUrl: result.url, width: "120px", key: -1 });
                DefaultImageOption.initialPreview = [GlobalDefine.ImageStorageServerDomain + result.url];
                DefaultImageOption.initialPreviewConfig = initConfig;
                DefaultImageOption.contentFileConfigList = initConfig;
                setTimeout(function () {
                    ele.fileinput(DefaultImageOption);
                    ele.fileinput("refresh", DefaultImageOption);
                }, 1000);
            }
        }).on("filedeleted", function () {
            $("#txtDefaultImage").val("");
            }).on("filebatchselected", function (event, files) {
                if ($("#topicDefaultImageContainer").find("[class='file-input has-error']").length === 0) {
                    $(this).fileinput("upload");
                }
            });
    },
    conentFileUploader: null,
    iniConentFileUploader: function () {
        var burl = GlobalDefine.ImageStorageServerDomain
        var viewUrls = [];
        var initialPreviewConfig = [];
        if (TopicMaintainHandler.contentFileList != null && TopicMaintainHandler.contentFileList.length > 0) {
            for (var i = 0; i < TopicMaintainHandler.contentFileList.length; i++) {
                var f = TopicMaintainHandler.contentFileList[i];
                var url = burl + f.FileRelativePath;
                viewUrls.push(url);
                initialPreviewConfig.push({ caption: f.FileName, filename: f.FileName, downloadUrl: url, size: f.Size, width: "120px", key: f.SysNo });
            }
            //TopicMaintainHandler.TotalSelectNum = TopicMaintainHandler.contentFileList.length;
        }
        TopicMaintainHandler.contentFileConfigList = initialPreviewConfig;

        var extensions = ['jpg', 'png', 'gif', 'bmp'];
        var dropZoneTitle = '拖拽文件到这里 &hellip;<br />支持多文件同时上传<br />上传的图片的像素比为：2048*1372<br />单文件大小不要超过4096kb';
        var maxFileSize = 4096;
        var option = TopicMaintainHandler.getContentFileOption();
        option = $.extend({
            language: 'zh',
            uploadUrl: GlobalDefine.GetImageUploadServerDomain("topic"), // you must set a valid URL here else you will get an error
            deleteUrl: TopicMaintainHandler.deleteFileUrl,
            allowedFileExtensions: extensions,
            uploadAsync: true,
            showClose: false,
            overwriteInitial: false,
            maxFileSize: maxFileSize,
            maxFileCount: 20,
            dropZoneTitle: dropZoneTitle,
            initialPreview: viewUrls,
            initialPreviewConfig: initialPreviewConfig,
            initialPreviewAsData: true,
            initialPreviewFileType: 'image',
            fileActionSettings: {
                showZoom: true,
                showDrag: true,
                showUpload: false
            },
            slugCallback: function (filename) {
                return filename.replace('(', '_').replace(']', '_');
            },
            showPreview: true,
            previewFileIcon: '<i class="fa fa-file"></i>',
            allowedPreviewTypes: null,
            previewFileIconSettings: { // configure your icon file extensions
                'doc': '<i class="fa fa-file-word-o text-primary"></i>',
                'xls': '<i class="fa fa-file-excel-o text-success"></i>',
                'ppt': '<i class="fa fa-file-powerpoint-o text-danger"></i>',
                'pdf': '<i class="fa fa-file-pdf-o text-danger"></i>',
                'zip': '<i class="fa fa-file-archive-o text-muted"></i>',
                'htm': '<i class="fa fa-file-code-o text-info"></i>',
                'txt': '<i class="fa fa-file-text-o text-info"></i>',
                'mov': '<i class="fa fa-file-movie-o text-warning"></i>',
                'mp3': '<i class="fa fa-file-audio-o text-warning"></i>',
                // note for these file types below no extension determination logic 
                // has been configured (the keys itself will be used as extensions)
                'jpg': '<i class="fa fa-file-photo-o text-danger"></i>',
                'gif': '<i class="fa fa-file-photo-o text-warning"></i>',
                'png': '<i class="fa fa-file-photo-o text-primary"></i>'
            },
            previewFileExtSettings: { // configure the logic for determining icon file extensions
                'doc': function (ext) {
                    return ext.match(/(doc|docx)$/i);

                },
                'xls': function (ext) {
                    return ext.match(/(xls|xlsx)$/i);

                },
                'ppt': function (ext) {
                    return ext.match(/(ppt|pptx)$/i);

                },
                'zip': function (ext) {
                    return ext.match(/(zip|rar|tar|gzip|gz|7z)$/i);

                },
                'htm': function (ext) {
                    return ext.match(/(htm|html)$/i);
                },
                'txt': function (ext) {
                    return ext.match(/(txt|ini|csv|java|php|js|css)$/i);

                },
                'mov': function (ext) {
                    return ext.match(/(avi|mpg|mkv|mov|mp4|3gp|webm|wmv)$/i);

                },
                'mp3': function (ext) {
                    return ext.match(/(mp3|wav)$/i);

                },
            }

        }, option);
        TopicMaintainHandler.conentFileUploader = $("#topicFile_Content").fileinput(option);
        $('#topicFile_Content').on('fileuploaded', function (event, data, previewId, index) {
            var result = data.response;
            if (result.state != "SUCCESS") {
                $.showError(result.message);
            } else {
                var fileSize = data.files[index].size
                TopicMaintainHandler.saveFileInfo(result.url, true, result.original, fileSize, index, function (f) {
                    var initialPreviewConfig = TopicMaintainHandler.contentFileConfigList;
                    var url = burl + f.FileRelativePath;
                    for (var i = initialPreviewConfig.length - 1; i >= 0; i--) {
                        if (initialPreviewConfig[i] == null) {
                            initialPreviewConfig.splice(i, 1);
                        }
                    }
                    initialPreviewConfig.push({ caption: f.FileName, filename: f.FileName, downloadUrl: url, size: f.Size, width: "120px", key: f.SysNo });
                    viewUrls = [];
                    for (var i = 0; i < initialPreviewConfig.length; i++) {
                        var fi = initialPreviewConfig[i]
                        viewUrls.push(fi.downloadUrl);
                    }
                    option.initialPreview = viewUrls;
                    option.initialPreviewConfig = initialPreviewConfig;
                    TopicMaintainHandler.contentFileConfigList = initialPreviewConfig;
                    if (TopicMaintainHandler.contentFileConfigList.length === TopicMaintainHandler.TotalSelectNum) {
                        $("#topicFile_Content").fileinput("destroy");
                        $("#topicFile_Content").fileinput(option);
                    }
                    // setTimeout(() => {
                    // $("#topicFile_Content").fileinput("refresh", option);
                    //  }, 200);
                });
            }
        }).on('fileremoved', function (event, id, index) {
            var contentlength = $(".file-preview-thumbnails").children(".file-preview-frame,.krajee-default,.kv-preview-thumb").length - 1;
            TopicMaintainHandler.TotalSelectNum = contentlength;
        }).on('filebatchselected', function (event, files) {
            var contentlength = $(".file-preview-thumbnails").children(".file-preview-frame,.krajee-default,.kv-preview-thumb").length;
            TopicMaintainHandler.TotalSelectNum = contentlength;
        }).on('filesorted', function (event, params) {
            TopicMaintainHandler.contentFileConfigList = params.stack;
        }).on("filedeleted", function (event, key, jqXHR, data) {
            var contentlength = $(".file-preview-thumbnails").children(".file-preview-frame,.krajee-default,.kv-preview-thumb").length - 1;
            TopicMaintainHandler.TotalSelectNum = contentlength;
            var initialPreviewConfig = TopicMaintainHandler.contentFileConfigList;
            for (var i = 0; i < initialPreviewConfig.length; i++) {
                var fi = initialPreviewConfig[i];
                if (fi == null || fi.key == key) {
                    initialPreviewConfig.splice(i, 1);
                    break;
                }
            }
            viewUrls = [];
            for (var i = 0; i < initialPreviewConfig.length; i++) {
                var fi = initialPreviewConfig[i]
                viewUrls.push(fi.downloadUrl);
            }
            TopicMaintainHandler.contentFileConfigList = initialPreviewConfig;
            option.initialPreview = viewUrls;
            option.initialPreviewConfig = initialPreviewConfig;
            $("#topicFile_Content").fileinput("destroy");
            $("#topicFile_Content").fileinput(option);
        });
    },
    saveFileInfo: function (filePath, isImage, fileName, size, index, saveed) {
        var topicSysNo = $("#SysNo").val()
        if (topicSysNo == "" || topicSysNo <= 0) {
            topicSysNo = TopicMaintainHandler.tempFileID;
        }
        var file = {
            MasterType: TopicMaintainHandler.topicFileMasterType,
            MasterID: topicSysNo,
            FileRelativePath: filePath,
            FileName: fileName,
            Size: size,
            Priority: index,
        };
        $.ajax({
            url: TopicMaintainHandler.saveFileInfoUrl,
            type: "POST",
            dataType: "json",
            data: { Data: encodeURI(JSON.stringify(file)) },
            success: function (data) {
                if (data.Success) {
                    $.showSuccess("上传成功");
                    file.SysNo = data.Data;
                    saveed(file);
                }
            }
        });
    },
   
    ini: function () {
        TopicMaintainHandler.InitEditor();
        TopicMaintainHandler.TopicContentEditor.addListener('ready', function (editor) {
            var type = 4;//parseInt($("#selectContentType").val());
            if (type == 4) {
                $("#fileContentContainer").hide();
                $("#htmlContentContainer").show();
            }
            else {
                $("#fileContentContainer").show();
                $("#htmlContentContainer").hide();
            }
        });
        $("#btnSave").click(function () {
            $("#TopicMaintainForm").bootstrapValidator('validate');
            TopicMaintainHandler.Save();
        });
        $("#btnPublish").click(function () {
            TopicMaintainHandler.PublishTopic();
        });
        $("#btnOffline").click(function () {
            TopicMaintainHandler.OfflineTopic();
        });
        $("#btnAbondon").click(function () {
            TopicMaintainHandler.AbondonTopic();
        });

        if (TopicMaintainHandler.topicContent != "") {
            TopicMaintainHandler.TopicContentEditor.addListener("ready", function () {
                TopicMaintainHandler.TopicContentEditor.setContent(TopicMaintainHandler.topicContent);
            });
        }
        TopicMaintainHandler.iniDefaultImageUploader();
        TopicMaintainHandler.iniConentFileUploader();
        $(function () {
            var type = 4;//parseInt($("#selectContentType").val());
            TopicMaintainHandler.setContentTypeText(type)
            $("#selectContentType").change(function () {
                TopicMaintainHandler.setContentTypeText(type)

                if (type == 4) {
                    $("#fileContentContainer").hide();
                    $("#htmlContentContainer").show();
                }
                else {
                    $("#fileContentContainer").show();
                    $("#htmlContentContainer").hide();
                    var option = TopicMaintainHandler.getContentFileOption();
                    $("#topicFile_Content").fileinput("refresh", option);
                }

            });
        });
    },
    setContentTypeText: function (type) {
        if (type == 1) {
            $("#contentTypeText").text("图片：");
        } else if (type == 2) {
            $("#contentTypeText").text("文件：");
        } else if (type == 3) {
            $("#contentTypeText").text("视频：");
        }
    }
}
