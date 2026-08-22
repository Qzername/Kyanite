using System.Collections.ObjectModel;

namespace Kyanite;

public static class ObservableCollectionExtensions
{
     /// <summary>
     /// replaces objects in observable collection with new objects also calling the CollectionChanged event
     /// </summary>
     public static void Replace<T>(this ObservableCollection<T> collection, T[] items)
     {
          collection.Clear();
          foreach (var item in items)
               collection.Add(item);
     }
}
