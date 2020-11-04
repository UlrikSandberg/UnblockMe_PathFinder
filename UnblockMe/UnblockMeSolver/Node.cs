using System;
using System.Linq;
using System.Collections.Generic;
using UnblockMe.PriorityQue;

namespace UnblockMe
{
    public class Node : MinQueComparable<Node>
    {
        //The state of this particular node
        public Node Parent { get; }
        public double[,] BoardConfiguration { get; }
        public List<Element> BoardBlocks { get; }

        public double PathCost { get; set; }
        public double HeuristicCost { get; set; }

        public Node(double[,] boardConfiguration, List<Element> boardBlocks, Node parent, double pathCost = 0)
        {
            BoardConfiguration = boardConfiguration;
            BoardBlocks = new List<Element>(boardBlocks);
            Parent = parent;

            PathCost = pathCost;

            //Calculate the heuristic cost
            HeuristicCost = CalculateHeuristics();

        }

        public double TotalCost()
        {
            return PathCost + HeuristicCost;
        }

        public bool Compare(Node compareTo)
        {
            if(TotalCost() > compareTo.TotalCost())
            {
                return true;
            }

            return false;
        }

        public bool IsDesiredGoalState()
        {
            //Calculate the heuristic cost, this can be done
            var winnerBlock = BoardBlocks.Where(x => x.Type.Equals(ElementType.WinnerBlock)).SingleOrDefault();

            var lastCoordinate = winnerBlock.GetCoordinateList().Last();

            for (int i = lastCoordinate.StartCol + 1; i < 6; i++)
            {
                if (BoardConfiguration[lastCoordinate.StartRow, i] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public List<Node> GetPath()
        {
            var list = new List<Node> { this };

            var parent = Parent;

            while(parent != null)
            {
                list.Add(parent);

                parent = parent.Parent != null ? parent.Parent : null;
            }

            return list;
        }

        private double CalculateHeuristics()
        {
            var heuristicCost = 0;
            //Calculate the heuristic cost, this can be done
            var winnerBlock = BoardBlocks.Where(x => x.Type.Equals(ElementType.WinnerBlock)).SingleOrDefault();

            var lastCoordinate = winnerBlock.GetCoordinateList().Last();

            for(int i = lastCoordinate.StartCol + 1; i < 6; i++)
            {
                if(BoardConfiguration[lastCoordinate.StartRow, i] != 0)
                {
                    heuristicCost++;
                }
            }

            return 0;//heuristicCost;
        }
    }
}
