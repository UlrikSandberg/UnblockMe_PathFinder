using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using UnblockMe.UnblockMeSolver;

namespace UnblockMe
{
    public partial class Board : ContentPage
    {
        public bool FindingSolution { get; set; } = false;
        private Solver boardEngine;

        private ElementType ChosenElementType = ElementType.WinnerBlock;

        private List<UnblockMeTileModel> CurrentlyChosen { get; set; } = new List<UnblockMeTileModel>();

        private Dictionary<int, Element> LockedElements { get; set; } = new Dictionary<int, Element>();


        public Board()
        {
            InitializeComponent();

            boardEngine = new Solver(new List<Element>());

            //DrawBoard();
            for(int i = 0; i < 6; i++)
            {
                for(int j = 0; j < 6; j++)
                {
                    var model = new UnblockMeTileModel
                    {
                        GridCol = i,
                        GridRow = j
                    };

                    var tile = new UnblockMeTIle();
                    tile.BindingContext = model;
                    tile.OnTileClicked += Tile_OnTileClicked;

                    tile.SetBinding(UnblockMeTIle.TileColorProperty, new Binding(nameof(UnblockMeTileModel.Color), source: model));

                    UnblockMeContainer.Children.Add(tile, i, j);
                }
            }
        }

        public void Tile_OnTileClicked(object sender, UnblockMeTileModel model)
        {



        }

        void Handle_ClickedEasy(object sender, System.EventArgs e)
        {
            if(!FindingSolution)
            {
                UnblockMeContainer.Children.Clear();
                Moves.Text = "Moves: 0";
                var list = new List<Element>
                {
                    new Element(ElementType.WinnerBlock, 2, 0, Orientation.Horizontal),
                    new Element(ElementType.Small, 0, 1, Orientation.Horizontal),
                    new Element(ElementType.Small, 1, 1, Orientation.Horizontal),
                    new Element(ElementType.Small, 1, 3, Orientation.Horizontal),
                    new Element(ElementType.Small, 2, 2, Orientation.Vertical),
                    new Element(ElementType.Small, 2, 3, Orientation.Vertical),
                    new Element(ElementType.Small, 3, 4, Orientation.Horizontal),
                    new Element(ElementType.Small, 4, 2, Orientation.Horizontal),
                    new Element(ElementType.Small, 4, 4, Orientation.Horizontal),
                    new Element(ElementType.Big, 3, 1, Orientation.Vertical),
                    new Element(ElementType.Big, 5, 2, Orientation.Horizontal)
                };

                boardEngine = new Solver(list);
                DrawBoard();
            }
        }

        void Handle_ClickedMedium(object sender, System.EventArgs e)
        {
            if(!FindingSolution)
            {
                UnblockMeContainer.Children.Clear();
                Moves.Text = "Moves:";
                var list = new List<Element>
                {
                    new Element(ElementType.WinnerBlock, 2, 1, Orientation.Horizontal),
                    new Element(ElementType.Big, 0, 0, Orientation.Vertical),
                    new Element(ElementType.Small, 0, 2, Orientation.Vertical),
                    new Element(ElementType.Big, 0, 3, Orientation.Horizontal),
                    new Element(ElementType.Small, 1, 3, Orientation.Vertical),
                    new Element(ElementType.Small, 3, 0, Orientation.Horizontal),
                    new Element(ElementType.Small, 3, 2, Orientation.Horizontal),
                    new Element(ElementType.Small, 4, 0, Orientation.Horizontal),
                    new Element(ElementType.Small, 4, 2, Orientation.Horizontal),
                    new Element(ElementType.Big, 2, 5, Orientation.Vertical),
                };

                boardEngine = new Solver(list);
                DrawBoard();
            }
        }

