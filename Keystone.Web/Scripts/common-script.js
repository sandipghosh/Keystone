/// <reference path="jquery-2.1.0.min.js" />
/// <reference path="jquery-ui-vsdoc.js" />

/*String utility prototype starts*/
var flashInterval;

String.prototype.startsWith = function (str) {
    return this.slice(0, str.length) == str;
};

String.prototype.endsWith = function (str) {
    return this.slice(-str.length) == str;
};

String.prototype.trim = function () {
    return this.replace(/^\s+|\s+$/, '');
};

String.prototype.contains = function (it) {
    return this.indexOf(it) != -1;
};

String.prototype.format = function () {
    var args = arguments;
    return this.replace(/\{\{|\}\}|\{(\d+)\}/g, function (m, n) {
        if (m == "{{") { return "{"; }
        if (m == "}}") { return "}"; }
        return args[n];
    });
};

String.prototype.isEmptyOrBlank = function () {
    return (!this || 0 === this.length) || (!this || /^\s*$/.test(this));
};

String.prototype.replaceAll = function (token, newToken, ignoreCase) {
    var str, i = -1, _token;
    if ((str = this.toString()) && typeof token === "string") {
        _token = ignoreCase === true ? token.toLowerCase() : undefined;
        while ((i = (
            _token !== undefined ?
                str.toLowerCase().indexOf(
                    _token,
                    i >= 0 ? i + newToken.length : 0
                ) : str.indexOf(
                    token,
                    i >= 0 ? i + newToken.length : 0
                )
        )) !== -1) {
            str = str.substring(0, i)
                .concat(newToken)
                .concat(str.substring(i + token.length));
        }
    }
    return str;
};

String.prototype.hasQueryString = function () {
    return (this.indexOf('?') > 0);
};

isNumber = function (s) {
    var n = s.toString().trim();
    return !isNaN(parseFloat(n)) && isFinite(n);
};

isInteger = function (s) {
    var n = s.trim();
    return n.length > 0 && !(/[^0-9]/).test(n);
};

isHTML = function (str) {
    return !!$(str)[0];
};

isFloat = function (s) {
    var n = s.trim();
    return n.length > 0 && !(/[^0-9.]/).test(n) && (/\.\d/).test(n);
}

isDate = function (dt) {
    return (null != dt) && !isNaN(dt) && ("undefined" !== typeof dt.getDate);
};

rgb2hex = function (rgb) {
    rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    return "#" +
     ("0" + parseInt(rgb[1], 10).toString(16)).slice(-2) +
     ("0" + parseInt(rgb[2], 10).toString(16)).slice(-2) +
     ("0" + parseInt(rgb[3], 10).toString(16)).slice(-2);
};

roundNumber = function (num, dec) {
    var result = Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
    return result;
};

//Implement function for Base64 encription
Base64Encode = function (input) {
    try {
        var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        input = escape(input);
        var output = "";
        var chr1, chr2, chr3 = "";
        var enc1, enc2, enc3, enc4 = "";
        var i = 0;

        do {
            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output = output +
               keyStr.charAt(enc1) +
               keyStr.charAt(enc2) +
               keyStr.charAt(enc3) +
               keyStr.charAt(enc4);
            chr1 = chr2 = chr3 = "";
            enc1 = enc2 = enc3 = enc4 = "";
        } while (i < input.length);

        return output;

    }
    catch (ex) { }
};

//Implement function for Base64 decription
Base64Decode = function (encoded) {
    try {
        var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        var output = "";
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;

        do {
            enc1 = keyStr.indexOf(encoded.charAt(i++));
            enc2 = keyStr.indexOf(encoded.charAt(i++));
            enc3 = keyStr.indexOf(encoded.charAt(i++));
            enc4 = keyStr.indexOf(encoded.charAt(i++));

            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;

            output = output + String.fromCharCode(chr1);

            if (enc3 != 64) {
                output = output + String.fromCharCode(chr2);
            }
            if (enc4 != 64) {
                output = output + String.fromCharCode(chr3);
            }
        } while (i < encoded.length);

        return output;

    }
    catch (ex) { }
};

GetLetterCaseFromPascalCase = function (str) {
    return str.replace(/([a-z])([A-Z])/g, '$1 $2');
};

