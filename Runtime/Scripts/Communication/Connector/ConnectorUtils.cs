namespace OC.Communication
{
    public static class ConnectorUtils
    {
        public static void SetBit(ref this byte self, int index, bool value)
        {
            index = Clamp(index, 0, 7);
            var mask = (byte)(1 << index);
            self = (byte)(value ? (self | mask) : (self & ~mask));
        }

        public static void SetBit(ref this ushort self, int index, bool value)
        {
            index = Clamp(index, 0, 15);
            var mask = (ushort)(1 << index);
            self = (ushort)(value ? (self | mask) : (self & ~mask));
        }

        public static void SetBit(ref this uint self, int index, bool value)
        {
            index = Clamp(index, 0, 31);
            var mask = (uint)(1 << index);
            self = value ? (self | mask) : (self & ~mask);
        }

        public static bool GetBit(this byte self, int index)
        {
            index = Clamp(index, 0, 7);
            return (self & (1 << index)) != 0;
        }

        public static bool GetBit(this ushort self, int index)
        {
            index = Clamp(index, 0, 15);
            return (self & (1 << index)) != 0;
        }

        public static bool GetBit(this uint self, int index)
        {
            index = Clamp(index, 0, 31);
            return (self & (1 << index)) != 0;
        }
        
        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            return value > max ? max : value;
        }
    }
}