using System;
using System.Runtime.CompilerServices;

namespace OC.Interactions
{
    [Flags]
    public enum InteractionState
    {
        Idle = 1,
        Hovered = 2,
        Selected = 4
    }

    public static class InteractionStateExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InteractionState SetFlag(this InteractionState state, InteractionState flag) => state | flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InteractionState RemoveFlag(this InteractionState state, InteractionState flag) => state & ~flag;
    }
}