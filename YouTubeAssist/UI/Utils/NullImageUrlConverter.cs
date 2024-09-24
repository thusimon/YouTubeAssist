using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace YouTubeAssist.UI.Utils
{
    class NullImageUrlConverter : IValueConverter
    {
        private readonly Uri profileUri = new Uri("pack://application:,,,/Assets/Images/profile_holder_350.png");
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? profileUri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
