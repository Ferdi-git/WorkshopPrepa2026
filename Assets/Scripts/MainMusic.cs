using UnityEngine;

public class MainMusic : MonoBehaviour
{

    private AudioSource audioSource;
    [SerializeField] private float fadeDuration = 1.5f;

    private Coroutine _fadeCoroutine;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
