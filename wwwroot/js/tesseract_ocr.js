// BlazorTwUI - 100% Local Tesseract.js WebAssembly OCR Engine Bridge
// Zero external CDN dependencies - All WASM, Workers, and Models are served locally from wwwroot/lib/tesseract

window.TesseractOcrBridge = {
    _worker: null,
    _isReady: false,

    isReady: function () {
        return typeof Tesseract !== 'undefined';
    },

    recognizeImage: async function (imageSource, dotNetHelper) {
        const startTime = performance.now();

        if (typeof Tesseract === 'undefined') {
            return {
                success: false,
                errorMessage: "Local Tesseract library is not loaded.",
                text: "",
                words: [],
                lines: [],
                confidence: 0,
                durationMs: 0
            };
        }

        try {
            // Configure 100% LOCAL execution: no CDN, no external network requests
            const worker = await Tesseract.createWorker('eng', 1, {
                workerPath: '/lib/tesseract/worker.min.js',
                corePath: '/lib/tesseract/tesseract-core-simd-lstm.wasm.js',
                langPath: '/lib/tesseract',
                gzip: true,
                logger: m => {
                    if (dotNetHelper && typeof dotNetHelper.invokeMethodAsync === 'function') {
                        try {
                            const pct = Math.round((m.progress || 0) * 100);
                            dotNetHelper.invokeMethodAsync('OnTesseractProgress', pct, m.status || 'processing');
                        } catch (e) { }
                    }
                }
            });

            const result = await worker.recognize(imageSource);
            await worker.terminate();

            const durationMs = Math.round(performance.now() - startTime);
            const data = result.data;

            // Extract words and bounding boxes
            const words = (data.words || []).map(w => ({
                text: w.text,
                confidence: w.confidence,
                bbox: {
                    x0: w.bbox ? w.bbox.x0 : 0,
                    y0: w.bbox ? w.bbox.y0 : 0,
                    x1: w.bbox ? w.bbox.x1 : 0,
                    y1: w.bbox ? w.bbox.y1 : 0
                }
            }));

            // Extract lines
            const lines = (data.lines || []).map(l => ({
                text: l.text,
                confidence: l.confidence
            }));

            return {
                success: true,
                errorMessage: null,
                text: data.text || "",
                lines: lines,
                words: words,
                confidence: data.confidence || 0,
                durationMs: durationMs
            };
        } catch (err) {
            console.error('[Local Tesseract] Recognition error:', err);
            return {
                success: false,
                errorMessage: err.message || err.toString(),
                text: "",
                words: [],
                lines: [],
                confidence: 0,
                durationMs: Math.round(performance.now() - startTime)
            };
        }
    },

    renderCheckToDataUrl: function (payer, checkNum, date, payee, amount, words, memo, bankName, micrLine) {
        try {
            const canvas = document.createElement('canvas');
            canvas.width = 1200;
            canvas.height = 540;
            const ctx = canvas.getContext('2d');

            // Background
            ctx.fillStyle = '#ffffff';
            ctx.fillRect(0, 0, canvas.width, canvas.height);

            // Outer border
            ctx.strokeStyle = '#334155';
            ctx.lineWidth = 3;
            ctx.strokeRect(10, 10, canvas.width - 20, canvas.height - 20);

            // Subtle security lines
            ctx.strokeStyle = '#f8fafc';
            ctx.lineWidth = 1;
            for (let y = 30; y < canvas.height - 30; y += 15) {
                ctx.beginPath();
                ctx.moveTo(20, y);
                ctx.lineTo(canvas.width - 20, y);
                ctx.stroke();
            }

            // Payer Information
            ctx.fillStyle = '#0f172a';
            ctx.font = 'bold 22px Arial, sans-serif';
            ctx.fillText(payer || 'ACME ENTERPRISE CORP', 40, 55);
            ctx.font = '14px Arial, sans-serif';
            ctx.fillStyle = '#475569';
            ctx.fillText('100 Silicon Way, Tech City, CA 94016', 40, 80);

            // Check Number & Date
            ctx.fillStyle = '#0f172a';
            ctx.font = 'bold 24px monospace';
            ctx.fillText('CHECK #' + (checkNum || '1001'), 980, 55);
            ctx.font = '16px Arial, sans-serif';
            ctx.fillText('DATE: ' + (date || new Date().toLocaleDateString()), 920, 95);

            // Pay to the Order of
            ctx.font = 'bold 15px Arial, sans-serif';
            ctx.fillStyle = '#334155';
            ctx.fillText('PAY TO THE ORDER OF:', 40, 170);

            ctx.font = 'bold italic 24px "Times New Roman", serif';
            ctx.fillStyle = '#0f172a';
            ctx.fillText(payee || 'Jane Doe Consulting', 240, 170);

            // Courtesy Box
            ctx.strokeStyle = '#0f172a';
            ctx.lineWidth = 2;
            ctx.strokeRect(960, 140, 200, 45);
            ctx.fillStyle = '#f8fafc';
            ctx.fillRect(961, 141, 198, 43);
            ctx.fillStyle = '#0f172a';
            ctx.font = 'bold 24px monospace';
            ctx.fillText('$' + amount, 980, 172);

            // Legal words
            ctx.font = 'italic 18px "Times New Roman", serif';
            ctx.fillStyle = '#0f172a';
            ctx.fillText(words || 'Five thousand and 00/100 Dollars', 40, 240);
            ctx.font = 'bold 14px Arial, sans-serif';
            ctx.fillText('DOLLARS', 1080, 240);

            // Bank Name
            ctx.font = 'bold 16px Arial, sans-serif';
            ctx.fillStyle = '#334155';
            ctx.fillText(bankName || 'JPMorgan Chase Bank, N.A.', 40, 310);

            // Memo
            ctx.font = '14px Arial, sans-serif';
            ctx.fillText('MEMO: ' + (memo || 'Consulting Services'), 40, 390);

            // Signature line
            ctx.strokeStyle = '#475569';
            ctx.beginPath();
            ctx.moveTo(850, 390);
            ctx.lineTo(1150, 390);
            ctx.stroke();
            ctx.font = 'italic 28px "Brush Script MT", cursive, serif';
            ctx.fillStyle = '#1e3a8a';
            ctx.fillText('Authorized Signature', 880, 380);

            // MICR Band
            ctx.fillStyle = '#f8fafc';
            ctx.fillRect(20, 440, canvas.width - 40, 75);
            ctx.strokeStyle = '#cbd5e1';
            ctx.lineWidth = 1;
            ctx.strokeRect(20, 440, canvas.width - 40, 75);

            // Convert E-13B unicode symbols to standard ASCII transit delimiters (A/C/B/D) with spacing
            // so canvas standard monospace fonts render legible glyphs for Tesseract OCR
            const printableMicr = (micrLine || 'A 121000358 A 0123456789 C 1001')
                .replace(/⑆/g, ' A ')
                .replace(/⑈/g, ' C ')
                .replace(/⑇/g, ' B ')
                .replace(/⑉/g, ' - ')
                .replace(/\s+/g, ' ')
                .trim();

            ctx.font = 'bold 28px monospace';
            ctx.fillStyle = '#0f172a';
            ctx.fillText(printableMicr, 120, 490);

            return canvas.toDataURL('image/png');
        } catch (e) {
            console.error('[Local Tesseract] Canvas rendering error:', e);
            return null;
        }
    }
};
