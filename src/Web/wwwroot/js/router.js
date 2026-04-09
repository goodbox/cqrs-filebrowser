import { setState } from './state.js';

let _onNavigate = null;

export function initRouter(onNavigate) {
  _onNavigate = onNavigate;

  window.addEventListener('popstate', () => {
    syncStateFromUrl();
    _onNavigate();
  });

  syncStateFromUrl();
}

export function navigate(path, searchQuery = '', recursive = false) {
  const params = new URLSearchParams();
  if (path) params.set('path', path);
  if (searchQuery) params.set('q', searchQuery);
  if (recursive) params.set('recursive', '1');

  const search = params.toString() ? '?' + params.toString() : '';
  const url = window.location.pathname + search;
  history.pushState({}, '', url);

  setState({ currentPath: path, searchQuery, recursive });
  _onNavigate();
}

function syncStateFromUrl() {
  const params = new URLSearchParams(window.location.search);
  setState({
    currentPath: params.get('path') || '',
    searchQuery: params.get('q') || '',
    recursive: params.get('recursive') === '1',
  });
}
