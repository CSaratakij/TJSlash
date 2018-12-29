using UnityEngine;

namespace TJ
{
    public class GameController : MonoBehaviour
    {
        public delegate void _Func();

        public event _Func OnGameStart;
        public event _Func OnGameStop;

        public static GameController Instance { get; private set; }
        public static bool IsGameStart { get; private set; }

        void Awake()
        {
            MakeSingleton();
        }

        void MakeSingleton()
        {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        public void GameStart()
        {
            if (IsGameStart)
                return;

            IsGameStart = true;
            OnGameStart?.Invoke();
        }

        public void GameStop()
        {
            if (!IsGameStart)
                return;

            IsGameStart = false;
            OnGameStop?.Invoke();
        }

        public void Reset()
        {
            IsGameStart = false;
        }
    }
}
