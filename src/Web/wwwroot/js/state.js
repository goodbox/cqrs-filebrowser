export const state = {
  currentPath: '',
  searchQuery: '',
  recursive: false,
  listing: null,
  searchResult: null,
  loading: false,
  error: null,
};

const subscribers = [];

export function subscribe(fn) {
  subscribers.push(fn);
}

export function setState(patch) {
  Object.assign(state, patch);
  subscribers.forEach(fn => fn(state));
}
