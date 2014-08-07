/// <reference path="jquery-2.1.0-vsdoc.js" />
/// <reference path="image-editor.js" />
/// <reference path="linq.min.js" />
/// <reference path="linq-vsdoc.js" />
/// <reference path="jquery.fs.boxer.min.js" />

//JQuery Font Selector 
(function ($) {
    //var settings;

    var methods = {
        init: function (options) {
            var settings = $.extend({
                'hide_fallbacks': false,
                'selected': function (style) { },
                'initial': '',
                'fonts': []
            }, options);

            var root = this;
            root.callback = settings['selected'];
            var visible = false;
            var selected = false;

            var displayName = function (font) {
                if (settings['hide_fallbacks'])
                    if (settings['type'] === 'fontSize')
                        return font.toString();
                    else
                        return font.substr(0, font.indexOf(','));
                else
                    return font;
            }

            var select = function (font) {
                if (settings['type'] === 'fontSize') {
                    root.find('span').html(font.toString());
                }
                else {
                    root.find('span').html(displayName(font).replace(/["']{1}/gi, ""));
                    root.css('font-family', font);
                }
                selected = font;
                root.callback(selected);
            }

            if (settings['type'] === 'fontFamily')
                $(this).prepend('<span>' + settings['initial'].replace(/'/g, '&#039;') + '</span>');
            else
                $(this).prepend('<span>' + settings['initial'] + '</span>');

            var ul = $('<ul class="fontSelectUl"></ul>');
            ul.appendTo(this.find('~.fontList'));
            ul.parent('div').hide();

            if (settings['fonts'] != undefined && settings['fonts'].length > 0) {
                for (var i = 0; i < settings['fonts'].length; i++) {
                    var item = $('<li>' + displayName(settings['fonts'][i]) + '</li>').appendTo(ul);
                    $(item).css('font-family', settings['fonts'][i]);
                }
            }

            if (settings['fontSize'] != undefined && settings['fontSize'].length > 0) {
                for (var i = 0; i < settings['fontSize'].length; i++) {
                    var item = $('<li>' + displayName(settings['fontSize'][i]) + '</li>').appendTo(ul);
                }
            }

            if (settings['initial'] != '')
                select(settings['initial']);

            ul.find('li').click(function () {
                ul.parent('div').slideUp('fast');

                if (settings['type'] === 'fontFamily')
                    select($(this).css('font-family'));
                else
                    select($(this).html());
            });

            $(this).click(function (event) {
                event.stopPropagation();
                $('.fontList').hide();

                if (!$(this).closest('.toolbar-button').is('.disabled')) {
                    ul.parent('div').slideDown('fast');
                }
            });

            $('html').click(function () {
                $('.fontList').slideUp('fast');
            })
        },
        selected: function () {
            if ($(this).data('type') == 'fontFamily')
                return this.css('font-family');
            else
                return $(this).find('span').html();
        },
        select: function (font) {
            if (!isNumber(font)) {
                this.find('span').html(font.substr(0, font.indexOf(',')).replace(/["']{1}/gi, ""));
                this.css('font-family', font);
            }
            else {
                this.find('span').html(font);
            }
            selected = font;
        }
    };

    $.fn.fontSelector = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.fontSelector');
        }
    }
})(jQuery);

//JQuery Color Selector
(function ($) {
    var ColorPicker = function () {
        var
			ids = {},
			inAction,
			charMin = 65,
			visible,
			tpl = '<div class="colorpicker"><div class="colorpicker_color"><div><div></div></div></div><div class="colorpicker_hue"><div></div></div><div class="colorpicker_new_color"></div><div class="colorpicker_current_color"></div><div class="colorpicker_hex"><input type="text" maxlength="6" size="6" /></div><div class="colorpicker_rgb_r colorpicker_field"><input type="text" maxlength="3" size="3" /><span></span></div><div class="colorpicker_rgb_g colorpicker_field"><input type="text" maxlength="3" size="3" /><span></span></div><div class="colorpicker_rgb_b colorpicker_field"><input type="text" maxlength="3" size="3" /><span></span></div><div class="colorpicker_hsb_h colorpicker_field"><input type="text" maxlength="3" size="3" /><span></span></div><div class="colorpicker_hsb_s colorpicker_field"><input type="text" maxlength="3" size="3" /><span></span></div><div class="colorpicker_hsb_b colorpicker_field"><input type="text" maxlength="3" size="3" /><span></span></div><div class="colorpicker_submit"></div></div>',
			defaults = {
			    eventName: 'click',
			    onShow: function () { },
			    onBeforeShow: function () { },
			    onHide: function () { },
			    onChange: function () { },
			    onSubmit: function () { },
			    color: 'ff0000',
			    livePreview: true,
			    flat: false
			},
			fillRGBFields = function (hsb, cal) {
			    var rgb = HSBToRGB(hsb);
			    $(cal).data('colorpicker').fields
					.eq(1).val(rgb.r).end()
					.eq(2).val(rgb.g).end()
					.eq(3).val(rgb.b).end();
			},
			fillHSBFields = function (hsb, cal) {
			    $(cal).data('colorpicker').fields
					.eq(4).val(hsb.h).end()
					.eq(5).val(hsb.s).end()
					.eq(6).val(hsb.b).end();
			},
			fillHexFields = function (hsb, cal) {
			    $(cal).data('colorpicker').fields
					.eq(0).val(HSBToHex(hsb)).end();
			},
			setSelector = function (hsb, cal) {
			    $(cal).data('colorpicker').selector.css('backgroundColor', '#' + HSBToHex({ h: hsb.h, s: 100, b: 100 }));
			    $(cal).data('colorpicker').selectorIndic.css({
			        left: parseInt(150 * hsb.s / 100, 10),
			        top: parseInt(150 * (100 - hsb.b) / 100, 10)
			    });
			},
			setHue = function (hsb, cal) {
			    $(cal).data('colorpicker').hue.css('top', parseInt(150 - 150 * hsb.h / 360, 10));
			},
			setCurrentColor = function (hsb, cal) {
			    $(cal).data('colorpicker').currentColor.css('backgroundColor', '#' + HSBToHex(hsb));
			},
			setNewColor = function (hsb, cal) {
			    $(cal).data('colorpicker').newColor.css('backgroundColor', '#' + HSBToHex(hsb));
			},
			keyDown = function (ev) {
			    var pressedKey = ev.charCode || ev.keyCode || -1;
			    if ((pressedKey > charMin && pressedKey <= 90) || pressedKey == 32) {
			        return false;
			    }
			    var cal = $(this).parent().parent();
			    if (cal.data('colorpicker').livePreview === true) {
			        change.apply(this);
			    }
			},
			change = function (ev) {
			    var cal = $(this).parent().parent(), col;
			    if (this.parentNode.className.indexOf('_hex') > 0) {
			        cal.data('colorpicker').color = col = HexToHSB(fixHex(this.value));
			    } else if (this.parentNode.className.indexOf('_hsb') > 0) {
			        cal.data('colorpicker').color = col = fixHSB({
			            h: parseInt(cal.data('colorpicker').fields.eq(4).val(), 10),
			            s: parseInt(cal.data('colorpicker').fields.eq(5).val(), 10),
			            b: parseInt(cal.data('colorpicker').fields.eq(6).val(), 10)
			        });
			    } else {
			        cal.data('colorpicker').color = col = RGBToHSB(fixRGB({
			            r: parseInt(cal.data('colorpicker').fields.eq(1).val(), 10),
			            g: parseInt(cal.data('colorpicker').fields.eq(2).val(), 10),
			            b: parseInt(cal.data('colorpicker').fields.eq(3).val(), 10)
			        }));
			    }
			    if (ev) {
			        fillRGBFields(col, cal.get(0));
			        fillHexFields(col, cal.get(0));
			        fillHSBFields(col, cal.get(0));
			    }
			    setSelector(col, cal.get(0));
			    setHue(col, cal.get(0));
			    setNewColor(col, cal.get(0));
			    cal.data('colorpicker').onChange.apply(cal, [col, HSBToHex(col), HSBToRGB(col)]);
			},
			blur = function (ev) {
			    var cal = $(this).parent().parent();
			    cal.data('colorpicker').fields.parent().removeClass('colorpicker_focus');
			},
			focus = function () {
			    charMin = this.parentNode.className.indexOf('_hex') > 0 ? 70 : 65;
			    $(this).parent().parent().data('colorpicker').fields.parent().removeClass('colorpicker_focus');
			    $(this).parent().addClass('colorpicker_focus');
			},
			downIncrement = function (ev) {
			    var field = $(this).parent().find('input').focus();
			    var current = {
			        el: $(this).parent().addClass('colorpicker_slider'),
			        max: this.parentNode.className.indexOf('_hsb_h') > 0 ? 360 : (this.parentNode.className.indexOf('_hsb') > 0 ? 100 : 255),
			        y: ev.pageY,
			        field: field,
			        val: parseInt(field.val(), 10),
			        preview: $(this).parent().parent().data('colorpicker').livePreview
			    };
			    $(document).bind('mouseup', current, upIncrement);
			    $(document).bind('mousemove', current, moveIncrement);
			},
			moveIncrement = function (ev) {
			    ev.data.field.val(Math.max(0, Math.min(ev.data.max, parseInt(ev.data.val + ev.pageY - ev.data.y, 10))));
			    if (ev.data.preview) {
			        change.apply(ev.data.field.get(0), [true]);
			    }
			    return false;
			},
			upIncrement = function (ev) {
			    change.apply(ev.data.field.get(0), [true]);
			    ev.data.el.removeClass('colorpicker_slider').find('input').focus();
			    $(document).unbind('mouseup', upIncrement);
			    $(document).unbind('mousemove', moveIncrement);
			    return false;
			},
			downHue = function (ev) {
			    var current = {
			        cal: $(this).parent(),
			        y: $(this).offset().top
			    };
			    current.preview = current.cal.data('colorpicker').livePreview;
			    $(document).bind('mouseup', current, upHue);
			    $(document).bind('mousemove', current, moveHue);
			},
			moveHue = function (ev) {
			    change.apply(
					ev.data.cal.data('colorpicker')
						.fields
						.eq(4)
						.val(parseInt(360 * (150 - Math.max(0, Math.min(150, (ev.pageY - ev.data.y)))) / 150, 10))
						.get(0),
					[ev.data.preview]
				);
			    return false;
			},
			upHue = function (ev) {
			    fillRGBFields(ev.data.cal.data('colorpicker').color, ev.data.cal.get(0));
			    fillHexFields(ev.data.cal.data('colorpicker').color, ev.data.cal.get(0));
			    $(document).unbind('mouseup', upHue);
			    $(document).unbind('mousemove', moveHue);
			    return false;
			},
			downSelector = function (ev) {
			    var current = {
			        cal: $(this).parent(),
			        pos: $(this).offset()
			    };
			    current.preview = current.cal.data('colorpicker').livePreview;
			    $(document).bind('mouseup', current, upSelector);
			    $(document).bind('mousemove', current, moveSelector);
			},
			moveSelector = function (ev) {
			    change.apply(
					ev.data.cal.data('colorpicker')
						.fields
						.eq(6)
						.val(parseInt(100 * (150 - Math.max(0, Math.min(150, (ev.pageY - ev.data.pos.top)))) / 150, 10))
						.end()
						.eq(5)
						.val(parseInt(100 * (Math.max(0, Math.min(150, (ev.pageX - ev.data.pos.left)))) / 150, 10))
						.get(0),
					[ev.data.preview]
				);
			    return false;
			},
			upSelector = function (ev) {
			    fillRGBFields(ev.data.cal.data('colorpicker').color, ev.data.cal.get(0));
			    fillHexFields(ev.data.cal.data('colorpicker').color, ev.data.cal.get(0));
			    $(document).unbind('mouseup', upSelector);
			    $(document).unbind('mousemove', moveSelector);
			    return false;
			},
			enterSubmit = function (ev) {
			    $(this).addClass('colorpicker_focus');
			},
			leaveSubmit = function (ev) {
			    $(this).removeClass('colorpicker_focus');
			},
			clickSubmit = function (ev) {
			    var cal = $(this).parent();
			    var col = cal.data('colorpicker').color;
			    cal.data('colorpicker').origColor = col;
			    setCurrentColor(col, cal.get(0));
			    cal.data('colorpicker').onSubmit(col, HSBToHex(col), HSBToRGB(col), cal.data('colorpicker').el);
			},
			show = function (ev) {
			    var cal = $('#' + $(this).data('colorpickerId'));
			    cal.data('colorpicker').onBeforeShow.apply(this, [cal.get(0)]);
			    var pos = $(this).offset();
			    var viewPort = getViewport();
			    var top = pos.top + this.offsetHeight;
			    var left = pos.left;
			    if (top + 176 > viewPort.t + viewPort.h) {
			        top -= this.offsetHeight + 176;
			    }
			    if (left + 356 > viewPort.l + viewPort.w) {
			        left -= 356;
			    }
			    cal.css({ left: left + 'px', top: top + 'px' });
			    if (cal.data('colorpicker').onShow.apply(this, [cal.get(0)]) != false) {
			        cal.show();
			    }
			    $(document).bind('mousedown', { cal: cal }, hide);
			    return false;
			},
			hide = function (ev) {
			    if (!isChildOf(ev.data.cal.get(0), ev.target, ev.data.cal.get(0))) {
			        if (ev.data.cal.data('colorpicker').onHide.apply(this, [ev.data.cal.get(0)]) != false) {
			            ev.data.cal.hide();
			        }
			        $(document).unbind('mousedown', hide);
			    }
			},
			isChildOf = function (parentEl, el, container) {
			    if (parentEl == el) {
			        return true;
			    }
			    if (parentEl.contains) {
			        return parentEl.contains(el);
			    }
			    if (parentEl.compareDocumentPosition) {
			        return !!(parentEl.compareDocumentPosition(el) & 16);
			    }
			    var prEl = el.parentNode;
			    while (prEl && prEl != container) {
			        if (prEl == parentEl)
			            return true;
			        prEl = prEl.parentNode;
			    }
			    return false;
			},
			getViewport = function () {
			    var m = document.compatMode == 'CSS1Compat';
			    return {
			        l: window.pageXOffset || (m ? document.documentElement.scrollLeft : document.body.scrollLeft),
			        t: window.pageYOffset || (m ? document.documentElement.scrollTop : document.body.scrollTop),
			        w: window.innerWidth || (m ? document.documentElement.clientWidth : document.body.clientWidth),
			        h: window.innerHeight || (m ? document.documentElement.clientHeight : document.body.clientHeight)
			    };
			},
			fixHSB = function (hsb) {
			    return {
			        h: Math.min(360, Math.max(0, hsb.h)),
			        s: Math.min(100, Math.max(0, hsb.s)),
			        b: Math.min(100, Math.max(0, hsb.b))
			    };
			},
			fixRGB = function (rgb) {
			    return {
			        r: Math.min(255, Math.max(0, rgb.r)),
			        g: Math.min(255, Math.max(0, rgb.g)),
			        b: Math.min(255, Math.max(0, rgb.b))
			    };
			},
			fixHex = function (hex) {
			    var len = 6 - hex.length;
			    if (len > 0) {
			        var o = [];
			        for (var i = 0; i < len; i++) {
			            o.push('0');
			        }
			        o.push(hex);
			        hex = o.join('');
			    }
			    return hex;
			},
			HexToRGB = function (hex) {
			    var hex = parseInt(((hex.indexOf('#') > -1) ? hex.substring(1) : hex), 16);
			    return { r: hex >> 16, g: (hex & 0x00FF00) >> 8, b: (hex & 0x0000FF) };
			},
			HexToHSB = function (hex) {
			    return RGBToHSB(HexToRGB(hex));
			},
			RGBToHSB = function (rgb) {
			    var hsb = {
			        h: 0,
			        s: 0,
			        b: 0
			    };
			    var min = Math.min(rgb.r, rgb.g, rgb.b);
			    var max = Math.max(rgb.r, rgb.g, rgb.b);
			    var delta = max - min;
			    hsb.b = max;
			    if (max != 0) {

			    }
			    hsb.s = max != 0 ? 255 * delta / max : 0;
			    if (hsb.s != 0) {
			        if (rgb.r == max) {
			            hsb.h = (rgb.g - rgb.b) / delta;
			        } else if (rgb.g == max) {
			            hsb.h = 2 + (rgb.b - rgb.r) / delta;
			        } else {
			            hsb.h = 4 + (rgb.r - rgb.g) / delta;
			        }
			    } else {
			        hsb.h = -1;
			    }
			    hsb.h *= 60;
			    if (hsb.h < 0) {
			        hsb.h += 360;
			    }
			    hsb.s *= 100 / 255;
			    hsb.b *= 100 / 255;
			    return hsb;
			},
			HSBToRGB = function (hsb) {
			    var rgb = {};
			    var h = Math.round(hsb.h);
			    var s = Math.round(hsb.s * 255 / 100);
			    var v = Math.round(hsb.b * 255 / 100);
			    if (s == 0) {
			        rgb.r = rgb.g = rgb.b = v;
			    } else {
			        var t1 = v;
			        var t2 = (255 - s) * v / 255;
			        var t3 = (t1 - t2) * (h % 60) / 60;
			        if (h == 360) h = 0;
			        if (h < 60) { rgb.r = t1; rgb.b = t2; rgb.g = t2 + t3 }
			        else if (h < 120) { rgb.g = t1; rgb.b = t2; rgb.r = t1 - t3 }
			        else if (h < 180) { rgb.g = t1; rgb.r = t2; rgb.b = t2 + t3 }
			        else if (h < 240) { rgb.b = t1; rgb.r = t2; rgb.g = t1 - t3 }
			        else if (h < 300) { rgb.b = t1; rgb.g = t2; rgb.r = t2 + t3 }
			        else if (h < 360) { rgb.r = t1; rgb.g = t2; rgb.b = t1 - t3 }
			        else { rgb.r = 0; rgb.g = 0; rgb.b = 0 }
			    }
			    return { r: Math.round(rgb.r), g: Math.round(rgb.g), b: Math.round(rgb.b) };
			},
			RGBToHex = function (rgb) {
			    var hex = [
					rgb.r.toString(16),
					rgb.g.toString(16),
					rgb.b.toString(16)
			    ];
			    $.each(hex, function (nr, val) {
			        if (val.length == 1) {
			            hex[nr] = '0' + val;
			        }
			    });
			    return hex.join('');
			},
			HSBToHex = function (hsb) {
			    return RGBToHex(HSBToRGB(hsb));
			},
			restoreOriginal = function () {
			    var cal = $(this).parent();
			    var col = cal.data('colorpicker').origColor;
			    cal.data('colorpicker').color = col;
			    fillRGBFields(col, cal.get(0));
			    fillHexFields(col, cal.get(0));
			    fillHSBFields(col, cal.get(0));
			    setSelector(col, cal.get(0));
			    setHue(col, cal.get(0));
			    setNewColor(col, cal.get(0));
			};
        return {
            init: function (opt) {
                opt = $.extend({}, defaults, opt || {});
                if (typeof opt.color == 'string') {
                    opt.color = HexToHSB(opt.color);
                } else if (opt.color.r != undefined && opt.color.g != undefined && opt.color.b != undefined) {
                    opt.color = RGBToHSB(opt.color);
                } else if (opt.color.h != undefined && opt.color.s != undefined && opt.color.b != undefined) {
                    opt.color = fixHSB(opt.color);
                } else {
                    return this;
                }
                return this.each(function () {
                    if (!$(this).data('colorpickerId')) {
                        var options = $.extend({}, opt);
                        options.origColor = opt.color;
                        var id = 'collorpicker_' + parseInt(Math.random() * 1000);
                        $(this).data('colorpickerId', id);
                        var cal = $(tpl).attr('id', id);
                        if (options.flat) {
                            cal.appendTo(this).show();
                        } else {
                            cal.appendTo(document.body);
                        }
                        options.fields = cal
											.find('input')
												.bind('keyup', keyDown)
												.bind('change', change)
												.bind('blur', blur)
												.bind('focus', focus);
                        cal
							.find('span').bind('mousedown', downIncrement).end()
							.find('>div.colorpicker_current_color').bind('click', restoreOriginal);
                        options.selector = cal.find('div.colorpicker_color').bind('mousedown', downSelector);
                        options.selectorIndic = options.selector.find('div div');
                        options.el = this;
                        options.hue = cal.find('div.colorpicker_hue div');
                        cal.find('div.colorpicker_hue').bind('mousedown', downHue);
                        options.newColor = cal.find('div.colorpicker_new_color');
                        options.currentColor = cal.find('div.colorpicker_current_color');
                        cal.data('colorpicker', options);
                        cal.find('div.colorpicker_submit')
							.bind('mouseenter', enterSubmit)
							.bind('mouseleave', leaveSubmit)
							.bind('click', clickSubmit);
                        fillRGBFields(options.color, cal.get(0));
                        fillHSBFields(options.color, cal.get(0));
                        fillHexFields(options.color, cal.get(0));
                        setHue(options.color, cal.get(0));
                        setSelector(options.color, cal.get(0));
                        setCurrentColor(options.color, cal.get(0));
                        setNewColor(options.color, cal.get(0));
                        if (options.flat) {
                            cal.css({
                                position: 'relative',
                                display: 'block'
                            });
                        } else {
                            $(this).bind(options.eventName, show);
                        }
                    }
                });
            },
            showPicker: function () {
                return this.each(function () {
                    if ($(this).data('colorpickerId')) {
                        show.apply(this);
                    }
                });
            },
            hidePicker: function () {
                return this.each(function () {
                    if ($(this).data('colorpickerId')) {
                        $('#' + $(this).data('colorpickerId')).hide();
                    }
                });
            },
            setColor: function (col) {
                if (typeof col == 'string') {
                    col = HexToHSB(col);
                } else if (col.r != undefined && col.g != undefined && col.b != undefined) {
                    col = RGBToHSB(col);
                } else if (col.h != undefined && col.s != undefined && col.b != undefined) {
                    col = fixHSB(col);
                } else {
                    return this;
                }
                return this.each(function () {
                    if ($(this).data('colorpickerId')) {
                        var cal = $('#' + $(this).data('colorpickerId'));
                        cal.data('colorpicker').color = col;
                        cal.data('colorpicker').origColor = col;
                        fillRGBFields(col, cal.get(0));
                        fillHSBFields(col, cal.get(0));
                        fillHexFields(col, cal.get(0));
                        setHue(col, cal.get(0));
                        setSelector(col, cal.get(0));
                        setCurrentColor(col, cal.get(0));
                        setNewColor(col, cal.get(0));
                    }
                });
            }
        };
    }();
    $.fn.extend({
        ColorPicker: ColorPicker.init,
        ColorPickerHide: ColorPicker.hidePicker,
        ColorPickerShow: ColorPicker.showPicker,
        ColorPickerSetColor: ColorPicker.setColor
    });
})(jQuery);

//jQuery plugin TextAreaExpander
(function ($) {
    $.fn.TextAreaExpander = function (minHeight, maxHeight) {

        var hCheck = !($.browser.msie || $.browser.opera);

        // resize a textarea
        function ResizeTextarea(e) {

            // event or initialize element?
            e = e.target || e;

            // find content length and box width
            var vlen = e.value.length, ewidth = e.offsetWidth;
            if (vlen != e.valLength || ewidth != e.boxWidth) {

                if (hCheck && (vlen < e.valLength || ewidth != e.boxWidth)) e.style.height = "0px";
                var h = Math.max(e.expandMin, Math.min(e.scrollHeight, e.expandMax));

                e.style.overflow = (e.scrollHeight > h ? "auto" : "hidden");
                e.style.height = h + "px";

                e.valLength = vlen;
                e.boxWidth = ewidth;
            }

            return true;
        };

        // initialize
        this.each(function () {

            // is a textarea?
            if (this.nodeName.toLowerCase() != "textarea") return;

            // set height restrictions
            var p = this.className.match(/expand(\d+)\-*(\d+)*/i);
            this.expandMin = minHeight || (p ? parseInt('0' + p[1], 10) : 0);
            this.expandMax = maxHeight || (p ? parseInt('0' + p[2], 10) : 99999);

            // initial resize
            ResizeTextarea(this);

            // zero vertical padding and add events
            if (!this.Initialized) {
                this.Initialized = true;
                $(this).css("padding-top", 0).css("padding-bottom", 0);
                $(this).bind("keyup", ResizeTextarea).bind("focus", ResizeTextarea);
            }
        });

        return this;
    };
})(jQuery);

(function ($, win) {
    var fonts = [
		'Arial,Arial,Helvetica,sans-serif',
		'Arial Black,Arial Black,Gadget,sans-serif',
		'Comic Sans MS,Comic Sans MS,cursive',
		'Courier New,Courier New,Courier,monospace',
		'Georgia,Georgia,serif',
		'Impact,Charcoal,sans-serif',
		'Lucida Console,Monaco,monospace',
		'Lucida Sans Unicode,Lucida Grande,sans-serif',
		'Palatino Linotype,Book Antiqua,Palatino,serif',
		'Tahoma,Geneva,sans-serif',
		'Times New Roman,Times,serif',
		'Trebuchet MS,Helvetica,sans-serif',
		'Verdana,Geneva,sans-serif',
		'Gill Sans,Geneva,sans-serif'
    ];

    var fontSize = [7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 30, 36, 48, 60, 72, 96, 150, 200, 300, 400, 600, 800, 1000, 1200, 1400];

    var weight = [0, 1, 2, 3, 4, 5, 6, 8, 10, 12, 14, 16, 18, 20, 24, 40];

    $(document).click(function (e) {
        $('.button-menu:visible').hide();

        if (editorCanvas.canvas.getActiveObject() == null) {
            setAllToobarCommandDisabled();
        }
    });

    $(document).ready(function () {
        //$('.button-text:not(".disabled"), .button-icon:not(".disabled")').parent('div')
        //    .click(function (e) {
        //        if (!$(this).is('.disabled')) {
        //            $('.button-menu').hide();
        //            $(this).find('.button-menu').css({
        //                'left': $(this).position().left + 2,
        //                'top': $(this).position().top + $(this).height() + 8,
        //                'min-width': ($(this).attr('id') == 'btn_text_number' ||
        //                    $(this).attr('id') == 'btn_symbol_bullet') ? '100%' : $(this).css('width')
        //            }).show();
        //        }
        //        e.stopPropagation();
        //    });

        //$('.toolbar-button').click(function (e) {
        //    var clickedButton = $(this).attr('id');
        //    e.stopPropagation();
        //});

        SetupFontPicker();
        SetupColorPicker();

        //$('#btn_top_toolbar_toggle').click(function (ex) {
        //    try {
        //        $('.toolbar').slideToggle('fast', function () {
        //            if ($('.toolbar').is(':visible')) {
        //                $('#btn_top_toolbar_toggle').addClass('open')
        //                    .css({ 'bottom': '-171px' });
        //            }
        //            else {
        //                $('#btn_top_toolbar_toggle').removeClass('open')
        //                    .css({ 'bottom': '-64px' });
        //            }
        //        })
        //    } catch (ex) {
        //        console.log(ex);
        //    }
        //});

        InitializeCanvas({
            width: parseInt($('#PageWidth').val()),
            height: parseInt($('#PageHeight').val()),
            bgImage: $('#TemplatePageUrl').val(),
            scaleValue: 0.5494588235294117,
            renderData: function () {
                var returnValue = '';
                var templateId = sessionStorage.getItem('SELECTED_TEMPLATE_ID');
                var currentlyProcessingImagePageId = ($.QueryString['pageid'] == undefined ?
                    $('#pageid').val() : $.QueryString['pageid']);

                var currentlyProcessingImageJsonData = sessionStorage
                    .getItem('{0}_{1}_JSON'.format(templateId, currentlyProcessingImagePageId));

                if (currentlyProcessingImageJsonData != null && currentlyProcessingImageJsonData != '') {
                    returnValue = currentlyProcessingImageJsonData;
                }
                else {
                    returnValue = $('#TemplatePageJson').val();
                }
                return returnValue;
            }
        })

        CommandAction();
        MakeToolbarSticky();
        setAllToobarCommandDisabled();
        $('#inline_textarea').TextAreaExpander();

        SetPopupCommandTooltip();
        Dropzone.autoDiscover = false;

        var prevZoomValue = 0;
        $('#imageZoomer').slider({
            min: 0,
            max: 500,
            value: 0,
            step: 10,
            change: function (event, ui) {
                var currentZoom = 0;
                if (prevZoomValue > ui.value)
                    currentZoom = -10;
                else
                    currentZoom = 10;

                editorCanvas.zoomBy(0, 0, currentZoom);
                prevZoomValue = ui.value;
            }
        });
    });

    $(document).on('click', '.dz-details', function () {
        var $self = $(this).find('.image-link');
        editorCanvas.onFileLoad($self.val());
        ConfirmUI('Selected image is added to the editor', function () {
            modal.close();
        }, null, null, null, 'Close window', 'Continue Upload');
    });

    this.AddImageToPage = function (elm) {
        try {
            var $self = $(elm).closest('.dz-preview').find('.dz-details .image-link');
            editorCanvas.onFileLoad($self.val());
            ConfirmUI('Selected image is added to the editor', function () {
                modal.close();
            }, null, null, null, 'Close window', 'Continue Upload');
        } catch (ex) {
            console.log(ex);
        }
    }

    $(document).on('click', '.hove-highlighter', function () {
        try {
            var $self = $(this);
            var currentHighligtedObjectName = $self.data('current-object');
            if (currentHighligtedObjectName != null || currentHighligtedObjectName != '') {
                var currentHighligtedObject = editorCanvas.canvas
                    .getItemByName(currentHighligtedObjectName);

                if (currentHighligtedObject) {
                    editorCanvas.canvas.setActiveObject(currentHighligtedObject);
                    $self.data('current-object', '');
                    $self.hide();
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    });

    $(document).on('click', '#crop-top, #crop-right, #crop-bottom, #crop-left', function (event) {
        try {
            switch ($(this).attr('id')) {
                case 'crop-top':
                    editorCanvas.zoomBy(0, -5, 0);
                    break;

                case 'crop-right':
                    editorCanvas.zoomBy(5, 0, 0);
                    break;

                case 'crop-bottom':
                    editorCanvas.zoomBy(0, 5, 0);
                    break;

                case 'crop-left':
                    editorCanvas.zoomBy(-5, 0, 0);
                    break;
            }
        } catch (ex) {
            console.log(ex);
        }
    });

    $(document).on('change', '#replaceImage', function (event) {
        try {
            var $self = $(this);
            var file = $self[0].files[0]
            var image = new Image();

            if (!window.FileReader) {
                alert("The file API isn't supported on this browser yet.", function () { return; });
                return;
            }

            if (file.size > ((1024 * 1024) * 2)) {
                alert("File size is too large. Maximum file size should be no more than 2 MB. Please resize your image.", function () { return; });
            }
            else {
                var reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onload = function (_file) {
                    image.src = _file.target.result;
                    try { $.blockUI({ message: $("#dataloading") }); } catch (ex) { }

                    image.onload = function () {
                        var h = this.height, w = this.width;
                        try { $.unblockUI(); } catch (ex) { }
                        if (h > 300 || w > 300) {
                            alert("File size is too large. Maximum file resolution should be no more than 300 pixels. Please resize your image.");
                        }
                        else
                            $self.parent('form').submit();
                    }
                }
            }

        } catch (ex) {
            try { $.unblockUI(); } catch (ex) { }
            console.log(ex);
        }
    });

    this.ReplaceImage_OnBegin = function () {
        try {
            try { $.blockUI({ message: $("#dataloading") }); } catch (ex) { }
        } catch (ex) {
            console.log(ex);
        }
    };

    this.ReplaceImage_OnSuccess = function () {
        try {
            var $data = $('#UploadTarget').contents().find('input#response');

            if ($data.length > 0) {
                var responseStr = Base64Decode($data.val());
                var responseJsonData = JSON.parse(responseStr);

                if (responseJsonData) {
                    if (responseJsonData.Status == 'success') {
                        editorCanvas.replaceImage(responseJsonData.url);
                    }
                    else {
                        alert(Base64Decode(responseJsonData.Message));
                    }
                }
            }

            try { $.unblockUI(); } catch (ex) { }
        } catch (ex) {
            console.log(ex);
        }
    };

    this.CommandAction = function () {
        $('.toolbar-button:not(".disabled")').unbind("click");
        $('.toolbar-button:not(".disabled")').click(function (e) {
            try {
                if (!$(this).is('.disabled')) {
                    if ($(this).has('.button-text:not(".disabled"), .button-icon:not(".disabled")')) {
                        $('.button-menu').hide();
                        $(this).find('.button-menu').css({
                            'left': $(this).position().left + 2,
                            'top': $(this).position().top + $(this).height() + 8,
                            'min-width': ($(this).attr('id') == 'btn_text_number' ||
                                $(this).attr('id') == 'btn_symbol_bullet') ? '100%' : $(this).css('width')
                        }).show();
                    }
                    e.stopPropagation();


                    switch ($(this).attr('id')) {
                        case 'btn_top_toolbar_toggle':
                            $('.toolbar').slideToggle('fast', function () {
                                if ($('.toolbar').is(':visible')) {
                                    $('#btn_top_toolbar_toggle').addClass('open')
                                        .css({ 'bottom': '-171px' });
                                }
                                else {
                                    $('#btn_top_toolbar_toggle').removeClass('open')
                                        .css({ 'bottom': '-64px' });
                                }
                            });
                            break;

                        case 'insert_company_name':
                        case 'insert_slogan':
                        case 'insert_your_name':
                        case 'insert_job_title':
                        case 'insert_phone':
                        case 'insert_fax':
                        case 'insert_email_address':
                        case 'insert_web_address':
                        case 'insert_address_line_1':
                        case 'insert_user_text':
                            editorCanvas.insertText($(this).data('body'), GetCurrentFontFamily(), 20);
                            $('.insert-text-menu').hide();
                            break;

                        case 'btn_insert_upload':
                            ShowImageUploadPopup();
                            break;

                        case 'btn_text_bold':
                        case 'btn_text_italic':
                            if ($(this).is('.on')) {
                                editorCanvas.setTextFormat($(this).data('formatter-type'), false);
                                $(this).removeClass('on');
                            }
                            else {
                                editorCanvas.setTextFormat($(this).data('formatter-type'), true);
                                $(this).addClass('on');
                            }
                            break;

                        case 'btn_text_gravity_west':
                        case 'btn_text_gravity_center':
                        case 'btn_text_gravity_east':
                            $('#btn_text_gravity_west, #btn_text_gravity_center, #btn_text_gravity_east').removeClass('on');
                            editorCanvas.setTextFormat($(this).data('formatter-type'));
                            $(this).addClass('on');
                            break;

                        case 'btn_symbol_bullet_none':
                        case 'btn_symbol_bullet_black_circle':
                        case 'btn_symbol_bullet_white_circle':
                        case 'btn_symbol_bullet_black_square':
                        case 'btn_symbol_bullet_white_square':
                        case 'btn_symbol_bullet_dash':
                            var formatType = $(this).data('symbol-bullet-format-type');
                            if (formatType == 'none') { editorCanvas.removeBulletList() }
                            else { editorCanvas.addBulletList('SymbolBulletPoint', formatType); }
                            $('#btn_symbol_bullet .button-menu').css('display', 'none');
                            $('#btn_symbol_bullet .on').removeClass('on');
                            $(this).addClass('on');
                            break;

                        case 'btn_text_number_none':
                        case 'btn_text_bullet_number_dot':
                        case 'btn_text_bullet_number_dash':
                        case 'btn_text_bullet_number_parenth':
                            var formatType = $(this).data('text-bullet-format-type');
                            if (formatType == 'none') { editorCanvas.removeBulletList() }
                            else { editorCanvas.addBulletList('NumberBulletPoint', formatType); }
                            $('#btn_text_number .button-menu').css('display', 'none');
                            $('#btn_symbol_bullet .on').removeClass('on');
                            $(this).addClass('on');
                            break;

                        case 'btn_edit_copy':
                            editorCanvas.copyObject();
                            break;

                        case 'btn_edit_paste':
                            editorCanvas.pasteObject();
                            break;

                        case 'btn_arrange_bring_front':
                        case 'btn_arrange_send_back':
                        case 'btn_arrange_bring_forward':
                        case 'btn_arrange_send_backward':

                        case 'btn_align_left':
                        case 'btn_align_center':
                        case 'btn_align_right':
                        case 'btn_align_top':
                        case 'btn_align_middle':
                        case 'btn_align_bottom':
                        case 'btn_dist_ver':
                        case 'btn_dist_hor':
                            editorCanvas.setPosition($(this).data('position-type'));
                            $('#btn_advance_align .button-menu, #btn_advance_layers .button-menu').hide();
                            break;

                        case 'btn_text_remove':
                        case 'btn_edit_delete':
                        case 'btn_shape_delete':
                            if (editorCanvas.deleteElement()) {
                                $('#textbox_tools, #crop_toolbar, #shape_toolbar').css('display', 'none');
                            }
                            break;

                        case 'btn_edit_lock':
                            if ($(this).is('.on')) {
                                editorCanvas.lockUnlockObject(false);
                                $(this).removeClass('on');
                            }
                            else {
                                editorCanvas.lockUnlockObject(true);
                                $(this).addClass('on');
                            }
                            break;

                        case 'btn_text_done':
                            var changedStr = $('#textbox_tools textarea').val();
                            if (!changedStr.isEmptyOrBlank()) {
                                editorCanvas.editText($('#textbox_tools textarea').val());
                                $('#textbox_tools').css('display', 'none');
                                $('#textbox_tools textarea').val('');
                                editorCanvas.canvas.discardActiveObject();
                            }
                            break;

                        case 'btn_crop_done':
                            $('#crop_toolbar').css('display', 'none');
                            break;

                        case 'btn_shape_rect':
                        case 'btn_shape_circle':
                        case 'btn_shape_hline':
                        case 'btn_shape_vline':
                            editorCanvas.insertShape($(this).data('shape-type'));
                            $('.insert-shape-menu').hide();
                            break;

                        case 'btn_zoom_out':
                            editorCanvas.canvasZoomOut();
                            break;

                        case 'btn_zoom_reset':
                            editorCanvas.resetZoom();
                            break;

                        case 'btn_zoom_in':
                            editorCanvas.canvasZoomIn();
                            break;

                            //case 'btn_view_preview':
                            //    break;

                        case 'btn_edit_undo':
                            editorCanvas.undo();
                            break;

                        case 'btn_edit_redo':
                            editorCanvas.redo();
                            break;

                        case 'shape_border_stroke_solid':
                        case 'shape_border_stroke_round_dot':
                        case 'shape_border_stroke_square_dot':
                        case 'shape_border_stroke_dash':
                        case 'shape_border_stroke_dash_dot':
                        case 'shape_border_stroke_long_dash':
                        case 'shape_border_stroke_long_dash_dot':
                            var arrayIndex = parseInt($(this).data('array-index'));
                            editorCanvas.setStrokeDashArray(arrayIndex);
                            $('#shape_border_stroke_menu').hide();
                            break;
                    }
                }
            } catch (ex) {
                console.log(ex);
            }
        });
    }

    var SetupColorPicker = function () {
        try {
            $('#fcolor').CreateColorPicker(function (hsb, hex, rgb, el) {
                editorCanvas.setForegroundColor(hex);
            });

            $('#bgcolor').CreateColorPicker(function (hsb, hex, rgb, el) {
                editorCanvas.setBackgroundColor(hex);
            });

            $('#fillcolor').CreateColorPicker(function (hsb, hex, rgb, el) {
                editorCanvas.setForegroundColor(hex);
            });

            $('#bordercolor').CreateColorPicker(function (hsb, hex, rgb, el) {
                editorCanvas.setStrokeColor(hex);
            });
            $('#bordercolor div, #fillcolor div').css({ 'top': '10px' });
        } catch (ex) {
            console.log(ex);
        }
    }

    var SetupFontPicker = function () {
        try {
            $('#fontSelector').CreateFontPicker('fontFamily',
            'Courier New,Courier New,Courier,monospace', function (style) {
                if (editorCanvas.canvas)
                    editorCanvas.setTextFontFamily(style);
            });
            $('#fontSelector span').css({ 'width': '94%' });

            $('#fontSizeSelector').CreateFontPicker('fontSize', 14, function (size) {
                if (editorCanvas.canvas)
                    editorCanvas.setTextFontSize(size);
            });
            $('#fontSizeSelector span').css({ 'width': '82%' });

            $('#weightSelector').CreateFontPicker('fontSize', 0, function (size) {
                if (editorCanvas.canvas)
                    editorCanvas.setShapeStrokeWidth(size);
            }, true);
            $('#weightSelector span').css({ 'width': '75%' });
        } catch (ex) {
            console.log(ex);
        }
    }

    $.fn.CreateColorPicker = function (onSubmitHandler) {
        var $self = $(this);
        $self.ColorPicker({
            color: '#0000ff',
            onShow: function (colpkr) {
                if (!$self.closest('.toolbar-button').is('.disabled')) {
                    $(colpkr).fadeIn(500);
                }
                return false;
            },
            onHide: function (colpkr) {
                $(colpkr).fadeOut(500);
                return false;
            },
            onSubmit: function (hsb, hex, rgb, el) {
                $self.filter('div').css('backgroundColor', '#' + hex);
                $(el).ColorPickerHide();
                if (onSubmitHandler != undefined)
                    onSubmitHandler(hsb, hex, rgb, el);
            }
        });
    };

    $.fn.CreateFontPicker = function (type, initialValue, onSelectHandler, enableWeight) {
        var $self = $(this);
        var settings = {
            'type': type,
            'hide_fallbacks': true,
            'initial': initialValue,
            'selected': onSelectHandler == undefined ? function (style) { } : onSelectHandler,
            'fonts': type.toLowerCase() == 'fontfamily' ? fonts : (enableWeight == undefined || enableWeight == null) ? fontSize : weight
        }
        $self.fontSelector(settings);
    }

    $.fn.MakeImageProportionate = function (srcHeight, srcWidth, maxHeight, maxWidth) {
        try {
            var maxWidth = (maxHeight) ? maxHeight : $(this).height(); // Max width for the image
            var maxHeight = (maxWidth) ? maxWidth : $(this).width();    // Max height for the image
            var ratio = 0;  // Used for aspect ratio
            // Check if the current width is larger than the max
            if (srcWidth > srcHeight) {
                maxHeight = (srcHeight / srcWidth) * maxHeight;

            } else if (srcHeight > srcWidth) {
                maxWidth = (srcWidth / srcHeight) * maxWidth;
            }
            $(this).css("width", maxWidth); // Set new width
            $(this).css("height", maxHeight);  // Scale height based on ratio
        } catch (ex) {
            console.log(ex);
        }
    }

    $.fn.center = function (type) {
        var $self = $(this);
        $self.css({ 'top': '0px', 'left': '0px' });
        var leftOffset = ($self.parent().width() - $self.width());
        var topOffset = ($self.parent().height() - $self.height());

        if (type == 'H') {
            $self.css("margin-left", (leftOffset / 2) + "px");
        }
        else if (type == 'V') {
            $self.css("margin-top", (topOffset / 2) + "px");
        }
        else {
            $self.css({
                "margin-left": (leftOffset / 2) + "px",
                "margin-top": (topOffset / 2) + "px"
            });
        }
        return this;
    }

    $.fn.centerByCss = function (type) {
        if (type == 'H') {
            this.css("left", (($(this).parent().width() / 2) - (parseInt($(this).css('width').replace('px', '')) / 2)) + 'px');
        }
        else if (type == 'V') {
            this.css("top", (($(this).parent().height() / 2) - (parseInt($(this).css('height').replace('px', '')) / 2)) + 'px');
        }
        else {
            this.css("left", (($(this).parent().width() / 2) - (parseInt($(this).css('width').replace('px', '')) / 2)) + 'px');
            this.css("top", (($(this).parent().height() / 2) - (parseInt($(this).css('height').replace('px', '')) / 2)) + 'px');
        }
        return this;
    }

    var MakeToolbarSticky = function () {
        var toolbarHeight = $('.toolbar').outerHeight();
        var toolbarTop = $('.toolbar').offset().top + 5;

        $(win).scroll(function (e) {
            editorCanvas.canvas.calcOffset();

            if ($('.toolbar').is(':visible')) {
                if ($(this).scrollTop() > (toolbarTop)) {
                    $('.toolbar').css({ 'position': 'fixed', 'top': 0 });
                }
                else if ($(this).scrollTop() <= (toolbarTop)) {
                    $('.toolbar').removeAttr('style');
                }
            }
        });

        $(win).scroll();
    };

    var GetCurrentFontFamily = function () {
        return $('#fontSelector').fontSelector('selected');
    };

    var GetCurrentFontSize = function () {
        return $('#fontSizeSelector').fontSelector('selected');
    };

    this.setAllToobarCommandDisabled = function () {
        try {
            //---Text format section
            $('#btn_text_font_face').addClass('disabled');
            $('#btn_text_font_size').addClass('disabled');
            $('#btn_text_font_color').addClass('disabled');

            $('#btn_text_bold').addClass('disabled');
            $('#btn_text_italic').addClass('disabled');
            $('#btn_text_gravity_west').addClass('disabled');
            $('#btn_text_gravity_center').addClass('disabled');
            $('#btn_text_gravity_east').addClass('disabled');
            $('#btn_symbol_bullet').addClass('disabled');
            $('#btn_text_number').addClass('disabled');

            $('#btn_edit_lock').addClass('disabled');
            $('#btn_edit_delete').addClass('disabled');
            $('#btn_bacground_color').addClass('disabled');
            $('#btn_edit_copy').addClass('disabled');

            $('#btn_advance_align').addClass('disabled');
            $('#btn_advance_layers').addClass('disabled');
            $('#btn_advance_rotate').addClass('disabled');
            $('#btn_advance_crop').addClass('disabled');
        } catch (ex) {
            console.log(ex);
        }
    };

    var SetPopupCommandTooltip = function () {
        try {
            $('.crop-toolbar-item:has("span")')
                .hover(function (event) {
                    $(this).find('span').show();
                }, function (event) {
                    $(this).find('span').hide();
                });
        } catch (ex) {
            console.log(ex);
        }
    };

    var ShowImageUploadPopup = function () {
        try {
            $.ajax({
                url: '{0}/Editor/GetUploadWindow'.format(virtualDirectory),
                type: 'GET',
                success: function (result) {
                    modal.open({
                        content: result,
                        width: '735px',
                        openCallBack: function (element) {
                            var $self = element;
                            $self.find('form#frmImageUpload').dropzone({
                                url: '{0}/Editor/UploadImage'.format(virtualDirectory),
                                previewTemplate: "<div class=\"dz-preview dz-file-preview\">\n  <div class=\"dz-details\">\n    <div class=\"dz-filename\"><span data-dz-name></span></div>\n    <div class=\"dz-size\" data-dz-size></div>\n <input type=\"hidden\" class=\"image-link\" />   <img data-dz-thumbnail />\n  </div>\n  <div class=\"dz-progress\"><span class=\"dz-upload\" data-dz-uploadprogress></span></div>\n  <div class=\"dz-success-mark\"><span>✔</span></div>\n  <div class=\"dz-error-mark\"><span>✘</span></div>\n  <div class=\"dz-error-message\"><span data-dz-errormessage></span></div>\n<a class=\"dz-remove\" href=\"javascript:void(0);\" onclick=\"AddImageToPage(this);\">Add to page</a></div>",
                                paramName: "files", // The name that will be used to transfer the file
                                maxFilesize: 2, // MB
                                addRemoveLinks: true,
                                acceptedFiles: 'image/*',
                                accept: function (file, done) {
                                    //var render = new FileReader();
                                    //render.readAsDataURL(file);
                                    //$(file.previewTemplate).find('.image-link').val(render.result);
                                    return done();
                                },
                                init: function () {
                                    var myDropzone = this;

                                    myDropzone.on("success", function (file, response) {
                                        $(file.previewTemplate).find('.image-link').val(response.url[0]);
                                        //alert(file);
                                        // Gets triggered when the files have successfully been sent.
                                        // Redirect user or notify of success.
                                    });
                                    myDropzone.on("error", function (file, response) {
                                        //alert(file);
                                        // Gets triggered when there was an error sending the files.
                                        // Maybe show form again, and notify user of error
                                    });
                                    myDropzone.on("successmultiple", function (files, response) {
                                        alert(file);
                                        // Gets triggered when the files have successfully been sent.
                                        // Redirect user or notify of success.
                                    });
                                }
                            });
                        }
                    });
                }
            });

        } catch (ex) {
            console.log(ex);
        }
    };

    var InitializeCanvas = function (options) {
        try {
            editorCanvas.init({
                name: 'canvas',
                width: options.width, height: options.height,
                backgroundSrc: options.bgImage,
                scaleValue: options.scaleValue,
                renderData: options.renderData()
            });
            $('.canvas-container').centerByCss('H');
        } catch (ex) {
            console.log(ex);
        }
    };

    var SetCanvasPosition = function () {
        try {
            $mainCanvas = $('#canvas');
            var displayWidth = $('#canvas').data('display-w');
            var displayHeight = $('#canvas').data('display-h');
            var style = {
                'width': displayWidth + 'px',
                'height': (parseInt(displayHeight) + 50) + 'px'
            };

            $('.canvas-container').css(style);
            $('.lower-canvas').css(style);
            $('.upper-canvas').css(style);
            $('.canvas-container').centerByCss('H');
        } catch (ex) {
            console.log(ex);
        }
    };
}(jQuery, window));