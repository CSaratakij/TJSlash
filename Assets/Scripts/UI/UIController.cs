using UnityEngine;
using UnityEngine.SceneManagement;

namespace TJ
{
    public class UIController : MonoBehaviour
    {
        enum UI
        {
            MainMenu,
            InGameMenu,
            PauseMenu,
            GameOverMenu
        }

        [SerializeField]
        RectTransform[] panels;

        bool isPause;

        void Awake()
        {
            SubscribeEvent();
        }

        void Start()
        {
            Show(UI.MainMenu);
        }

        void Update()
        {
            if (!GameController.IsGameStart)
                return;

            if (Input.GetButtonDown("Cancel")) {
                isPause = !isPause;
                Time.timeScale = isPause ? 0.0f : 1.0f;

                if (isPause) {
                    Show(UI.PauseMenu);
                }
                else {
                    Show(UI.InGameMenu);
                }
            }
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
            Show(UI.InGameMenu);
        }

        void OnGameStop()
        {
            Show(UI.GameOverMenu);
        }

        void Show(UI ui)
        {
            HideAll();
            panels[(int)ui].gameObject.SetActive(true);
        }

        void HideAll()
        {
            foreach (RectTransform rect in panels)
            {
                rect.gameObject.SetActive(false);
            }
        }

        void Reset()
        {
            isPause = false;
            Time.timeScale = 1.0f;
        }

        public void GameStart()
        {
            GameController.Instance.GameStart();
        }

        public void GameStop()
        {
            GameController.Instance.GameStop();
        }

        public void Restart()
        {
            Reset();
            GameController.Instance.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Resume()
        {
            Reset();
            Show(UI.InGameMenu);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
