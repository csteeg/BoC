/// <reference path="file://c:/temp/jquery-1.2.3-intellisense.js" />
//jQuery for asp.net mvc
//version 0.4 - 21 juli 2008
//by Chris van de steeg - http://www.chrisvandesteeg.nl
jQuery.mvc = {};

jQuery.fn.showResult = function() { this.queue([]); this.css("opacity", 0); this.fadeTo('normal', 1) };
jQuery.fn.hideResult = function() { /*this.fadeTo('fast',0)*/ };
jQuery.mvc.lockMessages = false;

jQuery.mvcJson = function(url, callback) {
    jQuery.mvc.request({ url: url, method: "POST", json: true, onSuccess: callback });
};

jQuery.fn.mvcJsonForm = function(opts) {
    var t = jQuery(this);
    var url = t.attr('action');

    if (!opts)
        opts = {};
    opts.url = url;
    opts.method = "POST";
    opts.json = true;
    opts.data = t.serialize();

    jQuery.mvc.request(opts);
}

jQuery.fn.mvcAjaxForm = function(opts) {
    var t = jQuery(this);
    var url = t.attr('action');
    if (!opts)
        opts = {};
    opts.url = url;
    opts.method = "POST";
    opts.json = false;
    opts.data = t.serialize();

    jQuery.mvc.request(opts);
}

jQuery.mvc.request = function(opts) {
    //if (typeof (jQuery.mvc.originalState) == 'undefined')
    //    jQuery.mvc.originalState = jQuery(document.body).html();
    if (!opts)
        return;
    if (!opts.data)
        opts.data = "";

    if (opts.showResultIn) {
        opts.showResultIn = jQuery(opts.showResultIn);
        if (!opts.showResultIn.is(":hidden"))
            opts.showResultIn.hideResult();
        //showResultIn.html('');

        if (opts.showResultIn.get(0) && opts.showResultIn.get(0).id) {
            opts.data += "&jqtarget=" + opts.showResultIn.get(0).id;
        }
    }

    var stopLoader;
    if (opts.loadingElement) {
        opts.loadingElement.show();
        stopLoader = function() { opts.loadingElement.hide(); }
    }


    return jQuery.ajax({
        type: opts.method,
        url: opts.url,
        data: opts.data,
        success: function(resultObj) {
            var goOn = true;
            if (opts.onBeforeSuccess)
                goOn = opts.onBeforeSuccess(resultObj);
            if (goOn != false) {
                jQuery.mvc.processRequest({
                    resultObj: resultObj,
                    callback: opts.callback,
                    isError: false,
                    showResultIn: opts.showResultIn,
                    url: opts.url,
                    insertionMode: opts.insertionMode

                });
            }
            if (opts.onSuccess)
                opts.onSuccess(resultObj);
        },
        error: function(resultObj, status) {
            var goOn = true;
            if (opts.onBeforeError)
                goOn = opts.onBeforeError(resultObj);
            if (goOn != false) {
                jQuery.mvc.processRequest({
                    resultObj: resultObj,
                    callback: opts.callback,
                    isError: true,
                    showResultIn: opts.showResultIn,
                    url: opts.url,
                    insertionMode: opts.insertionMode
                });
            }
            if (opts.onError)
                opts.onError(resultObj);
        },
        complete: stopLoader,
        dataType: (opts.json ? "json" : null),
        beforeSend: (!opts.json ? null : function(xml) {
            xml.setRequestHeader("X-IsJson", "true");
        })
    });
};

jQuery.mvc.clearMessages = function(force) {
    if (force || !jQuery.mvc.lockMessages) {
        jQuery('#errors').html('');
        jQuery('#messages').html('');
    }
}

jQuery.fn.mvcAddMessage = function(msg) {
    var t = jQuery(this);
    if (t.html() == '')
        t.html('<ul></ul>');
    var ul = jQuery('ul', t);
    ul.append('<li>' + msg + '</li>');
}

