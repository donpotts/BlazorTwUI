window.blazorTheme = {
    storageKey: 'btui-theme-mode',
    accentKey: 'btui-theme-accent',
    customHexKey: 'btui-theme-custom-hex',
    contrastKey: 'btui-theme-contrast',
    contrastQuery: null,
    contrastListener: null,
    dotNetHelper: null,
    mediaQuery: null,
    mediaListener: null,

    scrollToElement: function (id) {
        const el = document.getElementById(id);
        if (el) {
            el.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    },

    hexToRgb: function (hex) {
        let clean = hex.replace('#', '').trim();
        if (clean.length === 3) {
            clean = clean.split('').map(c => c + c).join('');
        }
        if (clean.length !== 6) return null;
        const num = parseInt(clean, 16);
        return {
            r: (num >> 16) & 255,
            g: (num >> 8) & 255,
            b: num & 255
        };
    },

    applyCustomShades: function (hex) {
        const rgb = this.hexToRgb(hex);
        if (!rgb) return;
        const root = document.documentElement;

        const mixWhite = (val, factor) => Math.round(val * (1 - factor) + 255 * factor);
        const mixBlack = (val, factor) => Math.round(val * factor);

        const s50 = `${mixWhite(rgb.r, 0.92)} ${mixWhite(rgb.g, 0.92)} ${mixWhite(rgb.b, 0.92)}`;
        const s100 = `${mixWhite(rgb.r, 0.82)} ${mixWhite(rgb.g, 0.82)} ${mixWhite(rgb.b, 0.82)}`;
        const s200 = `${mixWhite(rgb.r, 0.65)} ${mixWhite(rgb.g, 0.65)} ${mixWhite(rgb.b, 0.65)}`;
        const s300 = `${mixWhite(rgb.r, 0.45)} ${mixWhite(rgb.g, 0.45)} ${mixWhite(rgb.b, 0.45)}`;
        const s400 = `${mixWhite(rgb.r, 0.25)} ${mixWhite(rgb.g, 0.25)} ${mixWhite(rgb.b, 0.25)}`;
        const s500 = `${rgb.r} ${rgb.g} ${rgb.b}`;
        const s600 = `${mixBlack(rgb.r, 0.85)} ${mixBlack(rgb.g, 0.85)} ${mixBlack(rgb.b, 0.85)}`;
        const s700 = `${mixBlack(rgb.r, 0.72)} ${mixBlack(rgb.g, 0.72)} ${mixBlack(rgb.b, 0.72)}`;

        root.style.setProperty('--color-brand-50', s50);
        root.style.setProperty('--color-brand-100', s100);
        root.style.setProperty('--color-brand-200', s200);
        root.style.setProperty('--color-brand-300', s300);
        root.style.setProperty('--color-brand-400', s400);
        root.style.setProperty('--color-brand-500', s500);
        root.style.setProperty('--color-brand-600', s600);
        root.style.setProperty('--color-brand-700', s700);
        root.setAttribute('data-accent', 'custom');
    },

    clearCustomShades: function () {
        const root = document.documentElement;
        root.style.removeProperty('--color-brand-50');
        root.style.removeProperty('--color-brand-100');
        root.style.removeProperty('--color-brand-200');
        root.style.removeProperty('--color-brand-300');
        root.style.removeProperty('--color-brand-400');
        root.style.removeProperty('--color-brand-500');
        root.style.removeProperty('--color-brand-600');
        root.style.removeProperty('--color-brand-700');
    },

    init: function (dotNetRef) {
        this.dotNetHelper = dotNetRef;
        const urlParams = new URLSearchParams(window.location.search);
        const queryTheme = urlParams.get('theme');
        const queryAccent = urlParams.get('accent');
        const mode = queryTheme || localStorage.getItem(this.storageKey) || 'system';
        const accent = queryAccent || localStorage.getItem(this.accentKey) || 'indigo';
        const customHex = localStorage.getItem(this.customHexKey);

        this.applyTheme(mode);
        const contrast = localStorage.getItem(this.contrastKey) || 'system';
        this.applyContrast(contrast);

        if (!this.contrastQuery) {
            this.contrastQuery = window.matchMedia('(prefers-contrast: more)');
            this.contrastListener = () => {
                const current = localStorage.getItem(this.contrastKey) || 'system';
                if (current === 'system') {
                    const high = this.applyContrast('system');
                    if (this.dotNetHelper) {
                        this.dotNetHelper.invokeMethodAsync('OnSystemContrastChanged', high);
                    }
                }
            };
            this.contrastQuery.addEventListener('change', this.contrastListener);
        }

        if (accent === 'custom' && customHex) {
            this.applyCustomShades(customHex);
        } else {
            this.clearCustomShades();
            this.applyAccent(accent);
        }

        if (!this.mediaQuery) {
            this.mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
            this.mediaListener = (e) => {
                const currentMode = localStorage.getItem(this.storageKey) || 'system';
                if (currentMode === 'system') {
                    this.updateDomClass(e.matches);
                    if (this.dotNetHelper) {
                        this.dotNetHelper.invokeMethodAsync('OnSystemThemeChanged', e.matches);
                    }
                }
            };
            this.mediaQuery.addEventListener('change', this.mediaListener);
        }

        return {
            mode: mode,
            accent: accent,
            customHex: accent === 'custom' ? customHex : null,
            isDark: document.documentElement.classList.contains('dark'),
            contrast: contrast,
            isHighContrast: document.documentElement.getAttribute('data-contrast') === 'high'
        };
    },

    setContrast: function (contrast) {
        localStorage.setItem(this.contrastKey, contrast);
        return this.applyContrast(contrast);
    },

    // 'high' forces it on, 'standard' forces it off, 'system' follows prefers-contrast. Returns the effective state.
    applyContrast: function (contrast) {
        const high = contrast === 'high' ||
            (contrast !== 'standard' && window.matchMedia('(prefers-contrast: more)').matches);
        const root = document.documentElement;
        if (high) {
            root.setAttribute('data-contrast', 'high');
        } else {
            root.removeAttribute('data-contrast');
        }
        return high;
    },

    setMode: function (mode) {
        localStorage.setItem(this.storageKey, mode);
        return this.applyTheme(mode);
    },

    setAccent: function (accent) {
        this.clearCustomShades();
        localStorage.removeItem(this.customHexKey);
        localStorage.setItem(this.accentKey, accent);
        this.applyAccent(accent);
    },

    setCustomAccent: function (hex) {
        if (!hex) return;
        localStorage.setItem(this.accentKey, 'custom');
        localStorage.setItem(this.customHexKey, hex);
        this.applyCustomShades(hex);
    },

    applyTheme: function (mode) {
        let isDark = false;
        if (mode === 'dark') {
            isDark = true;
        } else if (mode === 'light') {
            isDark = false;
        } else {
            isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        }
        this.updateDomClass(isDark);
        return isDark;
    },

    applyAccent: function (accent) {
        document.documentElement.setAttribute('data-accent', accent || 'indigo');
    },

    updateDomClass: function (isDark) {
        const root = document.documentElement;
        if (isDark) {
            root.classList.add('dark');
        } else {
            root.classList.remove('dark');
        }
    },

    dispose: function () {
        if (this.contrastQuery && this.contrastListener) {
            this.contrastQuery.removeEventListener('change', this.contrastListener);
            this.contrastQuery = null;
        }
        if (this.mediaQuery && this.mediaListener) {
            this.mediaQuery.removeEventListener('change', this.mediaListener);
        }
        if (this.scrollHandler) {
            window.removeEventListener('scroll', this.scrollHandler);
            this.scrollHandler = null;
        }
        this.dotNetHelper = null;
        this.scrollDotNetHelper = null;
    },

    scrollToTop: function (smooth = true) {
        window.scrollTo({
            top: 0,
            left: 0,
            behavior: smooth ? 'smooth' : 'auto'
        });
    },

    registerScrollListener: function (dotNetHelper, threshold) {
        this.scrollDotNetHelper = dotNetHelper;
        this.scrollThreshold = threshold || 150;
        if (!this.scrollHandler) {
            this.scrollHandler = () => {
                const y = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
                const isVisible = y > (this.scrollThreshold || 150);
                const totalHeight = document.documentElement.scrollHeight - document.documentElement.clientHeight;
                const progress = totalHeight > 0 ? Math.min(100, Math.round((y / totalHeight) * 100)) : 0;
                if (this.scrollDotNetHelper) {
                    this.scrollDotNetHelper.invokeMethodAsync('OnScrollChanged', isVisible, progress);
                }
            };
            window.addEventListener('scroll', this.scrollHandler, { passive: true });
        }
        // Fire immediately
        this.scrollHandler();
    },

    unregisterScrollListener: function () {
        this.scrollDotNetHelper = null;
    }
};

window.blazorTailwind = {
    paletteHelper: null,
    paletteListenerAttached: false,

    initCommandPaletteListener: function (dotNetHelper) {
        this.paletteHelper = dotNetHelper;
        if (!this.paletteListenerAttached) {
            window.addEventListener('keydown', (e) => {
                if ((e.ctrlKey || e.metaKey) && (e.key === 'k' || e.key === 'K')) {
                    e.preventDefault();
                    if (this.paletteHelper) {
                        this.paletteHelper.invokeMethodAsync('TogglePaletteFromShortcut');
                    }
                }
            });
            this.paletteListenerAttached = true;
        }
    },

    copyToClipboard: async function (text) {
        try {
            if (navigator.clipboard && navigator.clipboard.writeText) {
                await navigator.clipboard.writeText(text);
                return true;
            }
            const textArea = document.createElement("textarea");
            textArea.value = text;
            textArea.style.position = "fixed";
            textArea.style.left = "-999999px";
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
            return true;
        } catch (err) {
            console.error('Failed to copy to clipboard:', err);
            return false;
        }
    },

    initSpotlightTracker: function (element) {
        if (!element) return;
        element.addEventListener('mousemove', (e) => {
            const rect = element.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            element.style.setProperty('--mouse-x', `${x}px`);
            element.style.setProperty('--mouse-y', `${y}px`);
        });
    }
};

window.blazorMediaPlayer = {
    init: function (mediaEl, dotNetRef) {
        if (!mediaEl || !dotNetRef) return;

        const updateState = () => {
            let bufferedEnd = 0;
            if (mediaEl.buffered && mediaEl.buffered.length > 0) {
                bufferedEnd = mediaEl.buffered.end(mediaEl.buffered.length - 1);
            }
            dotNetRef.invokeMethodAsync('OnMediaStateUpdate', {
                currentTime: mediaEl.currentTime || 0,
                duration: mediaEl.duration || 0,
                paused: mediaEl.paused,
                muted: mediaEl.muted,
                volume: mediaEl.volume,
                playbackRate: mediaEl.playbackRate,
                bufferedEnd: bufferedEnd,
                ended: mediaEl.ended
            }).catch(() => {});
        };

        mediaEl._btuiUpdateState = updateState;
        mediaEl.addEventListener('timeupdate', updateState);
        mediaEl.addEventListener('durationchange', updateState);
        mediaEl.addEventListener('play', updateState);
        mediaEl.addEventListener('pause', updateState);
        mediaEl.addEventListener('ended', updateState);
        mediaEl.addEventListener('volumechange', updateState);
        mediaEl.addEventListener('progress', updateState);
        mediaEl.addEventListener('loadedmetadata', updateState);
        mediaEl.addEventListener('loadeddata', updateState);
        mediaEl.addEventListener('seeked', updateState);
        mediaEl.addEventListener('seeking', updateState);

        // Fire once immediately
        updateState();
    },

    play: function (mediaEl) {
        if (mediaEl && mediaEl.play) {
            mediaEl.play().catch(e => console.warn('Play prevented:', e));
        }
    },

    pause: function (mediaEl) {
        if (mediaEl && mediaEl.pause) {
            mediaEl.pause();
        }
    },

    setCurrentTime: function (mediaEl, time) {
        if (mediaEl) {
            mediaEl.currentTime = time;
            if (typeof mediaEl._btuiUpdateState === 'function') {
                mediaEl._btuiUpdateState();
            }
        }
    },

    seekFromEvent: function (mediaEl, trackEl, clientX) {
        if (!mediaEl || !trackEl) return;
        const rect = trackEl.getBoundingClientRect();
        if (rect.width <= 0) return;
        const ratio = Math.max(0, Math.min(1, (clientX - rect.left) / rect.width));
        const duration = mediaEl.duration || 0;
        if (duration > 0) {
            mediaEl.currentTime = ratio * duration;
            if (typeof mediaEl._btuiUpdateState === 'function') {
                mediaEl._btuiUpdateState();
            }
        }
    },

    setVolume: function (mediaEl, vol) {
        if (mediaEl) {
            mediaEl.volume = Math.max(0, Math.min(1, vol));
            mediaEl.muted = vol === 0;
        }
    },

    setMuted: function (mediaEl, muted) {
        if (mediaEl) {
            mediaEl.muted = muted;
        }
    },

    setPlaybackRate: function (mediaEl, rate) {
        if (mediaEl) {
            mediaEl.playbackRate = rate;
        }
    },

    toggleFullscreen: function (containerEl) {
        if (!containerEl) return;
        if (!document.fullscreenElement) {
            if (containerEl.requestFullscreen) {
                containerEl.requestFullscreen().catch(() => {});
            }
        } else {
            if (document.exitFullscreen) {
                document.exitFullscreen().catch(() => {});
            }
        }
    },

    togglePiP: async function (videoEl) {
        if (!videoEl) return false;
        try {
            if (document.pictureInPictureElement) {
                await document.exitPictureInPicture();
                return false;
            } else if (videoEl.requestPictureInPicture) {
                await videoEl.requestPictureInPicture();
                return true;
            }
        } catch (e) {
            console.warn('PiP error:', e);
        }
        return false;
    }
};

window.blazorBarcodeScanner = {
    codeReader: null,
    activeStream: null,
    currentTrack: null,
    torchOn: false,

    initReader: function () {
        if (!this.codeReader && window.ZXing) {
            this.codeReader = new window.ZXing.BrowserMultiFormatReader();
        }
        return !!this.codeReader;
    },

    getVideoInputDevices: async function () {
        this.initReader();
        if (!this.codeReader) return [];
        try {
            const devices = await this.codeReader.listVideoInputDevices();
            return (devices || []).map(d => ({ deviceId: d.deviceId, label: d.label || `Camera ${d.deviceId.slice(0, 5)}` }));
        } catch (e) {
            console.warn('Cannot list video devices:', e);
            return [];
        }
    },

    startCamera: async function (videoEl, deviceId, dotNetHelper) {
        this.initReader();
        if (!this.codeReader || !videoEl) return false;

        try {
            this.stopCamera();

            this.codeReader.decodeFromVideoDevice(deviceId || undefined, videoEl, (result, err) => {
                if (result) {
                    this.playScanBeep();
                    dotNetHelper.invokeMethodAsync('OnBarcodeDetected', {
                        text: result.getText(),
                        format: result.getBarcodeFormat() ? result.getBarcodeFormat().toString() : 'QR_CODE',
                        timestamp: new Date().toISOString()
                    }).catch(() => {});
                }
            });

            setTimeout(() => {
                if (videoEl.srcObject && videoEl.srcObject.getVideoTracks) {
                    const tracks = videoEl.srcObject.getVideoTracks();
                    if (tracks.length > 0) {
                        this.currentTrack = tracks[0];
                    }
                }
            }, 800);

            return true;
        } catch (e) {
            console.error('Failed to start camera:', e);
            return false;
        }
    },

    stopCamera: function () {
        if (this.codeReader) {
            try {
                this.codeReader.reset();
            } catch (e) {}
        }
        if (this.currentTrack) {
            try {
                this.currentTrack.stop();
            } catch (e) {}
            this.currentTrack = null;
        }
        this.torchOn = false;
    },

    toggleTorch: async function () {
        if (!this.currentTrack) return false;
        try {
            const capabilities = this.currentTrack.getCapabilities ? this.currentTrack.getCapabilities() : {};
            if (capabilities.torch) {
                this.torchOn = !this.torchOn;
                await this.currentTrack.applyConstraints({
                    advanced: [{ torch: this.torchOn }]
                });
                return this.torchOn;
            }
        } catch (e) {
            console.warn('Torch not supported or failed:', e);
        }
        return false;
    },

    decodeDataUrl: async function (dataUrl) {
        this.initReader();
        if (!this.codeReader || !dataUrl) return null;
        return new Promise((resolve) => {
            const img = new Image();
            img.onload = async () => {
                try {
                    const result = await this.codeReader.decodeFromImageElement(img);
                    if (result) {
                        this.playScanBeep();
                        resolve({
                            text: result.getText(),
                            format: result.getBarcodeFormat() ? result.getBarcodeFormat().toString() : 'QR_CODE',
                            timestamp: new Date().toISOString()
                        });
                        return;
                    }
                } catch (e) {}
                resolve(null);
            };
            img.onerror = () => resolve(null);
            img.src = dataUrl;
        });
    },

    playScanBeep: function () {
        try {
            const ctx = new (window.AudioContext || window.webkitAudioContext)();
            const osc = ctx.createOscillator();
            const gain = ctx.createGain();
            osc.type = 'sine';
            osc.frequency.setValueAtTime(880, ctx.currentTime);
            gain.gain.setValueAtTime(0.15, ctx.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.01, ctx.currentTime + 0.12);
            osc.connect(gain);
            gain.connect(ctx.destination);
            osc.start();
            osc.stop(ctx.currentTime + 0.12);
        } catch (e) {}
    },

    downloadFile: function (filename, content, mimeType) {
        const blob = new Blob([content], { type: mimeType });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    },

    downloadDataUrl: function (filename, dataUrl) {
        const a = document.createElement('a');
        a.href = dataUrl;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    }
};

window.blazorSignaturePad = {
    instances: new Map(),

    init: function (canvasEl, dotNetHelper, options) {
        if (!canvasEl) return;

        options = Object.assign({
            penColor: '#0f172a',
            strokeWidth: 2.5,
            backgroundColor: 'transparent',
            guideLine: true
        }, options || {});

        const ctx = canvasEl.getContext('2d');
        let isDrawing = false;
        let strokes = [];
        let undoneStrokes = [];
        let currentStroke = null;

        function resize() {
            const rect = canvasEl.getBoundingClientRect();
            if (rect.width === 0 || rect.height === 0) return;
            const dpr = window.devicePixelRatio || 1;
            canvasEl.width = rect.width * dpr;
            canvasEl.height = rect.height * dpr;
            ctx.scale(dpr, dpr);
            redraw();
        }

        function getPos(e) {
            const rect = canvasEl.getBoundingClientRect();
            return {
                x: e.clientX - rect.left,
                y: e.clientY - rect.top
            };
        }

        function redraw() {
            const rect = canvasEl.getBoundingClientRect();
            ctx.clearRect(0, 0, rect.width, rect.height);

            for (const s of strokes) {
                drawStroke(s);
            }
            if (currentStroke) {
                drawStroke(currentStroke);
            }
        }

        function drawStroke(s) {
            if (!s.points || s.points.length === 0) return;
            ctx.save();
            ctx.strokeStyle = s.color;
            ctx.lineWidth = s.width;
            ctx.lineCap = 'round';
            ctx.lineJoin = 'round';

            if (s.points.length === 1) {
                ctx.fillStyle = s.color;
                ctx.beginPath();
                ctx.arc(s.points[0].x, s.points[0].y, s.width / 2, 0, Math.PI * 2);
                ctx.fill();
            } else {
                ctx.beginPath();
                ctx.moveTo(s.points[0].x, s.points[0].y);
                for (let i = 1; i < s.points.length - 1; i++) {
                    const midX = (s.points[i].x + s.points[i + 1].x) / 2;
                    const midY = (s.points[i].y + s.points[i + 1].y) / 2;
                    ctx.quadraticCurveTo(s.points[i].x, s.points[i].y, midX, midY);
                }
                const last = s.points[s.points.length - 1];
                ctx.lineTo(last.x, last.y);
                ctx.stroke();
            }
            ctx.restore();
        }

        function startDrawing(e) {
            e.preventDefault();
            isDrawing = true;
            try { canvasEl.setPointerCapture(e.pointerId); } catch(err){}
            const pos = getPos(e);
            currentStroke = {
                points: [pos],
                color: options.penColor,
                width: options.strokeWidth
            };
            undoneStrokes = [];
            redraw();
            notifyChange();
        }

        function draw(e) {
            if (!isDrawing || !currentStroke) return;
            e.preventDefault();
            const pos = getPos(e);
            currentStroke.points.push(pos);
            redraw();
        }

        function stopDrawing(e) {
            if (!isDrawing) return;
            e.preventDefault();
            isDrawing = false;
            try { canvasEl.releasePointerCapture(e.pointerId); } catch(err){}
            if (currentStroke && currentStroke.points.length > 0) {
                strokes.push(currentStroke);
                currentStroke = null;
                redraw();
                notifyChange();
            }
        }

        function notifyChange() {
            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync('OnSignatureChanged', {
                    isEmpty: strokes.length === 0,
                    strokeCount: strokes.length
                }).catch(() => {});
            }
        }

        canvasEl.addEventListener('pointerdown', startDrawing);
        canvasEl.addEventListener('pointermove', draw);
        canvasEl.addEventListener('pointerup', stopDrawing);
        canvasEl.addEventListener('pointercancel', stopDrawing);

        const resizeObserver = new ResizeObserver(() => resize());
        resizeObserver.observe(canvasEl);

        setTimeout(() => resize(), 50);

        const instance = {
            canvas: canvasEl,
            setPenColor: (c) => { options.penColor = c; },
            setStrokeWidth: (w) => { options.strokeWidth = w; },
            clear: () => {
                strokes = [];
                undoneStrokes = [];
                currentStroke = null;
                redraw();
                notifyChange();
            },
            undo: () => {
                if (strokes.length > 0) {
                    undoneStrokes.push(strokes.pop());
                    redraw();
                    notifyChange();
                }
            },
            redo: () => {
                if (undoneStrokes.length > 0) {
                    strokes.push(undoneStrokes.pop());
                    redraw();
                    notifyChange();
                }
            },
            isEmpty: () => strokes.length === 0,
            toDataURL: (mimeType = 'image/png', bgColor = 'transparent') => {
                const rect = canvasEl.getBoundingClientRect();
                const dpr = window.devicePixelRatio || 1;
                const exportCanvas = document.createElement('canvas');
                exportCanvas.width = rect.width * dpr;
                exportCanvas.height = rect.height * dpr;
                const expCtx = exportCanvas.getContext('2d');
                expCtx.scale(dpr, dpr);

                if (bgColor && bgColor !== 'transparent') {
                    expCtx.fillStyle = bgColor;
                    expCtx.fillRect(0, 0, rect.width, rect.height);
                }

                for (const s of strokes) {
                    if (!s.points || s.points.length === 0) continue;
                    expCtx.save();
                    expCtx.strokeStyle = s.color;
                    expCtx.lineWidth = s.width;
                    expCtx.lineCap = 'round';
                    expCtx.lineJoin = 'round';

                    if (s.points.length === 1) {
                        expCtx.fillStyle = s.color;
                        expCtx.beginPath();
                        expCtx.arc(s.points[0].x, s.points[0].y, s.width / 2, 0, Math.PI * 2);
                        expCtx.fill();
                    } else {
                        expCtx.beginPath();
                        expCtx.moveTo(s.points[0].x, s.points[0].y);
                        for (let i = 1; i < s.points.length - 1; i++) {
                            const midX = (s.points[i].x + s.points[i + 1].x) / 2;
                            const midY = (s.points[i].y + s.points[i + 1].y) / 2;
                            expCtx.quadraticCurveTo(s.points[i].x, s.points[i].y, midX, midY);
                        }
                        const last = s.points[s.points.length - 1];
                        expCtx.lineTo(last.x, last.y);
                        expCtx.stroke();
                    }
                    expCtx.restore();
                }
                return exportCanvas.toDataURL(mimeType);
            },
            toSvg: () => {
                const rect = canvasEl.getBoundingClientRect();
                let pathStr = '';
                for (const s of strokes) {
                    if (s.points.length < 2) continue;
                    let d = `M ${s.points[0].x.toFixed(1)} ${s.points[0].y.toFixed(1)}`;
                    for (let i = 1; i < s.points.length; i++) {
                        d += ` L ${s.points[i].x.toFixed(1)} ${s.points[i].y.toFixed(1)}`;
                    }
                    pathStr += `<path d="${d}" fill="none" stroke="${s.color}" stroke-width="${s.width}" stroke-linecap="round" stroke-linejoin="round"/>`;
                }
                return `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ${rect.width} ${rect.height}" width="${rect.width}" height="${rect.height}">${pathStr}</svg>`;
            },
            dispose: () => {
                resizeObserver.disconnect();
                canvasEl.removeEventListener('pointerdown', startDrawing);
                canvasEl.removeEventListener('pointermove', draw);
                canvasEl.removeEventListener('pointerup', stopDrawing);
                canvasEl.removeEventListener('pointercancel', stopDrawing);
                window.blazorSignaturePad.instances.delete(canvasEl);
            }
        };

        window.blazorSignaturePad.instances.set(canvasEl, instance);
    },

    setPenColor: function (canvasEl, color) {
        const inst = this.instances.get(canvasEl);
        if (inst) inst.setPenColor(color);
    },

    setStrokeWidth: function (canvasEl, width) {
        const inst = this.instances.get(canvasEl);
        if (inst) inst.setStrokeWidth(width);
    },

    clear: function (canvasEl) {
        const inst = this.instances.get(canvasEl);
        if (inst) inst.clear();
    },

    undo: function (canvasEl) {
        const inst = this.instances.get(canvasEl);
        if (inst) inst.undo();
    },

    redo: function (canvasEl) {
        const inst = this.instances.get(canvasEl);
        if (inst) inst.redo();
    },

    getDataURL: function (canvasEl, mimeType, bgColor) {
        const inst = this.instances.get(canvasEl);
        return inst ? inst.toDataURL(mimeType, bgColor) : null;
    },

    getSvg: function (canvasEl) {
        const inst = this.instances.get(canvasEl);
        return inst ? inst.toSvg() : null;
    },

    download: function (canvasEl, filename, mimeType, bgColor) {
        const inst = this.instances.get(canvasEl);
        if (!inst) return;
        const dataUrl = inst.toDataURL(mimeType, bgColor);
        window.blazorBarcodeScanner.downloadDataUrl(filename, dataUrl);
    },

    dispose: function (canvasEl) {
        const inst = this.instances.get(canvasEl);
        if (inst) inst.dispose();
    }
};

