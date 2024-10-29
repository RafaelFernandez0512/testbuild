namespace SigeTools.MAUI.Helpers
{
    public class TextSizeToRingSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string text)
            {
                // Calcula el tamaño requerido para el texto
                var label = new Label { Text = text };
                var sizeRequest = label.Measure(double.PositiveInfinity, double.PositiveInfinity);

                // Devuelve las dimensiones requeridas como Size
                return new Size(sizeRequest.Request.Width, sizeRequest.Request.Height);
            }

            return new Size(0, 0); // Valor predeterminado
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}