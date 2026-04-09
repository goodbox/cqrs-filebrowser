import { escapeHtml } from '../utils.js';
import { navigate } from '../router.js';

export function renderBreadcrumb(container, currentPath) {
  const parts = currentPath ? currentPath.split('/').filter(Boolean) : [];

  let html = '<nav class="breadcrumb">';
  html += `<span class="breadcrumb-item ${parts.length === 0 ? 'active' : ''}" data-path="">Root</span>`;

  parts.forEach((part, i) => {
    const path = parts.slice(0, i + 1).join('/');
    const isLast = i === parts.length - 1;
    html += `<span class="breadcrumb-sep">/</span>`;
    html += `<span class="breadcrumb-item ${isLast ? 'active' : ''}" data-path="${escapeHtml(path)}">${escapeHtml(part)}</span>`;
  });

  html += '</nav>';
  container.innerHTML = html;

  container.querySelectorAll('.breadcrumb-item:not(.active)').forEach(el => {
    el.addEventListener('click', () => navigate(el.dataset.path));
  });
}
