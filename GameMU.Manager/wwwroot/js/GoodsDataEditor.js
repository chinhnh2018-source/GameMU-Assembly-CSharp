/**
 * GoodsDataEditor.js  v2
 * Visual editor for GoodsData "id,count,bind,forge,append,lucky,excel|..." format.
 *
 * Tự động kích hoạt trong trang Edit khi có textarea có name bắt đầu bằng f_Goods*.
 * Compatible với Razor Pages form (prefix f_).
 */
const GoodsDataEditor = (() => {
    const FIELDS = ['GoodsID', 'GCount', 'Binding', 'Forge_level', 'AppendPropLev', 'Lucky', 'ExcellenceInfo'];
    const FIELD_LABELS = ['Mã vật phẩm', 'Số lượng', 'Ràng buộc', 'Cấp rèn', 'Cấp phụ', 'Lucky', 'Excellence'];
    const GOODS_NAME_PATTERNS = [
        /^f_Goods(One|Two|Thr|Four|List|ID|Id|IDs)$/i,
        /^f_Award(Goods)?$/i,
        /^f_GoodsID\d*$/i,
    ];

    function isGoodsField(name) {
        return GOODS_NAME_PATTERNS.some(p => p.test(name));
    }

    function isGoodsDataFormat(raw) {
        if (!raw || !raw.trim()) return false;
        const chunk = raw.split('|')[0].trim();
        const parts = chunk.split(',');
        // GoodsData: ít nhất 3 fields, field đầu là số nguyên dương
        return parts.length >= 3 && /^\d+$/.test(parts[0].trim()) && parts[0].trim() !== '0';
    }

    function parse(raw) {
        if (!raw || !raw.trim()) return [];
        return raw.split('|').filter(s => s.trim()).map(chunk => {
            const parts = chunk.split(',').map(s => s.trim());
            const obj = {};
            FIELDS.forEach((f, i) => obj[f] = parts[i] ?? '0');
            return obj;
        });
    }

    function serialize(items) {
        return items.filter(item => item.GoodsID && item.GoodsID !== '0').map(item =>
            FIELDS.map(f => item[f] || '0').join(',')
        ).join('|');
    }

    function syncToHidden(table) {
        const items = [];
        table.querySelectorAll('tbody tr[data-row]').forEach(tr => {
            const item = {};
            tr.querySelectorAll('input[data-field]').forEach(inp => {
                item[inp.dataset.field] = inp.value;
            });
            items.push(item);
        });
        const hidden = document.getElementById(table.dataset.hiddenId);
        if (hidden) hidden.value = serialize(items);
    }

    function createRow(item, rowIndex) {
        const tr = document.createElement('tr');
        tr.dataset.row = rowIndex;
        FIELDS.forEach((f, i) => {
            const td = document.createElement('td');
            const inp = document.createElement('input');
            inp.type = 'number';
            inp.min = '0';
            inp.value = item[f] || '0';
            inp.dataset.field = f;
            inp.className = 'form-control form-control-sm';
            inp.style.width = i === 0 ? '100px' : i === 1 ? '70px' : '55px';
            inp.style.minWidth = inp.style.width;
            if (f === 'Binding') { inp.max = '1'; inp.title = '0=Không ràng, 1=Ràng'; }
            inp.addEventListener('input', () => syncToHidden(tr.closest('table')));
            td.appendChild(inp);
            tr.appendChild(td);
        });
        const tdAct = document.createElement('td');
        const btnDel = document.createElement('button');
        btnDel.type = 'button';
        btnDel.className = 'btn btn-sm btn-outline-danger py-0';
        btnDel.textContent = '✕';
        btnDel.title = 'Xóa item này';
        btnDel.onclick = () => { tr.remove(); syncToHidden(tr.closest('table')); };
        tdAct.appendChild(btnDel);
        tr.appendChild(tdAct);
        return tr;
    }

    function initField(textarea) {
        if (textarea.dataset.goodsInit) return;
        textarea.dataset.goodsInit = '1';
        const raw = textarea.value;
        const name = textarea.name;          // e.g. "f_GoodsOne"
        const displayName = name.replace(/^f_/, '');

        const wrapper = document.createElement('div');
        wrapper.className = 'goods-editor mt-1';

        // Header
        const hdr = document.createElement('div');
        hdr.className = 'd-flex flex-wrap align-items-center gap-2 mb-1';
        hdr.innerHTML = `
            <span class="badge text-bg-secondary">${displayName}</span>
            <small class="text-muted">GoodsData — 7 trường mỗi item, ngăn cách bằng |</small>
            <button type="button" class="btn btn-sm btn-outline-primary btn-add-item ms-auto">＋ Thêm</button>
        `;
        wrapper.appendChild(hdr);

        // Table
        const wrap = document.createElement('div');
        wrap.style.overflowX = 'auto';
        const table = document.createElement('table');
        table.className = 'table table-sm table-bordered mb-1 goods-table';
        table.dataset.hiddenId = `hid-${name}`;

        // thead
        const thead = document.createElement('thead');
        thead.innerHTML = `<tr class="table-light">${
            FIELD_LABELS.map((lbl, i) =>
                `<th style="font-size:.7rem;white-space:nowrap" title="${FIELDS[i]}">${lbl}</th>`
            ).join('')
        }<th></th></tr>`;
        table.appendChild(thead);

        // tbody
        const tbody = document.createElement('tbody');
        parse(raw).forEach((item, i) => tbody.appendChild(createRow(item, i)));
        table.appendChild(tbody);
        wrap.appendChild(table);
        wrapper.appendChild(wrap);

        // Empty-state
        const empty = document.createElement('p');
        empty.className = 'text-muted small mb-0 goods-empty';
        empty.style.display = tbody.children.length ? 'none' : '';
        empty.textContent = '(chưa có item — nhấn ＋ Thêm để bắt đầu)';
        wrapper.appendChild(empty);

        function refreshEmpty() {
            empty.style.display = tbody.children.length ? 'none' : '';
        }

        // Hidden input (maintains the raw value for form submit)
        const hidden = document.createElement('input');
        hidden.type = 'hidden';
        hidden.id = `hid-${name}`;
        hidden.name = name;          // same name → replaces original textarea in submit
        hidden.value = raw;
        wrapper.appendChild(hidden);

        // Disable original textarea so it doesn't submit duplicate
        textarea.removeAttribute('name');
        textarea.style.display = 'none';
        textarea.parentNode.insertBefore(wrapper, textarea.nextSibling);

        // Add button
        hdr.querySelector('.btn-add-item').addEventListener('click', () => {
            const newItem = Object.fromEntries(FIELDS.map(f => [f, '0']));
            newItem.GCount = '1';
            tbody.appendChild(createRow(newItem, tbody.children.length));
            refreshEmpty();
            syncToHidden(table);
        });

        // Initial sync observation
        new MutationObserver(() => refreshEmpty())
            .observe(tbody, { childList: true });
    }

    function init() {
        // Target all textareas in edit form that match Goods field pattern
        document.querySelectorAll('textarea').forEach(ta => {
            const name = ta.getAttribute('name') || '';
            if (isGoodsField(name) || isGoodsDataFormat(ta.value)) {
                initField(ta);
            }
        });
    }

    // Public API
    return { init, parse, serialize };
})();

// Auto-init on Edit pages
document.addEventListener('DOMContentLoaded', () => {
    if (document.querySelector('form[method=post]') && document.querySelector('textarea')) {
        GoodsDataEditor.init();
    }
});
