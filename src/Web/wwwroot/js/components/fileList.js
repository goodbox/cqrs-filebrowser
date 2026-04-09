import { escapeHtml, formatDate, fileIcon } from '../utils.js';
import { navigate } from '../router.js';
import * as api from '../api.js';
import { renderMoveDialog } from './moveDialog.js';

export function renderFileList(container, listing, onRefresh) {
  if (!listing) {
    container.innerHTML = '<div class="empty-state">No data</div>';
    return;
  }

  const { directories, files } = listing;

  if (directories.length === 0 && files.length === 0) {
    container.innerHTML = '<div class="empty-state">This folder is empty.</div>';
    return;
  }

  let html = `
    <table class="file-table">
      <thead>
        <tr>
          <th style="width:100%">Name</th>
          <th class="col-size">Size</th>
          <th class="col-date">Modified</th>
          <th class="col-actions">Actions</th>
        </tr>
      </thead>
      <tbody>
  `;

  for (const dir of directories) {
    html += `
      <tr>
        <td>
          <div class="file-name-cell">
            <span class="file-icon">📁</span>
            <span class="file-name-link" data-type="dir" data-path="${escapeHtml(dir.relativePath)}">${escapeHtml(dir.name)}</span>
          </div>
        </td>
        <td class="col-size">${dir.totalSizeFormatted || '—'}</td>
        <td class="col-date">${formatDate(dir.lastModified)}</td>
        <td class="col-actions">
          <button class="btn btn-sm btn-move" data-path="${escapeHtml(dir.relativePath)}" title="Move/Rename">✏️</button>
          <button class="btn btn-sm btn-danger btn-delete" data-path="${escapeHtml(dir.relativePath)}" title="Delete">🗑️</button>
        </td>
      </tr>
    `;
  }

  for (const file of files) {
    html += `
      <tr>
        <td>
          <div class="file-name-cell">
            <span class="file-icon">${fileIcon(file.extension)}</span>
            <span class="file-name-link" data-type="file" data-path="${escapeHtml(file.relativePath)}">${escapeHtml(file.name)}</span>
          </div>
        </td>
        <td class="col-size">${file.sizeFormatted || '—'}</td>
        <td class="col-date">${formatDate(file.lastModified)}</td>
        <td class="col-actions">
          <button class="btn btn-sm btn-move" data-path="${escapeHtml(file.relativePath)}" title="Move/Rename">✏️</button>
          <button class="btn btn-sm btn-danger btn-delete" data-path="${escapeHtml(file.relativePath)}" title="Delete">🗑️</button>
        </td>
      </tr>
    `;
  }

  html += '</tbody></table>';
  container.innerHTML = html;

  // Navigate on directory/file click
  container.querySelectorAll('.file-name-link').forEach(el => {
    el.addEventListener('click', () => {
      if (el.dataset.type === 'dir') {
        navigate(el.dataset.path);
      } else {
        api.download(el.dataset.path);
      }
    });
  });

  // Delete buttons
  container.querySelectorAll('.btn-delete').forEach(btn => {
    btn.addEventListener('click', async (e) => {
      e.stopPropagation();
      const path = btn.dataset.path;
      if (!confirm(`Delete "${path}"?`)) return;
      try {
        await api.deleteEntry(path);
        onRefresh();
      } catch (err) {
        alert(`Delete failed: ${err.message}`);
      }
    });
  });

  // Move buttons
  container.querySelectorAll('.btn-move').forEach(btn => {
    btn.addEventListener('click', (e) => {
      e.stopPropagation();
      renderMoveDialog(btn.dataset.path, () => onRefresh());
    });
  });
}
