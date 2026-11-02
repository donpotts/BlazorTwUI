// Browser helpers for the agent-kit components: tail-following scroller, microphone capture, and the MCP app frame bridge.
window.twAgentKit = (function () {
    const scrollers = new WeakMap();
    let voice = null;
    const frames = new WeakMap();

    function distanceFromEnd(el) {
        return el.scrollHeight - el.scrollTop - el.clientHeight;
    }

    return {
        elementSize(el) { return el ? [el.clientWidth, el.clientHeight] : [0, 0]; },

        focusRoving(root) {
            const el = root && root.querySelector('[tabindex="0"]');
            if (el) el.focus();
        },

        // ---- TwMessageScroller ----
        observeTail(el, ref) {
            if (!el || scrollers.has(el)) return;
            const state = { stick: true, ref };
            const onScroll = () => {
                const stick = distanceFromEnd(el) < 24;
                if (stick !== state.stick) {
                    state.stick = stick;
                    ref.invokeMethodAsync('OnStickChanged', stick);
                }
            };
            const mo = new MutationObserver(() => { if (state.stick) el.scrollTop = el.scrollHeight; });
            mo.observe(el, { childList: true, subtree: true, characterData: true });
            el.addEventListener('scroll', onScroll, { passive: true });
            state.dispose = () => { mo.disconnect(); el.removeEventListener('scroll', onScroll); };
            scrollers.set(el, state);
            el.scrollTop = el.scrollHeight;
        },
        scrollToEnd(el) {
            if (!el) return;
            const s = scrollers.get(el);
            if (s && !s.stick) { s.stick = true; s.ref.invokeMethodAsync('OnStickChanged', true); }
            el.scrollTop = el.scrollHeight;
        },
        disposeTail(el) {
            const s = el && scrollers.get(el);
            if (s) { s.dispose(); scrollers.delete(el); }
        },

        // ---- TwVoiceInput ----
        async voiceStart(ref) {
            if (voice) return true;
            if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) return false;
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            const Ctx = window.AudioContext || window.webkitAudioContext;
            const ctx = new Ctx();
            const analyser = ctx.createAnalyser();
            analyser.fftSize = 64;
            ctx.createMediaStreamSource(stream).connect(analyser);
            const data = new Uint8Array(analyser.frequencyBinCount);
            const chunks = [];
            const recorder = new MediaRecorder(stream);
            recorder.ondataavailable = e => { if (e.data.size) chunks.push(e.data); };
            recorder.start();
            let last = 0, raf = 0;
            const tick = t => {
                raf = requestAnimationFrame(tick);
                if (t - last < 66) return; // ~15 fps is plenty for a meter
                last = t;
                analyser.getByteFrequencyData(data);
                ref.invokeMethodAsync('OnLevels', Array.from(data, v => v / 255));
            };
            raf = requestAnimationFrame(tick);
            voice = { stream, ctx, recorder, chunks, stop: () => cancelAnimationFrame(raf) };
            return true;
        },
        async voiceStop() {
            if (!voice) return null;
            const v = voice;
            voice = null;
            v.stop();
            const done = new Promise(r => { v.recorder.onstop = r; });
            if (v.recorder.state !== 'inactive') v.recorder.stop();
            await done;
            v.stream.getTracks().forEach(t => t.stop());
            await v.ctx.close();
            const blob = new Blob(v.chunks, { type: v.recorder.mimeType || 'audio/webm' });
            const buf = new Uint8Array(await blob.arrayBuffer());
            let bin = '';
            for (let i = 0; i < buf.length; i += 0x8000) bin += String.fromCharCode.apply(null, buf.subarray(i, i + 0x8000));
            return { mime: blob.type, base64: btoa(bin) };
        },

        // ---- TwMcpAppFrame ----
        attachFrame(iframe, ref) {
            if (!iframe || frames.has(iframe)) return;
            const handler = e => {
                // Only messages that came from this sandboxed frame; it has an opaque origin, so we cannot check e.origin.
                if (e.source !== iframe.contentWindow) return;
                let text;
                try { text = typeof e.data === 'string' ? e.data : JSON.stringify(e.data); } catch { return; }
                ref.invokeMethodAsync('OnFrameMessage', text);
            };
            window.addEventListener('message', handler);
            frames.set(iframe, handler);
        },
        postToFrame(iframe, json) {
            if (iframe && iframe.contentWindow) iframe.contentWindow.postMessage(JSON.parse(json), '*');
        },
        detachFrame(iframe) {
            const h = iframe && frames.get(iframe);
            if (h) { window.removeEventListener('message', h); frames.delete(iframe); }
        }
    };
})();
