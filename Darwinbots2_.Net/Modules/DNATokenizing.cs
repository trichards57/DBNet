using System;
using static DNAExecution;
using static DNAManipulations;
using static Globals;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Robots;
using static System.Math;
using static VBExtension;

internal static class DNATokenizing
{
    public static bool savingtofile = false;

    private static readonly int[,] dnamatrix = new int[9, 14];

    //make sure that when we are saving to file do not normalize custome sysvars

    public static void calc_dnamatrix()
    {
        //calculate dna matrix
        var result = "";

        var count = 0;

        for (var y_tipo = 0; y_tipo < 8; y_tipo++)
        {
            for (var y_value = 0; y_value < 13; y_value++)
            {
                var Y = new block
                {
                    tipo = y_tipo + 2,
                    value = y_value + 1
                };
                Parse(ref result, Y);
                if (result != "")
                {
                    dnamatrix[y_tipo, y_value] = count;
                    count++;
                }
                result = "";
            }
        }
    }

    public static string DetokenizeDNA(int n, int Position)
    {
        var DetokenizeDNA = "";
        var GeneEnd = false;
        var dna = rob[n].dna;
        var ingene = false;
        var coding = false;
        var t = 1;
        var gene = 0;
        var lastgene = 0;

        while (!(dna[t].tipo == 10 & dna[t].value == 1))
        {
            var temp = "";
            //Gene breaks
            var rob = Robots.rob[n];
            // If a Start or Else
            if (dna[t].tipo == 9 && (dna[t].value == 2 || dna[t].value == 3))
            {
                if (coding && !ingene)
                    DetokenizeDNA = DetokenizeDNA + vbCrLf + "''''''''''''''''''''''''  " + "Gene: " + Str(gene) + " Ends at position " + Str(t - 1) + "  '''''''''''''''''''''''";

                if (!ingene)
                    gene++;
                else
                    ingene = false;

                coding = true;
            }
            // If a Cond
            if (dna[t].tipo == 9 && (dna[t].value == 1))
            {
                if (coding)
                    DetokenizeDNA = DetokenizeDNA + vbCrLf + "''''''''''''''''''''''''  " + "Gene: " + Str(gene) + " Ends at position " + Str(t - 1) + "  '''''''''''''''''''''''" + vbCrLf;

                ingene = true;
                gene++;
                coding = true;
            }
            // If a stop
            if (dna[t].tipo == 9 && dna[t].value == 4)
            {
                if (coding)
                    GeneEnd = true;

                ingene = false;
                coding = false;
            }

            if (gene != lastgene)
            { //Botsareus 5/28/2013 Small bug fix: '0' no longer on top of dna
                if (gene > 1)
                {
                    temp += vbCrLf;
                    temp += "''''''''''''''''''''''''  ";
                    temp = temp + "Gene: " + Str(gene);
                    temp = temp + " Begins at position " + Str(t);
                    temp += "  '''''''''''''''''''''''";
                    temp += vbCrLf;
                }
                else
                    temp += vbCrLf;

                DetokenizeDNA += temp;
                temp = "";
                lastgene = gene;
            }

            var converttosysvar = IIf(dna[t + 1].tipo == 7, true, false);
            Parse(ref temp, dna[t], n, converttosysvar);
            if (temp == "")
                temp = "VOID"; //alert user that there is an invalid DNA entry.

            //This is probably a BUG!

            var tempint = dna[t].tipo;

            //formatting
            if (tempint == 5 || tempint == 6 || tempint == 7 || tempint == 9)
                temp += vbCrLf;

            DetokenizeDNA = DetokenizeDNA + " " + temp;

            if (GeneEnd)
            { // Indicate gene ended via a stop.  Needs to come after base pair
                DetokenizeDNA = DetokenizeDNA + "''''''''''''''''''''''''  " + "Gene: " + Str(gene) + " Ends at position " + Str(t) + "  '''''''''''''''''''''''" + vbCrLf;
                GeneEnd = false;
            }

            if (Position > 0 & t == Position)
                DetokenizeDNA = DetokenizeDNA + " '[<POSITION MARKER]" + Chr(13) + Chr(10); //Botsareus 2/25/2013 Makes the program easy to debug

            t++;
        }
        if (!(dna[t - 1].tipo == 9 && dna[t - 1].value == 4) && coding)
        { // End of DNA without a stop.
            DetokenizeDNA = DetokenizeDNA + "''''''''''''''''''''''''  " + "Gene: " + Str(gene) + " Ends at position " + Str(t - 1) + "  '''''''''''''''''''''''" + vbCrLf;
        }

        return DetokenizeDNA;
    }

    public static int DNAtoInt(int tipo, int value)
    {
        var DNAtoInt = 0;
        //make value sane
        if (value > 32000)
        {
            value = 32000;
        }
        if (value < -32000)
        {
            value = -32000;
        }
        //figure out conversion
        if (tipo < 2)
        {
            DNAtoInt = -16646;

            if (Abs(value) > 999)
            {
                value = (int)(512 * Sign(value) + value / 2.05);
            }

            DNAtoInt += value;

            if (tipo == 1)
            {
                DNAtoInt += 32729;
            }
        }
        else if (tipo > 1)
        {
            //other types
            DNAtoInt = 32691 + dnamatrix[tipo - 2, value - 1]; //dnamatrix adds max of 76 because we have 76 commands
        }
        return DNAtoInt;
    }

    /*
    ' loads the dna and parses it
    */

    public static string Hash(string s, int f)
    {
        var buf = new int[101];
        var Hash = "";

        s = s.Trim();

        while (Right(s, 2) == vbCrLf)
        {
            s = Left(s, Len(s) - 2);
        }

        for (var k = 0; k < f; k++)
            buf[k] = 0;

        for (var k = 1; k < Len(s); k++)
        {
            buf[k % f] = buf[k % f] + Asc(Mid(s, k, 1));
            buf[k % f] = buf[k % f] + buf[(k - 1) % f];
            buf[k % f] = buf[k % f] % 100;
        }

        for (var k = 0; k < f - 1; k++)
            Hash += Chr(buf[k] % 93 + 33);

        return Hash;
    }

    public static bool LoadDNA(ref string path, ref int n)
    {
        var DNApos = 0;
        var hold = "";

        rob[n].dna = new block[0];

        if (path == "")
            return false;

        VBOpenFile(1, path);

        var useref = false;

        while (!EOF(1))
        {
            var a = LineInput(1);
            var clonea = a;

            // eliminate comments at the end of a line
            // but preserves comments-only lines
            var pos = InStr(a, "'");
            if (pos > 1)
            {
                a = Left(a, pos - 1);
            }
            if (Right(a, 2) == vbCrLf)
            {
                a = Left(a, Len(a) - 2);
            }

            //Replace any tabs with spaces
            a = Replace(a, vbTab, " ");
            a = Trim(a);

            if ((Left(a, 1) != "'" && Left(a, 1) != "/") && a != "")
            {
                if (Left(a, 3) == "def")
                {
                    insertvar(n, a);
                    useref = true;
                }
                else
                {
                    pos = InStr(a, " ");
                    while (pos != 0)
                    {
                        var b = Left(a, pos - 1);
                        a = Right(a, Len(a) - pos);

                        while (Left(a, 0) == " ")
                        {
                            a = Right(a, Len(a) - 1);
                        }
                        pos = InStr(a, " ");

                        if (b != "")
                        {
                            DNApos++;
                            if (DNApos > UBound(rob[n].dna))
                            {
                                var robTmp = new block[DNApos + 5];
                                Array.Copy(rob[n].dna, robTmp, rob[n].dna.Length);
                                rob[n].dna = robTmp;
                            }
                            Parse(ref b, rob[n].dna[DNApos], n);
                        }
                    }
                    if (a != "")
                    {
                        DNApos++;
                        if (DNApos > UBound(rob[n].dna))
                        {
                            var robTmp = new block[DNApos + 5];
                            Array.Copy(rob[n].dna, robTmp, rob[n].dna.Length);
                            rob[n].dna = robTmp;
                        }
                        Parse(ref a, rob[n].dna[DNApos], n);
                    }
                }
            }
            else
            {
                if (Left(a, 2) == "'#" || Left(a, 2) == "/#")
                {
                    // embryo of a new feature, should allow recording
                    // in dna files info such robot colour, generation,
                    // mutations etc
                    getvals(n, a, hold);
                }
            }
            hold = hold + clonea + vbCrLf; //Botsareus 10/8/2015 Simpler hold
        }
        VBCloseFile(1);
        var LoadDNA = true;
        DNApos++;
        if (DNApos > UBound(rob[n].dna))
        {
            var robTmp = new block[DNApos + 1];
            Array.Copy(rob[n].dna, robTmp, rob[n].dna.Length);
            rob[n].dna = robTmp;
        }
        rob[n].dna[DNApos].tipo = 10;
        rob[n].dna[DNApos].value = 1;
        //ReDim Preserve rob[n].DNA(DnaLen(rob[n].DNA())) ' EricL commented out March 15, 2006
        //Botsareus 6/5/2013 Bug fix to do with leading zero on def
        if (useref)
        {
            if (rob[n].dna[0].tipo == 0 && rob[n].dna[0].value == 0 && rob[n].dna[1].tipo != 9)
            {
                for (DNApos = 0; DNApos < UBound(rob[n].dna) - 1; DNApos++)
                {
                    rob[n].dna[DNApos] = rob[n].dna[DNApos + 1];
                }
            }
        }
        return LoadDNA;
    }

    /*
    ' parses dna code, tokenizing or detokenizing instructions in the block
    ' structure
    ' types:
    ' 0 = number
    ' 1 = *number
    ' 2 = basic command
    ' 3 = advanced command
    ' 4 = bitwise command
    ' 5 = conditions
    ' 6 = logic
    ' 7 = stores
    ' 8 = Empty
    ' 9 = flow
    ' 10= Master flow

    ' if the string it's passed is "", then it will
    ' detokenize the command sent to it via bp and store in into command

    ' if the string it's passed isn't empty, then it will
    ' tokenize the string into the bp block.  If the command isn't
    ' recognized then it inserts 0.0 (that is, the number 0)

    ' this is all done byref, so be sure to understand that your variables
    ' WILL change after going through this subfunction.  Be sure to save any
    ' data you don't want modified elsewhere
    */

