/// <reference path="jquery-2.1.0-vsdoc.js" />


(function ($, win) {
    var isCreditCardInfoLoaded = false;

    $(document).ready(function () {
        var d = new Date();

        //$('#CreditCard_ExpiryMonth').spinner({ min: 01, max: 12, step: 1 });
        //$('#CreditCard_ExpiryYear').spinner({
        //    min: d.getFullYear(),
        //    max: d.getFullYear() + 20, step: 1
        //});

        $('input[name="PaymentTypeId"]:radio').change(function () {
            var $self = $(this);
            isCreditCardInfoLoaded = false;

            if ($self.val() == '1' && $self.is(":checked")) {
                //managePaypalIfame($('#CreditCardInfo'));
                $('#creditCardInfo, #submit-panel').show();
                $('.heading3').html('Credit Card Information');
            }
            else {
                $('#submit-panel').show();
                $('#creditCardInfo').hide();
                $('.heading3').html('Paypal Information');
            }
        });

        if ($('#Error').val() != '') {
            alert($('#Error').val());
        }

        //PaymentValidationSetup();
    });

    this.ShowChangeAddressDialog = function (type) {
        try {
            var uri = '{0}/UserAddress/GetChangeAddressPopup?type={1}'.format(virtualDirectory, type);
            var d = new Date();

            ShowModal(uri, 600, undefined, function ($model) {
                //$('#Month').spinner({ min: 01, max: 12, step: 1 });
                //$('#Year').spinner({ min: 1900, max: d.getFullYear(), step: 1 });
                AddressValidationSetup();
            });

        } catch (ex) {
            console.log(ex);
        }
    };

    this.SaveUserAddress_OnSuccess = function (data) {
        try {
            if (data) {
                if (data.Status) {
                    alert('Saved successfully', function () {
                        modal.close();
                        win.location.reload();
                    });
                }
                else {
                    alert(data.Message, function () {
                        modal.close();
                    });
                }
            }

        } catch (ex) {
            console.log(ex);
        }
    };

    this.ShoppingCart_OnBegin = function () {
        try {
            if ($('#frmShoppingCart #ShippingAddressId').val() == '0'
                && $('#frmShoppingCart #BillingAddressId').val() == '0') {

                alert('Shipping address or Billing address is not filled out.#Please provide valid address for both.', function () { return false; })
            }
            else {
                if ($('input[name="PaymentTypeId"]:checked').val() == "1") {
                    //$('#frmShoppingCart').attr('target', 'CreditCardInfo');
                    isCreditCardInfoLoaded = true;
                }
                $('#frmShoppingCart').submit();
                return true;
            }
        } catch (ex) {
            console.log(ex);
        }
    };

    var AddressValidationSetup = function () {
        try {
            $('#frmSaveUserAddress').validate({
                rules: {
                    Address1: { required: true },
                    SeconderyContact: { required: true },
                    State: { required: true },
                    City: { required: true },
                    Pin: { required: true }
                },
                messages: {
                    Address1: "Address1 is required",
                    SeconderyContact: "Phone is required",
                    State: "State is required",
                    City: "City is required",
                    Pin: "Zip Code is required"
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

    this.managePaypalIfame = function (elm) {
        try {
            $iframe = $(elm);
            if (isCreditCardInfoLoaded) {
                $iframe.attr({ 'height': '550', 'width': '490' });
                $('#submit-panel').hide();
            }
            else {
                $iframe.attr({ 'height': '0', 'width': '0' });
                $('#submit-panel').show();
            }
            return $iframe;
        } catch (ex) {
            console.log(ex);
        }
    }
}(jQuery, window));