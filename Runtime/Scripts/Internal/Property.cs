using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OC
{
    [Serializable]
    public class Property<T> : IProperty<T>
    {
        public event Action<T> OnValueChanged;

        public Property() : this(default)
        {
        }

        public Property(T value)
        {
            _value = value;
        }

        public Func<T, T> Validator;

        public T Value
        {
            get => _value;
            set => TrySetValue(value);
        }

        [SerializeField]
        private T _value;
        
        private T _lastValue;

        private EqualityComparer<T> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetValue(T value)
        {
            if (Validator is not null) value = Validator.Invoke(value);
            _comparer ??= EqualityComparer<T>.Default;
            if (_comparer.Equals(_value, value)) return false;

            SetValue(value);
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetValue(T value)
        {
            SetWithoutNotify(value);
            OnValueChanged?.Invoke(value);
        }
        
        public void ForceSetValue(T value)
        {
            SetValue(value);
        }

        public void SetWithoutNotify(T value)
        {
            _value = value;
            _lastValue = value;
        }
        
        public void OnValidate()
        {
            if (Validator is not null) _value = Validator.Invoke(_value);
            _comparer ??= EqualityComparer<T>.Default;
            if (_comparer.Equals(_value, _lastValue)) return;
            
            SetValue(_value);
        }

        public static implicit operator Property<T>(T value)
        {
            return new Property<T>(value);
        }

        public static implicit operator T(Property<T> binding) => binding._value;

        public override string ToString() =>  _value.ToString();
    }
}