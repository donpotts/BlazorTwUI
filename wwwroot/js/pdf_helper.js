// PDF Helper: Client-side Download & Printing for Flattened Documents
window.pdfHelper = {
    downloadBlob: function (filename, mimeType, base64Data) {
        try {
            const byteCharacters = atob(base64Data);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: mimeType });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            setTimeout(function () {
                if (document.body.contains(a)) {
                    document.body.removeChild(a);
                }
                URL.revokeObjectURL(url);
            }, 1000);
            return true;
        } catch (e) {
            console.error('Failed to download blob:', e);
            return false;
        }
    },
    printPdfBase64: function (base64Data) {
        try {
            const byteCharacters = atob(base64Data);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: 'application/pdf' });
            const blobUrl = URL.createObjectURL(blob);

            let iframe = document.getElementById('__pdf_print_iframe__');
            if (!iframe) {
                iframe = document.createElement('iframe');
                iframe.id = '__pdf_print_iframe__';
                iframe.style.position = 'fixed';
                iframe.style.right = '0';
                iframe.style.bottom = '0';
                iframe.style.width = '0';
                iframe.style.height = '0';
                iframe.style.border = '0';
                document.body.appendChild(iframe);
            }

            iframe.onload = function () {
                setTimeout(function () {
                    try {
                        iframe.contentWindow.focus();
                        iframe.contentWindow.print();
                    } catch (e) {
                        console.warn('Iframe print restricted, opening PDF tab:', e);
                        window.open(blobUrl, '_blank');
                    }
                }, 300);
            };
            iframe.src = blobUrl;
            return true;
        } catch (e) {
            console.error('Failed to print PDF:', e);
            return false;
        }
    },
    downloadText: function (filename, mimeType, textContent) {
        try {
            const blob = new Blob([textContent], { type: mimeType });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);
            return true;
        } catch (e) {
            console.error('Failed to download text:', e);
            return false;
        }
    },
    printDocument: function () {
        window.print();
        return true;
    },
    downloadSvgAsPng: function (filename, svgString, width, height) {
        try {
            width = width || 640;
            height = height || 320;
            const canvas = document.createElement('canvas');
            canvas.width = width * 2;
            canvas.height = height * 2;
            const ctx = canvas.getContext('2d');
            ctx.scale(2, 2);

            const blob = new Blob([svgString], { type: 'image/svg+xml;charset=utf-8' });
            const url = URL.createObjectURL(blob);
            const img = new Image();
            img.onload = function () {
                ctx.drawImage(img, 0, 0, width, height);
                URL.revokeObjectURL(url);
                canvas.toBlob(function (pngBlob) {
                    if (!pngBlob) return;
                    const pngUrl = URL.createObjectURL(pngBlob);
                    const a = document.createElement('a');
                    a.href = pngUrl;
                    a.download = filename.endsWith('.png') ? filename : filename + '.png';
                    document.body.appendChild(a);
                    a.click();
                    setTimeout(function () {
                        if (document.body.contains(a)) document.body.removeChild(a);
                        URL.revokeObjectURL(pngUrl);
                    }, 1000);
                }, 'image/png');
            };
            img.onerror = function (e) {
                console.error('Failed to load SVG into image for PNG export:', e);
                URL.revokeObjectURL(url);
            };
            img.src = url;
            return true;
        } catch (e) {
            console.error('Failed to export PNG:', e);
            return false;
        }
    }
};
