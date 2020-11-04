using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnblockMe.PriorityQue;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace UnblockMe
{
    public class Solver
    {
        public double[,] board;
        public List<Element> boardBlocks = new List<Element>();

        private MinQue<Node> Fringe = new MinQue<Node>();

        private List<double[,]> VisitedConfigurations = new List<double[,]>();

        public Solver(List<Element> initBoard)
        {
            GenerateBoard(6, 6);

            ConfigureBoard(initBoard);
        }

        public List<Node> Solve()
        {
            //From a specific board_Configuration --> Generate all the reachable board configuration from that point
            var initialNode = new Node(board, boardBlocks, null);
            Fringe.Insert(initialNode);

            Console.WriteLine("Inserted initialNode");
            while(Fringe.Size > 0)
            {
                var node = Fringe.ExtractMin();
                //PrintBoard(node.BoardConfiguration);
                //Check if we have reached goal state and if so stop the while loop and return a list of all steps required.
                if(node.IsDesiredGoalState())
                {
                    //Get the parents of the node.
                    return node.GetPath();
                }

                Fringe.Insert(ExpandNode(node));
                Console.WriteLine($"Inserting into the fringe. Size:{Fringe.Size}");
            }

            return null;
        }

        private List<Node> ExpandNode(Node node)
        {
            var childrens = new List<Node>();
            var possibleConfigurations = new Dictionary<int, PossibleMoves>();

            foreach (var element in node.BoardBlocks)
            {
                var coordinateList = element.GetCoordinateList();
                var startCoor = coordinateList.FirstOrDefault();
                var endCoor = coordinateList.Last();

                var list = new List<double>();
                var isMovable = false;

                var possibleNegativeMoves = 0;
                var possiblePositiveMoves = 0;

                //Figure if the element is encapsulated or is up against a wall.
                if (element.Orientation.Equals(Orientation.Horizontal))
                {
                    var bCol = startCoor.StartCol - 1;
                    //Scan backwards from the startCoordinate
                    while(bCol >= 0 && node.BoardConfiguration[startCoor.StartRow, bCol] == 0)
                    {
                        list.Add(node.BoardConfiguration[startCoor.StartRow, bCol]);
                        bCol--;
                        isMovable = true;
                        possibleNegativeMoves++;
                    }
                    //Add the element itself to the list.
                    for(int i = 0; i < element.Size; i++)
                    {
                        list.Add(element.BoardId);
                    }

                    var fCol = endCoor.StartCol + 1;
                    //Scan forwards from the end coordinate 
                    while(fCol < 6 && node.BoardConfiguration[startCoor.StartRow, fCol] == 0)
                    {
                        list.Add(node.BoardConfiguration[startCoor.StartRow, fCol]);
                        fCol++;
                        isMovable = true;
                        possiblePositiveMoves++;
                    }

                }
                if(element.Orientation.Equals(Orientation.Vertical))
                {
                    var bRow = startCoor.StartRow - 1;
                    while(bRow >= 0 && node.BoardConfiguration[bRow, startCoor.StartCol] == 0)
                    {
                        list.Add(node.BoardConfiguration[bRow, startCoor.StartCol]);
                        bRow--;
                        isMovable = true;
                        possibleNegativeMoves++;
                    }

                    //Add the element itself to the list
                    for(int i = 0; i < element.Size; i++)
                    {
                        list.Add(element.BoardId);
                    }

                    var fRow = endCoor.StartRow + 1;
                    while (fRow < 6 && node.BoardConfiguration[fRow, startCoor.StartCol] == 0)
                    {
                        list.Add(node.BoardConfiguration[fRow, startCoor.StartCol]);
                        fRow++;
                        isMovable = true;
                        possiblePositiveMoves++;
                    }
                }

                if(isMovable)
                {
                    possibleConfigurations.Add(element.BoardId, new PossibleMoves(possiblePositiveMoves, possibleNegativeMoves));
                }
            }

            foreach(var config in possibleConfigurations)
            {
                //Generate a board configuration based on the possible moves of the respective element
                for(int i = 1; i <= config.Value.NegativeMoves; i++)
                {
                    //Copy the board and board block
                    var copyBoard = Copy(node.BoardConfiguration);
                    var copyBlocks = Copy(node.BoardBlocks);

                    var block = copyBlocks.Where(x => x.BoardId.Equals(config.Key)).SingleOrDefault();

                    Move(copyBoard, block.GetCoordinateList().ToArray(), block.Orientation, false, i, block);

                    childrens.Add(new Node(copyBoard, copyBlocks, node, node.TotalCost() + 1));
                }
                for(int i = 1; i <= config.Value.PositiveMoves; i++)
                {
                    //Copy the board and board block
                    var copyBoard = Copy(node.BoardConfiguration);
                    var copyBlocks = Copy(node.BoardBlocks);

                    var block = copyBlocks.Where(x => x.BoardId.Equals(config.Key)).SingleOrDefault();

                    Move(copyBoard, block.GetCoordinateList().ToArray(), block.Orientation, true, i, block);

                    childrens.Add(new Node(copyBoard, copyBlocks, node, node.TotalCost() + 1));
                }
            }
            var finalList = new List<Node>();
            //Walk through each child if the board configuration has been seen before dont include it
            foreach(var child in childrens)
            {
                if(!CheckForDuplicates(child.BoardConfiguration))
                {
                    finalList.Add(child);
                    VisitedConfigurations.Add(Copy(child.BoardConfiguration));
                }
            }

            return finalList;
        }

        private bool CheckForDuplicates(double[,] config)
        {
            var duplicate = true;
            foreach (var prevConfig in VisitedConfigurations)
            {
                duplicate = true;
                for(int i = 0; i < prevConfig.GetUpperBound(0) + 1; i++)
                {
                    for(int j = 0; j < prevConfig.GetUpperBound(1) + 1; j++)
                    {
                        if(config[i,j] != prevConfig[i,j])
                        {
                            duplicate = false;
                            break;
                        }
                    }
                    if (!duplicate)
                        break;
                }
                if (duplicate)
                    break;
            }
            if(VisitedConfigurations.Count == 0)
            {
                return false;
            }
            return duplicate;
        }

        public void Move(double[,] board, ElementCoordinates[] coordinates, Orientation orientation, bool isPostive, int amount, Element element)
        {
            if (orientation.Equals(Orientation.Horizontal))
            {
                if(isPostive)
                {
                    //Do backwards scan - Make a board configuration moving this element on the i to the right
                    for (int i = coordinates.Length - 1; i >= 0; i--)
                    {
                        var coor = coordinates[i];
                        board[coor.StartRow, coor.StartCol + amount] = element.BoardId;
                        board[coor.StartRow, coor.StartCol] = 0;
                    }
                    element.Coordinates = new ElementCoordinates(element.Coordinates.StartRow, element.Coordinates.StartCol + amount);
                }
                else
                {
                    for(int i = 0; i < coordinates.Length; i++)
                    {
                        var coor = coordinates[i];
                        board[coor.StartRow, coor.StartCol - amount] = element.BoardId;
                        board[coor.StartRow, coor.StartCol] = 0;
                    }
                    element.Coordinates = new ElementCoordinates(element.Coordinates.StartRow, element.Coordinates.StartCol - amount);
                }
            }
            else
            {
                if(isPostive)
                {
                    for(int i = coordinates.Length - 1; i >= 0; i--)
                    {
                        var coor = coordinates[i];
                        board[coor.StartRow + amount, coor.StartCol] = element.BoardId;
                        board[coor.StartRow, coor.StartCol] = 0;
                    }
                    element.Coordinates = new ElementCoordinates(element.Coordinates.StartRow + amount, element.Coordinates.StartCol);
                }
                else
                {
                    for(int i = 0; i < coordinates.Length; i++)
                    {
                        var coor = coordinates[i];
                        board[coor.StartRow - amount, coor.StartCol] = element.BoardId;
                        board[coor.StartRow, coor.StartCol] = 0;
                    }
                    element.Coordinates = new ElementCoordinates(element.Coordinates.StartRow - amount, element.Coordinates.StartCol);
                }
            }
        }


        public void ConfigureBoard(List<Element> initBoard)
        {
            foreach(var block in initBoard)
            {
                InsertBlock(block);
            }

            foreach (var element in boardBlocks)
            {
                Console.WriteLine(element.Color + " " + element.Id);
            }
        }

        public void InsertBlock(Element element)
        {
            //Determine if the current board configurations allow insertion of block in at the respective coordinates
            foreach(var coordinate in element.GetCoordinateList())
            {
                if(coordinate.StartCol > 5 || coordinate.StartRow > 5)
                {
                    throw new ArgumentException($"Element is out of bounds");
                }

                if(board[coordinate.StartRow, coordinate.StartCol] != 0)
                {
                    throw new ArgumentException($"Element of type {element.Type} can't be inserted at coordinates --> Row:{element.Coordinates.StartRow}, Col:{element.Coordinates.StartCol}, because another element is already occupying respective coordinates");
                }

                //There are no elements of the board blocking this elements path.
                board[coordinate.StartRow, coordinate.StartCol] = boardBlocks.Count + 1;
            }

            element.BoardId = boardBlocks.Count + 1;

            boardBlocks.Add(element);
        }


        public void GenerateBoard(int rowSize, int colSize)
        {
            board = new double[rowSize, colSize];

            for(int i = 0; i < rowSize; i++)
            {
                for(int j = 0; j < colSize; j++)
                {
                    board[i, j] = 0;
                }
            }
        }

        private double[,] Copy(double[,] element)
        {
            var copy = new double[element.GetUpperBound(0) + 1, element.GetUpperBound(1) + 1];

            for(int i = 0; i < element.GetUpperBound(0) + 1; i++)
            {
                for(int j = 0; j < element.GetUpperBound(1) + 1;j++)
                {
                    copy[i, j] = element[i, j];
                }
            }

            return copy;
        }

        private List<Element> Copy(List<Element> elements)
        {
            return elements.Select(x => new Element(x)).ToList();
        }

        private void PrintBoard(double[,] board)
        {
            var builder = new StringBuilder();

            for(int i = 0; i < board.GetUpperBound(0) + 1; i++)
            {
                builder.Append("\n");
                for(int j = 0; j < board.GetUpperBound(1) + 1; j++)
                {
                    builder.Append($"  {board[i,j]}  ");
                }
                builder.Append("\n");
            }

            Console.WriteLine(builder.ToString());

        }
    }
}

