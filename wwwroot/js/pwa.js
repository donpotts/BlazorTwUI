// BlazorTwUI PWA Interop
(function () {
    let deferredPrompt = null;
    let dotNetRef = null;

    function isStandalone() {
        return window.matchMedia('(display-mode: standalone)').matches ||
            window.navigator.standalone === true ||
            document.referrer.includes('android-app://');
    }

    window.addEventListener('beforeinstallprompt', (e) => {
        // Prevent Chrome 76+ from automatically showing the prompt
        e.preventDefault();
        deferredPrompt = e;
        console.log('[PWA] beforeinstallprompt captured');
        notifyBlazor();
    });

    window.addEventListener('appinstalled', (evt) => {
        console.log('[PWA] Application successfully installed');
        deferredPrompt = null;
        notifyBlazor();
    });

    function notifyBlazor() {
        if (dotNetRef) {
            try {
                dotNetRef.invokeMethodAsync('OnPwaStateChanged', {
                    canInstall: deferredPrompt !== null,
                    isInstalled: isStandalone()
                });
            } catch (err) {
                console.warn('[PWA] DotNet notify error:', err);
            }
        }
    }

    window.blazorPwa = {
        init: function (dotNetHelper) {
            dotNetRef = dotNetHelper;
            return {
                canInstall: deferredPrompt !== null,
                isInstalled: isStandalone()
            };
        },
        canInstall: function () {
            return deferredPrompt !== null;
        },
        isInstalled: function () {
            return isStandalone();
        },
        promptInstall: async function () {
            if (!deferredPrompt) {
                console.warn('[PWA] No deferred install prompt available');
                return isStandalone() ? 'already_installed' : 'unsupported';
            }

            try {
                // Show the install prompt
                deferredPrompt.prompt();
                // Wait for the user to respond to the prompt
                const choiceResult = await deferredPrompt.userChoice;
                console.log('[PWA] User choice:', choiceResult.outcome);

                if (choiceResult.outcome === 'accepted') {
                    deferredPrompt = null;
                    notifyBlazor();
                }
                return choiceResult.outcome; // 'accepted' or 'dismissed'
            } catch (err) {
                console.error('[PWA] Error prompting installation:', err);
                return 'error';
            }
        }
    };
})();
