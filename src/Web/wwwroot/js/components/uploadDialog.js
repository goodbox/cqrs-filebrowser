import * as api from '../api.js';
import { formatSize } from '../utils.js';

export function renderUploadDialog(targetPath, onSuccess) {
  const overlay = document.createElement('div');
  overlay.className = 'modal-overlay';
  overlay.innerHTML = `
    <div class="modal" role="dialog" aria-modal="true">
      <div class="modal-header">
        <h2>Upload Files</h2>
        <button class="modal-close" title="Close">×</button>
      </div>
      <div class="drop-zone" id="drop-zone">
        <p>Drop files here or click to browse</p>
        <input type="file" class="file-input" id="file-input" multiple>
      </div>
      <div class="upload-file-list" id="file-list"></div>
      <div class="upload-progress" id="progress-wrap" style="display:none">
        <div class="upload-progress-bar" id="progress-bar"></div>
      </div>
      <div class="modal-footer">
        <button class="btn" id="upload-cancel">Cancel</button>
        <button class="btn btn-primary" id="upload-confirm" disabled>Upload</button>
      </div>
    </div>
  `;

  document.body.appendChild(overlay);

  let selectedFiles = [];

  const dropZone = overlay.querySelector('#drop-zone');
  const fileInput = overlay.querySelector('#file-input');
  const fileListEl = overlay.querySelector('#file-list');
  const confirmBtn = overlay.querySelector('#upload-confirm');
  const progressWrap = overlay.querySelector('#progress-wrap');
  const progressBar = overlay.querySelector('#progress-bar');

  const close = () => document.body.removeChild(overlay);

  overlay.querySelector('.modal-close').addEventListener('click', close);
  overlay.querySelector('#upload-cancel').addEventListener('click', close);
  overlay.addEventListener('click', (e) => { if (e.target === overlay) close(); });

  dropZone.addEventListener('click', () => fileInput.click());

  dropZone.addEventListener('dragover', (e) => {
    e.preventDefault();
    dropZone.classList.add('drag-over');
  });

  dropZone.addEventListener('dragleave', () => dropZone.classList.remove('drag-over'));

  dropZone.addEventListener('drop', (e) => {
    e.preventDefault();
    dropZone.classList.remove('drag-over');
    setFiles(Array.from(e.dataTransfer.files));
  });

  fileInput.addEventListener('change', () => setFiles(Array.from(fileInput.files)));

  function setFiles(files) {
    selectedFiles = files;
    fileListEl.innerHTML = files.map(f =>
      `<div class="upload-file-item">📄 <span>${f.name}</span> <span style="margin-left:auto;color:var(--text-muted)">${formatSize(f.size)}</span></div>`
    ).join('');
    confirmBtn.disabled = files.length === 0;
  }

  confirmBtn.addEventListener('click', async () => {
    if (!selectedFiles.length) return;

    confirmBtn.disabled = true;
    progressWrap.style.display = '';
    progressBar.style.width = '0%';

    try {
      await api.upload(targetPath, selectedFiles, (pct) => {
        progressBar.style.width = `${Math.round(pct * 100)}%`;
      });
      close();
      onSuccess();
    } catch (err) {
      alert(`Upload failed: ${err.message}`);
      confirmBtn.disabled = false;
      progressWrap.style.display = 'none';
    }
  });
}
