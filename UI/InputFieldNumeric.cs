using TMPro;
using UnityEngine;

public class InputFieldNumeric : Numeric
{
    [SerializeField] private TMP_InputField _ValueInputField;

    protected override string Text
    {
        get => _ValueInputField.text;
        set => _ValueInputField.text = value;
    }
}
