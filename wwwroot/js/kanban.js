/**
 * BlazorTwUI - Universal Kanban Drag & Drop Engine
 * Unified pointer, mouse, touch, and HTML5 drag support for Desktop, iOS Safari, and Android Chrome.
 */
window.BlazorKanban = {
    instances: new Map(),

    init: function (boardElement, dotNetHelper) {
        if (!boardElement) return;

        const boardId = boardElement.id || ('kanban-' + Math.random().toString(36).substring(2, 9));
        boardElement.id = boardId;

        let activeGhost = null;
        let draggedCard = null;
        let draggedCardId = null;
        let currentTargetColId = null;
        let isPointerDragging = false;
        let startX = 0;
        let startY = 0;
        let offsetX = 0;
        let offsetY = 0;
        let cardRect = null;

        function clearAllColumnHighlights() {
            const cols = boardElement.querySelectorAll('[data-column-id]');
            cols.forEach(col => {
                col.classList.remove('ring-2', 'ring-brand-500', 'bg-brand-50/50', 'dark:bg-brand-950/40', 'border-brand-500');
            });
        }

        // --- POINTER / MOUSE / TOUCH UNIVERSAL DRAG ENGINE ---
        function onPointerDown(e) {
            // Ignore non-primary clicks (right click, middle click)
            if (e.button !== undefined && e.button !== 0) return;

            // Don't drag if user clicked an interactive control inside the card
            if (e.target.closest('button, select, option, input, textarea, a')) return;

            const card = e.target.closest('.btui-kanban-card');
            if (!card) return;

            draggedCard = card;
            draggedCardId = card.getAttribute('data-card-id');
            cardRect = card.getBoundingClientRect();
            startX = e.clientX;
            startY = e.clientY;
            offsetX = startX - cardRect.left;
            offsetY = startY - cardRect.top;
            isPointerDragging = false;
            currentTargetColId = null;

            window.addEventListener('pointermove', onPointerMove, { passive: false });
            window.addEventListener('pointerup', onPointerUp);
            window.addEventListener('pointercancel', onPointerCancel);
        }

        function onPointerMove(e) {
            if (!draggedCard) return;

            const dist = Math.hypot(e.clientX - startX, e.clientY - startY);
            if (!isPointerDragging && dist > 5) {
                isPointerDragging = true;

                // Create floating card ghost
                activeGhost = draggedCard.cloneNode(true);
                activeGhost.id = 'kanban-drag-ghost';
                activeGhost.classList.add(
                    'fixed', 'pointer-events-none', 'z-50', 'shadow-2xl', 
                    'opacity-90', 'scale-105', 'rotate-1', 'ring-2', 'ring-brand-500', 
                    'transition-none', 'bg-white', 'dark:bg-slate-900'
                );
                activeGhost.style.width = cardRect.width + 'px';
                activeGhost.style.left = (e.clientX - offsetX) + 'px';
                activeGhost.style.top = (e.clientY - offsetY) + 'px';
                activeGhost.style.margin = '0';
                document.body.appendChild(activeGhost);

                draggedCard.classList.add('opacity-30', 'scale-95');

                if (navigator.vibrate) {
                    navigator.vibrate(12);
                }
            }

            if (isPointerDragging && activeGhost) {
                e.preventDefault();
                activeGhost.style.left = (e.clientX - offsetX) + 'px';
                activeGhost.style.top = (e.clientY - offsetY) + 'px';

                // Find column underneath current pointer
                activeGhost.style.display = 'none';
                const elUnder = document.elementFromPoint(e.clientX, e.clientY);
                activeGhost.style.display = '';

                const colUnder = elUnder ? elUnder.closest('[data-column-id]') : null;
                if (colUnder) {
                    const colId = colUnder.getAttribute('data-column-id');
                    if (currentTargetColId !== colId) {
                        clearAllColumnHighlights();
                        currentTargetColId = colId;
                        colUnder.classList.add('ring-2', 'ring-brand-500', 'bg-brand-50/50', 'dark:bg-brand-950/40', 'border-brand-500');
                        if (navigator.vibrate) navigator.vibrate(8);
                    }
                } else {
                    clearAllColumnHighlights();
                    currentTargetColId = null;
                }
            }
        }

        function onPointerUp(e) {
            window.removeEventListener('pointermove', onPointerMove);
            window.removeEventListener('pointerup', onPointerUp);
            window.removeEventListener('pointercancel', onPointerCancel);

            if (activeGhost) {
                activeGhost.remove();
                activeGhost = null;
            }

            if (draggedCard) {
                draggedCard.classList.remove('opacity-30', 'scale-95');
            }

            if (isPointerDragging && draggedCardId && currentTargetColId) {
                dotNetHelper.invokeMethodAsync('OnCardDroppedJs', draggedCardId, currentTargetColId);
                if (navigator.vibrate) navigator.vibrate([15, 30, 15]);
            }

            clearAllColumnHighlights();
            draggedCard = null;
            draggedCardId = null;
            currentTargetColId = null;
            isPointerDragging = false;
        }

        function onPointerCancel() {
            onPointerUp();
        }

        // --- HTML5 DRAG & DROP FALLBACK ---
        function onDragStart(e) {
            const card = e.target.closest('.btui-kanban-card');
            if (!card) return;

            draggedCardId = card.getAttribute('data-card-id');
            const col = card.closest('[data-column-id]');
            const sourceColId = col ? col.getAttribute('data-column-id') : '';

            e.dataTransfer.setData('text/plain', JSON.stringify({ cardId: draggedCardId, sourceColId }));
            e.dataTransfer.effectAllowed = 'move';
        }

        function onDragOver(e) {
            e.preventDefault();
            e.dataTransfer.dropEffect = 'move';
            const col = e.target.closest('[data-column-id]');
            if (col) {
                clearAllColumnHighlights();
                col.classList.add('ring-2', 'ring-brand-500', 'bg-brand-50/50', 'dark:bg-brand-950/40');
            }
        }

        function onDrop(e) {
            e.preventDefault();
            const col = e.target.closest('[data-column-id]');
            if (!col) return;

            const targetColId = col.getAttribute('data-column-id');
            clearAllColumnHighlights();

            let cardId = draggedCardId;
            try {
                const data = JSON.parse(e.dataTransfer.getData('text/plain') || '{}');
                if (data.cardId) cardId = data.cardId;
            } catch (err) { }

            if (cardId && targetColId) {
                dotNetHelper.invokeMethodAsync('OnCardDroppedJs', cardId, targetColId);
            }
        }

        // Attach listeners
        boardElement.addEventListener('pointerdown', onPointerDown);
        boardElement.addEventListener('dragstart', onDragStart);
        boardElement.addEventListener('dragover', onDragOver);
        boardElement.addEventListener('drop', onDrop);

        this.instances.set(boardId, {
            destroy: function () {
                boardElement.removeEventListener('pointerdown', onPointerDown);
                boardElement.removeEventListener('dragstart', onDragStart);
                boardElement.removeEventListener('dragover', onDragOver);
                boardElement.removeEventListener('drop', onDrop);
                window.removeEventListener('pointermove', onPointerMove);
                window.removeEventListener('pointerup', onPointerUp);
                window.removeEventListener('pointercancel', onPointerCancel);
            }
        });
    },

    destroy: function (boardElement) {
        if (!boardElement || !boardElement.id) return;
        const instance = this.instances.get(boardElement.id);
        if (instance) {
            instance.destroy();
            this.instances.delete(boardElement.id);
        }
    }
};
