using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UnblockMe.PathFinder
{
    public partial class PathFindingBoard : ContentPage
    {
        public MapObjects SelectedEditorItem = MapObjects.None;

        public List<TileModel> Tiles { get; set; } = new List<TileModel>();
        public TileModel[,] TileMap { get; set; }

        public TileModel GoalTile;
        public TileModel StartTile;

        public double AnimationValue { get; set; } = 1.0;
        public double HeuristicValue { get; set; } = 1.0;
        public bool IsAnimationExploratory { get; set; } = false;

        private bool _isSolving = false;
        public bool IsSolving
        {
            get
            {
                return _isSolving;
            }
            set
            {
                _isSolving = value;
                if(_isSolving)
                {
                    HeuristicSection.IsVisible = false;
                    MapObjectSection.IsVisible = false;
                    AnimationTimeSection.IsVisible = false;
                    Solver.IsVisible = true;
                    Solver.IsRunning = true;
                }
                else
                {
                    HeuristicSection.IsVisible = true;
                    MapObjectSection.IsVisible = true;
                    AnimationTimeSection.IsVisible = true;
                    Solver.IsVisible = false;
                    Solver.IsRunning = false;
                }
            }
        }

        public PathFindingBoard()
        {
            InitializeComponent();

            DrawBoard();
        }

        private void DrawBoard()
        {
            //Create Tiles
            TileMap = new TileModel[13, 12];
            for(int i = 0; i < 12; i++)
            {
                for(int j = 0; j < 13; j++)
                {
                    var model = new TileModel
                    {
                        GridCol = i,
                        GridRow = j
                    };
                    Tiles.Add(model);

                    var tile = new Tile();
                    tile.BindingContext = model;
                    tile.OnTileClicked += Tile_OnTileClicked;

                    tile.SetBinding(Tile.TileColorProperty, new Binding(nameof(TileModel.Color), source: model));

                    Container.Children.Add(tile, i, j);
                    TileMap[j, i] = model;
                }
            }
        }

        public async Task Solve()
        {
            if(GoalTile == null || StartTile == null)
            {
                this.DisplayAlert("Sorry", "Can't navigate without a Start and Goal Point", "OK");
                return;
            }
            IsSolving = true;
            foreach(var tile in Tiles)
            {
                if(tile.Occupant.Equals(MapObjects.Path))
                {
                    tile.Color = Color.White;
                    tile.Occupant = MapObjects.None;
                }
            }

            var pathFindingSolver = new PathFindingSolver(TileMap, StartTile, GoalTile, HeuristicValue, (int)AnimationValue, IsAnimationExploratory);
            var path = await pathFindingSolver.Solve();

            if(path != null)
            {
                if(!IsAnimationExploratory)
                {
                    foreach(var node in path)
                    {
                        if(!node.State.Occupant.Equals(MapObjects.Goal) && !node.State.Occupant.Equals(MapObjects.Start))
                        {
                            node.State.Color = Color.Blue;
                            node.State.Occupant = MapObjects.Path;
                            await Task.Delay(100);
                        }
                    }
                }
                IsSolving = false;
            }
            else
            {
                IsSolving = false;
                this.DisplayAlert("Sorry", "No route could be found", "OK");
            }
        }

        void Tile_OnTileClicked(object sender, TileModel binding)
        {
            switch(SelectedEditorItem)
            {
                case MapObjects.None:
                    break;
                case MapObjects.Goal:
                    if(binding.Occupant.Equals(MapObjects.Goal))
                    {
                        binding.Occupant = MapObjects.None;
                        binding.Color = Color.White;
                        GoalTile = null;
                    }
                    else
                    {
                        binding.Occupant = MapObjects.Goal;
                        binding.Color = Color.Green;
                        if (GoalTile != null)
                        {
                            GoalTile.Color = Color.White;
                            GoalTile.Occupant = MapObjects.None;
                        }
                        GoalTile = binding;
                    }
                    break;
                case MapObjects.Obstacles:
                    if(binding.Occupant.Equals(MapObjects.Goal))
                    {
                        GoalTile = null;
                    }

                    if(binding.Occupant.Equals(MapObjects.Start))
                    {
                        StartTile = null;
                    }

                    if(binding.Occupant.Equals(MapObjects.Obstacles))
                    {
                        binding.Occupant = MapObjects.None;
                        binding.Color = Color.White;
                    }
                    else
                    {
                        binding.Occupant = MapObjects.Obstacles;
                        binding.Color = Color.Black;
                    }
                    break;
                case MapObjects.Start:
                    if(binding.Occupant.Equals(MapObjects.Start))
                    {
                        binding.Occupant = MapObjects.None;
                        binding.Color = Color.White;
                        StartTile = null;
                    }
                    else
                    {
                        binding.Occupant = MapObjects.Start;
                        binding.Color = Color.Red;

                        if (StartTile != null)
                        {
                            StartTile.Color = Color.White;
                            StartTile.Occupant = MapObjects.None;
                        }
                        StartTile = binding;
                    }
                    break;
            }
        }

        void Handle_ClickedCreateNewMap(object sender, System.EventArgs e)
        {
            HideIdentifier(true);

            foreach(var tile in Tiles)
            {
                tile.Occupant = MapObjects.None;
                tile.Color = Color.White;
            }
        }

        void Handle_ClickedSolve(object sender, System.EventArgs e)
        {
            EditorItemFrame.BackgroundColor = Color.White;
            EditorItemLabel.Text = "None:";
            HideIdentifier(false);

            foreach (var tile in Tiles)
            {
                if (tile.Occupant.Equals(MapObjects.Path))
                {
                    tile.Color = Color.White;
                    tile.Occupant = MapObjects.None;
                }
            }

            Solve();
        }

        void Handle_ClearRoute(object sender, System.EventArgs e)
        {
            foreach (var tile in Tiles)
            {
                if (tile.Occupant.Equals(MapObjects.Path))
                {
                    tile.Color = Color.White;
                    tile.Occupant = MapObjects.None;
                }
            }
        }

        void Handle_ValueChanged_1(object sender, Xamarin.Forms.ValueChangedEventArgs e)
        {
            AnimationValue = e.NewValue;
            AnimationLabel.Text = $"AnimationTime: {AnimationValue}";
        }

        void Handle_ValueChanged(object sender, Xamarin.Forms.ValueChangedEventArgs e)
        {
            HeuristicValue = e.NewValue;
            SliderValue.Text = $"HeuristicValue: {HeuristicValue}";
        }

        void Handle_ClickedStartPoint(object sender, System.EventArgs e)
        {
            EditorItemFrame.BackgroundColor = Color.Red;
            EditorItemLabel.Text = "Start:";
            HideIdentifier(true);
            SelectedEditorItem = MapObjects.Start;
        }

        void Handle_ClickedGoal(object sender, System.EventArgs e)
        {
            EditorItemFrame.BackgroundColor = Color.Green;
            EditorItemLabel.Text = "Goal:";
            HideIdentifier(true);
            SelectedEditorItem = MapObjects.Goal;
        }

        void Handle_ClickedObstacles(object sender, System.EventArgs e)
        {
            EditorItemFrame.BackgroundColor = Color.Black;
            EditorItemLabel.Text = "Obstacles:";
            HideIdentifier(true);
            SelectedEditorItem = MapObjects.Obstacles;
        }

        private void HideIdentifier(bool isVisible)
        {
            if(isVisible)
            {
                EditorItemFrame.IsVisible = true;
                EditorItemLabel.IsVisible = true;
            }
            else
            {
                EditorItemFrame.IsVisible = false;
                EditorItemLabel.IsVisible = false;
            }
        }

        void Handle_ClickedExploratory(object sender, System.EventArgs e)
        {
            ExploratoryOption.BackgroundColor = Color.Black;
            BestSolutionOption.BackgroundColor = Color.White;
            IsAnimationExploratory = true;
        }

        void Handle_ClickedBstSolution(object sender, System.EventArgs e)
        {
            ExploratoryOption.BackgroundColor = Color.White;
            BestSolutionOption.BackgroundColor = Color.Black;
            IsAnimationExploratory = false;
        }
    }

    public enum MapObjects
    {
        Goal,
        Start,
        Obstacles,
        Path,
        None
    }
}
