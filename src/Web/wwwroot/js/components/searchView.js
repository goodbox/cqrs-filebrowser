import { escapeHtml, formatDate, fileIcon } from '../utils.js';
import { navigate } from '../router.js';
import * as api from '../api.js';

export function renderSearchView(container, searchResult) {
  if (!searchResult) {
    container.innerHTML = '';
    return;
  }

  const { matches, totalMatches, query } = searchResult;

  let html = `
    <div class="search-header">
      <h2>Search results for "${escapeHtml(query)}"</h2>
      <span class="search-badge">${totalMatches}</span>
    </div>
  `;

  if (matches.length === 0) {
    html += '<div class="empty-state">No matches found.</div>';
    container.innerHTML = html;
    return;
  }

  html += `
    <table class="file-table">
      <thead>
        <tr>
          <th style="width:100%">Name / Path</th>
          <th class="col-size">Size</th>
          <th class="col-date">Modified</th>
        </tr>
      </thead>
      <tbody>
  `;

  for (const match of matches) {
    const icon = match.isDirectory ? '📁' : fileIcon(match.relativePath.split('.').pop());
    html += `
      <tr>
        <td>
          <div class="file-name-cell">
            <span class="file-icon">${icon}</span>
            <span class="file-name-link" data-type="${match.isDirectory ? 'dir' : 'file'}" data-path="${escapeHtml(match.relativePath)}">
              ${escapeHtml(match.name)}
              <small style="color:var(--text-muted);font-size:11px;display:block">${escapeHtml(match.relativePath)}</small>
            </span>
          </div>
        </td>
        <td class="col-size">${match.sizeFormatted || '—'}</td>
        <td class="col-date">${formatDate(match.lastModified)}</td>
      </tr>
    `;
  }

  html += '</tbody></table>';
  container.innerHTML = html;

  container.querySelectorAll('.file-name-link').forEach(el => {
    el.addEventListener('click', () => {
      if (el.dataset.type === 'dir') {
        navigate(el.dataset.path);
      } else {
        api.download(el.dataset.path);
      }
    });
  });
}