/* ==========================================================================
   Interactive Image Comparison Slider
   ========================================================================== */
window.blazorImageCompare = {
    instances: new Map(),

    init: function (containerEl, dotNetHelper, initialPos) {
        if (!containerEl) return;

        let isDragging = false;
        let position = initialPos !== undefined ? initialPos : 50;

        const updatePosition = (clientX) => {
            const rect = containerEl.getBoundingClientRect();
            if (rect.width <= 0) return;
            const x = Math.max(0, Math.min(clientX - rect.left, rect.width));
            const pct = Math.round((x / rect.width) * 1000) / 10; // 0.0 - 100.0%
            position = pct;
            containerEl.style.setProperty('--compare-pos', `${pct}%`);
            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync('OnPositionChanged', pct);
            }
        };

        const onPointerDown = (e) => {
            isDragging = true;
            containerEl.setPointerCapture(e.pointerId);
            updatePosition(e.clientX);
        };

        const onPointerMove = (e) => {
            if (!isDragging) return;
            updatePosition(e.clientX);
        };

        const onPointerUp = (e) => {
            if (isDragging) {
                isDragging = false;
                try { containerEl.releasePointerCapture(e.pointerId); } catch (_) { }
            }
        };

        containerEl.style.setProperty('--compare-pos', `${position}%`);
        containerEl.addEventListener('pointerdown', onPointerDown);
        containerEl.addEventListener('pointermove', onPointerMove);
        containerEl.addEventListener('pointerup', onPointerUp);
        containerEl.addEventListener('pointercancel', onPointerUp);

        this.instances.set(containerEl, {
            dispose: () => {
                containerEl.removeEventListener('pointerdown', onPointerDown);
                containerEl.removeEventListener('pointermove', onPointerMove);
                containerEl.removeEventListener('pointerup', onPointerUp);
                containerEl.removeEventListener('pointercancel', onPointerUp);
                window.blazorImageCompare.instances.delete(containerEl);
            }
        });
    },

    setPosition: function (containerEl, pct) {
        if (!containerEl) return;
        const clamped = Math.max(0, Math.min(pct, 100));
        containerEl.style.setProperty('--compare-pos', `${clamped}%`);
    },

    dispose: function (containerEl) {
        const inst = this.instances.get(containerEl);
        if (inst) inst.dispose();
    }
};

