import { initRouter } from './router.js';
import { renderApp } from './components/fileBrowser.js';

const appEl = document.getElementById('app');

initRouter(() => renderApp(appEl));
renderApp(appEl);
