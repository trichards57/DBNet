using System.Windows;
using static Common;
using static ObstaclesManager;
using static SimOpt;
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
            TransparentOption.value = SimOpts.MakeAllShapesTransparent;
            OpaqueOption.value = !SimOpts.MakeAllShapesTransparent;
            RandomColorOption.value = !SimOpts.MakeAllShapesBlack;
            BlackColorOption.value = SimOpts.MakeAllShapesBlack;
            WidthSlider.value = CInt(defaultWidth * 1000);
            HeightSlider.value = CInt(defaultHeight * 1000);
            DriftRateSlider.value = SimOpts.ShapeDriftRate;
            if (SimOpts.AllowHorizontalShapeDrift)
            {
                HorizontalDriftCheck.value = 1;
            }
            else
            {
                HorizontalDriftCheck.value = 0;
            }
            if (SimOpts.AllowVerticalShapeDrift)
            {
                VerticalDriftCheck.value = 1;
            }
            else
            {
                VerticalDriftCheck.value = 0;
            }
            MazeWidthSlider.value = mazeCorridorWidth;
            WallThicknessSlider.value = mazeWallThickness;
            if (SimOpts.ShapesAreSeeThrough)
            {
                ShapesSeeThroughCheck.value = 1;
            }
            else
            {
                ShapesSeeThroughCheck.value = 0;
            }
            if (SimOpts.ShapesAbsorbShots)
            {
                ShapesAbsorbShotsCheck.value = 1;
            }
            else
            {
                ShapesAbsorbShotsCheck.value = 0;
            }
            if (SimOpts.ShapesAreVisable)
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
            SimOpts.MakeAllShapesBlack = true;
            ChangeAllObstacleColor(vbBlack);
            RandomColorOption.value = false;
        }

        private void CopyToTmpOpts()
        { //Botsareus 1/5/2014 Make sure the shape settings are saved
            TmpOpts.MakeAllShapesTransparent = SimOpts.MakeAllShapesTransparent;
            TmpOpts.MakeAllShapesBlack = SimOpts.MakeAllShapesBlack;
            TmpOpts.ShapeDriftRate = SimOpts.ShapeDriftRate;
            TmpOpts.AllowHorizontalShapeDrift = SimOpts.AllowHorizontalShapeDrift;
            TmpOpts.AllowVerticalShapeDrift = SimOpts.AllowVerticalShapeDrift;
            TmpOpts.ShapesAreSeeThrough = SimOpts.ShapesAreSeeThrough;
            TmpOpts.ShapesAbsorbShots = SimOpts.ShapesAbsorbShots;
            TmpOpts.ShapesAreVisable = SimOpts.ShapesAreVisable;
        }

        private void DriftRateSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            DriftRateSlider_Change();
        }

        private void DriftRateSlider_Change()
        {
            SimOpts.ShapeDriftRate = DriftRateSlider.value;
            if (leftCompactor > 0)
            {
                ObstaclesManager.Obstacles(leftCompactor).vel.x = (SimOpts.ShapeDriftRate * 0.1m) * Sgn(ObstaclesManager.Obstacles(leftCompactor).vel.x);
            }
            if (rightCompactor > 0)
            {
                ObstaclesManager.Obstacles(rightCompactor).vel.x = (SimOpts.ShapeDriftRate * 0.1m) * Sgn(ObstaclesManager.Obstacles(rightCompactor).vel.x);
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
            SimOpts.AllowHorizontalShapeDrift = HorizontalDriftCheck.value;
            if (!SimOpts.AllowHorizontalShapeDrift)
            {
                ObstaclesManager.StopAllHorizontalObstacleMovement();
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
            SimOpts.MakeAllShapesTransparent = false;
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
            SimOpts.MakeAllShapesBlack = false;
        }

        private void ShapesAbsorbShotsCheck_Click(object sender, RoutedEventArgs e)
        {
            ShapesAbsorbShotsCheck_Click();
        }

        private void ShapesAbsorbShotsCheck_Click()
        {
            SimOpts.ShapesAbsorbShots = ShapesAbsorbShotsCheck.value;
        }

        private void ShapesSeeThroughCheck_Click(object sender, RoutedEventArgs e)
        {
            ShapesSeeThroughCheck_Click();
        }

        private void ShapesSeeThroughCheck_Click()
        {
            SimOpts.ShapesAreSeeThrough = ShapesSeeThroughCheck.value;
            if (SimOpts.ShapesAreSeeThrough)
            {
                ShapesVisableCheck.IsEnabled = false;
                SimOpts.ShapesAreVisable = false;
            }
            else
            {
                ShapesVisableCheck.IsEnabled = true;
                SimOpts.ShapesAreVisable = ShapesVisableCheck.value;
            }
        }

        private void ShapesVisableCheck_Click(object sender, RoutedEventArgs e)
        {
            ShapesVisableCheck_Click();
        }

        private void ShapesVisableCheck_Click()
        {
            SimOpts.ShapesAreVisable = ShapesVisableCheck.value;
            if (SimOpts.ShapesAreVisable)
            {
                ShapesSeeThroughCheck.IsEnabled = false;
                SimOpts.ShapesAreSeeThrough = false;
            }
            else
            {
                ShapesSeeThroughCheck.IsEnabled = true;
                SimOpts.ShapesAreSeeThrough = ShapesSeeThroughCheck.value;
            }
        }

        private void TransparentOption_Click(object sender, RoutedEventArgs e)
        {
            TransparentOption_Click();
        }

        private void TransparentOption_Click()
        {
            SimOpts.MakeAllShapesTransparent = true;
            OpaqueOption.value = false;
        }

        private void VerticalDriftCheck_Click(object sender, RoutedEventArgs e)
        {
            VerticalDriftCheck_Click();
        }

        private void VerticalDriftCheck_Click()
        {
            SimOpts.AllowVerticalShapeDrift = VerticalDriftCheck.value;
            if (!SimOpts.AllowVerticalShapeDrift)
            {
                ObstaclesManager.StopAllVerticalObstacleMovement();
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
