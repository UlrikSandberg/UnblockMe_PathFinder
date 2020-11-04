using System;
using System.Linq;
using System.Collections.Generic;
using UnblockMe.PriorityQue;

namespace UnblockMe.PathFinder
{
    public class Node : MinQueComparable<Node>
    {
        public Node Parent { get; set; }
        public TileModel State { get; set; }

        public double HeuristicCost { get; }
        public double PathCost { get; }
        public double TotalCost
        {
            get
            {
                return HeuristicCost + PathCost;
            }
        }

        public Node(double heuristicCost, double pathCost, Node parent, TileModel state)
        {
            HeuristicCost = heuristicCost;
            PathCost = pathCost;
            Parent = parent;
            State = state;
        }

        public bool Compare(Node compareTo)
        {
            if(TotalCost > compareTo.TotalCost)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Node> GetPath()
        {
            var path = new List<Node>();

            path.Add(this);

            var parent = Parent;

            while(parent != null)
            {
                path.Add(parent);
                parent = parent.Parent;
            }

            //Reverse it so the result is in order respective to the execution
            var pathArr = path.ToArray();
            var arr = new Node[pathArr.Length];

            for(int i = 0; i < pathArr.Length; i++)
            {
                arr[i] = pathArr[pathArr.Length - 1 - i];
            }
            return arr.ToList();
        }
    }
}
