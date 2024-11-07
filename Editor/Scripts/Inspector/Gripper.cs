using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(MaterialFlow.Gripper), true), CanEditMultipleObjects]
    public class Gripper : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as MaterialFlow.Gripper;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            var pickButton = new UnityEngine.UIElements.Button{text = "Pick"};
            var placeButton = new UnityEngine.UIElements.Button{text = "Place"};
            pickButton.clicked += component.Pick;
            placeButton.clicked += component.Place;
            groupControl.Add(pickButton);
            groupControl.Add(placeButton);
            
            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Collision", Color.yellow).BindProperty(component.Collision).AlignedField());
            groupStatus.Add(new LampField("Is Active", Color.green).BindProperty(component.IsActive).AlignedField());
            groupStatus.Add(new LampField("Is Picked", Color.green).BindProperty(component.IsPicked).AlignedField());
            
            var groupCollision = new PropertyGroup("Collision");
            groupCollision.Add(new PropertyField{bindingPath = "_pickType"});
            groupCollision.Add(new PropertyField{bindingPath = "_dynamicSize"});
            groupCollision.Add(new Vector3Field("Add. Collider Size"){bindingPath = "_additionalColliderSize"}.AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new PropertyField{bindingPath = "_groupId"});
            groupSettings.Add(new PropertyField{bindingPath = "_collisionFilter"});

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnCollisionEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPayloadEnterEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPayloadExitEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPickEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPlaceEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnIsActiveChangedEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnIsPickedChangedEvent"});

            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupCollision);
            container.Add(groupSettings);
            container.Add(groupEvents);
            
            return container;
        }
    }
}