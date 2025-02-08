using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OC
{
    /// <summary>
    /// A generic reactive property that supports value change notification and optional validation.
    /// </summary>
    /// <typeparam name="T">The type of the property's value.</typeparam>
    [Serializable]
    public class Property<T> : IProperty<T>
    {
        public event Action<T> OnValueChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property{T}"/> class with the default value of type <typeparamref name="T"/>.
        /// </summary>
        public Property() : this(default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property{T}"/> class with the specified initial value.
        /// </summary>
        /// <param name="value">The initial value of the property.</param>
        public Property(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets or sets the optional validator function that is applied to new values before they are set.
        /// The function should return the validated (or transformed) value.
        /// </summary>
        public Func<T, T> Validator;

        public T Value
        {
            get => _value;
            set => TrySetValue(value);
        }
        
        public bool Force
        {
            get => _force;
            set => _force = value;
        }

        /// <summary>
        /// The backing field for the property value. This field is serialized for Unity Inspector support.
        /// </summary>
        [SerializeField]
        private T _value;
        
        [SerializeField]
        private bool _force;
        
        /// <summary>
        /// Stores the last value assigned to the property, used for change detection from inspector using OnValidate.
        /// </summary>
        private T _lastValue;

        /// <summary>
        /// The equality comparer used to compare values of type <typeparamref name="T"/>.
        /// </summary>
        private EqualityComparer<T> _comparer;

        /// <summary>
        /// Attempts to set the property value to a new value. 
        /// If the force flag (<c>_force</c>) is enabled, the method exits immediately, preventing automatic updates
        /// from the <see cref="Value"/> property. Otherwise, it applies the optional <see cref="Validator"/> (if provided)
        /// to the input value, compares the validated value with the current value using an equality comparer,
        /// and, if the values differ, updates the property by invoking <see cref="SetValue(T)"/>.
        /// </summary>
        /// <param name="value">The new value to attempt to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetValue(T value)
        {
            if (_force) return;
            if (Validator is not null) value = Validator.Invoke(value);
            _comparer ??= EqualityComparer<T>.Default;
            if (_comparer.Equals(_value, value)) return;

            SetValue(value);
        }
        
        /// <summary>
        /// Sets the property value and invokes the <see cref="OnValueChanged"/> event.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(T value)
        {
            SetValueWithoutNotify(value);
            OnValueChanged?.Invoke(value);
        }

        /// <summary>
        /// Sets the property value without triggering the <see cref="OnValueChanged"/> event.
        /// This method is useful when you want to update the property silently.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        public void SetValueWithoutNotify(T value)
        {
            _value = value;
            _lastValue = value;
        }
        
        /// <summary>
        /// Validates and updates the property value if needed.
        /// Intended to be called in the Unity Editor (e.g., via <c>OnValidate</c>),
        /// it applies the validator and, if the value has changed, triggers the <see cref="OnValueChanged"/> event.
        /// </summary>
        public void OnValidate()
        {
            if (Validator is not null) _value = Validator.Invoke(_value);
            _comparer ??= EqualityComparer<T>.Default;
            if (_comparer.Equals(_value, _lastValue)) return;
            
            SetValue(_value);
        }
        
        public void Subscribe(Action<T> action)
        {
            OnValueChanged += action;
            action?.Invoke(_value);
        }
        
        public void Unsubscribe(Action<T> action)
        {
            OnValueChanged -= action;
        }

        /// <summary>
        /// Defines an implicit conversion from a value of type <typeparamref name="T"/> to a <see cref="Property{T}"/>.
        /// This allows direct assignment of a value to a property.
        /// </summary>
        /// <param name="value">The value to wrap in a <see cref="Property{T}"/>.</param>
        public static implicit operator Property<T>(T value)
        {
            return new Property<T>(value);
        }

        /// <summary>
        /// Defines an implicit conversion from a <see cref="Property{T}"/> to its underlying value of type <typeparamref name="T"/>.
        /// This allows the property to be used directly as its underlying type.
        /// </summary>
        /// <param name="binding">The <see cref="Property{T}"/> instance.</param>
        public static implicit operator T(Property<T> binding) => binding._value;

        /// <summary>
        /// Returns a string that represents the current property value.
        /// </summary>
        /// <returns>A string representation of the property value.</returns>
        public override string ToString() =>  _value.ToString();
    }
}