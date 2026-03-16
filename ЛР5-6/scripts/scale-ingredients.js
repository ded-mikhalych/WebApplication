document.addEventListener('DOMContentLoaded', () => {
  function parseNumberToken(token){
    if (token.includes('/')){
      const [a,b] = token.split('/').map(s=>parseFloat(s.replace(',','.')));
      if (!isNaN(a) && !isNaN(b) && b!==0) return a / b;
      return null;
    }
    const v = parseFloat(token.replace(',','.'));
    return isNaN(v) ? null : v;
  }

  function formatNumberLike(origToken, value){

    if (origToken.includes('/')){

      const maxDen = 8;
      let best = null; let bestErr = Infinity;
      for(let den=1; den<=maxDen; den++){
        const num = Math.round(value * den);
        const err = Math.abs(value - num/den);
        if (err < bestErr){ bestErr = err; best = {num,den,err}; }
      }
      if (best && best.err < 1e-2 && best.den>0){
        const num = best.num; const den = best.den;
        if (num % den === 0) return String(num/den);
        return `${num}/${den}`;
      }

      return formatDecimal(value, origToken.indexOf(',')>=0);
    }

    const useComma = origToken.indexOf(',')>=0;
    return formatDecimal(value, useComma);
  }

  function formatDecimal(value, useComma){

    if (Math.abs(value - Math.round(value)) < 1e-6) return String(Math.round(value));

    let s = value.toFixed(2);
    s = s.replace(/\.00$/,'');
    s = s.replace(/0$/,'');
    if (s.endsWith('.')) s = s.slice(0,-1);
    if (useComma) s = s.replace('.',',');
    return s;
  }

  function scaleText(text, factor){

    return text.replace(/(\d+[\/,\.]?\d*)/g, (match)=>{
      const v = parseNumberToken(match);
      if (v === null) return match;
      const nv = v * factor;
      return formatNumberLike(match, nv);
    });
  }

  const ingredientsBlocks = document.querySelectorAll('.ingredients');
  if (!ingredientsBlocks || ingredientsBlocks.length===0) return;

  ingredientsBlocks.forEach(block => {
    const ul = block.querySelector('ul');
    if (!ul) return;

    const ctrl = document.createElement('div');
    ctrl.className = 'servings-control';
    ctrl.innerHTML = `<label>Порции: <button class="servings-decrease" type="button">−</button> <input class="servings-input" type="number" value="1" min="1" step="1"> <button class="servings-increase" type="button">+</button></label>`;
    block.insertBefore(ctrl, ul);

    const input = ctrl.querySelector('.servings-input');
    const btnDec = ctrl.querySelector('.servings-decrease');
    const btnInc = ctrl.querySelector('.servings-increase');


    const items = Array.from(ul.querySelectorAll('li'));
    items.forEach(li => { li.dataset.orig = li.innerHTML; });

    const baseServings = 1;
    let current = 1;

    function apply(){
      const factor = current / baseServings;
      items.forEach(li => {
        const orig = li.dataset.orig || li.innerHTML;

        const scaled = scaleText(orig, factor);
        li.innerHTML = scaled;
      });
    }

    btnDec.addEventListener('click', ()=>{ if (current>1){ current--; input.value = current; apply(); } });
    btnInc.addEventListener('click', ()=>{ current++; input.value = current; apply(); });
    input.addEventListener('change', ()=>{ const v = parseInt(input.value,10) || 1; current = Math.max(1,v); input.value=current; apply(); });
  });
});
