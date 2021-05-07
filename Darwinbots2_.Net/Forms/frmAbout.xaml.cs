using System.Windows;
using static Microsoft.VisualBasic.Constants;

namespace DBNet.Forms
{
    public partial class frmAbout : Window
    {
        private static frmAbout _instance;

        public frmAbout()
        {
            InitializeComponent();
        }

        public static frmAbout instance { set { _instance = null; } get { return _instance ?? (_instance = new frmAbout()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = frmAbout.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        // Option Explicit //False
        //Botsareus 2/24/2012 post 2.45 revisions update

        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            cmdOK_Click();
        }

        private void cmdOK_Click()
        {
            Hide();
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            Form_Load();
        }

        private void Form_Load()
        {
            Text1.Text = "";
            Text1.Text = Text1.Text + MDIForm1.instance.BaseCaption + vbCrLf;
            Text1.Text = Text1.Text + "http://www.darwinbots.com" + vbCrLf;
            Text1.Text = Text1.Text + "Original Copyright (C) 2003 Carlo Comis" + vbCrLf;
            Text1.Text = Text1.Text + "comis@libero.it" + vbCrLf;
            Text1.Text = Text1.Text + "http://digilander.libero.it/darwinbots" + vbCrLf;
            Text1.Text = Text1.Text + "" + vbCrLf;
            Text1.Text = Text1.Text + "Subsequent revisions by Purple Youko" + vbCrLf;
            Text1.Text = Text1.Text + "higginsb@missouri.edu" + vbCrLf;
            Text1.Text = Text1.Text + "" + vbCrLf;
            Text1.Text = Text1.Text + "2.4 - 2.42 work by Numsqil" + vbCrLf;
            Text1.Text = Text1.Text + "" + vbCrLf;
            Text1.Text = Text1.Text + "2.42 and beyond Copyright (C) Eric Lockard" + vbCrLf;
            Text1.Text = Text1.Text + "ericl@sulaadventures.com" + vbCrLf;
            Text1.Text = Text1.Text + "" + vbCrLf;
            Text1.Text = Text1.Text + "2.45.2 and beyond Copyright (C) Botsareus" + vbCrLf; //Botsareus 3/24/2012 more info
            Text1.Text = Text1.Text + "CosmoTwitt007" + vbCrLf; //Botsareus 3/24/2012 more info
            Text1.Text = Text1.Text + "" + vbCrLf; //Botsareus 3/24/2012 more info
            Text1.Text = Text1.Text + "Music by Testlund a.k.a. SoundStruggler" + vbCrLf; //Botsareus 6/11/2013 more info
            Text1.Text = Text1.Text + "" + vbCrLf; //Botsareus 6/11/2013 more info
            Text1.Text = Text1.Text + "Chloroplasts by PANDA" + vbCrLf; //Botsareus 8/31/2013 more info
            Text1.Text = Text1.Text + "" + vbCrLf; //Botsareus 8/31/2013 more info
            Text1.Text = Text1.Text + "Internet Mode by Peter" + vbCrLf; //Botsareus 2/25/2014 more info
            Text1.Text = Text1.Text + "" + vbCrLf; //Botsareus 2/25/2014 more info
            Text1.Text = Text1.Text + "All rights reserved. " + vbCrLf;
            Text1.Text = Text1.Text + "" + vbCrLf;
            Text1.Text = Text1.Text + "Redistribution and use in source and binary forms, with or without ";
            Text1.Text = Text1.Text + "modification, are permitted provided that:" + vbCrLf;
            Text1.Text = Text1.Text + "(1) source code distributions retain the above copyright notice and this ";
            Text1.Text = Text1.Text + "paragraph in its entirety," + vbCrLf;
            Text1.Text = Text1.Text + "(2) distributions including binary code include the above copyright notice and ";
            Text1.Text = Text1.Text + "this paragraph in its entirety in the documentation or other materials ";
            Text1.Text = Text1.Text + "provided with the distribution, and " + vbCrLf;
            Text1.Text = Text1.Text + "(3) Without the agreement of the authors redistribution of this product is only allowed ";
            Text1.Text = Text1.Text + "in non commercial terms and non profit distributions." + vbCrLf;
            Text1.Text = Text1.Text + "" + vbCrLf;
            Text1.Text = Text1.Text + "THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR IMPLIED ";
            Text1.Text = Text1.Text + "WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED WARRANTIES OF ";
            Text1.Text = Text1.Text + "MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.";
        }
    }
}
