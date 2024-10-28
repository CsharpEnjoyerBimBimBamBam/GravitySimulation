using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UnitsNumeric : Numeric
{
    public float UnitsValue => Value * _CurrentMultiplier;
    public event Action<string> UnitsChanged;
    public List<UnitsMultiplier> UnitsMultipliers
    {
        get => new List<UnitsMultiplier>(_UnitsMultipliers);
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            
            _UnitsMultipliers = value;
            _CurrentMultiplier = _UnitsMultipliers[0].Multiplier;
        }
    }
    public event Action InputFieldSelect;
    public event Action InputFieldDeselect;
    private float _CurrentMultiplier = 1;
    private string _LastText = "";
    [SerializeField] private List<UnitsMultiplier> _UnitsMultipliers = new List<UnitsMultiplier>();
    [SerializeField] private TMP_Dropdown _UnitsDropdown;
    [SerializeField] private TMP_InputField _InputField;

    private void Start()
    {
        if (_UnitsDropdown != null)
        {
            _UnitsDropdown.options = UnitsMultipliers.Select(X => new TMP_Dropdown.OptionData(X.Unit)).ToList();
            _UnitsDropdown.onValueChanged.AddListener((e) =>
            {
                UnitsMultiplier? unitsMultiplier = FindMultiplier();
                if (unitsMultiplier == null)
                {
                    _CurrentMultiplier = 1;
                    return;
                }

                _CurrentMultiplier = unitsMultiplier.Multiplier;
                UnitsChanged?.Invoke(unitsMultiplier.Unit);
            });
            if (_UnitsDropdown.options.Count != 0)
                _CurrentMultiplier = FindMultiplier()?.Multiplier ?? 1;
        }

        _InputField.onValueChanged.AddListener((text) => _LastText = text);

        _InputField.onEndEdit.AddListener((text) => 
        {
            if (!float.TryParse(text, out _))
                Text = _LastText;

            UpdateValue();
            InputFieldDeselect?.Invoke();
        });

        _InputField.onSelect.AddListener((e) => InputFieldSelect?.Invoke());
    }

    protected override string Text 
    { 
        get => _InputField.text;
        set => _InputField.text = value;
    }

    [Serializable]
    public class UnitsMultiplier
    {
        public string Unit;
        public float Multiplier;
    }

    private UnitsMultiplier? FindMultiplier() => UnitsMultipliers.FirstOrDefault(X => X.Unit == _UnitsDropdown.options[_UnitsDropdown.value].text);
}
