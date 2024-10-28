using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextNumeric : Numeric
{
    [SerializeField] private TMP_Text _ValueText;

    protected override string Text 
    { 
        get => _ValueText.text;
        set => _ValueText.text = value;
    }
}