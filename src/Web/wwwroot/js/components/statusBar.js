export function renderStatusBar(container, listing) {
  if (!listing) {
    container.innerHTML = '<div class="status-bar"></div>';
    return;
  }

  container.innerHTML = `
    <div class="status-bar">
      <span>📁 ${listing.totalDirectoryCount} folder${listing.totalDirectoryCount !== 1 ? 's' : ''}</span>
      <span>📄 ${listing.totalFileCount} file${listing.totalFileCount !== 1 ? 's' : ''}</span>
      <span>💾 ${listing.totalSizeFormatted}</span>
    </div>
  `;
}
