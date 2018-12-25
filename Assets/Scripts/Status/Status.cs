using System;

namespace TJ
{
    [Serializable]
    public struct Status
    {
        public float current;
        public float max;

        public Status(float current, float max)
        {
            this.current = current;
            this.max = max;
        }

        public void FullRestore()
        {
            current = max;
        }

        public void Clear()
        {
            current = 0.0f;
        }

        public void Restore(float value)
        {
            current = ((current + value)) > max ? max : (current + value);
        }

        public void Remove(float value)
        {
            current = ((current - value)) < 0 ? 0 : (current - value);
        }
    }

    [Serializable]
    public struct StatusInt
    {
        public int current;
        public int max;

        public StatusInt(int current, int max)
        {
            this.current = current;
            this.max = max;
        }

        public void FullRestore()
        {
            current = max;
        }

        public void Clear()
        {
            current = 0;
        }

        public void Restore(int value)
        {
            current = ((current + value)) > max ? max : (current + value);
        }

        public void Remove(int value)
        {
            current = ((current - value)) < 0 ? 0 : (current - value);
        }
    }
}
