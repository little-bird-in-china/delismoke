/*  KODataTable Create By Gavin 2016.09.01

构造参数:
    containerId - 容器id, ajaxUrl - 远程数据地址

开放方法:
    SetTableColumns(columns)        设置列头
    ReLoad(param)                   加载数据 
    GetSelectRows()                 获取选中行的数据
    GetAllRows()                    获取当前表格页所有数据
    GetRowByIndex(rowindex)         根据行号获取数据

方法参数:
    columns:                        列头数据定义
        cText,列头显示名称
        cName,列json键名称
        cWidth,列宽度
        cVisible,是否可见
        cType,数据列data,自定义列html
        cRender,自定义html函数

    param:                           查询参数,object

    rowindex:                        cType为html, cRender: function (row, rowindex) {} cRender函数的第二个参数

*/
function KODataTable(containerId, ajaxUrl) {
    var jsontable = this;
    function GridView() {
        var vm = this;
        vm.filter = ko.mapping.fromJS({
            PageIndex: 0,
            PageSize: 10
        });
        //行数据
        vm.data = ko.observableArray();
        //行起始数
        vm.StartIndex = ko.computed(function () {
            return vm.filter.PageIndex() * vm.filter.PageSize();
        }, vm);
        //总记录
        vm.total = ko.observable(0);
        //总页数
        vm.totalPage = ko.computed(function () {
            return Math.ceil(vm.total() / vm.filter.PageSize());
        }, vm);
        //每页数据Size
        vm.pageSizeSelect = ko.observableArray([10, 25, 50, 100, 150]);
        //是否显示CheckBox
        vm.showCheckBox = ko.observable(true);
        //设置表格列头信息
        vm.columns = ko.observableArray();
        //全选CheckBox
        vm.allSelected = ko.computed({
            read: function () {
                var _allSelected = true;
                for (var i = 0; i < vm.data().length; i++) {
                    if (vm.data()[i]._isSelected() == false) {
                        _allSelected = false;
                        break;
                    }
                }
                if (vm.data().length == 0) { _allSelected = false; };
                return _allSelected;
            },
            write: function (value) {
                for (var i = 0; i < vm.data().length; i++) {
                    vm.data()[i]._isSelected(value)
                }
            },
            owner: vm
        });
        vm.allCheckBoxClick = function () {
            vm.allSelected(!vm.allSelected());
        }
        //行点击事件
        vm.rowClick = function (rowdata, event) {
            if (event.target.tagName != 'BUTTON' && event.target.tagName != 'A') {
                rowdata._isSelected(!rowdata._isSelected());
            } else if (event.target.tagName == 'A') {
                return true;
            }
        };

        var current_query = {};

        //远程加载数据
        vm.ReLoad = function (param) {
            var pageinfo = {
                PageSize: vm.filter.PageSize(),
                PageIndex: vm.filter.PageIndex()
            };
            if (param == null) { param = {}; };
            current_query = param;
            $.extend(param, pageinfo);

            //构建排序条件
            for (var i = 0; i < vm.columns().length; i++) {
                var cur_column = vm.columns()[i];
                if (cur_column.cSortable && cur_column.cIsSorting()) {
                    if (cur_column.cSortAscending()) {
                        param.SortFields = cur_column.cSortName + " ASC";
                    } else {
                        param.SortFields = cur_column.cSortName + " DESC";
                    }
                    break;
                }
            }

            $.ajax({
                url: ajaxUrl,
                type: "POST",
                dataType: "json",
                data: { data: JSON.stringify(param) },
                success: function (result) {
                    if (result.Success) {
                        vm.SetTableData(result.aaData);
                        vm.total(result.iTotalRecords);
                    }
                }
            });
        }
        //设置数据
        vm.SetTableData = function (datas) {
            var list = [];
            for (var i = 0; i < datas.length; i++) {
                var rowData = CreateRowData(datas[i], i);
                list.push(rowData);
            }
            vm.data(list);
        }
        //创建数据行,内部方法
        function CreateRowData(rowObj, id) {
            var cells = [];
            var rowData = {
                _id: ko.observable(id),
                _isSelected: ko.observable(false),
                cells: cells,
                source: rowObj
            };
            for (var i = 0; i < gridView.columns().length; i++) {
                var column = gridView.columns()[i];
                var cell = {
                    cText: column.cText,
                    cName: column.cName,
                    cType: column.cType,
                    cRender: column.cRender,
                    cellData: ko.observable(rowObj[column.cName])
                };

                if (cell.cType == 'html') {
                    if (typeof cell.cRender != 'function') {
                        throw new Error('cRender must be a function');
                    }
                    cell.cHtml = cell.cRender(rowData.source, rowData._id());
                }

                cells.push(cell);
            }
            return rowData;
        }
        //分页控件
        vm.pageItem = ko.pureComputed(function () {
            var count = 5;
            var result = [];
            var pageIndex = vm.filter.PageIndex();
            var totalPage = vm.totalPage();
            var start = pageIndex - count;
            if (start < 0) {
                start = 0;
            }
            var end = start + count;
            if (end >= totalPage) {
                end = totalPage - 1;
            }
            for (; start <= end; start++) {
                result.push(start + 1);
            }
            return result;
        }, vm);
        //pageSize选择事件
        vm.pageSizeChange = function () {
            vm.ReLoad(current_query);
        }
        //分页点击事件
        vm.pageClick = function (rowdata, event) {
            vm.filter.PageIndex(rowdata - 1);
            vm.ReLoad(current_query);
        };
        //上一页点击事件
        vm.pagePrev = function () {
            if (vm.filter.PageIndex() > 0) {
                vm.filter.PageIndex(vm.filter.PageIndex() - 1);
                vm.ReLoad(current_query);
            }
        }
        //下一页点击事件
        vm.pageNext = function () {
            if (vm.filter.PageIndex() < vm.totalPage() - 1) {
                vm.filter.PageIndex(vm.filter.PageIndex() + 1);
                vm.ReLoad(current_query);
            }
        }
        //获取选择的行数据
        vm.GetSelectRows = function () {
            var result = [];
            for (var i = 0; i < vm.data().length; i++) {
                var row_data = vm.data()[i];
                if (row_data._isSelected() == true) {
                    for (var n = 0; n < row_data.cells.length; n++) {
                        var cell = row_data.cells[n];
                        if (cell.cType == 'data') {
                            row_data.source[cell.cName] = cell.cellData();
                        }
                    }
                    result.push(row_data.source);
                }
            }
            return result;
        }
        //获取所有表格数据
        vm.GetAllRows = function () {
            var result = [];
            for (var i = 0; i < vm.data().length; i++) {
                var row_data = vm.data()[i];
                for (var n = 0; n < row_data.cells.length; n++) {
                    var cell = row_data.cells[n];
                    if (cell.cType == 'data') {
                        row_data.source[cell.cName] = cell.cellData();
                    }
                }
                result.push(row_data.source);
            }
            return result;
        }
        //根据行号获取数据
        vm.GetRowByIndex = function (row_id) {
            for (var i = 0; i < vm.data().length; i++) {
                var row_data = vm.data()[i];
                if (row_data._id() == row_id) {
                    for (var n = 0; n < row_data.cells.length; n++) {
                        var cell = row_data.cells[n];
                        if (cell.cType == 'data') {
                            row_data.source[cell.cName] = cell.cellData();
                        }
                    }
                    return row_data.source;
                }
            }
        }
        vm.SortTable = function (column, event) {
            if (!column.cSortable) {
                return;
            }
            column.cSortAscending(!column.cSortAscending());
            for (var i = 0; i < vm.columns().length; i++) {
                vm.columns()[i].cIsSorting(false);
            }
            column.cIsSorting(true);
            vm.ReLoad(current_query);
        }
    }

    var gridView = new GridView();

    //设置列头
    jsontable.SetTableColumns = function (columns) {
        //列头数据定义
        //cText,列头显示名称
        //cName,列json键名称
        //cWidth,列宽度
        //cVisible,是否可见
        //cType,数据列data,自定义html
        //cRender,自定义html函数
        //cSortName, 排序字段名,默认为cName的值
        //cSortable,是否可以排序 默认false 
        //cIsSorting(),是否是当前排序列,默认false 
        //cSortAscending(),是否是升序.默认为true
        var cWidthPercent = 100 / columns.length;
        for (var i = 0; i < columns.length; i++) {
            var col = columns[i];

            if (col.cWidth == null || col.cWidth == '') {
                col.cWidth = cWidthPercent + '%';
            }
            if (col.cVisible == null || col.cVisible == '') {
                col.cVisible = true;
            }
            if (col.cType == null || col.cType == '') {
                col.cType = 'data';
            }

            if (col.cSortable == null || col.cSortable == '') {
                col.cSortable = false;
            }
            if (col.cSortName == null || col.cSortName == '') {
                col.cSortName = col.cName;
            }
            col.cIsSorting = col.cIsSorting ? ko.observable(true) : ko.observable(false);
            col.cSortAscending = col.cSortAscending ? ko.observable(true) : ko.observable(false)

            gridView.columns.push(col);
        }
    }

    //加载数据
    jsontable.ReLoad = function (param) {
        gridView.ReLoad(param);
    }
    //获取选择的数据
    jsontable.GetSelectRows = function () {
        return gridView.GetSelectRows();
    }
    //获取所有数据
    jsontable.GetAllRows = function () {
        return gridView.GetAllRows();
    }
    //根据行号获取数据
    jsontable.GetRowByIndex = function (rowindex) {
        return gridView.GetRowByIndex(rowindex);
    }

    ko.applyBindings(gridView, document.getElementById(containerId));
}
/***************** KODataTable End ********************/