//jQuery plugin popup window
(function ($) {
    $.fn.popupWindow = function (instanceSettings) {
        return this.each(function () {
            $(this).click(function () {

                $.fn.popupWindow.defaultSettings = {
                    centerBrowser: 0, // center window over browser window? {1 (YES) or 0 (NO)}. overrides top and left
                    centerScreen: 0, // center window over entire screen? {1 (YES) or 0 (NO)}. overrides top and left
                    height: 500, // sets the height in pixels of the window.
                    left: 0, // left position when the window appears.
                    location: 0, // determines whether the address bar is displayed {1 (YES) or 0 (NO)}.
                    menubar: 0, // determines whether the menu bar is displayed {1 (YES) or 0 (NO)}.
                    resizable: 0, // whether the window can be resized {1 (YES) or 0 (NO)}. Can also be overloaded using resizable.
                    scrollbars: 0, // determines whether scrollbars appear on the window {1 (YES) or 0 (NO)}.
                    status: 0, // whether a status line appears at the bottom of the window {1 (YES) or 0 (NO)}.
                    width: 500, // sets the width in pixels of the window.
                    windowName: null, // name of window set from the name attribute of the element that invokes the click
                    windowURL: null, // url used for the popup
                    top: 0, // top position when the window appears.
                    toolbar: 0 // determines whether a toolbar (includes the forward and back buttons) is displayed {1 (YES) or 0 (NO)}.
                };

                settings = $.extend({}, $.fn.popupWindow.defaultSettings, instanceSettings || {});

                var windowFeatures = 'height={0},width={1},toolbar={2},scrollbars={3},status={4},resizable={5},location={6},menuBar={7}'
                    .format(settings.height, settings.width, settings.toolbar, settings.scrollbars, settings.status,
                        settings.resizable, settings.location, settings.menubar);

                settings.windowName = this.name || settings.windowName;
                settings.windowURL = this.href || settings.windowURL;
                var centeredY, centeredX, popup;

                if (settings.centerBrowser) {
                    if ($.browser.msie) {//hacked together for IE browsers
                        centeredY = (window.screenTop - 120) + ((((document.documentElement.clientHeight + 120) / 2) - (settings.height / 2)));
                        centeredX = window.screenLeft + ((((document.body.offsetWidth + 20) / 2) - (settings.width / 2)));
                    } else {
                        centeredY = window.screenY + (((window.outerHeight / 2) - (settings.height / 2)));
                        centeredX = window.screenX + (((window.outerWidth / 2) - (settings.width / 2)));
                    }
                    window.open(settings.windowURL, settings.windowName, windowFeatures + ',left=' + centeredX + ',top=' + centeredY).focus();

                } else if (settings.centerScreen) {
                    centeredY = (screen.height - settings.height) / 2;
                    centeredX = (screen.width - settings.width) / 2;
                    window.open(settings.windowURL, settings.windowName, windowFeatures + ',left=' + centeredX + ',top=' + centeredY).focus();

                } else {
                    window.open(settings.windowURL, settings.windowName, windowFeatures + ',left=' + settings.left + ',top=' + settings.top).focus();
                }

                return false;
            });

        });
    };
})(jQuery);

