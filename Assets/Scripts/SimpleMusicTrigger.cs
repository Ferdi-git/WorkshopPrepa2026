using System.Collections;
using UnityEngine;

public class SimpleMusicTrigger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The AudioSource somewhere in your scene that plays the music.")]
    [SerializeField] private AudioSource audioSource;
    
    [Header("Music Settings")]
    [SerializeField] private AudioClip newMusicTrack;
    [SerializeField] private float fadeDuration = 1.5f;
    
    [Header("Target")]
    [SerializeField] private string targetTag = "Player";

    private Coroutine _fadeCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger if the Player walks in
        if (other.CompareTag(targetTag))
        {
            // If this track is already playing, don't restart it
            if (audioSource.clip == newMusicTrack && audioSource.isPlaying) return;

            // Stop any fade that is currently running so they don't fight
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

            // Start the transition
            _fadeCoroutine = StartCoroutine(TransitionMusicRoutine());
        }
    }

    private IEnumerator TransitionMusicRoutine()
    {
        // Make sure loop is enabled
        audioSource.loop = true;

        // STEP 1: Fade out the current music (if something is playing)
        if (audioSource.isPlaying && audioSource.volume > 0f)
        {
            float startVolume = audioSource.volume;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
                yield return null;
            }
        }

        // STEP 2: Swap the track and play it at 0 volume
        audioSource.clip = newMusicTrack;
        audioSource.volume = 0f;
        audioSource.Play();

        // STEP 3: Fade in the new music
        float fadeInTime = 0f;
        while (fadeInTime < fadeDuration)
        {
            fadeInTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, fadeInTime / fadeDuration);
            yield return null;
        }

        // Ensure volume lands perfectly at max
        audioSource.volume = 1f;
    }
}