function KOLocalTable(containerId) {
    var jsontable = this;
    function TableView() {
        var vm = this;
        //行数据
        vm.data = ko.observableArray();
        //是否显示CheckBox
        vm.showCheckBox = ko.observable(true);
        //设置表格列头信息
        vm.columns = ko.observableArray();
        //全选CheckBox
        vm.allSelected = ko.computed({
            read: function () {
                var _allSelected = true;
                for (var i = 0; i < vm.data().length; i++) {
                    if (vm.data()[i]._isSelected() == false) {
                        _allSelected = false;
                        break;
                    }
                }
                if (vm.data().length == 0) { _allSelected = false; };
                return _allSelected;
            },
            write: function (value) {
                for (var i = 0; i < vm.data().length; i++) {
                    vm.data()[i]._isSelected(value)
                }
            },
            owner: vm
        });
        vm.allCheckBoxClick = function () {
            vm.allSelected(!vm.allSelected());
        }
        //行点击事件
        vm.rowClick = function (rowdata, event) {
            if (event.target.tagName != 'BUTTON' && event.target.tagName != 'A') {
                rowdata._isSelected(!rowdata._isSelected());
            } else if (event.target.tagName == 'A') {
                return true;
            }
        };
        //设置数据
        vm.SetTableData = function (datas) {
            var list = [];
            for (var i = 0; i < datas.length; i++) {
                var rowData = CreateRowData(datas[i]);
                list.push(rowData);
            }
            vm.data(list);
        }
        //创建数据行,内部方法
        function CreateRowData(rowObj) {
            var cells = [];
            var rowData = {
                _isSelected: ko.observable(false),
                cells: cells,
                source: rowObj
            };
            for (var i = 0; i < gridView.columns().length; i++) {
                var column = gridView.columns()[i];
                var cell = {
                    cText: column.cText,
                    cName: column.cName,
                    cType: column.cType,
                    cRender: column.cRender,
                    cellData: ko.observable(rowObj[column.cName])
                };

                if (cell.cType == 'html') {
                    if (typeof cell.cRender != 'function') {
                        throw new Error('cRender must be a function');
                    }
                    cell.cHtml = cell.cRender(rowData.source);
                }

                cells.push(cell);
            }
            return rowData;
        }

        vm.AddTableRow = function (data) {
            var rowData = CreateRowData(data);
            vm.data.push(rowData);
        }

        //获取选择的行数据
        vm.GetSelectRows = function () {
            var result = [];
            for (var i = 0; i < vm.data().length; i++) {
                var row_data = vm.data()[i];
                var source_data = {};
                if (row_data._isSelected() == true) {
                    for (var n = 0; n < row_data.cells.length; n++) {
                        var cell = row_data.cells[n];
                        if (cell.cType == 'data') {
                            source_data[cell.cName] = cell.cellData();
                        }
                    }
                    result.push(source_data);
                }
            }
            return result;
        }
        //获取所有表格数据
        vm.GetAllRows = function () {
            var result = [];
            for (var i = 0; i < vm.data().length; i++) {
                var row_data = vm.data()[i];
                for (var n = 0; n < row_data.cells.length; n++) {
                    var cell = row_data.cells[n];
                    if (cell.cType == 'data') {
                        row_data.source[cell.cName] = cell.cellData();
                    }
                }
                result.push(row_data.source);
            }
            return result;
        }

        vm.RemoveSelectRows = function () {
            vm.data.remove(function (item) {
                return item._isSelected() == true;
            })
        }
    }

    var gridView = new TableView();

    //设置列头
    jsontable.SetTableColumns = function (columns) {
        //列头数据定义
        //cText,列头显示名称
        //cName,列json键名称
        //cWidth,列宽度
        //cVisible,是否可见
        //cType,数据列data,自定义html
        //cRender,自定义html函数
        var cWidthPercent = 100 / columns.length;
        for (var i = 0; i < columns.length; i++) {
            var col = columns[i];

            if (col.cWidth == null || col.cWidth == '') {
                col.cWidth = cWidthPercent + '%';
            }
            if (col.cVisible == null || col.cVisible == '') {
                col.cVisible = true;
            }
            if (col.cType == null || col.cType == '') {
                col.cType = 'data';
            }
            gridView.columns.push(col);
        }
    }
    //获取选择的数据
    jsontable.GetSelectRows = function () {
        return gridView.GetSelectRows();
    }
    //获取所有数据
    jsontable.GetAllRows = function () {
        return gridView.GetAllRows();
    }
    jsontable.SetTableData = function (datas) {
        gridView.SetTableData(datas);
    }
    jsontable.AddTableRow = function (data) {
        gridView.AddTableRow(data);
    }
    jsontable.RemoveSelectRows = function () {
        gridView.RemoveSelectRows();
    }

    ko.applyBindings(gridView, document.getElementById(containerId));
}

