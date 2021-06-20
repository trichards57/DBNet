using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static DarwinBots.Modules.HDRoutines;
using static DarwinBots.Modules.Multibots;
using static DarwinBots.Modules.SimOpt;

namespace DarwinBots.Modules
{
    internal static class Teleport
    {
        public const int MAXTELEPORTERS = 10;
        public static int teleporterDefaultWidth { get; set; }
        public static int teleporterFocus { get; set; }
        public static List<Teleporter> Teleporters { get; set; } = new();

        public static async Task CheckTeleporters(robot rob)
        {
            vector randomV = null;

            foreach (var tel in Teleporters.Where(t => t.Out || t.Local).Where(t => rob.exist && (TeleportCollision(rob, t))))
            {
                if (tel.Out)
                {
                    if (!(rob.Veg && !tel.TeleportVeggies) && !(rob.Corpse && !tel.TeleportCorpses) && !(!rob.Veg && tel.TeleportHeterotrophs))
                    {
                        tel.NumTeleported++;
                        var name = $@"\{DateTime.Today}{rob.FName}{Teleporters.IndexOf(tel)}{tel.NumTeleported}.dbo";
                        SaveOrganism(Path.Join(tel.path, name), rob);

                        await KillOrganism(rob);
                    }
                }
                else if (tel.Local)
                {
                    if (!(rob.Veg && !tel.TeleportVeggies) && !(rob.Corpse && !tel.TeleportCorpses) && !(!rob.Veg && !tel.TeleportHeterotrophs))
                    {
                        if (tel.Local)
                            tel.NumTeleported++;

                        randomV = new vector(ThreadSafeRandom.Local.Next(0, SimOpts.FieldWidth), ThreadSafeRandom.Local.Next(0, SimOpts.FieldHeight));

                        ReSpawn(rob, randomV.X, randomV.Y);
                    }
                }
            }
        }

        public static void DeleteAllTeleporters()
        {
            Teleporters.Clear();
        }

        public static void DeleteTeleporter(Teleporter tel)
        {
            Teleporters.Remove(tel);
        }

