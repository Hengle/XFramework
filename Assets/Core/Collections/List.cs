﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace XDEDZL.Collections
{
    public class List<T> : IList<T>
    {
        ListNode<T> firstNode;
        ListNode<T> lastNode;
        private int count = 0;

        public T this[int index]
        {
            get
            {
                if(count == 0)
                {
                    throw new Exception("List has no node.");
                }
                else if(index > count - 1)
                {
                    throw new Exception("Index was outside the bounds of the List.");
                }
                else
                {
                    ListNode<T> a = firstNode;
                    for (int i = 0; i < index; i++)
                    {
                        a = a.next;
                    }
                    return a.value;
                }
            }
            set
            {
                if (count == 0)
                {
                    throw new Exception("List has no node.");
                }
                else if (index > count - 1)
                {
                    throw new Exception("Index was outside the bounds of the List.");
                }
                else
                {
                    ListNode<T> a = firstNode;
                    for (int i = 0; i < index; i++)
                    {
                        a = a.next;
                    }
                    a.value = value;
                }
            }
        }

        public int Count { get { return count; } }

        public bool IsReadOnly { get; }

        public void Add(T item)
        {
            ListNode<T> node = new ListNode<T>();
            node.value = item;
            if (firstNode == null)
                firstNode = node;
            else
                lastNode.next = node;

            lastNode = node;
            count++;
        }

        public void Clear()
        {
            firstNode = null;
            lastNode = null;
            count = 0;
        }

        public bool Contains(T item)
        {
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class ListNode<T>
    {
        public ListNode<T> next;
        public T value;
    }
}