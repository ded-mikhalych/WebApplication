document.addEventListener("DOMContentLoaded", () => {
  const ingredientsDiv = document.getElementById("ingredients");
  const stepsDiv = document.getElementById("steps");
  const addIngredientBtn = document.getElementById("addIngredient");
  const addStepBtn = document.getElementById("addStep");
  let stepCount = 0;

  // === Кухня: загружаем список и обрабатываем добавление новой ===
  const cuisineSelect = document.getElementById("cuisineSelect");
  const newCuisineForm = document.getElementById("newCuisineForm");
  const newCuisineNameInput = document.getElementById("newCuisineName");
  const addCuisineBtn = document.getElementById("addCuisineBtn");
  const cancelNewCuisineBtn = document.getElementById("cancelNewCuisine");
  const newCuisineError = document.getElementById("newCuisineError");

  async function loadCuisines(selectValue) {
    try {
      const response = await fetch("/api/recipe/cuisines");
      const result = await response.json();
      if (!result.success) return;

      // Сохраняем текущий выбор, затем перестраиваем опции
      cuisineSelect.innerHTML = '<option value="">Не выбрана</option>';
      result.cuisines.forEach(c => {
        const opt = document.createElement("option");
        opt.value = c.id;
        opt.textContent = c.displayName;
        cuisineSelect.appendChild(opt);
      });
      const addNewOpt = document.createElement("option");
      addNewOpt.value = "__new__";
      addNewOpt.textContent = "— Добавить новую —";
      cuisineSelect.appendChild(addNewOpt);

      if (selectValue != null) {
        cuisineSelect.value = String(selectValue);
      }
    } catch (e) {
      console.error("Не удалось загрузить список кухонь:", e);
    }
  }

  loadCuisines(null);

  cuisineSelect.addEventListener("change", () => {
    if (cuisineSelect.value === "__new__") {
      newCuisineForm.style.display = "block";
      newCuisineNameInput.focus();
    } else {
      newCuisineForm.style.display = "none";
    }
  });

  cancelNewCuisineBtn.addEventListener("click", () => {
    newCuisineForm.style.display = "none";
    newCuisineNameInput.value = "";
    newCuisineError.textContent = "";
    cuisineSelect.value = "";
  });

  addCuisineBtn.addEventListener("click", async () => {
    const name = newCuisineNameInput.value.trim();
    newCuisineError.textContent = "";
    if (!name) {
      newCuisineError.textContent = "Введите название кухни";
      return;
    }

    addCuisineBtn.disabled = true;
    try {
      const response = await fetch("/api/recipe/cuisines", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ displayName: name })
      });
      const result = await response.json();
      if (!response.ok || !result.success) {
        newCuisineError.textContent = result.message || "Не удалось добавить кухню";
        return;
      }
      await loadCuisines(result.id);
      newCuisineForm.style.display = "none";
      newCuisineNameInput.value = "";
    } catch (e) {
      newCuisineError.textContent = "Ошибка при добавлении кухни";
    } finally {
      addCuisineBtn.disabled = false;
    }
  });

  // Добавление ингредиента
  addIngredientBtn.addEventListener("click", () => {
    const div = document.createElement("div");
    div.classList.add("ingredient");
    div.innerHTML = `
      <div class="input-row">
        <input type="text" name="ingredientNames[]" placeholder="Название ингредиента" maxlength="80" required>
        <input type="text" name="ingredientAmounts[]" placeholder="Количество" maxlength="40" required>
        <button type="button" onclick="this.closest('.ingredient').remove()">Удалить</button>
      </div>
    `;
    ingredientsDiv.appendChild(div);
  });

  // Добавление шага с картинкой
  addStepBtn.addEventListener("click", () => {
    stepCount++;
    const div = document.createElement("div");
    div.classList.add("step");
    div.setAttribute('draggable', 'true');
    div.innerHTML = `
      <span class="drag-handle" title="Перетащите, чтобы изменить порядок">☰</span>
      <h3>Шаг ${stepCount}</h3>
      <textarea name="steps[]" placeholder="Описание шага" required></textarea><br>
      <input type="file" name="stepImage${stepCount}" accept="image/*">
      <img class="preview" alt="Предпросмотр шага" style="display:none;">
      <button type="button" onclick="this.parentElement.remove()">Удалить шаг</button>
    `;
    stepsDiv.appendChild(div);
    makeStepDraggable(div);
    updateStepNumbers();

    // Навешиваем обработчик на только что добавленный input file
    const fileInput = div.querySelector('input[type="file"]');
    const previewImg = div.querySelector('img.preview');

    fileInput.addEventListener("change", () => handleFileSelect(fileInput, previewImg));
  });

  // Универсальная функция обработки выбора файла (для шагов и основного фото)
  function handleFileSelect(input, previewImg) {
    const file = input.files[0];
    if (!file) return;

    window.ImageEditor.open(file, function(dataUrl, blob) {
      if (dataUrl && blob) {
        // Пользователь нажал "Применить"
        previewImg.src = dataUrl;
        previewImg.style.display = "block";

        // Заменяем файл в input на отредактированный
        const newFile = new File([blob], file.name, { type: blob.type });
        const dt = new DataTransfer();
        dt.items.add(newFile);
        input.files = dt.files;
      } else {
        // Пользователь отменил — очищаем всё
        input.value = "";
        previewImg.src = "";
        previewImg.style.display = "none";
      }
    });
  }

  // === Основное изображение блюда ===
  const imageInput = document.getElementById("recipeImage");
  if (imageInput) {
    const imagePreview = document.createElement("img");
    imagePreview.classList.add("preview");
    imagePreview.alt = "Предпросмотр блюда";
    imagePreview.style.display = "none";
    imageInput.insertAdjacentElement("afterend", imagePreview);

    imageInput.addEventListener("change", () => handleFileSelect(imageInput, imagePreview));
  }

  // Отправка формы — просто заглушка
  const recipeForm = document.getElementById("recipeForm");
  recipeForm.addEventListener("submit", async e => {
    if (e.defaultPrevented) return;
    e.preventDefault();

    const difficultyMap = { easy: 1, medium: 2, hard: 3 };
    const titleInput = recipeForm.querySelector('input[name="title"]');
    const descriptionInput = recipeForm.querySelector('textarea[name="description"]');
    const authorInput = recipeForm.querySelector('input[name="author"]');
    const difficultyInput = recipeForm.querySelector('select[name="difficulty"]');
    const mainImageInput = recipeForm.querySelector('#recipeImage');
    const cuisineSelectEl = recipeForm.querySelector('#cuisineSelect');

    const ingredientNameInputs = Array.from(recipeForm.querySelectorAll('input[name="ingredientNames[]"]'));
    const ingredientAmountInputs = Array.from(recipeForm.querySelectorAll('input[name="ingredientAmounts[]"]'));

    const ingredients = ingredientNameInputs
      .map((nameInput, idx) => {
        const name = nameInput.value.trim();
        const amount = ingredientAmountInputs[idx]?.value.trim() || '';
        if (!name || !amount) return '';
        return `${name} — ${amount}`;
      })
      .filter(Boolean);

    const steps = Array.from(recipeForm.querySelectorAll('textarea[name="steps[]"]'))
      .map(s => s.value.trim())
      .filter(Boolean);

    const stepImageInputs = Array.from(recipeForm.querySelectorAll('.step input[type="file"]'));

    const formData = new FormData();
    formData.append('Title', titleInput?.value?.trim() || '');
    formData.append('Description', descriptionInput?.value?.trim() || '');
    formData.append('Author', authorInput?.value?.trim() || '');
    const selectedCuisineId = cuisineSelectEl?.value;
    if (selectedCuisineId && selectedCuisineId !== '__new__' && selectedCuisineId !== '') {
      formData.append('CuisineId', selectedCuisineId);
    }
    formData.append('Difficulty', String(difficultyMap[difficultyInput?.value] || 0));
    formData.append('CookingTime', '30');

    ingredients.forEach(i => formData.append('Ingredients', i));
    steps.forEach(s => formData.append('Steps', s));

    if (mainImageInput?.files?.[0]) {
      formData.append('MainImage', mainImageInput.files[0]);
    }

    stepImageInputs.forEach(input => {
      if (input.files?.[0]) {
        formData.append('StepImages', input.files[0]);
      }
    });

    const submitBtn = recipeForm.querySelector('input[type="submit"]');
    const originalText = submitBtn?.value || 'Сохранить';
    if (submitBtn) {
      submitBtn.disabled = true;
      submitBtn.value = 'Сохраняем...';
    }

    try {
      const response = await fetch('/api/recipe', {
        method: 'POST',
        body: formData
      });

      const result = await response.json();
      if (!response.ok || !result.success) {
        throw new Error(result.message || 'Не удалось сохранить рецепт');
      }

      alert('Рецепт успешно сохранен');
      if (result.data?.slug) {
        window.location.href = `/recipe/${encodeURIComponent(result.data.slug)}`;
        return;
      }

      window.location.href = '/catalog';
    } catch (error) {
      alert(error.message || 'Ошибка при сохранении рецепта');
    } finally {
      if (submitBtn) {
        submitBtn.disabled = false;
        submitBtn.value = originalText;
      }
    }
  });

  // --- Drag & Drop для шагов ---
  function makeStepDraggable(stepEl) {
    stepEl.addEventListener('dragstart', (ev) => {
      stepEl.classList.add('dragging');
      ev.dataTransfer.effectAllowed = 'move';
    });

    stepEl.addEventListener('dragend', () => {
      stepEl.classList.remove('dragging');
      document.querySelectorAll('.step.drag-over').forEach(el => el.classList.remove('drag-over'));
      updateStepNumbers();
    });

    stepEl.addEventListener('dragover', (ev) => {
      ev.preventDefault();
      ev.dataTransfer.dropEffect = 'move';
      ev.currentTarget.classList.add('drag-over');
    });

    stepEl.addEventListener('dragleave', (ev) => {
      ev.currentTarget.classList.remove('drag-over');
    });

    stepEl.addEventListener('drop', (ev) => {
      ev.preventDefault();
      const dragging = document.querySelector('.step.dragging');
      const droppedOn = ev.currentTarget;
      if (!dragging || dragging === droppedOn) return;

      const rect = droppedOn.getBoundingClientRect();
      const offset = ev.clientY - rect.top;
      const insertBefore = offset < rect.height / 2;

      if (insertBefore) {
        droppedOn.parentElement.insertBefore(dragging, droppedOn);
      } else {
        droppedOn.parentElement.insertBefore(dragging, droppedOn.nextSibling);
      }

      droppedOn.classList.remove('drag-over');
      updateStepNumbers();
    });
  }

  function updateStepNumbers() {
    const steps = Array.from(stepsDiv.querySelectorAll('.step'));
    steps.forEach((s, idx) => {
      const h3 = s.querySelector('h3');
      if (h3) h3.textContent = `Шаг ${idx + 1}`;
    });
  }

  // Инициализация существующих шагов (если редактирование)
  Array.from(stepsDiv.querySelectorAll('.step')).forEach(el => {
    if (!el.querySelector('.drag-handle')) {
      const handle = document.createElement('span');
      handle.className = 'drag-handle';
      handle.textContent = 'Drag';
      handle.title = 'Перетащите, чтобы изменить порядок';
      el.insertBefore(handle, el.firstChild);
    }
    makeStepDraggable(el);

    const fileInput = el.querySelector('input[type="file"]');
    const previewImg = el.querySelector('img.preview');
    if (fileInput && previewImg) {
      fileInput.addEventListener("change", () => handleFileSelect(fileInput, previewImg));
    }
  });
});