/***************** KOLocalTable End ********************/

ko.bindingHandlers.formValid = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var value = valueAccessor();
        if (value.isValid && !value.isValid()) {
            //手动触发验证
            //value.isModified(true);
            var errorMsg = value.error();
            $(element).popover({ trigger: 'hover', content: errorMsg });
            if ($(element).is(':hover')) {
                $(element).popover('show');
            }
        } else {
            $(element).popover('destroy');
        }
    }
};

function KOForm(containerId, treeD) {
    var jsonform = this;
    var treeData;
    function FromView() {
        var view_self = this;
        //标示编辑form是否显示
        view_self.EditFormIsShow = ko.observable(true);
        //所有按钮
        view_self.Buttons = {
            //保存按钮
            SaveBtn: {
                actionUrl: '',
                visible: ko.observable(true),
                text: ko.observable('保存'),
                callBack: null,
                disabled: ko.observable(false)
            },
            SaveBtnClick: function () {
                if (view_self.Buttons.SaveBtn.actionUrl == null || view_self.Buttons.SaveBtn.actionUrl == '') {
                    throw new Error('SaveBtn need config actionUrl !');
                }
                if (!view_self.IsValid()) {
                    $.showWarning('表单数据还有错误,请检查修改后再保存');
                    return;
                }
                view_self.Buttons.SaveBtn.disabled(true);
                var postData = view_self.GetData();
                $.ajax({
                    url: view_self.Buttons.SaveBtn.actionUrl,
                    type: "POST",
                    dataType: "json",
                    data: { data: JSON.stringify(postData) },
                    success: function (result) {
                        if (result.Success) {
                            if (treeD !== undefined && treeD.Data !== undefined && treeD.Data.length > 0) {
                                treeData = treeD.Data;
                                for (var i = 0; i < treeData.length; i++) {
                                    if (treeData[i].id === postData.SysNo) {
                                        for (var postDataprops in postData) {
                                            for (var treeDataprops in treeData[i].data) {
                                                if (postDataprops === treeDataprops) {
                                                    treeData[i].data["" + treeDataprops + ""] = postData["" + postDataprops + ""];
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                                treeD.SetTreeData(treeData);
                            }
                            if (view_self.Buttons.SaveBtn.callBack != null) {
                                view_self.Buttons.SaveBtn.callBack(result);
                            }
                        } 
                        view_self.Buttons.SaveBtn.disabled(false); 
                    }
                });

            },
            //重置按钮
            ResetBtn: {
                visible: ko.observable(true),
                resetFunc: null,
                text: ko.observable('重置')
            },
            ResetBtnClick: function () {
                if (view_self.Buttons.ResetBtn.resetFunc != null) {
                    view_self.Buttons.ResetBtn.resetFunc();
                } else {
                    view_self.Reset();
                }
            },
        };
        //所有controlgroup
        view_self.ControlGroups = ko.observableArray();
        //重置
        view_self.Reset = function () {
            for (var i = 0; i < view_self.ControlGroups().length; i++) {
                var rowdata = view_self.ControlGroups()[i];
                if (typeof rowdata.cDefault == 'function') {
                    rowdata.cValue(rowdata.cDefault());
                } else {
                    if (rowdata.cType == 'checkboxs') {
                        for (var n = 0; n < rowdata.cCheckboxs.length; n++) {
                            rowdata.cCheckboxs[n].cChecked(false);
                        }
                    }
                    rowdata.cValue(rowdata.cDefault);
                }
            }
        }
        //获取数据
        view_self.GetData = function () {
            var result = {};
            for (var i = 0; i < view_self.ControlGroups().length; i++) {
                var rowdata = view_self.ControlGroups()[i];
                //cDefault定义为function表示行值使用cDefault()获取
                if (typeof rowdata.cDefault == 'function') {
                    result[rowdata.cName()] = rowdata.cDefault();
                }
                else {
                    if (rowdata.cType == 'checkboxs') {
                        result[rowdata.cName()] = rowdata.cCheckboxsGet(rowdata.cCheckboxs);
                    }
                    else {
                        result[rowdata.cName()] = rowdata.cValue();
                    }
                }
            }
            return result;
        }
        //设置数据
        view_self.SetData = function (obj) {
            if (obj == null) { return; };
            for (var i = 0; i < view_self.ControlGroups().length; i++) {
                var rowdata = view_self.ControlGroups()[i];
                if (obj[rowdata.cName()] != null) {
                    //checkboxs的值是集合,需要循环赋值
                    if (rowdata.cType == 'checkboxs') {
                        for (var n = 0; n < rowdata.cCheckboxs.length; n++) {
                            var _checked = rowdata.cCheckboxsSet(rowdata.cCheckboxs[n], obj[rowdata.cName()]);
                            rowdata.cCheckboxs[n].cChecked(_checked);
                        }
                    }

                    rowdata.cValue(obj[rowdata.cName()]);
                }
                else if (rowdata.cDefault != null) {
                    if (typeof rowdata.cDefault == 'function') {
                        rowdata.cValue(rowdata.cDefault());
                    }
                    else {
                        rowdata.cValue(rowdata.cDefault);
                    }
                    //checkboxs的值是集合,需要循环赋值
                    if (rowdata.cType == 'checkboxs') {
                        for (var n = 0; n < rowdata.cCheckboxs.length; n++) {
                            var _checked = rowdata.cCheckboxsSet(rowdata.cCheckboxs[n], rowdata.cValue());
                            rowdata.cCheckboxs[n].cChecked(_checked);
                        }
                    }
                }
                else {
                    //checkboxs的值是集合,需要循环赋值
                    if (rowdata.cType == 'checkboxs') {
                        for (var n = 0; n < rowdata.cCheckboxs.length; n++) {
                            rowdata.cCheckboxs[n].cChecked(false);
                        }
                    }
                    rowdata.cValue('');
                }
            }
        }
        //验证form数据
        view_self.IsValid = function () {
            var result = true;
            for (var i = 0; i < view_self.ControlGroups().length; i++) {
                var rowdata = view_self.ControlGroups()[i];
                if (rowdata.cValue.isValid && !rowdata.cValue.isValid()) {
                    //手动触发验证
                    rowdata.cValue.isModified(true);
                    result = false;
                }
            }
            return result;
        }

        view_self.RadioClick = function (controlGroup, element, current, event) {
            controlGroup.cValue(current.value);
        }
        view_self.CheckBoxClick = function (controlGroup, element, current, event) {
            current.cChecked(!current.cChecked());
        }
    }

    var fromView = new FromView();

    //设置按钮
    jsonform.SetButtons = function (butttons) {
        for (var btn in butttons) {
            if (butttons.hasOwnProperty(btn) && fromView.Buttons[btn] != null) {
                for (var prop in fromView.Buttons[btn]) {
                    if (fromView.Buttons[btn].hasOwnProperty(prop) && butttons[btn][prop] != null) {
                        if (ko.isObservable(fromView.Buttons[btn][prop])) {
                            fromView.Buttons[btn][prop](butttons[btn][prop]);
                        }
                        else {
                            fromView.Buttons[btn][prop] = butttons[btn][prop];
                        }
                    }
                }
            }
        }
    }

    //设置Form行
    //cText ,       行标题
    //cName,        行对应实体字段
    //cVisible,     行是否可见
    //cDisabled,    行是否可编辑,可传入bool值或者返回bool值的方法
    //cDefault,     设置默认值,可以穿入值,或者返回值的方法
    //cType,        行类型,radio,select,area,input
    jsonform.SetControlGroups = function (datas) {
        var list = [];
        for (var i = 0; i < datas.length; i++) {
            var rowData = CreateRowData(datas[i], i);
            list.push(rowData);
        }
        fromView.ControlGroups(list);
    }

    function CreateRowData(rowObj, id) {
        var rowData = {};
        rowData._id = id;
        rowData.cText = ko.observable(rowObj.cText);
        rowData.cName = ko.observable(rowObj.cName);
        //设置是否可见
        rowData.cVisible = rowObj.cVisible == null ? ko.observable(true) : ko.observable(rowObj.cVisible);
        //设置是否不可编辑,可以穿入bool值,或者返回bool值得方法
        if (rowObj.cDisabled == null) {
            rowData.cDisabled = function () { return ko.observable(false) };
        }
        else {
            if (typeof rowObj.cDisabled == 'function') {
                rowData.cDisabled = function (row_data, all_data, element) { return ko.observable(rowObj.cDisabled(row_data, all_data, element)) };
            }
            else {
                rowData.cDisabled = function () { return ko.observable(rowObj.cDisabled) };
            }
        }
        //设置默认值,可以穿入值,或者返回值的方法
        rowData.cDefault = rowObj.cDefault != null ? rowObj.cDefault : '';

        if (typeof rowData.cDefault != 'function') {
            rowData.cValue = ko.observable(rowData.cDefault).extend(rowObj.cRule);
        }
        else {
            rowData.cValue = ko.observable(rowData.cDefault()).extend(rowObj.cRule);
        }

        if (rowObj.cType != null) {
            rowData.cType = rowObj.cType;
            if (rowObj.cType == 'radio') {
                if (rowObj.cRadioButtuns == null) {
                    throw new Error('the  radio type ControlGroup need config cRadioButtuns !');
                }
                rowData.cRadioButtuns = rowObj.cRadioButtuns;
            }
            if (rowObj.cType == 'checkboxs') {
                if (rowObj.cCheckboxs == null) {
                    throw new Error('the  checkbox type ControlGroup need config cCheckboxs !');
                }
                if (rowObj.cCheckboxsSet == null || typeof rowObj.cCheckboxsSet != 'function') {
                    throw new Error('the  checkbox type ControlGroup need config cCheckboxsSet , and it must be function !');
                }
                if (rowObj.cCheckboxsGet == null || typeof rowObj.cCheckboxsGet != 'function') {
                    throw new Error('the  checkbox type ControlGroup need config cCheckboxsGet , and it must be function !');
                }
                rowData.cCheckboxs = rowObj.cCheckboxs;
                rowData.cCheckboxsSet = rowObj.cCheckboxsSet;
                rowData.cCheckboxsGet = rowObj.cCheckboxsGet;
                for (var i = 0; i < rowData.cCheckboxs.length; i++) {
                    rowData.cCheckboxs[i].cChecked = ko.observable(false);
                }
            }
            if (rowObj.cType == 'select') {
                if (rowObj.cSelectOptions == null) {
                    throw new Error('the  select type ControlGroup need config cSelectOptions !');
                }
                rowData.cSelectOptions = rowObj.cSelectOptions;
            }
            if (rowObj.cType == 'area') {
                var cAreaSize = {
                    cols: 20,
                    rows: 3
                };
                $.extend(cAreaSize, rowObj.cAreaSize);
                rowData.cAreaSize = cAreaSize;
            }
        } else {
            rowData.cType = 'input'
        }
        return rowData;
    }

    //重置表单
    jsonform.Reset = function () {
        fromView.Reset();
    }

    //获取form数据
    jsonform.GetData = function () {
        return fromView.GetData();
    }

    //设置form数据
    jsonform.SetData = function (obj) {
        fromView.SetData(obj);
    }

    //验证form表单数据
    jsonform.IsValid = function () {
        fromView.IsValid();
    }

    //隐藏form
    jsonform.Hide = function () {
        fromView.EditFormIsShow(false);
    }

    //显示form
    jsonform.Show = function () {
        fromView.EditFormIsShow(true);
    }
    //切换显示隐藏
    jsonform.Toggle = function () {
        fromView.EditFormIsShow(!fromView.EditFormIsShow());
    }

    ko.applyBindings(fromView, document.getElementById(containerId));
}
/***************** KOForm End ********************/

ko.bindingHandlers.koVisible = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var value = valueAccessor();
        if (value() == true) {
            $(element).slideDown();
        }
        else {
            $(element).slideUp();
        }
    }
};

/*
PermissionJsonTree参数说明
var option = {
    id: entity.SysNo,
    text: entity.XXName,
    data: entity,
    Default:false
}
var node = {
    id: entity.SysNo,
    text: entity.XXName,
    data: entity,
    parent:entity.ParentSysNo,
    options: [option]
}

构造参数:
containerId - 容器id, ajaxUrl - 远程数据地址

开放方法:
ReLoad(param)                   加载数据
Reset()                         重置数据
SetTreeData()                   设置数据,适用于本地
GetCheckedData()                获取勾选的数据

*/
function KOPermissionTree(containerId, ajaxUrl) {
    var jsontree = this;
    function TreeView() {
        var view_self = this;
        view_self.data = ko.observableArray();

        view_self.SetTreeData = function (_data) {
            view_self.data(_data);
        };
        view_self.OpenClick = function (rowdata, event) {
            rowdata.open(!rowdata.open());
        };
        view_self.CheckBoxClick = function (element, current, event) {
            if (current.disabled != true) {
                current.checked(!current.checked());
            }
        }
    }

    var treeview = new TreeView();

    var source_data;

    jsontree.ReLoad = function (param) {
        $.ajax({
            url: ajaxUrl,
            type: "POST",
            dataType: "json",
            async: false,
            data: { data: JSON.stringify(param) },
            success: function (result) {
                if (result.Success) {
                    jsontree.SetTreeData(result.Data);
                }
            }
        });
    }

    jsontree.Reset = function () {
        for (var n = 0; n < source_data.length; n++) {
            var current = source_data[n];
            for (var i = 0; i < current.options.length; i++) {
                current.options[i].checked(current.options[i].Default);
            }
        }
    };

    jsontree.SetTreeData = function (datas) {
        source_data = datas;
        for (var i = 0; i < datas.length; i++) {
            CreateRowData(datas[i], i);
        }
        var toplist = [];
        for (var n = 0; n < source_data.length; n++) {
            if (source_data[n].parent == 0) {
                toplist.push(source_data[n]);
            }
        }
        if (toplist.length > 0) {
            toplist[toplist.length - 1].isLast = true;
        }
        treeview.SetTreeData(toplist);
    }

    function CreateRowData(rowObj, index) {
        //rowObj.id
        //rowObj.text
        //rowObj.data
        //rowObj.parent
        //rowObj.options
        //添加open监控属性
        rowObj.index = index;
        rowObj.open = ko.observable(true);
        if (rowObj.options != null) {
            for (var i = 0; i < rowObj.options.length; i++) {
                var current_option = rowObj.options[i];
                //GetDisabled返回true则需要将rowObj.disabled属性置为true
                if (jsontree.GetDisabled != null && jsontree.GetDisabled(current_option.data) == true) {
                    current_option.disabled = true;
                }
                else {
                    current_option.disabled = false;
                }
                current_option.checked = ko.observable(current_option.Default);
            }
        }
        else {
            rowObj.options = [];
        }
        //添加childrens
        rowObj.childrens = [];
        for (var n = 0; n < source_data.length; n++) {
            var current = source_data[n];
            if (current.parent == rowObj.id) {
                rowObj.childrens.push(current);
            }
        }
        if (rowObj.childrens.length > 0) {
            rowObj.childrens[rowObj.childrens.length - 1].isLast = true;
        }
    }

    jsontree.GetCheckedData = function () {
        var result = [];
        for (var i = 0; i < source_data.length; i++) {
            var item = source_data[i];
            for (var j = 0; j < item.options.length; j++) {
                var option = item.options[j];
                if (option.checked() == true) {
                    result.push(option.data);
                }
            }
        }
        return result;
    }

    jsontree.Open = function () {
        for (var i = 0; i < source_data.length; i++) {
            var item = source_data[i];
            item.open(true);
        }
    }

    jsontree.GetDisabled = function (node) {
        return false;
    }

    ko.applyBindings(treeview, document.getElementById(containerId));
}
/***************** KOPermissionTree End ********************/

/*
KOMenuTree参数说明
var option = {
   id: entity.SysNo,
   text: entity.XXName,
   data: entity,
   Default:false
}
var node = {
   id: entity.SysNo,
   text: entity.XXName,
   data: entity,
   parent:entity.ParentSysNo,
   options: [option]
}

构造参数:
containerId - 容器id, ajaxUrl - 远程数据地址

开放方法:
ReLoad(param)                   加载数据
Reset()                         重置数据
SetTreeData()                   设置数据,适用于本地
GetCheckedData()                获取勾选的数据

*/
function KOMenuTree(containerId, ajaxUrl) {
    var jsontree = this;
    function KOMenuTreeView() {
        var view_self = this;
        view_self.data = ko.observableArray();

        view_self.SetTreeData = function (_data) {
            view_self.data(_data);
        };
        view_self.OpenClick = function (rowdata, event) {
            rowdata.open(!rowdata.open());
        };
        view_self.CheckBoxClick = function (element, current, event) {
            current.checked(!current.checked());
        };
        view_self.addClick = function (element, currentData, event) {
            if (jsontree.AddClick != null && typeof jsontree.AddClick == 'function') {
                jsontree.AddClick(element, currentData, event);
            }
        }
        view_self.deleteClick = function (element, currentData, event) {
            if (jsontree.DeleteClick != null && typeof jsontree.DeleteClick == 'function') {
                jsontree.DeleteClick(element, currentData, event);
            }
        }
    }

    var treeview = new KOMenuTreeView();

    var source_data;

    jsontree.ReLoad = function (param) {
        $.ajax({
            url: ajaxUrl,
            type: "POST",
            dataType: "json",
            async: true,
            data: { data: JSON.stringify(param) },
            success: function (result) {
                if (result.Success) {
                    jsontree.SetTreeData(result.Data);
                }
            }
        });
    }

    jsontree.SetTreeData = function (datas) {
        source_data = datas;
        for (var i = 0; i < datas.length; i++) {
            CreateRowData(datas[i], i);
        }
        var toplist = [];
        for (var n = 0; n < source_data.length; n++) {
            if (source_data[n].parent == 0) {
                toplist.push(source_data[n]);
            }
        }
        if (toplist.length > 0) {
            toplist[toplist.length - 1].isLast = true;
        }
        treeview.SetTreeData(toplist);
    }

    function CreateRowData(rowObj, index) {
        //rowObj.id
        //rowObj.text
        //rowObj.data
        //rowObj.parent
        //rowObj.options
        //添加open监控属性
        rowObj.index = index;
        rowObj.open = ko.observable(true);
        if (rowObj.options != null) {
            for (var i = 0; i < rowObj.options.length; i++) {
                rowObj.options[i].checked = ko.observable(rowObj.options[i].Default);
            }
        } else {
            rowObj.options = [];
        }
        //添加childrens
        rowObj.childrens = [];
        for (var n = 0; n < source_data.length; n++) {
            var current = source_data[n];
            if (current.parent == rowObj.id) {
                rowObj.childrens.push(current);
            }
        }
        if (rowObj.childrens.length > 0) {
            rowObj.childrens[rowObj.childrens.length - 1].isLast = true;
        }
    }

    jsontree.GetCheckedData = function () {
        var result = [];
        for (var i = 0; i < source_data.length; i++) {
            var item = source_data[i];
            for (var j = 0; j < item.options.length; j++) {
                var option = item.options[j];
                if (option.checked() == true) {
                    result.push(option.data);
                }
            }
        }
        return result;
    }

    jsontree.Open = function () {
        for (var i = 0; i < source_data.length; i++) {
            var item = source_data[i];
            item.open(true);
        }
    }

    jsontree.DeleteClick = function (element, currentData, event) { }
    jsontree.AddClick = function (element, currentData, event) { }

    ko.applyBindings(treeview, document.getElementById(containerId));
}
/***************** KOMenuTree End ********************/


/*
KOTree参数说明

var node = {
    id: entity.SysNo,
    text: entity.XXName,
    data: entity,
    parent:entity.ParentSysNo
}

构造参数:
containerId - 容器id, ajaxUrl - 远程数据地址

开放方法:
ReLoad(param)                   加载数据
Reset()                         重置数据
SetTreeData()                   设置数据,适用于本地
*/
function KOTree(containerId, ajaxUrl) {
    var jsontree = this;
    function TreeView() {
        view_self = this;
        view_self.data = ko.observableArray();

        view_self.SetTreeData = function (_data) {
            view_self.data(_data);
        };
        view_self.OpenClick = function (rowdata, event) {
            console.log(rowdata.open());
            rowdata.open(!rowdata.open());
            console.log(rowdata.open());
        };
        view_self.editClick = function (element, currentData, event) {
            if (jsontree.EditClick != null && typeof jsontree.EditClick == 'function') {
                jsontree.EditClick(element, currentData, event);
            }
        }
        view_self.addClick = function (element, currentData, event) {
            if (jsontree.AddClick != null && typeof jsontree.AddClick == 'function') {
                jsontree.AddClick(element, currentData, event);
            }
        }
        view_self.deleteClick = function (element, currentData, event) {
            if (jsontree.DeleteClick != null && typeof jsontree.DeleteClick == 'function') {
                jsontree.DeleteClick(element, currentData, event);
            }
        }
        //添加一个 permissionClick
        view_self.permissionClick = function (element, currentData, event) {
            if (jsontree.PermissionClick != null && typeof jsontree.PermissionClick == 'function') {
                jsontree.PermissionClick(element, currentData, event);
            }
        }
    }
    var treeview = new TreeView();

    var source_data;

    jsontree.ReLoad = function (param) {
        $.ajax({
            url: ajaxUrl,
            type: "POST",
            dataType: "json",
            async: true,
            data: { data: JSON.stringify(param) },
            success: function (result) {
                if (result.Success) {
                    jsontree.SetTreeData(result.Data);
                }
            }
        });
    }

    jsontree.SetTreeData = function (datas) {
        jsontree.Data = datas;
        source_data = datas;
        for (var i = 0; i < datas.length; i++) {
            CreateRowData(datas[i], i);
        }
        var toplist = [];
        for (var n = 0; n < source_data.length; n++) {
            if (source_data[n].parent == 0) {
                toplist.push(source_data[n]);
            }
        }
        if (toplist.length > 0) {
            toplist[toplist.length - 1].isLast = true;
        }
        treeview.SetTreeData(toplist);
    }

    function CreateRowData(rowObj, index) {
        //rowObj.id
        //rowObj.text
        //rowObj.data
        //rowObj.parent
        //添加open监控属性
        rowObj.index = index;
        rowObj.open = ko.observable(true);
        //添加childrens
        rowObj.childrens = [];
        for (var n = 0; n < source_data.length; n++) {
            var current = source_data[n];
            if (current.parent == rowObj.id) {
                rowObj.childrens.push(current);
                current.ParentObj = rowObj;
            }
        }
        if (rowObj.childrens.length > 0) {
            rowObj.childrens[rowObj.childrens.length - 1].isLast = true;
        }
    }

    jsontree.Open = function () {
        for (var i = 0; i < source_data.length; i++) {
            var item = source_data[i];
            item.open(true);
        }
    }

    jsontree.EditClick = function () { }
    jsontree.AddClick = function () { }
    jsontree.DeleteClick = function () { }
    jsontree.PermissionClick = function () { }
    jsontree.Data;

    ko.applyBindings(treeview, document.getElementById(containerId));


}
/***************** KOTree End ********************/




/*****************JsonControl*/


//创建,批量启用,禁用,删除
function KOControl(containerId, form, table) {
    var jsoncontrol = this;

    function ControlView() {
        var view_self = this;
        view_self.Buttons = {
            //创建按钮
            CreateBtn: {
                actionUrl: '',
                visible: ko.observable(true),
                text: ko.observable('创建'),
                callBack: null
            },
            CreateBtnClick: function () {
                if (view_self.Buttons.CreateBtn.callBack != null && typeof view_self.Buttons.CreateBtn.callBack == 'function') {
                    form.Reset();
                    view_self.Buttons.CreateBtn.callBack(form, table);
                }
                else {
                    form.Reset();
                    form.Toggle();
                }
            },
            //启用按钮
            ActiveBtn: {
                actionUrl: '',
                visible: ko.observable(true),
                text: ko.observable('启用'),
                callBack: null
            },
            ActiveBtnClick: function () {
                if (view_self.Buttons.ActiveBtn.actionUrl == null || view_self.Buttons.ActiveBtn.actionUrl == '') {
                    throw new Error('ActiveBtn need config actionUrl !');
                }
                var datas = table.GetSelectRows();
                if (datas.length == 0) {
                    $.alert("请选择数据！");
                    return;
                }
                var sysnos = [];
                for (var i = 0; i < datas.length; i++) {
                    sysnos.push(datas[i].SysNo);
                }
                $.ajax({
                    url: view_self.Buttons.ActiveBtn.actionUrl,
                    type: "POST",
                    dataType: "json",
                    data: { data: JSON.stringify(sysnos) },
                    success: function (result) {
                        if (result.Success) {
                            $.alert('启用成功');
                            view_self.Buttons.ActiveBtn.callBack(form, table);
                            //table.ReLoad(bulidSearchQuery());
                        }
                    }
                });
            },
            //禁用按钮
            DeActiveBtn: {
                actionUrl: '',
                visible: ko.observable(true),
                text: ko.observable('禁用'),
                callBack: null
            },
            DeActiveBtnClick: function () {
                if (view_self.Buttons.DeActiveBtn.actionUrl == null || view_self.Buttons.DeActiveBtn.actionUrl == '') {
                    throw new Error('DeActiveBtn need config actionUrl !');
                }
                var datas = table.GetSelectRows();
                if (datas.length == 0) {
                    $.alert("请选择数据！");
                    return;
                }
                var sysnos = [];
                for (var i = 0; i < datas.length; i++) {
                    sysnos.push(datas[i].SysNo);
                }

                $.ajax({
                    url: view_self.Buttons.DeActiveBtn.actionUrl,
                    type: "POST",
                    dataType: "json",
                    data: { data: JSON.stringify(sysnos) },
                    success: function (result) {
                        if (result.Success) {
                            $.alert('禁用成功');
                            view_self.Buttons.DeActiveBtn.callBack(form, table);
                            //table.ReLoad(bulidSearchQuery());
                        }
                    }
                });
            },
            //删除按钮
            DeleteBtn: {
                actionUrl: '',
                visible: ko.observable(true),
                text: ko.observable('删除'),
                callBack: null
            },
            DeleteBtnClick: function () {

                if (view_self.Buttons.DeleteBtn.actionUrl == null || view_self.Buttons.DeleteBtn.actionUrl == '') {
                    throw new Error('DeActiveBtn need config actionUrl !');
                }
                var datas = table.GetSelectRows();
                if (datas.length == 0) {
                    $.alert("请选择数据！");
                    return;
                }
                var sysnos = [];
                for (var i = 0; i < datas.length; i++) {
                    sysnos.push(datas[i].SysNo);
                }
                $.confirm('确定要删除选择项', function (res) {
                    if (!res) {
                        return;
                    }
                    $.ajax({
                        url: view_self.Buttons.DeleteBtn.actionUrl,
                        type: "POST",
                        dataType: "json",
                        data: { data: JSON.stringify(sysnos) },
                        success: function (result) {
                            if (result.Success) {
                                $.alert('删除成功');
                                view_self.Buttons.DeleteBtn.callBack(form, table);
                                // table.ReLoad(bulidSearchQuery());
                            }
                        }
                    });
                });
            }
        };
    }

    var controlView = new ControlView();

    //设置按钮
    jsoncontrol.SetButtons = function (butttons) {
        for (var btn in butttons) {
            if (butttons.hasOwnProperty(btn) && controlView.Buttons[btn] != null) {
                for (var prop in controlView.Buttons[btn]) {
                    if (controlView.Buttons[btn].hasOwnProperty(prop) && butttons[btn][prop] != null) {
                        if (ko.isObservable(controlView.Buttons[btn][prop])) {
                            controlView.Buttons[btn][prop](butttons[btn][prop]);
                        }
                        else {
                            controlView.Buttons[btn][prop] = butttons[btn][prop];
                        }
                    }
                }
            }
        }
    }

    ko.applyBindings(controlView, document.getElementById(containerId));
}


//Tree选中，取消父节点，子节点全部选中，全部取消
function TreeSelectParentToSon(current) {
    if (current.childrens !== undefined && current.options !== undefined) {
        if (current.checked() === true) {
            if (current.childrens.length > 0) {
                for (var i = 0; i < current.childrens.length; i++) {
                    current.childrens[i].checked(true);
                    TreeSelectParentToSon(current.childrens[i])
                }
            }
            if (current.options.length > 0) {
                for (var j = 0; j < current.options.length; j++) {
                    current.options[j].checked(true);
                }
            }
        } else {
            if (current.childrens.length > 0) {
                for (var i = 0; i < current.childrens.length; i++) {
                    current.childrens[i].checked(false);
                    TreeSelectParentToSon(current.childrens[i])
                }
            }
            if (current.options.length > 0) {
                for (var j = 0; j < current.options.length; j++) {
                    current.options[j].checked(false);
                }
            }
        }
    }
}



function KORoleEditTree(containerId, ajaxUrl) {
    var jsontree = this;
    function TreeView() {
        var view_self = this;
        view_self.data = ko.observableArray();

        view_self.SetTreeData = function (_data) {
            view_self.data(_data);
        };
        view_self.OpenClick = function (rowdata, event) {
            rowdata.open(!rowdata.open());
        };
        view_self.CheckBoxClick = function (element, current, event) {
            if (current.disabled != true) {
                current.checked(!current.checked());
                TreeSelectParentToSon(current);
                if (current.parentcheck === undefined && current.checked() === false) {
                    for (var i = 0; i < source_data.length; i++) {
                        if (source_data[i].parentcheck !== undefined && source_data[i].childrens.length >= 0) {
                            if (source_data[i].id === current.rootno || source_data[i].id === current.parentno) {
                                source_data[i].checked(false);
                            }
                        } else {
                            break;
                        }
                    }
                }
                if (current.parentcheck === undefined && current.checked() === true) {
                    var parentindex = -1, rootindex = -1;
                    var parentcheck = true;
                    var notcurrentcheck = true;
                    var parentlist = null;
                    for (var i = 0; i < source_data.length; i++) {
                        if (source_data[i].id === current.rootno) {
                            rootindex = i;
                            parentlist = source_data[i].childrens;
                            for (var n = 0; n < parentlist.length; n++) {
                                for (var j = 0; j < parentlist[n].options.length; j++) {
                                    if (parentlist[n].options[j].checked() === false) {
                                        parentlist[n].checked(false);
                                        source_data[rootindex].checked(false);
                                        break;
                                    } else {
                                        parentlist[n].checked(true);
                                    }
                                }

                            }
                            break;
                        }
                        if (source_data[i].id === current.parentno && current.rootno === 0) {
                            rootindex = i;
                            parentlist = source_data[i].options;
                            break;
                        }
                    }
                    for (var i = 0; i < parentlist.length; i++) {
                        if (parentlist[i].checked() === false) {

                            source_data[rootindex].checked(false);
                            break;
                        } else {
                            for (var j = 0; j < source_data[rootindex].childrens.length; j++) {
                                if (source_data[rootindex].childrens[j].checked() === false) {
                                    break;
                                } else {
                                    source_data[rootindex].checked(true);
                                }
                            }

                        }
                    }

                }
                if (current.parentcheck !== undefined && current.checked() === true) {
                    for (var i = 0; i < source_data.length; i++) {
                        if (source_data[i].id === current.parent) {
                            for (var j = 0; j < source_data[i].childrens.length; j++) {
                                if (source_data[i].childrens[j].checked() === false) {
                                    source_data[i].checked(false);
                                    break;
                                }
                                else {
                                    source_data[i].checked(true);
                                }
                            }
                        }
                    }
                }
                if (current.parentcheck !== undefined && current.checked() === false) {
                    for (var i = 0; i < source_data.length; i++) {
                        if (source_data[i].id === current.parent) {
                            source_data[i].checked(false);
                            break;
                        }
                    }
                }
            }
        }
    }

    var treeview = new TreeView();

    var source_data;

    jsontree.ReLoad = function (param) {
        $.ajax({
            url: ajaxUrl,
            type: "POST",
            dataType: "json",
            async: false,
            data: { data: JSON.stringify(param) },
            success: function (result) {
                if (result.Success) {
                    jsontree.SetTreeData(result.Data);
                }
            }
        });
    }

    jsontree.Reset = function () {
        for (var n = 0; n < source_data.length; n++) {
            var current = source_data[n];
            for (var i = 0; i < current.options.length; i++) {
                current.options[i].checked(current.options[i].Default);
            }
        }
    };

    jsontree.SetTreeData = function (datas) {
        source_data = datas;
        for (var i = 0; i < datas.length; i++) {
            CreateRowData(datas[i], i);
        }
        var toplist = [];
        for (var n = 0; n < source_data.length; n++) {
            if (source_data[n].parent == 0) {
                toplist.push(source_data[n]);
            }
        }
        if (toplist.length > 0) {
            toplist[toplist.length - 1].isLast = true;
        }
        treeview.SetTreeData(toplist);
    }

    function CreateRowData(rowObj, index) {
        //rowObj.id
        //rowObj.text
        //rowObj.data
        //rowObj.parent
        //rowObj.options
        //添加open监控属性
        rowObj.index = index;
        rowObj.open = ko.observable(true);
        if (rowObj.options != null) {
            if (rowObj.parentcheck !== undefined && rowObj.parentcheck === false) {
                var current_option = rowObj;
                if (jsontree.GetDisabled != null && jsontree.GetDisabled(current_option.data) == true) {
                    current_option.disabled = true;
                }
                else {
                    current_option.disabled = false;
                }
                current_option.checked = ko.observable(false);
            }


            for (var i = 0; i < rowObj.options.length; i++) {
                var current_option = rowObj.options[i];
                //GetDisabled返回true则需要将rowObj.disabled属性置为true
                if (jsontree.GetDisabled != null && jsontree.GetDisabled(current_option.data) == true) {
                    current_option.disabled = true;
                }
                else {
                    current_option.disabled = false;
                }
                current_option.checked = ko.observable(current_option.Default);
            }
        }
        else {
            rowObj.options = [];
        }
        //添加childrens
        rowObj.childrens = [];
        for (var n = 0; n < source_data.length; n++) {
            var current = source_data[n];
            if (current.parent == rowObj.id) {
                rowObj.childrens.push(current);
            }
        }
        if (rowObj.childrens.length > 0) {
            rowObj.childrens[rowObj.childrens.length - 1].isLast = true;
        }
    }

    jsontree.GetCheckedData = function () {
        var result = [];
        for (var i = 0; i < source_data.length; i++) {
            var item = source_data[i];
            for (var j = 0; j < item.options.length; j++) {
                var option = item.options[j];
                if (option.checked() == true) {
                    result.push(option.data);
                }
            }
        }
        return result;
    }

    jsontree.Open = function () {
        for (var i = 0; i < source_data.length; i++) {
            var item = source_data[i];
            item.open(true);
        }
    }

    jsontree.GetDisabled = function (node) {
        return false;
    }

    ko.applyBindings(treeview, document.getElementById(containerId));
}
