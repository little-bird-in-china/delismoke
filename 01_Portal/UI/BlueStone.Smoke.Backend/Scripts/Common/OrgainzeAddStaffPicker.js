
var OrganizeAddStaffPicker = function (extend) {
    this.selectOrganize = null;
    this.selectStaff = null;

    this.isSelectAll = true;
    this.organizeSysNo = extend.organizeSysNo,
    this.staffSysNo = extend.staffSysNo,
    this._init();
    this._initOrganize();
}

OrganizeAddStaffPicker.prototype = {

    constructor: OrganizeAddStaffPicker,

    _init: function () {
        var _self = this;
        this.selectOrganize = $("#" + this.organizeSysNo);
        this.selectStaff = $("#" + this.staffSysNo);

        this.selectOrganize.change(function () {
            _self._organizeChange();
        })
    },

    _initOrganize: function () {
        var _self = this;
        $.ajax({
            url: "/Organize/GetAllOrganize",
            type: "GET",
            dataType: "json",
            success: function (result) {
                if (result.Success) {
                    var selAll = _self.selectOrganize.attr("isselectall");
                    if (selAll && selAll.toLowerCase() == "true") {
                        _self.isSelectAll = true;
                    } else {
                        _self.isSelectAll = false;
                    }
                    var selValue = $.trim(_self.selectOrganize.attr("selectvalue"));
                    if (_self.isSelectAll) {
                        _self.selectOrganize.empty();
                        _self.selectOrganize.append("<option value='0' selected='selected'>" + "选择部门" + "</option>");
                        _self.selectStaff.append("<option value='0' selected='selected'>" + "选择人员" + "</option>");
                         _self.selectOrganize.prev().find(".select2-chosen").text("选择部门");
                        //if (selValue == "" || selValue == 0) {
              
                        //    _self.selectStaff.prev().find(".select2-chosen").text("选择人员");
                        //}
                    }

                    var selectData = null;
                    for (var i = 0; i < result.Data.length; i++) {
                        var data = result.Data[i];
                        if ((selValue == "" || selValue == 0) && !_self.isSelectAll && i == 0) {
                            selectData = data;
                            _self.selectOrganize.append($("<option selected='selected'></option>").val(data.SysNo).html(data.Name));
                        } else {
                            if (data.SysNo == selValue) {
                                selectData = data;
                                _self.selectOrganize.append($("<option selected='selected'></option>").val(data.SysNo).html(data.Name));
                            } else {
                                _self.selectOrganize.append($("<option></option>").val(data.SysNo).html(data.Name));
                            }
                        }
                    }
                    if (selectData && selectData != null && selectData.SysNo > 0) {
                        _self.selectOrganize.prev().find(".select2-chosen").text(selectData.Name);
                    }

                    _self._organizeChange();
                }
            }
        });
    },

    _organizeChange: function () {
        var _self = this;
        _self.selectStaff.empty();
        if (_self.isSelectAll) {
            _self.selectStaff.append("<option value='0' selected='selected'>" + "选择人员" + "</option>");
            _self.selectStaff.prev().find(".select2-chosen").text("选择人员");
        }
        if (_self.selectOrganize.val() != "0") {
            $.ajax({
                url: "/Organize/GetStaffByOrganizeSysNo?organizeSysNo=" + $("#" + this.organizeSysNo).val(),
                type: "GET",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        _self.selectStaff.empty();
                        if (_self.isSelectAll) {
                            _self.selectStaff.append("<option value='0' selected='selected'>" + "选择人员" + "</option>");
                            _self.selectStaff.prev().find(".select2-chosen").text("选择人员");
                        }
                        var selectData = null;
                        var selValue = $.trim(_self.selectStaff.attr("selectvalue"));
                        for (var i = 0; i < result.Data.length; i++) {
                            var data = result.Data[i];
                            if (data.UserSysNo == selValue) {
                                selectData = data;
                                _self.selectStaff.append($("<option selected='selected'></option>").val(data.UserSysNo).html(data.Name));
                                _self.selectStaff.prev().find(".select2-chosen").text(data.Name);
                            } else {
                                _self.selectStaff.append($("<option></option>").val(data.UserSysNo).html(data.Name));
                            }
                        }

                        if (!_self.isSelectAll && (selectData == null || selectData.SysNo <= 0)) {
                            _self.selectStaff.find("option").eq(0).attr("selected", true);
                            _self.selectStaff.prev().find(".select2-chosen").text(result.Data[0].Name);
                        }
                    }
                }
            });
        }
    },

}