        public static void DrawTeleporters()
        {
            //dynamic DrawTeleporters = null;
            //int i = 0;

            //int sm = 0;

            //decimal telewidth = 0;

            //decimal zoomRatio = 0;

            //decimal aspectRatio = 0;

            //decimal twipWidth = 0;

            //int scw = 0;
            //int sch = 0;
            //int scm = 0;

            //int sct = 0;
            //int scl = 0;

            //decimal pictwidth = 0;

            //decimal pictmod = 0;

            //int hilightcolor = 0;

            //int visibleLeft = 0;

            //int visibleRight = 0;

            //int visibleTop = 0;

            //int visibleBottom = 0;

            //visibleLeft = Form1.ScaleLeft;
            //visibleRight = Form1.ScaleLeft + Form1.ScaleWidth;
            //visibleTop = Form1.ScaleTop;
            //visibleBottom = Form1.ScaleTop + Form1.ScaleHeight;

            //zoomRatio = Form1.ScaleWidth / SimOpts.FieldWidth;
            //aspectRatio = SimOpts.FieldHeight / SimOpts.FieldWidth;

            //Form1.FillStyle = 1;

            //for (i = 1; i < numTeleporters; i++)
            //{
            //    if (SimOpts.TotRunCycle >= 0)
            //    {
            //        if ((Form1.visiblew / RobSize) < 1000 & Teleporters(i).pos.x > visibleLeft && Teleporters(i).pos.x < visibleRight && Teleporters(i).pos.y > visibleTop && Teleporters(i).pos.y < visibleBottom)
            //        {
            //            pictwidth = (Form1.Teleporter.Picture.Height) * zoomRatio * SimOpts.FieldWidth / (2 * Form1.Width);
            //            pictmod = (SimOpts.TotRunCycle % 16) * pictwidth * 1.134m + Form1.ScaleLeft;

            //            Form1.PaintPicture(Form1.TeleporterMask.Picture, Teleporters(i).pos.x, Teleporters(i).pos.y, Teleporters(i).Width, Teleporters(i).Height, pictmod, Form1.ScaleTop, pictwidth);

            //            Form1.PaintPicture(Form1.Teleporter.Picture, Teleporters(i).pos.x, Teleporters(i).pos.y, Teleporters(i).Width, Teleporters(i).Height, pictmod, Form1.ScaleTop, pictwidth);
            //        }

            //        if (Teleporters(i).highlight && Teleporters(i).pos.x > visibleLeft && Teleporters(i).pos.x < visibleRight && Teleporters(i).pos.y > visibleTop && Teleporters(i).pos.y < visibleBottom)
            //        {
            //            if (Teleporters(i).In)
            //            {
            //                hilightcolor = vbGreen;
            //            }
            //            if (Teleporters(i).Out)
            //            {
            //                hilightcolor = vbRed;
            //            }
            //            if (Teleporters(i).local)
            //            {
            //                hilightcolor = vbYellow;
            //            }
            //            if (Teleporters(i).Internet)
            //            {
            //                hilightcolor = vbBlue;
            //            }
            //            Form1.Circle((Teleporters(i).pos.x + (Teleporters(i).Width / 2), Teleporters(i).pos.y + (Teleporters(i).Height / 3)), Teleporters(i).Width * 0.6m, hilightcolor);
            //        }

            //        if (i == teleporterFocus && Teleporters(i).pos.x > visibleLeft && Teleporters(i).pos.x < visibleRight && Teleporters(i).pos.y > visibleTop && Teleporters(i).pos.y < visibleBottom)
            //        {
            //            Form1.Circle((Teleporters(i).pos.x + (Teleporters(i).Width / 2), Teleporters(i).pos.y + (Teleporters(i).Height / 3)), Teleporters(i).Width * 0.7m, vbWhite);
            //        }
            //    }
            //}

            //Form1.FillStyle = 0;
            //// Form1.ScaleMode = sm     (SimOpts.TotRunCycle Mod 16) * (telewidth) * zoomRatio * SimOpts.FieldSize * aspectRatio * Teleporters(i).Height / Form1.Teleporter.Picture.Height + Form1.ScaleLeft,
            //return DrawTeleporters;
        }

        public static void DriftTeleporter(Teleporter tel)
        {
            var vel = SimOpts.MaxVelocity / 4;

            if (tel.DriftHorizontal)
                tel.Vel.X += ThreadSafeRandom.Local.NextDouble() - 0.5;

            if (tel.DriftVertical)
                tel.Vel.Y += ThreadSafeRandom.Local.NextDouble() - 0.5;

            if (tel.Vel.Magnitude() > vel)
                tel.Vel *= vel / tel.Vel.Magnitude();
        }

        public static void HighLightAllTeleporters()
        {
            foreach (var tel in Teleporters)
                tel.Highlight = true;
        }

        public static void MoveTeleporter(Teleporter tel)
        {
            if (tel.DriftHorizontal && tel.DriftVertical)
                tel.Pos += tel.Vel;

            tel.Center = new vector(tel.Pos.X + (tel.Width * 0.5), tel.Pos.Y + (tel.Height * 0.3));

            //Keep teleporters from drifting off into space.
            if (tel.Pos.X < 0)
            {
                if (tel.Pos.X + tel.Width < 0)
                    tel.Pos.X = 0;

                tel.Pos.X = SimOpts.Dxsxconnected ? tel.Pos.X + SimOpts.FieldWidth - tel.Width : SimOpts.MaxVelocity * 0.1;
            }
            if (tel.Pos.Y < 0)
            {
                if (tel.Pos.Y + tel.Height < 0)
                    tel.Pos.Y = 0;

                tel.Pos.Y = SimOpts.Updnconnected ? tel.Pos.Y + SimOpts.FieldHeight - tel.Height : SimOpts.MaxVelocity * 0.1;
            }
            if (tel.Pos.X + tel.Width > SimOpts.FieldWidth)
            {
                if (tel.Pos.X > SimOpts.FieldWidth)
                    tel.Pos.X = SimOpts.FieldWidth - tel.Width;

                tel.Pos.Y = SimOpts.Dxsxconnected ? tel.Pos.X - (SimOpts.FieldWidth - tel.Width) : -SimOpts.MaxVelocity * 0.1;
            }
            if (tel.Pos.Y + tel.Height > SimOpts.FieldHeight)
            {
                if (tel.Pos.Y > SimOpts.FieldHeight)
                    tel.Pos.Y = SimOpts.FieldHeight - tel.Height;

                tel.Pos.Y = SimOpts.Updnconnected ? tel.Pos.Y - (SimOpts.FieldHeight - tel.Height) : -SimOpts.MaxVelocity * 0.1;
            }
        }

