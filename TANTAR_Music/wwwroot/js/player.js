/* TANTAR Music Player */
(function () {
    'use strict';

    const audio = document.getElementById('audio-engine');
    const player = document.getElementById('music-player');
    const btnPlayPause = document.getElementById('btn-play-pause');
    const playIcon = document.getElementById('play-icon');
    const btnPrev = document.getElementById('btn-prev');
    const btnNext = document.getElementById('btn-next');
    const seekBar = document.getElementById('player-seek');
    const volumeBar = document.getElementById('player-volume');
    const currentTime = document.getElementById('player-current');
    const durationEl = document.getElementById('player-duration');
    const titleEl = document.getElementById('player-title');
    const artistEl = document.getElementById('player-artist');
    const coverEl = document.getElementById('player-cover');

    let queue = [];
    let queueIndex = -1;

    function formatTime(s) {
        if (isNaN(s)) return '0:00';
        const m = Math.floor(s / 60);
        const sec = Math.floor(s % 60).toString().padStart(2, '0');
        return `${m}:${sec}`;
    }

    function loadSong(song) {
        audio.src = song.filePath;
        titleEl.textContent = song.title;
        artistEl.textContent = song.artist;
        coverEl.src = song.cover || '/images/default-cover.svg';
        player.style.display = 'block';

        // Notify server
        if (song.id) {
            fetch(`/Song/Play/${song.id}`, { method: 'POST', headers: { 'RequestVerificationToken': getAntiForgery() } });
        }
    }

    function play(song) {
        loadSong(song);
        audio.play().then(() => setPlayIcon(true)).catch(() => {});
    }

    function setPlayIcon(playing) {
        playIcon.className = playing ? 'bi bi-pause-fill' : 'bi bi-play-fill';
    }

    // Public API
    window.TantarPlayer = {
        playNow(song) {
            queue = [song];
            queueIndex = 0;
            play(song);
        },
        setQueue(songs, startIndex) {
            queue = songs;
            queueIndex = startIndex || 0;
            play(queue[queueIndex]);
        },
        isPlaying() { return !audio.paused; }
    };

    btnPlayPause.addEventListener('click', () => {
        if (audio.paused) {
            audio.play().then(() => setPlayIcon(true));
        } else {
            audio.pause();
            setPlayIcon(false);
        }
    });

    btnPrev.addEventListener('click', () => {
        if (audio.currentTime > 3) {
            audio.currentTime = 0;
        } else {
            queueIndex = (queueIndex - 1 + queue.length) % queue.length;
            play(queue[queueIndex]);
        }
    });

    btnNext.addEventListener('click', () => {
        queueIndex = (queueIndex + 1) % queue.length;
        play(queue[queueIndex]);
    });

    audio.addEventListener('ended', () => {
        queueIndex = (queueIndex + 1) % queue.length;
        play(queue[queueIndex]);
    });

    audio.addEventListener('timeupdate', () => {
        if (!audio.duration) return;
        seekBar.max = Math.floor(audio.duration);
        seekBar.value = Math.floor(audio.currentTime);
        currentTime.textContent = formatTime(audio.currentTime);
        durationEl.textContent = formatTime(audio.duration);
    });

    seekBar.addEventListener('input', () => { audio.currentTime = seekBar.value; });

    volumeBar.addEventListener('input', () => { audio.volume = volumeBar.value / 100; });
    audio.volume = 0.8;

    audio.addEventListener('play', () => setPlayIcon(true));
    audio.addEventListener('pause', () => setPlayIcon(false));

    function getAntiForgery() {
        const el = document.querySelector('input[name="__RequestVerificationToken"]');
        return el ? el.value : '';
    }

    // Wire up all [data-play] buttons
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-play]');
        if (!btn) return;
        e.preventDefault();

        const song = {
            id: btn.dataset.songId,
            title: btn.dataset.title,
            artist: btn.dataset.artist,
            cover: btn.dataset.cover,
            filePath: btn.dataset.play
        };

        // Build queue from the songs list if available
        const queueContainer = document.querySelector('[data-queue]');
        if (queueContainer) {
            const allBtns = Array.from(queueContainer.querySelectorAll('[data-play]'));
            const songs = allBtns.map(b => ({
                id: b.dataset.songId,
                title: b.dataset.title,
                artist: b.dataset.artist,
                cover: b.dataset.cover,
                filePath: b.dataset.play
            }));
            const idx = allBtns.indexOf(btn);
            window.TantarPlayer.setQueue(songs, idx >= 0 ? idx : 0);
            return;
        }

        window.TantarPlayer.playNow(song);
    });
})();
