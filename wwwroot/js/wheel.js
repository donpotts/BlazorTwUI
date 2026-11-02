// Wheel Physics, Sound & Rotation Engine (inspired by MyWheel)
let currentWheelRotation = 0;
let wheelAnimationStart = 0;
let wheelAnimationDuration = 0;
let wheelStartRotation = 0;
let wheelTargetRotation = 0;
let wheelAnimationId = null;
let tickingInterval = null;
let audioContext = null;
let isAudioInitialized = false;

window.initWheelRotation = (initialRotation = 0) => {
    currentWheelRotation = initialRotation % 360;
    if (currentWheelRotation < 0) currentWheelRotation += 360;

    const wheel = document.getElementById('wheel');
    if (wheel) {
        wheel.classList.remove('spinning');
        wheel.style.animation = 'none';
        void wheel.offsetWidth;
        wheel.style.transform = `rotate(${currentWheelRotation}deg)`;
        wheel.style.setProperty('--start-rotation', currentWheelRotation + 'deg');
        wheel.style.setProperty('--spin-rotation', currentWheelRotation + 'deg');
        wheel.style.setProperty('--spin-duration', '0ms');
    }
};

window.setWheelRotation = (rotation, duration = 5000) => {
    const wheel = document.getElementById('wheel');
    if (!wheel) return;

    if (wheelAnimationId) {
        cancelAnimationFrame(wheelAnimationId);
        wheelAnimationId = null;
    }
    stopTicking();

    wheelStartRotation = currentWheelRotation % 360;
    if (wheelStartRotation < 0) wheelStartRotation += 360;
    wheelTargetRotation = rotation;
    wheelAnimationDuration = duration;
    wheelAnimationStart = Date.now();

    // 1. Detach old animation & force browser reflow so new animation restarts 100% reliably
    wheel.classList.remove('spinning');
    wheel.style.animation = 'none';
    void wheel.offsetWidth;

    // 2. Set updated CSS custom properties for start and destination
    wheel.style.setProperty('--start-rotation', wheelStartRotation + 'deg');
    wheel.style.setProperty('--spin-rotation', rotation + 'deg');
    wheel.style.setProperty('--spin-duration', duration + 'ms');

    // 3. Clear inline animation override and attach spinning class
    wheel.style.animation = '';
    wheel.classList.add('spinning');

    // 4. Start RAF position tracker & audio
    startWheelAnimation();
    initAudioContext();
    startTickingSound(duration);
};

function startWheelAnimation() {
    function updateRotation() {
        const elapsed = Date.now() - wheelAnimationStart;
        const progress = Math.min(elapsed / wheelAnimationDuration, 1);

        // cubic-bezier(0.17, 0.67, 0.12, 0.99) - starts fast, slows down naturally
        const easedProgress = cubicBezier(progress, 0.17, 0.67, 0.12, 0.99);
        currentWheelRotation = wheelStartRotation + (wheelTargetRotation - wheelStartRotation) * easedProgress;

        if (progress < 1) {
            wheelAnimationId = requestAnimationFrame(updateRotation);
        } else {
            currentWheelRotation = wheelTargetRotation % 360;
            if (currentWheelRotation < 0) currentWheelRotation += 360;
            wheelAnimationId = null;
        }
    }

    if (wheelAnimationId) {
        cancelAnimationFrame(wheelAnimationId);
    }

    wheelAnimationId = requestAnimationFrame(updateRotation);
}

function cubicBezier(t, x1, y1, x2, y2) {
    const cx = 3 * x1;
    const bx = 3 * (x2 - x1) - cx;
    const ax = 1 - cx - bx;

    const cy = 3 * y1;
    const by = 3 * (y2 - y1) - cy;
    const ay = 1 - cy - by;

    return ((ax * t + bx) * t + cx) * t + ay * t * t * t + by * t * t + cy * t;
}

window.completeWheelSpin = (finalRotation) => {
    if (wheelAnimationId) {
        cancelAnimationFrame(wheelAnimationId);
        wheelAnimationId = null;
    }
    stopTicking();

    let norm = (finalRotation !== undefined ? finalRotation : currentWheelRotation) % 360;
    if (norm < 0) norm += 360;
    currentWheelRotation = norm;

    const wheel = document.getElementById('wheel');
    if (wheel) {
        wheel.classList.remove('spinning');
        wheel.style.animation = 'none';
        void wheel.offsetWidth;
        wheel.style.transform = `rotate(${norm}deg)`;
        wheel.style.setProperty('--start-rotation', norm + 'deg');
        wheel.style.setProperty('--spin-rotation', norm + 'deg');
        wheel.style.setProperty('--spin-duration', '0ms');
    }
};

