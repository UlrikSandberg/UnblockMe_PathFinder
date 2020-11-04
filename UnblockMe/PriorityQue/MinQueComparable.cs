using System;
namespace UnblockMe.PriorityQue
{
    public interface MinQueComparable<T>
    {
        /// <summary>
        /// If the implementing element is less than compareTo argument then return false else return true
        /// </summary>
        /// <returns>The compare.</returns>
        /// <param name="compareTo">Compare to.</param>
        bool Compare(T compareTo);
    }
}
