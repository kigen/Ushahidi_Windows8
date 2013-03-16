using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Ushahidi
{
   public class ImageConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return FromAssets();
            }
            else
            {
                string url = value.ToString();
                return LoadPictrueByUrl(url);
            }
            }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }


        private async Task<BitmapImage> LoadPictrueByUrl(string url)
        {         
            var rass = RandomAccessStreamReference.CreateFromUri(new Uri(url));
            IRandomAccessStream stream = await rass.OpenReadAsync();
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);
            return bitmapImage;

        }

        public BitmapImage FromAssets()
        {
            var m_Image = new BitmapImage(new Uri("ms-appx:///Assets/placeholder.png"));
            return m_Image;
        }

    }
}
