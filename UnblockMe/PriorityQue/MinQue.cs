using System;
using System.Collections.Generic;

namespace UnblockMe.PriorityQue
{
    public class MinQue<T> where T : class, MinQueComparable<T> 
    {
        private T[] _heap;
        private int _heapSize = 0; 

        public MinQue()
        {
            _heap = new T[10];
        }

        public int Size
        {
            get
            {
                return _heapSize;
            }
        }

        /// <summary>
        /// Insert the specified element into the priorityQue and run the min-heapify procedure.
        /// 
        /// Running-Time --> O(log*n) 
        /// 
        /// </summary>
        /// <param name="e">E.</param>
        public void Insert(T e)
        {
            //Insert
            int i = _heapSize;

            //Insert e into the last index;
            _heap[i] = e;

            //As long as the index is above the root, namely 0, and the parent's key at our respective index is lower than our key, then swap child with parent
            while (i > 0 && _heap[Parent(i)].Compare(_heap[i]))
            {
                T temp = _heap[Parent(i)];
                _heap[Parent(i)] = _heap[i];
                _heap[i] = temp;

                //Set the index of interest to the index of the parent we just swapped
                i = Parent(i);
            }

            //Increment heapSize
            _heapSize++;

            if(_heap.Length / 2 < _heapSize)
            {
                //Increase Size!
                IncreaseSize();
            }
            else if (_heap.Length / 3 > _heapSize && _heap.Length > 11)
            {
                //DecreaseSize
                DecreaseSize();
            }
        }

        public void Insert(IEnumerable<T> e)
        {
            foreach(var t in e)
            {
                Insert(t);
            }
        }

        /// <summary>
        /// Extracts the minimum.
        /// 
        /// Running-time --> O(log*n), meaning it is proportional to the height of the heap-tree
        /// 
        /// </summary>
        /// <returns>The minimum.</returns>
        public T ExtractMin()
        {
            //Check if heapSize is below 1, if so throw a HeapUnderflowException();
            if(_heapSize < 1)
            {
                return null;
            }

            T first = _heap[0];

            //Swap the element in the last index to the front of the array or to the front of the heap
            _heap[0] = _heap[_heapSize - 1];

            //Decrement heapSize after extraction
            _heapSize--;

            MinHeapify(0);

            return first;
        }

        /// <summary>
        /// Performs a min heapify on the heap datastructure to repair the heap-order from the respective index
        /// 
        /// Running time --> O(log_2(n)), Meaning that it is proportional to the height of the heap-tree
        /// 
        /// </summary>
        /// <param name="i">The index.</param>
        private void MinHeapify(int i)
        {
            //Get index of left and right child respective to i
            int lIndex = Left(i);
            int rIndex = Right(i);

            int smallestIndex = i;

            //As long as lIndex is greater than the size of the heap and
            //lChild is less than itøs parent make lChild smallestIndex else make parent smallestIndex
            if(lIndex <= _heapSize -1 && !_heap[lIndex].Compare(_heap[i]))
            {
                smallestIndex = lIndex;
            }

            //Do the same challenge on the rightIndex
            if(rIndex <= _heapSize - 1 && !_heap[rIndex].Compare(_heap[smallestIndex]))
            {
                smallestIndex = rIndex;
            }

            //If the smallest is not the parent swap parent and the smallest child
            if(i != smallestIndex)
            {
                T temp = _heap[i];
                _heap[i] = _heap[smallestIndex];
                _heap[smallestIndex] = temp;

                //Since we made a swap we might have messed up the heap structure again.
                //Thus we call Min-Heapify yet again to ensure that the new parent is also in heap order
                MinHeapify(smallestIndex);
            }
        }

        /// <summary>
        /// Returns the respective parent index of i.
        /// </summary>
        /// <returns>The parent.</returns>
        /// <param name="index">Index.</param>
        private int Parent(int index) { return (index - 1) / 2; }

        /// <summary>
        /// Return the respective leftChild index of this parent
        /// </summary>
        /// <returns>The left.</returns>
        /// <param name="parentIndex">Parent index.</param>
        private int Left(int parentIndex) { return parentIndex == 0 ? 1 : parentIndex * 2; }

        /// <summary>
        /// Return the respective rightChild index of this parent
        /// </summary>
        /// <returns>The right.</returns>
        /// <param name="parentIndex">Parent index.</param>
        private int Right(int parentIndex) { return parentIndex == 0 ? 2 : parentIndex * 2 + 1; }

        /// <summary>
        /// Increases the size of the heap.
        /// 
        /// Running time --> O(n)
        /// 
        /// </summary>
        private void IncreaseSize()
        {
            var prevHeap = _heap;

            _heap = new T[prevHeap.Length * 2];

            for(int i = 0; i < prevHeap.Length; i++)
            {
                _heap[i] = prevHeap[i];
            }
        }

        /// <summary>
        /// Decreases the size.
        /// 
        /// Running time --> O(n)
        /// 
        /// </summary>
        private void DecreaseSize()
        {
            var prevHeap = _heap;

            _heap = new T[prevHeap.Length / 2];

            for(int i = 0; i < _heap.Length; i++)
            {
                _heap[i] = prevHeap[i];
            }
        }
    }
}
