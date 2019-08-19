using MvvmScarletToolkit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FS
{
    [ValueConversion(typeof(ConsoleColor), typeof(Brush))]
    public sealed class ConsoleColorConverter : ConverterMarkupExtension<ConsoleColorConverter>
    {
        private readonly static Dictionary<ConsoleColor, Brush> _lookup = new Dictionary<ConsoleColor, Brush>
        {
            [ConsoleColor.Gray] = new SolidColorBrush(Colors.Gray),
            [ConsoleColor.Black] = new SolidColorBrush(Colors.Black),
            [ConsoleColor.Blue] = new SolidColorBrush(Colors.Blue),
            [ConsoleColor.Cyan] = new SolidColorBrush(Colors.Cyan),
            [ConsoleColor.Gray] = new SolidColorBrush(Colors.Gray),
            [ConsoleColor.Green] = new SolidColorBrush(Colors.Gray),
            [ConsoleColor.Magenta] = new SolidColorBrush(Colors.Magenta),
            [ConsoleColor.Red] = new SolidColorBrush(Colors.Red),
            [ConsoleColor.White] = new SolidColorBrush(Colors.LightGray),
            [ConsoleColor.Yellow] = new SolidColorBrush(Colors.Orange),

            [ConsoleColor.DarkBlue] = new SolidColorBrush(Colors.DarkBlue),
            [ConsoleColor.DarkCyan] = new SolidColorBrush(Colors.DarkCyan),
            [ConsoleColor.DarkGray] = new SolidColorBrush(Colors.DarkGray),
            [ConsoleColor.DarkGreen] = new SolidColorBrush(Colors.DarkGreen),
            [ConsoleColor.DarkMagenta] = new SolidColorBrush(Colors.DarkMagenta),
            [ConsoleColor.DarkRed] = new SolidColorBrush(Colors.DarkRed),
            [ConsoleColor.DarkYellow] = new SolidColorBrush(Color.FromArgb(255, 218, 165, 32)),
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConsoleColor color)
            {
                return _lookup[color];
            }

            return _lookup[ConsoleColor.Gray];
        }
    }
}