    public static void LoadSysVars()
    {
        sysvar[1].Name = "up";
        sysvar[1].value = 1;

        sysvar[2].Name = "dn";
        sysvar[2].value = 2;

        sysvar[3].Name = "sx";
        sysvar[3].value = 3;

        sysvar[4].Name = "dx";
        sysvar[4].value = 4;

        sysvar[5].Name = "aimdx";
        sysvar[5].value = 5;

        sysvar[6].Name = "aimright";
        sysvar[6].value = 5;

        sysvar[7].Name = "aimsx";
        sysvar[7].value = 6;

        sysvar[8].Name = "aimleft";
        sysvar[8].value = 6;

        sysvar[9].Name = "shoot";
        sysvar[9].value = 7;

        sysvar[10].Name = "shootval";
        sysvar[10].value = 8;

        sysvar[11].Name = "robage";
        sysvar[11].value = 9;

        sysvar[12].Name = "mass";
        sysvar[12].value = 10;

        sysvar[13].Name = "maxvel";
        sysvar[13].value = 11;

        sysvar[14].Name = "timer";
        sysvar[14].value = 12;

        sysvar[15].Name = "aim";
        sysvar[15].value = 18;

        sysvar[16].Name = "setaim";
        sysvar[16].value = 19;

        sysvar[17].Name = "bodgain";
        sysvar[17].value = 194;

        sysvar[18].Name = "bodloss";
        sysvar[18].value = 195;

        sysvar[19].Name = "velscalar";
        sysvar[19].value = 196;

        sysvar[20].Name = "velsx";
        sysvar[20].value = 197;

        sysvar[21].Name = "veldx";
        sysvar[21].value = 198;

        sysvar[22].Name = "veldn";
        sysvar[22].value = 199;

        sysvar[23].Name = "velup";
        sysvar[23].value = 200;

        sysvar[24].Name = "vel";
        sysvar[24].value = 200;

        sysvar[25].Name = "hit";
        sysvar[25].value = 201;

        sysvar[26].Name = "shflav";
        sysvar[26].value = 202;

        sysvar[27].Name = "pain";
        sysvar[27].value = 203;

        sysvar[28].Name = "pleas";
        sysvar[28].value = 204;

        sysvar[29].Name = "hitup";
        sysvar[29].value = 205;

        sysvar[30].Name = "hitdn";
        sysvar[30].value = 206;

        sysvar[31].Name = "hitdx";
        sysvar[31].value = 207;

        sysvar[32].Name = "hitsx";
        sysvar[32].value = 208;

        sysvar[33].Name = "shang";
        sysvar[33].value = 209;

        sysvar[34].Name = "shup";
        sysvar[34].value = 210;

        sysvar[35].Name = "shdn";
        sysvar[35].value = 211;

        sysvar[36].Name = "shdx";
        sysvar[36].value = 212;

        sysvar[37].Name = "shsx";
        sysvar[37].value = 213;

        sysvar[38].Name = "edge";
        sysvar[38].value = 214;

        sysvar[39].Name = "fixed";
        sysvar[39].value = 215;

        sysvar[40].Name = "fixpos";
        sysvar[40].value = 216;

        sysvar[41].Name = "depth";
        sysvar[41].value = 217;

        sysvar[42].Name = "ypos";
        sysvar[42].value = 217;

        sysvar[43].Name = "daytime";
        sysvar[43].value = 218;

        sysvar[44].Name = "xpos";
        sysvar[44].value = 219;

        sysvar[45].Name = "kills";
        sysvar[45].value = 220;

        sysvar[46].Name = "hitang";
        sysvar[46].value = 221;

        sysvar[47].Name = "repro";
        sysvar[47].value = 300;

        sysvar[48].Name = "mrepro";
        sysvar[48].value = 301;

        sysvar[49].Name = "sexrepro";
        sysvar[49].value = 302;

        sysvar[50].Name = "nrg";
        sysvar[50].value = 310;

        sysvar[51].Name = "body";
        sysvar[51].value = 311;

        sysvar[52].Name = "fdbody";
        sysvar[52].value = 312;

        sysvar[53].Name = "strbody";
        sysvar[53].value = 313;

        sysvar[54].Name = "setboy";
        sysvar[54].value = 314;

        sysvar[55].Name = "rdboy";
        sysvar[55].value = 315;

        sysvar[56].Name = "tie";
        sysvar[56].value = 330;

        sysvar[57].Name = "stifftie";
        sysvar[57].value = 331;

        sysvar[58].Name = "mkvirus";
        sysvar[58].value = 335;

        sysvar[59].Name = "dnalen";
        sysvar[59].value = 336;

        sysvar[60].Name = "vtimer";
        sysvar[60].value = 337;

        sysvar[61].Name = "vshoot";
        sysvar[61].value = 338;

        sysvar[62].Name = "genes";
        sysvar[62].value = 339;

        sysvar[63].Name = "delgene";
        sysvar[63].value = 340;

        sysvar[64].Name = "thisgene";
        sysvar[64].value = 341;

        sysvar[65].Name = "sun";
        sysvar[65].value = 400;

        sysvar[66].Name = "trefbody";
        sysvar[66].value = 437;

        sysvar[67].Name = "trefxpos";
        sysvar[67].value = 438;

        sysvar[68].Name = "trefypos";
        sysvar[68].value = 439;

        sysvar[69].Name = "trefvelmysx";
        sysvar[69].value = 440;

        sysvar[70].Name = "trefvelmydx";
        sysvar[70].value = 441;

        sysvar[71].Name = "trefvelmydn";
        sysvar[71].value = 442;

        sysvar[72].Name = "trefvelmyup";
        sysvar[72].value = 443;

        sysvar[73].Name = "trefvelscalar";
        sysvar[73].value = 444;

        sysvar[74].Name = "trefvelyoursx";
        sysvar[74].value = 445;

        sysvar[75].Name = "trefvelyourdx";
        sysvar[75].value = 446;

        sysvar[76].Name = "trefvelyourdn";
        sysvar[76].value = 447;

        sysvar[77].Name = "trefvelyourup";
        sysvar[77].value = 448;

        sysvar[78].Name = "trefshell";
        sysvar[78].value = 449;

        sysvar[79].Name = "tieang";
        sysvar[79].value = 450;

        sysvar[80].Name = "tielen";
        sysvar[80].value = 451;

        sysvar[81].Name = "tieloc";
        sysvar[81].value = 452;

        sysvar[82].Name = "tieval";
        sysvar[82].value = 453;

        sysvar[83].Name = "tiepres";
        sysvar[83].value = 454;

        sysvar[84].Name = "tienum";
        sysvar[84].value = 455;

        sysvar[85].Name = "trefup";
        sysvar[85].value = 456;

        sysvar[86].Name = "trefdn";
        sysvar[86].value = 457;

        sysvar[87].Name = "trefsx";
        sysvar[87].value = 458;

        sysvar[88].Name = "trefdx";
        sysvar[88].value = 459;

        sysvar[89].Name = "trefaimdx";
        sysvar[89].value = 460;

        sysvar[90].Name = "trefaimsx";
        sysvar[90].value = 461;

        sysvar[91].Name = "trefshoot";
        sysvar[91].value = 462;

        sysvar[92].Name = "trefeye";
        sysvar[92].value = 463;

        sysvar[93].Name = "trefnrg";
        sysvar[93].value = 464;

        sysvar[94].Name = "trefage";
        sysvar[94].value = 465;

        sysvar[95].Name = "numties";
        sysvar[95].value = 466;

        sysvar[96].Name = "deltie";
        sysvar[96].value = 467;

        sysvar[97].Name = "fixang";
        sysvar[97].value = 468;

        sysvar[98].Name = "fixlen";
        sysvar[98].value = 469;

        sysvar[99].Name = "multi";
        sysvar[99].value = 470;

        sysvar[100].Name = "readtie";
        sysvar[100].value = 471;

        sysvar[101].Name = "fertilized";
        sysvar[101].value = 303;

        sysvar[102].Name = "memval";
        sysvar[102].value = 473;

        sysvar[103].Name = "memloc";
        sysvar[103].value = 474;

        sysvar[104].Name = "tmemval";
        sysvar[104].value = 475;

        sysvar[105].Name = "tmemloc";
        sysvar[105].value = 476;

        sysvar[106].Name = "reffixed";
        sysvar[106].value = 477;

        sysvar[107].Name = "treffixed";
        sysvar[107].value = 478;

        sysvar[108].Name = "trefaim";
        sysvar[108].value = 479;

        sysvar[109].Name = "tieang1";
        sysvar[109].value = 480;

        sysvar[110].Name = "tieang2";
        sysvar[110].value = 481;

        sysvar[111].Name = "tieang3";
        sysvar[111].value = 482;

        sysvar[112].Name = "tieang4";
        sysvar[112].value = 483;

        sysvar[113].Name = "tielen1";
        sysvar[113].value = 484;

        sysvar[114].Name = "tielen2";
        sysvar[114].value = 485;

        sysvar[115].Name = "tielen3";
        sysvar[115].value = 486;

        sysvar[116].Name = "tielen4";
        sysvar[116].value = 487;

        sysvar[117].Name = "eye1";
        sysvar[117].value = 501;

        sysvar[118].Name = "eye2";
        sysvar[118].value = 502;

        sysvar[119].Name = "eye3";
        sysvar[119].value = 503;

        sysvar[120].Name = "eye4";
        sysvar[120].value = 504;

        sysvar[121].Name = "eye5";
        sysvar[121].value = 505;

        sysvar[122].Name = "eye6";
        sysvar[122].value = 506;

        sysvar[123].Name = "eye7";
        sysvar[123].value = 507;

        sysvar[124].Name = "eye8";
        sysvar[124].value = 508;

        sysvar[125].Name = "eye9";
        sysvar[125].value = 509;

        sysvar[126].Name = "refmulti";
        sysvar[126].value = 686;

        sysvar[127].Name = "refshell";
        sysvar[127].value = 687;

        sysvar[128].Name = "refbody";
        sysvar[128].value = 688;

        sysvar[129].Name = "refxpos";
        sysvar[129].value = 689;

        sysvar[130].Name = "refypos";
        sysvar[130].value = 690;

        sysvar[131].Name = "refvelscalar";
        sysvar[131].value = 695;

        sysvar[132].Name = "refvelsx";
        sysvar[132].value = 696;

        sysvar[133].Name = "refveldx";
        sysvar[133].value = 697;

        sysvar[134].Name = "refveldn";
        sysvar[134].value = 698;

        sysvar[135].Name = "refvel";
        sysvar[135].value = 699;

        sysvar[136].Name = "refvelup";
        sysvar[136].value = 699;

        sysvar[137].Name = "refup";
        sysvar[137].value = 701;

        sysvar[138].Name = "refdn";
        sysvar[138].value = 702;

        sysvar[139].Name = "refsx";
        sysvar[139].value = 703;

        sysvar[140].Name = "refdx";
        sysvar[140].value = 704;

        sysvar[141].Name = "refaimdx";
        sysvar[141].value = 705;

        sysvar[142].Name = "refaimsx";
        sysvar[142].value = 706;

        sysvar[143].Name = "refshoot";
        sysvar[143].value = 707;

        sysvar[144].Name = "refeye";
        sysvar[144].value = 708;

        sysvar[145].Name = "refnrg";
        sysvar[145].value = 709;

        sysvar[146].Name = "refage";
        sysvar[146].value = 710;

        sysvar[147].Name = "refaim";
        sysvar[147].value = 711;

        sysvar[148].Name = "reftie";
        sysvar[148].value = 712;

        sysvar[149].Name = "refpoison";
        sysvar[149].value = 713;

        sysvar[150].Name = "refvenom";
        sysvar[150].value = 714;

        sysvar[151].Name = "refkills";
        sysvar[151].value = 715;

        sysvar[152].Name = "myup";
        sysvar[152].value = 721;

        sysvar[153].Name = "mydn";
        sysvar[153].value = 722;

        sysvar[154].Name = "mysx";
        sysvar[154].value = 723;

        sysvar[155].Name = "mydx";
        sysvar[155].value = 724;

        sysvar[156].Name = "myaimdx";
        sysvar[156].value = 725;

        sysvar[157].Name = "myaimsx";
        sysvar[157].value = 726;

        sysvar[158].Name = "myshoot";
        sysvar[158].value = 727;

        sysvar[159].Name = "myeye";
        sysvar[159].value = 728;

        sysvar[160].Name = "myties";
        sysvar[160].value = 729;

        sysvar[161].Name = "mypoison";
        sysvar[161].value = 730;

        sysvar[162].Name = "myvenom";
        sysvar[162].value = 731;

        sysvar[163].Name = "out1";
        sysvar[163].value = 800;

        sysvar[164].Name = "out2";
        sysvar[164].value = 801;

        sysvar[165].Name = "out3";
        sysvar[165].value = 802;

        sysvar[166].Name = "out4";
        sysvar[166].value = 803;

        sysvar[167].Name = "out5";
        sysvar[167].value = 804;

        sysvar[168].Name = "out6";
        sysvar[168].value = 805;

        sysvar[169].Name = "out7";
        sysvar[169].value = 806;

        sysvar[170].Name = "out8";
        sysvar[170].value = 807;

        sysvar[171].Name = "out9";
        sysvar[171].value = 808;

        sysvar[172].Name = "out10";
        sysvar[172].value = 809;

        sysvar[173].Name = "in1";
        sysvar[173].value = 810;

        sysvar[174].Name = "in2";
        sysvar[174].value = 811;

        sysvar[175].Name = "in3";
        sysvar[175].value = 812;

        sysvar[176].Name = "in4";
        sysvar[176].value = 813;

        sysvar[177].Name = "in5";
        sysvar[177].value = 814;

        sysvar[178].Name = "in6";
        sysvar[178].value = 815;

        sysvar[179].Name = "in7";
        sysvar[179].value = 816;

        sysvar[180].Name = "in8";
        sysvar[180].value = 817;

        sysvar[181].Name = "in9";
        sysvar[181].value = 818;

        sysvar[182].Name = "in10";
        sysvar[182].value = 819;

        sysvar[183].Name = "mkslime";
        sysvar[183].value = 820;

        sysvar[184].Name = "slime";
        sysvar[184].value = 821;

        sysvar[185].Name = "mkshell";
        sysvar[185].value = 822;

        sysvar[186].Name = "shell";
        sysvar[186].value = 823;

        sysvar[187].Name = "strvenom";
        sysvar[187].value = 824;

        sysvar[188].Name = "mkvenom";
        sysvar[188].value = 824;

        sysvar[189].Name = "venom";
        sysvar[189].value = 825;

        sysvar[190].Name = "strpoison";
        sysvar[190].value = 826;

        sysvar[191].Name = "mkpoison";
        sysvar[191].value = 826;

        sysvar[192].Name = "poison";
        sysvar[192].value = 827;

        sysvar[193].Name = "waste";
        sysvar[193].value = 828;

        sysvar[194].Name = "pwaste";
        sysvar[194].value = 829;

        sysvar[195].Name = "sharenrg";
        sysvar[195].value = 830;

        sysvar[196].Name = "sharewaste";
        sysvar[196].value = 831;

        sysvar[197].Name = "shareshell";
        sysvar[197].value = 832;

        sysvar[198].Name = "shareslime";
        sysvar[198].value = 833;

        sysvar[199].Name = "ploc";
        sysvar[199].value = 834;

        sysvar[200].Name = "vloc";
        sysvar[200].value = 835;

        sysvar[201].Name = "venval";
        sysvar[201].value = 836;

        sysvar[202].Name = "paralyzed";
        sysvar[202].value = 837;

        sysvar[203].Name = "poisoned";
        sysvar[203].value = 838;

        sysvar[204].Name = "backshot";
        sysvar[204].value = 900;

        sysvar[205].Name = "aimshoot";
        sysvar[205].value = 901;

        sysvar[206].Name = "eyef";
        sysvar[206].value = 510;

        sysvar[207].Name = "focuseye";
        sysvar[207].value = 511;

        sysvar[208].Name = "eye1dir";
        sysvar[208].value = 521;

        sysvar[209].Name = "eye2dir";
        sysvar[209].value = 522;

        sysvar[210].Name = "eye3dir";
        sysvar[210].value = 523;

        sysvar[211].Name = "eye4dir";
        sysvar[211].value = 524;

        sysvar[212].Name = "eye5dir";
        sysvar[212].value = 525;

        sysvar[213].Name = "eye6dir";
        sysvar[213].value = 526;

        sysvar[214].Name = "eye7dir";
        sysvar[214].value = 527;

        sysvar[215].Name = "eye8dir";
        sysvar[215].value = 528;

        sysvar[216].Name = "eye9dir";
        sysvar[216].value = 529;

        sysvar[217].Name = "eye1width";
        sysvar[217].value = 531;

        sysvar[218].Name = "eye2width";
        sysvar[218].value = 532;

        sysvar[219].Name = "eye3width";
        sysvar[219].value = 533;

        sysvar[220].Name = "eye4width";
        sysvar[220].value = 534;

        sysvar[221].Name = "eye5width";
        sysvar[221].value = 535;

        sysvar[222].Name = "eye6width";
        sysvar[222].value = 536;

        sysvar[223].Name = "eye7width";
        sysvar[223].value = 537;

        sysvar[224].Name = "eye8width";
        sysvar[224].value = 538;

        sysvar[225].Name = "eye9width";
        sysvar[225].value = 539;

        sysvar[226].Name = "reftype";
        sysvar[226].value = 685;

        sysvar[227].Name = "totalbots";
        sysvar[227].value = 401;

        sysvar[228].Name = "totalmyspecies";
        sysvar[228].value = 402;

        sysvar[229].Name = "tout1";
        sysvar[229].value = 410;

        sysvar[230].Name = "tout2";
        sysvar[230].value = 411;

        sysvar[231].Name = "tout3";
        sysvar[231].value = 412;

        sysvar[232].Name = "tout4";
        sysvar[232].value = 413;

        sysvar[233].Name = "tout5";
        sysvar[233].value = 414;

        sysvar[234].Name = "tout6";
        sysvar[234].value = 415;

        sysvar[235].Name = "tout7";
        sysvar[235].value = 416;

        sysvar[236].Name = "tout8";
        sysvar[236].value = 417;

        sysvar[237].Name = "tout9";
        sysvar[237].value = 418;

        sysvar[238].Name = "tout10";
        sysvar[238].value = 419;

        sysvar[239].Name = "tin1";
        sysvar[239].value = 420;

        sysvar[240].Name = "tin2";
        sysvar[240].value = 421;

        sysvar[241].Name = "tin3";
        sysvar[241].value = 422;

        sysvar[242].Name = "tin4";
        sysvar[242].value = 423;

        sysvar[243].Name = "tin5";
        sysvar[243].value = 424;

        sysvar[244].Name = "tin6";
        sysvar[244].value = 425;

        sysvar[245].Name = "tin7";
        sysvar[245].value = 426;

        sysvar[246].Name = "tin8";
        sysvar[246].value = 427;

        sysvar[247].Name = "tin9";
        sysvar[247].value = 428;

        sysvar[248].Name = "tin10";
        sysvar[248].value = 429;

        sysvar[249].Name = "pval";
        sysvar[249].value = 839;

        //Botsareus 8/14/2013 New chloroplast code
        sysvar[250].Name = "chlr";
        sysvar[250].value = 920;

        sysvar[251].Name = "mkchlr";
        sysvar[251].value = 921;

        sysvar[252].Name = "rmchlr";
        sysvar[252].value = 922;

        sysvar[253].Name = "light";
        sysvar[253].value = 923;

        sysvar[254].Name = "availability";
        sysvar[254].value = 923;

        sysvar[255].Name = "sharechlr";
        sysvar[255].value = 924;

        //informational

        //sysvarIN[1].Name = "up"
        //sysvarIN[1].value = 1

        //sysvarIN[2].Name = "dn"
        //sysvarIN[2].value = 2

        //sysvarIN[3].Name = "sx"
        //sysvarIN[3].value = 3

        //sysvarIN[4].Name = "dx"
        //sysvarIN[4].value = 4

        //sysvarIN[5].Name = "aimdx"
        //sysvarIN[5].value = 5

        //sysvarIN[6].Name = "aimright"
        //sysvarIN[6].value = 5

        //sysvarIN[7].Name = "aimsx"
        //sysvarIN[7].value = 6

        //sysvarIN[8].Name = "aimleft"
        //sysvarIN[8].value = 6

        //sysvarIN[9].Name = "shoot"
        //sysvarIN[9].value = 7

        //sysvarIN[10].Name = "shootval"
        //sysvarIN[10].value = 8

        sysvarIN[11].Name = "robage";
        sysvarIN[11].value = 9;

        sysvarIN[12].Name = "mass";
        sysvarIN[12].value = 10;

        sysvarIN[13].Name = "maxvel";
        sysvarIN[13].value = 11;

        sysvarIN[14].Name = "timer";
        sysvarIN[14].value = 12;

        sysvarIN[15].Name = "aim";
        sysvarIN[15].value = 18;

        //sysvarIN[16].Name = "setaim"
        //sysvarIN[16].value = 19

        sysvarIN[17].Name = "bodgain";
        sysvarIN[17].value = 194;

        sysvarIN[18].Name = "bodloss";
        sysvarIN[18].value = 195;

        sysvarIN[19].Name = "velscalar";
        sysvarIN[19].value = 196;

        sysvarIN[20].Name = "velsx";
        sysvarIN[20].value = 197;

        sysvarIN[21].Name = "veldx";
        sysvarIN[21].value = 198;

        sysvarIN[22].Name = "veldn";
        sysvarIN[22].value = 199;

        sysvarIN[23].Name = "velup";
        sysvarIN[23].value = 200;

        sysvarIN[24].Name = "vel";
        sysvarIN[24].value = 200;

        sysvarIN[25].Name = "hit";
        sysvarIN[25].value = 201;

        sysvarIN[26].Name = "shflav";
        sysvarIN[26].value = 202;

        sysvarIN[27].Name = "pain";
        sysvarIN[27].value = 203;

        sysvarIN[28].Name = "pleas";
        sysvarIN[28].value = 204;

        sysvarIN[29].Name = "hitup";
        sysvarIN[29].value = 205;

        sysvarIN[30].Name = "hitdn";
        sysvarIN[30].value = 206;

        sysvarIN[31].Name = "hitdx";
        sysvarIN[31].value = 207;

        sysvarIN[32].Name = "hitsx";
        sysvarIN[32].value = 208;

        sysvarIN[33].Name = "shang";
        sysvarIN[33].value = 209;

        sysvarIN[34].Name = "shup";
        sysvarIN[34].value = 210;

        sysvarIN[35].Name = "shdn";
        sysvarIN[35].value = 211;

        sysvarIN[36].Name = "shdx";
        sysvarIN[36].value = 212;

        sysvarIN[37].Name = "shsx";
        sysvarIN[37].value = 213;

        sysvarIN[38].Name = "edge";
        sysvarIN[38].value = 214;

        sysvarIN[39].Name = "fixed";
        sysvarIN[39].value = 215;

        //sysvarIN[40].Name = "fixpos"
        //sysvarIN[40].value = 216

        sysvarIN[41].Name = "depth";
        sysvarIN[41].value = 217;

        sysvarIN[42].Name = "ypos";
        sysvarIN[42].value = 217;

        sysvarIN[43].Name = "daytime";
        sysvarIN[43].value = 218;

        sysvarIN[44].Name = "xpos";
        sysvarIN[44].value = 219;

        sysvarIN[45].Name = "kills";
        sysvarIN[45].value = 220;

        //sysvarIN[46].Name = "hitang"
        //sysvarIN[46].value = 221

        //sysvarIN[47].Name = "repro"
        //sysvarIN[47].value = 300

        //sysvarIN[48].Name = "mrepro"
        //sysvarIN[48].value = 301

        //sysvarIN[49].Name = "sexrepro"
        //sysvarIN[49].value = 302

        sysvarIN[50].Name = "nrg";
        sysvarIN[50].value = 310;

        sysvarIN[51].Name = "body";
        sysvarIN[51].value = 311;

        //sysvarIN[52].Name = "fdbody"
        //sysvarIN[52].value = 312

        //sysvarIN[53].Name = "strbody"
        //sysvarIN[53].value = 313

        //sysvarIN[54].Name = "setboy"
        //sysvarIN[54].value = 314

        sysvarIN[55].Name = "rdboy";
        sysvarIN[55].value = 315;

        //sysvarIN[56].Name = "tie"
        //sysvarIN[56].value = 330

        //sysvarIN[57].Name = "stifftie"
        //sysvarIN[57].value = 331

        //sysvarIN[58].Name = "mkvirus"
        //sysvarIN[58].value = 335

        sysvarIN[59].Name = "dnalen";
        sysvarIN[59].value = 336;

        sysvarIN[60].Name = "vtimer";
        sysvarIN[60].value = 337;

        //sysvarIN[61].Name = "vshoot"
        //sysvarIN[61].value = 338

        sysvarIN[62].Name = "genes";
        sysvarIN[62].value = 339;

        //sysvarIN[63].Name = "delgene"
        //sysvarIN[63].value = 340

        sysvarIN[64].Name = "thisgene";
        sysvarIN[64].value = 341;

        sysvarIN[65].Name = "sun";
        sysvarIN[65].value = 400;

        sysvarIN[66].Name = "trefbody";
        sysvarIN[66].value = 437;

        sysvarIN[67].Name = "trefxpos";
        sysvarIN[67].value = 438;

        sysvarIN[68].Name = "trefypos";
        sysvarIN[68].value = 439;

        sysvarIN[69].Name = "trefvelmysx";
        sysvarIN[69].value = 440;

        sysvarIN[70].Name = "trefvelmydx";
        sysvarIN[70].value = 441;

        sysvarIN[71].Name = "trefvelmydn";
        sysvarIN[71].value = 442;

        sysvarIN[72].Name = "trefvelmyup";
        sysvarIN[72].value = 443;

        sysvarIN[73].Name = "trefvelscalar";
        sysvarIN[73].value = 444;

        sysvarIN[74].Name = "trefvelyoursx";
        sysvarIN[74].value = 445;

        sysvarIN[75].Name = "trefvelyourdx";
        sysvarIN[75].value = 446;

        sysvarIN[76].Name = "trefvelyourdn";
        sysvarIN[76].value = 447;

        sysvarIN[77].Name = "trefvelyourup";
        sysvarIN[77].value = 448;

        sysvarIN[78].Name = "trefshell";
        sysvarIN[78].value = 449;

        sysvarIN[79].Name = "tieang";
        sysvarIN[79].value = 450;

        sysvarIN[80].Name = "tielen";
        sysvarIN[80].value = 451;

        //sysvarIN[81].Name = "tieloc"
        //sysvarIN[81].value = 452

        //sysvarIN[82].Name = "tieval"
        //sysvarIN[82].value = 453

        sysvarIN[83].Name = "tiepres";
        sysvarIN[83].value = 454;

        //sysvarIN[84].Name = "tienum"
        //sysvarIN[84].value = 455

        sysvarIN[85].Name = "trefup";
        sysvarIN[85].value = 456;

        sysvarIN[86].Name = "trefdn";
        sysvarIN[86].value = 457;

        sysvarIN[87].Name = "trefsx";
        sysvarIN[87].value = 458;

        sysvarIN[88].Name = "trefdx";
        sysvarIN[88].value = 459;

        sysvarIN[89].Name = "trefaimdx";
        sysvarIN[89].value = 460;

        sysvarIN[90].Name = "trefaimsx";
        sysvarIN[90].value = 461;

        sysvarIN[91].Name = "trefshoot";
        sysvarIN[91].value = 462;

        sysvarIN[92].Name = "trefeye";
        sysvarIN[92].value = 463;

        sysvarIN[93].Name = "trefnrg";
        sysvarIN[93].value = 464;

        sysvarIN[94].Name = "trefage";
        sysvarIN[94].value = 465;

        sysvarIN[95].Name = "numties";
        sysvarIN[95].value = 466;

        //sysvarIN[96].Name = "deltie"
        //sysvarIN[96].value = 467

        //sysvarIN[97].Name = "fixang"
        //sysvarIN[97].value = 468

        //sysvarIN[98].Name = "fixlen"
        //sysvarIN[98].value = 469

        sysvarIN[99].Name = "multi";
        sysvarIN[99].value = 470;

        //sysvarIN[100].Name = "readtie"
        //sysvarIN[100].value = 471

        sysvarIN[101].Name = "fertilized";
        sysvarIN[101].value = 303;

        sysvarIN[102].Name = "memval";
        sysvarIN[102].value = 473;

        //sysvarIN[103].Name = "memloc"
        //sysvarIN[103].value = 474

        sysvarIN[104].Name = "tmemval";
        sysvarIN[104].value = 475;

        //sysvarIN[105].Name = "tmemloc"
        //sysvarIN[105].value = 476

        sysvarIN[106].Name = "reffixed";
        sysvarIN[106].value = 477;

        sysvarIN[107].Name = "treffixed";
        sysvarIN[107].value = 478;

        sysvarIN[108].Name = "trefaim";
        sysvarIN[108].value = 479;

        sysvarIN[109].Name = "tieang1";
        sysvarIN[109].value = 480;

        sysvarIN[110].Name = "tieang2";
        sysvarIN[110].value = 481;

        sysvarIN[111].Name = "tieang3";
        sysvarIN[111].value = 482;

        sysvarIN[112].Name = "tieang4";
        sysvarIN[112].value = 483;

        sysvarIN[113].Name = "tielen1";
        sysvarIN[113].value = 484;

        sysvarIN[114].Name = "tielen2";
        sysvarIN[114].value = 485;

        sysvarIN[115].Name = "tielen3";
        sysvarIN[115].value = 486;

        sysvarIN[116].Name = "tielen4";
        sysvarIN[116].value = 487;

        sysvarIN[117].Name = "eye1";
        sysvarIN[117].value = 501;

        sysvarIN[118].Name = "eye2";
        sysvarIN[118].value = 502;

        sysvarIN[119].Name = "eye3";
        sysvarIN[119].value = 503;

        sysvarIN[120].Name = "eye4";
        sysvarIN[120].value = 504;

        sysvarIN[121].Name = "eye5";
        sysvarIN[121].value = 505;

        sysvarIN[122].Name = "eye6";
        sysvarIN[122].value = 506;

        sysvarIN[123].Name = "eye7";
        sysvarIN[123].value = 507;

        sysvarIN[124].Name = "eye8";
        sysvarIN[124].value = 508;

        sysvarIN[125].Name = "eye9";
        sysvarIN[125].value = 509;

        sysvarIN[126].Name = "refmulti";
        sysvarIN[126].value = 686;

        sysvarIN[127].Name = "refshell";
        sysvarIN[127].value = 687;

        sysvarIN[128].Name = "refbody";
        sysvarIN[128].value = 688;

        sysvarIN[129].Name = "refxpos";
        sysvarIN[129].value = 689;

        sysvarIN[130].Name = "refypos";
        sysvarIN[130].value = 690;

        sysvarIN[131].Name = "refvelscalar";
        sysvarIN[131].value = 695;

        sysvarIN[132].Name = "refvelsx";
        sysvarIN[132].value = 696;

        sysvarIN[133].Name = "refveldx";
        sysvarIN[133].value = 697;

        sysvarIN[134].Name = "refveldn";
        sysvarIN[134].value = 698;

        sysvarIN[135].Name = "refvel";
        sysvarIN[135].value = 699;

        sysvarIN[136].Name = "refvelup";
        sysvarIN[136].value = 699;

        sysvarIN[137].Name = "refup";
        sysvarIN[137].value = 701;

        sysvarIN[138].Name = "refdn";
        sysvarIN[138].value = 702;

        sysvarIN[139].Name = "refsx";
        sysvarIN[139].value = 703;

        sysvarIN[140].Name = "refdx";
        sysvarIN[140].value = 704;

        sysvarIN[141].Name = "refaimdx";
        sysvarIN[141].value = 705;

        sysvarIN[142].Name = "refaimsx";
        sysvarIN[142].value = 706;

        sysvarIN[143].Name = "refshoot";
        sysvarIN[143].value = 707;

        sysvarIN[144].Name = "refeye";
        sysvarIN[144].value = 708;

        sysvarIN[145].Name = "refnrg";
        sysvarIN[145].value = 709;

        sysvarIN[146].Name = "refage";
        sysvarIN[146].value = 710;

        sysvarIN[147].Name = "refaim";
        sysvarIN[147].value = 711;

        sysvarIN[148].Name = "reftie";
        sysvarIN[148].value = 712;

        sysvarIN[149].Name = "refpoison";
        sysvarIN[149].value = 713;

        sysvarIN[150].Name = "refvenom";
        sysvarIN[150].value = 714;

        sysvarIN[151].Name = "refkills";
        sysvarIN[151].value = 715;

        sysvarIN[152].Name = "myup";
        sysvarIN[152].value = 721;

        sysvarIN[153].Name = "mydn";
        sysvarIN[153].value = 722;

        sysvarIN[154].Name = "mysx";
        sysvarIN[154].value = 723;

        sysvarIN[155].Name = "mydx";
        sysvarIN[155].value = 724;

        sysvarIN[156].Name = "myaimdx";
        sysvarIN[156].value = 725;

        sysvarIN[157].Name = "myaimsx";
        sysvarIN[157].value = 726;

        sysvarIN[158].Name = "myshoot";
        sysvarIN[158].value = 727;

        sysvarIN[159].Name = "myeye";
        sysvarIN[159].value = 728;

        sysvarIN[160].Name = "myties";
        sysvarIN[160].value = 729;

        sysvarIN[161].Name = "mypoison";
        sysvarIN[161].value = 730;

        sysvarIN[162].Name = "myvenom";
        sysvarIN[162].value = 731;

        //sysvarIN[163].Name = "out1"
        //sysvarIN[163].value = 800

        //sysvarIN[164].Name = "out2"
        //sysvarIN[164].value = 801

        //sysvarIN[165].Name = "out3"
        //sysvarIN[165].value = 802

        //sysvarIN[166].Name = "out4"
        //sysvarIN[166].value = 803

        //sysvarIN[167].Name = "out5"
        //sysvarIN[167].value = 804

        //sysvarIN[168].Name = "out6"
        //sysvarIN[168].value = 805

        //sysvarIN[169].Name = "out7"
        //sysvarIN[169].value = 806

        //sysvarIN[170].Name = "out8"
        //sysvarIN[170].value = 807

        //sysvarIN[171].Name = "out9"
        //sysvarIN[171].value = 808

        //sysvarIN[172].Name = "out10"
        //sysvarIN[172].value = 809

        sysvarIN[173].Name = "in1";
        sysvarIN[173].value = 810;

        sysvarIN[174].Name = "in2";
        sysvarIN[174].value = 811;

        sysvarIN[175].Name = "in3";
        sysvarIN[175].value = 812;

        sysvarIN[176].Name = "in4";
        sysvarIN[176].value = 813;

        sysvarIN[177].Name = "in5";
        sysvarIN[177].value = 814;

        sysvarIN[178].Name = "in6";
        sysvarIN[178].value = 815;

        sysvarIN[179].Name = "in7";
        sysvarIN[179].value = 816;

        sysvarIN[180].Name = "in8";
        sysvarIN[180].value = 817;

        sysvarIN[181].Name = "in9";
        sysvarIN[181].value = 818;

        sysvarIN[182].Name = "in10";
        sysvarIN[182].value = 819;

        //sysvarIN[183].Name = "mkslime"
        //sysvarIN[183].value = 820

        sysvarIN[184].Name = "slime";
        sysvarIN[184].value = 821;

        //sysvarIN[185].Name = "mkshell"
        //sysvarIN[185].value = 822

        sysvarIN[186].Name = "shell";
        sysvarIN[186].value = 823;

        //sysvarIN[187].Name = "strvenom"
        //sysvarIN[187].value = 824

        //sysvarIN[188].Name = "mkvenom"
        //sysvarIN[188].value = 824

        sysvarIN[189].Name = "venom";
        sysvarIN[189].value = 825;

        //sysvarIN[190].Name = "strpoison"
        //sysvarIN[190].value = 826

        //sysvarIN[191].Name = "mkpoison"
        //sysvarIN[191].value = 826

        sysvarIN[192].Name = "poison";
        sysvarIN[192].value = 827;

        sysvarIN[193].Name = "waste";
        sysvarIN[193].value = 828;

        sysvarIN[194].Name = "pwaste";
        sysvarIN[194].value = 829;

        //sysvarIN[195].Name = "sharenrg"
        //sysvarIN[195].value = 830

        //sysvarIN[196].Name = "sharewaste"
        //sysvarIN[196].value = 831

        //sysvarIN[197].Name = "shareshell"
        //sysvarIN[197].value = 832

        //sysvarIN[198].Name = "shareslime"
        //sysvarIN[198].value = 833

        //sysvarIN[199].Name = "ploc"
        //sysvarIN[199].value = 834

        //sysvarIN[200].Name = "vloc"
        //sysvarIN[200].value = 835

        //sysvarIN[201].Name = "venval"
        //sysvarIN[201].value = 836

        sysvarIN[202].Name = "paralyzed";
        sysvarIN[202].value = 837;

        sysvarIN[203].Name = "poisoned";
        sysvarIN[203].value = 838;

        //sysvarIN[204].Name = "backshot"
        //sysvarIN[204].value = 900

        //sysvarIN[205].Name = "aimshoot"
        //sysvarIN[205].value = 901

        sysvarIN[206].Name = "eyef";
        sysvarIN[206].value = 510;

        //sysvarIN[207].Name = "focuseye"
        //sysvarIN[207].value = 511

        //sysvarIN[208].Name = "eye1dir"
        //sysvarIN[208].value = 521

        //sysvarIN[209].Name = "eye2dir"
        //sysvarIN[209].value = 522

        //sysvarIN[210].Name = "eye3dir"
        //sysvarIN[210].value = 523

        //sysvarIN[211].Name = "eye4dir"
        //sysvarIN[211].value = 524

        //sysvarIN[212].Name = "eye5dir"
        //sysvarIN[212].value = 525

        //sysvarIN[213].Name = "eye6dir"
        //sysvarIN[213].value = 526

        //sysvarIN[214].Name = "eye7dir"
        //sysvarIN[214].value = 527

        //sysvarIN[215].Name = "eye8dir"
        //sysvarIN[215].value = 528

        //sysvarIN[216].Name = "eye9dir"
        //sysvarIN[216].value = 529

        //sysvarIN[217].Name = "eye1width"
        //sysvarIN[217].value = 531

        //sysvarIN[218].Name = "eye2width"
        //sysvarIN[218].value = 532

        //sysvarIN[219].Name = "eye3width"
        //sysvarIN[219].value = 533

        //sysvarIN[220].Name = "eye4width"
        //sysvarIN[220].value = 534

        //sysvarIN[221].Name = "eye5width"
        //sysvarIN[221].value = 535

        //sysvarIN[222].Name = "eye6width"
        //sysvarIN[222].value = 536

        //sysvarIN[223].Name = "eye7width"
        //sysvarIN[223].value = 537

        //sysvarIN[224].Name = "eye8width"
        //sysvarIN[224].value = 538

        //sysvarIN[225].Name = "eye9width"
        //sysvarIN[225].value = 539

        sysvarIN[226].Name = "reftype";
        sysvarIN[226].value = 685;

        sysvarIN[227].Name = "totalbots";
        sysvarIN[227].value = 401;

        sysvarIN[228].Name = "totalmyspecies";
        sysvarIN[228].value = 402;

        //sysvarIN[229].Name = "tout1"
        //sysvarIN[229].value = 410

        //sysvarIN[230].Name = "tout2"
        //sysvarIN[230].value = 411

        //sysvarIN[231].Name = "tout3"
        //sysvarIN[231].value = 412

        //sysvarIN[232].Name = "tout4"
        //sysvarIN[232].value = 413

        //sysvarIN[233].Name = "tout5"
        //sysvarIN[233].value = 414

        //sysvarIN[234].Name = "tout6"
        //sysvarIN[234].value = 415

        //sysvarIN[235].Name = "tout7"
        //sysvarIN[235].value = 416

        //sysvarIN[236].Name = "tout8"
        //sysvarIN[236].value = 417

        //sysvarIN[237].Name = "tout9"
        //sysvarIN[237].value = 418

        //sysvarIN[238].Name = "tout10"
        //sysvarIN[238].value = 419

        sysvarIN[239].Name = "tin1";
        sysvarIN[239].value = 420;

        sysvarIN[240].Name = "tin2";
        sysvarIN[240].value = 421;

        sysvarIN[241].Name = "tin3";
        sysvarIN[241].value = 422;

        sysvarIN[242].Name = "tin4";
        sysvarIN[242].value = 423;

        sysvarIN[243].Name = "tin5";
        sysvarIN[243].value = 424;

        sysvarIN[244].Name = "tin6";
        sysvarIN[244].value = 425;

        sysvarIN[245].Name = "tin7";
        sysvarIN[245].value = 426;

        sysvarIN[246].Name = "tin8";
        sysvarIN[246].value = 427;

        sysvarIN[247].Name = "tin9";
        sysvarIN[247].value = 428;

        sysvarIN[248].Name = "tin10";
        sysvarIN[248].value = 429;

        //sysvarIN[249].Name = "pval"
        //sysvarIN[249].value = 839

        //Botsareus 8/14/2013 New chloroplast code
        sysvarIN[250].Name = "chlr";
        sysvarIN[250].value = 920;

        //sysvarIN[251].Name = "mkchlr"
        //sysvarIN[251].value = 921

        //sysvarIN[252].Name = "rmchlr"
        //sysvarIN[252].value = 922

        sysvarIN[253].Name = "light";
        sysvarIN[253].value = 923;

        sysvarIN[254].Name = "availability";
        sysvarIN[254].value = 923;

        //sysvarIN[255].Name = "sharechlr"
        //sysvarIN[255].value = 924

        //functional

        sysvarOUT[1].Name = "up";
        sysvarOUT[1].value = 1;

        sysvarOUT[2].Name = "dn";
        sysvarOUT[2].value = 2;

        sysvarOUT[3].Name = "sx";
        sysvarOUT[3].value = 3;

        sysvarOUT[4].Name = "dx";
        sysvarOUT[4].value = 4;

        sysvarOUT[5].Name = "aimdx";
        sysvarOUT[5].value = 5;

        sysvarOUT[6].Name = "aimright";
        sysvarOUT[6].value = 5;

        sysvarOUT[7].Name = "aimsx";
        sysvarOUT[7].value = 6;

        sysvarOUT[8].Name = "aimleft";
        sysvarOUT[8].value = 6;

        sysvarOUT[9].Name = "shoot";
        sysvarOUT[9].value = 7;

        sysvarOUT[10].Name = "shootval";
        sysvarOUT[10].value = 8;

        //sysvarOUT[11].Name = "robage"
        //sysvarOUT[11].value = 9

        //sysvarOUT[12].Name = "mass"
        //sysvarOUT[12].value = 10

        //sysvarOUT[13].Name = "maxvel"
        //sysvarOUT[13].value = 11

        //sysvarOUT[14].Name = "timer"
        //sysvarOUT[14].value = 12

        //sysvarOUT[15].Name = "aim"
        //sysvarOUT[15].value = 18

        sysvarOUT[16].Name = "setaim";
        sysvarOUT[16].value = 19;

        //sysvarOUT[17].Name = "bodgain"
        //sysvarOUT[17].value = 194

        //sysvarOUT[18].Name = "bodloss"
        //sysvarOUT[18].value = 195

        //sysvarOUT[19].Name = "velscalar"
        //sysvarOUT[19].value = 196

        //sysvarOUT[20].Name = "velsx"
        //sysvarOUT[20].value = 197

        //sysvarOUT[21].Name = "veldx"
        //sysvarOUT[21].value = 198

        //sysvarOUT[22].Name = "veldn"
        //sysvarOUT[22].value = 199

        //sysvarOUT[23].Name = "velup"
        //sysvarOUT[23].value = 200

        //sysvarOUT[24].Name = "vel"
        //sysvarOUT[24].value = 200

        //sysvarOUT[25].Name = "hit"
        //sysvarOUT[25].value = 201

        //sysvarOUT[26].Name = "shflav"
        //sysvarOUT[26].value = 202

        //sysvarOUT[27].Name = "pain"
        //sysvarOUT[27].value = 203

        //sysvarOUT[28].Name = "pleas"
        //sysvarOUT[28].value = 204

        //sysvarOUT[29].Name = "hitup"
        //sysvarOUT[29].value = 205

        //sysvarOUT[30].Name = "hitdn"
        //sysvarOUT[30].value = 206

        //sysvarOUT[31].Name = "hitdx"
        //sysvarOUT[31].value = 207

        //sysvarOUT[32].Name = "hitsx"
        //sysvarOUT[32].value = 208

        //sysvarOUT[33].Name = "shang"
        //sysvarOUT[33].value = 209

        //sysvarOUT[34].Name = "shup"
        //sysvarOUT[34].value = 210

        //sysvarOUT[35].Name = "shdn"
        //sysvarOUT[35].value = 211

        //sysvarOUT[36].Name = "shdx"
        //sysvarOUT[36].value = 212

        //sysvarOUT[37].Name = "shsx"
        //sysvarOUT[37].value = 213

        //sysvarOUT[38].Name = "edge"
        //sysvarOUT[38].value = 214

        //sysvarOUT[39].Name = "fixed"
        //sysvarOUT[39].value = 215

        sysvarOUT[40].Name = "fixpos";
        sysvarOUT[40].value = 216;

        //sysvarOUT[41].Name = "depth"
        //sysvarOUT[41].value = 217

        //sysvarOUT[42].Name = "ypos"
        //sysvarOUT[42].value = 217

        //sysvarOUT[43].Name = "daytime"
        //sysvarOUT[43].value = 218

        //sysvarOUT[44].Name = "xpos"
        //sysvarOUT[44].value = 219

        //sysvarOUT[45].Name = "kills"
        //sysvarOUT[45].value = 220

        //sysvarOUT[46].Name = "hitang"
        //sysvarOUT[46].value = 221

        sysvarOUT[47].Name = "repro";
        sysvarOUT[47].value = 300;

        sysvarOUT[48].Name = "mrepro";
        sysvarOUT[48].value = 301;

        sysvarOUT[49].Name = "sexrepro";
        sysvarOUT[49].value = 302;

        //sysvarOUT[50].Name = "nrg"
        //sysvarOUT[50].value = 310

        //sysvarOUT[51].Name = "body"
        //sysvarOUT[51].value = 311

        sysvarOUT[52].Name = "fdbody";
        sysvarOUT[52].value = 312;

        sysvarOUT[53].Name = "strbody";
        sysvarOUT[53].value = 313;

        sysvarOUT[54].Name = "setboy";
        sysvarOUT[54].value = 314;

        //sysvarOUT[55].Name = "rdboy"
        //sysvarOUT[55].value = 315

        sysvarOUT[56].Name = "tie";
        sysvarOUT[56].value = 330;

        sysvarOUT[57].Name = "stifftie";
        sysvarOUT[57].value = 331;

        sysvarOUT[58].Name = "mkvirus";
        sysvarOUT[58].value = 335;

        //sysvarOUT[59].Name = "dnalen"
        //sysvarOUT[59].value = 336

        //sysvarOUT[60].Name = "vtimer"
        //sysvarOUT[60].value = 337

        sysvarOUT[61].Name = "vshoot";
        sysvarOUT[61].value = 338;

        //sysvarOUT[62].Name = "genes"
        //sysvarOUT[62].value = 339

        sysvarOUT[63].Name = "delgene";
        sysvarOUT[63].value = 340;

        //sysvarOUT[64].Name = "thisgene"
        //sysvarOUT[64].value = 341

        //sysvarOUT[65].Name = "sun"
        //sysvarOUT[65].value = 400

        //sysvarOUT[66].Name = "trefbody"
        //sysvarOUT[66].value = 437

        //sysvarOUT[67].Name = "trefxpos"
        //sysvarOUT[67].value = 438

        //sysvarOUT[68].Name = "trefypos"
        //sysvarOUT[68].value = 439

        //sysvarOUT[69].Name = "trefvelmysx"
        //sysvarOUT[69].value = 440

        //sysvarOUT[70].Name = "trefvelmydx"
        //sysvarOUT[70].value = 441

        //sysvarOUT[71].Name = "trefvelmydn"
        //sysvarOUT[71].value = 442

        //sysvarOUT[72].Name = "trefvelmyup"
        //sysvarOUT[72].value = 443

        //sysvarOUT[73].Name = "trefvelscalar"
        //sysvarOUT[73].value = 444

        //sysvarOUT[74].Name = "trefvelyoursx"
        //sysvarOUT[74].value = 445

        //sysvarOUT[75].Name = "trefvelyourdx"
        //sysvarOUT[75].value = 446

        //sysvarOUT[76].Name = "trefvelyourdn"
        //sysvarOUT[76].value = 447

        //sysvarOUT[77].Name = "trefvelyourup"
        //sysvarOUT[77].value = 448

        //sysvarOUT[78].Name = "trefshell"
        //sysvarOUT[78].value = 449

        //sysvarOUT[79].Name = "tieang"
        //sysvarOUT[79].value = 450

        //sysvarOUT[80].Name = "tielen"
        //sysvarOUT[80].value = 451

        sysvarOUT[81].Name = "tieloc";
        sysvarOUT[81].value = 452;

        sysvarOUT[82].Name = "tieval";
        sysvarOUT[82].value = 453;

        //sysvarOUT[83].Name = "tiepres"
        //sysvarOUT[83].value = 454

        sysvarOUT[84].Name = "tienum";
        sysvarOUT[84].value = 455;

        //sysvarOUT[85].Name = "trefup"
        //sysvarOUT[85].value = 456

        //sysvarOUT[86].Name = "trefdn"
        //sysvarOUT[86].value = 457

        //sysvarOUT[87].Name = "trefsx"
        //sysvarOUT[87].value = 458

        //sysvarOUT[88].Name = "trefdx"
        //sysvarOUT[88].value = 459

        //sysvarOUT[89].Name = "trefaimdx"
        //sysvarOUT[89].value = 460

        //sysvarOUT[90].Name = "trefaimsx"
        //sysvarOUT[90].value = 461

        //sysvarOUT[91].Name = "trefshoot"
        //sysvarOUT[91].value = 462

        //sysvarOUT[92].Name = "trefeye"
        //sysvarOUT[92].value = 463

        //sysvarOUT[93].Name = "trefnrg"
        //sysvarOUT[93].value = 464

        //sysvarOUT[94].Name = "trefage"
        //sysvarOUT[94].value = 465

        //sysvarOUT[95].Name = "numties"
        //sysvarOUT[95].value = 466

        sysvarOUT[96].Name = "deltie";
        sysvarOUT[96].value = 467;

        sysvarOUT[97].Name = "fixang";
        sysvarOUT[97].value = 468;

        sysvarOUT[98].Name = "fixlen";
        sysvarOUT[98].value = 469;

        //sysvarOUT[99].Name = "multi"
        //sysvarOUT[99].value = 470

        sysvarOUT[100].Name = "readtie";
        sysvarOUT[100].value = 471;

        //sysvarOUT[101].Name = "fertilized"
        //sysvarOUT[101].value = 303

        //sysvarOUT[102].Name = "memval"
        //sysvarOUT[102].value = 473

        sysvarOUT[103].Name = "memloc";
        sysvarOUT[103].value = 474;

        //sysvarOUT[104].Name = "tmemval"
        //sysvarOUT[104].value = 475

        sysvarOUT[105].Name = "tmemloc";
        sysvarOUT[105].value = 476;

        //sysvarOUT[106].Name = "reffixed"
        //sysvarOUT[106].value = 477

        //sysvarOUT[107].Name = "treffixed"
        //sysvarOUT[107].value = 478

        //sysvarOUT[108].Name = "trefaim"
        //sysvarOUT[108].value = 479

        sysvarOUT[109].Name = "tieang1";
        sysvarOUT[109].value = 480;

        sysvarOUT[110].Name = "tieang2";
        sysvarOUT[110].value = 481;

        sysvarOUT[111].Name = "tieang3";
        sysvarOUT[111].value = 482;

        sysvarOUT[112].Name = "tieang4";
        sysvarOUT[112].value = 483;

        sysvarOUT[113].Name = "tielen1";
        sysvarOUT[113].value = 484;

        sysvarOUT[114].Name = "tielen2";
        sysvarOUT[114].value = 485;

        sysvarOUT[115].Name = "tielen3";
        sysvarOUT[115].value = 486;

        sysvarOUT[116].Name = "tielen4";
        sysvarOUT[116].value = 487;

        //sysvarOUT[117].Name = "eye1"
        //sysvarOUT[117].value = 501

        //sysvarOUT[118].Name = "eye2"
        //sysvarOUT[118].value = 502

        //sysvarOUT[119].Name = "eye3"
        //sysvarOUT[119].value = 503

        //sysvarOUT[120].Name = "eye4"
        //sysvarOUT[120].value = 504

        //sysvarOUT[121].Name = "eye5"
        //sysvarOUT[121].value = 505

        //sysvarOUT[122].Name = "eye6"
        //sysvarOUT[122].value = 506

        //sysvarOUT[123].Name = "eye7"
        //sysvarOUT[123].value = 507

        //sysvarOUT[124].Name = "eye8"
        //sysvarOUT[124].value = 508

        //sysvarOUT[125].Name = "eye9"
        //sysvarOUT[125].value = 509

        //sysvarOUT[126].Name = "refmulti"
        //sysvarOUT[126].value = 686

        //sysvarOUT[127].Name = "refshell"
        //sysvarOUT[127].value = 687

        //sysvarOUT[128].Name = "refbody"
        //sysvarOUT[128].value = 688

        //sysvarOUT[129].Name = "refxpos"
        //sysvarOUT[129].value = 689

        //sysvarOUT[130].Name = "refypos"
        //sysvarOUT[130].value = 690

        //sysvarOUT[131].Name = "refvelscalar"
        //sysvarOUT[131].value = 695

        //sysvarOUT[132].Name = "refvelsx"
        //sysvarOUT[132].value = 696

        //sysvarOUT[133].Name = "refveldx"
        //sysvarOUT[133].value = 697

        //sysvarOUT[134].Name = "refveldn"
        //sysvarOUT[134].value = 698

        //sysvarOUT[135].Name = "refvel"
        //sysvarOUT[135].value = 699

        //sysvarOUT[136].Name = "refvelup"
        //sysvarOUT[136].value = 699

        //sysvarOUT[137].Name = "refup"
        //sysvarOUT[137].value = 701

        //sysvarOUT[138].Name = "refdn"
        //sysvarOUT[138].value = 702

        //sysvarOUT[139].Name = "refsx"
        //sysvarOUT[139].value = 703

        //sysvarOUT[140].Name = "refdx"
        //sysvarOUT[140].value = 704

        //sysvarOUT[141].Name = "refaimdx"
        //sysvarOUT[141].value = 705

        //sysvarOUT[142].Name = "refaimsx"
        //sysvarOUT[142].value = 706

        //sysvarOUT[143].Name = "refshoot"
        //sysvarOUT[143].value = 707

        //sysvarOUT[144].Name = "refeye"
        //sysvarOUT[144].value = 708

        //sysvarOUT[145].Name = "refnrg"
        //sysvarOUT[145].value = 709

        //sysvarOUT[146].Name = "refage"
        //sysvarOUT[146].value = 710

        //sysvarOUT[147].Name = "refaim"
        //sysvarOUT[147].value = 711

        //sysvarOUT[148].Name = "reftie"
        //sysvarOUT[148].value = 712

        //sysvarOUT[149].Name = "refpoison"
        //sysvarOUT[149].value = 713

        //sysvarOUT[150].Name = "refvenom"
        //sysvarOUT[150].value = 714

        //sysvarOUT[151].Name = "refkills"
        //sysvarOUT[151].value = 715

        //sysvarOUT[152].Name = "myup"
        //sysvarOUT[152].value = 721

        //sysvarOUT[153].Name = "mydn"
        //sysvarOUT[153].value = 722

        //sysvarOUT[154].Name = "mysx"
        //sysvarOUT[154].value = 723

        //sysvarOUT[155].Name = "mydx"
        //sysvarOUT[155].value = 724

        //sysvarOUT[156].Name = "myaimdx"
        //sysvarOUT[156].value = 725

        //sysvarOUT[157].Name = "myaimsx"
        //sysvarOUT[157].value = 726

        //sysvarOUT[158].Name = "myshoot"
        //sysvarOUT[158].value = 727

        //sysvarOUT[159].Name = "myeye"
        //sysvarOUT[159].value = 728

        //sysvarOUT[160].Name = "myties"
        //sysvarOUT[160].value = 729

        //sysvarOUT[161].Name = "mypoison"
        //sysvarOUT[161].value = 730

        //sysvarOUT[162].Name = "myvenom"
        //sysvarOUT[162].value = 731

        sysvarOUT[163].Name = "out1";
        sysvarOUT[163].value = 800;

        sysvarOUT[164].Name = "out2";
        sysvarOUT[164].value = 801;

        sysvarOUT[165].Name = "out3";
        sysvarOUT[165].value = 802;

        sysvarOUT[166].Name = "out4";
        sysvarOUT[166].value = 803;

        sysvarOUT[167].Name = "out5";
        sysvarOUT[167].value = 804;

        sysvarOUT[168].Name = "out6";
        sysvarOUT[168].value = 805;

        sysvarOUT[169].Name = "out7";
        sysvarOUT[169].value = 806;

        sysvarOUT[170].Name = "out8";
        sysvarOUT[170].value = 807;

        sysvarOUT[171].Name = "out9";
        sysvarOUT[171].value = 808;

        sysvarOUT[172].Name = "out10";
        sysvarOUT[172].value = 809;

        //sysvarOUT[173].Name = "in1"
        //sysvarOUT[173].value = 810

        //sysvarOUT[174].Name = "in2"
        //sysvarOUT[174].value = 811

        //sysvarOUT[175].Name = "in3"
        //sysvarOUT[175].value = 812

        //sysvarOUT[176].Name = "in4"
        //sysvarOUT[176].value = 813

        //sysvarOUT[177].Name = "in5"
        //sysvarOUT[177].value = 814

        //sysvarOUT[178].Name = "in6"
        //sysvarOUT[178].value = 815

        //sysvarOUT[179].Name = "in7"
        //sysvarOUT[179].value = 816

        //sysvarOUT[180].Name = "in8"
        //sysvarOUT[180].value = 817

        //sysvarOUT[181].Name = "in9"
        //sysvarOUT[181].value = 818

        //sysvarOUT[182].Name = "in10"
        //sysvarOUT[182].value = 819

        sysvarOUT[183].Name = "mkslime";
        sysvarOUT[183].value = 820;

        //sysvarOUT[184].Name = "slime"
        //sysvarOUT[184].value = 821

        sysvarOUT[185].Name = "mkshell";
        sysvarOUT[185].value = 822;

        //sysvarOUT[186].Name = "shell"
        //sysvarOUT[186].value = 823

        sysvarOUT[187].Name = "strvenom";
        sysvarOUT[187].value = 824;

        sysvarOUT[188].Name = "mkvenom";
        sysvarOUT[188].value = 824;

        //sysvarOUT[189].Name = "venom"
        //sysvarOUT[189].value = 825

        sysvarOUT[190].Name = "strpoison";
        sysvarOUT[190].value = 826;

        sysvarOUT[191].Name = "mkpoison";
        sysvarOUT[191].value = 826;

        //sysvarOUT[192].Name = "poison"
        //sysvarOUT[192].value = 827

        //sysvarOUT[193].Name = "waste"
        //sysvarOUT[193].value = 828

        //sysvarOUT[194].Name = "pwaste"
        //sysvarOUT[194].value = 829

        sysvarOUT[195].Name = "sharenrg";
        sysvarOUT[195].value = 830;

        sysvarOUT[196].Name = "sharewaste";
        sysvarOUT[196].value = 831;

        sysvarOUT[197].Name = "shareshell";
        sysvarOUT[197].value = 832;

        sysvarOUT[198].Name = "shareslime";
        sysvarOUT[198].value = 833;

        sysvarOUT[199].Name = "ploc";
        sysvarOUT[199].value = 834;

        sysvarOUT[200].Name = "vloc";
        sysvarOUT[200].value = 835;

        sysvarOUT[201].Name = "venval";
        sysvarOUT[201].value = 836;

        //sysvarOUT[202].Name = "paralyzed"
        //sysvarOUT[202].value = 837

        //sysvarOUT[203].Name = "poisoned"
        //sysvarOUT[203].value = 838

        sysvarOUT[204].Name = "backshot";
        sysvarOUT[204].value = 900;

        sysvarOUT[205].Name = "aimshoot";
        sysvarOUT[205].value = 901;

        //sysvarOUT[206].Name = "eyef"
        //sysvarOUT[206].value = 510

        sysvarOUT[207].Name = "focuseye";
        sysvarOUT[207].value = 511;

        sysvarOUT[208].Name = "eye1dir";
        sysvarOUT[208].value = 521;

        sysvarOUT[209].Name = "eye2dir";
        sysvarOUT[209].value = 522;

        sysvarOUT[210].Name = "eye3dir";
        sysvarOUT[210].value = 523;

        sysvarOUT[211].Name = "eye4dir";
        sysvarOUT[211].value = 524;

        sysvarOUT[212].Name = "eye5dir";
        sysvarOUT[212].value = 525;

        sysvarOUT[213].Name = "eye6dir";
        sysvarOUT[213].value = 526;

        sysvarOUT[214].Name = "eye7dir";
        sysvarOUT[214].value = 527;

        sysvarOUT[215].Name = "eye8dir";
        sysvarOUT[215].value = 528;

        sysvarOUT[216].Name = "eye9dir";
        sysvarOUT[216].value = 529;

        sysvarOUT[217].Name = "eye1width";
        sysvarOUT[217].value = 531;

        sysvarOUT[218].Name = "eye2width";
        sysvarOUT[218].value = 532;

        sysvarOUT[219].Name = "eye3width";
        sysvarOUT[219].value = 533;

        sysvarOUT[220].Name = "eye4width";
        sysvarOUT[220].value = 534;

        sysvarOUT[221].Name = "eye5width";
        sysvarOUT[221].value = 535;

        sysvarOUT[222].Name = "eye6width";
        sysvarOUT[222].value = 536;

        sysvarOUT[223].Name = "eye7width";
        sysvarOUT[223].value = 537;

        sysvarOUT[224].Name = "eye8width";
        sysvarOUT[224].value = 538;

        sysvarOUT[225].Name = "eye9width";
        sysvarOUT[225].value = 539;

        //sysvarOUT[226].Name = "reftype"
        //sysvarOUT[226].value = 685

        //sysvarOUT[227].Name = "totalbots"
        //sysvarOUT[227].value = 401

        //sysvarOUT[228].Name = "totalmyspecies"
        //sysvarOUT[228].value = 402

        sysvarOUT[229].Name = "tout1";
        sysvarOUT[229].value = 410;

        sysvarOUT[230].Name = "tout2";
        sysvarOUT[230].value = 411;

        sysvarOUT[231].Name = "tout3";
        sysvarOUT[231].value = 412;

        sysvarOUT[232].Name = "tout4";
        sysvarOUT[232].value = 413;

        sysvarOUT[233].Name = "tout5";
        sysvarOUT[233].value = 414;

        sysvarOUT[234].Name = "tout6";
        sysvarOUT[234].value = 415;

        sysvarOUT[235].Name = "tout7";
        sysvarOUT[235].value = 416;

        sysvarOUT[236].Name = "tout8";
        sysvarOUT[236].value = 417;

        sysvarOUT[237].Name = "tout9";
        sysvarOUT[237].value = 418;

        sysvarOUT[238].Name = "tout10";
        sysvarOUT[238].value = 419;

        //sysvarOUT[239].Name = "tin1"
        //sysvarOUT[239].value = 420

        //sysvarOUT[240].Name = "tin2"
        //sysvarOUT[240].value = 421

        //sysvarOUT[241].Name = "tin3"
        //sysvarOUT[241].value = 422

        //sysvarOUT[242].Name = "tin4"
        //sysvarOUT[242].value = 423

        //sysvarOUT[243].Name = "tin5"
        //sysvarOUT[243].value = 424

        //sysvarOUT[244].Name = "tin6"
        //sysvarOUT[244].value = 425

        //sysvarOUT[245].Name = "tin7"
        //sysvarOUT[245].value = 426

        //sysvarOUT[246].Name = "tin8"
        //sysvarOUT[246].value = 427

        //sysvarOUT[247].Name = "tin9"
        //sysvarOUT[247].value = 428

        //sysvarOUT[248].Name = "tin10"
        //sysvarOUT[248].value = 429

        sysvarOUT[249].Name = "pval";
        sysvarOUT[249].value = 839;

        //Botsareus 8/14/2013 New chloroplast code
        //sysvarOUT[250].Name = "chlr"
        //sysvarOUT[250].value = 920

        sysvarOUT[251].Name = "mkchlr";
        sysvarOUT[251].value = 921;

        sysvarOUT[252].Name = "rmchlr";
        sysvarOUT[252].value = 922;

        //sysvarOUT[253].Name = "light"
        //sysvarOUT[253].value = 923

        //sysvarOUT[254].Name = "availability"
        //sysvarOUT[254].value = 923

        sysvarOUT[255].Name = "sharechlr";
        sysvarOUT[255].value = 924;
    }

