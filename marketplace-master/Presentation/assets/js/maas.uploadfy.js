(function ($) {
    function uploadfy($inputFile, options) {
        $inputFile.change(function (e) {
            var files = e.target.files;

            if (files && files.length > 0) {
                options.started();

                var formData = new FormData();

                for (var i = 0, len = files.length; i < len; i++) {
                    var file = files[i];

                    if (options.otherFiles !== true) {
                        if (!file.type.match('image.*'))
                            continue;
                    }
                    if (options.otherFiles === true) {
                        if (/application\/\S*/.test(file.type) && !/application\/pdf/.test(file.type))
                            continue;

                        if (/text\/javascript/.test(file.type))
                            continue;
                    }

                    // Add the file to the request.
                    formData.append('file[]', file, file.name);
                }

                $.ajax({
                    type: 'POST',
                    url: options.url,
                    credentials: true,
                    cache: false,
                    contentType: false,
                    processData: false,
                    data: formData,
                    xhr: function () {
                        var myXhr = $.ajaxSettings.xhr();

                        if (myXhr.upload) {
                            myXhr.upload.addEventListener('progress', function (e) {
                                if (e.lengthComputable) {
                                    var percentage = (e.loaded * 100) / e.total;
                                    options.progress(percentage);
                                }
                            }, false);
                        }

                        return myXhr;
                    },
                    success: function (response) {
                        options.completed(response);
                    },
                    error: function (e) {
                        options.error(e);
                    },
                    complete: function () {
                        options.finaly();
                    }
                });
            }
        });
    }

    $.fn.uploadfy = function (options) {
        var settings = $.extend({
            url: '/api/upload/' + ((options !== null ? options.fileType : null) || 'product') + 'image', // ex. api/uploadProductImage
            started: $.noop,
            progress: $.noop,
            completed: $.noop,
            error: $.noop,
            finaly: $.noop
        }, options);

        uploadfy(this, settings);
        return this;
    };

}(window.jQuery));