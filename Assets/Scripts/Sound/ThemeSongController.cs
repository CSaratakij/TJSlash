using UnityEngine;

namespace TJ
{
    public class ThemeSongController : MonoBehaviour
    {
        [SerializeField]
        AudioClip themeSong;

        AudioSource audioSource;


        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            SubscribeEvent();
        }

        void OnDestroy()
        {
            UnSubscribeEvent();
        }

        void SubscribeEvent()
        {
            GameController.Instance.OnGameStart += OnGameStart;
            GameController.Instance.OnGameStop += OnGameStop;
        }

        void UnSubscribeEvent()
        {
            GameController.Instance.OnGameStart -= OnGameStart;
            GameController.Instance.OnGameStop -= OnGameStop;
        }

        void OnGameStart()
        {
            audioSource.clip = themeSong;
            audioSource.PlayDelayed(0.5f);
        }

        void OnGameStop()
        {
            audioSource.Stop();
        }
    }
}