    public static void Parse(ref string Command, block bp, int n = 0, bool converttosysvar = true)
    {
        var detok = IIf(Command == "", true, false);

        if (detok)
        {
            switch (bp.tipo)
            {
                case 0:  //number
                    Command = converttosysvar ? SysvarDetok(bp.value, n) : bp.value.ToString();

                    break;//*.number
                case 1:
                    Command = "*" + SysvarDetok(bp.value, n);
                    break;//basic commands
                case 2:
                    Command = BasicCommandDetok(bp.value);
                    break;//advanced commands
                case 3:
                    Command = AdvancedCommandDetok(bp.value);
                    break;//bit commands
                case 4:
                    Command = BitwiseCommandDetok(bp.value);
                    break;//conditions
                case 5:
                    Command = ConditionsDetok(bp.value);
                    break;//logic
                case 6:
                    Command = LogicDetok(bp.value);
                    break;//stores
                case 7:
                    Command = StoresDetok(bp.value);
                    break;

                case 8:
                    //nothing
                    break;

                case 9:
                    Command = FlowDetok(bp.value);
                    break;

                case 10:
                    Command = MasterFlowDetok(bp.value);
                    break;
            }
        }
        else
        {
            bp.value = 0;

            //Botsareus 11/27/2013 Automatically lower case var
            if (bp.value == 0)
            {
                bp = BasicCommandTok(LCase(Command));
            }
            if (bp.value == 0)
            {
                bp = AdvancedCommandTok(LCase(Command));
            }
            if (bp.value == 0)
            {
                bp = BitwiseCommandTok(LCase(Command));
            }
            if (bp.value == 0)
            {
                bp = ConditionsTok(LCase(Command));
            }
            if (bp.value == 0)
            {
                bp = LogicTok(LCase(Command));
            }
            if (bp.value == 0)
            {
                bp = StoresTok(LCase(Command));
            }
            if (bp.value == 0)
            {
                bp = FlowTok(LCase(Command));
            }
            if (bp.value == 0)
            {
                bp = MasterFlowTok(LCase(Command));
            }
            if (bp.value == 0 & Left(Command, 1) == "*")
            {
                bp.tipo = 1;
                bp.value = SysvarTok(Right(Command, Len(Command) - 1), n);
            }
            else if (bp.value == 0)
            {
                bp.tipo = 0;
                bp.value = SysvarTok(Command, n);
            }
        }
    }

