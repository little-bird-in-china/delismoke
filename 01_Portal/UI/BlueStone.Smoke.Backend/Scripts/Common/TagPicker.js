var TagHandler = {
    masterSysNo: 0,
    dataSysNo: 0,
    masterName: "",
    LoadData: function () {
        var masterName = TagHandler.masterName;
        var masterSysNo = TagHandler.masterSysNo;
        var html = " <a id='showAddTagModel' style='margin-left:5px' onclick='TagHandler.ShowTagListInfo(" + masterSysNo + "," + '"' + masterName + '"' + ")'>添加</a>";
        var taglist = "";
        $.ajax({
            url: "/Tag/GetTagListByMasterSysNo?masterSysNo=" + masterSysNo,
            type: "GET",
            dataType: "json",
            success: function (result) {
                if (result.Success) {
                    if (result.Data != null && result.Data.length > 0) {
                        for (var i = 0; i < result.Data.length; i++) {
                            var data = result.Data[i];
                            taglist = taglist + "<label   style='margin-left:5px' class='btn btn- sm label_tagLabel' sysno=" + data.SysNo + " style='margin- left:3px; border - color:#357ebd' onclick='$(this).remove()'>" + data.Name + "  X" + "</label>"
                        }
                    };
                    html = taglist + html;
                    $("#tagList").append(html);
                } else {
                    $("#tagList").append(html);
                }
            }
        });
    },
    Init: function (masterSysNo, masterName) {
        TagHandler.masterSysNo = masterSysNo;
        TagHandler.masterName = masterName;
        TagHandler.LoadData();
    },
    ShowTagListInfo: function (masterSysNo, masterName) {
        var pageData = TagHandler.GetPageTagList();
        var pageDataString = JSON.stringify(pageData);
        showModal("/Tag/TagPicker?masterSysNo=" + masterSysNo + "&MasterName=" + masterName + "&pd=" + pageDataString, function (data) {
            var taglist = "";
            var html = " <a id='showAddTagModel' onclick='TagHandler.ShowTagListInfo(" + masterSysNo + "," + '"' + masterName + '"' + ")'>添加</a>";
            if (data) {
                if (data != null && data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        var info = data[i];
                        taglist = taglist + "<label style='margin-left:5px' class='btn btn- sm label_tagLabel' sysno=" + info.SysNo + " style='margin- left:3px; border - color:#357ebd' onclick='$(this).remove()'>" + info.Name + "  X" + "</label>"
                    }
                };
                html = taglist + html;
                $("#tagList").html("");
                $("#tagList").append(html);
            }
        });
    },
    DeleteTag: function (obj) {
        var sysno = $(obj).attr("sysNo");
        $.ajax({
            url: "/Tag/DeleteTag",
            type: "GET",
            dataType: "json",
            data: { SysNo: sysno },
            success: function (result) {
                if (result.Success) {
                    $(obj).parent().remove();
                }
            }
        });
    },
    SaveTag: function () {
        var tagName = $("#TagName").val();
        $.ajax({
            url: "/Tag/SaveTag",
            type: "GET",
            dataType: "json",
            data: { tagName: tagName, masterName: TagHandler.masterName },
            success: function (result) {
                if (result.Success) {
                    if (result.Data != null && result.Data.length > 0) {
                        var html = "";
                        for (var i = 0; i < result.Data.length; i++) {
                            var data = result.Data[i];
                            var checked = "";
                            if (data.IsSelect == 1) {
                                checked = "checked='checked'";
                            };
                            html = html + "<label style='margin- left:5px' class='select_tag'  tagName='" + data.Name + "'>" +
                                "<input class='sysno' type='checkbox'" + checked + "value='" + data.SysNo + "'/>" +
                                "<span class='text name' style='text-align:left'>" + data.Name +
                                "</span>" +
                                "<span sysno='" + data.SysNo + "' onclick='TagHandler.DeleteTag(this)' style='color:red'>" + "     X" +
                                "</span></label>";
                        }
                        $("#tagDataList").append(html);
                    };
                }
            }
        });
    },
    SaveModelTag: function () {
        var selectedData = [];
        $(".select_tag").each(function () {
            var $this = $(this);
            var isSelect = $this.find("input[type = 'checkbox']").prop("checked") == true ? 1 : 0;
            if (isSelect == "1") {
                selectedData.push({
                    SysNo: $.trim($this.find('input.sysno').val()),
                    Name: $this.attr("tagName")
                })
            }
        })
        hideModal(selectedData);
    },
    GetPageTagList: function () {
        var data = [];
        $("#tagList").find("label.label_tagLabel").each(function () {
            var $this = $(this);
            data.push({
                MasterName: TagHandler.masterName,
                MasterSysNo: TagHandler.dataSysNo,
                TagSysNo: $this.attr("sysno")
            })
        });
        return data;
    },
    SaveTagRelationInfo: function (dataSysNo) {
        var data = {
            TagRelationList: [],
            MasterSysNo: dataSysNo
        };
        TagHandler.dataSysNo = dataSysNo;
         data.TagRelationList = TagHandler.GetPageTagList();
        $.ajax({
            url: "/Tag/SaveTagRelationInfo",
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: $.toJSON(data),
            success: function (result) {
            }
        });
    }
};