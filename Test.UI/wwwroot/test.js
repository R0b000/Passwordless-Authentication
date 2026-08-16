/* ------------------------------------------------------------------
   downloadHtmlAsPdf
   Renders a full HTML page string (like the Novapulse landing page)
   into a hidden iframe so Tailwind / Google fonts / page JS all load,
   then exports it as an A4 PDF and triggers the browser download.

   Usage (called from Blazor via IJSRuntime):
       await JS.InvokeVoidAsync("downloadHtmlAsPdf", htmlString, "file.pdf");
------------------------------------------------------------------ */

window.downloadHtmlAsPdf = async function (htmlString, fileName) {
  const name = fileName || 'document.pdf';

  // 1. Make sure html2pdf.js is on the page.
  if (typeof window.html2pdf === 'undefined') {
    await loadScript('https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.2/html2pdf.bundle.min.js');
  }

  // 2. Render the HTML inside a hidden, normal-width iframe.
  const iframe = document.createElement('iframe');
  iframe.style.position = 'fixed';
  iframe.style.top = '-99999px';
  iframe.style.left = '-99999px';
  iframe.style.width = '794px';     // A4 width @ 96dpi
  iframe.style.height = '1123px';   // A4 height @ 96dpi
  iframe.style.border = '0';
  iframe.style.background = '#FFFFFF';
  document.body.appendChild(iframe);

  const iDoc = iframe.contentDocument;
  iDoc.open();
  iDoc.write(htmlString);
  iDoc.close();

  // 3. Wait for the page (and its external resources) to load.
  await new Promise((resolve) => {
    if (iDoc.readyState === 'complete') resolve();
    else iframe.addEventListener('load', resolve);
  });
  await new Promise((resolve) => setTimeout(resolve, 700)); // let web fonts settle

  try {
    // 4. Render body -> canvas -> A4 PDF and download.
    await html2pdf()
      .set({
        margin: 0,
        filename: name,
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: {
          scale: 2,
          useCORS: true,
          logging: false,
          backgroundColor: '#FFFFFF',
          windowWidth: iframe.contentWindow.innerWidth
        },
        jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' },
        pagebreak: { mode: ['css', 'legacy'] }
      })
      .from(iframe.contentDocument.body)
      .save();
  } finally {
    // Cleanup the temp iframe regardless of success/failure.
    document.body.removeChild(iframe);
  }
};

// Helper: dynamically load a <script> from a URL.
function loadScript(src) {
  return new Promise((resolve, reject) => {
    const s = document.createElement('script');
    s.src = src;
    s.onload = resolve;
    s.onerror = () => reject(new Error('Failed to load ' + src));
    document.head.appendChild(s);
  });
}
/* ------------------------------------------------------------------
   downloadHtmlAsPdf
   Renders a full HTML page string (like the Novapulse landing page)
   into a hidden iframe so Tailwind / Google fonts / page JS all load,
   then exports it as an A4 PDF and triggers the browser download.

   Usage (called from Blazor via IJSRuntime):
       await JS.InvokeVoidAsync("downloadHtmlAsPdf", htmlString, "file.pdf");
------------------------------------------------------------------ */

window.downloadHtmlAsPdf = async function (htmlString, fileName) {
  const name = fileName || 'document.pdf';

  // 1. Make sure html2pdf.js is on the page.
  if (typeof window.html2pdf === 'undefined') {
    await loadScript('https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.2/html2pdf.bundle.min.js');
  }

  // 2. Render the HTML inside a hidden, normal-width iframe.
  const iframe = document.createElement('iframe');
  iframe.style.position = 'fixed';
  iframe.style.top = '-99999px';
  iframe.style.left = '-99999px';
  iframe.style.width = '794px';     // A4 width @ 96dpi
  iframe.style.height = '1123px';   // A4 height @ 96dpi
  iframe.style.border = '0';
  iframe.style.background = '#FFFFFF';
  document.body.appendChild(iframe);

  const iDoc = iframe.contentDocument;
  iDoc.open();
  iDoc.write(htmlString);
  iDoc.close();

  // 3. Wait for the page (and its external resources) to load.
  await new Promise((resolve) => {
    if (iDoc.readyState === 'complete') resolve();
    else iframe.addEventListener('load', resolve);
  });
  await new Promise((resolve) => setTimeout(resolve, 700)); // let web fonts settle

  try {
    // 4. Render body -> canvas -> A4 PDF and download.
    await html2pdf()
      .set({
        margin: 0,
        filename: name,
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: {
          scale: 2,
          useCORS: true,
          logging: false,
          backgroundColor: '#FFFFFF',
          windowWidth: iframe.contentWindow.innerWidth
        },
        jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' },
        pagebreak: { mode: ['css', 'legacy'] }
      })
      .from(iframe.contentDocument.body)
      .save();
  } finally {
    // Cleanup the temp iframe regardless of success/failure.
    document.body.removeChild(iframe);
  }
};

// Helper: dynamically load a <script> from a URL.
function loadScript(src) {
  return new Promise((resolve, reject) => {
    const s = document.createElement('script');
    s.src = src;
    s.onload = resolve;
    s.onerror = () => reject(new Error('Failed to load ' + src));
    document.head.appendChild(s);
  });
}
