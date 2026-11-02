// Browser helpers for the window and HTML editor components.
window.twEnterprise = (function () {
    const dragging = new WeakMap();
    const editors = new WeakMap();

    // ---- TwWindow: pointer drag on the title bar. Position is written straight to CSS, .NET hears about it once on release. ----
    function attachWindow(root, handle, ref) {
        if (!root || !handle || dragging.has(root)) return;
        let start = null;
        const down = e => {
            if (e.button !== 0 || e.target.closest('button')) return;
            const r = root.getBoundingClientRect();
            start = { dx: e.clientX - r.left, dy: e.clientY - r.top };
            handle.setPointerCapture(e.pointerId);
            ref.invokeMethodAsync('OnFocused');
        };
        const move = e => {
            if (!start) return;
            const x = Math.max(0, Math.min(window.innerWidth - 60, e.clientX - start.dx));
            const y = Math.max(0, Math.min(window.innerHeight - 40, e.clientY - start.dy));
            root.style.left = x + 'px';
            root.style.top = y + 'px';
        };
        const up = e => {
            if (!start) return;
            start = null;
            try { handle.releasePointerCapture(e.pointerId); } catch { }
            const r = root.getBoundingClientRect();
            ref.invokeMethodAsync('JsMoved', Math.round(r.left), Math.round(r.top));
        };
        handle.addEventListener('pointerdown', down);
        handle.addEventListener('pointermove', move);
        handle.addEventListener('pointerup', up);
        handle.addEventListener('pointercancel', up);
        dragging.set(root, () => {
            handle.removeEventListener('pointerdown', down);
            handle.removeEventListener('pointermove', move);
            handle.removeEventListener('pointerup', up);
            handle.removeEventListener('pointercancel', up);
        });
    }

    function detachWindow(root) {
        const d = root && dragging.get(root);
        if (d) { d(); dragging.delete(root); }
    }

    // ---- TwHtmlEditor ----
    const ALLOWED = new Set(['P', 'BR', 'B', 'STRONG', 'I', 'EM', 'U', 'UL', 'OL', 'LI', 'A', 'H2', 'H3', 'BLOCKQUOTE', 'CODE', 'PRE', 'DIV', 'SPAN']);
    const SAFE_URL = /^(https?:|mailto:)/i;

    // Parse into an inert document, rebuild only allowlisted elements and attributes, drop everything else (scripts, handlers, styles, unknown tags keep their text).
    function sanitize(html) {
        const doc = new DOMParser().parseFromString('<body>' + html + '</body>', 'text/html');
        const out = document.createElement('div');
        const walk = (src, dst) => {
            for (const node of Array.from(src.childNodes)) {
                if (node.nodeType === Node.TEXT_NODE) { dst.appendChild(document.createTextNode(node.textContent)); continue; }
                if (node.nodeType !== Node.ELEMENT_NODE) continue;
                const tag = node.tagName;
                if (tag === 'SCRIPT' || tag === 'STYLE' || tag === 'IFRAME' || tag === 'OBJECT' || tag === 'EMBED' || tag === 'TEMPLATE') continue;
                if (!ALLOWED.has(tag)) { walk(node, dst); continue; } // unknown tag: keep its text and children, drop the tag
                const el = document.createElement(tag);
                if (tag === 'A') {
                    const href = node.getAttribute('href') || '';
                    if (SAFE_URL.test(href.trim())) {
                        el.setAttribute('href', href.trim());
                        el.setAttribute('rel', 'noopener noreferrer nofollow');
                        el.setAttribute('target', '_blank');
                    }
                }
                walk(node, el);
                dst.appendChild(el);
            }
        };
        walk(doc.body, out);
        return out.innerHTML;
    }

    function attachEditor(el, ref, initial) {
        if (!el || editors.has(el)) return;
        el.innerHTML = sanitize(initial || '');
        let timer = 0;
        const report = () => ref.invokeMethodAsync('OnHtmlChanged', sanitize(el.innerHTML));
        const onInput = () => { clearTimeout(timer); timer = setTimeout(report, 150); };
        el.addEventListener('input', onInput);
        el.addEventListener('blur', report);
        // Pasted content is reduced to plain text so nothing unexpected enters the document.
        const onPaste = e => {
            e.preventDefault();
            const text = (e.clipboardData || window.clipboardData).getData('text/plain');
            document.execCommand('insertText', false, text);
        };
        el.addEventListener('paste', onPaste);
        editors.set(el, () => { el.removeEventListener('input', onInput); el.removeEventListener('blur', report); el.removeEventListener('paste', onPaste); clearTimeout(timer); });
    }

    return {
        attachWindow, detachWindow, sanitize, attachEditor,
        setEditorHtml(el, html) {
            if (!el) return;
            const clean = sanitize(html || '');
            if (el.innerHTML !== clean) el.innerHTML = clean;
        },
        editorCommand(el, cmd, arg) {
            if (!el) return;
            el.focus();
            if (cmd === 'createLink') {
                if (!arg || !SAFE_URL.test(arg.trim())) return false;
                document.execCommand('createLink', false, arg.trim());
            } else if (cmd === 'formatBlock') {
                document.execCommand('formatBlock', false, arg);
            } else {
                document.execCommand(cmd, false, null);
            }
            el.dispatchEvent(new Event('input', { bubbles: true }));
            return true;
        },
        detachEditor(el) {
            const d = el && editors.get(el);
            if (d) { d(); editors.delete(el); }
        }
    };
})();
