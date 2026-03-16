document.addEventListener('DOMContentLoaded', () => {
  // Создаём элементы модального окна один раз
  const overlay = document.createElement('div');
  overlay.className = 'modal-overlay';
  overlay.setAttribute('aria-hidden', 'true');

  const modal = document.createElement('div');
  modal.className = 'modal-content';
  modal.setAttribute('role', 'dialog');
  modal.setAttribute('aria-modal', 'true');

  const img = document.createElement('img');
  img.className = 'modal-img';
  img.alt = '';

  const caption = document.createElement('div');
  caption.className = 'modal-caption';

  const closeBtn = document.createElement('button');
  closeBtn.className = 'modal-close';
  closeBtn.type = 'button';
  closeBtn.innerText = '✕';
  closeBtn.setAttribute('aria-label', 'Закрыть');

  modal.appendChild(closeBtn);
  modal.appendChild(img);
  modal.appendChild(caption);
  overlay.appendChild(modal);
  document.body.appendChild(overlay);

  let lastFocused = null;

  function openModal(src, alt, captionText) {
    lastFocused = document.activeElement;
    img.src = src;
    img.alt = alt || '';
    caption.textContent = captionText || '';
    overlay.style.display = 'flex';
    overlay.setAttribute('aria-hidden', 'false');
    document.body.style.overflow = 'hidden';
    // focus for accessibility
    closeBtn.focus();
  }

  function closeModal() {
    overlay.style.display = 'none';
    overlay.setAttribute('aria-hidden', 'true');
    document.body.style.overflow = '';
    img.src = '';
    if (lastFocused && typeof lastFocused.focus === 'function') lastFocused.focus();
  }

  // Закрытие по оверле
  overlay.addEventListener('click', (e) => {
    if (e.target === overlay) closeModal();
  });
  closeBtn.addEventListener('click', closeModal);

  // Закрытие по Esc
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && overlay.style.display === 'flex') {
      closeModal();
    }
  });

  // Навешиваем обработчики на изображения шагов в рецептах
  const stepImgs = document.querySelectorAll('.recipe-page .steps .step img');
  stepImgs.forEach((thumb) => {
    // указатель-кей для юзера
    thumb.style.cursor = 'zoom-in';
    thumb.addEventListener('click', () => {
      // подпись — ищем заголовок шага рядом
      const step = thumb.closest('.step');
      const h3 = step ? step.querySelector('.step-content h3') : null;
      const captionText = h3 ? h3.textContent : '';
      // открываем модал с оригинальным src (если доступно) или с тем же
      const largeSrc = thumb.dataset.large || thumb.src;
      openModal(largeSrc, thumb.alt, captionText);
    });
  });
});
