// Tailwind Blazor Dashboard Manager (Mobile-First, Draggable & Resizable Grid Coordinator)
let gridInstances = new Map();

export function init(containerSelector, dotNetHelper, layoutData, isLocked, cellHeight = 75) {
    const container = document.querySelector(containerSelector);
    if (!container) {
        console.warn('TwDashboard: Container not found', containerSelector);
        return;
    }

    const gridEl = container.querySelector('.grid-stack');
    if (!gridEl) {
        console.warn('TwDashboard: .grid-stack element not found in', containerSelector);
        return;
    }

    // Check if GridStack library is loaded
    if (typeof GridStack === 'undefined') {
        console.error('TwDashboard: GridStack is not defined. Ensure gridstack-all.js is loaded.');
        return;
    }

    // Patch GridStack's fixed container lookup to prevent scrolled drag-helper offset bug
    if (GridStack.Utils && !GridStack.Utils._fixedContainerPatched) {
        GridStack.Utils._fixedContainerPatched = true;
        GridStack.Utils.getContainerForPositionFixedElement = function (el) {
            let curr = el;
            while (curr && curr !== document.documentElement && curr.parentElement) {
                const s = window.getComputedStyle(curr);
                if (s.transform !== 'none' || s.perspective !== 'none' || s.filter !== 'none' || s.backdropFilter !== 'none') {
                    return curr;
                }
                curr = curr.parentElement;
            }
            return {
                getBoundingClientRect: () => ({
                    left: 0,
                    top: 0,
                    right: window.innerWidth,
                    bottom: window.innerHeight,
                    width: window.innerWidth,
                    height: window.innerHeight,
                    x: 0,
                    y: 0
                })
            };
        };
    }

    // Destroy existing instance if any (re-init after a re-render must not leak the old grid or its listener)
    teardown(containerSelector);

    // Check screen width for mobile-first defaults
    const isMobile = window.innerWidth < 768;

    const options = {
        float: true,
        cellHeight: cellHeight,
        handle: '.move-handle',
        disableOneColumnMode: false,
        margin: 12,
        animate: true,
        staticGrid: isLocked,
        draggable: {
            handle: '.move-handle',
            scroll: false,
            appendTo: 'body'
        },
        resizable: {
            handles: 'e, se, s, sw, w'
        }
    };

    const grid = GridStack.init(options, gridEl);

    // Mobile resize listener to adapt cell height if orientation changes. Kept on the instance so destroy()
    // can remove it; a stale listener firing on a destroyed grid throws inside GridStack.
    const onResize = () => {
        if (grid && !grid.destroyed) {
            grid.cellHeight(window.innerWidth < 768 ? 65 : cellHeight);
        }
    };
    window.addEventListener('resize', onResize);
    gridInstances.set(containerSelector, { grid, dotNetHelper, onResize });

    // Load initial layout
    if (layoutData && layoutData.length > 0) {
        grid.load(layoutData);
    }

    // Populate content placeholders into GridStack elements
    requestAnimationFrame(() => {
        populateGridContent(gridEl);
        configureDragHandles(gridEl);
        setupDashboardDelegation(gridEl, dotNetHelper);
    });

    // Listen for drag/resize changes
    grid.on('change', (event, items) => {
        if (!items || !dotNetHelper) return;
        const newLayout = items.map(item => ({
            id: item.id || item.el?.getAttribute('gs-id'),
            x: item.x,
            y: item.y,
            w: item.w,
            h: item.h
        }));
        dotNetHelper.invokeMethodAsync('OnLayoutUpdatedFromJS', newLayout);
    });

    return true;
}

export function populateGridContent(gridEl) {
    if (!gridEl) return;
    const items = gridEl.querySelectorAll('.grid-stack-item');
    items.forEach(item => {
        const id = item.getAttribute('gs-id');
        if (!id) return;

        const placeholder = document.getElementById(`content-for-${id}`);
        const contentHost = item.querySelector('.grid-stack-item-content');

        if (placeholder && contentHost && contentHost.children.length === 0) {
            const clone = placeholder.cloneNode(true);
            clone.style.display = 'block';
            while (clone.firstChild) {
                contentHost.appendChild(clone.firstChild);
            }
        }
    });
}

export function configureDragHandles(gridEl) {
    if (!gridEl) return;
    const handles = gridEl.querySelectorAll('.move-handle');
    handles.forEach(handle => {
        handle.style.touchAction = 'none';
        handle.style.cursor = 'grab';
    });
}

export function addCard(containerSelector, cardData) {
    const instance = gridInstances.get(containerSelector);
    if (!instance || !instance.grid) return;

    const grid = instance.grid;
    const maxCols = grid.getColumn();
    if (cardData.x + cardData.w > maxCols) {
        cardData.x = Math.max(0, maxCols - cardData.w);
    }

    grid.addWidget(cardData);
    requestAnimationFrame(() => {
        const gridEl = grid.el;
        populateGridContent(gridEl);
        configureDragHandles(gridEl);
    });
}

export function removeCard(containerSelector, cardId) {
    const instance = gridInstances.get(containerSelector);
    if (!instance || !instance.grid) return;

    const el = instance.grid.el.querySelector(`[gs-id="${cardId}"]`);
    if (el) {
        instance.grid.removeWidget(el);
    }
}

export function loadData(containerSelector, layoutData) {
    const instance = gridInstances.get(containerSelector);
    if (!instance || !instance.grid) return;

    instance.grid.load(layoutData);
    requestAnimationFrame(() => {
        populateGridContent(instance.grid.el);
        configureDragHandles(instance.grid.el);
    });
}

export function setLocked(containerSelector, isLocked) {
    const instance = gridInstances.get(containerSelector);
    if (!instance || !instance.grid) return;
    instance.grid.setStatic(isLocked);
}

export function compact(containerSelector) {
    const instance = gridInstances.get(containerSelector);
    if (!instance || !instance.grid) return;
    instance.grid.compact();
}

export function setupDashboardDelegation(gridEl, dotNetHelper) {
    if (!gridEl || gridEl._delegationInitialized) return;
    gridEl._delegationInitialized = true;

    gridEl.addEventListener('click', (event) => {
        const actionBtn = event.target.closest('[data-tw-action]');
        if (!actionBtn) return;

        const action = actionBtn.getAttribute('data-tw-action');
        const cardId = actionBtn.getAttribute('data-card-id');

        if (action && cardId && dotNetHelper) {
            event.preventDefault();
            event.stopPropagation();
            dotNetHelper.invokeMethodAsync('OnCardActionFromJS', action, cardId);
        }
    });
}

export function destroy(containerSelector) {
    teardown(containerSelector);
}

function teardown(containerSelector) {
    const instance = gridInstances.get(containerSelector);
    if (!instance) return;
    window.removeEventListener('resize', instance.onResize);
    try {
        instance.grid.destroyed = true;
        instance.grid.destroy(false);
    } catch (e) { }
    gridInstances.delete(containerSelector);
}
