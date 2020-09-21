using System;
using System.Windows.Data;

namespace TagTracker.Converters
{
    /// <summary>
    /// Converts a <c>null</c> value, or a 0, to a replacement value.
    /// </summary>
    public class MissingValueConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the string to use to represent a missing value.
        /// </summary>
        /// <value>
        /// The string to use to represent a missing value.
        /// </value>
        public string ReplacementValue { get; set; }

        /// <summary>
        /// Converts a <paramref name="value"/> to either <see cref="MissingValue"/> if it is an 
        /// integer equal to 0, or an empty string. Otherwise, the value is returned unchanged.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// Either <see cref="MissingValue"/> or <paramref name="value"/>.
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                int paramValue;

                if (parameter == null || !int.TryParse((string)parameter, out paramValue))
                {
                    paramValue = 0;
                }

                return (int)value == paramValue ? ReplacementValue : value;
            }

            return string.IsNullOrEmpty((string)value) ? ReplacementValue : value;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
