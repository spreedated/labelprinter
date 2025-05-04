using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LabelWhisper.ViewLogic
{
    public sealed class TitleConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || !values.All(x => x is string) || values.Count <= 1)
            {
                return null;
            }

            return $"{values[0]}\n{values[1]}";
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
