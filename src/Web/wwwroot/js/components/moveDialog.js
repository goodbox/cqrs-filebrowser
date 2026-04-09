import * as api from '../api.js';

export function renderMoveDialog(currentPath, onSuccess) {
  const overlay = document.createElement('div');
  overlay.className = 'modal-overlay';
  overlay.innerHTML = `
    <div class="modal" role="dialog" aria-modal="true">
      <div class="modal-header">
        <h2>Move / Rename</h2>
        <button class="modal-close" title="Close">×</button>
      </div>
      <div class="move-form">
        <label>From</label>
        <input type="text" id="move-from" value="${currentPath}" readonly>
        <label>To (new relative path)</label>
        <input type="text" id="move-to" value="${currentPath}">
      </div>
      <div class="modal-footer">
        <button class="btn" id="move-cancel">Cancel</button>
        <button class="btn btn-primary" id="move-confirm">Move</button>
      </div>
    </div>
  `;

  document.body.appendChild(overlay);

  const toInput = overlay.querySelector('#move-to');
  toInput.focus();
  toInput.select();

  const close = () => document.body.removeChild(overlay);

  overlay.querySelector('.modal-close').addEventListener('click', close);
  overlay.querySelector('#move-cancel').addEventListener('click', close);
  overlay.addEventListener('click', (e) => { if (e.target === overlay) close(); });

  overlay.querySelector('#move-confirm').addEventListener('click', async () => {
    const from = overlay.querySelector('#move-from').value;
    const to = toInput.value.trim();
    if (!to || to === from) { close(); return; }

    try {
      await api.moveEntry(from, to);
      close();
      onSuccess();
    } catch (err) {
      alert(`Move failed: ${err.message}`);
    }
  });

  toInput.addEventListener('keydown', (e) => {
    if (e.key === 'Enter') overlay.querySelector('#move-confirm').click();
    if (e.key === 'Escape') close();
  });
}
