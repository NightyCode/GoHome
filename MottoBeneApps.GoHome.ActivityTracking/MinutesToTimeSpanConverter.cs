namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Globalization;
    using System.Windows.Data;

    #endregion


    public class MinutesToTimeSpanConverter : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double minutes;

            if (value is int)
            {
                minutes = (int)value;
            }
            else if (value is double)
            {
                minutes = (double)value;
            }
            else
            {
                return null;
            }

            return TimeSpan.FromMinutes(minutes);
        }


        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                return null;
            }

            var span = (TimeSpan)value;

            return span.TotalMinutes;
        }

        #endregion
    }
}