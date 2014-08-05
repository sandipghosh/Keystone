fabric.Cropzoomimage = fabric.util.createClass(fabric.Image,
{
    type: 'cropzoomimage',
    zoomedXY: false,

    initialize: function (element, options) {
        options || (options = {});
        this.callSuper('initialize', element, options);
        this.set({
            orgSrc: element.src,
            cx: 0, // clip-x
            cy: 0, // clip-y
            cw: element.width, // clip-width
            ch: element.height // clip-height
        });
    },

    zoomBy: function (x, y, z, callback) {
        if (x || y) { this.zoomedXY = true; }
        this.cx += x;
        this.cy += y;

        if (z) {
            this.cw -= z;
            this.ch -= z / (this.width / this.height);
        }

        if (z && !this.zoomedXY) {
            // Zoom to center of image initially
            this.cx = this.width / 2 - (this.cw / 2);
            this.cy = this.height / 2 - (this.ch / 2);
        }

        if (this.cw > this.width) { this.cw = this.width; }
        if (this.ch > this.height) { this.ch = this.height; }
        if (this.cw < 1) { this.cw = 1; }
        if (this.ch < 1) { this.ch = 1; }
        if (this.cx < 0) { this.cx = 0; }
        if (this.cy < 0) { this.cy = 0; }
        if (this.cx > this.width - this.cw) { this.cx = this.width - this.cw; }
        if (this.cy > this.height - this.ch) { this.cy = this.height - this.ch; }

        this.rerender(callback);
    },

    rerender: function (callback) {
        var img = new Image(), obj = this;
        img.onload = function () {
            var canvas = fabric.util.createCanvasElement();
            canvas.width = obj.width;
            canvas.height = obj.height;
            canvas.getContext('2d').drawImage(this, obj.cx, obj.cy, obj.cw, obj.ch, 0, 0, obj.width, obj.height);

            img.onload = function () {
                obj.setElement(this);
                obj.applyFilters(editorCanvas.canvas.renderAll.bind(editorCanvas.canvas));
                obj.set({
                    left: obj.left,
                    top: obj.top,
                    angle: obj.angle
                });
                obj.setCoords();
                if (callback) { callback(obj); }
            };
            img.src = canvas.toDataURL('image/png');
        };
        img.src = this.orgSrc;
    },

    toObject: function () {
        return fabric.util.object.extend(this.callSuper('toObject'), {
            orgSrc: this.orgSrc,
            cx: this.cx,
            cy: this.cy,
            cw: this.cw,
            ch: this.ch,
            name: this.name,
            cornerSize: this.cornerSize,
            padding: this.padding,
            transparentCorners: this.transparentCorners,
            hasRotatingPoint: this.hasRotatingPoint,
            centeredScaling: this.centeredScaling,
            borderScaleFactor: this.borderScaleFactor,
            borderOpacityWhenMoving: this.borderOpacityWhenMoving
        });
    }
});

fabric.Cropzoomimage.async = true;


//Hackfix: Single left click to select any iText and single left click on any selected itext for inline edit.
fabric.util.object.extend(fabric.IText.prototype, {
     //Initializes "selected" event handler
     //removed delayed selection init
    initSelectedHandler: function () {
        this.on('selected', function () {

            var _this = this;
            setTimeout(function () {
                _this.selected = true;
            }, 500);

            if (this.canvas && !this.canvas._hasITextHandlers) {
                this._initCanvasHandlers();
                this.canvas._hasITextHandlers = true;
            }
        });
    },

    //Initializes "mouseup" event handler
    initMouseupHandler: function () {
        this.on('mouseup', function (options) {
            this.__isMousedown = false;
            if (this._isObjectMoved(options.e)) return;

            if (this.selected) {
                this.enterEditing();
                this.initDelayedCursor(true);
            }
            var _this = this;
            setTimeout(function () {
                _this.selected = true;
            }, 500);
        });
    }
});

////Hackfix: Starting IText inline editing scrolls to top of the page
//fabric.IText.prototype.initHiddenTextarea = function () {
//    this.hiddenTextarea = fabric.document.createElement('textarea');

//    this.hiddenTextarea.setAttribute('autocapitalize', 'off');
//    this.hiddenTextarea.style.cssText = 'position: absolute; top: 0; left: -9999px';

//    //When I use something like the following, the calling of focus() lets the document scroll positiion where it is:
//    if (this.canvas && this.canvas.upperCanvasEl) {
//        this.canvas.upperCanvasEl.appendChild(this.hiddenTextarea);
//    } else {
//        // fallback with old code, since I'm not sure if upperCanvasEl is always available
//        // when calling the function
//        fabric.document.body.appendChild(this.hiddenTextarea);
//    }

//    fabric.util.addListener(this.hiddenTextarea, 'keydown', this.onKeyDown.bind(this));
//    fabric.util.addListener(this.hiddenTextarea, 'keypress', this.onKeyPress.bind(this));

//    if (!this._clickHandlerInitialized && this.canvas) {
//        fabric.util.addListener(this.canvas.upperCanvasEl, 'click', this.onClick.bind(this));
//        this._clickHandlerInitialized = true;
//    }
//};

////Hackfix: Starting IText inline editing scrolls to top of the page onclick
//fabric.IText.prototype.onClick = function () { };

fabric.Cropzoomimage.fromObject = function (object, callback) {
    fabric.util.loadImage(object.src, function (img) {
        fabric.Image.prototype._initFilters.call(object, object, function (filters) {
            object.filters = filters || [];
            var instance = new fabric.Cropzoomimage(img, object);
            if (callback) { callback(instance); }
        });
    }, null, object.crossOrigin);
};

fabric.Cropzoomimage.fromObject = function (object, callback) {
    fabric.util.loadImage(object.src, function (img) {
        fabric.Image.prototype._initFilters.call(object, object, function (filters) {
            object.filters = filters || [];
            var instance = new fabric.Cropzoomimage(img, object);
            if (callback) { callback(instance); }
        });
    }, null, object.crossOrigin);
};

fabric.Canvas.prototype.getAbsoluteCoords = function (object) {
    return {
        left: (object.left + this._offset.left) - 32,
        top: (object.top + this._offset.top)
    }
};

fabric.Canvas.prototype.getItemByName = function (name) {
    var object = null,
        objects = this.getObjects();

    for (var i = 0, len = this.size() ; i < len; i++) {
        if (objects[i].name && objects[i].name === name) {
            object = objects[i];
            break;
        }
    }

    return object;
};