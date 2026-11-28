
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;
    private AudioSource audioSource;

    private void Awake()
    {
        // Mencegah musik dibuat lebih dari satu kali
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Mempertahankan objek saat berpindah scene
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        // Memulai musik
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
