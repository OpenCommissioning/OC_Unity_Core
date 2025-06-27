using System;

namespace OC.MaterialFlow
{
    [Flags]
    public enum CollisionFilter
    {
        None = 0,
        Part = 1,
        Assembly = 2,
        Transport = 4,
        Static = 8,
        Storage = 16,
        All = ~0
    }
}