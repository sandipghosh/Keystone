/// <reference path="fabric.js" />
/// <reference path="common-script.js" />
/// <reference path="jquery-2.1.0-vsdoc.js" />

var editorCanvas = {
    canvas: null,
    imgCorners: ['TL', 'TR', 'BR', 'BL', 'MT', 'MB', 'ML', 'MR', 'MTR'],
    stokeArrayCollection: [[], [3, 3], [5, 5], [10, 5], [10, 5, 5, 10], [20, 5], [10, 5, 5, 10]],
    cornerImages: [],
    copiedObject: null,
    copiedObjects: new Array(),

    CANVAS_SCALE: 1.000000000000000,
    SCALE_FACTOR: 1.200000000000000,
    ZoomType: {
        ZoomIn: 'ZoomIn',
        ZoomOut: 'ZoomOut',
        ZoomReset: 'ZoomReset'
    },
    //globalBoundryLimit: 30,
    globalBoundryLimit: 10,
    current: null,
    scaleDownExecution: 0,
    currentScaleX: 0,
    currentScaleY: 0,
    list: [],
    state: [],
    index: 0,
    index2: 0,
    action: false,
    refresh: true,
    commonBorderColor: '#ffc13d',
    commonCornerColor: '#fa7a08',
    commonEditBorderColor: '#83b61a',
    showGrid: false,
    gridScaleSpacing: 10,
    gridLinePrefix: 'gridLine',

    windowScrollTop: 0,
    objectType: {
        Group: 'group',
        IText: 'i-text',
        Text: 'text',
        Circle: 'circle',
        Rect: 'rect',
        Line: 'line',
        Image: 'cropzoomimage'
    },

    objManip: function (prop, value) {
        var obj = editorCanvas.canvas.getActiveObject();
        if (!obj) { return true; }

        switch (prop) {
            case 'zoomBy-x':
                obj.zoomBy(value, 0, 0, function () { editorCanvas.canvas.renderAll() });
                break;
            case 'zoomBy-y':
                obj.zoomBy(0, value, 0, function () { editorCanvas.canvas.renderAll() });
                break;
            case 'zoomBy-z':
                obj.zoomBy(0, 0, value, function () { editorCanvas.canvas.renderAll() });
                break;
            default:
                obj.set(prop, obj.get(prop) + value);
                break;
        }

        if ('left' === prop || 'top' === prop) { obj.setCoords(); }
        editorCanvas.canvas.renderAll();
        return false;
    },

    zoomBy: function (x, y, z) {
        var activeObject = editorCanvas.canvas.getActiveObject();
        if (activeObject) {
            if (activeObject.type == editorCanvas.objectType.Image)
                activeObject.zoomBy(x, y, z, function () { editorCanvas.canvas.renderAll() });
            else
                alert('Not Applicable');
        }
    },

    canvasZoomIn: function () {
        try {
            if (editorCanvas.CANVAS_SCALE < 4.900000000000000) {
                editorCanvas.CANVAS_SCALE = editorCanvas.CANVAS_SCALE * editorCanvas.SCALE_FACTOR;
                editorCanvas.canvas.setHeight(editorCanvas.canvas.getHeight() * editorCanvas.SCALE_FACTOR);
                editorCanvas.canvas.setWidth(editorCanvas.canvas.getWidth() * editorCanvas.SCALE_FACTOR);

                var objects = editorCanvas.canvas.getObjects();
                for (var i in objects) {
                    editorCanvas.scaleObjectOnZoom(objects[i], editorCanvas.ZoomType.ZoomIn);
                }

                var bgImageObj = editorCanvas.canvas.backgroundImage;
                if (bgImageObj) {
                    editorCanvas.scaleObjectOnZoom(bgImageObj, editorCanvas.ZoomType.ZoomIn);
                }

                editorCanvas.canvas.renderAll();
                editorCanvas.canvas.calcOffset();
                $('.canvas-container').center('H');
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    canvasZoomOut: function () {
        try {
            if (editorCanvas.CANVAS_SCALE > .200000000000000) {
                editorCanvas.CANVAS_SCALE = editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR;
                editorCanvas.canvas.setHeight(editorCanvas.canvas.getHeight() * (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR));
                editorCanvas.canvas.setWidth(editorCanvas.canvas.getWidth() * (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR));

                var objects = editorCanvas.canvas.getObjects();
                for (var i in objects) {
                    editorCanvas.scaleObjectOnZoom(objects[i], editorCanvas.ZoomType.ZoomOut);
                }

                var bgImageObj = editorCanvas.canvas.backgroundImage;
                if (bgImageObj) {
                    editorCanvas.scaleObjectOnZoom(bgImageObj, editorCanvas.ZoomType.ZoomOut);
                }

                editorCanvas.canvas.renderAll();
                editorCanvas.canvas.calcOffset();
                $('.canvas-container').center('H');
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    resetZoom: function () {
        try {
            editorCanvas.canvas.setHeight(editorCanvas.canvas.getHeight() * (1 / editorCanvas.CANVAS_SCALE));
            editorCanvas.canvas.setWidth(editorCanvas.canvas.getWidth() * (1 / editorCanvas.CANVAS_SCALE));

            var objects = editorCanvas.canvas.getObjects();
            for (var i in objects) {
                editorCanvas.scaleObjectOnZoom(objects[i], editorCanvas.ZoomType.ZoomReset);
            }

            var bgImageObj = editorCanvas.canvas.backgroundImage;
            if (bgImageObj) {
                editorCanvas.scaleObjectOnZoom(bgImageObj, editorCanvas.ZoomType.ZoomReset);
            }

            editorCanvas.canvas.renderAll();
            editorCanvas.CANVAS_SCALE = 1.00;
            editorCanvas.canvas.calcOffset();
            $('.canvas-container').center('H');
        } catch (ex) {
            console.log(ex);
        }
    },

    canvasScaleDown: function (options) {
        try {
            if (editorCanvas.CANVAS_SCALE > .200000000000000) {
                editorCanvas.CANVAS_SCALE = options.scaleValue / editorCanvas.SCALE_FACTOR;

                if (!options.IsCanvasOnly) {
                    editorCanvas.canvas.setHeight(options.orgHeight * (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR));
                    editorCanvas.canvas.setWidth(options.orgWidth * (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR));

                    var objects = editorCanvas.canvas.getObjects();
                    for (var i in objects) {
                        editorCanvas.scaleObjectOnZoom(objects[i], editorCanvas.ZoomType.ZoomOut);
                    }

                    var bgImageObj = editorCanvas.canvas.backgroundImage;
                    if (bgImageObj) {
                        editorCanvas.scaleObjectOnZoom(bgImageObj, editorCanvas.ZoomType.ZoomOut);
                    }
                }
                else {
                    editorCanvas.canvas.setHeight((options.orgHeight * (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR)) - 5);
                    editorCanvas.canvas.setWidth((options.orgWidth * (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR)) - 5);
                }

                editorCanvas.canvas.renderAll();
                editorCanvas.canvas.calcOffset();
                $('.canvas-container').center('H');
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    scaleObjectOnZoom: function (targetObject, zoomType) {
        try {
            var scaleX = targetObject.scaleX;
            var scaleY = targetObject.scaleY;
            var left = targetObject.left;
            var top = targetObject.top;

            var tempScaleX = 1;
            var tempScaleY = 1;
            var tempLeft = 1;
            var tempTop = 1;

            if (zoomType == editorCanvas.ZoomType.ZoomIn) {
                tempScaleX = scaleX * editorCanvas.SCALE_FACTOR;
                tempScaleY = scaleY * editorCanvas.SCALE_FACTOR;
                tempLeft = left * editorCanvas.SCALE_FACTOR;
                tempTop = top * editorCanvas.SCALE_FACTOR;
            }
            else if (zoomType == editorCanvas.ZoomType.ZoomOut) {
                tempScaleX = scaleX * (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR);
                tempScaleY = scaleY * (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR);
                tempLeft = left * (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR);
                tempTop = top * (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR);
            }
            else if (zoomType == editorCanvas.ZoomType.ZoomReset) {
                tempScaleX = scaleX * (1 / editorCanvas.CANVAS_SCALE);
                tempScaleY = scaleY * (1 / editorCanvas.CANVAS_SCALE);
                tempLeft = left * (1 / editorCanvas.CANVAS_SCALE);
                tempTop = top * (1 / editorCanvas.CANVAS_SCALE);
            }

            targetObject.scaleX = tempScaleX;
            targetObject.scaleY = tempScaleY;
            targetObject.left = tempLeft;
            targetObject.top = tempTop;
            targetObject.setCoords();

        } catch (ex) {
            console.log(ex);
        }
    },

    deleteElement: function () {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                ConfirmUI("Are you sure you want to delete?",
                    function () {
                        editorCanvas.state[++editorCanvas.index] = editorCanvas.getJsonDataFromCanvas(true);
                        editorCanvas.canvas.remove(activeObject);
                        editorCanvas.showHidePopupCommand(activeObject, 'hide');
                    },
                    function () {
                        return false;
                    });
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    //bulletType --> 'SymbolBulletPoint' / 'NumberBulletPoint'
    addBulletList: function (bulletType, formatType) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) {
                    editorCanvas.removeBulletList();
                    activeObject = editorCanvas.canvas.getActiveObject();
                }

                if (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text) {
                    var bulletPoints = [activeObject];
                    var lines = activeObject.text.split('\n');
                    var fontSize = activeObject.fontSize;
                    var bulletTextFormat;

                    $.each(lines, function (index, element) {
                        var bulletPoint;
                        var bulletPointName = '{0}_{1}_{2}_{3}'.format(activeObject.name, bulletType, formatType, index + 1);

                        if (bulletType == 'SymbolBulletPoint') {
                            var symbolBulletPointTop = (activeObject.top + ((index) *
                                (activeObject.fontSize * activeObject.lineHeight))) + (activeObject.fontSize / 2);

                            switch (formatType) {
                                case 'type1':
                                    bulletPoint = new fabric.Circle({
                                        name: bulletPointName,
                                        radius: (activeObject.fontSize / 5),
                                        fill: activeObject.fill,
                                        top: symbolBulletPointTop
                                    });
                                    break;

                                case 'type2':
                                    bulletPoint = new fabric.Circle({
                                        name: bulletPointName,
                                        radius: (activeObject.fontSize / 5),
                                        fill: activeObject.backgroundColor == null ?
                                            '#ffffff' : activeObject.backgroundColor,
                                        strokeWidth: 1,
                                        stroke: activeObject.fill,
                                        top: symbolBulletPointTop
                                    });
                                    break;

                                case 'type3':
                                    bulletPoint = new fabric.Rect({
                                        name: bulletPointName,
                                        fill: activeObject.fill,
                                        height: (activeObject.fontSize / 3),
                                        width: (activeObject.fontSize / 3),
                                        top: symbolBulletPointTop
                                    });
                                    break;

                                case 'type4':
                                    bulletPoint = new fabric.Rect({
                                        name: bulletPointName,
                                        fill: activeObject.backgroundColor == null ?
                                            '#ffffff' : activeObject.backgroundColor,
                                        strokeWidth: 1,
                                        stroke: activeObject.fill,
                                        height: (activeObject.fontSize / 3),
                                        width: (activeObject.fontSize / 3),
                                        top: symbolBulletPointTop
                                    });
                                    break;
                            }
                        }
                        else if (bulletType == 'NumberBulletPoint') {
                            switch (formatType) {
                                case 'type1':
                                    bulletTextFormat = (activeObject.textAlign == 'right') ?
                                        '.{0}'.format(index + 1) : '{0}.'.format(index + 1);
                                    break;

                                case 'type2':
                                    bulletTextFormat = (activeObject.textAlign == 'right') ?
                                        '-{0}'.format(index + 1) : '{0}-'.format(index + 1);
                                    break;

                                case 'type3':
                                    bulletTextFormat = (activeObject.textAlign == 'right') ?
                                        '({0}'.format(index + 1) : '{0})'.format(index + 1);;
                                    break;
                            }

                            bulletPoint = new fabric.Text(bulletTextFormat, {
                                name: bulletPointName,
                                fontStyle: activeObject.fontStyle,
                                fontFamily: activeObject.fontFamily,
                                fontSize: activeObject.fontSize,
                                fontWeight: activeObject.fontWeight,
                                fontStyle: activeObject.fontStyle,
                                fill: activeObject.fill,
                                top: (activeObject.top + ((index) *
                                    (activeObject.fontSize * activeObject.lineHeight)))
                            });

                        }

                        if (activeObject.textAlign == 'right') {
                            bulletPoint.set({
                                left: (activeObject.left + activeObject.width) + (activeObject.fontSize / 2)
                            });
                        }
                        else {
                            bulletPoint.set({
                                left: activeObject.left - (activeObject.fontSize)
                            });
                        }

                        bulletPoints.push(bulletPoint);
                    });

                    var bulletGroup = new fabric.Group(bulletPoints, {
                        name: '{0}_{1}_Group_{2}'.format(activeObject.name, bulletType, formatType),
                        left: activeObject.left,
                        top: activeObject.top,
                        cornerSize: 7,
                        padding: 4,
                        transparentCorners: false,
                        hasRotatingPoint: false,
                        centeredScaling: true,
                        borderScaleFactor: 0.35,
                        borderOpacityWhenMoving: 1,
                        borderColor: editorCanvas.commonBorderColor,
                        cornerColor: editorCanvas.commonCornerColor
                    });

                    editorCanvas.canvas.remove(activeObject);
                    editorCanvas.addControlEvents(bulletGroup);
                    editorCanvas.canvas.add(bulletGroup).setActiveObject(bulletGroup);
                    editorCanvas.canvas.renderAll();
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    removeBulletList: function (textAttribute) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) {
                    var textObject = Enumerable.From(activeObject._objects)
                        .FirstOrDefault(function (x) { return x.type == editorCanvas.objectType.IText || x.type == editorCanvas.objectType.Text });

                    if (textObject) {
                        textObject.set({
                            left: activeObject.left,
                            top: activeObject.top,
                            hasControls: true,
                            cornerSize: 7,
                            padding: 4,
                            transparentCorners: false,
                            hasRotatingPoint: false,
                            centeredScaling: true,
                            borderScaleFactor: 0.35,
                            borderOpacityWhenMoving: 1,
                            borderColor: editorCanvas.commonBorderColor,
                            cornerColor: editorCanvas.commonCornerColor
                        });

                        if (textAttribute) {
                            if (typeof textAttribute.text !== 'undefined') {
                                textObject.set({ text: textAttribute.text });
                            }
                            if (typeof textAttribute.fontFamily !== 'undefined') {
                                textObject.set({ fontFamily: textAttribute.fontFamily });
                            }
                            if (typeof textAttribute.fontSize !== 'undefined') {
                                textObject.set({ fontSize: textAttribute.fontSize });
                            }
                            if (typeof textAttribute.textAlign !== 'undefined') {
                                textObject.set({ textAlign: textAttribute.textAlign });
                            }
                            if (typeof textAttribute.fontWeight !== 'undefined') {
                                textObject.set({ fontWeight: textAttribute.fontWeight });
                            }
                            if (typeof textAttribute.fontStyle !== 'undefined') {
                                textObject.set({ fontStyle: textAttribute.fontStyle });
                            }
                            if (typeof textAttribute.fill !== 'undefined') {
                                textObject.set({ fill: textAttribute.fill });
                            }
                            if (typeof textAttribute.backgroundColor !== 'undefined') {
                                textObject.set({ backgroundColor: textAttribute.backgroundColor });
                            }
                        }

                        editorCanvas.canvas.remove(activeObject);
                        editorCanvas.addControlEvents(textObject);
                        editorCanvas.canvas.add(textObject).setActiveObject(textObject);
                        editorCanvas.canvas.renderAll();
                    }
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    getBulletType: function (retyrnType) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) {
                    if (retyrnType == 'bulletType')
                        return activeObject.name.split('_')[2];
                    else if (retyrnType == 'bulletFormatType')
                        return activeObject.name.split('_')[4];
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    getBulletTextObject: function (activeObject) {
        try {
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group) {
                    var textObject = Enumerable.From(activeObject._objects)
                        .FirstOrDefault(function (x) { return x.type == editorCanvas.objectType.IText || x.type == editorCanvas.objectType.Text });

                    return textObject;
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    formatBullatedText: function (formatObject) {
        try {
            var bulletType = editorCanvas.getBulletType('bulletType');
            var bulletFormatType = editorCanvas.getBulletType('bulletFormatType');

            editorCanvas.removeBulletList(formatObject);
            editorCanvas.addBulletList(bulletType, bulletFormatType);

        } catch (ex) {
            console.log(ex);
        }
    },

    addControlEvents: function (activeObject) {
        try {
            if (activeObject) {
                if (activeObject.name && !activeObject.name.startsWith(editorCanvas.gridLinePrefix)) {
                    activeObject.on('selected', function (options) {
                        $('.hove-highlighter').hide();
                        editorCanvas.hideAllPopupCommand();
                        editorCanvas.setToolbarCommand(editorCanvas.canvas.getActiveObject());
                        editorCanvas.positionBtn(editorCanvas.canvas.getActiveObject());
                        editorCanvas.showHidePopupCommand(editorCanvas.canvas.getActiveObject(), 'show');
                    });

                    activeObject.on('moving', function (e) {
                        editorCanvas.positionBtn(editorCanvas.canvas.getActiveObject());
                        $('.hove-highlighter').hide();
                    });

                    activeObject.on('scaling', function (e) {
                        editorCanvas.positionBtn(editorCanvas.canvas.getActiveObject());
                        $('.hove-highlighter').hide();
                    });

                    activeObject.on('rotating', function (e) {
                        editorCanvas.positionBtn(editorCanvas.canvas.getActiveObject());
                    });

                    if (activeObject.type == editorCanvas.objectType.IText) {
                        activeObject.on('editing:entered', function (e) {
                            $('#textbox_tools').hide();
                            setAllToobarCommandDisabled();
                            editorCanvas.setToolbarCommand(activeObject);
                        });
                    }

                    editorCanvas.canvas.observe('object:scaling', function (e) {
                        var activeObject = editorCanvas.canvas.getActiveObject();
                        editorCanvas.positionBtn(activeObject);
                    });
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    positionHoverIndicator: function (hoverObject) {
        try {
            if (hoverObject) {
                if (hoverObject.name && hoverObject.name.startsWith(editorCanvas.gridLinePrefix))
                    return;

                var $indicatorPopup = $('.hove-highlighter');
                var indicatorPopupWidth = $indicatorPopup.width();
                var indicatorPopupHeight = $indicatorPopup.height();
                var absCoords = editorCanvas.canvas.getAbsoluteCoords(hoverObject);

                $indicatorPopup.attr('title', '');
                if ((hoverObject.type == editorCanvas.objectType.Group && hoverObject.name.contains('BulletPoint_Group')) ||
                    (hoverObject.type == editorCanvas.objectType.IText || hoverObject.type == editorCanvas.objectType.Text)) {
                    $indicatorPopup.css({
                        'left': (absCoords.left + 25).toString() + 'px',
                        'top': (absCoords.top - 5).toString() + 'px',
                        'width': (hoverObject.oCoords.br.x - hoverObject.oCoords.bl.x) + 'px',
                        'height': (hoverObject.oCoords.bl.y - hoverObject.oCoords.tl.y) + 'px'
                    });
                    $indicatorPopup.attr('title', 'Click here to edit address, phone #, web address, e-mail');
                    $indicatorPopup.find('.hover-descriptor').text('Anything left outside the red dotted lines will be cut off during trimming');
                }
                else if (hoverObject.type == editorCanvas.objectType.Circle || hoverObject.type == editorCanvas.objectType.Rect
                    || hoverObject.type == editorCanvas.objectType.Image) {
                    $indicatorPopup.css({
                        'left': (absCoords.left + 25).toString() + 'px',
                        'top': (absCoords.top - 5).toString() + 'px',
                        'width': (hoverObject.oCoords.br.x - hoverObject.oCoords.bl.x) + 'px',
                        'height': (hoverObject.oCoords.bl.y - hoverObject.oCoords.tl.y) + 'px'
                    });
                    if (hoverObject.type == editorCanvas.objectType.Image) {
                        $indicatorPopup.find('.hover-descriptor').text('Image size not to exceed 300 pixels');
                        $indicatorPopup.attr('title', 'Click here to edit and insert jpeg of your logo');
                    }
                }
                else if (hoverObject.type == editorCanvas.objectType.Line) {
                    $indicatorPopup.css({
                        'left': (absCoords.left + 25).toString() + 'px',
                        'top': (absCoords.top - 5).toString() + 'px',
                        'width': ((hoverObject.oCoords.br.x - hoverObject.oCoords.bl.x)) + 'px',
                        'height': ((hoverObject.oCoords.bl.y - hoverObject.oCoords.tl.y) - 3) + 'px'
                    });
                    $indicatorPopup.find('.hover-descriptor').empty();
                }
                $indicatorPopup.data('current-object', hoverObject.name);
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    positionBtn: function (activeObject) {
        try {
            if (activeObject) {
                var $commandPopup;
                var absCoords = editorCanvas.canvas.getAbsoluteCoords(activeObject);

                if ((activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) ||
                    (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text)) {

                    $commandPopup = $('#textbox_tools');
                    var commandPopupWidth = $commandPopup.width();
                    var commandPopupHeight = $commandPopup.height();

                    if (activeObject.left > (commandPopupWidth + 30)) {
                        $commandPopup.css({
                            'left': ((absCoords.left - commandPopupWidth) - 2).toString() + 'px',
                            'top': (absCoords.top - 6).toString() + 'px'
                        });
                    }
                    else {
                        $commandPopup.css({
                            'left': (absCoords.left + (activeObject.width * activeObject.scaleX) + 42) + 'px',
                            'top': (absCoords.top - 6).toString() + 'px'
                        });
                    }
                }
                else if (activeObject.type == editorCanvas.objectType.Circle ||
					activeObject.type == editorCanvas.objectType.Rect ||
					activeObject.type == editorCanvas.objectType.Line) {

                    $commandPopup = $('#shape_toolbar');
                    var commandPopupWidth = $commandPopup.width();
                    var commandPopupHeight = $commandPopup.height();

                    if (activeObject.top > (commandPopupHeight + 30)) {
                        $commandPopup.css({
                            'left': (absCoords.left).toString() + 'px',
                            'top': (absCoords.top - (commandPopupHeight + 15)).toString() + 'px'
                        });
                    }
                    else {
                        $commandPopup.css({
                            'left': (absCoords.left).toString() + 'px',
                            'top': (absCoords.top + (activeObject.height * activeObject.scaleY) + 10).toString() + 'px'
                        });
                    }
                }
                else if (activeObject.type == editorCanvas.objectType.Image) {
                    $commandPopup = $('#crop_toolbar');
                    var commandPopupWidth = $commandPopup.width();
                    var commandPopupHeight = $commandPopup.height();

                    if (activeObject.top > (commandPopupHeight + 30)) {
                        $commandPopup.css({
                            'left': (absCoords.left + 22).toString() + 'px',
                            'top': (absCoords.top - (commandPopupHeight + 15)).toString() + 'px'
                        });
                    }
                    else {
                        $commandPopup.css({
                            'left': (absCoords.left + 22).toString() + 'px',
                            'top': (absCoords.top + (activeObject.height * activeObject.scaleY) + 10).toString() + 'px'
                        });
                    }
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    onFileLoad: function (event) {
        var $img = $('<img>', { src: event });

        $img.load(function () {
            var image = new fabric.Cropzoomimage(this, {
                name: 'Image_' + new Date().getTime().toString(),
                left: 300,
                top: $(window).scrollTop() == 0 ? 100 : $(window).scrollTop(),
                cornerSize: 7,
                padding: 4,
                transparentCorners: false,
                hasRotatingPoint: false,
                centeredScaling: true,
                borderScaleFactor: 0.35,
                borderOpacityWhenMoving: 1,
                borderColor: editorCanvas.commonBorderColor,
                cornerColor: editorCanvas.commonCornerColor
            });

            editorCanvas.addControlEvents(image);
            editorCanvas.canvas.add(image);
        });
    },

    replaceImage: function (imageUrl) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Image) {
                    var $img = $('<img>', { src: imageUrl });
                    try { $.blockUI({ message: $("#dataloading") }); } catch (ex) { }

                    $img.load(function () {
                        var $self = $(this);

                        var image = new fabric.Cropzoomimage(this, {
                            name: activeObject.name,
                            left: activeObject.left,
                            top: activeObject.top,
                            cornerSize: 7,
                            padding: 4,
                            transparentCorners: false,
                            hasRotatingPoint: false,
                            centeredScaling: true,
                            borderScaleFactor: 0.35,
                            borderOpacityWhenMoving: 1,
                            borderColor: editorCanvas.commonBorderColor,
                            cornerColor: editorCanvas.commonCornerColor
                        });

                        editorCanvas.canvas.remove(activeObject);
                        editorCanvas.addControlEvents(image);
                        editorCanvas.canvas.add(image).setActiveObject(image);

                        editorCanvas.canvas.fire('object:moving', { target: image });
                        image.setCoords();
                        editorCanvas.canvas.renderAll();
                        editorCanvas.positionBtn(image);

                        try { $.unblockUI(); } catch (ex) { }
                    });
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    insertText: function (text, fontFamily, fontSize) {
        try {
            var textObject = new fabric.Text(text, {
                name: 'Text_' + new Date().getTime().toString(),
                fontFamily: fontFamily,
                textAlign: 'left',
                fill: '#00000',
                fontSize: fontSize,
                editingBorderColor: '#83b61a'
            });
            editorCanvas.action = true;
            editorCanvas.commonObjectSettings(textObject);
            editorCanvas.addControlEvents(textObject);
            editorCanvas.canvas.add(textObject).setActiveObject(textObject);
            editorCanvas.canvas.renderAll();

        } catch (ex) {
            console.ex;
        }
    },

    insertShape: function (type) {
        try {
            var shapeObj;

            switch (type) {
                case editorCanvas.objectType.Rect:
                    shapeObj = new fabric.Rect({
                        name: 'Rect_' + new Date().getTime().toString(),
                        fill: '#fdf6bb',
                        width: 150,
                        height: 150
                    });
                    break;

                case editorCanvas.objectType.Circle:
                    shapeObj = new fabric.Circle({
                        name: 'Circle_' + new Date().getTime().toString(),
                        radius: 72,
                        fill: '#fdf6bb',
                        width: 150,
                        height: 150
                    });
                    break;

                case 'H-Line':
                    shapeObj = new fabric.Line([50, 100, 500, 100], {
                        name: 'H-Line_' + new Date().getTime().toString(),
                        stroke: '#c1c1c1',
                        strokeWidth: 3,
                        padding: 10,
                        originX: 'center',
                        originY: 'center'
                    });
                    break;

                case 'V-Line':
                    shapeObj = new fabric.Line([100, 50, 100, 500], {
                        name: 'V-Line_' + new Date().getTime().toString(),
                        stroke: '#c1c1c1',
                        strokeWidth: 3,
                        padding: 10,
                        originX: 'center',
                        originY: 'center'
                    });
                    break;
            }

            editorCanvas.commonObjectSettings(shapeObj);
            editorCanvas.addControlEvents(shapeObj);
            editorCanvas.canvas.add(shapeObj).setActiveObject(shapeObj);
            editorCanvas.canvas.renderAll();

        } catch (ex) {
            console.log(ex);
        }
    },

    commonObjectSettings: function (activeObject) {
        try {
            if (activeObject) {
                activeObject.set({
                    left: 300,
                    top: $(window).scrollTop() == 0 ? 100 : $(window).scrollTop(),
                    cornerSize: 7,
                    padding: 4,
                    transparentCorners: false,
                    hasRotatingPoint: false,
                    centeredScaling: true,
                    borderScaleFactor: 0.35,
                    borderOpacityWhenMoving: 1,
                    borderColor: editorCanvas.commonBorderColor,
                    cornerColor: editorCanvas.commonCornerColor
                });
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    editText: function (str) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) {
                    editorCanvas.formatBullatedText({ text: str });
                }
                else if (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text) {
                    activeObject.set({ text: str });
                }
                editorCanvas.objectRepostioning();
                editorCanvas.canvas.renderAll();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    setTextFontFamily: function (fontfamily) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) {
                    editorCanvas.formatBullatedText({ fontFamily: fontfamily });
                }
                else if (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text) {
                    editorCanvas.setActiveStyle('fontFamily', fontfamily, activeObject);
                }
                editorCanvas.canvas.renderAll();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    setTextFontSize: function (fontsize) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) {
                    editorCanvas.formatBullatedText({ fontSize: fontsize });
                }
                else if (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text) {
                    editorCanvas.setActiveStyle('fontSize', fontsize, activeObject);
                }
                editorCanvas.canvas.renderAll();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    setTextFormat: function (formatOption, status) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) {
                    if (formatOption == 'left' || formatOption == 'center' || formatOption == 'right') {
                        editorCanvas.formatBullatedText({ textAlign: formatOption });
                    }
                    else if (formatOption == 'bold') {
                        editorCanvas.formatBullatedText({ fontWeight: status == true ? formatOption : 'normal' });
                    }
                    else if (formatOption == 'italic') {
                        editorCanvas.formatBullatedText({ fontStyle: status == true ? formatOption : 'normal' });
                    }
                }
                else if (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text) {
                    if (formatOption == 'left' || formatOption == 'center' || formatOption == 'right') {
                        editorCanvas.setActiveStyle('textAlign', formatOption, activeObject);
                    }
                    else if (formatOption == 'bold') {
                        editorCanvas.setActiveStyle('fontWeight', (status == true ? formatOption : 'normal'), activeObject);
                    }
                    else if (formatOption == 'italic') {
                        editorCanvas.setActiveStyle('fontStyle', (status == true ? formatOption : 'normal'), activeObject);
                    }
                }
                editorCanvas.canvas.renderAll();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    setForegroundColor: function (colorcode) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) {
                    editorCanvas.formatBullatedText({ fill: '#' + colorcode });
                }
                else if (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text) {
                    editorCanvas.setActiveStyle('fill', '#{0}'.format(colorcode), activeObject);
                }
                else if (activeObject.type == editorCanvas.objectType.Circle || activeObject.type == editorCanvas.objectType.Rect) {
                    activeObject.set({ fill: '#' + colorcode });
                }
                editorCanvas.canvas.renderAll();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    setBackgroundColor: function (colorcode) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) {
                    editorCanvas.formatBullatedText({ backgroundColor: '#' + colorcode });
                }
                else if (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text) {
                    editorCanvas.setActiveStyle('textBackgroundColor', '#{0}'.format(colorcode), activeObject);
                }
                editorCanvas.canvas.renderAll();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    setStrokeColor: function (colorcode) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) {
                    editorCanvas.formatBullatedText({ stroke: '#' + colorcode });
                }
                else if (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text) {
                    editorCanvas.setActiveStyle('stroke', '#{0}'.format(colorcode), activeObject);
                }
                else if (activeObject.type == editorCanvas.objectType.Circle || activeObject.type == editorCanvas.objectType.Rect || activeObject.type == editorCanvas.objectType.Line) {
                    activeObject.set({ stroke: '#' + colorcode });
                }
                editorCanvas.canvas.renderAll();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    setShapeStrokeWidth: function (size) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Circle ||
				activeObject.type == editorCanvas.objectType.Rect ||
				activeObject.type == editorCanvas.objectType.Line) {

                    activeObject.set({ strokeWidth: size == '0' ? null : size });
                    editorCanvas.canvas.renderAll();
                    editorCanvas.canvas.calcOffset();
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    setStrokeDashArray: function (arrayIndex) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                if (activeObject.type == editorCanvas.objectType.Circle ||
				activeObject.type == editorCanvas.objectType.Rect ||
				activeObject.type == editorCanvas.objectType.Line) {

                    activeObject.set({ strokeDashArray: editorCanvas.stokeArrayCollection[arrayIndex] });
                    editorCanvas.canvas.renderAll();
                    editorCanvas.canvas.calcOffset();
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    setToolbarCommand: function (activeObject) {
        try {
            if (activeObject) {

                //Remove all active class from all toobar command
                $('.on').removeClass('on');
                $('#btn_symbol_bullet_none, #btn_text_number_none').addClass('on');

                if (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text ||
                    (activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group'))) {

                    if (activeObject.type == editorCanvas.objectType.Group) {
                        $('#btn_symbol_bullet .on, #btn_text_number .on').removeClass('on');
                        var bulletType = editorCanvas.getBulletType('bulletType');
                        var bulletFormatType = editorCanvas.getBulletType('bulletFormatType');

                        if (bulletType === 'SymbolBulletPoint') {
                            $('[data-symbol-bullet-format-type="' + bulletFormatType + '"]').addClass('on');
                        }
                        else if (bulletType === 'NumberBulletPoint') {
                            $('[data-text-bullet-format-type="' + bulletFormatType + '"]').addClass('on');
                        }

                        activeObject = editorCanvas.getBulletTextObject(activeObject);
                    }

                    //---Text format section
                    $('#btn_text_font_face').removeClass('disabled');
                    $('#btn_text_font_size').removeClass('disabled');
                    $('#btn_text_font_color').removeClass('disabled');

                    $('#btn_text_bold').removeClass('disabled');
                    $('#btn_text_italic').removeClass('disabled');

                    if ((activeObject.type == editorCanvas.objectType.IText && !activeObject.isEditing) ||
                        activeObject.type == editorCanvas.objectType.Text) {
                        $('#btn_text_gravity_west').removeClass('disabled');
                        $('#btn_text_gravity_center').removeClass('disabled');
                        $('#btn_text_gravity_east').removeClass('disabled');
                        $('#btn_symbol_bullet').removeClass('disabled');
                        $('#btn_text_number').removeClass('disabled');

                        switch (activeObject.textAlign) {
                            case 'left':
                                $('#btn_text_gravity_west').addClass('on');
                                break;

                            case 'center':
                                $('#btn_text_gravity_center').addClass('on');
                                break;

                            case 'right':
                                $('#btn_text_gravity_east').addClass('on');
                                break;
                        }
                    }

                    $('#btn_bacground_color').removeClass('disabled');
                    $('#inline_textarea').val(activeObject.text);
                    $('#btn_text_bold').addClass(activeObject.fontWeight === 'bold' ? 'on' : '');
                    $('#btn_text_italic').addClass(activeObject.fontStyle ? 'on' : '');

                    //Set fonts on selected text
                    $('#fontSelector').fontSelector('select', activeObject.fontFamily);
                    $('#fontSizeSelector').fontSelector('select', activeObject.fontSize);

                    $('#fcolor').css('background-color', activeObject.originalState.fill);
                    if (activeObject.originalState.backgroundColor !== '')
                        $('#bgcolor').css('backgroundColor', activeObject.originalState.backgroundColor);
                }

                else if (activeObject.type == editorCanvas.objectType.Circle || activeObject.type == editorCanvas.objectType.Rect) {
                    $('#shape_toolbar #fillcolor').closest('.toolbar-button').show();
                    $('#shape_toolbar #fillcolor').css('backgroundColor', activeObject.originalState.fill);
                    $('#shape_toolbar #bordercolor').css('backgroundColor', activeObject.stroke == null ? '#ffffff' : activeObject.stroke);
                    $('#shape_toolbar #weightSelector').fontSelector('select', activeObject.strokeWidth == null ? '0' : activeObject.strokeWidth);
                }
                else if (activeObject.type == editorCanvas.objectType.Line) {
                    $('#shape_toolbar #fillcolor').closest('.toolbar-button').hide();
                    $('#shape_toolbar #bordercolor').css('backgroundColor', activeObject.stroke == null ? '#ffffff' : activeObject.stroke);
                    $('#shape_toolbar #weightSelector').fontSelector('select', activeObject.strokeWidth == null ? '0' : activeObject.strokeWidth);
                }
                else if (activeObject.type == editorCanvas.objectType.Image) {
                    $('#btn_advance_rotate').removeClass('disabled');
                    $('#btn_advance_crop').removeClass('disabled');
                }

                $('#btn_edit_lock').removeClass('disabled');
                $('#btn_edit_delete').removeClass('disabled');
                $('#btn_edit_copy').removeClass('disabled');

                $('#btn_advance_align').removeClass('disabled');
                $('#btn_advance_layers').removeClass('disabled');
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    flipHorizontal: function () {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) { activeObject.toggle("flipX"); }
            editorCanvas.canvas.renderAll();
        } catch (ex) {
            console.log(ex);
        }
    },

    flipVertical: function () {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) { activeObject.toggle("flipY"); }
            editorCanvas.canvas.renderAll();
        } catch (ex) {
            console.log(ex);
        }
    },

    addStamp: function (e) {
        fabric.Image.fromURL(e.src, function (img) {
            img.MIN_SCALE_LIMIT = 0.01;
            editorCanvas.canvas.add(img.set({
                left: 525 - fabric.util.getRandomInt(0, 200),
                top: 178 - fabric.util.getRandomInt(0, 40),
                angle: fabric.util.getRandomInt(5, 25)
            }).scale(0.25));

            editorCanvas.canvas.calcOffset();
            editorCanvas.canvas.renderAll();
        });
    },

    hideAllPopupCommand: function () {
        try {
            $('#textbox_tools textarea').val('');
            $('#textbox_tools, #crop_toolbar, #shape_toolbar').hide();
        } catch (ex) {
            console.log(ex);
        }
    },

    showHidePopupCommand: function (activeObject, status) {
        try {
            if (activeObject) {
                var $commandPopup = null;
                if ((activeObject.type == editorCanvas.objectType.Group && activeObject.name.contains('BulletPoint_Group')) ||
                    (activeObject.type == editorCanvas.objectType.IText || activeObject.type == editorCanvas.objectType.Text)) {

                    if ((activeObject.type == editorCanvas.objectType.IText && !activeObject.isEditing) ||
                    activeObject.type != editorCanvas.objectType.IText)
                        $commandPopup = $('#textbox_tools');
                }
                else if (activeObject.type == editorCanvas.objectType.Circle ||
					activeObject.type == editorCanvas.objectType.Rect ||
					activeObject.type == editorCanvas.objectType.Line) {
                    $commandPopup = $('#shape_toolbar');
                }
                else if (activeObject.type == editorCanvas.objectType.Image) {
                    $commandPopup = $('#crop_toolbar');
                }

                if (status === 'show') {
                    if ($commandPopup != null) {
                        $commandPopup.show();
                    }
                }
                else if (status === 'hide') {
                    if ($commandPopup != null) {
                        $commandPopup.filter('textarea').val('');
                        $commandPopup.hide();
                    }
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    copyObject: function () {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                var activeCopiedObject = fabric.util.object.clone(activeObject);
                editorCanvas.canvas.discardActiveObject();
                activeCopiedObject = editorCanvas.generateObjectNameOnClone(activeCopiedObject);

                activeCopiedObject.set({
                    top: (activeCopiedObject.top + 20),
                    left: (activeCopiedObject.left + 20)
                });
                editorCanvas.copiedObject = activeCopiedObject;
                $('#btn_edit_paste').removeClass('disabled');
                CommandAction();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    pasteObject: function () {
        try {
            if (editorCanvas.copiedObject) {
                editorCanvas.canvas.discardActiveObject();
                var newClonedInstance = fabric.util.object.clone(editorCanvas.copiedObject)
                newClonedInstance = editorCanvas.generateObjectNameOnClone(newClonedInstance);

                editorCanvas.commonObjectSettings(newClonedInstance);
                editorCanvas.addControlEvents(newClonedInstance);
                editorCanvas.canvas.add(newClonedInstance).setActiveObject(newClonedInstance);
                editorCanvas.canvas.renderAll();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    lockUnlockObject: function (status) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            if (activeObject) {
                activeObject.selectable = (!status);
                //activeObject.hasBorders = (!status);
                activeObject.hasControls = (!status);
                activeObject.perPixelTargetFind = (!status);

                activeObject.lockMovementX = (status);
                activeObject.lockMovementY = (status);
                activeObject.lockScaling = (status);

                editorCanvas.canvas.setActiveObject(activeObject);

                editorCanvas.canvas.renderAll();
                editorCanvas.canvas.calcOffset();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    generateObjectNameOnClone: function (activeObject) {
        try {
            var currentTimeStamp = new Date().getTime().toString();
            if (activeObject.type === editorCanvas.objectType.Group) {
                $.each(activeObject._objects, function (index, element) {
                    var childObjName = element.name.split('_');
                    if (childObjName.length > 0) {
                        childObjName[1] = currentTimeStamp;
                        element.name = childObjName.join('_');
                    }
                });
            }
            var objName = activeObject.name.split('_');
            if (objName.length > 0) {
                objName[1] = currentTimeStamp;
                activeObject.name = objName.join('_');
            }

            return activeObject;

        } catch (ex) {
            console.log(ex);
        }
    },

    setPosition: function (positionType) {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            var setPos = false, setLayer = false;
            if (activeObject) {
                switch (positionType) {
                    case 'front':
                        activeObject.bringToFront();
                        editorCanvas.canvas.discardActiveObject();
                        break;

                    case 'back':
                        activeObject.sendToBack();
                        editorCanvas.canvas.discardActiveObject();
                        break;

                    case 'forword':
                        activeObject.bringForward();
                        editorCanvas.canvas.discardActiveObject();
                        break;

                    case 'backward':
                        activeObject.sendBackwards();
                        editorCanvas.canvas.discardActiveObject();
                        break;

                    case 'alignleft':
                        activeObject.left = 10; setPos = true;
                        break;

                    case 'aligncenter':
                        activeObject.left = ((activeObject.canvas.getWidth() / 2) - (activeObject.currentWidth / 2));
                        setPos = true;
                        break;

                    case 'alignright':
                        activeObject.left = (activeObject.canvas.getWidth() - (activeObject.currentWidth + 30));
                        setPos = true;
                        break;

                    case 'aligntop':
                        activeObject.top = 10; setPos = true;
                        setPos = true;
                        break;

                    case 'alignmiddle':
                        activeObject.top = ((activeObject.canvas.getHeight() / 2) - (activeObject.currentHeight / 2));
                        setPos = true;
                        break;

                    case 'alignbottom':
                        activeObject.top = (activeObject.canvas.getHeight() - (activeObject.currentHeight + 30));
                        setPos = true;
                        break;

                    case 'distributevertically':
                    case 'distributehorizontally':
                        break;
                }

                if (setPos) {
                    editorCanvas.canvas.renderAll();
                    activeObject.setCoords();
                    editorCanvas.canvas.setActiveObject(activeObject);
                    editorCanvas.positionBtn(activeObject);
                }
            }

        } catch (ex) {
            console.log(ex);
        }
    },

    setActiveStyle: function (styleName, value, object) {
        try {
            var style = {};
            style[styleName] = value;

            if (object.setSelectionStyles && object.isEditing) {
                object.setSelectionStyles(style);
            }
            else {
                object.set(style);
            }

            object.setCoords();
            editorCanvas.canvas.renderAll();
        } catch (ex) {
            console.log(ex);
        }
    },

    undo: function () {
        try {
            editorCanvas.showHideUndoRedo();

            if (editorCanvas.index < 1) {
                editorCanvas.index = 1;
                return;
            }

            --editorCanvas.index;
            editorCanvas.loadCanvasFromTempStack(editorCanvas.index);
            editorCanvas.action = false;
        } catch (ex) {
            console.log(ex);
        }
    },

    redo: function () {
        try {
            editorCanvas.action = false;
            editorCanvas.showHideUndoRedo();

            if (editorCanvas.index >= editorCanvas.state.length - 1) {
                editorCanvas.loadCanvasFromTempStack(editorCanvas.index);
                return;
            }

            ++editorCanvas.index;
            editorCanvas.loadCanvasFromTempStack(editorCanvas.index);
        } catch (ex) {
            console.log(ex);
        }
    },

    showHideUndoRedo: function () {
        try {
            if (editorCanvas.state.length > editorCanvas.index) {
                $('#btn_edit_redo').removeClass('disabled');
            }
            else {
                $('#btn_edit_redo').addClass('disabled');
            }

            if (editorCanvas.index < 1) {
                $('#btn_edit_undo').addClass('disabled');
            }
            else {
                $('#btn_edit_undo').removeClass('disabled');
            }
            CommandAction();
        } catch (ex) {
            console.log(ex);
        }
    },

    loadCanvasFromTempStack: function (stackIndex) {
        try {
            editorCanvas.loadCanvasFromJsonData(editorCanvas.state[stackIndex], true, false);
            setTimeout(function () { editorCanvas.createBoundryLimit() }, 500);
        } catch (ex) {
            console.log(ex);
        }
    },

    createBoundryLimit: function () {
        try {
            var boundingBox = editorCanvas.canvas.getItemByName('bounding-box');
            if (boundingBox != null && boundingBox != undefined) {
                boundingBox.bringToFront();
            }
            else {
                boundingBox = new fabric.Rect({
                    name: 'bounding-box',
                    fill: 'transparent',
                    top: editorCanvas.globalBoundryLimit,
                    left: editorCanvas.globalBoundryLimit,
                    width: editorCanvas.canvas.getWidth() - (editorCanvas.globalBoundryLimit * 2),
                    height: editorCanvas.canvas.getHeight() - (editorCanvas.globalBoundryLimit * 2),
                    hasBorders: false,
                    hasControls: false,
                    lockMovementX: true,
                    lockMovementY: true,
                    evented: false,
                    stroke: '#fd0000',
                    strokeDashArray: editorCanvas.stokeArrayCollection[2]
                });

                editorCanvas.canvas.add(boundingBox);
            }

            editorCanvas.createGridLines();
            editorCanvas.canvas.renderAll();

        } catch (ex) {
            console.log(ex);
        }
    },

    destroyBoundryLimit: function () {
        try {
            var boundingBox = editorCanvas.canvas.getItemByName('bounding-box');
            if (boundingBox != null && boundingBox != undefined) {
                editorCanvas.canvas.remove(boundingBox);

                editorCanvas.destroyGridLine();
                editorCanvas.canvas.renderAll();
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    init: function (config) {
        try {
            editorCanvas.canvas = new fabric.Canvas(config.name, {
                isDrawingMode: false,
                selection: false
            });

            editorCanvas.canvasLoad(config, function (option) {
                if (editorCanvas.scaleDownExecution == 0) {
                    editorCanvas.canvasScaleDown({
                        scaleValue: config.scaleValue,
                        orgWidth: config.width,
                        orgHeight: config.height,
                        IsCanvasOnly: option.IsCanvasOnly
                    });
                    editorCanvas.scaleDownExecution += 1;
                    editorCanvas.createRuler();
                }
                editorCanvas.createBoundryLimit();

                editorCanvas.canvasEventHandlerSetup();
                editorCanvas.state[0] = editorCanvas.getJsonDataFromCanvas(true);
            });

        } catch (ex) {
            console.log(ex);
        }
    },

    canvasEventHandlerSetup: function () {
        try {
            editorCanvas.canvas.on('selection:cleared', function (options) {
                editorCanvas.hideAllPopupCommand();
            });

            editorCanvas.canvas.on("after:render", function () {
                editorCanvas.canvas.calcOffset();
            });

            editorCanvas.canvas.on('mouse:down', function (e) {
                try {
                    //editorCanvas.showCrosshair(e);
                } catch (ex) {
                    console.log(ex);
                }
            });

            editorCanvas.canvas.on('mouse:up', function (e) {
                try {
                    if (editorCanvas.showGrid) {
                        //editorCanvas.hideCrosshair(e);
                    }
                } catch (ex) {
                    console.log(ex);
                }
            });

            editorCanvas.canvas.on('mouse:over', function (e) {
                try {
                    var hoverObject = e.target;
                    var activeObject = editorCanvas.canvas.getActiveObject();

                    if (activeObject == null || hoverObject.name !== activeObject.name) {

                        if (hoverObject.name && !hoverObject.name.startsWith(editorCanvas.gridLinePrefix)) {
                            editorCanvas.positionHoverIndicator(hoverObject);
                            $('.hove-highlighter').show();
                            if (hoverObject.type == editorCanvas.objectType.Image ||
                                hoverObject.type == editorCanvas.objectType.IText ||
                                hoverObject.type == editorCanvas.objectType.Text) {
                                $('.hover-descriptor').css({ 'top': ($('.hove-highlighter').height() + 5) + 'px' })
                                $('.hover-descriptor').slideDown(200);
                            }
                            else { $('.hover-descriptor').hide(); }
                        }
                    }

                    editorCanvas.windowScrollTop = $(window).scrollTop();
                } catch (ex) {
                    console.log(ex);
                }
            });

            editorCanvas.canvas.on('mouse:out', function (e) {
                try {
                    $('.hove-highlighter, .hover-descriptor').hide();
                } catch (ex) {
                    console.log(ex);
                }
            });

            editorCanvas.canvas.on("object:added", function (e) {
                try {
                    if (editorCanvas.action === true) {
                        var object = e.target;
                        editorCanvas.state[++editorCanvas.index] = editorCanvas.getJsonDataFromCanvas(true);
                        editorCanvas.refresh = true;
                        editorCanvas.action = false;
                        editorCanvas.canvas.renderAll();
                    }
                } catch (ex) {
                    console.log(ex);
                }
            });

            editorCanvas.canvas.on("object:modified", function (e) {
                try {
                    var activeObject = e.target;
                    editorCanvas.state[++editorCanvas.index] = editorCanvas.getJsonDataFromCanvas(true);
                    editorCanvas.action = false;

                    //editorCanvas.limitBoundry(activeObject);
                    activeObject.setCoords();
                    editorCanvas.canvas.renderAll();
                    editorCanvas.hideCrosshair(e);
                } catch (ex) {
                    console.log(ex);
                }
            });

            editorCanvas.canvas.on('object:selected', function (e) {
                try {
                    var activeObject = e.target;
                    if (activeObject != null && activeObject != undefined) {
                        editorCanvas.currentScaleX = activeObject.get('scaleX');
                        editorCanvas.currentScaleY = activeObject.get('scaleY');
                    }
                } catch (ex) {
                    console.log(ex);
                }
            });

            editorCanvas.canvas.on('object:scaling', function (e) {
                try {
                    var activeObject = e.target;
                    if (editorCanvas.currentScaleX == activeObject.get('scaleX')
                        && editorCanvas.currentScaleY != activeObject.get('scaleY')) {
                        activeObject.scale(activeObject.get('scaleY'));
                    }
                    else if (editorCanvas.currentScaleX != activeObject.get('scaleX')
                        && editorCanvas.currentScaleY == activeObject.get('scaleY')) {
                        activeObject.scale(activeObject.get('scaleX'));
                    }
                    editorCanvas.currentScaleX = activeObject.get('scaleX');
                    editorCanvas.currentScaleY = activeObject.get('scaleY');

                    editorCanvas.snapObjectOnScaling(e);
                } catch (ex) {
                    console.log(ex);
                }
            });

            editorCanvas.canvas.on('object:moving', function (e) {
                try {
                    var activeObject = (e.target == undefined ? editorCanvas.canvas.getActiveObject() : e.target);
                    if (activeObject.type != editorCanvas.objectType.IText ||
                        activeObject.type != editorCanvas.objectType.Text ||
                        activeObject.type != editorCanvas.objectType.Line) {

                        editorCanvas.snapObjectOnMoving(e);
                        editorCanvas.limitBoundry(activeObject);
                        editorCanvas.showCrosshair(e);
                    }
                } catch (ex) {
                    console.log(ex);
                }
            });

            editorCanvas.canvas.on('text:editing:exited', function (e) {
                try {
                    editorCanvas.state[++editorCanvas.index] = editorCanvas.getJsonDataFromCanvas(true);
                    editorCanvas.action = false;
                } catch (ex) {
                    console.log(ex);
                }
            });

            editorCanvas.canvas.on('text:changed', function (e) {
                try {
                    var activeObject = (e.target == undefined ? editorCanvas.canvas.getActiveObject() : e.target);
                    editorCanvas.objectRepostioning();
                } catch (ex) {
                    console.log(ex);
                }
            });
        } catch (ex) {
            console.log(ex);
        }
    },

    canvasLoad: function (config, completeCallbackHandler) {
        try {
            if (config.renderData) {
                if (config.renderData != '') {
                    editorCanvas.loadCanvasFromJsonData(config.renderData, true, true, completeCallbackHandler);
                }
            }
            else {
                editorCanvas.canvas.setBackgroundImage(config.backgroundSrc, function () {
                    if (completeCallbackHandler != null && completeCallbackHandler != undefined) {
                        completeCallbackHandler({ IsCanvasOnly: false });
                    }
                    editorCanvas.canvas.renderAll();
                });

                editorCanvas.canvasEventHandlerSetup();
            }
            editorCanvas.canvas.setDimensions({ width: config.width, height: config.height });
        } catch (ex) {
            console.log(ex);
        }
    },

    loadCanvasFromJsonData: function (jsonString, isEncoaded, setActive, completeCallbackHandler) {
        try {
            var jsonData;

            if (isEncoaded) {
                var str = decodeURIComponent(editorCanvas.escape(unescape(Base64Decode(jsonString))));
                jsonData = JSON.parse(str);
            }
            else {
                jsonData = JSON.parse(jsonString);
            }

            try { $.blockUI({ message: $("#dataloading") }); } catch (ex) { }

            editorCanvas.canvas.loadFromJSON(jsonData,
                function () {
                    editorCanvas.canvas.renderAll.bind(editorCanvas.canvas);
                    if (completeCallbackHandler != null && completeCallbackHandler != undefined) {
                        completeCallbackHandler({ IsCanvasOnly: true });
                    }
                    try { $.unblockUI(); } catch (ex) { }
                },
                function (jsonObj, fabricObj) {
                    editorCanvas.action = false;
                    fabricObj.set({
                        borderColor: editorCanvas.commonBorderColor,
                        cornerColor: editorCanvas.commonCornerColor,
                        editingBorderColor: editorCanvas.commonEditBorderColor
                    });

                    editorCanvas.addControlEvents(fabricObj);

                    if (setActive) {
                        if (fabricObj.name && !fabricObj.name.startsWith(editorCanvas.gridLinePrefix)) {
                            fabricObj.set('active', true);
                            //fabricObj.bringToFront();
                        }
                    }
                });

        } catch (ex) {
            try { $.unblockUI(); } catch (ex) { }
            console.log(ex);
        }
    },

    getJsonDataFromCanvas: function (isEncoaded) {
        try {
            var jsonString;
            var canvasData = editorCanvas.canvas.toJSON
                (['name', 'padding', 'cornerSize', 'transparentCorners', 'hasRotatingPoint',
                    'centeredScaling', 'borderScaleFactor', 'borderOpacityWhenMoving']);

            if (canvasData != null && canvasData != undefined) {
                for (var i = 0; i < canvasData.objects.length; ++i) {
                    if (canvasData.objects[i].name != null && canvasData.objects[i].name != undefined) {
                        if (canvasData.objects[i].name === 'bounding-box' ||
                            canvasData.objects[i].name.startsWith(editorCanvas.gridLinePrefix)) {
                            canvasData.objects.splice(i--, 1);
                        }
                    }
                }
                editorCanvas.showHideUndoRedo();
                jsonString = JSON.stringify(canvasData);

                if (isEncoaded)
                    return Base64Encode(jsonString);
                else
                    return jsonString;
            }

        } catch (ex) {
            console.log(ex);
        }
    },

    convertCanvasToImage: function (options) {
        try {
            var defaultOptions = {
                format: 'jpeg',
                quality: 1,
                multiplier: 1
            };

            var obj = $.extend(defaultOptions, options);
            editorCanvas.destroyBoundryLimit();

            var dataUrl = editorCanvas.canvas.toDataURL(obj);
            editorCanvas.createBoundryLimit();

            return dataUrl;

        } catch (ex) {
            console.log(ex);
        }
    },

    convertCanvasToSVG: function (options) {
        try {
            editorCanvas.destroyBoundryLimit();

            var canvasScaleFactor = 0, xPoint = 0, yPoint = 0; imageScaleFactor = 0;
            if (editorCanvas.CANVAS_SCALE > .200000000000000) {
                editorCanvas.CANVAS_SCALE = canvasScaleValue / editorCanvas.SCALE_FACTOR;
            }
            canvasScaleFactor = (editorCanvas.CANVAS_SCALE / editorCanvas.SCALE_FACTOR);

            var svgData = editorCanvas.canvas.toSVG({
                viewBox: {
                    x: 0,
                    y: 0,
                    width: (options.originalWidth),
                    height: (options.originalHeight)
                }
            });

            var xmlDoc = $.parseXML(svgData);
            var $xml = $(xmlDoc);
            var $svg = $xml.find('svg');

            if (options.templateId == 7)
                imageScaleFactor = 0.385;
            else
                imageScaleFactor = canvasScaleFactor.toFixed(4)

            $xml.find('g[transform$="scale(0.38)"],g[transform$="scale(0.38 0.38)"]')
                .each(function () {
                    var prevValue = $(this).attr('transform');
                    $(this).attr('transform',
                        prevValue.replace('scale(0.38 0.38)', 'scale({0} {0})'.format(imageScaleFactor))
                            .replace('scale(0.38)', 'scale({0})'.format(imageScaleFactor)));
                });

            if ($svg.length > 0) {
                $svg.attr({
                    'viewBox': '{0} {1} {2} {3}'.format(xPoint, yPoint,
                        parseFloat(options.originalWidth * canvasScaleFactor),
                        parseFloat(options.originalHeight * canvasScaleFactor))
                });
            }

            editorCanvas.createBoundryLimit();
            return xmlDocToString($xml);

        } catch (ex) {
            console.log(ex);
        }
    },

    getRealtimeWidth: function (activeObject) {
        try {
            if (activeObject != null && activeObject != undefined) {
                return (activeObject.oCoords.tr.x - activeObject.oCoords.tl.x);
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    getRealtimeHeight: function (activeObject) {
        try {
            if (activeObject != null && activeObject != undefined) {
                return (activeObject.oCoords.bl.y - activeObject.oCoords.tl.y);
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    limitBoundry: function (activeObject) {
        try {
            var boundingBox = editorCanvas.canvas.getItemByName('bounding-box');

            if ((activeObject != null && activeObject != undefined)
				&& (boundingBox != null && boundingBox != undefined)) {

                var paddspace = (activeObject.padding + 4);
                var top = activeObject.top + paddspace;
                var bottom = top + activeObject.height;
                var left = activeObject.left + paddspace;
                var right = left + activeObject.width;

                var topBound = boundingBox.top;
                var bottomBound = (topBound + boundingBox.height);
                var leftBound = boundingBox.left;
                var rightBound = (leftBound + boundingBox.width);

                var finalLeft = Math.min(Math.max(left, leftBound),
					(rightBound - editorCanvas.getRealtimeWidth(activeObject)));

                var finalTop = Math.min(Math.max(top, topBound),
					(bottomBound - editorCanvas.getRealtimeHeight(activeObject)));

                activeObject.setLeft(finalLeft == leftBound ? finalLeft + paddspace : finalLeft);
                activeObject.setTop(finalTop == topBound ? finalTop + paddspace : finalTop);
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    makeTextNonMoveable: function (activeObject) {
        try {
            if (activeObject != null && activeObject != undefined) {
                if (activeObject.type == editorCanvas.objectType.IText ||
                    activeObject.type == editorCanvas.objectType.Text ||
                    activeObject.type == editorCanvas.objectType.Line) {
                    activeObject.set({
                        hasControls: false,
                        lockMovementX: true,
                        lockMovementY: true
                    });
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    escape: function (str) {
        return str
          .replace(/\n/g, '\\n')
          .replace(/\r/g, '\\r')
          .replace(/\t/g, '\\t')
          .replace(/\0/g, '');
    },

    objectRepostioning: function () {
        try {
            var activeObject = editorCanvas.canvas.getActiveObject();
            var boundingBox = editorCanvas.canvas.getItemByName('bounding-box');

            if ((activeObject != null && activeObject != undefined) &&
                (boundingBox != null && boundingBox != undefined)) {
                if ((activeObject.left + activeObject.width) > boundingBox.width) {
                    activeObject.left = (boundingBox.width - activeObject.width);
                }
                if ((activeObject.top + activeObject.height) > boundingBox.height) {
                    activeObject.top = (boundingBox.height - activeObject.height);
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    createGridLines: function () {
        try {
            if (editorCanvas.showGrid) {
                if (editorCanvas.hasGridLine())
                    return;

                var canvasWidth = editorCanvas.canvas.width;
                var canvasHeight = editorCanvas.canvas.height;

                for (var i = 0; i < (canvasWidth / editorCanvas.gridScaleSpacing) ; i++) {
                    var horizontalLine = new fabric.Line([i * editorCanvas.gridScaleSpacing, 0, i * editorCanvas.gridScaleSpacing, canvasHeight], {
                        stroke: i % editorCanvas.gridScaleSpacing > 0 ? '#E6E6E6' : '#CDCDCD',
                        selectable: false,
                        opacity: i % editorCanvas.gridScaleSpacing > 0 ? 0.1 : 0.3,
                        name: editorCanvas.gridLinePrefix + 'H_' + i,
                        hasBorders: false,
                        hasControls: false,
                        lockMovementX: true,
                        lockMovementY: true
                    });

                    editorCanvas.canvas.add(horizontalLine);
                    horizontalLine.sendToBack();
                }

                for (var i = 0; i < (canvasHeight / editorCanvas.gridScaleSpacing) ; i++) {
                    var verticalLine = new fabric.Line([0, i * editorCanvas.gridScaleSpacing, canvasWidth, i * editorCanvas.gridScaleSpacing], {
                        stroke: i % editorCanvas.gridScaleSpacing > 0 ? '#E6E6E6' : '#CDCDCD',
                        selectable: false,
                        opacity: i % editorCanvas.gridScaleSpacing > 0 ? 0.1 : 0.3,
                        name: editorCanvas.gridLinePrefix + 'V_' + i,
                        hasBorders: false,
                        hasControls: false,
                        lockMovementX: true,
                        lockMovementY: true
                    });

                    editorCanvas.canvas.add(verticalLine);
                    verticalLine.sendToBack();
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    destroyGridLine: function () {
        try {
            if (editorCanvas.showGrid) {
                var objects = editorCanvas.canvas.getObjects();

                for (var i = 0; i < editorCanvas.canvas.size() ; i++) {
                    if (objects[i].name && objects[i].type == editorCanvas.objectType.Line &&
                        (objects[i].name.startsWith(editorCanvas.gridLinePrefix))) {
                        editorCanvas.canvas.remove(objects[i]);
                        i--;
                    }
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    hasGridLine: function () {
        try {
            var objects = editorCanvas.canvas.getObjects();

            for (var i = 0, len = editorCanvas.canvas.size() ; i < len; i++) {
                if (objects[i].name && objects[i].type == editorCanvas.objectType.Line &&
                    (objects[i].name.startsWith(editorCanvas.gridLinePrefix))) {
                    return true;
                }
            }
            return false;
        } catch (ex) {
            console.log(ex);
        }
    },

    snapObjectOnMoving: function (options) {
        try {
            if (editorCanvas.showGrid) {
                options.target.set({
                    left: Math.round(options.target.left / editorCanvas.gridScaleSpacing) * editorCanvas.gridScaleSpacing,
                    top: Math.round(options.target.top / editorCanvas.gridScaleSpacing) * editorCanvas.gridScaleSpacing
                });
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    snapObjectOnScaling: function (options) {
        try {
            if (editorCanvas.showGrid) {
                var target = options.target,
                w = target.getWidth(),
                h = target.getHeight(),
                snap = {   // Closest snapping points
                    top: Math.round(target.top / editorCanvas.gridScaleSpacing) * editorCanvas.gridScaleSpacing,
                    left: Math.round(target.left / editorCanvas.gridScaleSpacing) * editorCanvas.gridScaleSpacing,
                    bottom: Math.round((target.top + h) / editorCanvas.gridScaleSpacing) * editorCanvas.gridScaleSpacing,
                    right: Math.round((target.left + w) / editorCanvas.gridScaleSpacing) * editorCanvas.gridScaleSpacing
                },
                threshold = editorCanvas.gridScaleSpacing * 0.2,
                dist = {   // Distance from snapping points
                    top: Math.abs(snap.top - target.top),
                    left: Math.abs(snap.left - target.left),
                    bottom: Math.abs(snap.bottom - target.top - h),
                    right: Math.abs(snap.right - target.left - w)
                },
                attrs = {
                    scaleX: target.scaleX,
                    scaleY: target.scaleY,
                    top: target.top,
                    left: target.left
                };

                switch (target.__corner) {
                    case 'tl':
                        if (dist.left < dist.top && dist.left < threshold) {
                            attrs.scaleX = (w - (snap.left - target.left)) / target.width;
                            attrs.scaleY = (attrs.scaleX / target.scaleX) * target.scaleY;
                            attrs.top = target.top + (h - target.height * attrs.scaleY);
                            attrs.left = snap.left;
                        }
                        else if (dist.top < threshold) {
                            attrs.scaleY = (h - (snap.top - target.top)) / target.height;
                            attrs.scaleX = (attrs.scaleY / target.scaleY) * target.scaleX;
                            attrs.left = attrs.left + (w - target.width * attrs.scaleX);
                            attrs.top = snap.top;
                        }
                        break;
                    case 'mt':
                        if (dist.top < threshold) {
                            attrs.scaleY = (h - (snap.top - target.top)) / target.height;
                            attrs.top = snap.top;
                        }
                        break;
                    case 'tr':
                        if (dist.right < dist.top && dist.right < threshold) {
                            attrs.scaleX = (snap.right - target.left) / target.width;
                            attrs.scaleY = (attrs.scaleX / target.scaleX) * target.scaleY;
                            attrs.top = target.top + (h - target.height * attrs.scaleY);
                        }
                        else if (dist.top < threshold) {
                            attrs.scaleY = (h - (snap.top - target.top)) / target.height;
                            attrs.scaleX = (attrs.scaleY / target.scaleY) * target.scaleX;
                            attrs.top = snap.top;
                        }
                        break;
                    case 'ml':
                        if (dist.left < threshold) {
                            attrs.scaleX = (w - (snap.left - target.left)) / target.width;
                            attrs.left = snap.left;
                        }
                        break;
                    case 'mr':
                        if (dist.right < threshold)
                            attrs.scaleX = (snap.right - target.left) / target.width;
                        break;
                    case 'bl':
                        if (dist.left < dist.bottom && dist.left < threshold) {
                            attrs.scaleX = (w - (snap.left - target.left)) / target.width;
                            attrs.scaleY = (attrs.scaleX / target.scaleX) * target.scaleY;
                            attrs.left = snap.left;
                        }
                        else if (dist.bottom < threshold) {
                            attrs.scaleY = (snap.bottom - target.top) / target.height;
                            attrs.scaleX = (attrs.scaleY / target.scaleY) * target.scaleX;
                            attrs.left = attrs.left + (w - target.width * attrs.scaleX);
                        }
                        break;
                    case 'mb':
                        if (dist.bottom < threshold)
                            attrs.scaleY = (snap.bottom - target.top) / target.height;
                        break;
                    case 'br':
                        if (dist.right < dist.bottom && dist.right < threshold) {
                            attrs.scaleX = (snap.right - target.left) / target.width;
                            attrs.scaleY = (attrs.scaleX / target.scaleX) * target.scaleY;
                        }
                        else if (dist.bottom < threshold) {
                            attrs.scaleY = (snap.bottom - target.top) / target.height;
                            attrs.scaleX = (attrs.scaleY / target.scaleY) * target.scaleX;
                        }
                        break;
                }

                target.set(attrs);
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    showCrosshair: function (options) {
        try {
            if (editorCanvas.showGrid && options.target) {
                if (options.target.name && !options.target.name.startsWith(editorCanvas.gridLinePrefix)) {
                    var absCoords = editorCanvas.canvas.getAbsoluteCoords(options.target);
                    $('.vMouse, .hMouse').show();
                    $('.hMouse').css({
                        left: (absCoords.left + 25) + 'px',
                        top: editorCanvas.canvas._offset.top + 'px',
                        height: options.target.canvas.height + 'px'
                    });
                    $('.vMouse').css({
                        top: (absCoords.top - 5) + 'px',
                        left: options.target.canvas._offset.left + 'px',
                        width: options.target.canvas.width + 'px'
                    });
                    $('.ruler').fadeIn('slow');
                }
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    hideCrosshair: function (options) {
        try {
            if (editorCanvas.showGrid) {
                $('.vMouse, .hMouse').hide();
                $('.ruler').fadeOut('slow');
            }
        } catch (ex) {
            console.log(ex);
        }
    },

    createRuler: function () {
        try {
            $('.canvas-container').ruler({
                vRuleSize: 18,
                hRuleSize: 18,
                showCrosshair: false,
                showMousePos: false
            });
        } catch (ex) {
            console.log(ex);
        }
    }
};