/* ==========================================================================
   Multi-Style Celebration & Particle Physics Engine
   Supports: confetti, fireworks, emojis, coins, stars, streamers
   ========================================================================== */
window.blazorCelebration = {
    fire: function (canvasEl, opts) {
        if (!canvasEl) return;
        const ctx = canvasEl.getContext('2d');
        if (!ctx) return;

        if (canvasEl._blazorAnimId) {
            cancelAnimationFrame(canvasEl._blazorAnimId);
            canvasEl._blazorAnimId = null;
        }

        opts = opts || {};
        const style = (opts.style || 'confetti').toLowerCase();
        const count = opts.count || 90;
        const originX = opts.x !== undefined ? opts.x : 0.5;
        const originY = opts.y !== undefined ? opts.y : 0.5;
        const speed = typeof opts.speed === 'number' && opts.speed > 0 ? opts.speed : 1.0;

        // Play matched procedural Sound FX (freely distributable Web Audio synthesis)
        if (opts.sound !== false && window.blazorSoundFx) {
            window.blazorSoundFx.play(style, typeof opts.soundVolume === 'number' ? opts.soundVolume : 1.0);
        }

        const colors = opts.colors || [
            '#6366f1', '#4f46e5', '#10b981', '#f59e0b',
            '#ef4444', '#ec4899', '#8b5cf6', '#06b6d4', '#eab308'
        ];

        // Detect Fullscreen canvas (or attached fixed layer)
        const isFullScreen = canvasEl.classList.contains('fixed') || opts.fullscreen || false;

        if (isFullScreen) {
            if (canvasEl.parentElement !== document.body) {
                document.body.appendChild(canvasEl);
            }
            canvasEl.style.position = 'fixed';
            canvasEl.style.top = '0px';
            canvasEl.style.left = '0px';
            canvasEl.style.width = '100vw';
            canvasEl.style.height = '100vh';
            canvasEl.style.zIndex = '99999';
            canvasEl.style.pointerEvents = 'none';
        }

        // Resize canvas to match display size exactly
        const rect = canvasEl.getBoundingClientRect();
        const dpr = window.devicePixelRatio || 1;
        const width = isFullScreen ? window.innerWidth : rect.width;
        const height = isFullScreen ? window.innerHeight : rect.height;

        canvasEl.width = width * dpr;
        canvasEl.height = height * dpr;
        ctx.scale(dpr, dpr);

        const startX = width * originX;
        const startY = height * originY;

        const particles = [];

        // Helper to draw a 5-pointed star
        function drawStar(c, cx, cy, spikes, outerRadius, innerRadius) {
            let rot = (Math.PI / 2) * 3;
            let x = cx;
            let y = cy;
            const step = Math.PI / spikes;

            c.beginPath();
            c.moveTo(cx, cy - outerRadius);
            for (let i = 0; i < spikes; i++) {
                x = cx + Math.cos(rot) * outerRadius;
                y = cy + Math.sin(rot) * outerRadius;
                c.lineTo(x, y);
                rot += step;

                x = cx + Math.cos(rot) * innerRadius;
                y = cy + Math.sin(rot) * innerRadius;
                c.lineTo(x, y);
                rot += step;
            }
            c.lineTo(cx, cy - outerRadius);
            c.closePath();
            c.fill();
        }

        if (style === 'fireworks') {
            const bursts = isFullScreen ? [
                { x: width * 0.5, y: height * 0.3, delay: 0 },
                { x: width * 0.22, y: height * 0.38, delay: 10 },
                { x: width * 0.78, y: height * 0.35, delay: 16 },
                { x: width * 0.38, y: height * 0.22, delay: 24 },
                { x: width * 0.64, y: height * 0.26, delay: 30 }
            ] : [
                { x: width * 0.5, y: height * 0.35, delay: 0 },
                { x: width * 0.28, y: height * 0.42, delay: 12 },
                { x: width * 0.72, y: height * 0.38, delay: 18 }
            ];

            bursts.forEach(burst => {
                const burstColor = colors[Math.floor(Math.random() * colors.length)];
                const burstCount = Math.floor((isFullScreen ? Math.max(count, 160) : count) / bursts.length);
                for (let i = 0; i < burstCount; i++) {
                    const angle = (Math.PI * 2 * (i / burstCount)) + ((Math.random() - 0.5) * 0.2);
                    const speedVal = ((isFullScreen ? 4.5 : 3.5) + Math.random() * 9.5) * speed;
                    particles.push({
                        type: 'firework',
                        x: burst.x,
                        y: burst.y,
                        vx: Math.cos(angle) * speedVal,
                        vy: Math.sin(angle) * speedVal,
                        color: Math.random() > 0.3 ? burstColor : '#ffffff',
                        size: (isFullScreen ? 3.0 : 2.5) + Math.random() * 4.0,
                        opacity: 1,
                        decay: ((isFullScreen ? 0.009 : 0.014) + Math.random() * 0.014) * Math.min(2.0, Math.max(0.6, Math.sqrt(speed))),
                        delay: burst.delay,
                        trail: []
                    });
                }
            });
        } else if (style === 'emojis' || style === 'reactions') {
            const emojis = opts.emojis || ['❤️', '🔥', '🚀', '🎉', '⭐', '🏆', '💎', '👏', '🥳', '✨'];
            const fontSizes = isFullScreen ? [28, 34, 40, 48] : [24, 28, 32, 36];
            const particleCount = isFullScreen ? Math.max(count, 160) : count;
            const staggerDuration = isFullScreen ? 45 : Math.max(8, Math.min(25, Math.floor((particleCount * 0.2) / Math.max(0.4, speed))));

            for (let i = 0; i < particleCount; i++) {
                const size = fontSizes[Math.floor(Math.random() * fontSizes.length)];

                // FULL SCREEN: Spread across 100% of screen width from edge to edge
                const spreadX = isFullScreen
                    ? Math.random() * width
                    : ((opts.x !== undefined && Math.abs(opts.x - 0.5) > 0.3)
                        ? startX + (Math.random() - 0.5) * (width * 0.7)
                        : width * (0.05 + Math.random() * 0.9));

                // Spawn at the bottom edge
                const spawnY = height + Math.random() * (isFullScreen ? 60 : 30);

                // Fast, energetic upward launch
                const baseVy = isFullScreen ? -(6.5 + Math.random() * 7.5) : -(4.5 + Math.random() * 5.5);
                const vy = baseVy * speed;
                const vx = (Math.random() - 0.5) * 2.5;

                const swayAmp = isFullScreen ? (20 + Math.random() * 30) : (16 + Math.random() * 22);
                const swayFreq = (0.05 + Math.random() * 0.04) * Math.min(2.5, Math.sqrt(speed));
                const phase = Math.random() * Math.PI * 2;

                const delay = Math.floor((i / particleCount) * staggerDuration) + Math.floor(Math.random() * 4);

                // Decay calibrated to guarantee particles float ALL THE WAY through the top of the screen
                const travelFrames = height / Math.abs(vy);
                const decay = 1.0 / (travelFrames + (isFullScreen ? 50 : 30));

                particles.push({
                    type: 'emoji',
                    emoji: emojis[Math.floor(Math.random() * emojis.length)],
                    baseX: spreadX,
                    x: spreadX,
                    y: spawnY,
                    vx: vx,
                    vy: vy,
                    swayAmp: swayAmp,
                    swayFreq: swayFreq,
                    phase: phase,
                    size: size,
                    font: `${size}px "Segoe UI Emoji", "Apple Color Emoji", "Noto Color Emoji", sans-serif`,
                    opacity: 1,
                    lifeOpacity: 1,
                    decay: decay,
                    scale: 0.3,
                    delay: delay
                });
            }
        } else if (style === 'coins') {
            const coinCount = isFullScreen ? Math.max(count, 150) : count;
            for (let i = 0; i < coinCount; i++) {
                particles.push({
                    type: 'coin',
                    x: width * (0.05 + Math.random() * 0.9),
                    y: -30 - (Math.random() * height * 0.5),
                    vx: ((Math.random() - 0.5) * 4) * speed,
                    vy: ((isFullScreen ? 3 : 2) + Math.random() * 5) * speed,
                    size: (isFullScreen ? 20 : 16) + Math.random() * 10,
                    flipAngle: Math.random() * Math.PI * 2,
                    flipSpeed: (0.08 + Math.random() * 0.14) * Math.sqrt(speed),
                    bounces: 0,
                    maxBounces: 2 + Math.floor(Math.random() * 2),
                    floorY: height * (0.88 + Math.random() * 0.08),
                    opacity: 1,
                    decay: (isFullScreen ? 0.004 : 0.006) + Math.random() * 0.006
                });
            }
        } else if (style === 'stars') {
            const starCount = isFullScreen ? Math.max(count, 140) : count;
            for (let i = 0; i < starCount; i++) {
                const angle = Math.random() * Math.PI * 2;
                const speedVal = ((isFullScreen ? 6 : 4) + Math.random() * 12) * speed;
                particles.push({
                    type: 'star',
                    x: isFullScreen ? width * 0.5 : startX,
                    y: isFullScreen ? height * 0.45 : startY,
                    vx: Math.cos(angle) * speedVal,
                    vy: (Math.sin(angle) * speedVal) - 2,
                    color: Math.random() > 0.4 ? '#fbbf24' : (Math.random() > 0.5 ? '#f59e0b' : '#ffffff'),
                    size: (isFullScreen ? 8 : 6) + Math.random() * 8,
                    rotation: Math.random() * 360,
                    rotationSpeed: (Math.random() - 0.5) * 10 * speed,
                    opacity: 1,
                    decay: (isFullScreen ? 0.008 : 0.013) + Math.random() * 0.012
                });
            }
        } else if (style === 'streamers') {
            const streamerCount = isFullScreen ? Math.min(count, 70) : Math.min(count, 45);
            for (let i = 0; i < streamerCount; i++) {
                particles.push({
                    type: 'streamer',
                    x: width * (0.02 + Math.random() * 0.96),
                    y: -50 - Math.random() * 150,
                    vy: ((isFullScreen ? 3.0 : 2.2) + Math.random() * 3.5) * speed,
                    color: colors[Math.floor(Math.random() * colors.length)],
                    width: (isFullScreen ? 8 : 6) + Math.random() * 5,
                    length: (isFullScreen ? 60 : 40) + Math.random() * 50,
                    waveAngle: Math.random() * Math.PI * 2,
                    waveSpeed: (0.06 + Math.random() * 0.06) * Math.sqrt(speed),
                    opacity: 1,
                    decay: (isFullScreen ? 0.004 : 0.007) + Math.random() * 0.006
                });
            }
        } else {
            // Default: Confetti Cannons (dual-angled burst with 3D fluttering ribbons)
            const confettiCount = isFullScreen ? Math.max(count, 150) : count;
            for (let i = 0; i < confettiCount; i++) {
                const fromLeft = Math.random() > 0.5;
                const angle = fromLeft
                    ? (-Math.PI * 0.25) + ((Math.random() - 0.5) * 0.8)
                    : (-Math.PI * 0.75) + ((Math.random() - 0.5) * 0.8);
                const speedVal = ((isFullScreen ? 9 : 7) + Math.random() * 14) * speed;

                particles.push({
                    type: 'confetti',
                    x: fromLeft ? (isFullScreen ? width * 0.04 : width * 0.1) : (isFullScreen ? width * 0.96 : width * 0.9),
                    y: height * 0.92,
                    vx: Math.cos(angle) * speedVal,
                    vy: Math.sin(angle) * speedVal,
                    color: colors[Math.floor(Math.random() * colors.length)],
                    size: (isFullScreen ? 9 : 7) + Math.random() * 8,
                    rotX: Math.random() * Math.PI,
                    rotY: Math.random() * Math.PI,
                    rotSpeedX: ((Math.random() - 0.5) * 0.15) * speed,
                    rotSpeedY: ((Math.random() - 0.5) * 0.15) * speed,
                    opacity: 1,
                    decay: (isFullScreen ? 0.007 : 0.01) + Math.random() * 0.012
                });
            }
        }

        let animationFrameId;
        let tick = 0;

        const animate = () => {
            ctx.clearRect(0, 0, width, height);
            let active = false;
            tick++;

            for (let i = 0; i < particles.length; i++) {
                const p = particles[i];

                if (p.delay && p.delay > 0) {
                    p.delay--;
                    active = true;
                    continue;
                }

                if (p.opacity <= 0) continue;
                active = true;

                if (p.type === 'firework') {
                    p.x += p.vx;
                    p.y += p.vy;
                    p.vy += 0.12; // gentle spark gravity
                    p.vx *= 0.96; // drag
                    p.vy *= 0.96;
                    p.opacity -= p.decay;

                    ctx.save();
                    ctx.globalAlpha = Math.max(0, p.opacity);
                    ctx.fillStyle = p.color;
                    ctx.beginPath();
                    ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
                    ctx.fill();
                    ctx.restore();
                } else if (p.type === 'emoji') {
                    p.phase += p.swayFreq;
                    p.baseX += p.vx;
                    p.vx *= 0.95; // lateral velocity decay into harmonic sway
                    p.x = p.baseX + Math.sin(p.phase) * p.swayAmp;
                    p.y += p.vy;
                    p.vy *= 0.99; // slight air drag while maintaining high-speed ascent

                    // Snappy pop-in scale
                    if (p.scale < 1.0) {
                        p.scale = Math.min(1.0, p.scale + 0.14);
                    }

                    // Natural decay & graceful top fade
                    p.lifeOpacity -= p.decay;
                    let currentAlpha = p.lifeOpacity;

                    // Fade out as it nears the top 12% (fullscreen) or 20% (stage) of canvas
                    const fadeThreshold = height * (isFullScreen ? 0.12 : 0.2);
                    if (p.y < fadeThreshold) {
                        const topFade = Math.max(0, p.y / fadeThreshold);
                        currentAlpha = Math.min(currentAlpha, topFade);
                    }
                    p.opacity = currentAlpha;

                    if (p.opacity > 0 && p.y > -80) {
                        ctx.save();
                        ctx.globalAlpha = Math.max(0, Math.min(1, p.opacity));
                        ctx.translate(p.x, p.y);
                        if (p.scale < 0.98) {
                            ctx.scale(p.scale, p.scale);
                        }
                        ctx.font = p.font;
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'middle';
                        ctx.fillText(p.emoji, 0, 0);
                        ctx.restore();
                    }
                } else if (p.type === 'coin') {
                    p.x += p.vx;
                    p.y += p.vy;
                    p.vy += 0.35; // coin gravity
                    p.flipAngle += p.flipSpeed;

                    if (p.y >= p.floorY && p.bounces < p.maxBounces) {
                        p.vy = -p.vy * 0.55;
                        p.vx *= 0.75;
                        p.bounces++;
                    } else if (p.bounces >= p.maxBounces) {
                        p.opacity -= p.decay * 3;
                    }

                    ctx.save();
                    ctx.globalAlpha = Math.max(0, p.opacity);
                    ctx.translate(p.x, p.y);
                    const scaleX = Math.cos(p.flipAngle);
                    ctx.scale(scaleX, 1);

                    // Outer gold rim
                    ctx.fillStyle = '#fbbf24';
                    ctx.beginPath();
                    ctx.arc(0, 0, p.size, 0, Math.PI * 2);
                    ctx.fill();

                    // Inner coin surface
                    ctx.fillStyle = '#f59e0b';
                    ctx.beginPath();
                    ctx.arc(0, 0, p.size * 0.8, 0, Math.PI * 2);
                    ctx.fill();

                    // Currency star/dollar imprint
                    ctx.fillStyle = '#ffffff';
                    ctx.font = `bold ${Math.round(p.size * 0.9)}px sans-serif`;
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';
                    ctx.fillText('$', 0, 0);

                    ctx.restore();
                } else if (p.type === 'star') {
                    p.x += p.vx;
                    p.y += p.vy;
                    p.vy += 0.18;
                    p.vx *= 0.97;
                    p.rotation += p.rotationSpeed;
                    p.opacity -= p.decay;

                    ctx.save();
                    ctx.globalAlpha = Math.max(0, p.opacity);
                    ctx.translate(p.x, p.y);
                    ctx.rotate((p.rotation * Math.PI) / 180);
                    ctx.fillStyle = p.color;
                    drawStar(ctx, 0, 0, 5, p.size, p.size * 0.45);
                    ctx.restore();
                } else if (p.type === 'streamer') {
                    p.y += p.vy;
                    p.waveAngle += p.waveSpeed;
                    p.opacity -= p.decay;

                    ctx.save();
                    ctx.globalAlpha = Math.max(0, p.opacity);
                    ctx.strokeStyle = p.color;
                    ctx.lineWidth = p.width;
                    ctx.lineCap = 'round';

                    ctx.beginPath();
                    const segments = 6;
                    const segLen = p.length / segments;
                    for (let s = 0; s < segments; s++) {
                        const sy = p.y + s * segLen;
                        const sx = p.x + Math.sin(p.waveAngle + s * 0.8) * 18;
                        if (s === 0) ctx.moveTo(sx, sy);
                        else ctx.lineTo(sx, sy);
                    }
                    ctx.stroke();
                    ctx.restore();
                } else {
                    // Confetti cannon particle
                    p.x += p.vx;
                    p.y += p.vy;
                    p.vy += 0.28; // gravity
                    p.vx *= 0.98; // air drag
                    p.rotX += p.rotSpeedX;
                    p.rotY += p.rotSpeedY;
                    p.opacity -= p.decay;

                    ctx.save();
                    ctx.globalAlpha = Math.max(0, p.opacity);
                    ctx.translate(p.x, p.y);
                    ctx.scale(Math.cos(p.rotX), Math.sin(p.rotY));
                    ctx.fillStyle = p.color;
                    ctx.fillRect(-p.size / 2, -p.size / 3, p.size, p.size * 0.7);
                    ctx.restore();
                }
            }

            if (active) {
                animationFrameId = requestAnimationFrame(animate);
            } else {
                ctx.clearRect(0, 0, width, height);
                cancelAnimationFrame(animationFrameId);
            }
        };

        animate();
    }
};

