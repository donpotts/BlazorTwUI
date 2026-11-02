/**
 * BlazorTwUI — Freely Distributable Procedural Sound FX Synthesizer
 * 100% Royalty-Free, Zero-Asset Web Audio API Synthesis
 * Free to distribute anywhere under MIT / Public Domain (No MP3/WAV copyright risks).
 */
(function () {
    let ctx = null;
    let isMuted = false;
    let masterVolume = 0.65;

    function getAudioContext() {
        if (!ctx) {
            ctx = new (window.AudioContext || window.webkitAudioContext)();
        }
        if (ctx && ctx.state === 'suspended') {
            ctx.resume();
        }
        return ctx;
    }

    // Helper: generate a short white noise buffer for pops, explosions & whooshes
    function createNoiseBuffer(audioCtx, durationSec) {
        const bufferSize = Math.floor(audioCtx.sampleRate * durationSec);
        const buffer = audioCtx.createBuffer(1, bufferSize, audioCtx.sampleRate);
        const data = buffer.getChannelData(0);
        for (let i = 0; i < bufferSize; i++) {
            data[i] = Math.random() * 2 - 1;
        }
        return buffer;
    }

    window.blazorSoundFx = {
        setMuted: function (muted) {
            isMuted = !!muted;
        },

        getMuted: function () {
            return isMuted;
        },

        setVolume: function (vol) {
            masterVolume = Math.max(0, Math.min(1, vol));
        },

        getVolume: function () {
            return masterVolume;
        },

        // Master playback router
        play: function (style, localVol) {
            if (isMuted) return;
            const volume = (typeof localVol === 'number' ? localVol : 1.0) * masterVolume;
            if (volume <= 0.001) return;

            const audioCtx = getAudioContext();
            if (!audioCtx) return;

            const s = (style || 'confetti').toLowerCase();

            try {
                if (s === 'emojis' || s === 'reactions') {
                    this.playFloatingReactions(volume);
                } else if (s === 'fireworks') {
                    this.playFireworks(volume);
                } else if (s === 'coins') {
                    this.playCoinCascade(volume);
                } else if (s === 'stars') {
                    this.playStarburst(volume);
                } else if (s === 'streamers') {
                    this.playStreamers(volume);
                } else if (s === 'fanfare' || s === 'victory') {
                    this.playVictoryFanfare(volume);
                } else if (s === 'partyhorn' || s === 'birthday') {
                    this.playPartyHorn(volume);
                } else {
                    this.playConfetti(volume);
                }
            } catch (err) {
                // Silently handle audio context restrictions on un-interacted tabs
            }
        },

        // 1. Confetti Pop: Champagne cork pop + paper flutter
        playConfetti: function (vol = masterVolume) {
            if (isMuted) return;
            const audioCtx = getAudioContext();
            const now = audioCtx.currentTime;

            // Cork pop: filtered noise transient
            const noise = audioCtx.createBufferSource();
            noise.buffer = createNoiseBuffer(audioCtx, 0.05);

            const filter = audioCtx.createBiquadFilter();
            filter.type = 'bandpass';
            filter.frequency.setValueAtTime(1100, now);
            filter.Q.setValueAtTime(3.0, now);

            const noiseGain = audioCtx.createGain();
            noiseGain.gain.setValueAtTime(0.7 * vol, now);
            noiseGain.gain.exponentialRampToValueAtTime(0.001, now + 0.05);

            noise.connect(filter);
            filter.connect(noiseGain);
            noiseGain.connect(audioCtx.destination);
            noise.start(now);

            // Pop pitch sweep
            const osc = audioCtx.createOscillator();
            const oscGain = audioCtx.createGain();
            osc.type = 'sine';
            osc.frequency.setValueAtTime(420, now);
            osc.frequency.exponentialRampToValueAtTime(160, now + 0.08);

            oscGain.gain.setValueAtTime(0.45 * vol, now);
            oscGain.gain.exponentialRampToValueAtTime(0.001, now + 0.08);

            osc.connect(oscGain);
            oscGain.connect(audioCtx.destination);
            osc.start(now);
            osc.stop(now + 0.08);
        },

        // 2. Fireworks Rockets: Whistle ascent + Sub-bass Boom
        playFireworks: function (vol = masterVolume) {
            if (isMuted) return;
            const audioCtx = getAudioContext();
            const now = audioCtx.currentTime;

            // Rising rocket whistle
            const whistle = audioCtx.createOscillator();
            const whistleGain = audioCtx.createGain();
            whistle.type = 'sine';
            whistle.frequency.setValueAtTime(450, now);
            whistle.frequency.exponentialRampToValueAtTime(1450, now + 0.16);

            whistleGain.gain.setValueAtTime(0.02 * vol, now);
            whistleGain.gain.linearRampToValueAtTime(0.28 * vol, now + 0.14);
            whistleGain.gain.exponentialRampToValueAtTime(0.001, now + 0.18);

            whistle.connect(whistleGain);
            whistleGain.connect(audioCtx.destination);
            whistle.start(now);
            whistle.stop(now + 0.18);

            // Sub-bass explosion boom (detuned dual sine + filtered low-pass rumble)
            setTimeout(() => {
                const boomTime = audioCtx.currentTime;
                const boomOsc = audioCtx.createOscillator();
                const boomGain = audioCtx.createGain();

                boomOsc.type = 'triangle';
                boomOsc.frequency.setValueAtTime(85, boomTime);
                boomOsc.frequency.exponentialRampToValueAtTime(32, boomTime + 0.45);

                boomGain.gain.setValueAtTime(0.7 * vol, boomTime);
                boomGain.gain.exponentialRampToValueAtTime(0.001, boomTime + 0.5);

                boomOsc.connect(boomGain);
                boomGain.connect(audioCtx.destination);
                boomOsc.start(boomTime);
                boomOsc.stop(boomTime + 0.5);

                // Noise rumble
                const rumble = audioCtx.createBufferSource();
                rumble.buffer = createNoiseBuffer(audioCtx, 0.4);
                const rumbleFilter = audioCtx.createBiquadFilter();
                rumbleFilter.type = 'lowpass';
                rumbleFilter.frequency.setValueAtTime(260, boomTime);

                const rumbleGain = audioCtx.createGain();
                rumbleGain.gain.setValueAtTime(0.5 * vol, boomTime);
                rumbleGain.gain.exponentialRampToValueAtTime(0.001, boomTime + 0.4);

                rumble.connect(rumbleFilter);
                rumbleFilter.connect(rumbleGain);
                rumbleGain.connect(audioCtx.destination);
                rumble.start(boomTime);
            }, 160);
        },

        // 3. Floating Reactions: Warm pentatonic bubbly chime arpeggio (C5 -> E5 -> G5 -> C6)
        playFloatingReactions: function (vol = masterVolume) {
            if (isMuted) return;
            const audioCtx = getAudioContext();
            const now = audioCtx.currentTime;

            // Pentatonic ascending notes: C5, E5, G5, C6
            const freqs = [523.25, 659.25, 783.99, 1046.50];

            freqs.forEach((freq, idx) => {
                const noteTime = now + (idx * 0.055);
                const osc = audioCtx.createOscillator();
                const gain = audioCtx.createGain();

                // Sine with warm subtle vibrato for floating sensation
                osc.type = 'sine';
                osc.frequency.setValueAtTime(freq, noteTime);
                osc.frequency.linearRampToValueAtTime(freq * 1.015, noteTime + 0.15);

                gain.gain.setValueAtTime(0, noteTime);
                gain.gain.linearRampToValueAtTime(0.28 * vol, noteTime + 0.012);
                gain.gain.exponentialRampToValueAtTime(0.001, noteTime + 0.28);

                osc.connect(gain);
                gain.connect(audioCtx.destination);
                osc.start(noteTime);
                osc.stop(noteTime + 0.28);
            });
        },

        // 4. Coins Jackpot: Metallic multi-coin cascade chime
        playCoinCascade: function (vol = masterVolume) {
            if (isMuted) return;
            const audioCtx = getAudioContext();
            const now = audioCtx.currentTime;

            // Rapid cluster of high metallic coin rings
            const coinFreqs = [1860, 2240, 1980, 2480, 2120, 2760, 2320];

            coinFreqs.forEach((freq, idx) => {
                const coinTime = now + (idx * 0.045);
                const osc = audioCtx.createOscillator();
                const gain = audioCtx.createGain();

                osc.type = 'sine';
                osc.frequency.setValueAtTime(freq, coinTime);

                gain.gain.setValueAtTime(0.25 * vol, coinTime);
                gain.gain.exponentialRampToValueAtTime(0.001, coinTime + 0.14);

                osc.connect(gain);
                gain.connect(audioCtx.destination);
                osc.start(coinTime);
                osc.stop(coinTime + 0.14);
            });
        },

        // 5. Cosmic Starburst: High crystalline shimmer
        playStarburst: function (vol = masterVolume) {
            if (isMuted) return;
            const audioCtx = getAudioContext();
            const now = audioCtx.currentTime;

            // Glockenspiel crystalline chime (E6, A6, E7)
            const starFreqs = [1318.51, 1760.00, 2637.02];

            starFreqs.forEach((freq, idx) => {
                const t = now + (idx * 0.035);
                const osc = audioCtx.createOscillator();
                const gain = audioCtx.createGain();

                osc.type = 'sine';
                osc.frequency.setValueAtTime(freq, t);

                gain.gain.setValueAtTime(0.22 * vol, t);
                gain.gain.exponentialRampToValueAtTime(0.001, t + 0.55);

                osc.connect(gain);
                gain.connect(audioCtx.destination);
                osc.start(t);
                osc.stop(t + 0.55);
            });
        },

        // 6. Carnival Streamers: Air whoosh & whip flutter
        playStreamers: function (vol = masterVolume) {
            if (isMuted) return;
            const audioCtx = getAudioContext();
            const now = audioCtx.currentTime;

            const noise = audioCtx.createBufferSource();
            noise.buffer = createNoiseBuffer(audioCtx, 0.35);

            const filter = audioCtx.createBiquadFilter();
            filter.type = 'bandpass';
            filter.frequency.setValueAtTime(450, now);
            filter.frequency.linearRampToValueAtTime(1800, now + 0.15);
            filter.frequency.linearRampToValueAtTime(600, now + 0.35);
            filter.Q.setValueAtTime(2.5, now);

            const gain = audioCtx.createGain();
            gain.gain.setValueAtTime(0.01 * vol, now);
            gain.gain.linearRampToValueAtTime(0.4 * vol, now + 0.12);
            gain.gain.exponentialRampToValueAtTime(0.001, now + 0.35);

            noise.connect(filter);
            filter.connect(gain);
            gain.connect(audioCtx.destination);
            noise.start(now);
        },

        // 7. Victory Fanfare: Grand multi-voice triumphal chord arpeggio
        playVictoryFanfare: function (vol = masterVolume) {
            if (isMuted) return;
            const audioCtx = getAudioContext();
            const now = audioCtx.currentTime;

            // Notes: C4, G4, C5, E5, G5
            const fanfare = [
                { f: 261.63, delay: 0.00, dur: 0.14 },
                { f: 392.00, delay: 0.10, dur: 0.14 },
                { f: 523.25, delay: 0.20, dur: 0.18 },
                { f: 659.25, delay: 0.32, dur: 0.20 },
                { f: 783.99, delay: 0.45, dur: 0.55 }
            ];

            fanfare.forEach(note => {
                const t = now + note.delay;
                const osc = audioCtx.createOscillator();
                const gain = audioCtx.createGain();
                const filter = audioCtx.createBiquadFilter();

                // Sawtooth filtered for rich brass-like brilliance
                osc.type = 'sawtooth';
                osc.frequency.setValueAtTime(note.f, t);

                filter.type = 'lowpass';
                filter.frequency.setValueAtTime(1400, t);

                gain.gain.setValueAtTime(0, t);
                gain.gain.linearRampToValueAtTime(0.32 * vol, t + 0.02);
                gain.gain.exponentialRampToValueAtTime(0.001, t + note.dur);

                osc.connect(filter);
                filter.connect(gain);
                gain.connect(audioCtx.destination);
                osc.start(t);
                osc.stop(t + note.dur);
            });
        },

        // 8. Birthday Party Horn: Celebratory buzzing party horn
        playPartyHorn: function (vol = masterVolume) {
            if (isMuted) return;
            const audioCtx = getAudioContext();
            const now = audioCtx.currentTime;

            const osc = audioCtx.createOscillator();
            const gain = audioCtx.createGain();
            const filter = audioCtx.createBiquadFilter();

            osc.type = 'sawtooth';
            osc.frequency.setValueAtTime(320, now);
            osc.frequency.linearRampToValueAtTime(370, now + 0.12);
            osc.frequency.linearRampToValueAtTime(340, now + 0.4);

            filter.type = 'bandpass';
            filter.frequency.setValueAtTime(800, now);
            filter.Q.setValueAtTime(4.0, now);

            gain.gain.setValueAtTime(0, now);
            gain.gain.linearRampToValueAtTime(0.35 * vol, now + 0.03);
            gain.gain.exponentialRampToValueAtTime(0.001, now + 0.42);

            osc.connect(filter);
            filter.connect(gain);
            gain.connect(audioCtx.destination);
            osc.start(now);
            osc.stop(now + 0.42);
        }
    };
})();
