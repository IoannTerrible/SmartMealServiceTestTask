using System.Globalization;
using System.Windows.Data;

namespace SmsClientLibrary.WpfClient.Converters;

/// <summary>Converts a bool to its inverse: true → false, false → true.</summary>
public class InverseBooleanConverter : IValueConverter
{
    // Convert from source (bool) to target (bool)
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b) return !b;
        return Binding.DoNothing;
    }

    // ConvertBack is also implemented so you can two‑way bind if needed
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b) return !b;
        return Binding.DoNothing;
    }
}
