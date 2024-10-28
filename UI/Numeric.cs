using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Numeric : MonoBehaviour
{
    public float DefaultValue = 1;
    public float MinValue 
    { 
        get => _MinValue; 
        set 
        {
            _MinValue = Mathf.Clamp(value, _MinValue, _MaxValue);
            UpdateValue();
        } 
    }
    public float MaxValue
    {
        get => _MaxValue;
        set
        {
            _MaxValue = Mathf.Clamp(value, _MinValue, _MaxValue);
            UpdateValue();
        }
    }
    public virtual float Value
    {
        get => ValidateValue(out float Value) ? Value : DefaultValue;
        set
        {
            float NewValue = Mathf.Clamp(value, MinValue, MaxValue);
            Text = NewValue.ToString();
            ValueChanged?.Invoke(NewValue);
        }
    }
    public event Action<float> ValueChanged;
    [SerializeField] private bool _FormatBigValues;
    [SerializeField] private float _MinFormatValue = 0.00001f;
    [SerializeField] private float _MaxFormatValue = 1000;
    [SerializeField] private float _MinValue = 0.25f;
    [SerializeField] private float _MaxValue = 128;

    protected abstract string Text { get; set; }

    protected bool ValidateValue(out float Value)
    {
        if (_FormatBigValues && NumberFormatter.TryParse(Text, out FormattedNumberData Data))
        {
            Value = Data.Calculate();
            return true;
        }

        if (float.TryParse(Text, out Value))
            return true;

        Text = DefaultValue.ToString();
        return false;
    }

    protected void UpdateValue() => Value = Value;
}