    public static string SaveRobHeader(ref int n)
    {
        var totmut = rob[n].Mutations + rob[n].OldMutations;
        if (totmut > 2000000000)
        {
            totmut = 2000000000; //Overflow prevention
        }

        return "'#generation: " + CStr(rob[n].generation) + vbCrLf + "'#mutations: " + CStr(totmut) + vbCrLf;
    }

    public static string SysvarDetok(int n, int robn)
    {
        var t = 0;
        var SysvarDetok = n.ToString();

        while (sysvar[t + 1].value != 0)
        {
            if (sysvar[t + 1].value == n)
            { //Botsareus 3/20/2016 Bug fix, do not modulate sysvars
                SysvarDetok = "." + sysvar[t + 1].Name;
            }
            t++;
        }

        if (savingtofile)
            return SysvarDetok;

        if (robn > 0 & n != 0)
        { // EricL 4/17/2006 Added n<>0 to address parse bug when DNA contains 0 store 'Botsareus 9/7/2013 modified for high range fix
            for (t = 1; t < UBound(rob[robn].vars); t++)
            {
                if (rob[robn].vars[t].value == n)
                { //Botsareus 3/20/2016 Bug fix, do not modulate sysvars
                    SysvarDetok = "." + rob[robn].vars[t].Name;
                }
            }
        }

        return SysvarDetok;
    }