jQuery.mvc.processRequest = function(opts) {
    jQuery.mvc.clearMessages()
    var msgContainer = jQuery('#messages');
    var errorContainer = jQuery('#errors');
    var resultObj = opts.resultObj;
    var callback = opts.callback;
    var isError = opts.isError;
    var showResultIn = opts.showResultIn;
    var url = opts.url;

    if ((typeof (resultObj) != 'undefined') &&
        typeof (resultObj.getResponseHeader) == 'function' &&
        resultObj.getResponseHeader("Content-Type").indexOf("json") >= 0) {
        resultObj = eval("(" + resultObj.responseText + ")");
    }

    var result = true;
    var errorSet = false;
    if (jQuery.isFunction(callback))
        result = callback(resultObj);

    if (result != false && typeof (resultObj) != 'undefined') {
        if (msgContainer && typeof (resultObj.messages) != 'undefined') {
            jQuery(resultObj.messages).each(function(t) {
                msgContainer.mvcAddMessage(this);
            });
        }
        if (errorContainer && typeof (resultObj.errors) != 'undefined') {
            jQuery(resultObj.errors).each(function(t) {
                errorContainer.mvcAddMessage(this);
            });
            errorSet = true;
        }
    }
    if (result && showResultIn) {
        var html = typeof (resultObj.responseText) == 'undefined' ?
            resultObj : resultObj.responseText;

        if (html.match(/<html/igm)) {
            //we received a whole page :(
            document.location.href = url;
        }
        else {
            if (!opts.insertionMode || opts.insertionMode == Sys.Mvc.InsertionMode.replace)
                showResultIn.html(html);
            else if (opts.insertionMode == Sys.Mvc.InsertionMode.insertBefore)
                showResultIn.prepend(html);
            else
                showResultIn.append(html);

            var ajaxMsg = jQuery('.jqmvc-messages', showResultIn);
            msgContainer.append(ajaxMsg.html());
            ajaxMsg.remove();

            var errorMsg = jQuery('.jqmvc-errors', showResultIn);
            errorContainer.append(errorMsg.html());
            errorSet = errorMsg.size() > 0;
            errorMsg.remove();
            jQuery('a', showResultIn).supportHistory();

            showResultIn.showResult();
        }
    }
    if (isError && !errorSet) {
        jQuery(document.body).html(resultObj.responseText);
    }

    return result;
}
jQuery.mvc.initialize = function() {
    //if (typeof(jQuery.mvc.originalState) != 'undefined')
    //    jQuery(document.body).html(jQuery.mvc.originalState);
    jQuery('a').supportHistory();
    jQuery("#loading").ajaxStart(function() {
        $(this).show();
    });
    jQuery("#loading").ajaxStop(function() {
        $(this).hide();
    });
    if (typeof ($.ajaxHistory) != 'undefined') {
        jQuery.ajaxHistory.initialize();
    }
}
jQuery.fn.supportHistory = function() {
    if (typeof ($.ajaxHistory) != 'undefined') {
        return this.each(function() {
            var link = jQuery(this);
            if (typeof link.attr('onclick') != 'undefined' && link.attr('href').substr(0, 1) != '#' &&
			    (link.attr('onclick') + '').match(/Sys\.Mvc\.AsyncHyperlink\.handleClick/ig)) {
                link.attr('href', '#' + link.attr('href'));
                var func = link.attr('onclick');
                link.attr('onclick', '');
                link.history(func);
            }
        })
    }
    return this;
};

jQuery().ready(function() {
    jQuery.mvc.initialize();
})

/* ajaxhelper compatible: */
Sys = {};
Sys.Mvc = {};
Sys.Mvc.InsertionMode = {
    replace: 0,
    insertBefore: 1,
    insertAfter: 2
}

Sys.Mvc.AsyncForm = {}
Sys.Mvc.AsyncForm.handleSubmit = function(form, event, ajaxOptions) {
    if (event) event.preventDefault();

    if (ajaxOptions.confirm) {
        if (!confirm(ajaxOptions.confirm)) {
            return;
        }
    }

    if (ajaxOptions.onBegin) {
        ajaxOptions.onBegin();
    }


    if (ajaxOptions.httpMethod) {
        method = ajaxOptions.httpMethod;
    }

    var loadingElement;
    if (ajaxOptions.loadingElementId) {
        loadingElement = jQuery('#' + ajaxOptions.loadingElementId);
    }

    jQuery(form).mvcAjaxForm({
        showResultIn: '#' + ajaxOptions.updateTargetId,
        insertionMode: ajaxOptions.insertionMode,
        loadingElement: loadingElement,
        onSuccess: ajaxOptions.onSuccess,
        onError: ajaxOptions.onFailure,
        callback: ajaxOptions.onComplete
    });
}

Sys.UI = {};
Sys.UI.DomEvent = function SysUIDomEvent(ev) {
    if (!ev) ev = window.event;
    if (ev) return jQuery.event.fix(ev);
}

Sys.Mvc.AsyncHyperlink = {}
Sys.Mvc.AsyncHyperlink.handleClick = function(anchor, dummy, ajaxOptions) {
    var method = "GET";
    var body = "";
    var link = jQuery(anchor);
    var hash = link.attr('href').substr(0, 1)
    var url = link.attr('href');

    if (hash != '#') {
        alert(hash);
        url = '#' + url;
        link.attr('href', url);
    }

    if (ajaxOptions.confirm) {
        if (!confirm(ajaxOptions.confirm)) {
            return;
        }
    }

    if (ajaxOptions.onBegin) {
        ajaxOptions.onBegin();
    }

    if (ajaxOptions.httpMethod) {
        method = ajaxOptions.httpMethod;
    }

    var loadingElement;
    if (ajaxOptions.loadingElementId) {
        loadingElement = jQuery('#' + ajaxOptions.loadingElementId);
    }

    jQuery.mvc.request({
        url: url.substr(1),
        showResultIn: '#' + ajaxOptions.updateTargetId,
        method: method,
        json: false,
        data: "__MVCASYNCPOST=true",
        insertionMode: ajaxOptions.insertionMode,
        loadingElement: loadingElement,
        onSuccess: ajaxOptions.onSuccess,
        onError: ajaxOptions.onFailure,
        callback: ajaxOptions.onComplete
    });

    return false;
}

Function.__typeName = 'Function';
Function.__class = true;
Function.createDelegate = function(instance, method) {
    if (method) {
        return function() {
            return method.apply(instance, arguments);
        }
    }
}
