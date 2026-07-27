/* TANTAR — Playlist actions (add/remove songs) */
(function () {
    'use strict';

    let cachedPlaylists = null;
    let pendingSongId = null;
    let addModal = null;

    function getCsrfToken() {
        const el = document.querySelector('input[name="__RequestVerificationToken"]');
        return el ? el.value : '';
    }

    function showToast(message, isError = false) {
        const existing = document.querySelector('.tantar-toast');
        if (existing) existing.remove();

        const toast = document.createElement('div');
        toast.className = 'tantar-toast' + (isError ? ' tantar-toast-error' : '');
        toast.innerHTML = `<i class="bi bi-${isError ? 'exclamation-circle' : 'check-circle'} me-2"></i>${message}`;
        document.body.appendChild(toast);

        requestAnimationFrame(() => toast.classList.add('show'));
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => toast.remove(), 350);
        }, 2500);
    }

    async function fetchMyPlaylists() {
        if (cachedPlaylists) return cachedPlaylists;
        try {
            const res = await fetch('/Playlist/GetMyPlaylists');
            if (!res.ok) return [];
            cachedPlaylists = await res.json();
            return cachedPlaylists;
        } catch {
            return [];
        }
    }

    function renderModalBody(playlists, songId) {
        const body = document.getElementById('playlist-modal-body');
        if (!body) return;

        if (!playlists.length) {
            body.innerHTML = '<p class="text-secondary small text-center py-3 mb-0">You have no playlists yet.</p>';
            return;
        }

        body.innerHTML = playlists.map(p => `
            <button class="dropdown-item py-2 px-3 d-flex align-items-center gap-2 playlist-pick"
                    data-playlist-id="${p.id}" data-playlist-name="${escapeHtml(p.name)}">
                <i class="bi bi-collection-fill text-secondary small"></i>
                <span class="text-truncate">${escapeHtml(p.name)}</span>
            </button>
        `).join('');

        body.querySelectorAll('.playlist-pick').forEach(btn => {
            btn.addEventListener('click', () => addSong(songId, btn.dataset.playlistId, btn.dataset.playlistName));
        });
    }

    async function openAddToPlaylist(songId) {
        const modalEl = document.getElementById('addToPlaylistModal');
        if (!modalEl) return;

        pendingSongId = songId;

        if (!addModal) addModal = new bootstrap.Modal(modalEl);

        const body = document.getElementById('playlist-modal-body');
        if (body) body.innerHTML = '<p class="text-secondary small text-center py-3 mb-0">Loading…</p>';

        addModal.show();

        const playlists = await fetchMyPlaylists();
        renderModalBody(playlists, songId);
    }

    async function addSong(songId, playlistId, playlistName) {
        if (addModal) addModal.hide();

        const res = await fetch(`/Playlist/AddSong?playlistId=${playlistId}&songId=${songId}`, {
            method: 'POST',
            headers: { 'RequestVerificationToken': getCsrfToken() }
        });

        if (res.ok) {
            showToast(`Added to <strong>${playlistName}</strong>`);
        } else if (res.status === 403) {
            showToast('You cannot modify that playlist.', true);
        } else {
            showToast('Failed to add song.', true);
        }
    }

    async function removeSong(playlistId, songId, rowEl) {
        const res = await fetch(`/Playlist/RemoveSong?playlistId=${playlistId}&songId=${songId}`, {
            method: 'POST',
            headers: { 'RequestVerificationToken': getCsrfToken() }
        });

        if (res.ok) {
            rowEl?.remove();
            showToast('Removed from playlist.');
        } else if (res.status === 403) {
            showToast('You cannot modify that playlist.', true);
        } else {
            showToast('Failed to remove song.', true);
        }
    }

    function escapeHtml(str) {
        return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
    }

    // Wire up "Add to playlist" buttons via delegation
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-add-to-playlist]');
        if (btn) {
            e.preventDefault();
            e.stopPropagation();
            openAddToPlaylist(btn.dataset.addToPlaylist);
        }

        const removeBtn = e.target.closest('[data-remove-from-playlist]');
        if (removeBtn) {
            e.preventDefault();
            const row = removeBtn.closest('.song-row');
            removeSong(removeBtn.dataset.playlistId, removeBtn.dataset.removeFromPlaylist, row);
        }
    });

    // Expose for inline onclick usage
    window.TantarPlaylist = { openAddToPlaylist, removeSong };
})();
