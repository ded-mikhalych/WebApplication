document.addEventListener('DOMContentLoaded', () => {
  function parseNumberToken(token) {
    if (token.includes('/')) {
      const [a, b] = token.split('/').map(s => parseFloat(s.replace(',', '.')));
      if (!isNaN(a) && !isNaN(b) && b !== 0) return a / b;
      return null;
    }

    const value = parseFloat(token.replace(',', '.'));
    return isNaN(value) ? null : value;
  }

  function formatDecimal(value, useComma) {
    if (Math.abs(value - Math.round(value)) < 1e-6) return String(Math.round(value));

    let formatted = value.toFixed(2);
    formatted = formatted.replace(/\.00$/, '');
    formatted = formatted.replace(/0$/, '');
    if (formatted.endsWith('.')) formatted = formatted.slice(0, -1);
    if (useComma) formatted = formatted.replace('.', ',');
    return formatted;
  }

  function formatNumberLike(origToken, value) {
    if (origToken.includes('/')) {
      const maxDenominator = 8;
      let best = null;
      let bestError = Infinity;

      for (let denominator = 1; denominator <= maxDenominator; denominator++) {
        const numerator = Math.round(value * denominator);
        const error = Math.abs(value - numerator / denominator);
        if (error < bestError) {
          bestError = error;
          best = { numerator, denominator, error };
        }
      }

      if (best && best.error < 1e-2 && best.denominator > 0) {
        if (best.numerator % best.denominator === 0) return String(best.numerator / best.denominator);
        return `${best.numerator}/${best.denominator}`;
      }

      return formatDecimal(value, origToken.includes(','));
    }

    return formatDecimal(value, origToken.includes(','));
  }

  function scaleText(text, factor) {
    return text.replace(/(\d+[\/,\.]?\d*)/g, match => {
      const value = parseNumberToken(match);
      if (value === null) return match;
      return formatNumberLike(match, value * factor);
    });
  }

  const ingredientsBlocks = document.querySelectorAll('.ingredients');
  if (!ingredientsBlocks.length) return;

  ingredientsBlocks.forEach(block => {
    const list = block.querySelector('ul');
    if (!list) return;

    const control = document.createElement('div');
    control.className = 'servings-control';
    control.innerHTML = '<label>Порции: <button class="servings-decrease" type="button">−</button> <input class="servings-input" type="number" value="1" min="1" step="1"> <button class="servings-increase" type="button">+</button></label>';
    block.insertBefore(control, list);

    const input = control.querySelector('.servings-input');
    const decrease = control.querySelector('.servings-decrease');
    const increase = control.querySelector('.servings-increase');
    const items = Array.from(list.querySelectorAll('li'));

    items.forEach(item => {
      item.dataset.orig = item.innerHTML;
    });

    let current = 1;

    function apply() {
      const factor = current;
      items.forEach(item => {
        item.innerHTML = scaleText(item.dataset.orig || item.innerHTML, factor);
      });
    }

    decrease.addEventListener('click', () => {
      if (current > 1) {
        current--;
        input.value = current;
        apply();
      }
    });

    increase.addEventListener('click', () => {
      current++;
      input.value = current;
      apply();
    });

    input.addEventListener('change', () => {
      const value = parseInt(input.value, 10) || 1;
      current = Math.max(1, value);
      input.value = current;
      apply();
    });
  });
});