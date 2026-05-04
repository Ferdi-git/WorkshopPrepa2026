using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private void Awake()
    {
        Instance = this;

        soundObjects = FindObjectsByType<SoundObject>(FindObjectsSortMode.None);
    }

    SoundObject[] soundObjects;

    public void PlaySound(string soundName)
    {
        for (int i = 0; i < soundObjects.Length; i++)
        {
            if(soundObjects[i].soundName == soundName)
            {
                soundObjects[i].Play();
            }
        }
    }
}
