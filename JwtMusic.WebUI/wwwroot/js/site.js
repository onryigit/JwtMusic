let currentAudioUrl;
async function playSong(id,title,artist,cover){
  try{
    const response=await fetch(`/Songs/Play?id=${id}`);
    if(response.status===403){$('#packageModal').modal('show');return;}
    if(response.status===401){window.location='/Login/SignIn';return;}
    if(!response.ok)throw new Error('Müzik dosyası yüklenemedi.');
    const blob=await response.blob();
    if(currentAudioUrl)URL.revokeObjectURL(currentAudioUrl);
    currentAudioUrl=URL.createObjectURL(blob);
    const player=document.getElementById('audio-player');
    player.src=currentAudioUrl;
    document.getElementById('player-title').textContent=title;
    document.getElementById('player-artist').textContent=artist;
    document.getElementById('player-cover').src=cover;
    document.getElementById('music-player').classList.remove('d-none');
    await player.play();
  }catch(error){alert(error.message);}
}
