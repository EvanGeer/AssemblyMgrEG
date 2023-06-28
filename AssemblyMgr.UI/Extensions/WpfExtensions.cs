using System.Windows.Media;
using System.Windows;

namespace AssemblyMgr.UI.Extensions
{
    internal static class WpfExtensions
    {
        public static T FindParent<T>(this DependencyObject child)
            where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent == null) return null;

            if (parent is T foundParent) return foundParent;

            return FindParent<T>(parent);
        }
    }
}
