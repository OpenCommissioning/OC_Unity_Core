using OC.Components;
using OC.Interactions;
using UnityEngine.Events;

namespace OC.Editor
{
    public static class InteractionFactory
    {
        public static void TryFindAndConnectToTarget(this Interaction interaction)
        {
            var parent = interaction.transform.GetComponentInParent<MonoComponent>(2);
            if (parent == null) return;

            switch (parent)
            {
                case Button button:
                    interaction.Target = button.gameObject;
                    interaction.Mode = Interaction.InteractionMode.Click | Interaction.InteractionMode.Hover;
#if UNITY_EDITOR
                    interaction.OnPointerClickEvent = new UnityEvent();
                    interaction.OnPointerDownEvent = new UnityEvent();
                    interaction.OnPointerUpEvent = new UnityEvent();
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(interaction.OnPointerDownEvent, button.Press);
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(interaction.OnPointerUpEvent, button.Release);
#endif
                    
                    Logging.Logger.Log($"Interaction {interaction.GetType().Name} connected to {button.gameObject.name}", interaction);
                    break;
                
                case Switch @switch:
                    interaction.Target = @switch.gameObject;
                    interaction.Mode = Interaction.InteractionMode.Click | Interaction.InteractionMode.Hover;
                    
#if UNITY_EDITOR
                    interaction.OnPointerClickEvent = new UnityEvent();
                    interaction.OnPointerDownEvent = new UnityEvent();
                    interaction.OnPointerUpEvent = new UnityEvent();
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(interaction.OnPointerClickEvent, @switch.Click);
#endif
                    
                    Logging.Logger.Log($"Interaction {interaction.GetType().Name} connected to {@switch.gameObject.name}", interaction);
                    break;
                    
            }
        }
    }
}