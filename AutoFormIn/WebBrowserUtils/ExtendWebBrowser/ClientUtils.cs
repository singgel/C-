using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Security;
using System.Threading;

namespace WebBrowserUtils.ExtendWebBrowser
{
    internal static class ClientUtils
    {
        internal class WeakRefCollection : IList, ICollection, IEnumerable
        {
            internal class WeakRefObject
            {
                private int hash;
                private WeakReference weakHolder;
                internal bool IsAlive
                {
                    get
                    {
                        return this.weakHolder.IsAlive;
                    }
                }
                internal object Target
                {
                    get
                    {
                        return this.weakHolder.Target;
                    }
                }
                internal WeakRefObject(object obj)
                {
                    this.weakHolder = new WeakReference(obj);
                    this.hash = obj.GetHashCode();
                }
                public override int GetHashCode()
                {
                    return this.hash;
                }
                public override bool Equals(object obj)
                {
                    ClientUtils.WeakRefCollection.WeakRefObject weakRefObject = obj as ClientUtils.WeakRefCollection.WeakRefObject;
                    return weakRefObject == this || (weakRefObject != null && (weakRefObject.Target == this.Target || (this.Target != null && this.Target.Equals(weakRefObject.Target))));
                }
            }
            private int refCheckThreshold = 2147483647;
            private ArrayList _innerList;
            internal ArrayList InnerList
            {
                get
                {
                    return this._innerList;
                }
            }
            public int RefCheckThreshold
            {
                get
                {
                    return this.refCheckThreshold;
                }
                set
                {
                    this.refCheckThreshold = value;
                }
            }
            public object this[int index]
            {
                get
                {
                    ClientUtils.WeakRefCollection.WeakRefObject weakRefObject = this.InnerList[index] as ClientUtils.WeakRefCollection.WeakRefObject;
                    if (weakRefObject != null && weakRefObject.IsAlive)
                    {
                        return weakRefObject.Target;
                    }
                    return null;
                }
                set
                {
                    this.InnerList[index] = this.CreateWeakRefObject(value);
                }
            }
            public bool IsFixedSize
            {
                get
                {
                    return this.InnerList.IsFixedSize;
                }
            }
            public int Count
            {
                get
                {
                    return this.InnerList.Count;
                }
            }
            object ICollection.SyncRoot
            {
                get
                {
                    return this.InnerList.SyncRoot;
                }
            }
            public bool IsReadOnly
            {
                get
                {
                    return this.InnerList.IsReadOnly;
                }
            }
            bool ICollection.IsSynchronized
            {
                get
                {
                    return this.InnerList.IsSynchronized;
                }
            }
            public void ScavengeReferences()
            {
                int num = 0;
                int count = this.Count;
                for (int i = 0; i < count; i++)
                {
                    if (this[num] == null)
                    {
                        this.InnerList.RemoveAt(num);
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            public override bool Equals(object obj)
            {
                ClientUtils.WeakRefCollection weakRefCollection = obj as ClientUtils.WeakRefCollection;
                if (weakRefCollection == this)
                {
                    return true;
                }
                if (weakRefCollection == null || this.Count != weakRefCollection.Count)
                {
                    return false;
                }
                for (int i = 0; i < this.Count; i++)
                {
                    if (this.InnerList[i] != weakRefCollection.InnerList[i] && (this.InnerList[i] == null || !this.InnerList[i].Equals(weakRefCollection.InnerList[i])))
                    {
                        return false;
                    }
                }
                return true;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public void RemoveByHashCode(object value)
            {
                if (value == null)
                {
                    return;
                }
                int hashCode = value.GetHashCode();
                for (int i = 0; i < this.InnerList.Count; i++)
                {
                    if (this.InnerList[i] != null && this.InnerList[i].GetHashCode() == hashCode)
                    {
                        this.RemoveAt(i);
                        return;
                    }
                }
            }
            public void Clear()
            {
                this.InnerList.Clear();
            }
            public bool Contains(object value)
            {
                return this.InnerList.Contains(this.CreateWeakRefObject(value));
            }
            public void RemoveAt(int index)
            {
                this.InnerList.RemoveAt(index);
            }
            public void Remove(object value)
            {
                this.InnerList.Remove(this.CreateWeakRefObject(value));
            }
            public int IndexOf(object value)
            {
                return this.InnerList.IndexOf(this.CreateWeakRefObject(value));
            }
            public void Insert(int index, object value)
            {
                this.InnerList.Insert(index, this.CreateWeakRefObject(value));
            }
            public int Add(object value)
            {
                if (this.Count > this.RefCheckThreshold)
                {
                    this.ScavengeReferences();
                }
                return this.InnerList.Add(this.CreateWeakRefObject(value));
            }
            public void CopyTo(Array array, int index)
            {
                this.InnerList.CopyTo(array, index);
            }
            public IEnumerator GetEnumerator()
            {
                return this.InnerList.GetEnumerator();
            }
            internal WeakRefCollection()
            {
                this._innerList = new ArrayList(4);
            }
            internal WeakRefCollection(int size)
            {
                this._innerList = new ArrayList(size);
            }
            private ClientUtils.WeakRefCollection.WeakRefObject CreateWeakRefObject(object value)
            {
                if (value == null)
                {
                    return null;
                }
                return new ClientUtils.WeakRefCollection.WeakRefObject(value);
            }
            private static void Copy(ClientUtils.WeakRefCollection sourceList, int sourceIndex, ClientUtils.WeakRefCollection destinationList, int destinationIndex, int length)
            {
                if (sourceIndex < destinationIndex)
                {
                    sourceIndex += length;
                    destinationIndex += length;
                    while (length > 0)
                    {
                        destinationList.InnerList[--destinationIndex] = sourceList.InnerList[--sourceIndex];
                        length--;
                    }
                    return;
                }
                while (length > 0)
                {
                    destinationList.InnerList[destinationIndex++] = sourceList.InnerList[sourceIndex++];
                    length--;
                }
            }
        }
        public static bool IsCriticalException(Exception ex)
        {
            return ex is NullReferenceException || ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException || ex is ExecutionEngineException || ex is IndexOutOfRangeException || ex is AccessViolationException;
        }
        public static bool IsSecurityOrCriticalException(Exception ex)
        {
            return ex is SecurityException || ClientUtils.IsCriticalException(ex);
        }
        public static int GetBitCount(uint x)
        {
            int num = 0;
            while (x > 0u)
            {
                x &= x - 1u;
                num++;
            }
            return num;
        }
        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue)
        {
            return value >= minValue && value <= maxValue;
        }
        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue, int maxNumberOfBitsOn)
        {
            bool flag = value >= minValue && value <= maxValue;
            return flag && ClientUtils.GetBitCount((uint)value) <= maxNumberOfBitsOn;
        }
        public static bool IsEnumValid_Masked(Enum enumValue, int value, uint mask)
        {
            return ((long)value & (long)((ulong)mask)) == (long)value;
        }
        public static bool IsEnumValid_NotSequential(Enum enumValue, int value, params int[] enumValues)
        {
            for (int i = 0; i < enumValues.Length; i++)
            {
                if (enumValues[i] == value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
