
var AreaPicker = function (extend) {
    this.selectProvince = null;
    this.selectCity = null;
    this.selectDistrict = null;
    this.isSelectAll = true;
    this.provinceID = extend.provinceID,
    this.cityID = extend.cityID ,
    this.regionID = extend.regionID,
    this._init();
    this._initProvince();
}

AreaPicker.prototype = {

    constructor: AreaPicker,

    _init: function () {
        var _self = this;
        this.selectProvince = $("#" + this.provinceID);
        this.selectCity = $("#" + this.cityID);
        this.selectDistrict = $("#" + this.regionID);

        this.selectProvince.change(function () {
            _self._provinceChange();
        })

        this.selectCity.change(function () {
            _self._cityChange();
        });
    },

    _initProvince: function () {
        var _self = this;
        $.ajax({
            url: "/Index/GetProvince",
            type: "GET",
            dataType: "json",
            success: function (result) {
                if (result.Success) {
                    var selAll = _self.selectProvince.attr("isselectall");
                    if (selAll && selAll.toLowerCase() == "true") {
                        _self.isSelectAll = true;
                    } else {
                        _self.isSelectAll = false;
                    }
                    var selValue = $.trim(_self.selectProvince.attr("selectvalue"));
                    if (_self.isSelectAll) {
                        _self.selectProvince.append("<option value='0' selected='selected'>" + "选择省份" + "</option>");
                        _self.selectCity.append("<option value='0' selected='selected'>" + "选择城市" + "</option>");
                        _self.selectDistrict.append("<option value='0' selected='selected'>" + "选择地区" + "</option>");
                        if (selValue == "" || selValue == 0) {
                            _self.selectProvince.prev().find(".select2-chosen").text("选择省份");
                            _self.selectCity.prev().find(".select2-chosen").text("选择城市");
                            _self.selectDistrict.prev().find(".select2-chosen").text("选择地区");
                        }
                    }

                    var selectData = null;
                    for (var i = 0; i < result.Data.length; i++) {
                        var data = result.Data[i];
                        if ((selValue == "" || selValue == 0) && !_self.isSelectAll && i == 0) {
                            selectData = data;
                            _self.selectProvince.append($("<option selected='selected'></option>").val(data.SysNo).html(data.ProvinceName));
                        } else {
                            if (data.SysNo == selValue) {
                                selectData = data;
                                _self.selectProvince.append($("<option selected='selected'></option>").val(data.SysNo).html(data.ProvinceName));
                            } else {
                                _self.selectProvince.append($("<option></option>").val(data.SysNo).html(data.ProvinceName));
                            }
                        }
                    }
                    if (selectData && selectData != null && selectData.SysNo > 0) {
                        _self.selectProvince.prev().find(".select2-chosen").text(selectData.ProvinceName);
                    }

                    _self._provinceChange();
                }
            }
        });
    },

    _provinceChange: function () {
        var _self = this;
        _self.selectCity.empty();
        _self.selectDistrict.empty();
        if (_self.isSelectAll) {
            _self.selectCity.append("<option value='0' selected='selected'>" + "选择城市" + "</option>");
            _self.selectDistrict.append("<option value='0' selected='selected'>" + "选择地区" + "</option>");
            _self.selectCity.prev().find(".select2-chosen").text("选择城市");
            _self.selectDistrict.prev().find(".select2-chosen").text("选择地区");
        }
        if (_self.selectProvince.val() != "0") {
            

            $.ajax({
                url: "/Index/GetCity?provinceSysNo=" + $("#" + this.provinceID).val(),
                type: "GET",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        var selectData = null;
                        var selValue = $.trim(_self.selectCity.attr("selectvalue"));
                        for (var i = 0; i < result.Data.length; i++) {
                            var data = result.Data[i];
                            if (data.SysNo == selValue) {
                                selectData = data;
                                _self.selectCity.append($("<option selected='selected'></option>").val(data.SysNo).html(data.CityName));
                                _self.selectCity.prev().find(".select2-chosen").text(data.CityName);
                            } else {
                                _self.selectCity.append($("<option></option>").val(data.SysNo).html(data.CityName));
                            }
                        }
                        if (!_self.isSelectAll && (selectData == null || selectData.SysNo <= 0)) {
                            _self.selectCity.find("option").eq(0).attr("selected", true);
                            _self.selectCity.prev().find(".select2-chosen").text(result.Data[0].CityName);
                        }
                        _self._cityChange();
                    }
                }
            });
        }
    },

    _cityChange: function () {
        var _self = this;
        _self.selectDistrict.empty();
        if (_self.isSelectAll) {
            _self.selectDistrict.append("<option value='0' selected='selected'>" + "选择地区" + "</option>");
            _self.selectDistrict.prev().find(".select2-chosen").text("选择地区");
        }
        if (_self.selectCity.val() != "0") {
            
            $.ajax({
                url: "/Index/GetDistrict?citySysNo=" + $("#" + this.cityID).val(),
                type: "GET",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        var selectData = null;
                        var selValue = $.trim(_self.selectDistrict.attr("selectvalue"));
                        for (var i = 0; i < result.Data.length; i++) {
                            var data = result.Data[i];
                            if (data.SysNo == selValue) {
                                selectData = data;
                                _self.selectDistrict.append($("<option selected='selected'></option>").val(data.SysNo).html(data.DistrictName));
                                _self.selectDistrict.prev().find(".select2-chosen").text(data.DistrictName);
                            } else {
                                _self.selectDistrict.append($("<option></option>").val(data.SysNo).html(data.DistrictName));
                            }
                        }

                        if (!_self.isSelectAll && (selectData == null || selectData.SysNo <= 0)) {
                            _self.selectDistrict.find("option").eq(0).attr("selected", true);
                            _self.selectDistrict.prev().find(".select2-chosen").text(result.Data[0].DistrictName);
                        }
                    }
                }
            });
        }
    }
}