    public static int SysvarTok(string a, int n)
    {
        var SysvarTok = 0;

        if (Left(a, 1) == ".")
        {
            a = Right(a, Len(a) - 1);

            for (var t = 1; t < UBound(sysvar); t++)
            {
                if (LCase(sysvar[t].Name) == LCase(a))
                {
                    SysvarTok = sysvar[t].value;
                }
            }

            if (n > 0)
            {
                for (var t = 1; t < UBound(rob[n].vars); t++)
                {
                    if (rob[n].vars[t].Name == a)
                    {
                        SysvarTok = rob[n].vars[t].value;
                    }
                }
            }
        }
        else
        {
            SysvarTok = (int)Val(a);
        }
        return SysvarTok;
    }

    public static string TipoDetok(int tipo)
    {
        var TipoDetok = "";
        switch (tipo)
        {
            case 0:
                TipoDetok = "number";
                break;

            case 1:
                TipoDetok = "*number";
                break;

            case 2:
                TipoDetok = "basic command";
                break;

            case 3:
                TipoDetok = "advanced command";
                break;

            case 4:
                TipoDetok = "bit command";
                break;

            case 5:
                TipoDetok = "condition";
                break;

            case 6:
                TipoDetok = "logic operator";
                break;

            case 7:
                TipoDetok = "store command";
                break;

            case 9:
                TipoDetok = "flow command";
                break;
        }
        return TipoDetok;
    }

