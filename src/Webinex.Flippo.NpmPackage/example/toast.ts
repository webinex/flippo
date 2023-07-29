export function toast(text: string) {
  let container = document.getElementById('toast_container');

  if (!container) {
    container = document.createElement('div');
    container.id = 'toast_container';
    container.classList.add('toast-container');
    document.body.appendChild(container);
  }

  const toast = document.createElement('span');
  toast.textContent = text;

  toast.classList.add('toast');

  container.appendChild(toast);

  setTimeout(() => container?.removeChild(toast), 5000);
}
