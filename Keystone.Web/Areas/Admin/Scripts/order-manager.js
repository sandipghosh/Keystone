/// <reference path="../../../Scripts/jquery-2.1.0-vsdoc.js" />
/// <reference path="../../../Scripts/jquery-2.1.0.min.js" />
/// <reference path="../../../Scripts/common-script.js" />

/// <reference path="jquery.jqGrid.min.js" />
/// <reference path="jquery.linq-vsdoc.js" />
/// <reference path="grid-intregration.js" />

(function ($, win) {
    var gridOperationMode;

    $(document).ready(function () {
        try {
            var schemaData = $('#SchemaData').val();
            var schemaSubData = $('#SchemaSubData').val();

            if (schemaData != '' && schemaSubData != '') {
                var schemaJsonData = JSON.parse(Base64Decode(schemaData));
                var schemaJsonSubData = JSON.parse(Base64Decode(schemaSubData));

                if (schemaJsonData instanceof Object == true &&
                    schemaJsonSubData instanceof Object == true) {
                    $gridElement = $('#grid');

                    var subGridHandler = subGridRowExpandedHandler({
                        modelData: schemaJsonSubData,
                        renderURL: '{0}/Admin/OrderManager/GetOrderItems'.format(virtualDirectory),
                        
                        parentGridElement: $gridElement,
                        keyFieldName: 'OrderId',
                        recordtext: 'Total Order Items {2}',
                    });

                    $gridElement.SetupGrid({
                        modelSchema: schemaJsonData,
                        datatype: 'json',
                        pagerid: '#pager',
                        renderURL: '{0}/Admin/OrderManager/GetOrders'.format(virtualDirectory),
                        recordtext: 'Total Orders {2}',
                        searchOperators: true,
                        grouping: true,
                        subGrid: true,
                        subGridOptions: defaultSubGridOptions,
                        subGridRowExpanded: subGridHandler.handler,
                        exportCallback_Func: function (data) {
                            if (data) {
                                var criteria = $gridElement.jqGrid('getGridParam', 'postData').filters;
                                criteria = (criteria == undefined || criteria == null) ? '' : Base64Encode(criteria);

                                $.fileDownload('{0}/Admin/OrderManager/Export?filters={1}'.format(virtualDirectory, criteria))
                                    .done(function () { alert('File download a success!'); })
                                    .fail(function () { alert('File download failed!'); });
                            };
                        },
                        editURL: '{0}/Admin/OrderManager/SetOrder'.format(virtualDirectory),
                        insertMode: 'none',
                        editable: true,
                        gridComplete_Func: function () {
                            var ids = $gridElement.jqGrid('getDataIDs');
                            for (var i = 0; i < ids.length; i++) {
                                var orderId = $gridElement.jqGrid('getCell', ids[i], 'OrderId');

                                var $downloadPDF = $("<span class=\"grid-command command_download_pdf\" title=\"Download order as PDF\" onclick=\"{0}({1},'{2}')\"></span>".format('DownloadOrder', orderId, 'PDF'));
                                var $downloadZip = $("<span class=\"grid-command command_download_zip\" title=\"Download order as Zip\" onclick=\"{0}({1},'{2}')\"></span>".format('DownloadOrder', orderId, 'Archive'));

                                //var $downloadPDF = $("<a href=\"{0}/Admin/OrderManager/DownloadOrderedFile?orderId={1}&drownloadType={2}\"><span class=\"grid-command command_download_pdf\" title=\"Download order as PDF\"></span></a>".format(virtualDirectory, orderId, 'PDF'));
                                //var $downloadZip = $("<a href=\"{0}/Admin/OrderManager/DownloadOrderedFile?orderId={1}&drownloadType={2}\"><span class=\"grid-command command_download_zip\" title=\"Download order as Zip\"></span></a>".format(virtualDirectory, orderId, 'Archive'));

                                $gridElement.jqGrid('setRowData', ids[i], {
                                    Download: $downloadPDF[0].outerHTML //+ $downloadZip[0].outerHTML
                                });
                            }
                        }
                    });
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    });

    this.DownloadOrder = function (orderId, downloadType) {
        try {

            $.fileDownload('{0}/Admin/OrderManager/DownloadOrderedFile?orderId={1}&drownloadType={2}'
                .format(virtualDirectory, orderId, downloadType), {
                    prepareCallback: function (url) {
                        try { $.blockUI({ message: $("#dataloading") }); } catch (ex) { }
                    },
                    successCallback: function (url) {
                        try { $.unblockUI(); } catch (ex) { }
                    },
                    failCallback: function (responseHtml, url) {
                        try { $.unblockUI(); } catch (ex) { }
                    }
                });
                
                //.done(function () { alert('File download a success!'); })
                //.fail(function () { alert('File download failed!'); });
        } catch (ex) {
            console.log(ex);
        }
    }

    this.editGridRow = function (rowId) {
        try {
            var $gridElement = $('#grid');
            lastSelId = rowId;
            gridOperationMode = OperationMode.Edit;
            $gridElement.jqGrid('setSelection', rowId, true);
            $gridElement.changeMode({ rowid: rowId, mode: gridRowMode.Edit });

            var editparameters = {
                keys: false,
                oneditfunc: function (rowid) {
                    $('#{0}_Status'.format(rowid)).change(function (e) {
                        var $self = $(this);
                        var selectedValue = $(this).find('option:selected').val();
                        $('#{0}_StatusId'.format(rowid)).val(selectedValue);
                    });
                },
                errorfunc: function (rowid) {
                    restoreGridRow(rowId);
                }
            };

            $gridElement.jqGrid('editRow', rowId, editparameters);

        } catch (ex) {
            console.log(ex);
        }
    };

    this.saveGridRow = function (rowId) {
        try {
            var $gridElement = $('#grid');
            var saveparameters = {
                keys: false,
                aftersavefunc: function (rowid) {
                    restoreGridRow(rowId);
                },
                errorfunc: function (rowid, result, postedData) {
                    alert(result.responseText);
                    restoreGridRow(rowId);
                },
                extraparam: { GridMode: gridOperationMode }
            };
            $gridElement.jqGrid('saveRow', rowId, saveparameters);
        } catch (ex) {
            console.log(ex);
        }
    };

    this.restoreGridRow = function (rowId) {
        try {
            var $gridElement = $('#grid');
            $gridElement.jqGrid('restoreRow', rowId, function (rowid) {
                $gridElement.changeMode({ rowid: rowId, mode: gridRowMode.Normal });
                $.unblockUI();
            });
        }
        catch (ex) { }
    }

}(jQuery, window));