        public static Teleporter NewTeleporter(bool PortIn, bool PortOut, double Height)
        {
            if (Teleporters.Count >= MAXTELEPORTERS)
            {
                return null;
            }
            else
            {
                var aspectRatio = (double)SimOpts.FieldHeight / SimOpts.FieldWidth;
                var randomX = ThreadSafeRandom.Local.Next(0, (int)(SimOpts.FieldWidth - (teleporterDefaultWidth * aspectRatio)));
                var randomy = ThreadSafeRandom.Local.Next(0, SimOpts.FieldHeight - teleporterDefaultWidth);

                var teleporter = new Teleporter
                {
                    Exist = true,
                    Pos = new vector(randomX, randomy),
                    Width = Height * aspectRatio,
                    Height = Height,
                    Vel = new vector(0, 0),
                    Color = Color.White,
                    In = PortIn,
                    Out = PortOut,
                    DriftHorizontal = true,
                    DriftVertical = true,
                    NumTeleported = 0,
                    NumTeleportedIn = 0,
                };

                return teleporter;
            }
        }

        public static void ResizeTeleporter(Teleporter tel, double height)
        {
            if (!tel.Exist)
                return;

            var aspectRatio = (double)SimOpts.FieldHeight / SimOpts.FieldWidth;
            tel.Width = height * aspectRatio;
            tel.Height = height;
        }

        public static bool TeleportCollision(robot rob, Teleporter tel)
        {
            return (rob.pos - tel.Center).Magnitude() < tel.Width / 2 + rob.radius;
        }

        public static void TeleportInBots()
        {
            if (SimOpts.Specie.Count > 45)
                return;

            foreach (var tel in Teleporters)
            {
                if (tel.In)
                {
                    if (tel.PollCountDown <= 0)
                    {
                        tel.PollCountDown = tel.InboundPollCycles;
                        var maxBotsPerCyclePerTeleporter = tel.BotsPerPoll;
                        foreach (var file in Directory.EnumerateFiles(tel.path))
                        {
                            switch (Path.GetExtension(file))
                            {
                                case "dbo":
                                    LoadOrganism(file, tel.Pos.X + tel.Width / 2, tel.Pos.Y + tel.Height / 3);
                                    tel.NumTeleportedIn++;
                                    File.Delete(file);
                                    maxBotsPerCyclePerTeleporter--;
                                    break;

                                case "temp":
                                    continue;

                                default:
                                    MessageBox.Show($"Non dbo file {Path.GetFileName(file)} found in {tel.path}.  Inbound Teleporter Deleted.", "Non DBO File Found", MessageBoxButton.OK, MessageBoxImage.Information);
                                    tel.Exist = false;
                                    Teleporters.Remove(tel);
                                    break;
                            }

                            if (maxBotsPerCyclePerTeleporter <= 0)
                                break;
                        }
                    }
                    else
                    {
                        tel.PollCountDown--;
                    }
                }
            }
        }

        public static void UnHighLightAllTeleporters()
        {
            foreach (var tel in Teleporters)
                tel.Highlight = false;
        }

        public static void UpdateTeleporters()
        {
            if (SimOpts.TotRunCycle >= 0)
            {
                foreach (var tel in Teleporters)
                {
                    DriftTeleporter(tel);
                    MoveTeleporter(tel);
                }
            }

            TeleportInBots();
        }

        public static Teleporter WhichTeleporter(double x, double y)
        {
            return Teleporters.FirstOrDefault(t => x >= t.Pos.X && x <= t.Pos.X + t.Width && y >= t.Pos.Y && y <= t.Pos.Y + t.Height);
        }
    }
}
