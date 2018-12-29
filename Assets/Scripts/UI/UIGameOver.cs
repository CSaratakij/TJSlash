using UnityEngine;
using UnityEngine.UI;

namespace TJ
{
    public class UIGameOver : MonoBehaviour
    {
        [SerializeField]
        Text txtGameOver;

        [SerializeField]
        Stat stat;


        void Awake()
        {
            SubscribeEvent();
        }

        void OnDestroy()
        {
            UnSubscribeEvent();
        }

        void OnGameStop()
        {
            txtGameOver.text = (stat.health.current > 0) ? "Thank you for playing" : "Game Over";
        }

        void SubscribeEvent()
        {
            GameController.Instance.OnGameStop += OnGameStop;
        }

        void UnSubscribeEvent()
        {
            GameController.Instance.OnGameStop -= OnGameStop;
        }
    }
}
