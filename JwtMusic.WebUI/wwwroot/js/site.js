let currentAudioUrl = null;
let currentTrack = null;

const audioPlayer = document.getElementById('audio-player');
const playerBar = document.getElementById('music-player');
const progressInput = document.getElementById('player-progress');
const volumeInput = document.getElementById('player-volume');

document.addEventListener('click', event => {
    const play = event.target.closest('.js-play');
    if (play) {
        event.preventDefault();
        playSong({ id: play.dataset.id, title: play.dataset.title, artist: play.dataset.artist, cover: play.dataset.cover });
    }
    if (event.target.closest('[data-close-modal]')) closePackageModal();
    if (event.target === document.getElementById('packageModal')) closePackageModal();
});

document.addEventListener('keydown', event => {
    if (event.key === 'Escape') closePackageModal();
});

document.querySelector('.js-logout-form')?.addEventListener('submit', () => {
    localStorage.removeItem('reverb.jwt');
});

async function playSong(track) {
    if (!audioPlayer) return;
    if (currentTrack?.id === track.id && audioPlayer.src) {
        audioPlayer.paused ? await audioPlayer.play() : audioPlayer.pause();
        return;
    }

    const button = document.querySelector(`.js-play[data-id="${track.id}"]`);
    if (button) button.classList.add('is-loading');
    try {
        const response = await fetch(`/Songs/Play?id=${encodeURIComponent(track.id)}`);
        if (response.status === 403) {
            let message = 'Mevcut paketiniz bu şarkıyı desteklememektedir. Lütfen paketinizi yükseltin.';
            try { message = (JSON.parse(await response.text())).message || message; } catch (_) { }
            openPackageModal(message);
            return;
        }
        if (response.status === 401) { window.location = '/Login/SignIn'; return; }
        if (!response.ok) throw new Error('Müzik önizlemesi yüklenemedi. İnternet bağlantınızı kontrol edin.');

        const contentType = response.headers.get('content-type') || '';
        let source;
        if (contentType.includes('application/json')) {
            source = (await response.json()).streamUrl;
            releaseObjectUrl();
        } else {
            const blob = await response.blob();
            releaseObjectUrl();
            currentAudioUrl = URL.createObjectURL(blob);
            source = currentAudioUrl;
        }

        currentTrack = track;
        audioPlayer.src = source;
        audioPlayer.load();
        document.getElementById('player-title').textContent = track.title;
        document.getElementById('player-artist').textContent = track.artist;
        document.getElementById('player-cover').src = track.cover;
        playerBar.classList.remove('d-none');
        await audioPlayer.play();
    } catch (error) { window.alert(error.message); }
    finally { if (button) button.classList.remove('is-loading'); }
}

function releaseObjectUrl() {
    if (!currentAudioUrl) return;
    URL.revokeObjectURL(currentAudioUrl);
    currentAudioUrl = null;
}

function getPageQueue() {
    const seen = new Set();
    return Array.from(document.querySelectorAll('.js-play')).map(button => ({
        id: button.dataset.id,
        title: button.dataset.title,
        artist: button.dataset.artist,
        cover: button.dataset.cover
    })).filter(track => track.id && !seen.has(track.id) && seen.add(track.id));
}

function changeTrack(offset) {
    const queue = getPageQueue();
    if (!queue.length) return;
    const foundIndex = queue.findIndex(track => track.id === currentTrack?.id);
    const currentIndex = foundIndex < 0 ? 0 : foundIndex;
    playSong(queue[(currentIndex + offset + queue.length) % queue.length]);
}

function formatTime(value) {
    if (!Number.isFinite(value) || value < 0) return '0:00';
    return `${Math.floor(value / 60)}:${Math.floor(value % 60).toString().padStart(2, '0')}`;
}

function paintRange(input, value, max) {
    if (!input) return;
    const percentage = max > 0 ? Math.min(100, Math.max(0, value / max * 100)) : 0;
    input.style.setProperty('--range-progress', `${percentage}%`);
}

function setButtonIcon(button, icon, label) {
    if (!button) return;
    button.innerHTML = `<i data-feather="${icon}"></i>`;
    button.setAttribute('aria-label', label);
    if (window.feather) feather.replace();
}

function syncPlayButton() {
    setButtonIcon(document.getElementById('player-toggle'), audioPlayer?.paused ? 'play' : 'pause', audioPlayer?.paused ? 'Oynat' : 'Duraklat');
}

function syncVolumeButton() {
    if (!audioPlayer) return;
    const icon = audioPlayer.muted || audioPlayer.volume === 0 ? 'volume-x' : audioPlayer.volume < .5 ? 'volume-1' : 'volume-2';
    setButtonIcon(document.getElementById('player-mute'), icon, audioPlayer.muted ? 'Sesi aç' : 'Sesi kapat');
}

document.getElementById('player-toggle')?.addEventListener('click', async () => {
    if (!audioPlayer?.src) return;
    audioPlayer.paused ? await audioPlayer.play() : audioPlayer.pause();
});
document.getElementById('player-previous')?.addEventListener('click', () => changeTrack(-1));
document.getElementById('player-next')?.addEventListener('click', () => changeTrack(1));

progressInput?.addEventListener('input', () => {
    if (!audioPlayer || !Number.isFinite(audioPlayer.duration)) return;
    audioPlayer.currentTime = Number(progressInput.value);
    paintRange(progressInput, audioPlayer.currentTime, audioPlayer.duration);
});

volumeInput?.addEventListener('input', () => {
    if (!audioPlayer) return;
    audioPlayer.volume = Number(volumeInput.value);
    audioPlayer.muted = false;
    paintRange(volumeInput, audioPlayer.volume, 1);
});

document.getElementById('player-mute')?.addEventListener('click', () => {
    if (!audioPlayer) return;
    audioPlayer.muted = !audioPlayer.muted;
});

audioPlayer?.addEventListener('loadedmetadata', () => {
    progressInput.max = Number.isFinite(audioPlayer.duration) ? audioPlayer.duration : 0;
    document.getElementById('player-duration').textContent = formatTime(audioPlayer.duration);
    paintRange(progressInput, 0, audioPlayer.duration);
});
audioPlayer?.addEventListener('timeupdate', () => {
    progressInput.value = audioPlayer.currentTime;
    document.getElementById('player-elapsed').textContent = formatTime(audioPlayer.currentTime);
    paintRange(progressInput, audioPlayer.currentTime, audioPlayer.duration);
});
audioPlayer?.addEventListener('play', syncPlayButton);
audioPlayer?.addEventListener('pause', syncPlayButton);
audioPlayer?.addEventListener('volumechange', syncVolumeButton);
audioPlayer?.addEventListener('ended', () => changeTrack(1));

if (audioPlayer) {
    audioPlayer.volume = Number(volumeInput?.value ?? .8);
    paintRange(volumeInput, audioPlayer.volume, 1);
}

function openPackageModal(message) {
    const modal = document.getElementById('packageModal');
    if (!modal) return;
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
