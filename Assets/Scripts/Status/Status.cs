using System;

namespace TJ
{
    [Serializable]
    public struct Status
    {
        public delegate void _Func(float value);
        public event _Func OnValueChanged;

        public float current;
        public float max;

        public Status(float current, float max)
        {
            this.current = current;
            this.max = max;
            this.OnValueChanged = null;
        }

        public void FullRestore()
        {
            current = max;
            OnValueChanged?.Invoke(current);
        }

        public void Clear()
        {
            current = 0.0f;
            OnValueChanged?.Invoke(current);
        }

        public void Restore(float value)
        {
            current = ((current + value)) > max ? max : (current + value);
            OnValueChanged?.Invoke(current);
        }

        public void Remove(float value)
        {
            current = ((current - value)) < 0 ? 0 : (current - value);
            OnValueChanged?.Invoke(current);
        }
    }

    [Serializable]
    public struct StatusInt
    {
        public delegate void _Func(int value);
        public event _Func OnValueChanged;

        public int current;
        public int max;

        public StatusInt(int current, int max)
        {
            this.current = current;
            this.max = max;
            this.OnValueChanged = null;
        }

        public void FullRestore()
        {
            current = max;
            OnValueChanged?.Invoke(current);
        }

        public void Clear()
        {
            current = 0;
            OnValueChanged?.Invoke(current);
        }

        public void Restore(int value)
        {
            current = ((current + value)) > max ? max : (current + value);
            OnValueChanged?.Invoke(current);
        }

        public void Remove(int value)
        {
            current = ((current - value)) < 0 ? 0 : (current - value);
            OnValueChanged?.Invoke(current);
        }
    }
}
