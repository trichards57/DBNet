using System.Windows;
using static Common;
using static Obstacles;
using static SimOptModule;
using static VBConstants;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class ObstacleForm : Window
    {
        private static ObstacleForm _instance;

        public ObstacleForm()
        {
            InitializeComponent();
        }

        public static ObstacleForm instance { set { _instance = null; } get { return _instance ?? (_instance = new ObstacleForm()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = ObstacleForm.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        // Copyright (c) 2006 Eric Lockard
        // eric@sulaadventures.com
        // All rights reserved.
        //Redistribution and use in source and binary forms, with or without
        //modification, are permitted provided that:
        //(1) source code distributions retain the above copyright notice and this
        //    paragraph in its entirety,
        //(2) distributions including binary code include the above copyright notice and
        //    this paragraph in its entirety in the documentation or other materials
        //    provided with the distribution, and
        //(3) Without the agreement of the author redistribution of this product is only allowed
        //    in non commercial terms and non profit distributions.
        //THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR IMPLIED
        //WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED WARRANTIES OF
        //MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
        // Option Explicit
        //Botsareus 6/12/2012 form's icon change

        public void InitShapesDialog()
        {
            TransparentOption.value = SimOpts.makeAllShapesTransparent;
            OpaqueOption.value = !SimOpts.makeAllShapesTransparent;
            RandomColorOption.value = !SimOpts.makeAllShapesBlack;
            BlackColorOption.value = SimOpts.makeAllShapesBlack;
            WidthSlider.value = CInt(defaultWidth * 1000);
            HeightSlider.value = CInt(defaultHeight * 1000);
            DriftRateSlider.value = SimOpts.shapeDriftRate;
            if (SimOpts.allowHorizontalShapeDrift)
            {
                HorizontalDriftCheck.value = 1;
            }
            else
            {
                HorizontalDriftCheck.value = 0;
            }
            if (SimOpts.allowVerticalShapeDrift)
            {
                VerticalDriftCheck.value = 1;
            }
            else
            {
                VerticalDriftCheck.value = 0;
            }
            MazeWidthSlider.value = mazeCorridorWidth;
            WallThicknessSlider.value = mazeWallThickness;
            if (SimOpts.shapesAreSeeThrough)
            {
                ShapesSeeThroughCheck.value = 1;
            }
            else
            {
                ShapesSeeThroughCheck.value = 0;
            }
            if (SimOpts.shapesAbsorbShots)
            {
                ShapesAbsorbShotsCheck.value = 1;
            }
            else
            {
                ShapesAbsorbShotsCheck.value = 0;
            }
            if (SimOpts.shapesAreVisable)
            {
                ShapesVisableCheck.value = 1;
            }
            else
            {
                ShapesVisableCheck.value = 0;
            }
        }

        private void BlackColorOption_Click(object sender, RoutedEventArgs e)
        {
            BlackColorOption_Click();
        }

        private void BlackColorOption_Click()
        {
            SimOpts.makeAllShapesBlack = true;
            ChangeAllObstacleColor(vbBlack);
            RandomColorOption.value = false;
        }

        private void CopyToTmpOpts()
        { //Botsareus 1/5/2014 Make sure the shape settings are saved
            TmpOpts.makeAllShapesTransparent = SimOpts.makeAllShapesTransparent;
            TmpOpts.makeAllShapesBlack = SimOpts.makeAllShapesBlack;
            TmpOpts.shapeDriftRate = SimOpts.shapeDriftRate;
            TmpOpts.allowHorizontalShapeDrift = SimOpts.allowHorizontalShapeDrift;
            TmpOpts.allowVerticalShapeDrift = SimOpts.allowVerticalShapeDrift;
            TmpOpts.shapesAreSeeThrough = SimOpts.shapesAreSeeThrough;
            TmpOpts.shapesAbsorbShots = SimOpts.shapesAbsorbShots;
            TmpOpts.shapesAreVisable = SimOpts.shapesAreVisable;
        }

        private void DriftRateSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            DriftRateSlider_Change();
        }

        private void DriftRateSlider_Change()
        {
            SimOpts.shapeDriftRate = DriftRateSlider.value;
            if (leftCompactor > 0)
            {
                Obstacles.Obstacles(leftCompactor).vel.x = (SimOpts.shapeDriftRate * 0.1m) * Sgn(Obstacles.Obstacles(leftCompactor).vel.x);
            }
            if (rightCompactor > 0)
            {
                Obstacles.Obstacles(rightCompactor).vel.x = (SimOpts.shapeDriftRate * 0.1m) * Sgn(Obstacles.Obstacles(rightCompactor).vel.x);
            }
        }

        private void HeightSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            HeightSlider_Change();
        }

        private void HeightSlider_Change()
        {
            defaultHeight = HeightSlider.value * 0.001m;
        }

        private void HorizontalDriftCheck_Click(object sender, RoutedEventArgs e)
        {
            HorizontalDriftCheck_Click();
        }

        private void HorizontalDriftCheck_Click()
        {
            SimOpts.allowHorizontalShapeDrift = HorizontalDriftCheck.value;
            if (!SimOpts.allowHorizontalShapeDrift)
            {
                Obstacles.StopAllHorizontalObstacleMovement();
            }
        }

        private void MakeShape_Click(object sender, RoutedEventArgs e)
        {
            MakeShape_Click();
        }

        private void MakeShape_Click()
        {
            decimal randomX = 0;

            decimal randomy = 0;

            randomX = Random(0, SimOpts.FieldWidth) - SimOpts.FieldWidth * (defaultWidth / 2);
            randomy = Random(0, SimOpts.FieldHeight) - SimOpts.FieldHeight * (defaultHeight / 2);
            NewObstacle(randomX, randomy, SimOpts.FieldWidth * defaultWidth, SimOpts.FieldHeight * defaultHeight);
        }

        private void MazeWidthSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            MazeWidthSlider_Change();
        }

        private void MazeWidthSlider_Change()
        {
            mazeCorridorWidth = MazeWidthSlider.value;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            OK_Click();
        }

        private void OK_Click()
        {
            CopyToTmpOpts();
            this
}

        private void OpaqueOption_Click(object sender, RoutedEventArgs e)
        {
            OpaqueOption_Click();
        }

        private void OpaqueOption_Click()
        {
            SimOpts.makeAllShapesTransparent = false;
            TransparentOption.value = false;
        }

        private void RandomColorOption_Click(object sender, RoutedEventArgs e)
        {
            RandomColorOption_Click();
        }

        private void RandomColorOption_Click()
        {
            BlackColorOption.value = false;
            ChangeAllObstacleColor(-1);
            SimOpts.makeAllShapesBlack = false;
        }

        private void ShapesAbsorbShotsCheck_Click(object sender, RoutedEventArgs e)
        {
            ShapesAbsorbShotsCheck_Click();
        }

        private void ShapesAbsorbShotsCheck_Click()
        {
            SimOpts.shapesAbsorbShots = ShapesAbsorbShotsCheck.value;
        }

        private void ShapesSeeThroughCheck_Click(object sender, RoutedEventArgs e)
        {
            ShapesSeeThroughCheck_Click();
        }

        private void ShapesSeeThroughCheck_Click()
        {
            SimOpts.shapesAreSeeThrough = ShapesSeeThroughCheck.value;
            if (SimOpts.shapesAreSeeThrough)
            {
                ShapesVisableCheck.IsEnabled = false;
                SimOpts.shapesAreVisable = false;
            }
            else
            {
                ShapesVisableCheck.IsEnabled = true;
                SimOpts.shapesAreVisable = ShapesVisableCheck.value;
            }
        }

        private void ShapesVisableCheck_Click(object sender, RoutedEventArgs e)
        {
            ShapesVisableCheck_Click();
        }

        private void ShapesVisableCheck_Click()
        {
            SimOpts.shapesAreVisable = ShapesVisableCheck.value;
            if (SimOpts.shapesAreVisable)
            {
                ShapesSeeThroughCheck.IsEnabled = false;
                SimOpts.shapesAreSeeThrough = false;
            }
            else
            {
                ShapesSeeThroughCheck.IsEnabled = true;
                SimOpts.shapesAreSeeThrough = ShapesSeeThroughCheck.value;
            }
        }

        private void TransparentOption_Click(object sender, RoutedEventArgs e)
        {
            TransparentOption_Click();
        }

        private void TransparentOption_Click()
        {
            SimOpts.makeAllShapesTransparent = true;
            OpaqueOption.value = false;
        }

        private void VerticalDriftCheck_Click(object sender, RoutedEventArgs e)
        {
            VerticalDriftCheck_Click();
        }

        private void VerticalDriftCheck_Click()
        {
            SimOpts.allowVerticalShapeDrift = VerticalDriftCheck.value;
            if (!SimOpts.allowVerticalShapeDrift)
            {
                Obstacles.StopAllVerticalObstacleMovement();
            }
        }

        private void WallThicknessSlider_Click(object sender, RoutedEventArgs e)
        {
            WallThicknessSlider_Click();
        }

        private void WallThicknessSlider_Click()
        {
            mazeWallThickness = WallThicknessSlider.value;
        }

        private void WidthSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            WidthSlider_Change();
        }

        private void WidthSlider_Change()
        {
            defaultWidth = WidthSlider.value * 0.001m;
        }
    }
}
