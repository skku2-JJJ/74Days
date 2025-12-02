using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private static AudioSource audioSource;
    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }   
    }


    public void PlaySound(AudioClip audioClip)
    {
        if (audioSource == null)
        {
            Debug.Log("오디오소스없음");
            return;
        }
        audioSource.PlayOneShot(audioClip);
        
    }
    public void PlaySoundOnly(AudioClip audioClip)
    {
        if (audioSource == null)
        {
            Debug.Log("오디오소스없음");
            return;
        }
        audioSource.clip = audioClip;
        audioSource.Play();

    }

    public void PlayStop()
    {
        audioSource.Stop();
        audioSource.clip = null;    
    }
}
