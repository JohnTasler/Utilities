using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Tasler.Windows.Data
{
  [ContentProperty("Mappings")]
  public class MappingConverter : IValueConverter
  {
    #region Properties
    private MappingCollection mappings = new MappingCollection();
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public MappingCollection Mappings
    {
      get { return this.mappings; }
    }
    #endregion Properties

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      object mappedValue = this.mappings.Find(value);
      Debug.WriteLine(string.Format("MappingConverter.Convert: {0}={1}\n", value, mappedValue));
      return mappedValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion IValueConverter Members
  }
                             
  public class Mapping
  {
    public object Key { get; set; }
    public object Value { get; set; }
  }

  public class MappingCollection
    : IList<Mapping>
    , ICollection<Mapping>
    , IEnumerable<Mapping>
    , IList
    , ICollection
    , IEnumerable
  {
    #region Instance Fields
    private List<Mapping> list = new List<Mapping>();
    private Dictionary<object, object> dictionary = new Dictionary<object, object>();
    #endregion Instance Fields

    #region Methods
    internal object Find(object key)
    {
      object result = null;
      this.dictionary.TryGetValue(key, out result);
      return result;
    }
    #endregion Methods

    #region IList<Mapping> Members

    int IList<Mapping>.IndexOf(Mapping item)
    {
      return this.list.IndexOf(item);
    }

    void IList<Mapping>.Insert(int index, Mapping item)
    {
      this.list.Insert(index, item);
      this.dictionary.Add(item.Key, item.Value);
    }

    void IList<Mapping>.RemoveAt(int index)
    {
      this.dictionary.Remove(this.list[index].Key);
      this.list.RemoveAt(index);
    }

    Mapping IList<Mapping>.this[int index]
    {
      get
      {
        return this.list[index];
      }
      set
      {
        if (index < this.list.Count)
          this.dictionary.Remove(this.list[index].Key);

        this.dictionary[value.Key] = value.Value;
        this.list[index] = value;
      }
    }

    #endregion IList<Mapping> Members

    #region ICollection<Mapping> Members

    int ICollection<Mapping>.Count
    {
      get
      {
        return this.list.Count;
      }
    }

    bool ICollection<Mapping>.IsReadOnly
    {
      get
      {
        return ((IList)this.list).IsReadOnly;
      }
    }

    public void Add(Mapping item)
    {
      this.list.Add(item);
      this.dictionary.Add(item.Key, item.Value);
    }

    void ICollection<Mapping>.Clear()
    {
      this.dictionary.Clear();
      this.list.Clear();
    }

    bool ICollection<Mapping>.Contains(Mapping item)
    {
      return this.list.Contains(item);
    }

    void ICollection<Mapping>.CopyTo(Mapping[] array, int arrayIndex)
    {
      this.list.CopyTo(array, arrayIndex);
    }

    bool ICollection<Mapping>.Remove(Mapping item)
    {
      this.dictionary.Remove(item.Key);
      return this.list.Remove(item);
    }

    #endregion ICollection<Mapping> Members

    #region IEnumerable<Mapping> Members

    IEnumerator<Mapping> IEnumerable<Mapping>.GetEnumerator()
    {
      return this.list.GetEnumerator();
    }

    #endregion IEnumerable<Mapping> Members

    #region IList Members

    int IList.Add(object value)
    {
      ((IList<Mapping>)this).Add((Mapping)value);
      return this.list.Count - 1;
    }

    void IList.Clear()
    {
      ((IList<Mapping>)this).Clear();
    }

    bool IList.Contains(object value)
    {
      return ((IList<Mapping>)this).Contains((Mapping)value);
    }

    int IList.IndexOf(object value)
    {
      return ((IList<Mapping>)this).IndexOf((Mapping)value);
    }

    void IList.Insert(int index, object value)
    {
      ((IList<Mapping>)this).Insert(index, (Mapping)value);
    }

    bool IList.IsFixedSize
    {
      get
      {
        return false;
      }
    }

    bool IList.IsReadOnly
    {
      get
      {
        return false;
      }
    }

    void IList.Remove(object value)
    {
      ((IList<Mapping>)this).Remove((Mapping)value);
    }

    void IList.RemoveAt(int index)
    {
      ((IList<Mapping>)this).RemoveAt(index);
    }

    object IList.this[int index]
    {
      get
      {
        return ((IList<Mapping>)this)[index];
      }
      set
      {
        ((IList<Mapping>)this)[index] = (Mapping)value;
      }
    }

    #endregion IList Members

    #region ICollection Members

    void ICollection.CopyTo(Array array, int index)
    {
      ((IList<Mapping>)this).CopyTo((Mapping[])array, index);
    }

    int ICollection.Count
    {
      get
      {
        return ((IList<Mapping>)this).Count;
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return false;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return this.list;
      }
    }

    #endregion ICollection Members

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.list.GetEnumerator();
    }

    #endregion IEnumerable Members
  }
}
