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
            if (schemaData != '') {
                var schemaJsonData = JSON.parse(Base64Decode(schemaData));
                var $gridElement = $('#grid');

                if (schemaJsonData) {
                    $gridElement.SetupGrid({
                        modelSchema: schemaJsonData,
                        //rowNum: 2,
                        datatype: 'json',
                        pagerid: '#pager',
                        renderURL: '{0}/Admin/TestimonialManager/GetTestimonial'.format(virtualDirectory),
                        editURL: '{0}/Admin/TestimonialManager/SetTestimonial'.format(virtualDirectory),
                        editable: true,
                        searchOperators: true,
                        addCommandTitle: 'Add Testimonial',
                        exportCallback_Func: function (data) {
                            if (data) {
                                var criteria = $gridElement.jqGrid('getGridParam', 'postData').filters;
                                criteria = (criteria == undefined || criteria == null) ? '' : Base64Encode(criteria);

                                $.fileDownload('{0}/Admin/TestimonialManager/Export?filters={1}'.format(virtualDirectory, criteria))
                                    .done(function () { alert('File download a success!'); })
                                    .fail(function () { alert('File download failed!'); });
                            }
                        },
                        insert_func: function () {
                            insertGridRow();
                        }
                    });
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    });

    this.insertGridRow = function () {
        try {
            lastSelId = GetNewRowID();
            var parameters = {
                rowID: lastSelId,
                initdata: { TestimonialId: 0, StatusId: 1, DisplayOrder: 0 },
                position: "first"
            };
            gridOperationMode = OperationMode.Insert;
            var $gridElement = $('#grid');

            $gridElement.jqGrid('addRow', parameters);
            $gridElement.changeMode({ rowid: lastSelId, mode: gridRowMode.Edit });

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