window.stopWheelSpin = () => {
    if (wheelAnimationId) {
        cancelAnimationFrame(wheelAnimationId);
        wheelAnimationId = null;
    }
    stopTicking();

    let stoppedRotation = currentWheelRotation % 360;
    if (stoppedRotation < 0) {
        stoppedRotation += 360;
    }

    const wheel = document.getElementById('wheel');
    if (wheel) {
        wheel.classList.remove('spinning');
        wheel.style.animation = 'none';
        void wheel.offsetWidth;
        wheel.style.transform = `rotate(${stoppedRotation}deg)`;
        wheel.style.setProperty('--start-rotation', stoppedRotation + 'deg');
        wheel.style.setProperty('--spin-rotation', stoppedRotation + 'deg');
        wheel.style.setProperty('--spin-duration', '0ms');
    }

    currentWheelRotation = stoppedRotation;
    return stoppedRotation;
};

window.resetWheel = () => {
    if (wheelAnimationId) {
        cancelAnimationFrame(wheelAnimationId);
        wheelAnimationId = null;
    }
    stopTicking();

    currentWheelRotation = 0;
    wheelStartRotation = 0;
    wheelTargetRotation = 0;

    const wheel = document.getElementById('wheel');
    if (wheel) {
        wheel.classList.remove('spinning');
        wheel.style.animation = 'none';
        void wheel.offsetWidth;
        wheel.style.transform = 'rotate(0deg)';
        wheel.style.setProperty('--start-rotation', '0deg');
        wheel.style.setProperty('--spin-rotation', '0deg');
        wheel.style.setProperty('--spin-duration', '0ms');
    }

    return 0;
};

function initAudioContext() {
    if (!isAudioInitialized) {
        try {
            audioContext = new (window.AudioContext || window.webkitAudioContext)();
            isAudioInitialized = true;
        } catch (error) {
            // Audio context not supported
        }
    }

    if (audioContext && audioContext.state === 'suspended') {
        audioContext.resume();
    }
}

function startTickingSound(duration) {
    stopTicking();
    if (!audioContext) return;

    const tickingDuration = Math.max(500, duration - 800);
    const startTime = Date.now();
    let lastTickTime = 0;

    function scheduleNextTick() {
        const elapsed = Date.now() - startTime;
        const progress = elapsed / tickingDuration;

        if (elapsed >= tickingDuration || progress >= 1) {
            stopTicking();
            return;
        }

        // Calculate dynamic tick rate - starts fast (80ms), slows down (300ms)
        const baseTickRate = 80;
        const maxTickRate = 300;
        const tickRate = baseTickRate + (maxTickRate - baseTickRate) * (progress * progress);

        const now = Date.now();
        if (now - lastTickTime >= tickRate) {
            playTickSound();
            lastTickTime = now;
        }

        tickingInterval = setTimeout(scheduleNextTick, 20);
    }

    scheduleNextTick();
}

function stopTicking() {
    if (tickingInterval) {
        clearTimeout(tickingInterval);
        tickingInterval = null;
    }
}

window.stopTicking = stopTicking;

function playTickSound() {
    if (!audioContext) return;
    try {
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();

        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);

        oscillator.frequency.setValueAtTime(1000, audioContext.currentTime);
        oscillator.type = 'square';

        gainNode.gain.setValueAtTime(0, audioContext.currentTime);
        gainNode.gain.linearRampToValueAtTime(0.04, audioContext.currentTime + 0.001);
        gainNode.gain.exponentialRampToValueAtTime(0.001, audioContext.currentTime + 0.03);

        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + 0.03);
    } catch (e) {
        // Audio error ignored
    }
}

window.playWheelSound = (soundType) => {
    initAudioContext();
    if (!audioContext) return;

    try {
        let frequency = 400;
        let duration = 0.25;
        let type = 'sine';

        switch (soundType) {
            case 'spin':
                frequency = 320;
                duration = 0.25;
                type = 'sawtooth';
                break;
            case 'select':
                frequency = 880;
                duration = 0.35;
                type = 'sine';
                break;
            case 'stop':
                frequency = 440;
                duration = 0.2;
                type = 'triangle';
                break;
            default:
                return;
        }

        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();

        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);

        oscillator.frequency.setValueAtTime(frequency, audioContext.currentTime);
        oscillator.type = type;

        gainNode.gain.setValueAtTime(0, audioContext.currentTime);
        gainNode.gain.linearRampToValueAtTime(0.25, audioContext.currentTime + 0.01);
        gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + duration);

        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + duration);

        if (soundType === 'spin') {
            oscillator.frequency.linearRampToValueAtTime(frequency * 1.5, audioContext.currentTime + duration);
        }
    } catch (e) {
        // Audio error ignored
    }
};

// Enable audio context on first click
document.addEventListener('click', function unlockAudio() {
    try {
        if (!audioContext) {
            audioContext = new (window.AudioContext || window.webkitAudioContext)();
        }
        if (audioContext && audioContext.state === 'suspended') {
            audioContext.resume();
        }
    } catch (e) {}
    document.removeEventListener('click', unlockAudio);
}, { once: true });
