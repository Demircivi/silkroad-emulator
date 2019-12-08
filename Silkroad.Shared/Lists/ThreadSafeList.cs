using System;
using System.Collections.Generic;
using System.Linq;

namespace Silkroad.Shared.Lists
{
    public class ThreadSafeList<TItemType> where TItemType : class
    {
        private readonly List<TItemType> _list;

        public ThreadSafeList()
        {
            _list = new List<TItemType>();
        }

        public void Add(TItemType item)
        {
            lock (_list)
            {
                _list.Add(item);
            }
        }

        public void Remove(TItemType item)
        {
            lock (_list)
            {
                _list.Remove(item);
            }
        }

        public TItemType Find(Func<TItemType, bool> predicate)
        {
            lock (_list)
            {
                return _list.FirstOrDefault(predicate);
            }
        }
    }
}
