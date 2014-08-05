/// <reference path="jquery-2.1.0-vsdoc.js" />
/// <reference path="common-script.js" />
/// <reference path="knockout-3.0.0.js" />
/// <reference path="knockout.mapping-2.4.1.js" />
/// <reference path="linq-vsdoc.js" />
/// <reference path="jquery-ui-vsdoc.js" />

(function ($, win) {
    var OrderItemViewModel = ko.observable();

    ko.bindingHandlers.radioCheck = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            try {
                var $self = $(element);
                var value = valueAccessor()();
                radioCheckUncheck($self, value);
            } catch (ex) {
                console.log(ex);
            }
        }
    };

    var radioCheckUncheck = function ($element, value) {
        if ($element.val() == value.toString()) {
            $element.attr('checked', 'checked');
        }
        else {
            $element.removeAttr('checked');
        }
    }

    ko.bindingHandlers.showDescription = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            try {
                var fullDesccription = valueAccessor()();
                $.each(fullDesccription.split('#'), function (index, value) {
                    $(element).append('<li>{0}</li>'.format(value));
                });
            } catch (ex) {
                console.log(ex);
            }
        }
    };

    ko.bindingHandlers.populateTemplatePriceByDelivery = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            try {
                $(element).click(function () {
                    var $self = $(this);
                    var data = {
                        templateId: viewModel.TemplateId(),
                        deliveryScheduleId: parseInt($self.val()),
                        quantity: viewModel.Quantity
                    };
                    GetOrderItemPrice(data, function (price) {
                        viewModel.Price(price);
                        viewModel.DeliveryScheduleId(data.deliveryScheduleId);
                        UpdateOrderItemDateOnChange(viewModel);
                    });
                });
            } catch (ex) {
                console.log(ex);
            }
        }
    };

    ko.bindingHandlers.populateTemplatePriceByQuantity = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            try {
                $(element).change(function () {
                    var $self = $(this);
                    var data = {
                        templateId: viewModel.TemplateId(),
                        deliveryScheduleId: viewModel.DeliveryScheduleId(),
                        quantity: parseInt($self.find('option:selected').val())
                    };
                    GetOrderItemPrice(data, function (price) {
                        viewModel.Price(price);
                        UpdateOrderItemDateOnChange(viewModel);
                    });
                });
            } catch (ex) {
                console.log(ex);
            }
        }
    };

    var GetOrderItemPrice = function (param, callback) {
        try {
            $.ajax({
                type: 'GET',
                //async: false,
                url: '{0}/PriceSummary/GetPrice'.format(virtualDirectory),
                data: param,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result) {
                        callback(result.Price);
                    }
                },
                error: function (status, error) {
                    alert(status);
                }
            });
        } catch (ex) {
            console.log(ex);
        }
    }

    var OrderItemAdditionalBinding = function (dataCollection) {
        try {
            dataCollection.PromoCodeId = ko.observable(parseInt($('#PromoCodeId').val()));
            dataCollection.DiscountAmount = ko.observable(parseInt($('#DiscountAmount').val()));
            dataCollection.OrderId = ko.observable(parseInt($('#OrderId').val()));

            dataCollection.SubTotal = ko.computed(function () {
                var self = this;
                var total = Enumerable.From(self.OrderItems())
                    .Sum(function (x) {
                        return x.Price();
                    });
                return total;
            }, dataCollection).extend({ notify: 'always' });

            dataCollection.TotalAmount = ko.computed(function () {
                var self = this;
                return self.SubTotal() - self.DiscountAmount();
            }, dataCollection).extend({ notify: 'always' });

            dataCollection.RemoveItem = function (data) {
                try {
                    ConfirmUI("Are you sure to delete this order item?",
                        function () {
                            $.ajax({
                                url: '{0}/ShoppingCart/DeleteOrderItem?draftId={1}'.format(virtualDirectory, data.DraftId()),
                                type: 'GET',
                                success: function (result, textStatus, jqXHR) {
                                    if (result) {
                                        dataCollection.OrderItems.remove(data);
                                    }
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    alert(textStatus);
                                }
                            });

                        }, null)

                } catch (ex) {
                    console.log(ex);
                }
            }

            dataCollection.EditItem = function (data) {
                try {
                    ConfirmUI("Are you sure to edit this order item?",
                        function () {
                            $.ajax({
                                url: '{0}/ShoppingCart/EditOrderItem?draftId={1}'.format(virtualDirectory, data.DraftId()),
                                type: 'GET',
                                success: function (result, textStatus, jqXHR) {
                                    if (result) {
                                        dataCollection.OrderItems.remove(data);
                                        window.location.href = Base64Decode(result);
                                    }
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    alert(textStatus);
                                }
                            });
                        }, null)
                } catch (ex) {
                    console.log(ex);
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    };

    this.EditItemFromCollcetion = function (draftId) {
        try {
            $.each(OrderItemViewModel.OrderItems(), function (index, element) {
                if (element.DraftId() == draftId) {
                    OrderItemViewModel.EditItem(element);
                    return false;
                }
            });
        } catch (ex) {
            console.log(ex);
        }
    };

    this.GetPromoDiscount = function (promoCode) {
        try {
            $.ajax({
                type: 'GET',
                url: '{0}/ShoppingCart/IsPromoCodeExists'.format(virtualDirectory),
                data: { promoCodeName: promoCode, ordrtId: OrderItemViewModel.OrderId() },
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data, textStatus, jqXHR) {
                    if (data) {
                        if (data.Status) {
                            
                            if (OrderItemViewModel.SubTotal() < data.PromoCode.PromoAmount) {

                                ConfirmUI('Your promotion code of {0} still has a balance of {1} available to be used on this order.#Please continue shopping for additional marketing materials in order to take advantage of the balance, or checkout now.#Note...any remaining balance on the promotion code not used towards this order will be forfeited.'
                                    .format(accounting.formatMoney(data.PromoCode.PromoAmount), accounting.formatMoney(data.PromoCode.PromoAmount - OrderItemViewModel.SubTotal())),
                                    function () {
                                        OrderItemViewModel.PromoCodeId(data.PromoCode.PromoCodeId);
                                        OrderItemViewModel.DiscountAmount(OrderItemViewModel.SubTotal());
                                    }, 
                                    function () {
                                        $('#PromoCode').val('');
                                        OrderItemViewModel.DiscountAmount(0);
                                    }, null, null, 'Apply Promo', 'Discard Promo');

                                //function (msg, yeshandle, nohandle, height, width, okCaption, cancelCaption)
                                //OrderItemViewModel.DiscountAmount(OrderItemViewModel.SubTotal());
                            }
                            else {
                                OrderItemViewModel.PromoCodeId(data.PromoCode.PromoCodeId);
                                OrderItemViewModel.DiscountAmount(data.PromoCode.PromoAmount);
                            }
                        }
                        else {
                            OrderItemViewModel.DiscountAmount(0);
                            alert(data.Message);
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

    this.InsertOrder = function (redirectionUrl) {
        try {
            var orderItemData = ko.mapping.toJS(OrderItemViewModel.OrderItems(),
                { ignore: ['TemplateCode', 'TemplateTitle', 'TemplateDesc', 'TemplateWidth', 'TemplateHeight'] });

            if (orderItemData) {
                if (orderItemData.length > 0) {
                    var orderItemIdentifiers = Enumerable.From(orderItemData).
                        Select(function (x) { return x.OrderItemIdentifier; }).ToArray();

                    var order = {
                        OrderId: OrderItemViewModel.OrderId(),
                        OrderItems: orderItemData,
                        SubTotal: OrderItemViewModel.SubTotal(),
                        DiscountAmount: OrderItemViewModel.DiscountAmount(),
                        TotalAmount: OrderItemViewModel.TotalAmount(),
                        OrderReferance: orderItemIdentifiers.join('-'),
                        PromoCodeId: OrderItemViewModel.PromoCodeId()
                    };
                }
            }

            var generatedUri = '';
            if (OrderItemViewModel.OrderId() == 0) {
                generatedUri = '{0}/ShoppingCart/InsertOrder'.format(virtualDirectory);
            }
            else {
                generatedUri = '{0}/ShoppingCart/UpdateOrder'.format(virtualDirectory);
            }

            $.ajax({
                type: 'POST',
                url: generatedUri,
                data: JSON.stringify(order),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data, textStatus, jqXHR) {
                    if (data) {
                        if (data.Status) {
                            win.location.href = '{0}?orderId={1}'.format(redirectionUrl, data.OrderId);
                        }
                        else {
                            alert(data.Message);
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
    }

    this.ModalCallbackForPreview = function ($modal) {
        try {
            var $self = $modal;
            $self.find('.button-text').button();
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

        } catch (ex) {
            console.log(ex);
        }
    };

    this.ShowPreviewWindow = function (data) {
        try {
            $.ajax({
                url: '{0}/ShoppingCart/ShowOrderItemPreview?draftId={1}'.format(virtualDirectory, data.DraftId()),
                type: 'GET',
                success: function (result, textStatus, jqXHR) {
                    if (result) {
                        modal.open({
                            content: result,
                            width: (typeof windowWidth === 'undefined') ? '735px' : windowWidth,
                            openCallBack: ModalCallbackForPreview
                        });
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

    this.ShowShareWindow = function () {
        try {
            $('#frmShareDraft .field-validation-error').remove();
            $('#frmShareDraft').find('input, select, textarea')
				.removeClass('input-validation-error');
            $('#share-template').show();
        } catch (ex) {
            console.log(ex);
        }
    };

    this.ShareOrderItemSuccessCallback = function (data) {
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

    this.ShareOrderItemBeginCallback = function () {
        try {
            $.blockUI({ message: $("#dataloading") });
        } catch (ex) {
            console.log(ex);
        }
    }

    this.ShareOrderItemCompletionCallback = function () {
        try {
            $.unblockUI();
        } catch (ex) {
            console.log(ex);
        }
    }

    this.ShareOrderItem = function () {
        try {
            if ($('#frmShareOrderItem').valid()) {
                $("#frmShareOrderItem").submit();
            }
        } catch (ex) {
            console.log();
        }
    };

    var EmailShareValidationSetup = function () {
        try {
            $('#frmShareOrderItem').validate({
                rules: {
                    recipient: { required: true },
                    emailId: { required: true, email: true },
                    comment: { required: true }
                },
                messages: {
                    recipient: "Recipient is required to share the image",
                    comment: "Comment is required to share the image",
                    emailId: {
                        required: "Recipient's email id is required to share the image",
                        email: "Recipient's email id is invalid"
                    }
                },
                errorElement: 'span',
                errorPlacement: function (error, element) {
                    element.addClass('input-validation-error');
                    error.addClass('field-validation-error');
                    error.css({ 'width': '81%' });
                    error.insertBefore(element);
                },
                success: function (element) {
                    element.next('input, select, textarea')
				        .removeClass('input-validation-error');
                    element.remove();
                }
            });
        } catch (ex) {
            console.log(ex);
        }
    };

    var UpdateOrderItemDateOnChange = function (data) {
        try {
            var jsonStr = ko.toJSON(data);
            $.ajax({
                type: 'POST',
                url: '{0}/ShoppingCart/SaveTempOrder'.format(virtualDirectory),
                data: jsonStr,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result) {
                        if (result.Status) {
                            $('#OrderItem_Quantity_' + data.OrderItemIdentifier()).html(data.Quantity());
                            $('#OrderItem_Price_' + data.OrderItemIdentifier())
                                .html(accounting.formatMoney(data.Price()));
                        }
                    }
                },
                error: function (status, error) {
                    alert(status);
                }
            });
        } catch (ex) {
            console.log(ex);
        }
    }

    $(document).ready(function () {
        try {
            if ($('#OrderData').val() != '') {
                var orderData = $.parseJSON(Base64Decode($('#OrderData').val()));
                OrderItemViewModel = ko.mapping.fromJS({ OrderItems: orderData });
                OrderItemAdditionalBinding(OrderItemViewModel);

                var total = Enumerable.From(orderData).Sum(function (x) { return x.Price });
                $('.one_color').html(orderData.length);
                ko.applyBindings(OrderItemViewModel, document.getElementById('page-container'));
            }
            else {
                $('#item-container').hide();
            }

            var errorMessage = $('#ErrorMessage').val();
            if (errorMessage != '') {
                alert(errorMessage);
            }
        } catch (ex) {
            console.log(ex);
        }
    });
}(jQuery, window));