(function (p, z) { function q(a) { return !!("" === a || a && a.charCodeAt && a.substr) } function m(a) { return u ? u(a) : "[object Array]" === v.call(a) } function r(a) { return "[object Object]" === v.call(a) } function s(a, b) { var d, a = a || {}, b = b || {}; for (d in b) b.hasOwnProperty(d) && null == a[d] && (a[d] = b[d]); return a } function j(a, b, d) { var c = [], e, h; if (!a) return c; if (w && a.map === w) return a.map(b, d); for (e = 0, h = a.length; e < h; e++) c[e] = b.call(d, a[e], e, a); return c } function n(a, b) { a = Math.round(Math.abs(a)); return isNaN(a) ? b : a } function x(a) { var b = c.settings.currency.format; "function" === typeof a && (a = a()); return q(a) && a.match("%v") ? { pos: a, neg: a.replace("-", "").replace("%v", "-%v"), zero: a } : !a || !a.pos || !a.pos.match("%v") ? !q(b) ? b : c.settings.currency.format = { pos: b, neg: b.replace("%v", "-%v"), zero: b } : a } var c = { version: "0.3.2", settings: { currency: { symbol: "$", format: "%s%v", decimal: ".", thousand: ",", precision: 2, grouping: 3 }, number: { precision: 0, grouping: 3, thousand: ",", decimal: "." } } }, w = Array.prototype.map, u = Array.isArray, v = Object.prototype.toString, o = c.unformat = c.parse = function (a, b) { if (m(a)) return j(a, function (a) { return o(a, b) }); a = a || 0; if ("number" === typeof a) return a; var b = b || ".", c = RegExp("[^0-9-" + b + "]", ["g"]), c = parseFloat(("" + a).replace(/\((.*)\)/, "-$1").replace(c, "").replace(b, ".")); return !isNaN(c) ? c : 0 }, y = c.toFixed = function (a, b) { var b = n(b, c.settings.number.precision), d = Math.pow(10, b); return (Math.round(c.unformat(a) * d) / d).toFixed(b) }, t = c.formatNumber = function (a, b, d, i) { if (m(a)) return j(a, function (a) { return t(a, b, d, i) }); var a = o(a), e = s(r(b) ? b : { precision: b, thousand: d, decimal: i }, c.settings.number), h = n(e.precision), f = 0 > a ? "-" : "", g = parseInt(y(Math.abs(a || 0), h), 10) + "", l = 3 < g.length ? g.length % 3 : 0; return f + (l ? g.substr(0, l) + e.thousand : "") + g.substr(l).replace(/(\d{3})(?=\d)/g, "$1" + e.thousand) + (h ? e.decimal + y(Math.abs(a), h).split(".")[1] : "") }, A = c.formatMoney = function (a, b, d, i, e, h) { if (m(a)) return j(a, function (a) { return A(a, b, d, i, e, h) }); var a = o(a), f = s(r(b) ? b : { symbol: b, precision: d, thousand: i, decimal: e, format: h }, c.settings.currency), g = x(f.format); return (0 < a ? g.pos : 0 > a ? g.neg : g.zero).replace("%s", f.symbol).replace("%v", t(Math.abs(a), n(f.precision), f.thousand, f.decimal)) }; c.formatColumn = function (a, b, d, i, e, h) { if (!a) return []; var f = s(r(b) ? b : { symbol: b, precision: d, thousand: i, decimal: e, format: h }, c.settings.currency), g = x(f.format), l = g.pos.indexOf("%s") < g.pos.indexOf("%v") ? !0 : !1, k = 0, a = j(a, function (a) { if (m(a)) return c.formatColumn(a, f); a = o(a); a = (0 < a ? g.pos : 0 > a ? g.neg : g.zero).replace("%s", f.symbol).replace("%v", t(Math.abs(a), n(f.precision), f.thousand, f.decimal)); if (a.length > k) k = a.length; return a }); return j(a, function (a) { return q(a) && a.length < k ? l ? a.replace(f.symbol, f.symbol + Array(k - a.length + 1).join(" ")) : Array(k - a.length + 1).join(" ") + a : a }) }; if ("undefined" !== typeof exports) { if ("undefined" !== typeof module && module.exports) exports = module.exports = c; exports.accounting = c } else "function" === typeof define && define.amd ? define([], function () { return c }) : (c.noConflict = function (a) { return function () { p.accounting = a; c.noConflict = z; return c } }(p.accounting), p.accounting = c) })(this);

