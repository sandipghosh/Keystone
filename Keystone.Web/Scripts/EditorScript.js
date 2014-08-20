/// <reference path="jquery-2.1.0-vsdoc.js" />
/// <reference path="common-script.js" />

(function ($, win) {
    var setWatermarkExecutionCounter = 0;
    $.validator.addMethod(
        "multiemail",
         function (value, element) {
             if (this.optional(element)) // return true on optional element 
                 return true;
             var emails = value.split(/[;,]+/); // split element by , and ;
             valid = true;
             for (var i in emails) {
                 value = emails[i];
                 valid = valid &&
                    $.validator.methods.email.call(this, $.trim(value), element);
             }
             return valid;
         },

        $.validator.messages.email
    );

    $(document).ready(function () {
        try {

        } catch (ex) {
            console.log(ex);
        }
    });

    this.ModalCallbackForPreview = function ($modal) {
        try {
            var $self = $modal;
            var imageUrl = $self.find('#PreviewImageSaveDraft').attr('src');
            var $img = $('<img>', { src: imageUrl });

            $img.load(function () {
                var h = this.height, w = this.width;

                $self.find('#lnkPreviewFullScreen')
                    .attr('href', imageUrl)
                    .popupWindow({ height: h, width: w, scrollbars: 1 });

                var printUrl = '{0}/Editor/PrintPreview/{1}/{2}/{3}'.format(virtualDirectory,
                    h, w, encodeURIComponent(Base64Encode(imageUrl)));

                $self.find('#lnkPrint')
                    .attr('href', printUrl)
                    .popupWindow({ height: h, width: w, scrollbars: 1 });
            });

            EmailShareValidationSetup();
            $('#disclaimer-watermark').flash();

        } catch (ex) {
            console.log(ex);
        }
    };

    this.SaveDraftDialogOpenCallback = function ($modal) {
        try {
            $modal.find('.image-navigation-wrapper').css('margin-left', '0');
            CreateUserValidationSetup();
            SignInUserValidationSetup('#frmSignInUser');
            DraftValidationSetup();
        } catch (ex) {
            console.log(ex);
        }
    };

    var Draft = function (draftId, draftName, templateId, deliveryScheduleId, quantity, price) {
        this.DraftId = draftId;
        this.DraftName = draftName;
        this.TemplateId = templateId;
        this.DeliveryScheduleId = deliveryScheduleId;
        this.Quantity = quantity;
        this.Price = price;
        this.DraftPages = [];
    };

    var DraftPage = function (templateId, templatePageId,
        draftPreviewUrl, finalImageUrl, draftJsonString) {
        this.TemplateId = templateId;
        this.TemplatePageId = templatePageId;
        this.DraftPreviewUrl = draftPreviewUrl;
        this.DraftJsonString = draftJsonString;
        this.FinalImageUrl = finalImageUrl;
    };

    this.SetWatermarkForPreview = function (uri) {
        try {
            editorCanvas.canvas.deactivateAll();
            var dataUrl = editorCanvas.canvas.toDataURL({
                format: 'jpeg',
                quality: 0.8
            });

            var rawData = dataUrl.replace('data:image/jpeg;base64,', '');
            var jsonData = editorCanvas.getJsonDataFromCanvas(false);

            $.ajax({
                type: 'POST',
                url: '{0}/Editor/SetWatermarkForPreview'.format(virtualDirectory),
                data: JSON.stringify({
                    'imageRawData': rawData,
                    'imageJSONData': jsonData
                }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (draftImage, textStatus, jqXHR) {
                    if (draftImage) {
                        if (draftImage.Status === 'success') {
                            sessionStorage.setItem('DRAFT_IMAGE_URL', draftImage.Url);
                            $.ajax({
                                url: uri,
                                type: 'GET',
                                success: function (result, textStatus, jqXHR) {
                                    if (result) {
                                        modal.open({
                                            content: result,
                                            width: (typeof windowWidth === 'undefined') ? '735px' : windowWidth,
                                            openCallBack: function (element) {
                                                var $self = element;
                                                $self.find('img.print_ready_image').attr('src', draftImage.Url)
                                                    .MakeImageProportionate(editorCanvas.canvas.height, editorCanvas.canvas.width);
                                                $self.find('img.print_ready_image').center('H');

                                                $self.find('#lnkPreviewFullScreen')
                                                    .attr('href', sessionStorage.getItem("DRAFT_IMAGE_URL"))
                                                    .popupWindow({
                                                        height: editorCanvas.canvas.height,
                                                        width: editorCanvas.canvas.width,
                                                        scrollbars: 1
                                                    });

                                                var printUrl = '{0}/Editor/PrintPreview/{1}/{2}/{3}'.format(virtualDirectory, editorCanvas.canvas.height,
                                                    editorCanvas.canvas.width, encodeURIComponent(Base64Encode(sessionStorage.getItem("DRAFT_IMAGE_URL"))));

                                                $self.find('#lnkPrint')
                                                    .attr('href', printUrl)
                                                    .popupWindow({
                                                        height: editorCanvas.canvas.height,
                                                        width: editorCanvas.canvas.width
                                                    });
                                            }
                                        });
                                    }
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    alert(textStatus);
                                }
                            });
                        }
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus);
                }
            });

        } catch (ex) {
            console.log(ex);
        }
    };

    this.PreviewDraft = function (uri, templateid, pageid) {
        try {
            SetDraftData(templateid, pageid, function () {
                $.ajax({
                    url: uri,
                    type: 'GET',
                    success: function (result, textStatus, jqXHR) {
                        if (result) {
                            modal.open({
                                content: result,
                                width: (typeof windowWidth === 'undefined') ? '735px' : windowWidth,
                                openCallBack: function (element) {
                                    var $self = element;
                                    var previewImageUrl = Base64Decode(sessionStorage.getItem('{0}_{1}_DRAFT_URL'.format(templateid, pageid)));

                                    $self.find('img.print_ready_image').attr('src', previewImageUrl)
                                        .MakeImageProportionate(editorCanvas.canvas.height, editorCanvas.canvas.width);
                                    $self.find('img.print_ready_image').center('H');

                                    $self.find('#lnkPreviewFullScreen')
                                        .attr('href', previewImageUrl)
                                        .popupWindow({
                                            height: editorCanvas.canvas.height,
                                            width: editorCanvas.canvas.width,
                                            scrollbars: 1
                                        });

                                    var printUrl = '{0}/Editor/PrintPreview/{1}/{2}/{3}'.format(virtualDirectory, editorCanvas.canvas.height,
                                        editorCanvas.canvas.width, encodeURIComponent(Base64Encode(previewImageUrl)));

                                    $self.find('#lnkPrint')
                                        .attr('href', printUrl)
                                        .popupWindow({
                                            height: editorCanvas.canvas.height,
                                            width: editorCanvas.canvas.width
                                        });
                                }
                            });
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            });
        } catch (ex) {
            console.log(ex);
        }
    };

    var SetDraftData = function (templateid, pageid, callback) {
        try {
            editorCanvas.canvas.deactivateAll();
            var jsonString = editorCanvas.getJsonDataFromCanvas(true);

            if (setWatermarkExecutionCounter == 0) {
                setWatermark(templateid, pageid, function (imageUrl) {
                    sessionStorage.setItem('CURRENT_EDITED_IMAGE', '{0}_{1}'.format(templateid, pageid));
                    sessionStorage.setItem('{0}_{1}_DRAFT_URL'.format(templateid, pageid), imageUrl.draftImageUrl);
                    sessionStorage.setItem('{0}_{1}_FINAL_URL'.format(templateid, pageid), imageUrl.finalImageUrl);
                    sessionStorage.setItem('{0}_{1}_JSON'.format(templateid, pageid), jsonString);

                    if (callback) {
                        callback();
                    }
                });

                setWatermarkExecutionCounter += 1;
            }
            else {
                if (callback) {
                    callback();
                }
                setWatermarkExecutionCounter = 0;
            }

            /*var imageUrl = setWatermark(templateid, pageid);

            sessionStorage.setItem('CURRENT_EDITED_IMAGE', '{0}_{1}'.format(templateid, pageid));
            sessionStorage.setItem('{0}_{1}_DRAFT_URL'.format(templateid, pageid), imageUrl.draftImageUrl);
            sessionStorage.setItem('{0}_{1}_FINAL_URL'.format(templateid, pageid), imageUrl.finalImageUrl);
            sessionStorage.setItem('{0}_{1}_JSON'.format(templateid, pageid), jsonString);*/
        } catch (ex) {
            console.log(ex);
        }
    };

    var setWatermark = function (templateid, pageid, callback) {
        try {
            try { $.blockUI({ message: $("#dataloading") }); } catch (ex) { }
            var dataUrl = Base64Encode(editorCanvas.convertCanvasToSVG({
                originalWidth: parseInt($('#PageWidth').val()),
                originalHeight: parseInt($('#PageHeight').val()),
                templateId: parseInt($('#TemplateId').val())
            }));
            var rawData = dataUrl;

            var watermarkImageUrl = {
                draftImageUrl: '',
                finalImageUrl: ''
            };

            /*Alternative option of JSON post*/
            $('#imageRawData').val(rawData);
            $('#templateId').val(templateid);
            $('#pageId').val(pageid);

            var options = {
                dataType: 'json',
                beforeSubmit: function (arr, $form, options) { },
                success: function (responseData, statusText, xhr, frmElement) {
                    if (responseData) {
                        if (responseData.Status === 'success') {
                            watermarkImageUrl.draftImageUrl = responseData.draftUrl;
                            watermarkImageUrl.finalImageUrl = responseData.finalUrl;

                            if (callback) {
                                callback(watermarkImageUrl);
                                try { $.unblockUI(); } catch (ex) { }
                            }
                        }
                    }
                },
                uploadProgress: function (event, position, total, percentComplete) {
                    console.log(position);
                    console.log(total);
                    console.log(percentComplete);
                },
                error: function (response, status, err) {
                    alert(response);
                    try { $.unblockUI(); } catch (ex) { }
                }
            };
            $('#frmCanvasData').ajaxSubmit(options);
        } catch (ex) {
            console.log(ex);
        }
        return '';
    };

    var SaveDraft = function (draftId, draftName, backToEditorPage) {
        try {
            var templateid = sessionStorage.getItem('SELECTED_TEMPLATE_ID');
            var deliveryId = sessionStorage.getItem('SELECTED_TEMPLATE_DELIVERY_{0}'.format(templateid));
            var quantity = sessionStorage.getItem('SELECTED_TEMPLATE_QUANTITY_{0}'.format(templateid));
            var price = sessionStorage.getItem('SELECTED_TEMPLATE_PRICE_{0}'.format(templateid));

            var deliveryScheduleId = deliveryId;
            var draft = new Draft(draftId, draftName, templateid, deliveryScheduleId, quantity, price);
            draft.DraftPages = GetDraftPageInfoFromSessionStorage(templateid);
            var processUrl = '{0}/Draft/{1}'.format(virtualDirectory, draftId > 0 ? 'UpdateDraft' : 'SaveDraft');

            $.ajax({
                type: 'POST',
                url: processUrl,
                data: JSON.stringify(draft),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result, textStatus, jqXHR) {
                    if (result) {
                        if (result.Status) {
                            if (backToEditorPage) {
                                $('#DraftId').val(result.DraftId);
                                modal.close();
                            } else {
                                if (processUrl.endsWith('UpdateDraft')) {
                                    ShowModal('{0}/Editor/GetOrderPrint?templateId={1}&deliveryId={2}&quantity={3}&price={4}'
                                        .format(virtualDirectory, result.TemplateId, result.DeliveryId, parseInt(result.Quantity), result.Price), null,
                                        null, OrderPrintOpenModalCallback);
                                } else {
                                    ShowModal('{0}/Editor/GetOrderPrint?templateId={1}&deliveryId={2}&quantity={3}&price={4}'
                                        .format(virtualDirectory, templateid, deliveryId, quantity, price), null, null,
                                        OrderPrintOpenModalCallback);
                                }
                            }
                        }
                        else {
                            alert(result.Message);
                        }
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus);
                }
            });
        } catch (ex) {
            console.log(ex);
        }
    };

    var OrderPrintOpenModalCallback = function () {
        try {
            $('#disclaimer').flash();
        } catch (ex) {
            console.log(ex);
        }
    };

    this.NavigatePage = function (pageurl, templateid, pageid) {
        try {
            var previousPageId = $('.page-navigator li.selected').data('pageid');
            SetDraftData(templateid, parseInt(previousPageId), function () {
                win.location.href = pageurl;
            });
        } catch (ex) {
            console.log(ex);
        }
    };

    this.CreatedPreviewOfAllPageForATemplate = function (url, templateid, pageid, modalCallback) {
        try {
            SetDraftData(templateid, pageid, function () {
                var draftUrl = Enumerable.From(GetDraftPageInfoFromSessionStorage(templateid))
                    .Select(function (x) { return x.DraftPreviewUrl; }).ToArray();

                $.ajax({
                    type: 'POST',
                    url: '{0}/Editor/CreatedPreviewOfAllPageForATemplate'.format(virtualDirectory),
                    data: JSON.stringify({
                        'draftUrl': draftUrl,
                        'templateId': templateid
                    }),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (result, textStatus, jqXHR) {
                        if (result) {
                            if (result.Status == 'success') {
                                for (var i = 0; i < result.Drafts.length; i++) {
                                    sessionStorage.setItem('{0}_{1}_DRAFT_URL'.format
                                        (result.Drafts[i].TemplateId, result.Drafts[i].TemplatePageId), result.Drafts[i].DraftImageUrl);
                                    sessionStorage.setItem('{0}_{1}_FINAL_URL'.format
                                        (result.Drafts[i].TemplateId, result.Drafts[i].TemplatePageId), result.Drafts[i].FileImageUrl);
                                    sessionStorage.setItem('{0}_{1}_JSON'.format
                                        (result.Drafts[i].TemplateId, result.Drafts[i].TemplatePageId), result.Drafts[i].DraftImageJson);
                                }

                                var data = {
                                    drafts: GetDraftPageInfoFromSessionStorage(templateid),
                                    templateId: result.TemplateId
                                };

                                $.ajax({
                                    url: url,
                                    data: (url.hasQueryString() === false ? JSON.stringify(data) : null),
                                    type: (url.hasQueryString() === false ? 'POST' : 'GET'),
                                    contentType: (url.hasQueryString() === false ?
                                        'application/json; charset=utf-8' : 'application/x-www-form-urlencoded; charset=UTF-8'),
                                    success: function (result, textStatus, jqXHR) {
                                        modal.open({
                                            content: result,
                                            width: (typeof windowWidth === 'undefined') ? '735px' : windowWidth,
                                            openCallBack: modalCallback
                                        });
                                    },
                                    error: function (jqXHR, textStatus, errorThrown) {
                                        alert(textStatus);
                                    }
                                });
                            }
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            });

        } catch (ex) {
            console.log(ex);
        }
    };

    var GetDraftPageInfoFromSessionStorage = function (templateid) {
        var draftPages = [];
        try {
            var pageIds = [];
            for (var key in sessionStorage) {
                if (key.startsWith(templateid.toString())) {
                    var pageKey = key.split('_')[1];
                    if ($.inArray(pageKey, pageIds) < 0) {
                        pageIds.push(pageKey);
                    }
                }
            }

            if (pageIds.length > 0) {
                for (var i = 0; i < pageIds.length; i++) {
                    var draftPage = new DraftPage(templateid, pageIds[i],
                        sessionStorage.getItem('{0}_{1}_DRAFT_URL'.format(templateid, pageIds[i])),
                        sessionStorage.getItem('{0}_{1}_FINAL_URL'.format(templateid, pageIds[i])),
                        sessionStorage.getItem('{0}_{1}_JSON'.format(templateid, pageIds[i])));

                    draftPages.push(draftPage);
                }
            }

        } catch (ex) {
            console.log(ex);
        }
        return draftPages;
    };

    this.NavigatePageOnClient = function (url, element) {
        try {
            $('#image-preloader').css({ 'height': $('#PreviewImageSaveDraft').height + 'px' }).show();
            $('#PreviewImageSaveDraft').one('load', function () {
                $('#image-preloader').hide();

                var imageUrl = $(this).attr('src');
                var h = $(this).height(), w = $(this).width();

                $('#lnkPreviewFullScreen').attr('href', imageUrl)
                    .popupWindow({ height: h, width: w, scrollbars: 1 });

                var printUrl = '{0}/Editor/PrintPreview/{1}/{2}/{3}'.format(virtualDirectory,
                    h, w, encodeURIComponent(Base64Encode(imageUrl)));

                $('#lnkPrint').attr('href', printUrl)
                    .popupWindow({ height: h, width: w, scrollbars: 1 });

            }).attr('src', url + '?' + Math.random());
            $(element).closest('ul.page-navigator').find('li').removeClass('selected');
            $(element).parent('li').addClass('selected');
        } catch (ex) {
            console.log(ex);
        }
    };

    this.ContinueAndSave = function () {
        try {
            modal.close();
            var draftId = parseInt($('#DraftId').val());

            if (!isNaN(draftId) && draftId > 0) {
                $.blockUI({ message: $("#dataloading") });
                SaveDraft(draftId, '');
            }
            else {
                var url = '{0}/Editor/GetSaveDesign'.format(virtualDirectory);
                var currentTemplate = win.sessionStorage.getItem('CURRENT_EDITED_IMAGE').toString().split('_');
                if (currentTemplate.length > 0) {
                    CreatedPreviewOfAllPageForATemplate(url, parseInt(currentTemplate[0]),
                        parseInt(currentTemplate[1]), SaveDraftDialogOpenCallback);
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    };

    this.PrceedDirectOrderPrint = function (url, templateid, pageid, modalCallback) {
        try {
            var draftId = parseInt($('#DraftId').val());
            if (!isNaN(draftId) && draftId > 0) {
                var deliveryId = sessionStorage.getItem('SELECTED_TEMPLATE_DELIVERY_{0}'.format(templateid));
                var quantity = sessionStorage.getItem('SELECTED_TEMPLATE_QUANTITY_{0}'.format(templateid));
                var price = sessionStorage.getItem('SELECTED_TEMPLATE_PRICE_{0}'.format(templateid));

                var alternetUrl = '{0}/Editor/GetOrderPrint?templateId={1}&deliveryId={2}&quantity={3}&price={4}'
                    .format(virtualDirectory, (templateid === null ? 0 : templateid),
                        (deliveryId === null ? 0 : deliveryId), (quantity === null ? 0 : quantity),
                        (price === null ? 0 : price));

                CreatedPreviewOfAllPageForATemplate(alternetUrl, templateid, pageid);
            }
            else {
                CreatedPreviewOfAllPageForATemplate(url, templateid, pageid, modalCallback);
            }
        } catch (ex) {
            console.log(ex);
        }
    };

    var CreateUserValidationSetup = function () {
        try {
            var validationData = GetValidationErrorMarkup();
            validationData.rules = {
                FirstName: {
                    required: true
                },
                LastName: {
                    required: true
                },
                EmailId: {
                    required: true, email: true
                }
            };
            validationData.messages = {
                FirstName: "User's first name is required to register",
                LastName: "User's last name is required to register",
                EmailId: {
                    required: "User's email id is required to register",
                    email: "User's email id is invalid"
                }
            };
            $('#frmCreateUserAccount').validate(validationData);
        } catch (ex) {
            console.log(ex);
        }
    };

    var SignInUserValidationSetup = function (formId) {
        try {
            var validationData = GetValidationErrorMarkup();
            validationData.rules = {
                UserId: {
                    required: true
                },
                Password: {
                    required: true
                }
            };
            validationData.messages = {
                UserId: "UserId is required to register",
                Password: "Password is required to register"
            };
            $(formId).validate(validationData);
        } catch (ex) {
            console.log(ex);
        }
    };

    var EmailShareValidationSetup = function () {
        try {
            var validationData = GetValidationErrorMarkup();
            validationData.rules = {
                recipient: {
                    required: true
                },
                emailId: {
                    required: true, multiemail: true
                },
                comment: {
                    required: true
                }
            };
            validationData.messages = {
                recipient: "Recipient is required to share the image",
                comment: "Comment is required to share the image",
                emailId: {
                    required: "Recipient's email id is required to share the image",
                    multiemail: "Recipient's email id is invalid"
                }
            };
            $('#frmShareDraft').validate(validationData);
        } catch (ex) {
            console.log(ex);
        }
    };

    var DraftValidationSetup = function () {
        try {
            var validationData = GetValidationErrorMarkup();
            validationData.rules = {
                draftName: {
                    required: true
                }
            };
            validationData.messages = {
                draftName: "Draft name is required"
            };
            $('#frmDraftName').validate(validationData);
        } catch (ex) {
            console.log(ex);
        }
    };

    var IsDraftValid = function () {
        try {
            //$('#draftName').wrap('<form id="frmDraftName" />');
            $('#frmDraftName').validate({
                rules: {
                    draftName: {
                        required: true
                    }
                },
                errorElement: 'span',
                errorPlacement: function (error, element) {
                    element.addClass('input-validation-error');
                    error.addClass('field-validation-error')
				        .insertBefore(element);
                },
                success: function (element) {
                    element.next('input, select, textarea')
				        .removeClass('input-validation-error');
                    element.remove();
                }
            });
            var isValid = $('#frmDraftName').valid();
            //$('#draftName').unwrap();
            return isValid;
        } catch (ex) {
            console.log(ex);
        }
    };

    this.SaveAndProceedToShoppingCart = function (url) {
        try {
            if ($('#frmDraftName').valid()) {
                if ($('#create-account').is(':visible')) {
                    $("#frmCreateUserAccount").attr('data-ajax-success', 'SaveDraftToContinue(data, false)').submit();
                }
                else if ($('#signin-account').is(':visible')) {
                    $("#frmSignInUser").attr('data-ajax-success', 'SaveDraftToContinue(data, false)').submit();
                }
                else {
                    SaveDraft(0, $('#draftName').val());
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    };

    this.SaveAsDraft = function () {
        try {
            if ($('#frmDraftName').valid()) {
                if ($('#create-account').is(':visible')) {
                    $("#frmCreateUserAccount").attr('data-ajax-success', 'SaveDraftToContinue(data, true)').submit();
                }
                else if ($('#signin-account').is(':visible')) {
                    $("#frmSignInUser").attr('data-ajax-success', 'SaveDraftToContinue(data, true)').submit();
                }
                else {
                    var draftId = parseInt($('#DraftId').val());
                    SaveDraft((isNaN(draftId) ? 0 : draftId), $('#draftName').val(), true);
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    };

    this.SaveDraftToContinue = function (data, backToEditorPage) {
        try {
            if (data) {
                if (data.Status) {
                    SaveDraft(0, $('#draftName').val(), backToEditorPage);
                }
                else {
                    alert(data.Message);
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    };

    this.GotoShoppingCart = function () {
        try {
            var templateid = sessionStorage.getItem('SELECTED_TEMPLATE_ID');
            var deliveryId = sessionStorage.getItem('SELECTED_TEMPLATE_DELIVERY_{0}'.format(templateid));
            var quantity = sessionStorage.getItem('SELECTED_TEMPLATE_QUANTITY_{0}'.format(templateid));
            var price = sessionStorage.getItem('SELECTED_TEMPLATE_PRICE_{0}'.format(templateid));

            if (templateid !== null || templateid !== undefined)
                $('#templateId').val(templateid);
            if (deliveryId !== null || deliveryId !== undefined)
                $('#deliveryId').val(deliveryId);
            if (quantity !== null || quantity != undefined)
                $('#quantity').val(quantity);
            if (price !== null || price !== undefined)
                $('#price').val(price);

            $('#frmAddOrder').submit();
        } catch (ex) {
            console.log(ex);
        }
    };

    this.ShareDraftSuccessCallback = function (data) {
        try {
            if (data) {
                if (data.Status) {
                    $('#share-template').hide();
                    alert("Share successfully made.");
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    };

    this.ShareDraftBeginCallback = function () {
        try {
            $.blockUI({ message: $("#dataloading") });
        } catch (ex) {
            console.log(ex);
        }
    };

    this.ShareDraftCompletionCallback = function () {
        try {
            $.unblockUI();
        } catch (ex) {
            console.log(ex);
        }
    };

    this.ShareDraft = function () {
        try {
            if ($('#frmShareDraft').valid()) {
                $("#frmShareDraft").submit();
            }
        } catch (ex) {
            console.log();
        }
    };

    this.ShowShareWindow = function () {
        try {
            $('#frmShareOrderItem .field-validation-error').remove();
            $('#frmShareOrderItem').find('input, select, textarea')
				.removeClass('input-validation-error');
            $('#share-template').show();
        } catch (ex) {
            console.log(ex);
        }
    };

}(jQuery, window));
