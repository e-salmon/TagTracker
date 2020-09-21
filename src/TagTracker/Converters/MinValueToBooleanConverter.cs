using System;
using System.Windows.Data;

namespace TagTracker.Converters
{
    /// <summary>
    /// Converter to return <c>true</c> if a value is above a specified <see cref="MinimumValue"/>
    /// threshold, or <c>false</c> otherwise.
    /// </summary>
    public class MinValueToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public int MinimumValue { get; set; }

        /// <summary>
        /// Converts a value to a boolean by checking whether the value is equal to or greater than
        /// the <see cref="MinimumValue"/>.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// <c>true</c> if the value is equal to or greater than MinimumValue; Otherwise <c>false</c>.
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (value is int)
            {
                return (int)value >= MinimumValue;
            }

            throw new InvalidOperationException("Value to be converted must be an integer.");
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
