// Real In-Browser MediaRecorder & WebRTC Capture Engine
(function () {
    let mediaStream = null;
    let mediaRecorder = null;
    let recordedChunks = [];
    let audioContext = null;
    let analyserNode = null;
    let dataArray = null;
    let currentBlobUrl = null;

    window.blazorMedia = {
        start: async function (isAudioOnly, videoElementId) {
            try {
                recordedChunks = [];
                if (currentBlobUrl) {
                    URL.revokeObjectURL(currentBlobUrl);
                    currentBlobUrl = null;
                }

                const constraints = {
                    audio: true,
                    video: isAudioOnly ? false : { width: { ideal: 1280 }, height: { ideal: 720 }, facingMode: "user" }
                };

                mediaStream = await navigator.mediaDevices.getUserMedia(constraints);

                // Attach to live video preview element if in video mode
                if (!isAudioOnly && videoElementId) {
                    const videoEl = document.getElementById(videoElementId);
                    if (videoEl) {
                        videoEl.srcObject = mediaStream;
                        videoEl.play().catch(() => {});
                    }
                }

                // Setup Audio Analyser for live visualizer
                try {
                    audioContext = new (window.AudioContext || window.webkitAudioContext)();
                    if (audioContext.state === 'suspended') {
                        await audioContext.resume();
                    }
                    const source = audioContext.createMediaStreamSource(mediaStream);
                    analyserNode = audioContext.createAnalyser();
                    analyserNode.fftSize = 64;
                    source.connect(analyserNode);
                    dataArray = new Uint8Array(analyserNode.frequencyBinCount);
                } catch (audioErr) {
                    console.warn("Audio analyser setup skipped:", audioErr);
                }

                // Determine mimeType
                let mimeType = 'video/webm;codecs=vp8,opus';
                if (isAudioOnly) {
                    if (MediaRecorder.isTypeSupported('audio/webm')) mimeType = 'audio/webm';
                    else if (MediaRecorder.isTypeSupported('audio/mp4')) mimeType = 'audio/mp4';
                    else mimeType = '';
                } else {
                    if (!MediaRecorder.isTypeSupported(mimeType)) {
                        mimeType = MediaRecorder.isTypeSupported('video/mp4') ? 'video/mp4' : '';
                    }
                }

                const options = mimeType ? { mimeType } : {};
                mediaRecorder = new MediaRecorder(mediaStream, options);

                mediaRecorder.ondataavailable = function (e) {
                    if (e.data && e.data.size > 0) {
                        recordedChunks.push(e.data);
                    }
                };

                mediaRecorder.start(250); // Slice every 250ms
                return { success: true, mimeType: mediaRecorder.mimeType || 'audio/webm' };
            } catch (err) {
                console.error("getUserMedia or MediaRecorder failed:", err);
                return { success: false, error: err.message || "Permission denied or media hardware unavailable" };
            }
        },

        getFrequencyLevels: function () {
            if (!analyserNode || !dataArray) {
                return null;
            }
            analyserNode.getByteFrequencyData(dataArray);
            // Sample 16 bands
            const levels = [];
            const step = Math.max(1, Math.floor(dataArray.length / 16));
            for (let i = 0; i < 16; i++) {
                const val = dataArray[i * step] || 0;
                levels.push(Math.round((val / 255) * 100));
            }
            return levels;
        },

        pause: function () {
            if (mediaRecorder && mediaRecorder.state === 'recording') {
                mediaRecorder.pause();
                return true;
            }
            return false;
        },

        resume: function () {
            if (mediaRecorder && mediaRecorder.state === 'paused') {
                mediaRecorder.resume();
                return true;
            }
            return false;
        },

        stop: function (videoElementId) {
            return new Promise((resolve) => {
                if (!mediaRecorder) {
                    resolve({ success: false });
                    return;
                }

                mediaRecorder.onstop = function () {
                    const mimeType = mediaRecorder.mimeType || (recordedChunks[0] ? recordedChunks[0].type : 'audio/webm');
                    const blob = new Blob(recordedChunks, { type: mimeType });
                    currentBlobUrl = URL.createObjectURL(blob);

                    // Stop all hardware tracks
                    if (mediaStream) {
                        mediaStream.getTracks().forEach(t => t.stop());
                        mediaStream = null;
                    }

                    // Detach live video stream preview
                    if (videoElementId) {
                        const videoEl = document.getElementById(videoElementId);
                        if (videoEl) {
                            videoEl.srcObject = null;
                        }
                    }

                    if (audioContext && audioContext.state !== 'closed') {
                        audioContext.close().catch(() => {});
                        audioContext = null;
                    }

                    resolve({
                        success: true,
                        blobUrl: currentBlobUrl,
                        mimeType: mimeType,
                        sizeBytes: blob.size
                    });
                };

                if (mediaRecorder.state !== 'inactive') {
                    mediaRecorder.stop();
                } else {
                    mediaRecorder.onstop();
                }
            });
        },

        download: function (filename) {
            if (!currentBlobUrl) return false;
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = currentBlobUrl;
            a.download = filename || 'recording.webm';
            document.body.appendChild(a);
            a.click();
            setTimeout(() => {
                document.body.removeChild(a);
            }, 100);
            return true;
        }
    };
})();
