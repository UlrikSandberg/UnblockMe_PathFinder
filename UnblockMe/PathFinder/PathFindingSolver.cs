using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnblockMe.PriorityQue;
using Xamarin.Forms;

namespace UnblockMe.PathFinder
{
    public class PathFindingSolver
    {
        public MinQue<Node> Fringe = new MinQue<Node>();
        public List<TileModel> VisitedNodes = new List<TileModel>();

        public TileModel[,] TileMap;

        public TileModel Start;

        public TileModel Goal;

        public double CustomHeuristicValue { get; }

        public bool ShowExploration { get; }

        public int AnimationSpeed { get; }

        public PathFindingSolver(TileModel[,] tileMap, TileModel start, TileModel goal, double customHeuristicValue, int animationSpeed, bool showExploration = false)
        {
            TileMap = tileMap;
            Start = start;
            Goal = goal;
            CustomHeuristicValue = customHeuristicValue;
            ShowExploration = showExploration;
            AnimationSpeed = animationSpeed;
        }

        public async Task<List<Node>> Solve()
        {
            Fringe.Insert(new Node(Heuristics(Start, Goal), 0, null, Start));

            while(Fringe.Size > 0)
            {
                var node = Fringe.ExtractMin();

                if(ValidateGoal(node))
                {
                    return node.GetPath();
                }

                if(ShowExploration)
                {
                    if (!node.State.Occupant.Equals(MapObjects.Start))
                    {
                        await Task.Delay(AnimationSpeed);
                        node.State.Color = Color.Blue;
                        node.State.Occupant = MapObjects.Path;
                    }
                }

                VisitedNodes.Add(node.State);
                Fringe.Insert(ExpandNode(node));
                Console.WriteLine($"Inserting intro Fringe. Size:{Fringe.Size}");
            }

            return null;
        }

        public List<Node> ExpandNode(Node node)
        {
            var nodes = new List<Node>();

            var tile = node.State;

            //Travers all possible 9 nearby tiles and expand their nodes.
            for(int r = tile.GridRow - 1; r <= tile.GridRow + 1; r++)
            {
                if (r < 0 || r > 12)
                    continue;

                for(int c = tile.GridCol - 1; c <= tile.GridCol + 1; c++)
                {
                    if (c < 0 || c > 11)
                        continue;

                    var adjacentTile = TileMap[r, c];
                    if(adjacentTile != tile && !VisitedNodes.Contains(adjacentTile))//Mean it is not the node we are expanding
                    {
                        if(!adjacentTile.Occupant.Equals(MapObjects.Obstacles))
                        {
                            nodes.Add(new Node(Heuristics(adjacentTile, Goal), node.TotalCost + 1, node, adjacentTile));
                        }
                    }
                }
            }

            return nodes;
        }

        public double Heuristics(TileModel from, TileModel to)
        {
            //We will calculate the vector length of the two respective coordinates
            var xLength = to.GridCol - from.GridCol;
            var yLength = to.GridRow - from.GridRow;

            var vectorLength = Math.Pow(Math.Abs(Math.Sqrt(Math.Pow(xLength, 2) + Math.Pow(yLength, 2))), CustomHeuristicValue);

            return vectorLength;
        }

        public bool ValidateGoal(Node node)
        {
            if(node.State == Goal)
            {
                return true;
            }
            return false;
        }
    }
}
