importScripts('https://www.gstatic.com/firebasejs/5.9.0/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/5.9.0/firebase-messaging.js');

// Initialize the Firebase app in the service worker by passing in the
// messagingSenderId.
firebase.initializeApp({
    'messagingSenderId': '327533907626'
});

// Retrieve an instance of Firebase Messaging so that it can handle background
// messages.
const messaging = firebase.messaging();
console.log("initialized sw");

messaging.setBackgroundMessageHandler(function (payload) {
    console.log('[firebase-messaging-sw.js] Received background message ', payload);
    // Customize notification here
    var notificationTitle = payload.notification.title;
    var notificationOptions = {
        body: payload.notification.body,
        icon: payload.data.custom_icon
    };

    var notif = self.registration.showNotification(notificationTitle,
        notificationOptions);

    if (payload.data.custom_link) {
        notif.onclick = function (e) {
            window.open(payload.data.custom_link);
        };
    }

    return notif;
});

self.addEventListener('notificationclick', function (e) {
    var notification = e.notification;
    var action = e.action;

    if (action === 'close') {
        notification.close();
    } else {
        if (notification.data.link) {
            window.open(notification.data.link);
        }
        notification.close();
    }
});