window.blazorConfetti = window.blazorCelebration;

/* ==========================================================================
   Clipboard Helper
   ========================================================================== */
window.blazorClipboard = {
    copyText: async function (text) {
        if (!text) return false;
        try {
            if (navigator.clipboard && navigator.clipboard.writeText) {
                await navigator.clipboard.writeText(text);
                return true;
            }
        } catch (_) { }

        try {
            const textArea = document.createElement("textarea");
            textArea.value = text;
            textArea.style.position = "fixed";
            textArea.style.left = "-999999px";
            textArea.style.top = "-999999px";
            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();
            const successful = document.execCommand('copy');
            document.body.removeChild(textArea);
            return successful;
        } catch (_) {
            return false;
        }
    }
};








// -----------------------------------------------------------------------------
// Floating Unclipped Popover Engine (TwPopover)
// Breaks out of parent overflow-hidden / overflow-auto clipping containers
// -----------------------------------------------------------------------------
window.twPopover = {
    activePopovers: new Map(),

    position: function (anchorId, popoverId, placementStr, showArrow, arrowId, dotNetRef) {
        const anchorEl = document.getElementById(anchorId);
        const popoverEl = document.getElementById(popoverId);
        if (!anchorEl || !popoverEl) return;

        const update = () => {
            const a = document.getElementById(anchorId);
            const p = document.getElementById(popoverId);
            if (!a || !p) {
                this.cleanup(popoverId);
                return;
            }

            const anchorRect = a.getBoundingClientRect();
            if (anchorRect.width === 0 && anchorRect.height === 0) return;

            const popoverRect = p.getBoundingClientRect();
            const popW = popoverRect.width || 288;
            const popH = popoverRect.height || 200;
            const offset = 8;
            const vw = window.innerWidth;
            const vh = window.innerHeight;

            let placement = (placementStr || 'Bottom').toLowerCase();

            // Auto-flip if overflowing viewport boundaries
            if (placement.startsWith('top') && anchorRect.top - popH - offset < 8) {
                placement = placement.replace('top', 'bottom');
            } else if (placement.startsWith('bottom') && anchorRect.bottom + popH + offset > vh - 8) {
                placement = placement.replace('bottom', 'top');
            } else if (placement === 'left' && anchorRect.left - popW - offset < 8) {
                placement = 'right';
            } else if (placement === 'right' && anchorRect.right + popW + offset > vw - 8) {
                placement = 'left';
            }

            let top = 0;
            let left = 0;

            switch (placement) {
                case 'top':
                    top = anchorRect.top - popH - offset;
                    left = anchorRect.left + (anchorRect.width / 2) - (popW / 2);
                    break;
                case 'topstart':
                    top = anchorRect.top - popH - offset;
                    left = anchorRect.left;
                    break;
                case 'topend':
                    top = anchorRect.top - popH - offset;
                    left = anchorRect.right - popW;
                    break;
                case 'bottom':
                    top = anchorRect.bottom + offset;
                    left = anchorRect.left + (anchorRect.width / 2) - (popW / 2);
                    break;
                case 'bottomstart':
                    top = anchorRect.bottom + offset;
                    left = anchorRect.left;
                    break;
                case 'bottomend':
                    top = anchorRect.bottom + offset;
                    left = anchorRect.right - popW;
                    break;
                case 'left':
                    top = anchorRect.top + (anchorRect.height / 2) - (popH / 2);
                    left = anchorRect.left - popW - offset;
                    break;
                case 'right':
                    top = anchorRect.top + (anchorRect.height / 2) - (popH / 2);
                    left = anchorRect.right + offset;
                    break;
                default:
                    top = anchorRect.bottom + offset;
                    left = anchorRect.left + (anchorRect.width / 2) - (popW / 2);
                    break;
            }

            // Viewport containment clamp
            left = Math.max(8, Math.min(left, vw - popW - 8));
            top = Math.max(8, Math.min(top, vh - popH - 8));

            p.style.position = 'fixed';
            p.style.top = Math.round(top) + 'px';
            p.style.left = Math.round(left) + 'px';
            p.style.zIndex = '9999';
            p.style.margin = '0';
            p.style.transform = 'none';

            // Arrow placement pointing at anchor center
            if (showArrow && arrowId) {
                const arr = document.getElementById(arrowId);
                if (arr) {
                    if (placement.startsWith('top')) {
                        arr.style.top = '100%';
                        arr.style.bottom = 'auto';
                        arr.style.marginTop = '-5px';
                        arr.style.marginBottom = 'auto';
                        const arrowX = Math.max(16, Math.min(popW - 16, (anchorRect.left + anchorRect.width / 2) - left));
                        arr.style.left = Math.round(arrowX) + 'px';
                        arr.style.right = 'auto';
                        arr.style.transform = 'translateX(-50%) rotate(45deg)';
                    } else if (placement.startsWith('bottom')) {
                        arr.style.top = 'auto';
                        arr.style.bottom = '100%';
                        arr.style.marginTop = 'auto';
                        arr.style.marginBottom = '-5px';
                        const arrowX = Math.max(16, Math.min(popW - 16, (anchorRect.left + anchorRect.width / 2) - left));
                        arr.style.left = Math.round(arrowX) + 'px';
                        arr.style.right = 'auto';
                        arr.style.transform = 'translateX(-50%) rotate(45deg)';
                    } else if (placement === 'left') {
                        arr.style.left = '100%';
                        arr.style.right = 'auto';
                        arr.style.marginLeft = '-5px';
                        arr.style.marginRight = 'auto';
                        const arrowY = Math.max(16, Math.min(popH - 16, (anchorRect.top + anchorRect.height / 2) - top));
                        arr.style.top = Math.round(arrowY) + 'px';
                        arr.style.bottom = 'auto';
                        arr.style.transform = 'translateY(-50%) rotate(45deg)';
                    } else if (placement === 'right') {
                        arr.style.left = 'auto';
                        arr.style.right = '100%';
                        arr.style.marginLeft = 'auto';
                        arr.style.marginRight = '-5px';
                        const arrowY = Math.max(16, Math.min(popH - 16, (anchorRect.top + anchorRect.height / 2) - top));
                        arr.style.top = Math.round(arrowY) + 'px';
                        arr.style.bottom = 'auto';
                        arr.style.transform = 'translateY(-50%) rotate(45deg)';
                    }
                }
            }
        };

        update();
        requestAnimationFrame(update);

        this.cleanup(popoverId);

        const onScrollOrResize = () => update();
        window.addEventListener('scroll', onScrollOrResize, { passive: true, capture: true });
        window.addEventListener('resize', onScrollOrResize, { passive: true });

        // Global Escape key listener
        const onKeyDown = (e) => {
            if (e.key === 'Escape') {
                if (dotNetRef) {
                    try {
                        dotNetRef.invokeMethodAsync('CloseFromJs');
                    } catch (_) {}
                }
            }
        };
        window.addEventListener('keydown', onKeyDown);

        this.activePopovers.set(popoverId, { onScrollOrResize, onKeyDown });
    },

    cleanup: function (popoverId) {
        if (this.activePopovers.has(popoverId)) {
            const item = this.activePopovers.get(popoverId);
            if (item) {
                if (item.onScrollOrResize) {
                    window.removeEventListener('scroll', item.onScrollOrResize, { capture: true });
                    window.removeEventListener('resize', item.onScrollOrResize);
                }
                if (item.onKeyDown) {
                    window.removeEventListener('keydown', item.onKeyDown);
                }
            }
            this.activePopovers.delete(popoverId);
        }
    }
};


