window.downloadHtmlAsPdf = async function (htmlString, fileName) {
    const name = fileName || 'invoice.pdf';

    // 1. Ensure html2pdf is present
    if (typeof window.html2pdf === 'undefined') {
        await new Promise((resolve, reject) => {
            const s = document.createElement('script');
            s.src = 'https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.2/html2pdf.bundle.min.js';
            s.onload = resolve;
            s.onerror = () => reject(new Error('Failed to load html2pdf.js'));
            document.head.appendChild(s);
        });
    }

    // 2. Create invisible iframe
    const iframe = document.createElement('iframe');
    iframe.style.position = 'fixed';
    iframe.style.top = '0';
    iframe.style.left = '0';
    iframe.style.width = '794px';
    iframe.style.height = '1123px';
    iframe.style.border = '0';
    iframe.style.zIndex = '-9999';
    iframe.style.opacity = '0';
    iframe.style.pointerEvents = 'none';
    document.body.appendChild(iframe);

    // 3. Write HTML
    const iDoc = iframe.contentDocument || iframe.contentWindow.document;
    iDoc.open();
    iDoc.write(htmlString);
    iDoc.close();

    // 4. Wait for styles to settle
    await new Promise((resolve) => setTimeout(resolve, 250));

    try {
        const targetElement = iDoc.querySelector('.page-container') || iDoc.body;

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
                    width: 794,
                    windowWidth: 794,
                    x: 0,
                    y: 0,
                    scrollX: 0,
                    scrollY: 0
                },
                jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
            })
            .from(targetElement)
            .save();
    } catch (err) {
        console.error("PDF generation failed:", err);
    } finally {
        document.body.removeChild(iframe);
    }
};