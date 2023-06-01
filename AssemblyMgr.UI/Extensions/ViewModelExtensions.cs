using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AssemblyMgr.UI.Extensions
{
    public static class ViewModelExtensions
    {
        public static void Notify(this INotifyPropertyChanged vm, PropertyChangedEventHandler PropertyChanged, [CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(vm, new PropertyChangedEventArgs(propertyName));

        public static void Notify(this INotifyPropertyChanged vm, PropertyChangedEventHandler PropertyChanged, Action updateAction, [CallerMemberName] string propertyName = null)
            => vm.Notify(PropertyChanged, updateAction, null, propertyName);

        public static void Notify(this INotifyPropertyChanged vm, PropertyChangedEventHandler PropertyChanged, Action updateAction, string[] alsoNotify, [CallerMemberName] string callerName = null)
        {
            updateAction.Invoke();

            if (vm == null || PropertyChanged == null) 
                return;
            
            PropertyChanged?.Invoke(vm, new PropertyChangedEventArgs(callerName));

            if (alsoNotify == null) 
                return;

            foreach (var additionalProperty in alsoNotify)
                PropertyChanged?.Invoke(vm, new PropertyChangedEventArgs(additionalProperty));
        }
    }
}
