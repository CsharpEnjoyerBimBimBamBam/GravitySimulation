using Unity.VisualScripting;

public class NumberFormatter
{
    public static string FormatAsAPowerOf(float Value, int PowerOf)
    {
        FormattedNumberData Data = SplitValue(Value, PowerOf);

        string FormattedValue = $"{Data.Value}*{Data.Multiplier}^{Data.Power}";
        if (Data.Value == 1)
            FormattedValue = $"{Data.Multiplier}^{Data.Power}";
        return FormattedValue;
    }

    public static string FormatAsAPowerOfTen(float Value)
    {
        FormattedNumberData Data = SplitValue(Value, 10);
        return $"{Data.Value}E{Data.Power}";
    }

    public static bool TryParse(string FormattedValue, out FormattedNumberData Data)
    {
        if (TryParsePowerOf(FormattedValue, out Data))
            return true;

        if (TryParsePowerOfWithUnitMultiplier(FormattedValue, out Data))
            return true;

        if (TryParsePowerOfTen(FormattedValue, out Data))
            return true;

        return false;
    }

    private static bool TryParsePowerOf(string FormattedValue, out FormattedNumberData Data)
    {
        float Value;
        int PowerOf, Power;
        Data = new FormattedNumberData();

        if (!FormattedValue.Contains("*") || !FormattedValue.Contains("^"))
            return false;

        bool IsValueParsed = true;

        IsValueParsed = float.TryParse(FormattedValue.PartBefore(char.Parse("*")), out Value);

        string AfterMultiplier = FormattedValue.PartAfter(char.Parse("*"));

        IsValueParsed = int.TryParse(AfterMultiplier.PartBefore(char.Parse("^")), out PowerOf);

        IsValueParsed = int.TryParse(AfterMultiplier.PartAfter(char.Parse("^")), out Power);

        if (IsValueParsed)
        {
            Data = new FormattedNumberData(Value, PowerOf, Power);
            return true;
        }
        return false;
    }

    private static bool TryParsePowerOfWithUnitMultiplier(string FormattedValue, out FormattedNumberData Data)
    {
        int PowerOf, Power;
        Data = new FormattedNumberData();

        if (!FormattedValue.Contains("^"))
            return false;

        string[] SplitedNumber = FormattedValue.Split("^");

        if (SplitedNumber.Length != 2)
            return false;

        bool IsValueParsed = false;

        IsValueParsed = int.TryParse(SplitedNumber[0], out PowerOf);

        IsValueParsed = int.TryParse(SplitedNumber[1], out Power);

        if (IsValueParsed)
        {
            Data = new FormattedNumberData(1, PowerOf, Power);
            return true;
        }
        return false;
    }

    private static bool TryParsePowerOfTen(string FormattedValue, out FormattedNumberData Data)
    {
        float Value;
        int Power;
        Data = new FormattedNumberData();
        FormattedValue = FormattedValue.ToLower();

        if (!FormattedValue.Contains("e"))
            return false;

        string[] SplitedNumber = FormattedValue.Split("e");

        bool IsValueParsed = true;

        IsValueParsed = float.TryParse(SplitedNumber[0], out Value);

        IsValueParsed = int.TryParse(SplitedNumber[1], out Power);

        if (IsValueParsed)
        {
            Data = new FormattedNumberData(Value, 10, Power);
            return true;
        }
        return false;
    }

    private static FormattedNumberData SplitValue(float Value, int PowerOf)
    {
        int Power = 0;

        while (true)
        {
            Value /= PowerOf;
            if (Value < 1)
            {
                Value *= PowerOf;
                break;
            }
            Power++;
        }

        return new FormattedNumberData(Value, PowerOf, Power);
    }
}