window.clipboardHelper = {
    copyText: async function (text) {
        if (navigator.clipboard && navigator.clipboard.writeText) {
            try {
                await navigator.clipboard.writeText(text);
                return true;
            } catch (err) {
                // fall through to fallback
            }
        }
        try {
            const textArea = document.createElement('textarea');
            textArea.value = text;
            textArea.style.position = 'fixed';
            textArea.style.top = '0';
            textArea.style.left = '0';
            textArea.style.opacity = '0';
            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();
            const success = document.execCommand('copy');
            document.body.removeChild(textArea);
            return success;
        } catch (e) {
            console.error('Clipboard copy failed:', e);
            return false;
        }
    }
};


window.svgExporter = {
    exportSvgElement: function (svgEl, filename) {
        if (!svgEl) return false;
        try {
            const clone = svgEl.cloneNode(true);
            if (!clone.getAttribute('xmlns')) {
                clone.setAttribute('xmlns', 'http://www.w3.org/2000/svg');
            }
            let svgString = clone.outerHTML;
            if (!svgString.startsWith('<?xml')) {
                svgString = '<?xml version="1.0" encoding="UTF-8"?>\n' + svgString;
            }
            return window.pdfHelper.downloadText(filename || 'word-balloon.svg', 'image/svg+xml', svgString);
        } catch (e) {
            console.error('Failed to export SVG element:', e);
            return false;
        }
    }
};
