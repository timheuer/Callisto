using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Callisto.TestApp.TemplateSelectors
{
    public class LiveTileTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Front { get; set; }
        public DataTemplate Back { get; set; }

        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            if (item is Callisto.TestApp.SamplePages.LiveTileSample.LiveTileData)
                return this.Front;
            else
                return this.Back;

            return base.SelectTemplateCore(item, container);
        }
    }
}
