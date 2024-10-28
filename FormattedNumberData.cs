using UnityEngine;

public class FormattedNumberData
{
    public FormattedNumberData(float value, int multiplier, int power)
    {
        Value = value;
        Multiplier = multiplier;
        Power = power;
    }

    public FormattedNumberData() { }

    public float Value { get; private set; }
    public int Multiplier { get; private set; }
    public int Power { get; private set; }

    public float Calculate() => Value * Mathf.Pow(Multiplier, Power);
}
