using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CoolBytes.ScriptInterpreter.Helpers;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    internal void RaisePropertyChanged([CallerMemberName()] string nomPropriete = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomPropriete));
    }

    internal void OnPropertyChanged<T>(Expression<Func<T>> action)
    {
        string propertyName = GetPropertyName(action);
        RaisePropertyChanged(propertyName);
    }

    internal static string GetPropertyName<T>(Expression<Func<T>> action)
    {
        MemberExpression expression = (MemberExpression)action.Body;
        string propertyName = expression.Member.Name;
        return propertyName;
    }

    internal void RaiseCollectionPropertyChanged<T>(IEnumerable<T> collection, params KeyValuePair<string, List<string>>[] propertiesDictionnary) where T : INotifyPropertyChanged
    {
        if (collection != null)
        {
            foreach (T item in collection)
            {
                DoPropertyChangedForRaise(item, propertiesDictionnary);
            }
        }
    }

    internal void RaiseChildPropertyChanged<T>(T item, params KeyValuePair<string, List<string>>[] propertiesDictionnary) where T : INotifyPropertyChanged
    {
        if (item is not null)
        {
            DoPropertyChangedForRaise(item, propertiesDictionnary);
        }
    }

    internal void RaiseCollectionItemsChanged<T>(ObservableCollection<T> collection, params string[] propertiesRaise) where T : INotifyPropertyChanged
    {
        if (collection != null)
        {
            collection.CollectionChanged += (sender, e) =>
            {
                foreach (string propertyName in propertiesRaise.ToList())
                {
                    this.RaisePropertyChanged(propertyName);
                }
            };
        }
    }

    internal static KeyValuePair<string, List<string>> BuildPropertyForRaise(string property, params string[] propertiesRaise)
    {
        return new KeyValuePair<string, List<string>>(property, [.. propertiesRaise]);
    }

    internal void DoPropertyChangedForRaise<T>(T item, params KeyValuePair<string, List<string>>[] propertiesDictionnary) where T : INotifyPropertyChanged
    {
        item.PropertyChanged += (sender, e) =>
        {
            foreach (KeyValuePair<string, List<string>> kvItem in propertiesDictionnary)
                if (e.PropertyName == kvItem.Key)
                    foreach (string valueItem in kvItem.Value)
                        this.RaisePropertyChanged(valueItem);
        };
    }
}
