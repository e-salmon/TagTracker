using System;
using System.Globalization;
using System.Windows.Data;

namespace TagTracker.Converters
{
    /// <summary>
    /// The count-to-value converter.
    /// </summary>
    public class CountToValueConverter : IValueConverter
    {
        public string Zero { get; set; }
        
        public string One { get; set; }
        
        public string Many { get; set; }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Zero;
            }

            if (value is int)
            {
                var count = (int)value;
                return count == 0 ? Zero : count == 1 ? One : Many;
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
        /// <exception cref="System.NotImplementedException">One way converter, no conversion back.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}