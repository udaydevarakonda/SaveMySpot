window.downloadFile = async (url, fileName) => {
    try {
        const response = await fetch(url);
        if (!response.ok) {
            const text = await response.text();
            throw new Error(text || `Download failed with status ${response.status}`);
        }
        const blob = await response.blob();
        const objectUrl = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = objectUrl;
        link.download = fileName || '';
        document.body.appendChild(link);
        link.click();
        link.remove();
        URL.revokeObjectURL(objectUrl);
    } catch (err) {
        console.error('downloadFile error', err);
        alert('Failed to download file: ' + err.message);
    }
};
