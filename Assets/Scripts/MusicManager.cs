using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] clips;
    [SerializeField] string[] names;

    private void Start()
    {
        if(names.Length != clips.Length)
        {
            Debug.LogError("Please assign the same number of clips as there are names");
        }

        PlayMusic(names[0]);
    }

    public void PlayMusic(string musicName)
    {
        source.clip = clips[0];

        for (int i = 0; i < clips.Length; i++)
        {
            if (names[i] == musicName)
            {
                source.clip = clips[i];
            }
        }

        source.Stop();
        source.Play();

    }
}
