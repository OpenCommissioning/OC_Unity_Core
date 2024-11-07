using System;
using UnityEngine.UIElements;

namespace OC.Interactions.UIElements
{
    public class MouseEvents : MouseManipulator
    {
        public event Action Clicked;
        public event Action Down;
        public event Action Up;

        private bool _active;

        public MouseEvents()
        {
            activators.Add(new ManipulatorActivationFilter()
            {
                button = MouseButton.LeftMouse
            });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (!CanStartManipulation(evt)) return;
            ProcessDownEvent(evt, PointerId.mousePointerId);
        }
        
        private void OnMouseUp(MouseUpEvent evt)
        {
            if (!_active || !CanStopManipulation(evt)) return;
            ProcessUpEvent(evt, PointerId.mousePointerId);
        }
        
        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            if (!_active) return;
            ProcessUpEvent(evt, PointerId.mousePointerId);
        }

        private void ProcessDownEvent(EventBase evt, int pointerId)
        {
            _active = true;
            target.CapturePointer(pointerId);
            evt.StopImmediatePropagation();
            Down?.Invoke();
        }

        private void ProcessUpEvent(EventBase evt, int pointerId)
        {
            _active = false;
            target.ReleasePointer(pointerId);
            evt.StopPropagation();
            Up?.Invoke();
            Clicked?.Invoke();
        }
    }
}