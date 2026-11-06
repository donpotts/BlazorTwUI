// TwLottie: plays a .lottie / Lottie .json file on a canvas with dotlottie-web (MIT, vendored in lib/dotlottie, no CDN).
// Loaded only by pages that render a TwLottie (Blazor imports this module on first render).
// Under prefers-reduced-motion it never plays: it holds the file's "poster" marker frame (or frame 0).
// canvas.dataset.state tells tests and screen code what happened: loading | playing | poster | complete | error.

const base = new URL('../lib/dotlottie/', import.meta.url);
let libPromise = null;

function lib() {
    if (!libPromise) {
        libPromise = import(new URL('dotlottie-web.js', base).href).then(m => {
            m.DotLottie.setWasmUrl(new URL('dotlottie-player.wasm', base).href);
            return m;
        });
    }
    return libPromise;
}

const reducedQuery = () => window.matchMedia ? window.matchMedia('(prefers-reduced-motion: reduce)') : null;

function posterFrame(player, markerName, frame) {
    if (typeof frame === 'number' && frame >= 0) return frame;
    try {
        const marker = (player.markers() || []).find(m => m && m.name === markerName);
        // dotlottie-web 0.81 reports a marker as { name, start, end } (frames); older players used { time }.
        const at = marker ? (typeof marker.start === 'number' ? marker.start : marker.time) : undefined;
        if (typeof at === 'number') return at;
    } catch { /* no markers */ }
    return 0;
}

function showPoster(entry) {
    const { player, canvas, opts } = entry;
    try { player.pause(); } catch { }
    try { player.setFrame(posterFrame(player, opts.posterMarker || 'poster', opts.posterFrame)); } catch { }
    canvas.dataset.state = 'poster';
}

export async function create(canvas, opts, dotnet) {
    if (!canvas) return null;
    canvas.dataset.state = 'loading';
    let m;
    try { m = await lib(); }
    catch (e) { canvas.dataset.state = 'error'; console.warn('[TwLottie] player failed to load', e); return null; }

    const mq = reducedQuery();
    const still = () => opts.reducedMotion !== 'play' && !!(mq && mq.matches);
    const entry = { canvas, opts, dotnet, player: null, mq, onChange: null };
    if (opts.reducedMotion === 'hide' && still()) {
        // Pure decoration under reduced motion: show nothing (the text next to it carries the message).
        canvas.hidden = true;
        canvas.dataset.state = 'hidden';
        return entry;
    }

    const player = new m.DotLottie({
        canvas,
        src: opts.src,
        autoplay: !!opts.autoplay && !still(),
        loop: !!opts.loop,
        speed: opts.speed > 0 ? opts.speed : 1,
        renderConfig: { autoResize: true, freezeOnOffscreen: true }
    });
    entry.player = player;

    player.addEventListener('load', () => {
        if (still()) showPoster(entry);
        else canvas.dataset.state = opts.autoplay ? 'playing' : 'ready';
    });
    player.addEventListener('play', () => { if (still()) showPoster(entry); else canvas.dataset.state = 'playing'; });
    player.addEventListener('complete', () => {
        canvas.dataset.state = 'complete';
        if (dotnet) dotnet.invokeMethodAsync('OnLottieComplete').catch(() => { });
    });
    player.addEventListener('loadError', () => {
        canvas.dataset.state = 'error';
        if (dotnet) dotnet.invokeMethodAsync('OnLottieError').catch(() => { });
    });

    if (mq && mq.addEventListener) {
        entry.onChange = () => { if (still()) showPoster(entry); };
        mq.addEventListener('change', entry.onChange);
    }
    return entry;
}

export function play(entry) {
    if (!entry) return;
    const mq = entry.mq;
    if (entry.opts.reducedMotion !== 'play' && mq && mq.matches) { if (entry.player) showPoster(entry); return; }
    if (entry.player) entry.player.play();
}

export function stop(entry) { if (entry && entry.player) entry.player.stop(); }

export function destroy(entry) {
    if (!entry) return;
    if (entry.mq && entry.onChange && entry.mq.removeEventListener) entry.mq.removeEventListener('change', entry.onChange);
    try { entry.player && entry.player.destroy(); } catch { }
    entry.player = null;
}