    private static string AdvancedCommandDetok(int n)
    {
        var AdvancedCommandDetok = "";

        switch (n)
        {
            case 1:
                AdvancedCommandDetok = "angle";
                break;

            case 2:
                AdvancedCommandDetok = "dist";
                break;

            case 3:
                AdvancedCommandDetok = "ceil";
                break;

            case 4:
                AdvancedCommandDetok = "floor";
                break;

            case 5:
                AdvancedCommandDetok = "sqr";
                break;

            case 6:
                AdvancedCommandDetok = "pow";
                break;

            case 7:
                AdvancedCommandDetok = "pyth";
                break;

            case 8:
                AdvancedCommandDetok = "anglecmp";
                break;

            case 9:
                AdvancedCommandDetok = "root";
                break;

            case 10:
                AdvancedCommandDetok = "logx";
                break;

            case 11:
                AdvancedCommandDetok = "sin";
                break;

            case 12:
                AdvancedCommandDetok = "cos";
                break;

            case 13:
                if (!ismutating)
                {
                    AdvancedCommandDetok = "debugint"; //Botsareus 1/31/2013 the new debugint command
                }
                break;

            case 14:
                if (!ismutating)
                {
                    AdvancedCommandDetok = "debugbool"; //Botsareus 1/31/2013 the new debugbool command
                }
                break;
        }
        return AdvancedCommandDetok;
    }

