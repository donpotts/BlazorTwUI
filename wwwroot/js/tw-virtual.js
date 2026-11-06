// Helper for virtualized lists (TwCombobox): bring the item at `index` into view so Blazor's Virtualize renders it.
// The item height is read from a rendered item when there is one (falls back to `size`). Loaded on demand with import().
export function scrollToIndex(list, index, size, before) {
    if (!list) return;
    const sample = list.querySelector('[role=option]');
    const h = sample ? sample.getBoundingClientRect().height || size : size;
    const itemTop = index * h;
    if (itemTop < list.scrollTop) list.scrollTop = Math.max(0, itemTop - (before ?? 0) * h);
    else if (itemTop + h > list.scrollTop + list.clientHeight) list.scrollTop = itemTop + h - list.clientHeight + (before ?? 0) * h;
}
