import { state, setState } from '../state.js';
import * as api from '../api.js';
import { renderBreadcrumb } from './breadcrumb.js';
import { renderFileList } from './fileList.js';
import { renderStatusBar } from './statusBar.js';
import { renderSearchView } from './searchView.js';
import { renderUploadDialog } from './uploadDialog.js';

export async function renderApp(appEl) {
  appEl.innerHTML = `
    <div id="toolbar"></div>
    <div id="breadcrumb-container"></div>
    <div class="main-content" id="main-content"></div>
    <div id="status-bar-container"></div>
  `;

  const toolbarEl = appEl.querySelector('#toolbar');
  const breadcrumbEl = appEl.querySelector('#breadcrumb-container');
  const mainEl = appEl.querySelector('#main-content');
  const statusBarEl = appEl.querySelector('#status-bar-container');

  renderToolbar(toolbarEl);
  await loadContent(breadcrumbEl, mainEl, statusBarEl);
}

function renderToolbar(toolbarEl) {
  toolbarEl.innerHTML = `
    <div class="toolbar">
      <h1>📂 File Browser</h1>
      <form class="search-form" id="search-form">
        <input type="text" id="search-input" placeholder="Search files and folders…" value="${escapeValue(state.searchQuery)}">
        <label><input type="checkbox" id="recursive-check" ${state.recursive ? 'checked' : ''}> Recursive</label>
        <button type="submit" class="btn btn-primary">Search</button>
        ${state.searchQuery ? '<button type="button" id="clear-search" class="btn">Clear</button>' : ''}
      </form>
      <button class="btn btn-primary" id="upload-btn">⬆️ Upload</button>
    </div>
  `;

  const form = toolbarEl.querySelector('#search-form');
  form.addEventListener('submit', (e) => {
    e.preventDefault();
    const q = toolbarEl.querySelector('#search-input').value.trim();
    const recursive = toolbarEl.querySelector('#recursive-check').checked;
    import('../router.js').then(({ navigate }) => navigate(state.currentPath, q, recursive));
  });

  const clearBtn = toolbarEl.querySelector('#clear-search');
  if (clearBtn) {
    clearBtn.addEventListener('click', () => {
      import('../router.js').then(({ navigate }) => navigate(state.currentPath, '', false));
    });
  }

  toolbarEl.querySelector('#upload-btn').addEventListener('click', () => {
    renderUploadDialog(state.currentPath, () => {
      import('../router.js').then(({ navigate }) => navigate(state.currentPath, state.searchQuery, state.recursive));
    });
  });
}

async function loadContent(breadcrumbEl, mainEl, statusBarEl) {
  setState({ loading: true, error: null });
  mainEl.innerHTML = '<div class="loading">Loading…</div>';

  try {
    if (state.searchQuery) {
      const result = await api.search(state.searchQuery, state.currentPath, state.recursive);
      setState({ searchResult: result, listing: null, loading: false });
      renderBreadcrumb(breadcrumbEl, state.currentPath);
      renderSearchView(mainEl, state.searchResult);
      renderStatusBar(statusBarEl, null);
    } else {
      const listing = await api.browse(state.currentPath);
      setState({ listing, searchResult: null, loading: false });
      renderBreadcrumb(breadcrumbEl, listing.currentPath);
      renderFileList(mainEl, state.listing, () => {
        import('../router.js').then(({ navigate }) => navigate(state.currentPath, state.searchQuery, state.recursive));
      });
      renderStatusBar(statusBarEl, state.listing);
    }
  } catch (err) {
    setState({ loading: false, error: err.message });
    mainEl.innerHTML = `<div class="error-state">Error: ${err.message}</div>`;
    renderStatusBar(statusBarEl, null);
  }
}

function escapeValue(str) {
  return String(str || '').replace(/"/g, '&quot;');
}
