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
        }
    }
}
