// Mandatory Scroll-Reactive Floating Pill Header Engine (Rule 8)
(function initFloatingHeader() {
  const bar = document.getElementById('mainHeader');
  const shell = document.getElementById('headerShell');
  if (!bar || !shell) return;

  let isCompact = false;
  let ticking = false;
  const enterThreshold = 75;
  const exitThreshold = 20;
  const topDeadZone = 16;

  function evaluateScroll() {
    const y = window.scrollY;
    const next = y <= topDeadZone ? false : isCompact ? y > exitThreshold : y > enterThreshold;

    if (next !== isCompact) {
      isCompact = next;
      if (isCompact) {
        bar.classList.add('is-compact');
        shell.classList.add('is-compact-shell');
      } else {
        bar.classList.remove('is-compact');
        shell.classList.remove('is-compact-shell');
      }
    }
    ticking = false;
  }

  window.addEventListener('scroll', function () {
    if (!ticking) {
      ticking = true;
      requestAnimationFrame(evaluateScroll);
    }
  }, { passive: true });

  evaluateScroll();
})();