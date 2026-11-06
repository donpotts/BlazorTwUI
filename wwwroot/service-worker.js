// BlazorTwUI PWA Service Worker (v0.8.2)
const CACHE_NAME = 'btui-pwa-v0.8.2';
// On localhost (development) the worker caches nothing: a rebuild changes every _framework fingerprint, and a cached
// boot manifest then fails Subresource Integrity ("Failed to find a valid digest").
const IS_DEV = ['localhost', '127.0.0.1', '[::1]'].includes(self.location.hostname);

const PRECACHE_ASSETS = [
    './',
    './index.html',
    './manifest.webmanifest',
    './manifest.json',
    './favicon.svg',
    './favicon.png',
    './apple-touch-icon.png',
    './css/app.min.css',
    './js/sound_fx.js',
    './js/theme.js',
    './js/wheel.js',
    './js/pwa.js',
    './js/media_recorder.js'
];

self.addEventListener('install', event => {
    self.skipWaiting();
    if (IS_DEV) return;
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => {
            return cache.addAll(PRECACHE_ASSETS).catch(err => {
                console.warn('[PWA] Precache warning:', err);
            });
        })
    );
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys => {
            return Promise.all(
                keys.map(key => {
                    if (IS_DEV || key !== CACHE_NAME) {
                        return caches.delete(key);
                    }
                })
            );
        }).then(() => self.clients.claim())
    );
});

self.addEventListener('fetch', event => {
    // Only handle GET requests
    if (event.request.method !== 'GET') return;

    if (IS_DEV) return; // development: straight to the network

    const url = new URL(event.request.url);

    // Fingerprinted framework files are integrity-checked by Blazor and cached by the browser: never keep a copy here.
    if (url.pathname.includes('/_framework/')) return;

    // Always bypass cache for Range requests, video/audio streams, and HyperFrames live stage
    if (
        event.request.headers.has('range') ||
        url.pathname.endsWith('.mp4') ||
        url.pathname.endsWith('.webm') ||
        url.pathname.endsWith('.mp3') ||
        url.pathname.includes('/videos/')
    ) {
        return;
    }

    // Navigation requests (HTML document): Network-first with cache fallback
    if (event.request.mode === 'navigate') {
        event.respondWith(
            fetch(event.request)
                .then(networkResponse => {
                    return caches.open(CACHE_NAME).then(cache => {
                        cache.put(event.request, networkResponse.clone());
                        return networkResponse;
                    });
                })
                .catch(() => caches.match('./index.html').then(r => r || Response.error()))
        );
        return;
    }

    // Static assets, wasm, and framework: Network-first with cache fallback
    event.respondWith(
        fetch(event.request)
            .then(networkResponse => {
                if (networkResponse && networkResponse.status === 200 && networkResponse.type === 'basic') {
                    const responseClone = networkResponse.clone();
                    caches.open(CACHE_NAME).then(cache => {
                        cache.put(event.request, responseClone);
                    });
                }
                return networkResponse;
            })
            .catch(() => caches.match(event.request).then(r => r || Response.error()))
    );
});