    private static block AdvancedCommandTok(string s)
    {
        var AdvancedCommandTok = new block
        {
            value = 0,
            tipo = 3
        };

        switch (s)
        {
            case "angle":
                AdvancedCommandTok.value = 1;
                break;

            case "dist":
                AdvancedCommandTok.value = 2;
                break;

            case "ceil":
                AdvancedCommandTok.value = 3;
                break;

            case "floor":
                AdvancedCommandTok.value = 4;
                break;

            case "sqr":
                AdvancedCommandTok.value = 5;
                break;

            case "pow":
                AdvancedCommandTok.value = 6;
                break;

            case "pyth":
                AdvancedCommandTok.value = 7;
                break;

            case "anglecmp":
                AdvancedCommandTok.value = 8;
                break;

            case "root":
                AdvancedCommandTok.value = 9;
                break;

            case "logx":
                AdvancedCommandTok.value = 10;
                break;

            case "sin":
                AdvancedCommandTok.value = 11;
                break;

            case "cos":
                AdvancedCommandTok.value = 12;
                break;//Botsareus 1/31/2013 the new debugint command
            case "debugint":
                if (!ismutating)
                {
                    AdvancedCommandTok.value = 13;
                }
                break;//Botsareus 1/31/2013 the new debugbool command
            case "debugbool":
                if (!ismutating)
                {
                    AdvancedCommandTok.value = 14;
                }
                break;
        }
        return AdvancedCommandTok;
    }

