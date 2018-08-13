var fileInfoManager = {
    CategoryName: {
        RecommandBannerImage: "RecommandBannerImage",/*推荐广告图片*/
    },

    saveFileInfo: function (masterType, masterID, filePath, fileName, size, index, categoryName, isSingle, saveed) {
        var file = {
            MasterType: masterType,
            MasterID: masterID,
            FileRelativePath: filePath,
            FileName: fileName,
            Size: size,
            Priority: index,
            CategoryName: categoryName,
            IsSingle: isSingle == null || isSingle//是否单文件
        };
        $.ajax({
            url: "/Common/SaveFileInfo",
            type: "POST",
            dataType: "json",
            data: { Data: encodeURI(JSON.stringify(file)) },
            success: function (data) {
                if (data.Success) {
                    file.SysNo = data.Data;
                    if (saveed !== null) {
                        saveed(file);
                    }
                }
            }
        });
    },

    batchSaveFileInfo: function (files,callback) {
        if (files && files != null && files.length > 0) {
            $.ajax({
                url: "/Common/BatchSaveFileInfo",
                type: "POST",
                dataType: "json",
                contentType: "application/json",
                data: $.toJSON(files),
                success: function (data) {
                    if (data.Success) {
                        if (callback !== null) {
                            callback();
                        }
                    }
                }
            });
        }
    },
    download: function (object) {
        var sysNo = $(object).attr("data-key");
        window.location.href = "/Common/DownloadRemoteFile?sysNo=" + sysNo;
    }
}