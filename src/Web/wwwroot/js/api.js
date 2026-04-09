const BASE = '/api/fs';

async function request(url) {
  const res = await fetch(url);
  if (!res.ok) {
    let msg = `HTTP ${res.status}`;
    try { const body = await res.json(); msg = body.detail || body.title || msg; } catch {}
    throw new Error(msg);
  }
  return res.json();
}

export function browse(path = '') {
  return request(`${BASE}/browse?path=${encodeURIComponent(path)}`);
}

export function search(query, path = '', recursive = false) {
  return request(`${BASE}/search?q=${encodeURIComponent(query)}&path=${encodeURIComponent(path)}&recursive=${recursive}`);
}

export function download(path) {
  const url = `${BASE}/download?path=${encodeURIComponent(path)}`;
  const a = document.createElement('a');
  a.href = url;
  a.download = '';
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
}

export function upload(targetPath, files, onProgress) {
  return new Promise((resolve, reject) => {
    const formData = new FormData();
    for (const file of files) formData.append('files', file);

    const xhr = new XMLHttpRequest();
    xhr.open('POST', `${BASE}/upload?path=${encodeURIComponent(targetPath)}`);

    xhr.upload.onprogress = (e) => {
      if (e.lengthComputable && onProgress) onProgress(e.loaded / e.total);
    };

    xhr.onload = () => {
      if (xhr.status >= 200 && xhr.status < 300) {
        try { resolve(JSON.parse(xhr.responseText)); }
        catch { resolve([]); }
      } else {
        let msg = `HTTP ${xhr.status}`;
        try { const body = JSON.parse(xhr.responseText); msg = body.detail || body.title || msg; } catch {}
        reject(new Error(msg));
      }
    };

    xhr.onerror = () => reject(new Error('Network error'));
    xhr.send(formData);
  });
}

export async function deleteEntry(path) {
  const res = await fetch(`${BASE}/entry?path=${encodeURIComponent(path)}`, { method: 'DELETE' });
  if (!res.ok) {
    let msg = `HTTP ${res.status}`;
    try { const body = await res.json(); msg = body.detail || body.title || msg; } catch {}
    throw new Error(msg);
  }
}

export async function moveEntry(from, to) {
  const res = await fetch(`${BASE}/move`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ from, to }),
  });
  if (!res.ok) {
    let msg = `HTTP ${res.status}`;
    try { const body = await res.json(); msg = body.detail || body.title || msg; } catch {}
    throw new Error(msg);
  }
}
