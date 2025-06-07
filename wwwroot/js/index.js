const videos = [
    document.getElementById('video1'),
    document.getElementById('video2'),
    document.getElementById('video3')
];
let currentVideoIndex = 0;
let timeoutId = null;
const defaultDuration = 10;

function playNextVideo() {
    try {
        if (timeoutId) {
            clearTimeout(timeoutId);
            timeoutId = null;
        }

        console.log(`Switching from video ${currentVideoIndex + 1} to video ${(currentVideoIndex + 1) % videos.length + 1}.`);

        videos[currentVideoIndex].classList.add('hidden');
        videos[currentVideoIndex].pause();
        videos[currentVideoIndex].currentTime = 0;

        currentVideoIndex = (currentVideoIndex + 1) % videos.length;

        videos[currentVideoIndex].classList.remove('hidden');
        const playPromise = videos[currentVideoIndex].play();

        if (playPromise !== undefined) {
            playPromise
                .then(() => {
                    console.log(`Video ${currentVideoIndex + 1} started playing.`);
                    const duration = videos[currentVideoIndex].duration;
                    if (duration && !isNaN(duration)) {
                        timeoutId = setTimeout(() => {
                            console.log(`Timeout triggered for video ${currentVideoIndex + 1}.`);
                            playNextVideo();
                        }, duration * 1000 + 500);
                    } else {
                        console.warn(`Duration not available for video ${currentVideoIndex + 1}, using default ${defaultDuration}s.`);
                        timeoutId = setTimeout(() => {
                            console.log(`Default timeout triggered for video ${currentVideoIndex + 1}.`);
                            playNextVideo();
                        }, defaultDuration * 1000);
                    }
                })
                .catch(error => {
                    console.error(`Error playing video ${currentVideoIndex + 1}:`, error);
                    playNextVideo();
                });
        }
    } catch (error) {
        console.error('Error in playNextVideo:', error);
        playNextVideo();
    }
}

videos.forEach((video, index) => {
    video.addEventListener('ended', () => {
        console.log(`Ended event triggered for video ${index + 1}.`);
        playNextVideo();
    });

    video.addEventListener('error', (e) => {
        console.error(`Error loading video ${index + 1}:`, e.target.error.message, `(Code: ${e.target.error.code})`);
        playNextVideo();
    });

    video.addEventListener('canplay', () => {
        console.log(`Video ${index + 1} is ready to play.`);
    });

    video.addEventListener('loadedmetadata', () => {
        console.log(`Metadata loaded for video ${index + 1}. Duration: ${video.duration}s`);
        console.log(`Video ${index + 1} source URL: ${video.currentSrc}`);
    });
});

try {
    console.log('Starting playback of video 1.');
    console.log('Video 1 source URL:', videos[0].currentSrc);
    const playPromise = videos[currentVideoIndex].play();
    if (playPromise !== undefined) {
        playPromise
            .then(() => {
                console.log('Video 1 started playing successfully.');
                const duration = videos[currentVideoIndex].duration;
                if (duration && !isNaN(duration)) {
                    timeoutId = setTimeout(() => {
                        console.log('Timeout triggered for video 1.');
                        playNextVideo();
                    }, duration * 1000 + 500);
                } else {
                    console.warn(`Duration not available for video 1, using default ${defaultDuration}s.`);
                    timeoutId = setTimeout(() => {
                        console.log('Default timeout triggered for video 1.');
                        playNextVideo();
                    }, defaultDuration * 1000);
                }
            })
            .catch(error => {
                console.error('Error starting first video:', error);
                playNextVideo();
            });
    }
} catch (error) {
    console.error('Error starting playback:', error);
    playNextVideo();
}