window.signalrNotifications = {
    connection: null,
    start: async function (hubUrl, accessToken) {
        if (!window.signalrNotifications.connection) {
            const options = {};
            if (accessToken) {
                options.accessTokenFactory = () => accessToken;
            }
            window.signalrNotifications.connection = new signalR.HubConnectionBuilder()
                .withUrl(hubUrl, options)
                .withAutomaticReconnect()
                .build();

            window.signalrNotifications.connection.onreconnected((id) => console.info('SignalR reconnected', id));
            window.signalrNotifications.connection.onclose((err) => console.info('SignalR closed', err));
        }

        if (window.signalrNotifications.connection.state === signalR.HubConnectionState.Disconnected) {
            await window.signalrNotifications.connection.start();
        }
    },
    // Register a DotNetObjectReference so JS can call back into .NET when a notification arrives
    onNewNotification: function (dotNetRef) {
        if (!window.signalrNotifications.connection) return;
        if (!dotNetRef) return;

        // Ensure we only bind once
        if (window.signalrNotifications._boundNewNotification) return;

        window.signalrNotifications.connection.on('NewNotification', function (notification) {
            try {
                // Invoke the .NET handler
                dotNetRef.invokeMethodAsync('HandleNewNotification', notification).catch(err => console.error('Error invoking .NET HandleNewNotification:', err));
            } catch (e) {
                console.error('Error invoking .NET handler for NewNotification', e);
            }
        });

        window.signalrNotifications._boundNewNotification = true;
    },
    // click-away registration: calls .NET method CloseDropdownFromJs when click occurs outside the selector element
    registerClickAway: function (selector, dotNetRef) {
        try {
            if (!selector || !dotNetRef) return;
            // save handler so we can unregister later
            if (window.signalrNotifications._clickAwayHandler) {
                // remove existing
                document.removeEventListener('click', window.signalrNotifications._clickAwayHandler, true);
                window.signalrNotifications._clickAwayHandler = null;
            }

            var handler = function (e) {
                try {
                    var el = document.querySelector(selector);
                    if (!el) return;
                    if (!el.contains(e.target)) {
                        dotNetRef.invokeMethodAsync('CloseDropdownFromJs').catch(err => console.error('Error invoking CloseDropdownFromJs', err));
                    }
                } catch (ex) {
                    console.error('clickAway handler error', ex);
                }
            };

            window.signalrNotifications._clickAwayHandler = handler;
            // use capture phase to detect clicks early
            document.addEventListener('click', handler, true);
        } catch (ex) {
            console.error('registerClickAway error', ex);
        }
    },
    unregisterClickAway: function () {
        try {
            if (window.signalrNotifications._clickAwayHandler) {
                document.removeEventListener('click', window.signalrNotifications._clickAwayHandler, true);
                window.signalrNotifications._clickAwayHandler = null;
            }
        } catch (ex) {
            console.error('unregisterClickAway error', ex);
        }
    }
};

(function(){
    // Safe wrapper to avoid errors when signalrNotifications is not yet defined
    window.safeSignalr = {
        registerClickAwayIfExists: function(selector, dotNetRef) {
            try {
                if (window.signalrNotifications && typeof window.signalrNotifications.registerClickAway === 'function') {
                    window.signalrNotifications.registerClickAway(selector, dotNetRef);
                }
            } catch (e) { console.error('safe registerClickAwayIfExists error', e); }
        },
        unregisterClickAwayIfExists: function() {
            try {
                if (window.signalrNotifications && typeof window.signalrNotifications.unregisterClickAway === 'function') {
                    window.signalrNotifications.unregisterClickAway();
                }
            } catch (e) { console.error('safe unregisterClickAwayIfExists error', e); }
        },
        // safe start: invoke start if available
        startIfExists: function(hubUrl, accessToken) {
            try {
                if (window.signalrNotifications && typeof window.signalrNotifications.start === 'function') {
                    return window.signalrNotifications.start(hubUrl, accessToken);
                }
                return Promise.resolve();
            } catch (e) { console.error('safe startIfExists error', e); return Promise.resolve(); }
        },
        onNewNotificationIfExists: function(dotNetRef) {
            try {
                if (window.signalrNotifications && typeof window.signalrNotifications.onNewNotification === 'function') {
                    window.signalrNotifications.onNewNotification(dotNetRef);
                }
            } catch (e) { console.error('safe onNewNotificationIfExists error', e); }
        }
    };
})();
