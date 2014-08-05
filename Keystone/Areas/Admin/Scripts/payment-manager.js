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

                if (schemaJsonData instanceof Object == true) {
                    $gridElement = $('#grid');

                    $gridElement.SetupGrid({
                        modelSchema: schemaJsonData,
                        datatype: 'json',
                        pagerid: '#pager',
                        renderURL: '{0}/Admin/PaymentManager/GetPaymentInfos'.format(virtualDirectory),
                        recordtext: 'Total Orders {2}',
                        searchOperators: true,
                        grouping: true,
                        insertMode: 'none',
                        exportCallback_Func: function (data) {
                            if (data) {
                                var criteria = $gridElement.jqGrid('getGridParam', 'postData').filters;
                                criteria = (criteria == undefined || criteria == null) ? '' : Base64Encode(criteria);

                                $.fileDownload('{0}/Admin/PaymentManager/Export?filters={1}'.format(virtualDirectory, criteria))
                                    .done(function () { alert('File download a success!'); })
                                    .fail(function () { alert('File download failed!'); });
                            };
                        }
                        //editURL: '{0}/Admin/TestimonialManager/SetTestimonial'.format(virtualDirectory),
                        //editable: true,
                        //addCommandTitle: 'Add Customer',
                        //insert_func: function () {
                        //    insertGridRow();
                        //}
                    });
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    });
}(jQuery, window));