(function ($, win) {
    //var loadingCounter = 0;
    $(document).ready(function () {
        $('#mycarousel').jcarousel({
            auto: 1,
            scroll: 1,
            wrap: 'last',
            initCallback: function (carousel) {
                // Disable autoscrolling if the user clicks the prev or next button.
                carousel.buttonNext.bind('click', function () {
                    carousel.startAuto(0);
                });

                carousel.buttonPrev.bind('click', function () {
                    carousel.startAuto(0);
                });

                // Pause autoscrolling if the user moves with the cursor over the clip.
                carousel.clip.hover(function () {
                    carousel.stopAuto();
                }, function () {
                    carousel.startAuto();
                });
            }
        });

        $('.button').button();
    });

    // Prevent the backspace key from navigating back.
    $(document).unbind('keydown').bind('keydown', function (event) {
        var doPrevent = false;
        if (event.keyCode === 8) {
            var d = event.srcElement || event.target;
            if ((d.tagName.toUpperCase() === 'INPUT' && (d.type.toUpperCase() === 'TEXT' || d.type.toUpperCase() === 'PASSWORD' || d.type.toUpperCase() === 'FILE' || d.type.toUpperCase() === 'EMAIL'))
                 || d.tagName.toUpperCase() === 'TEXTAREA') {
                doPrevent = d.readOnly || d.disabled;
            }
            else {
                doPrevent = true;
            }
        }

        if (doPrevent) {
            event.preventDefault();
        }
    });

    //This code works on all browsers and swallows the backspace key when not on a form element, 
    //or if the form element is disabled|readOnly. It is also efficient, 
    //which is important when it is executing on every key typed in.
    $(document).on("keydown keypress", function (e) {
        var rx = /INPUT|SELECT|TEXTAREA/i;
        if (e.which == 8) { // 8 == backspace
            if (!rx.test(e.target.tagName) || e.target.disabled || e.target.readOnly) {
                e.preventDefault();
            }
        }
    });

    $.ajaxSetup({
        beforeSend: function (jqXHR, settings) {
            if (!settings.url.contains('Keepalive')) {
                loadingCounter += 1;
                $(document).css('cursor', 'wait !important');
                try { $.blockUI({ message: $("#dataloading") }); } catch (ex) { }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('error');
        },
        complete: function (jqXHR, textStatus) {
            if (loadingCounter > 1)
            { loadingCounter -= 1 }
            else {
                loadingCounter = 0;
                try { $.unblockUI(); } catch (ex) { }
            }
            $(document).css('cursor', 'default !important');
        }
    });

    $.fn.MakeCenterScreen = function () {
        var $self = $(this);

        var docWidth = $(document).width() / 2;
        var elmWidth = $self.width() / 2;

        $self.css({ 'left': (docWidth - elmWidth).toString() + 'px' });
    };

    $.fn.flash = function (color, duration) {
        var $self = $(this);

        flashInterval = setInterval(function () {
            $self.toggleClass('disclaimer-flash');
        }, 2000)
        return $self;
    };

    /*Implementing function to overload general alert function*/
    win.alert = function (msg, okhandle, height, width, okCaption, title) {
        try {
            msg = msg.toString();
            $(function () {
                if (okCaption != undefined || okCaption != null)
                    $('#dialog-message').attr('title', title);
                else
                    $('#dialog-message').attr('title', 'Message');

                $("#dialog-message p").html('').addClass('alert');

                if (msg.indexOf('#') > 0) {
                    $.each(msg.split('#'), function (index, value) {
                        if (value != '') {
                            $("#dialog-message p.alert").append('<li>{0}</li>'.format(value));
                        }
                    });
                    $("#dialog-message p.alert").wrapInner('<ul/>');
                }
                else {
                    $("#dialog-message p.alert").html(msg);
                }

                $("#dialog:ui-dialog").dialog("destroy");

                $("#dialog-message").dialog({
                    modal: true,
                    position: ['center', 'middle'],
                    height: (height != undefined || height != null) ? height : 'auto',
                    width: (width != undefined || width != null) ? width : 'auto',
                    autoResize: true,
                    resizable: false,
                    open: function (event, ui) {
                        $("#dialog-message").closest('div.ui-dialog').MakeCenterScreen();
                    },
                    buttons: [
                        {
                            text: (okCaption != undefined || okCaption != null) ? okCaption : "OK",
                            click: function () {
                                $(this).dialog("close");
                                if (okhandle != undefined || okhandle != null) {
                                    okhandle();
                                }
                            }
                        }
                    ]
                });
            });
        } catch (ex) {
            if (window.console)
                window.console.log(ex);
        }
    };

    /*Implementing function to overload general confirm function*/
    this.ConfirmUI = function (msg, yeshandle, nohandle, height, width, okCaption, cancelCaption) {
        try {
            $("#dialog-message p").html('');
            $("#dialog-message p").addClass('confirmation');

            if (msg.indexOf('#') > 0) {
                $.each(msg.split('#'), function (index, value) {
                    if (value != '') {
                        $("#dialog-message p.confirmation").append('<li>{0}</li>'.format(value));
                    }
                });
                $("#dialog-message p.confirmation").wrapInner('<ul/>');
            }
            else {
                $("#dialog-message p.confirmation").html(msg);
            }

            //$("#dialog-message p").addClass('confirmation').html(msg);
            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-message").dialog({
                modal: true,
                position: ['center', 'center'],
                height: (height != undefined || height != null) ? height : 'auto',
                width: (width != undefined || width != null) ? width : 'auto',
                autoResize: true,
                resizable: false,
                buttons: [
                    {
                        text: (okCaption != undefined || okCaption != null) ? okCaption : "YES",
                        click: function () {
                            $(this).dialog("close");
                            if (yeshandle != undefined || yeshandle != null) {
                                yeshandle();
                            }
                        }
                    },
                    {
                        text: (cancelCaption != undefined || cancelCaption != null) ? cancelCaption : "NO",
                        click: function () {
                            $(this).dialog("close");
                            if (nohandle != undefined || nohandle != null) {
                                nohandle();
                            }
                        }
                    }
                ]
            });
        } catch (ex) {
            if (window.console)
                window.console.log(ex);
        }
    };

    this.SignInUser = function (formId) {
        try {
            if ($(formId).valid()) {
                SetCredential({
                    UserId: '#UserId[data-type="popup"]',
                    Password: '#Password[data-type="popup"]',
                    IsRemember: $('#chkRememberCredential[data-type="popup"]').is(":checked")
                });
                $(formId).submit();
            }
        } catch (ex) {
            console.log(ex);
        }
    };

    this.showSignInPopup = function (e) {
        try {
            var $self = $(e).closest('.logout');
            $('#signin-popup').css({
                'top': 40,
                'left': $self.offset().left - ($('#signin-popup').width() - $self.width() - 30),
                'float': 'right',
                'bottom': 'auto'
            });
            GetCredential({
                UserId: '#UserId[data-type="popup"]',
                Password: '#Password[data-type="popup"]',
                IsRemember: $('#chkRememberCredential[data-type="popup"]').is(":checked")
            });
            $('#signin-popup').show();
        } catch (ex) {
            console.log(ex);
        }
    };

    this.SignInCompleteCallback = function (data) {
        try {
            if (data) {
                if (data.Status) {
                    $("#signin-popup").hide();
                    location.reload();
                }
                else {
                    alert(data.Message);
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    };

    this.showNavPopup = function (event) {
        try {
            $('.popup-cotainer').hide();
            var $self = $(event.target);
            var $popup = $self.siblings('.popup-cotainer');
            $popup.css({
                'top': $self.offset().top + $self.height() + 10,
                'left': $self.offset().left - ($popup.width() - $self.width()),
                'float': 'right',
                'bottom': 'auto'
            });

            $popup.show();
            event.preventDefault();
        } catch (ex) {
            console.log(ex);
        }
    };

    /*Implement function to keep the application session alive of force to logout*/
    this.HandleSessionTimeout = function (btnText1, btnText2, sessionTimeout) {
        try {
            /*Handling timeout section starts*/
            $("#timeout-dialog").dialog({
                autoOpen: false,
                modal: true,
                width: 350,
                height: 180,
                closeOnEscape: false,
                draggable: false,
                resizable: false,
                buttons:
                [{
                    text: btnText1,
                    click: function () { $(this).dialog('close'); }
                },
                {
                    text: btnText2,
                    click: function () { $.idleTimeout.options.onTimeout.call(this); }
                }]
            });

            $.idleTimeout('#timeout-dialog', 'div.ui-dialog-buttonpane button:first', {
                idleAfter: sessionTimeout, // user is considered idle after 5 minutes of no movement
                //pollingInterval: 60, // a request to keepalive.php (below) will be sent to the server every minute
                keepAliveURL: '/UserAccount/Keepalive'.format(virtualDirectory),
                serverResponseEquals: 'OK', // the response from keepalive.php must equal the text "OK"
                onTimeout: function () {
                    // redirect the user when they timeout.
                    try { $.unblockUI(); } catch (ex) { }
                    $(this).dialog('close');
                    $.blockUI({ message: $("#dataloading") });
                    window.location = '/UserAccount/SignOutUser'.format(virtualDirectory);
                },
                onIdle: function () {
                    // show the dialog when the user idles
                    $(this).dialog("open");
                },
                onCountdown: function (counter) {
                    // update the counter span inside the dialog during each second of the countdown
                    $("#timeout-dialog-countdown").html(counter);
                },
                onResume: function () {
                    // the dialog is closed by a button in the dialog
                    // no need to do anything else
                }
            });
            /*Handling timeout section ends*/
        }
        catch (ex) {
            console.log(ex);
        }
    };

    this.GetValidationErrorMarkup = function () {
        try {
            var markup = {
                errorElement: 'span',
                errorPlacement: function (error, element) {
                    //element.addClass('input-validation-error');
                    error.css({ 'width': '81%' });
                    error.addClass('field-validation-error');
                    error.insertBefore(element);
                },
                success: function (element) {
                    //element.next('input, select, textarea')
                    //    .removeClass('input-validation-error');
                    element.remove();
                }
            };

            return markup;
        } catch (ex) {
            console.log(ex);
        }
    };

    this.SetCredential = function (options) {
        try {
            var defaultOptions = {
                UserId: '#UserId',
                Password: '#Password',
                IsRemember: false
            }
            var obj = $.extend(defaultOptions, options);
            $.cookie('UserId', $(obj.UserId).val());
            $.cookie('Password', $(obj.Password).val());
            $.cookie('IsRemember', obj.IsRemember);
        } catch (ex) {
            console.log(ex);
        }
    }

    this.GetCredential = function (options) {
        try {
            var defaultOptions = {
                UserId: '#UserId',
                Password: '#Password',
                IsRemember: false
            }
            var obj = $.extend(defaultOptions, options);
            $(obj.UserId).val($.cookie('UserId'));
            $(obj.Password).val($.cookie('Password'));
            if ($.cookie('IsRemember') == 'true') $('#chkRememberCredential').attr('checked', 'checked');
        } catch (ex) {
            console.log(ex);
        }
    }
}(jQuery, window));

(function ($) {
    $.QueryString = (function (a) {
        if (a == "") return {};
        var b = {};
        for (var i = 0; i < a.length; ++i) {
            var p = a[i].split('=');
            if (p.length != 2) continue;
            b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, " "));
        }
        return b;
    })(window.location.search.substr(1).split('&'));

    xmlDocToString = function ($xml) {
        /* unwrap xml document from jQuery*/
        var doc = $xml[0];
        var string;
        /* for IE*/
        if (window.ActiveXObject) {
            string = doc.xml;
            if (string == undefined) {
                string = (new XMLSerializer()).serializeToString(doc);
            }
        }
        else {
            string = (new XMLSerializer()).serializeToString(doc);
        }
        return string;
    }
})(jQuery);

