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
    }
};
