using System;
using System.Runtime.CompilerServices;

namespace OC.Interactions
{
    [Flags]
    public enum InteractionState
    {
        Disabled = 0,
        Enabled = 1,
        Selected = 2,
        Hovered = 3
    }

    public static class InteractionStateExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InteractionState SetFlag(this InteractionState state, InteractionState flag) => state | flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InteractionState RemoveFlag(this InteractionState state, InteractionState flag) => state & ~flag;
    }
}