document.addEventListener('DOMContentLoaded', () => {
  const form = document.getElementById('recipeForm');
  if (!form) return;
  const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/webp'];
  const MAX_IMAGE_SIZE = 2 * 1024 * 1024;

  // === Серые подсказки (видны сразу, исчезают при валидности) ===
  const hints = {
    title: "Обязательно, от 3 до 120 символов",
    description: "Обязательно, от 10 до 250 символов",
    author: "От 2 до 60 символов или пусто",
    cuisine: "От 3 до 60 символов, только буквы/пробелы/дефисы",
    difficulty: "Обязательно выбрать сложность",
    "ingredientNames[]": "Укажите название ингредиента",
    "ingredientAmounts[]": "Укажите количество ингредиента",
    "steps[]": "Добавьте хотя бы один шаг (от 10 до 1000 символов)",
    image: "JPG/PNG/WebP до 2 МБ"
  };

  function getErrorEl(input) {
    const field = input.closest('.field');
    return field ? field.querySelector('.error-msg') : null;
  }

  function showHint(input, text) {
    const el = getErrorEl(input);
    if (el) {
      el.textContent = text;
      el.style.color = "var(--muted)";
    }
  }

  function clearHint(input) {
    const el = getErrorEl(input);
    if (el) el.textContent = "";
  }

  function isImageValid(file) {
    return ALLOWED_IMAGE_TYPES.includes(file.type) && file.size <= MAX_IMAGE_SIZE;
  }

  function isFieldValid(input) {
    if (input.name === "title") {
      const v = input.value.trim();
      return v.length >= 3 && v.length <= 120;
    }
    if (input.name === "description") {
      const v = input.value.trim();
      return v.length >= 10 && v.length <= 250;
    }
    if (input.name === "author") {
      const v = input.value.trim();
      return !v || (v.length >= 2 && v.length <= 60);
    }
    if (input.name === "cuisine") {
      const v = input.value.trim();
      return !v || (v.length >= 3 && v.length <= 60 && /^[\p{L}\s\-]+$/u.test(v));
    }
    if (input.name === "difficulty") return ["easy", "medium", "hard"].includes(input.value);
    if (input.name === "ingredientNames[]") {
      const v = input.value.trim();
      return v.length > 0 && v.length <= 80;
    }
    if (input.name === "ingredientAmounts[]") {
      const v = input.value.trim();
      return v.length > 0 && v.length <= 40;
    }
    if (input.name === "steps[]") {
      const v = input.value.trim();
      return v.length >= 10 && v.length <= 1000;
    }
    if (input.id === "recipeImage") {
      if (!input.files?.[0]) return true;
      return isImageValid(input.files[0]);
    }
    return true;
  }

  function updateHint(input) {
    if (isFieldValid(input)) {
      clearHint(input);
    } else {
      const key = input.name || (input.id === "recipeImage" ? "image" : "");
      if (hints[key]) showHint(input, hints[key]);
    }
  }

  // === Инициализация всех полей ===
  form.querySelectorAll('input, textarea, select').forEach(input => {
    const key = input.name || (input.id === "recipeImage" ? "image" : "");
    if (hints[key]) showHint(input, hints[key]);

    input.addEventListener('input', () => updateHint(input));
    input.addEventListener('change', () => updateHint(input));
  });

  // === Главное изображение — только подсказка (редактор открывает add-recipe.js) ===
  const imageInput = document.getElementById('recipeImage');
  if (imageInput) {
    showHint(imageInput, hints.image);
    // НИЧЕГО НЕ ДЕЛАЕМ с change — это делает add-recipe.js
    // Он уже читает файл и вызывает openImageEditor()
  }

  // === Авторазмер описания + счётчик ===
  const descEl = form.querySelector('[name="description"]');
  const descCounter = document.getElementById('descCount');
  const MAX_DESC = 250;

  function autosizeTextarea(el) {
    if (!el) return;
    el.style.height = 'auto';
    el.style.height = (el.scrollHeight + 2) + 'px';
  }

  if (descEl) {
    if (!descEl.hasAttribute('maxlength')) descEl.setAttribute('maxlength', MAX_DESC);
    if (descCounter) descCounter.textContent = String(descEl.value.length);
    autosizeTextarea(descEl);

    descEl.addEventListener('input', () => {
      const v = descEl.value;
      if (v.length > MAX_DESC) descEl.value = v.slice(0, MAX_DESC);
      if (descCounter) descCounter.textContent = String(descEl.value.length);
      autosizeTextarea(descEl);
      updateHint(descEl);
    });
  }

  // === Твоя оригинальная валидация на submit (без изменений) ===
  form.addEventListener('submit', (e) => {
    const errors = [];
    let firstInvalid = null;

    const title = form.querySelector('[name="title"]');
    const desc = form.querySelector('[name="description"]');
    const author = form.querySelector('[name="author"]');
    const cuisine = form.querySelector('[name="cuisine"]');
    const ingredientNameInputs = Array.from(form.querySelectorAll('input[name="ingredientNames[]"]'));
    const ingredientAmountInputs = Array.from(form.querySelectorAll('input[name="ingredientAmounts[]"]'));
    const stepTextareas = Array.from(form.querySelectorAll('textarea[name="steps[]"]'));
    const stepImageInputs = Array.from(form.querySelectorAll('.step input[type="file"]'));
    const difficulty = form.querySelector('[name="difficulty"]');
    const imageInput = form.querySelector('#recipeImage');

    if (!title || title.value.trim().length < 3 || title.value.trim().length > 120) {
      errors.push('Название должно быть от 3 до 120 символов');
      if (!firstInvalid) firstInvalid = title;
    }

    if (!desc || desc.value.trim().length < 10 || desc.value.trim().length > 250) {
      errors.push('Описание должно быть от 10 до 250 символов');
      if (!firstInvalid) firstInvalid = desc;
    }

    if (author && author.value.trim().length > 0 && (author.value.trim().length < 2 || author.value.trim().length > 60)) {
      errors.push('Имя автора должно быть от 2 до 60 символов или оставьте поле пустым');
      if (!firstInvalid) firstInvalid = author;
    }

    if (cuisine && cuisine.value.trim().length > 0) {
      if (cuisine.value.trim().length < 3 || cuisine.value.trim().length > 60) {
        errors.push('Поле "Кухня" должно быть от 3 до 60 символов');
        if (!firstInvalid) firstInvalid = cuisine;
      } else if (!/^[\p{L}\s\-]+$/u.test(cuisine.value.trim())) {
        errors.push('Поле "Кухня" должно содержать только буквы, пробелы и дефисы');
        if (!firstInvalid) firstInvalid = cuisine;
      }
    }

    const ingredientPairs = ingredientNameInputs.map((nameInput, idx) => ({
      nameInput,
      amountInput: ingredientAmountInputs[idx],
      name: nameInput.value.trim(),
      amount: ingredientAmountInputs[idx]?.value.trim() || ''
    }));

    const completeIngredients = ingredientPairs.filter(p => p.name.length > 0 && p.amount.length > 0);
    if (completeIngredients.length === 0) {
      errors.push('Добавьте хотя бы один ингредиент с названием и количеством');
      if (!firstInvalid && ingredientNameInputs.length) firstInvalid = ingredientNameInputs[0];
    }

    const tooLongIngredient = ingredientPairs.find(p => p.name.length > 80 || p.amount.length > 40);
    if (tooLongIngredient) {
      errors.push('Название ингредиента до 80 символов, количество до 40 символов');
      if (!firstInvalid) firstInvalid = tooLongIngredient.nameInput;
    }

    const halfFilledIngredient = ingredientPairs.find(p => (p.name.length > 0) !== (p.amount.length > 0));
    if (halfFilledIngredient) {
      errors.push('Для каждого ингредиента заполните и название, и количество');
      if (!firstInvalid) firstInvalid = halfFilledIngredient.nameInput;
    }

    const stepsFilled = stepTextareas.map(t => t.value.trim()).filter(v => v.length > 0);
    if (stepsFilled.length === 0) {
      errors.push('Добавьте хотя бы один шаг приготовления');
      if (!firstInvalid && stepTextareas.length) firstInvalid = stepTextareas[0];
    }

    const invalidStep = stepTextareas.find(t => {
      const v = t.value.trim();
      return v.length > 0 && (v.length < 10 || v.length > 1000);
    });
    if (invalidStep) {
      errors.push('Каждый шаг должен быть от 10 до 1000 символов');
      if (!firstInvalid) firstInvalid = invalidStep;
    }

    if (!difficulty || !['easy', 'medium', 'hard'].includes(difficulty.value)) {
      errors.push('Выберите корректную сложность');
      if (!firstInvalid) firstInvalid = difficulty;
    }

    if (imageInput && imageInput.files && imageInput.files.length > 0) {
      const file = imageInput.files[0];
      if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
        errors.push('Изображение должно быть в формате JPG/PNG/WebP');
        if (!firstInvalid) firstInvalid = imageInput;
      } else if (file.size > MAX_IMAGE_SIZE) {
        errors.push('Изображение не должно быть больше 2 МБ');
        if (!firstInvalid) firstInvalid = imageInput;
      }
    }

    const badStepImage = stepImageInputs.find(input => input.files?.[0] && !isImageValid(input.files[0]));
    if (badStepImage) {
      errors.push('Изображения шагов должны быть JPG/PNG/WebP и не более 2 МБ');
      if (!firstInvalid) firstInvalid = badStepImage;
    }

    if (errors.length) {
      e.preventDefault();
      alert('Ошибка в форме:\n- ' + errors.join('\n- '));
      if (firstInvalid && typeof firstInvalid.focus === 'function') firstInvalid.focus();
    }
  });
});
