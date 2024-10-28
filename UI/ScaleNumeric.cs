using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class ScaleNumeric : TextNumeric
{
    public int Multiplier
    {
        get => _Multiplier;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value must be bigger than 0");
            _Multiplier = value;
        }
    }
    public event Action<float> ValueIncreased;
    public event Action<float> ValueDecreased;
    [SerializeField] private int _Multiplier = 2;
    [SerializeField] private Button _UpScaleButton;
    [SerializeField] private Button _DownScaleButton;

    public void IncreaseValue()
    {
        Value *= Multiplier;
        ValueIncreased?.Invoke(Value);
    }

    public void DecreaseValue()
    {
        Value /= Multiplier;
        ValueDecreased?.Invoke(Value);
    }

    private void Start()
    {
        ValidateValue(out _);

        _UpScaleButton.onClick.AddListener(() =>
        {
            if (!ValidateValue(out _))
                return;

            IncreaseValue();
        });

        _DownScaleButton.onClick.AddListener(() =>
        {
            if (!ValidateValue(out _))
                return;

            DecreaseValue();
        });
    }
}
