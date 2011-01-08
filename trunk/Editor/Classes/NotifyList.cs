using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Paril.Collections
{
	public class NotifyList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
	{
		List<T> _internalList;

		public NotifyList()
		{
			_internalList = new List<T>();
		}

		public NotifyList(IEnumerable<T> collection)
		{
			_internalList = new List<T>(collection);
		}

		public NotifyList(int capacity)
		{
			_internalList = new List<T>(capacity);
		}

		#region IList<T> Members
		T IList<T>.this[int index]
		{
			get { return this[index]; }
			set { this[index] = value; }
		}

		int IList<T>.IndexOf(T item)
		{
			return IndexOf(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			Insert(index, item);
		}

		void IList<T>.RemoveAt(int index)
		{
			RemoveAt(index);
		}
		#endregion

		#region ICollection<T> Members
		int ICollection<T>.Count
		{
			get { return Count; }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		void ICollection<T>.Clear()
		{
			Clear();
		}

		bool ICollection<T>.Contains(T item)
		{
			return Contains(item);
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.Remove(T item)
		{
			Remove(item);
			return true;
		}
		#endregion

		#region IList Members
		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool IList.IsReadOnly
		{
			get { return false; }
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (T)value; }
		}

		int IList.Add(object value)
		{
			return Add((T)value);
		}

		void IList.Clear()
		{
			Clear();
		}

		bool IList.Contains(object value)
		{
			return Contains((T)value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T)value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T)value);
		}

		void IList.Remove(object value)
		{
			Remove((T)value);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}
		#endregion

		#region ICollection Members
		int ICollection.Count
		{
			get { return Count; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return null; }
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo(array, index);
		}
		#endregion

		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}
		#endregion

		/// <summary>
		/// Event called per-object when an object is removed.
		/// </summary>
		public event NotifyListEventHandler<T> ObjectRemoved;

		/// <summary>
		/// Event called per-object when an object is added.
		/// </summary>
		public event NotifyListEventHandler<T> ObjectAdded;

		/// <summary>
		/// Event called after objects are removed to the list.
		/// </summary>
		public event EventHandler ObjectsRemoved;

		/// <summary>
		/// Event called after objects are added to the list.
		/// </summary>
		public event EventHandler ObjectsAdded;

		public IEnumerator<T> GetEnumerator()
		{
			return _internalList.GetEnumerator();
		}

		public T this[int index]
		{
			get { return _internalList[index]; }
			set { _internalList[index] = value; }
		}

		public int IndexOf(T value)
		{
			return _internalList.IndexOf(value);
		}

		public void Insert(int index, T value)
		{
			_internalList.Insert(index, value);
		}

		public void Remove(T value)
		{
			_internalList.Remove(value);

			if (ObjectRemoved != null)
				ObjectRemoved(this, new NotifyListEventArgs<T>(value));

			if (ObjectsRemoved != null)
				ObjectsRemoved(this, EventArgs.Empty);
		}

		public void RemoveRange(int index, int count)
		{
			for (int i = index; i < index + count; ++i)
				RemoveAt(i);

			if (ObjectsRemoved != null)
				ObjectsRemoved(this, EventArgs.Empty);
		}

		public void RemoveAt(int index)
		{
			T value = this[index];
			_internalList.RemoveAt(index);

			if (ObjectRemoved != null)
				ObjectRemoved(this, new NotifyListEventArgs<T>(value));

			if (ObjectsRemoved != null)
				ObjectsRemoved(this, EventArgs.Empty);
		}

		public int Capacity
		{
			get { return _internalList.Capacity; }
		}

		public int Count
		{
			get { return _internalList.Count; }
		}

		public int Add(T value)
		{
			_internalList.Add(value);

			if (ObjectAdded != null)
				ObjectAdded(this, new NotifyListEventArgs<T>(value));

			if (ObjectsAdded != null)
				ObjectsAdded(this, EventArgs.Empty);

			return Count - 1;
		}

		public void AddRange(IEnumerable<T> collection)
		{
			_internalList.AddRange(collection);

			if (ObjectsAdded != null)
				ObjectsAdded(this, EventArgs.Empty);
		}

		public void Clear()
		{
			_internalList.Clear();
		}

		public bool Contains(T value)
		{
			return _internalList.Contains(value);
		}

		public void CopyTo(Array array, int startIndex)
		{
			_internalList.CopyTo((T[])array, startIndex);
		}

		public void CopyTo(T[] array, int startIndex)
		{
			_internalList.CopyTo(array, startIndex);
		}

		public void CopyTo(T[] array)
		{
			_internalList.CopyTo(array);
		}

		public T[] ToArray()
		{
			return _internalList.ToArray();
		}
	}

	public class NotifyListEventArgs<T> : EventArgs
	{
		public T Value
		{
			get;
			set;
		}

		public NotifyListEventArgs(T value)
		{
			Value = value;
		}
	}

	public delegate void NotifyListEventHandler<T> (object sender, NotifyListEventArgs<T> e);
}
