using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;  // Thêm thư viện này để sử dụng SolidColorBrush

namespace Maze_Solving.Views.Converters
{
    /// <summary>
    /// Converter for cell type to color
    /// </summary>
    public class CellTypeColorConverter : IValueConverter
    {
        /// <summary>
        /// Convert cell type to color
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.White;  // Nếu giá trị là null, trả về màu trắng

            switch ((Models.Enums.CellType)value)
            {
                case Models.Enums.CellType.Empty:
                    return Brushes.White;
                case Models.Enums.CellType.Wall:
                    return Brushes.Black;
                case Models.Enums.CellType.Start:
                    return Brushes.Green;
                case Models.Enums.CellType.End:
                    return Brushes.Red;
                case Models.Enums.CellType.Search:
                    return Brushes.Cyan;
                case Models.Enums.CellType.Path:
                    return Brushes.HotPink;
                case Models.Enums.CellType.Visited:
                    return Brushes.Yellow;
                default:
                    return Brushes.White;  // Mặc định là màu trắng
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
