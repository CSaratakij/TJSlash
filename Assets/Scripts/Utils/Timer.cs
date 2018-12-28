using UnityEngine;

namespace TJ
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        float current;

        [SerializeField]
        float max;

        public delegate void _Func();
        public delegate void _FuncValue(bool value);

        public event _Func OnTimerStart;
        public event _Func OnTimerStop;
        public event _FuncValue OnTimerPause;

        public float Current => current;
        public float Max => max;

        public bool IsPause { get; private set; }
        public bool IsStart { get; private set; }


        void Update()
        {
            TickHandler();
        }

        void TickHandler()
        {
            if (!IsStart || IsPause)
                return;

            if (current > 0.0f)
                Tick();
            else
                Stop();
        }

        void Tick()
        {
            current -= 1.0f* Time.deltaTime;
        }

        public void Pause(bool value)
        {
            IsPause = value;
            OnTimerPause?.Invoke(value);
        }

        public void Countdown()
        {
            if (IsStart)
                return;

            IsStart = true;
            OnTimerStart?.Invoke();
        }

        public void Stop()
        {
            if (!IsStart)
                return;

            Reset();
            OnTimerStop?.Invoke();
        }

        public void Reset()
        {
            current = max;
            IsStart = false;
            IsPause = false;
        }
    }
}
