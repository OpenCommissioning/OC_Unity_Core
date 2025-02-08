using System;

namespace OC
{
    /// <summary>
    /// Represents the base interface for property types.
    /// </summary>
    public interface IProperty
    {
        
    }
    
    /// <summary>
    /// Represents a read-only reactive property that notifies subscribers when its value changes.
    /// </summary>
    /// <typeparam name="T">The type of the property's value.</typeparam>
    public interface IPropertyReadOnly<T> : IProperty
    {
        /// <summary>
        /// Gets the current value of the property.
        /// </summary>
        public T Value { get; }
        
        /// <summary>
        /// Validates the property's value.
        /// Typically called in the Unity Editor to ensure the value meets required conditions.
        /// </summary>
        public void OnValidate();
        
        /// <summary>
        /// Subscribes an action to be invoked when the property value changes.
        /// </summary>
        /// <param name="action">The action to subscribe.</param>
        public void Subscribe(Action<T> action);
        
        /// <summary>
        /// Unsubscribes an action from the property value change notifications.
        /// </summary>
        /// <param name="action">The action to unsubscribe.</param>
        public void Unsubscribe(Action<T> action);
        
        /// <summary>
        /// Occurs when the property value changes.
        /// Subscribers are notified with the new value.
        /// </summary>
        public event Action<T> OnValueChanged;
    }
    
    /// <summary>
    /// Represents a reactive property that supports both read and write operations,
    /// including change notifications and value updates without triggering notifications.
    /// </summary>
    /// <typeparam name="T">The type of the property's value.</typeparam>
    public interface IProperty<T> : IPropertyReadOnly<T>
    {
        /// <summary>
        /// Gets or sets the current value of the property.
        /// Setting the value may trigger change notifications.
        /// </summary>
        public new T Value { get; set; }
        
        /// <summary>
        /// Gets or sets a flag that determines whether the property should be updated forcefully.
        /// When set to <c>true</c>, the property value can be overwritten regardless of potential conflicts with other code.
        /// This is typically used in UI scenarios where a user needs to override the current value.
        /// </summary>
        public bool Force { get; set; }
        
        /// <summary>
        /// Sets the property value and triggers the change notification event.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetValue(T value);
        
        /// <summary>
        /// Sets the property value without triggering the change notification event.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetValueWithoutNotify(T value);
    }
}