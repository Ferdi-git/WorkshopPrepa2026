using UnityEngine;

public class SoundObject : MonoBehaviour
{
    public string soundName;

    AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void Play()
    {
        source.Play();
    }
}