        void Handle_ClickedExpert(object sender, System.EventArgs e)
        {
            if (!FindingSolution)
            {
                UnblockMeContainer.Children.Clear();
                Moves.Text = "Moves:";
                var list = new List<Element>
                {
                    new Element(ElementType.WinnerBlock, 2, 0, Orientation.Horizontal),
                    new Element(ElementType.Small, 0, 1, Orientation.Vertical),
                    new Element(ElementType.Small, 0, 2, Orientation.Horizontal),
                    new Element(ElementType.Small, 0, 4, Orientation.Horizontal),
                    new Element(ElementType.Small, 1, 3, Orientation.Vertical),
                    new Element(ElementType.Small, 1, 4, Orientation.Horizontal),

                    new Element(ElementType.Big, 3, 0, Orientation.Vertical),
                    new Element(ElementType.Big, 3, 1, Orientation.Horizontal),

                    new Element(ElementType.Small, 5, 3, Orientation.Horizontal),
                    new Element(ElementType.Small, 4, 2, Orientation.Vertical),

                    new Element(ElementType.Big, 2, 4, Orientation.Vertical),
                    new Element(ElementType.Big, 3, 5, Orientation.Vertical),
                };

                boardEngine = new Solver(list);
                DrawBoard();
            }
        }

        public void DrawBoard()
        {
            int index = 0;

            foreach(var element in boardEngine.boardBlocks)
            {
                Console.WriteLine($"Add element {index} With color R:{element.Color.R}, B:{element.Color.B}, G:{element.Color.G}");

                foreach(var eleCoor in element.GetCoordinateList())
                {
                    var frame = new Frame
                    {
                        CornerRadius = 0,
                        BackgroundColor = element.Color,
                        Padding = new Thickness(0,0,0,0),
                        HasShadow = false,
                        Content = new Label
                        {
                            Text = element.BoardId.ToString(),
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                    };
                    UnblockMeContainer.Children.Add(frame, eleCoor.StartCol, eleCoor.StartRow);
                }
                index++;
            }
        }

        public async Task DrawSolution(List<Node> solutions)
        {
            var moves = 0;
            foreach(var node in solutions)
            {
                UnblockMeContainer.Children.Clear();
                int index = 0;

                foreach (var element in node.BoardBlocks)
                {
                    foreach (var eleCoor in element.GetCoordinateList())
                    {
                        var frame = new Frame
                        {
                            CornerRadius = 0,
                            BackgroundColor = element.Color,
                            Padding = new Thickness(0, 0, 0, 0),
                            HasShadow = false,
                            Content = new Label
                            {
                                Text = element.BoardId.ToString(),
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center
                            }
                        };
                        UnblockMeContainer.Children.Add(frame, eleCoor.StartCol, eleCoor.StartRow);
                    }
                    index++;

                }
                Moves.Text = $"Moves: {moves}";
                moves++;
                await Task.Delay(500);
            }
            Activity.IsVisible = false;
            Activity.IsRunning = false;
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Activity.IsVisible = true;
            Activity.IsRunning = true;
            StartSolution();
        }

        private void StartSolution()
        {
            var solution1 = boardEngine.Solve();

            var solution = new Node[0];

            if(solution1 != null)
            {
                solution = solution1.ToArray();
            }
            else
            {
                Console.WriteLine("No solution!");
                return;
            }

            Console.WriteLine("Animate each board state in respective order to show result");

            var reversedList = new Node[solution.Length];

            for(int i = solution.Length -1; i >= 0; i--)
            {
                reversedList[solution.Length - 1 - i] = solution[i];
            }


            DrawSolution(reversedList.ToList());
        }

        void Handle_ClickedSmall(object sender, System.EventArgs e)
        {
            Small.BackgroundColor = Color.Black;
            Big.BackgroundColor = Color.White;
            ChosenElementType = ElementType.Small;
        }

        void Handle_ClickedBig(object sender, System.EventArgs e)
        {
            Small.BackgroundColor = Color.White;
            Big.BackgroundColor = Color.Black;
            ChosenElementType = ElementType.Big;
        }
    }
}