/***
Wrapper/Helper Class for datagrid based on jQuery Datatable Plugin
***/
var Datatable = function () {

    var tableOptions; // main options
    var dataTable; // datatable object
    var table; // actual table jquery object
    var tableContainer; // actual table container object
    var tableWrapper; // actual table wrapper jquery object
    var tableInitialized = false;
    var ajaxParams = {}; // set filter mode
    var the;
    return {

        //main function to initiate the module
        init: function (options) {
            if (!$().dataTable) {
                return;
            }

            the = this;
            // default settings
            options = $.extend(true, {
                src: "", // actual table  
                filterApplyAction: "filter",
                filterCancelAction: "filter_cancel",
                resetGroupActionInputOnSuccess: true,
                loadingMessage: 'Loading...',
                dataTable: {
                    "dom": "t<'row DTTTFooter'<'pagging'p><'totalcount'is>>", // datatable layout
                    "pageLength": 15, // default records per page
                    "language": { // language settings
                        //// data tables spesific
                        "lengthMenu": " _MENU_ ",
                        "info": "<span class='seperator'></span>共 _TOTAL_ 条数据",
                        "infoEmpty": "",
                        "emptyTable": "暂无数据",
                        "zeroRecords": "暂无数据",
                        "sInfoFiltered": '',
                        select: {
                            rows: {
                                _: "<span class='seperator'>&nbsp;&nbsp;</span><span class='page-tip'>选中 %d 行</span>",
                                0: ''
                            }
                        },
                        "paginate": {
                            "previous": "上一页",
                            "next": "下一页",
                            "last": "最后一页",
                            "first": "第一页",
                            "page": "",
                            "pageOf": ""
                        }
                    },
                    "stripe": false,
                    "orderCellsTop": true,
                    "columnDefs": [{ // define columns sorting options(by default all columns are sortable extept the first checkbox column)
                        'orderable': false,
                        'targets': [0]
                    }],
                    columns: [
                        {
                            orderable: false,
                            render: function (data, type, full, row) {
                                return data === undefined ? '' : data;
                            }
                        }
                    ],
                    "pagingType": "simple_numbers",
                    "autoWidth": false, // disable fixed width and enable fluid table
                    'scrollX': true,
                    "processing": false, // enable/disable display message box on record load
                    "serverSide": true, // enable/disable server side ajax loading
                    stateSave: false,
                    'deferLoading': '',
                    "ajax": { // define ajax settings
                        "url": "", // ajax URL
                        "type": "POST", // request type
                        "timeout": 20000,
                        cache: false,
                        "data": function (data) { // add request parameters before submit
                            $.each(ajaxParams, function (key, value) {
                                data[key] = value;
                            });
                        },
                        "dataSrc": function (res) { // Manipulate the data returned from the server
                            if (res.Success === false) {
                                //ajax出错时，返回空数据
                                return [];
                            }
                            if (res.customActionStatus) {
                                if (tableOptions.resetGroupActionInputOnSuccess) {
                                    $('.table-group-action-input', tableWrapper).val("");
                                }
                            }
                            if (tableOptions.onSuccess) {
                                tableOptions.onSuccess.call(undefined, the, res);
                            }
                            return res.aaData;
                        },
                        "error": function () { // handle general connection errors
                            if (tableOptions.onError) {
                                tableOptions.onError.call(undefined, the);
                            }
                        }
                    },
                    "drawCallback": function (oSettings) { // run some code on table redraw
                        if (tableInitialized === false) { // check if table has been initialized
                            tableInitialized = true; // set table initialized
                            table.show(); // display table
                        }
                        $('table > thead > tr:first-child > th:first-child :checkbox', tableWrapper).prop('checked', false);
                        //多按钮处理
                        $('.table-popover').remove();
                        $('[data-toggle=table-popover]', table).each(function () {
                            $(this).popover({
                                html: !0,
                                placement: 'auto left',
                                container: 'body',
                                trigger: 'focus',
                                template: '<div class="popover table-popover"><div class="arrow"><\/div><h3 class="popover-title"><\/h3><div class="popover-content row"><\/div><\/div>'
                            }).on('show.bs.popover', function () { //展示时,关闭非当前所有弹窗
                                $(this).parents('tr').eq(0).siblings().find('[data-toggle="table-popover"]').popover('hide');
                            });
                        });
                        $('[data-toggle=table-popover]', table).hover(function () {
                            var $popover = $('#' + $(this).attr('aria-describedby'));
                            if ($popover.length <= 0) {
                                $(this).popover('show');
                            }
                        });
                        // callback for ajax data load
                        if (tableOptions.onDataLoad) {
                            tableOptions.onDataLoad.call(undefined, the);
                        }
                    }
                }
            }, options);
            if (!options.dataTable.serverSide) {
                options.dataTable.ajax = null;
            }
            tableOptions = options;
            // create table's jquery object
            table = $(options.src);
            tableContainer = table.parents(".table-container");

            //多选、单选
            if (options.dataTable.select === 'multiple') {
                var obj = {
                    orderable: false, className: 'select-checkbox',
                    width: '35px',
                    title: '<label><input type="checkbox"><span class="text"></span></label>',
                    render: function () {
                        return '';
                    }
                };
                options.dataTable.columns.unshift(obj);
                if (options.dataTable.order) {
                    for (var i = 0; i < options.dataTable.order.length; i++) {
                        options.dataTable.order[i][0] = options.dataTable.order[i][0] + 1;
                    }
                }
            } else if (options.dataTable.select === 'single') {
                var obj = {
                    orderable: false,
                    className: 'select-checkbox select-radio',
                    width: '35px',
                    title: '', render: function () {
                        return '';
                    }
                };
                options.dataTable.columns.unshift(obj);
                if (options.dataTable.order) {
                    for (var i = 0; i < options.dataTable.order.length; i++) {
                        options.dataTable.order[i][0] = options.dataTable.order[i][0] + 1;
                    }
                }

            }
            // initialize a datatable
            dataTable = table.DataTable(options.dataTable);
            // get table wrapper
            tableWrapper = table.parents('.dataTables_wrapper');
            //全选、反选
            $('table > thead > tr:first-child > th:first-child :checkbox', tableWrapper).click(function () {
                var $che = $(this).get(0);
                if ($che.checked) {
                    dataTable.rows().select();
                } else {
                    dataTable.rows().deselect();
                }
            });



        },

        setAjaxParam: function (name, value) {
            ajaxParams[name] = value;
        },

        addAjaxParam: function (name, value) {
            if (!ajaxParams[name]) {
                ajaxParams[name] = [];
            }

            skip = false;
            for (var i = 0; i < (ajaxParams[name]).length; i++) { // check for duplicates
                if (ajaxParams[name][i] === value) {
                    skip = true;
                }
            }

            if (skip === false) {
                ajaxParams[name].push(value);
            }
        },

        clearAjaxParams: function () {
            ajaxParams = {};
        },
        getAjaxParams: function () {
            return ajaxParams;
        },
        getDataTable: function () {
            return dataTable;
        },

        getTableWrapper: function () {
            return tableWrapper;
        },

        gettableContainer: function () {
            return tableContainer;
        },

        getTable: function () {
            return table;
        },
        reload: function () {
            dataTable.ajax.reload(null, true);
        },
        refresh: function () {
            dataTable.ajax.reload(null, false);
        },
        addRowData: function (data) {//添加一行数据
            dataTable.row.add(data).draw().node();
        },
        //获取当前记录数
        getRowsCount: function () {
            return the.getDataTable().context[0]._iRecordsTotal;
        },
        getRowsData: function () {
            var data = [];
            the.getDataTable().rows().every(function (rowIdx, tableLoop, rowLoop) {
                data.push(this.data())
            });
            return data;
        },
        getRows: function () {
            return the.getDataTable().rows();
        },
        getPopOverBtns: function (btns) {//获取多按钮
            var html = '<span\
                        data-toggle="table-popover"\
                        data-content="{0}">{1}</span>'.format(btns, '<i class="fa fa-cogs"></i>');
            return html;
        },
        getSelectedRowsData: function () {//获取选中行的数据
            var rows = [];
            $('tbody > tr.selected', table).each(function () {
                rows.push(dataTable.row($(this)).data());
            });
            return rows;
        }

    };

};