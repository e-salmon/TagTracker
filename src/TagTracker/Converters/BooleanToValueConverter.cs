using System;
using System.Windows.Data;

namespace TagTracker.Converters
{
    /// <summary>
    /// Converts a boolean value to either a <see cref="TrueValue"/> or <see cref="FalseValue"/>
    /// representation.
    /// </summary>
    public class BooleanToValueConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the value to use when boolean evaluation returns <c>true</c>.
        /// </summary>
        /// <value>
        /// The true value.
        /// </value>
        public object TrueValue { get; set; }

        /// <summary>
        /// Gets or sets the value to use when boolean evaluation returns <c>false</c>.
        /// </summary>
        /// <value>
        /// The false value.
        /// </value>
        public object FalseValue { get; set; }

        /// <summary>
        /// Converts a boolean value to either <see cref="TrueValue"/> or <see cref="FalseValue"/>.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// Either <see cref="TrueValue"/> or <see cref="FalseValue"/>.
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? TrueValue : FalseValue;
            }

            throw new InvalidOperationException("Value to be converted must be a boolean.");
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
