document.addEventListener('DOMContentLoaded', () => {
  const overlay = document.createElement('div');
  overlay.className = 'modal-overlay';
  overlay.setAttribute('aria-hidden', 'true');

  const modal = document.createElement('div');
  modal.className = 'modal-content';
  modal.setAttribute('role', 'dialog');
  modal.setAttribute('aria-modal', 'true');

  const image = document.createElement('img');
  image.className = 'modal-img';
  image.alt = '';

  const caption = document.createElement('div');
  caption.className = 'modal-caption';

  const closeButton = document.createElement('button');
  closeButton.className = 'modal-close';
  closeButton.type = 'button';
  closeButton.innerText = '✕';
  closeButton.setAttribute('aria-label', 'Закрыть');

  modal.appendChild(closeButton);
  modal.appendChild(image);
  modal.appendChild(caption);
  overlay.appendChild(modal);
  document.body.appendChild(overlay);

  let lastFocused = null;

  function openModal(src, alt, captionText) {
    lastFocused = document.activeElement;
    image.src = src;
    image.alt = alt || '';
    caption.textContent = captionText || '';
    overlay.style.display = 'flex';
    overlay.setAttribute('aria-hidden', 'false');
    document.body.style.overflow = 'hidden';
    closeButton.focus();
  }

  function closeModal() {
    overlay.style.display = 'none';
    overlay.setAttribute('aria-hidden', 'true');
    document.body.style.overflow = '';
    image.src = '';
    if (lastFocused && typeof lastFocused.focus === 'function') lastFocused.focus();
  }

  overlay.addEventListener('click', e => {
    if (e.target === overlay) closeModal();
  });

  closeButton.addEventListener('click', closeModal);

  document.addEventListener('keydown', e => {
    if (e.key === 'Escape' && overlay.style.display === 'flex') {
      closeModal();
    }
  });

  const stepImages = document.querySelectorAll('.recipe-page .steps .step img');
  stepImages.forEach(thumb => {
    thumb.style.cursor = 'zoom-in';
    thumb.addEventListener('click', () => {
      const step = thumb.closest('.step');
      const heading = step ? step.querySelector('.step-content h3') : null;
      const largeSrc = thumb.dataset.large || thumb.src;
      openModal(largeSrc, thumb.alt, heading ? heading.textContent : '');
    });
  });
});