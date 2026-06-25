/**
 * GoodsDataEditor.js
 * Component parse/render/serialize GoodsData format:
 * "GoodsID,GCount,Binding,Forge_level,AppendPropLev,Lucky,ExcellenceInfo|..."
 *
 * Cách dùng:
 * 1. Thêm <script src="/js/GoodsDataEditor.js"></script> vào Edit.cshtml
 * 2. Gọi GoodsDataEditor.init() sau khi DOM loaded
 */

const GoodsDataEditor = (() => {
    const FIELDS = ['GoodsID', 'GCount', 'Binding', 'Forge_level', 'AppendPropLev', 'Lucky', 'ExcellenceInfo'];
    const GOODS_ATTRS = ['GoodsOne', 'GoodsTwo', 'GoodsThr', 'GoodsFour', 'GoodsList',
                         'GoodsID', 'GoodsId', 'Award', 'AwardGoods'];

    /** Parse "id,count,bind,forge,append,lucky,excel|..." → array of objects */
    function parse(raw) {
        if (!raw || raw.trim() === '') return [];
        return raw.split('|')
            .filter(s => s.trim())
            .map(chunk => {
                const parts = chunk.split(',').map(s => s.trim());
                const obj = {};
                FIELDS.forEach((f, i) => obj[f] = parts[i] || '0');
                return obj;
            });
    }

    /** Array of objects → "id,count,bind,...|..." */
    function serialize(items) {
        return items
            .filter(item => item.GoodsID && item.GoodsID !== '0')
            .map(item => FIELDS.map(f => item[f] || '0').join(','))
            .join('|');
    }

    /** Tạo một row trong bảng */
    function createRow(item, rowIndex) {
        const tr = document.createElement('tr');
        tr.dataset.row = rowIndex;

        FIELDS.forEach((f, i) => {
            const td = document.createElement('td');
            const input = document.createElement('input');
            input.type = 'number';
            input.value = item[f] || '0';
            input.dataset.field = f;
            input.dataset.row = rowIndex;
            input.className = i === 0
                ? 'goods-id-input form-control form-control-sm'
                : 'form-control form-control-sm';
            input.style.width = i === 0 ? '90px' : '60px';
            input.min = '0';
            if (f === 'Binding') { input.max = '1'; }
            td.appendChild(input);
            tr.appendChild(td);
        });

        // Delete button
        const tdDel = document.createElement('td');
        const btnDel = document.createElement('button');
        btnDel.type = 'button';
        btnDel.className = 'btn btn-sm btn-outline-danger';
        btnDel.textContent = '✕';
        btnDel.onclick = () => { tr.remove(); syncToHidden(tr.closest('table')); };
        tdDel.appendChild(btnDel);
        tr.appendChild(tdDel);

        // Sync hidden input on change
        tr.querySelectorAll('input[type=number]').forEach(inp => {
            inp.addEventListener('input', () => syncToHidden(tr.closest('table')));
        });

        return tr;
    }

    /** Đọc toàn bộ rows trong tbody → serialize → cập nhật hidden input */
    function syncToHidden(table) {
        const items = [];
        table.querySelectorAll('tbody tr').forEach(tr => {
            const item = {};
            tr.querySelectorAll('input[data-field]').forEach(inp => {
                item[inp.dataset.field] = inp.value;
            });
            items.push(item);
        });
        const hidden = document.getElementById(table.dataset.hiddenId);
        if (hidden) hidden.value = serialize(items);
    }

    /** Khởi tạo editor cho 1 textarea/input */
    function initField(textarea) {
        const raw = textarea.value;
        const fieldName = textarea.name || textarea.id;

        // Tạo container
        const wrapper = document.createElement('div');
        wrapper.className = 'goods-editor-wrapper border rounded p-2 bg-light';

        // Header
        const header = document.createElement('div');
        header.className = 'd-flex align-items-center gap-2 mb-2';
        header.innerHTML = `
            <span class="badge bg-secondary">${fieldName}</span>
            <small class="text-muted">GoodsData editor (7 fields)</small>
            <button type="button" class="btn btn-sm btn-outline-primary ms-auto" id="btn-add-${fieldName}">
                + Thêm item
            </button>
        `;
        wrapper.appendChild(header);

        // Table
        const tableWrapper = document.createElement('div');
        tableWrapper.style.overflowX = 'auto';

        const table = document.createElement('table');
        table.className = 'table table-sm table-bordered mb-1';
        table.dataset.hiddenId = `hidden-${fieldName}`;

        // Thead
        const thead = document.createElement('thead');
        thead.innerHTML = `<tr>
            ${FIELDS.map(f => `<th class="text-nowrap" style="font-size:0.75rem">${f}</th>`).join('')}
            <th></th>
        </tr>`;
        table.appendChild(thead);

        // Tbody
        const tbody = document.createElement('tbody');
        const items = parse(raw);
        items.forEach((item, i) => tbody.appendChild(createRow(item, i)));
        table.appendChild(tbody);
        tableWrapper.appendChild(table);
        wrapper.appendChild(tableWrapper);

        // Hidden input giữ giá trị raw
        const hidden = document.createElement('input');
        hidden.type = 'hidden';
        hidden.id = `hidden-${fieldName}`;
        hidden.name = textarea.name;
        hidden.value = raw;
        wrapper.appendChild(hidden);

        // Ẩn textarea gốc
        textarea.type = 'hidden';
        textarea.name = ''; // bỏ name để không submit
        textarea.style.display = 'none';
        textarea.parentNode.insertBefore(wrapper, textarea);

        // Add row button
        document.getElementById(`btn-add-${fieldName}`).addEventListener('click', () => {
            const newItem = { GoodsID: '0', GCount: '1', Binding: '0',
                              Forge_level: '0', AppendPropLev: '0', Lucky: '0', ExcellenceInfo: '0' };
            const row = createRow(newItem, tbody.children.length);
            tbody.appendChild(row);
            syncToHidden(table);
        });

        return wrapper;
    }

    /** Khởi tạo tất cả Goods fields trong form */
    function init() {
        // Tìm các textarea/input có name trong GOODS_ATTRS
        GOODS_ATTRS.forEach(attrName => {
            document.querySelectorAll(`textarea[name="${attrName}"], input[name="${attrName}"]`)
                .forEach(el => {
                    if (el.dataset.goodsEditorInit) return;
                    el.dataset.goodsEditorInit = '1';
                    initField(el);
                });
        });

        // Auto-detect: textarea/input có value chứa pipe-separated "number,number,...|..."
        document.querySelectorAll('textarea, input[type=text]').forEach(el => {
            if (el.dataset.goodsEditorInit) return;
            if (isGoodsDataFormat(el.value)) {
                el.dataset.goodsEditorInit = '1';
                initField(el);
            }
        });
    }

    /** Kiểm tra value có phải GoodsData format không */
    function isGoodsDataFormat(raw) {
        if (!raw || raw.trim() === '') return false;
        // Pattern: "number,number,number,...|..." ít nhất 7 fields
        const firstChunk = raw.split('|')[0];
        const parts = firstChunk.split(',');
        return parts.length === 7 && parts.every(p => /^\d+$/.test(p.trim()));
    }

    return { init, parse, serialize };
})();

// Auto-init khi DOM ready
document.addEventListener('DOMContentLoaded', () => {
    // Chỉ init trên trang Edit
    if (window.location.pathname.startsWith('/edit/')) {
        GoodsDataEditor.init();
    }
});
