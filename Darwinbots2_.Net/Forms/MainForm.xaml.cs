using DarwinBots.Model.Display;
using DarwinBots.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DarwinBots.Forms
{
    public partial class MainForm
    {
        private readonly List<RobotDisplay> _robotDisplays = new();
        private readonly List<ShotDisplay> _shotDisplays = new();
        private readonly List<TieDisplay> _tieDisplays = new();
        private readonly MainViewModel _viewModel = new();

        public MainForm()
        {
            InitializeComponent();

            DataContext = _viewModel;
            _viewModel.Dispatcher = Dispatcher;
            _viewModel.UpdateAvailable += OnUpdateAvailable;
        }

        protected override void OnClosed(EventArgs e)
        {
            _viewModel.StopSimulation();
            base.OnClosed(e);
        }

        private void OnUpdateAvailable(object sender, UpdateAvailableArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MainCanvas.Width = e.Update.FieldSize.Width;
                MainCanvas.Height = e.Update.FieldSize.Height;

                // TODO : Handle Sun
                MainCanvas.Background = Brushes.DarkBlue;
            });

            for (var i = 0; i < e.Update.RobotUpdates.Count; i++)
            {
                var update = e.Update.RobotUpdates[i];

                while (_robotDisplays.Count <= i)
                {
                    Ellipse ellipse = null;

                    Dispatcher.Invoke(() =>
                    {
                        ellipse = new Ellipse();
                        MainCanvas.Children.Add(ellipse);
                        ellipse.StrokeThickness = 10;
                        Canvas.SetZIndex(ellipse, 100);
                    });

                    _robotDisplays.Add(new RobotDisplay()
                    {
                        Display = ellipse
                    });
                }

                var display = _robotDisplays[i];

                Dispatcher.Invoke(() =>
                {
                    Canvas.SetLeft(display.Display, update.Position.X - update.Radius);
                    Canvas.SetTop(display.Display, update.Position.Y - update.Radius);
                    display.Display.Width = update.Radius * 2;
                    display.Display.Height = update.Radius * 2;
                    display.Display.Stroke = new SolidColorBrush(update.Color);
                });
            }

            while (_robotDisplays.Count > e.Update.RobotUpdates.Count)
            {
                var el = _robotDisplays[^1];
                Dispatcher.Invoke(() =>
                {
                    MainCanvas.Children.Remove(el.Display);
                });
                el.Display = null;
                _robotDisplays.Remove(el);
            }

            for (var i = 0; i < e.Update.TieUpdates.Count; i++)
            {
                var update = e.Update.TieUpdates[i];

                while (_tieDisplays.Count <= i)
                {
                    Line line = null;

                    Dispatcher.Invoke(() =>
                    {
                        line = new Line();
                        MainCanvas.Children.Add(line);
                        Canvas.SetZIndex(line, 10);
                    });

                    _tieDisplays.Add(new TieDisplay()
                    {
                        Display = line
                    });
                }

                var display = _tieDisplays[i];

                Dispatcher.Invoke(() =>
                {
                    display.Display.X1 = update.StartPoint.X;
                    display.Display.Y1 = update.StartPoint.Y;
                    display.Display.X2 = update.EndPoint.X;
                    display.Display.Y2 = update.EndPoint.Y;
                    display.Display.Stroke = new SolidColorBrush(update.Color);
                    display.Display.StrokeThickness = update.Width;
                });
            }

            while (_tieDisplays.Count > e.Update.TieUpdates.Count)
            {
                var el = _tieDisplays[^1];
                Dispatcher.Invoke(() =>
                {
                    MainCanvas.Children.Remove(el.Display);
                });
                el.Display = null;
                _tieDisplays.Remove(el);
            }

            for (var i = 0; i < e.Update.ShotUpdates.Count; i++)
            {
                var update = e.Update.ShotUpdates[i];

                while (_shotDisplays.Count <= i)
                {
                    Ellipse ellipse = null;

                    Dispatcher.Invoke(() =>
                    {
                        ellipse = new Ellipse();
                        MainCanvas.Children.Add(ellipse);
                        ellipse.StrokeThickness = 10;
                        Canvas.SetZIndex(ellipse, 100);
                    });

                    _shotDisplays.Add(new ShotDisplay()
                    {
                        Display = ellipse
                    });
                }

                var display = _shotDisplays[i];

                Dispatcher.Invoke(() =>
                {
                    Canvas.SetLeft(display.Display, update.Position.X - 20);
                    Canvas.SetTop(display.Display, update.Position.Y - 20);
                    display.Display.Width = 20 * 2;
                    display.Display.Height = 20 * 2;
                    display.Display.Stroke = new SolidColorBrush(update.Color);
                    display.Display.Fill = new SolidColorBrush(update.Color);
                });
            }

            while (_shotDisplays.Count > e.Update.ShotUpdates.Count)
            {
                var el = _shotDisplays[^1];
                Dispatcher.Invoke(() =>
                {
                    MainCanvas.Children.Remove(el.Display);
                });
                el.Display = null;
                _shotDisplays.Remove(el);
            }
        }
    }
}
