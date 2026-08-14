let currentAudioUrl;

document.addEventListener('click', event => {
    const play = event.target.closest('.js-play');
    if (play) playSong(play.dataset.id, play.dataset.title, play.dataset.artist, play.dataset.cover);
    if (event.target.closest('[data-close-modal]')) closePackageModal();
});

async function playSong(id, title, artist, cover) {
    const button = document.querySelector(`.js-play[data-id="${id}"]`);
    if (button) button.classList.add('is-loading');
    try {
        const response = await fetch(`/Songs/Play?id=${encodeURIComponent(id)}`);
        if (response.status === 403) {
            let message = 'Mevcut paketiniz bu şarkıyı desteklememektedir. Lütfen paketinizi yükseltin.';
            try { message = (JSON.parse(await response.text())).message || message; } catch (_) { }
            openPackageModal(message);
            return;
        }
        if (response.status === 401) { window.location = '/Login/SignIn'; return; }
        if (!response.ok) throw new Error('Müzik dosyası yüklenemedi.');
        const blob = await response.blob();
        if (currentAudioUrl) URL.revokeObjectURL(currentAudioUrl);
        currentAudioUrl = URL.createObjectURL(blob);
        const player = document.getElementById('audio-player');
        player.src = currentAudioUrl;
        document.getElementById('player-title').textContent = title;
        document.getElementById('player-artist').textContent = artist;
        document.getElementById('player-cover').src = cover;
        document.getElementById('music-player').classList.remove('d-none');
        await player.play();
    } catch (error) { window.alert(error.message); }
    finally { if (button) button.classList.remove('is-loading'); }
}

function openPackageModal(message) {
    const modal = document.getElementById('packageModal');
    document.getElementById('packageMessage').textContent = message;
    modal.classList.add('show');
    modal.setAttribute('aria-hidden', 'false');
}

function closePackageModal() {
    const modal = document.getElementById('packageModal');
    if (!modal) return;
    modal.classList.remove('show');
    modal.setAttribute('aria-hidden', 'true');
}