    private static string BasicCommandDetok(int n)
    {
        var BasicCommandDetok = "";

        switch (n)
        {
            case 1:
                BasicCommandDetok = "add";
                break;

            case 2:
                BasicCommandDetok = "sub";
                break;

            case 3:
                BasicCommandDetok = "mult";
                break;

            case 4:
                BasicCommandDetok = "div";
                break;

            case 5:
                BasicCommandDetok = "rnd";
                break;

            case 6:
                BasicCommandDetok = "*";
                break;

            case 7:
                BasicCommandDetok = "mod";
                break;

            case 8:
                BasicCommandDetok = "sgn";
                break;

            case 9:
                BasicCommandDetok = "abs";
                break;

            case 10:
                BasicCommandDetok = "dup";
                break;

            case 11:
                BasicCommandDetok = "drop";
                break;

            case 12:
                BasicCommandDetok = "clear";
                break;

            case 13:
                BasicCommandDetok = "swap";
                break;

            case 14:
                BasicCommandDetok = "over";
                break;
        }
        return BasicCommandDetok;
    }

    private static block BasicCommandTok(string s)
    {
        var BasicCommandTok = new block
        {
            value = 0,
            tipo = 2
        };

        switch (s)
        {
            case "add":
                BasicCommandTok.value = 1;
                break;

            case "sub":
                BasicCommandTok.value = 2;
                break;

            case "mult":
                BasicCommandTok.value = 3;
                break;

            case "div":
                BasicCommandTok.value = 4;
                break;

            case "rnd":
                BasicCommandTok.value = 5;
                break;

            case "*":
                BasicCommandTok.value = 6;
                break;

            case "mod":
                BasicCommandTok.value = 7;
                break;

            case "sgn":
                BasicCommandTok.value = 8;
                break;

            case "abs":
                BasicCommandTok.value = 9;
                break;

            case "dup":
                BasicCommandTok.value = 10;
                break;

            case "dupint":
                BasicCommandTok.value = 10;
                break;

            case "drop":
                BasicCommandTok.value = 11;
                break;

            case "dropint":
                BasicCommandTok.value = 11;
                break;

            case "clear":
                BasicCommandTok.value = 12;
                break;

            case "clearint":
                BasicCommandTok.value = 12;
                break;

            case "swap":
                BasicCommandTok.value = 13;
                break;

            case "swapint":
                BasicCommandTok.value = 13;
                break;

            case "over":
                BasicCommandTok.value = 14;
                break;

            case "overint":
                BasicCommandTok.value = 14;
                break;
        }
        return BasicCommandTok;
    }

    private static string BitwiseCommandDetok(int n)
    {
        var BitwiseCommandDetok = "";

        switch (n)
        {
            case 1:
                BitwiseCommandDetok = "~"; //bitwise compliment
                break;

            case 2:
                BitwiseCommandDetok = "&"; //bitwise AND
                break;

            case 3:
                BitwiseCommandDetok = "|"; //bitwise OR
                break;

            case 4:
                BitwiseCommandDetok = "^"; //bitwise XOR
                break;

            case 5:
                BitwiseCommandDetok = "++";
                break;

            case 6:
                BitwiseCommandDetok = "--";
                break;

            case 7:
                BitwiseCommandDetok = "-";
                break;

            case 8:
                BitwiseCommandDetok = "<<"; //bit shift left
                break;

            case 9:
                BitwiseCommandDetok = ">>"; //bit shift right
                break;
        }
        return BitwiseCommandDetok;
    }

    private static block BitwiseCommandTok(string s)
    {
        var BitwiseCommandTok = new block
        {
            value = 0,
            tipo = 4
        };

        switch (s)
        {
            case "~":
                BitwiseCommandTok.value = 1;
                break;

            case "&":
                BitwiseCommandTok.value = 2;
                break;

            case "|":
                BitwiseCommandTok.value = 3;
                break;

            case "^":
                BitwiseCommandTok.value = 4;
                break;

            case "++":
                BitwiseCommandTok.value = 5;
                break;

            case "--":
                BitwiseCommandTok.value = 6;
                break;

            case "-":
                BitwiseCommandTok.value = 7;
                break;

            case "<<":
                BitwiseCommandTok.value = 8;
                break;

            case ">>":
                BitwiseCommandTok.value = 9;
                break;
        }
        return BitwiseCommandTok;
    }

    private static string ConditionsDetok(int n)
    {
        var ConditionsDetok = "";

        switch (n)
        {
            case 1:
                ConditionsDetok = "<";
                break;

            case 2:
                ConditionsDetok = ">";
                break;

            case 3:
                ConditionsDetok = "=";
                break;

            case 4:
                ConditionsDetok = "!=";
                break;

            case 5:
                ConditionsDetok = "%=";
                break;

            case 6:
                ConditionsDetok = "!%=";
                break;

            case 7:
                ConditionsDetok = "~=";
                break;

            case 8:
                ConditionsDetok = "!~=";
                break;

            case 9:
                ConditionsDetok = ">=";
                break;

            case 10:
                ConditionsDetok = "<=";
                break;
        }
        return ConditionsDetok;
    }

    private static block ConditionsTok(string s)
    {
        var ConditionsTok = new block
        {
            value = 0,
            tipo = 5
        };

        switch (s)
        {
            case "<":
                ConditionsTok.value = 1;
                break;

            case ">":
                ConditionsTok.value = 2;
                break;

            case "=":
                ConditionsTok.value = 3;
                break;

            case "!=":
                ConditionsTok.value = 4;
                break;

            case "%=":
                ConditionsTok.value = 5;
                break;

            case "!%=":
                ConditionsTok.value = 6;
                break;

            case "~=":
                ConditionsTok.value = 7;
                break;

            case "!~=":
                ConditionsTok.value = 8;
                break;

            case ">=":
                ConditionsTok.value = 9;
                break;

            case "<=":
                ConditionsTok.value = 10;
                break;
        }
        return ConditionsTok;
    }

    private static string FlowDetok(int n)
    {
        var FlowDetok = "";

        switch (n)
        {
            case 1:
                FlowDetok = "cond";
                break;

            case 2:
                FlowDetok = "start";
                break;

            case 3:
                FlowDetok = "else";
                break;

            case 4:
                FlowDetok = "stop";
                //   Case 5
                //     FlowDetok = "cross"
                break;
        }
        return FlowDetok;
    }

    private static block FlowTok(string s)
    {
        var FlowTok = new block
        {
            value = 0,
            tipo = 9
        };

        switch (s)
        {
            case "cond":
                FlowTok.value = 1;
                break;

            case "start":
                FlowTok.value = 2;
                break;

            case "else":
                FlowTok.value = 3;
                break;

            case "stop":
                FlowTok.value = 4;
                break;
        }
        return FlowTok;
    }

    private static void getvals(int n, string a, string hold)
    {
        // here we divide the string in its two parts
        // parameter's name and value, which shall be separated by ':'
        var Name = Left(a, InStr(a, ":") - 1);
        var value = Right(a, Len(a) - InStr(a, ":"));
        Name = Trim(Name);
        Name = Mid(Name, 3);
        value = Trim(value);

        // Botsareus 10/8/2015
        // here we take the appropriate action
        // depending on the parameter's name
        // we record it in the rob structure

        if (Name == "generation")
            rob[n].generation = (int)Val(value);

        if (Name == "mutations")
            rob[n].OldMutations = (int)Val(value);

        if (Name == "tag")
            rob[n].tag = Left(replacechars(value), 45);

        if (Name == "hash")
        {
            var value2 = Hash(hold, 20);
            if (value2 != value)
            {
                rob[n].generation = 0;
                rob[n].OldMutations = 0;
            }
        }
    }

    private static string LogicDetok(int n)
    {
        var LogicDetok = "";

        switch (n)
        {
            case 1:
                LogicDetok = "and";
                break;

            case 2:
                LogicDetok = "or";
                break;

            case 3:
                LogicDetok = "xor";
                break;

            case 4:
                LogicDetok = "not";
                break;

            case 5:
                LogicDetok = "true";
                break;

            case 6:
                LogicDetok = "false";
                break;

            case 7:
                LogicDetok = "dropbool";
                break;

            case 8:
                LogicDetok = "clearbool";
                break;

            case 9:
                LogicDetok = "dupbool";
                break;

            case 10:
                LogicDetok = "swapbool";
                break;

            case 11:
                LogicDetok = "overbool";
                break;
        }
        return LogicDetok;
    }

    private static block LogicTok(string s)
    {
        var LogicTok = new block
        {
            value = 0,
            tipo = 6
        };

        switch (s)
        {
            case "and":
                LogicTok.value = 1;
                break;

            case "or":
                LogicTok.value = 2;
                break;

            case "xor":
                LogicTok.value = 3;
                break;

            case "not":
                LogicTok.value = 4;
                break;

            case "true":
                LogicTok.value = 5;
                break;

            case "false":
                LogicTok.value = 6;
                break;

            case "dropbool":
                LogicTok.value = 7;
                break;

            case "clearbool":
                LogicTok.value = 8;
                break;

            case "dupbool":
                LogicTok.value = 9;
                break;

            case "swapbool":
                LogicTok.value = 10;
                break;

            case "overbool":
                LogicTok.value = 11;
                break;
        }
        return LogicTok;
    }

    private static string MasterFlowDetok(int n)
    {
        var MasterFlowDetok = "";

        switch (n)
        {
            case 1:
                MasterFlowDetok = "end";
                break;
        }
        return MasterFlowDetok;
    }

    private static block MasterFlowTok(string s)
    {
        var MasterFlowTok = new block
        {
            tipo = 10
        };

        switch (s)
        {
            case "end":
                MasterFlowTok.value = 1;
                break;
        }

        return MasterFlowTok;
    }

    private static string StoresDetok(int n)
    {
        var StoresDetok = "";

        switch (n)
        {
            case 1:
                StoresDetok = "store";
                break;

            case 2:
                StoresDetok = "inc";
                break;

            case 3:
                StoresDetok = "dec";
                break;//Botsareus 9/7/2013 New dna commands
            case 4:
                StoresDetok = "addstore";
                break;

            case 5:
                StoresDetok = "substore";
                break;

            case 6:
                StoresDetok = "multstore";
                break;

            case 7:
                StoresDetok = "divstore";
                break;

            case 8:
                StoresDetok = "ceilstore";
                break;

            case 9:
                StoresDetok = "floorstore";
                break;

            case 10:
                StoresDetok = "rndstore";
                break;

            case 11:
                StoresDetok = "sgnstore";
                break;

            case 12:
                StoresDetok = "absstore";
                break;

            case 13:
                StoresDetok = "sqrstore";
                break;

            case 14:
                StoresDetok = "negstore";
                break;
        }
        return StoresDetok;
    }

    private static block StoresTok(string s)
    {
        block StoresTok = null;
        StoresTok.value = 0;
        StoresTok.tipo = 7;
        switch (s)
        {
            case "store":
                StoresTok.value = 1;
                break;

            case "inc":
                StoresTok.value = 2;
                break;

            case "dec":
                StoresTok.value = 3;
                break;//Botsareus 9/7/2013 New dna commands
            case "addstore":
                StoresTok.value = 4;
                break;

            case "substore":
                StoresTok.value = 5;
                break;

            case "multstore":
                StoresTok.value = 6;
                break;

            case "divstore":
                StoresTok.value = 7;
                break;

            case "ceilstore":
                StoresTok.value = 8;
                break;

            case "floorstore":
                StoresTok.value = 9;
                break;

            case "rndstore":
                StoresTok.value = 10;
                break;

            case "sgnstore":
                StoresTok.value = 11;
                break;

            case "absstore":
                StoresTok.value = 12;
                break;

            case "sqrstore":
                StoresTok.value = 13;
                break;

            case "negstore":
                StoresTok.value = 14;
                break;
        }
        return StoresTok;
    }

    /*
    ' embryo of a new feature, should allow recording
    ' in dna files info such robot colour, generation, mutations etc
    */
    /*
    ' calculates the hash function, i.e. simply a string of length f
    ' which is unlikely to be generated by a different input s
    */
    /*
    ' saves a robot's header informations; can be padded with any
    ' other information, such as color, base energy, etc.
    */
    /*
    ' loads the sysvars.txt file
    */
}
