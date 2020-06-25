var CookieConsentLoader = (function () {

    var cookieName = "CookieConsent"; // As in Presentation.CookieConsentAttribute

    function init() {
        mustShowForm() && loadFormInto("#cookieConsentFormContainer");
    }

    function loadFormInto(containerElement) {
        $(containerElement).load("/cookieconsent/form");
    }

    function mustShowForm() {
        return !userHasAnsweredForm();
    }

    function userHasAnsweredForm() {
        return readCookie(cookieName) === "true";
    }

    function readCookie(cookieName) {
        var key = cookieName + "=",
            parts = document.cookie.split(";");

        for (var i = 0; i < parts.length; i++) {
            while (parts[i].charAt(0) === " ") {
                parts[i] = parts[i].substring(1);
            }
            if (parts[i].indexOf(key) === 0) {
                var value = parts[i].substring(key.length);
                return unescape(value);
            }
        }
        return null;
    }

    return {
        init: init
    };

})();