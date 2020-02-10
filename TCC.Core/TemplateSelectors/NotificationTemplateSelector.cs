﻿using System.Windows;
using System.Windows.Controls;
using TCC.Utils;
using TCC.Windows.Widgets;

namespace TCC.TemplateSelectors
{
    public class NotificationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Default { get; set; }
        public DataTemplate Progress { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var n = (NotificationInfoBase) item;

            if (n != null)
                return n.NotificationTemplate switch
                {
                    NotificationTemplate.Progress => Progress,
                    _ => Default
                };
            else return Default;
        }
    }
}