(function ($, win) {
    this.ShowModal = function (uri, windowWidth, beforeShowCallback, afterShowCallback) {
        $.ajax({
            url: uri,
            type: 'GET',
            success: function (result, textStatus, jqXHR) {
                if (result) {
                    if (typeof beforeShowCallback !== 'undefined' && beforeShowCallback != null) {
                        var callbackReturn = beforeShowCallback();
                        if (!callbackReturn) return;
                    }

                    try { clearInterval(flashInterval); } catch (ex) { }
                    modal.open({
                        content: result,
                        width: (windowWidth == undefined || windowWidth == null) ?
                            '735px' : windowWidth,
                        openCallBack: afterShowCallback
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
    };

    this.GetTemplatePrice = function (templateId, deliveryId, quantity, $displayElement) {
        try {
            $.ajax({
                type: 'GET',
                //async: false,
                url: '{0}/PriceSummary/GetPrice'.format(virtualDirectory),
                data: {
                    'templateId': parseInt(templateId),
                    'deliveryScheduleId': parseInt(deliveryId),
                    'quantity': parseInt(quantity)
                },
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result) {
                        $displayElement.html(accounting.formatMoney(result.Price));

                        sessionStorage.setItem('SELECTED_TEMPLATE_DELIVERY_{0}'.format(templateId), parseInt(deliveryId));
                        sessionStorage.setItem('SELECTED_TEMPLATE_QUANTITY_{0}'.format(templateId), parseInt(quantity));
                        sessionStorage.setItem('SELECTED_TEMPLATE_PRICE_{0}'.format(templateId), result.Price);
                    }
                },
                error: function (status, error) {
                    alert(status);
                }
            });
        } catch (ex) {
            console.log(ex);
        }
    };

    this.DeleteDraft = function (draftId, draftHtmlElementId) {
        try {
            ConfirmUI('Are you sure you want to delete the draft', function () {
                $.ajax({
                    type: 'POST',
                    url: '{0}/Draft/DeleteDraft?draftId={1}'.format(virtualDirectory, draftId),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (result) {
                        if (result) {
                            if (result.Status) {
                                $(draftHtmlElementId).remove();
                            }
                            else {
                                alert(result.Message);
                            }
                        }
                    },
                    error: function (status, error) {
                        alert(status);
                    }
                });
            }, null, null, null, 'Delete', 'Cancel');

        } catch (ex) {
            console.log(ex);
        }
    }
}(jQuery, window));