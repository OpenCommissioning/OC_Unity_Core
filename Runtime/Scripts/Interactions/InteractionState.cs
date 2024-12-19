using System;
using System.Runtime.CompilerServices;

namespace OC.Interactions
{
    [Flags]
    public enum InteractionState
    {
        Disabled = 1,
        Enabled = 2,
        Selected = 4,
        Hovered = 8
    }

    public static class InteractionStateExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InteractionState SetFlag(this InteractionState state, InteractionState flag) => state | flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InteractionState RemoveFlag(this InteractionState state, InteractionState flag) => state & ~flag;
    }
}