/*  JsonTable Create By Gavin 2016.09.01

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
function JsonTable(containerId, ajaxUrl) {
    var jsontable = this;
    function GridView() {
        this.filter = ko.mapping.fromJS({
            PageIndex: 0,
            PageSize: 10
        });
        //行数据
        this.data = ko.observableArray();
        //行起始数
        this.StartIndex = ko.computed(function () {
            return this.filter.PageIndex() * this.filter.PageSize();
        }, this);
        //总记录
        this.total = ko.observable(0);
        //总页数
        this.totalPage = ko.computed(function () {
            return Math.ceil(this.total() / this.filter.PageSize());
        }, this);
        //每页数据Size
        this.pageSizeSelect = ko.observableArray([10, 25, 50, 100, 150]);
        //是否显示CheckBox
        this.showCheckBox = ko.observable(true);
        //设置表格列头信息
        this.columns = ko.observableArray();
        //全选CheckBox
        this.allSelected = ko.computed({
            read: function () {
                var _allSelected = true;
                for (var i = 0; i < this.data().length; i++) {
                    if (this.data()[i]._isSelected() == false) {
                        _allSelected = false;
                        break;
                    }
                }
                if (this.data().length == 0) { _allSelected = false; };
                return _allSelected;
            },
            write: function (value) {
                for (var i = 0; i < this.data().length; i++) {
                    this.data()[i]._isSelected(value)
                }
            },
            owner: this
        });
        //行点击事件
        this.rowClick = function (rowdata, event) {
            if (event.target.tagName != 'BUTTON') {
                rowdata._isSelected(!rowdata._isSelected());
            }
        };

        grid_self = this;

        var current_query = {};

        //远程加载数据
        this.ReLoad = function (param) {
            var pageinfo = {
                PageSize: grid_self.filter.PageSize(),
                PageIndex: grid_self.filter.PageIndex()
            };
            if (param == null) { param = {}; };
            current_query = param;
            $.extend(param, pageinfo);
            $.ajax({
                url: ajaxUrl,
                type: "POST",
                dataType: "json",
                data: { data: JSON.stringify(param) },
                success: function (result) {
                    if (result.Success) {
                        grid_self.SetTableData(result.aaData);
                        grid_self.total(result.iTotalRecords);
                    }
                }
            });
        }
        //设置数据
        this.SetTableData = function (datas) {
            var list = [];
            for (var i = 0; i < datas.length; i++) {
                var rowData = CreateRowData(datas[i], i);
                list.push(rowData);
            }
            grid_self.data(list);
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
        this.pageItem = ko.pureComputed(function () {
            var count = 5;
            var result = [];
            var pageIndex = this.filter.PageIndex();
            var totalPage = this.totalPage();
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
        }, this);
        //pageSize选择事件
        this.pageSizeChange = function () {
            grid_self.ReLoad(current_query);
        }
        //分页点击事件
        this.pageClick = function (rowdata, event) {
            grid_self.filter.PageIndex(rowdata - 1);
            grid_self.ReLoad(current_query);
        };
        //上一页点击事件
        this.pagePrev = function () {
            if (grid_self.filter.PageIndex() > 0) {
                grid_self.filter.PageIndex(grid_self.filter.PageIndex() - 1);
                grid_self.ReLoad(current_query);
            }
        }
        //下一页点击事件
        this.pageNext = function () {
            if (grid_self.filter.PageIndex() < grid_self.totalPage() - 1) {
                grid_self.filter.PageIndex(grid_self.filter.PageIndex() + 1);
                grid_self.ReLoad(current_query);
            }
        }
        //获取选择的行数据
        this.GetSelectRows = function () {
            var result = [];
            for (var i = 0; i < grid_self.data().length; i++) {
                var row_data = grid_self.data()[i];
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
        this.GetAllRows = function () {
            var result = [];
            for (var i = 0; i < grid_self.data().length; i++) {
                var row_data = grid_self.data()[i];
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
        this.GetRowByIndex = function (row_id) {
            for (var i = 0; i < grid_self.data().length; i++) {
                var row_data = grid_self.data()[i];
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

ko.bindingHandlers.formVisible = {
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

ko.bindingHandlers.formValid = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var value = valueAccessor();
        if (value.isValid && !value.isValid()) {
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

function JsonForm(containerId) {
    var jsonform = this;

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
                    $.showError('表单数据还有错误,请检查修改后再保存');
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
                actionUrl: '',
                visible: ko.observable(true),
                callBack: null,
                text: ko.observable('重置')
            },
            ResetBtnClick: function () {
                view_self.Reset();
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
                    rowdata.cValue(rowdata.cDefault);
                }
            }
        }
        //获取数据
        view_self.GetData = function () {
            var result = {};
            for (var i = 0; i < view_self.ControlGroups().length; i++) {
                var rowdata = view_self.ControlGroups()[i];
                if (typeof rowdata.cDefault == 'function') {
                    result[rowdata.cName()] = rowdata.cDefault();
                } else {
                    result[rowdata.cName()] = rowdata.cValue();
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
                    rowdata.cValue(obj[rowdata.cName()]);
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
        
        if (rowObj.cType != null) {
            rowData.cType = rowObj.cType;
            if (rowObj.cType == 'radio') {
                if (rowObj.cRadioButtuns == null) {
                    throw new Error('the  radio type ControlGroup need config cRadioButtuns !');
                }
                rowData.cRadioButtuns = rowObj.cRadioButtuns;
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
        if (typeof rowData.cDefault != 'function') {
            rowData.cValue = ko.observable(rowData.cDefault).extend(rowObj.cRule);
        }
        else {
            rowData.cValue = ko.observable(rowData.cDefault()).extend(rowObj.cRule);
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

//创建,批量启用,禁用,删除
function JsonControl(containerId, form, table) {
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
                    $.showError("请选择数据！");
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
                            $.showSuccess('启用成功');
                            table.ReLoad(bulidSearchQuery());
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
                    $.showError("请选择数据！");
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
                            $.showSuccess('禁用成功');
                            table.ReLoad(bulidSearchQuery());
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
                    $.showError("请选择数据！");
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
                                $.showSuccess('删除成功');
                                table.ReLoad(bulidSearchQuery());
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
function PermissionJsonTree(containerId, ajaxUrl) {
    var jsontree = this;
    function TreeView() {
        view_self = this;
        view_self.data = ko.observableArray();

        view_self.SetTreeData = function (_data) {
            view_self.data(_data);
        };
        view_self.OpenClick = function (rowdata, event) {
            rowdata.open(!rowdata.open());
        };
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
        rowObj.open = ko.observable(false);
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

    ko.applyBindings(treeview, document.getElementById(containerId));
}

/*
参数说明
var Data = {
    leftData: [{
        id: 1,
        text: 'XXXX',
        data: entity
    }, {
        id: 2,
        text: 'XXXX',
        data: entity
    }, {
        id: 3,
        text: 'XXXX',
        data: entity
    }],
    rightData: [{
        id: 4,
        text: 'XXXX',
        data: entity
    }, {
        id: 5,
        text: 'XXXX',
        data: entity
    }]
}
*/
function DualSelect(containerId, ajaxUrl) {
    var jsonselect = this;

    function SelectView() {
        view_self = this;
        view_self.leftData = ko.observableArray();
        view_self.selectLeftData = ko.observableArray();
        view_self.rightData = ko.observableArray();
        view_self.selectRightData = ko.observableArray();
        //选中右移
        view_self.ToRight = function () {
            var result = [];
            for (var i = 0; i < view_self.leftData().length; i++) {
                if (view_self.selectLeftData().indexOf(view_self.leftData()[i].id) != -1) {
                    view_self.rightData.push(view_self.leftData()[i])
                }
                else {
                    result.push(view_self.leftData()[i]);
                }
            }
            view_self.leftData(result);
            view_self.selectLeftData([]);
        };
        //所有右移
        view_self.AllToRight = function () {
            var result = [];
            for (var i = 0; i < view_self.leftData().length; i++) {
                view_self.rightData.push(view_self.leftData()[i])
            }
            view_self.leftData(result);
            view_self.selectLeftData([]);
        };
        //选中左移
        view_self.ToLeft = function () {
            var result = [];
            for (var i = 0; i < view_self.rightData().length; i++) {
                if (view_self.selectRightData().indexOf(view_self.rightData()[i].id) != -1) {
                    view_self.leftData.push(view_self.rightData()[i])
                }
                else {
                    result.push(view_self.rightData()[i]);
                }
            }
            view_self.rightData(result);
            view_self.selectRightData([]);
        };
        //所有左移
        view_self.AllToLeft = function () {
            var result = [];
            for (var i = 0; i < view_self.rightData().length; i++) {
                view_self.leftData.push(view_self.rightData()[i])
            }
            view_self.rightData(result);
            view_self.selectRightData([]);
        };
        //左边总记录数
        view_self.LeftTotalCount = ko.computed(function () {
            return this.leftData().length;
        }, view_self);
        //左边选中数
        view_self.LeftSelectCount = ko.computed(function () {
            return this.selectLeftData().length;
        }, view_self);
        //右边总记录数
        view_self.RightTotalCount = ko.computed(function () {
            return this.rightData().length;
        }, view_self);
        //右边选中数
        view_self.RightSelectCount = ko.computed(function () {
            return this.selectRightData().length;
        }, view_self);
        view_self.leftInputFilter = ko.observable('');
        view_self.rightInputFilter = ko.observable('');
        //Filter过滤
        view_self.DoDualSelectFilter = function (element, viewModel) {
            var filter_text = '';
            var position = element.getAttribute("position");

            var filterArray = [];
            if (position == 'left') {
                filterArray = viewModel.leftData();
                filter_text = $.trim(viewModel.leftInputFilter());
                viewModel.selectLeftData([]);
            } else if (position == 'right') {
                filterArray = viewModel.rightData();
                filter_text = $.trim(viewModel.rightInputFilter());
                viewModel.selectRightData([]);
            }

            if (filter_text == '') {
                for (var i = 0; i < filterArray.length; i++) {
                    filterArray[i]._visible(true);
                };
                return;
            }
            for (var i = 0; i < filterArray.length; i++) {
                var current = filterArray[i];
                if (current.text.indexOf(filter_text) >= 0) {
                    current._visible(true);
                }
                else {
                    current._visible(false);
                }
            }
        }

        view_self.Buttons = {
            SaveBtn: {
                actionUrl: '',
                visible: ko.observable(true),
                text: ko.observable('保存'),
                callBack: null,
                disabled: ko.observable(false),
                extraParameter: {}
            },
            SaveBtnClick: function () {
                if (view_self.Buttons.SaveBtn.actionUrl == null || view_self.Buttons.SaveBtn.actionUrl == '') {
                    throw new Error('SaveBtn need config actionUrl !');
                }
                view_self.Buttons.SaveBtn.disabled(true);

                var hasData = [];
                for (var i = 0; i < view_self.rightData().length; i++) {
                    hasData.push(view_self.rightData()[i].data);
                }

                $.extend(view_self.Buttons.SaveBtn.extraParameter, { data: JSON.stringify(hasData) });

                $.ajax({
                    url: view_self.Buttons.SaveBtn.actionUrl,
                    type: "POST",
                    dataType: "json",
                    data: view_self.Buttons.SaveBtn.extraParameter,
                    success: function (result) {
                        if (result.Success) {
                            if (view_self.Buttons.SaveBtn.callBack != null) {
                                view_self.Buttons.SaveBtn.callBack(result);
                            }
                        }
                        view_self.Buttons.SaveBtn.disabled(false);
                    }
                });

            }
        }
    }

    //设置按钮
    jsonselect.SetButtons = function (butttons) {
        for (var btn in butttons) {
            if (butttons.hasOwnProperty(btn) && select_view.Buttons[btn] != null) {
                for (var prop in select_view.Buttons[btn]) {
                    if (select_view.Buttons[btn].hasOwnProperty(prop) && butttons[btn][prop] != null) {
                        if (ko.isObservable(select_view.Buttons[btn][prop])) {
                            select_view.Buttons[btn][prop](butttons[btn][prop]);
                        }
                        else {
                            select_view.Buttons[btn][prop] = butttons[btn][prop];
                        }
                    }
                }
            }
        }
    }

    var select_view = new SelectView();

    jsonselect.SetData = function (data) {
        for (var i = 0; i < data.leftData.length; i++) {
            CreateRowData(data.leftData[i]);
        }
        for (var i = 0; i < data.rightData.length; i++) {
            CreateRowData(data.rightData[i]);
        }
        select_view.leftData(data.leftData);
        select_view.rightData(data.rightData);
    }

    function CreateRowData(rowObj) {
        rowObj._visible = ko.observable(true);
    }

    jsonselect.ReLoad = function (param) {
        $.ajax({
            url: ajaxUrl,
            type: "POST",
            dataType: "json",
            data: { data: JSON.stringify(param) },
            success: function (result) {
                if (result.Success) {
                    jsonselect.SetData(result.Data);
                }
            }
        });
    }

    ko.applyBindings(select_view, document.getElementById(containerId));
}