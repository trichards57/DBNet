using VB6 = Microsoft.VisualBasic.Compatibility.VB6;
using System.Runtime.InteropServices;
using static VBExtension;
using static VBConstants;
using Microsoft.VisualBasic;
using System;
using System.Windows;
using System.Windows.Controls;
using static System.DateTime;
using static System.Math;
using static Microsoft.VisualBasic.Globals;
using static Microsoft.VisualBasic.Collection;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.DateAndTime;
using static Microsoft.VisualBasic.ErrObject;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Financial;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.VBMath;
using System.Collections.Generic;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ColorConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.DrawStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.FillStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.GlobalModule;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.Printer;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterCollection;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterObjectConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ScaleModeConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.SystemColorConstants;
using ADODB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DBNet.Forms;
using static stringops;
using static varspecie;
using static stayontop;
using static localizzazione;
using static SimOptModule;
using static Common;
using static Flex;
using static Robots;
using static Ties;
using static Shots_Module;
using static Globals;
using static Physics;
using static F1Mode;
using static DNAExecution;
using static Vegs;
using static Senses;
using static Multibots;
using static HDRoutines;
using static Scripts;
using static Database;
using static Buckets_Module;
using static NeoMutations;
using static Master;
using static DNAManipulations;
using static DNATokenizing;
using static Bitwise;
using static Obstacles;
using static Teleport;
using static IntOpts;
using static stuffcolors;
using static Evo;
using static DBNet.Forms.MDIForm1;
using static DBNet.Forms.datirob;
using static DBNet.Forms.InfoForm;
using static DBNet.Forms.ColorForm;
using static DBNet.Forms.parentele;
using static DBNet.Forms.Consoleform;
using static DBNet.Forms.frmAbout;
using static DBNet.Forms.optionsform;
using static DBNet.Forms.NetEvent;
using static DBNet.Forms.grafico;
using static DBNet.Forms.ActivForm;
using static DBNet.Forms.Form1;
using static DBNet.Forms.Contest_Form;
using static DBNet.Forms.DNA_Help;
using static DBNet.Forms.MutationsProbability;
using static DBNet.Forms.PhysicsOptions;
using static DBNet.Forms.CostsForm;
using static DBNet.Forms.EnergyForm;
using static DBNet.Forms.ObstacleForm;
using static DBNet.Forms.TeleportForm;
using static DBNet.Forms.frmGset;
using static DBNet.Forms.frmMonitorSet;
using static DBNet.Forms.frmPBMode;
using static DBNet.Forms.frmRestriOps;
using static DBNet.Forms.frmEYE;
using static DBNet.Forms.frmFirstTimeInfo;


static class Physics {
//                 P H Y S I C S
//Important
// Option Explicit
// experimental object: used to store a grid of environmental
// features
public static bool envgridpres = false;
public static decimal nlink = 0;// links physics constants
public static decimal plink = 0;
public static decimal klink = 0;
public static decimal mlink = 0;
public const decimal smudgefactor = 50; //just to keep the bots more likely to stay visible
public static bool boylabldisp = false;
public static decimal BouyancyScaling = 0;


public static dynamic NetForces(ref int n) {
  dynamic NetForces = null;
  decimal mag = 0;

  int sign = 0;

  vector staticV = null;


//The physics engine breaks apart if bot masses are less than about .1


  if (Abs(rob[n].vel.X) < 0.0000001m) {
    rob[n].vel.X = 0; //Prevents underflow errors down the line
  }
  if (Abs(rob[n].vel.Y) < 0.0000001m) {
    rob[n].vel.Y = 0; //Prevents underflow erros down the line
  }
  PlanetEaters(n);
  FrictionForces(n);
  SphereDragForces(n);
//TieDragForces n Botsareus 6/18/2016 Disabled for not doing anything
  BrownianForces(n);
//BouyancyForces n  BouyancyForces are no longer needed since boy is proportional to y gravity
  GravityForces(n);
  VoluntaryForces(n);

  return NetForces;
}

public static void CalcMass(ref int n) {
  dynamic _WithVar_2428;
  _WithVar_2428 = rob[n];
    _WithVar_2428.mass = (_WithVar_2428.body / 1000) + (_WithVar_2428.shell / 200) + (_WithVar_2428.chloroplasts / 32000) * 31680; //Panda 8/14/2013 set value for mass 'Botsareus 8/16/2014 Vegys get energy liner
//If .mass < 0.1 Then .mass = 0.1 'stops the Euler integration from wigging out too badly.
    if (_WithVar_2428.mass < 1) {
      _WithVar_2428.mass = 1; //stops the Euler integration from wigging out too badly.
    }
    if (_WithVar_2428.mass > 32000) {
      _WithVar_2428.mass = 32000;
    }

}

public static void AddedMass(ref int n) {
//added mass is a simple enough concept.
//To move an object through a liquid, you must also move
//that liquid out of the way.

  const decimal fourthirdspi = 4.18879m;

  const decimal AddedMassCoefficientForASphere = 0.5m;

  dynamic _WithVar_1840;
  _WithVar_1840 = rob[n];
    if (SimOpts.Density == 0) {
      _WithVar_1840.AddedMass = 0;
    } else {
      _WithVar_1840.AddedMass = AddedMassCoefficientForASphere * SimOpts.Density * fourthirdspi * _WithVar_1840.radius * _WithVar_1840.radius * _WithVar_1840.radius;
    }
}

public static void FrictionForces(ref int n) {
  decimal Impulse = 0;

  decimal mag = 0;

  decimal ZGrav = 0;


  dynamic _WithVar_8326;
  _WithVar_8326 = rob[n];

    if (SimOpts.Zgravity == 0) {
goto getout;
    }

    ZGrav = SimOpts.Zgravity;


    _WithVar_8326.ImpulseStatic = CSng(_WithVar_8326.mass * ZGrav * SimOpts.CoefficientStatic); // * 1 cycle (timestep = 1)

    Impulse = CSng(_WithVar_8326.mass * ZGrav * SimOpts.CoefficientKinetic); // * 1 cycle (timestep = 1)

//Here we calculate the reduction in angular momentum due to friction
    if (Abs(rob[n].ma) > 0) {
      if (Impulse < 48) {
        rob[n].ma = rob[n].ma * (48 - Impulse) / 48;
      } else {
        rob[n].ma = 0;
      }
      if (Abs(rob[n].ma) < 0.0000001m) {
        rob[n].ma = 0;
      }
    }

    if (Impulse > VectorMagnitude(ref _WithVar_8326.vel)) {
      Impulse = VectorMagnitude(ref _WithVar_8326.vel); // EricL 5/3/2006 Added to insure friction only counteracts
    }

    if (Impulse < 0.0000001m) {
goto getout; // Prevents the accumulation of very very low velocity in sims without density
    }

//EricL 5/7/2006 Changed to operate directly on velocity
    _WithVar_8326.vel = VectorSub(ref _WithVar_8326.vel, ref VectorScalar(ref VectorUnit(ref _WithVar_8326.vel), ref Impulse)); //kinetic friction points in opposite direction of velocity
getout:
}

public static void BrownianForces(ref int n) {
  if (SimOpts.PhysBrown == 0) {
goto getout;
  }
  decimal Impulse = 0;

  decimal RandomAngle = 0;


  Impulse = SimOpts.PhysBrown * 0.5m * rndy();

  RandomAngle = rndy() * 2 * PI;
  rob[n].ImpulseInd = VectorAdd(ref rob[n].ImpulseInd, ref VectorSet(Cos(RandomAngle) * Impulse, Sin(RandomAngle) * Impulse));
  rob[n].ma = rob[n].ma + (Impulse / 100) * (rndy() - 0.5m); // turning motion due to brownian motion

getout:
}

public static void SphereDragForces(ref int n) { //for bots
  decimal Impulse = 0;

  vector ImpulseVector = null;

  decimal mag = 0;


//No Drag if no velocity or no density
  if ((rob[n].vel.X == 0& rob[n].vel.Y == 0) || SimOpts.Density == 0) {
goto getout;
  }

//Here we calculate the reduction in angular momentum due to fluid density
//I'm sure there there is a better calculation
  if (Abs(rob[n].ma) > 0) {
    if (SimOpts.Density < 0.000001m) {
      rob[n].ma = rob[n].ma * (1 - (SimOpts.Density * 1000000));
    } else {
      rob[n].ma = 0;
    }
    if (Abs(rob[n].ma) < 0.0000001m) {
      rob[n].ma = 0;
    }
  }

  mag = VectorMagnitude(ref rob[n].vel);

  if (mag < 0.0000001m) {
goto getout; // Prevents accumulation of really small velocities.
  }


  Impulse = CSng(0.5m * SphereCd(mag, rob[n].radius) * SimOpts.Density * mag * mag * (PI * rob[n].radius ^ 2));

  if (Impulse > mag) {
    Impulse = mag * 0.99m; // Prevents the resistance force from exceeding the velocity!
  }
  ImpulseVector = VectorScalar(ref VectorUnit(ref rob[n].vel), ref Impulse);
//rob[n].ImpulseRes = VectorAdd(rob[n].ImpulseRes, ImpulseVector)
  rob[n].vel = VectorSub(ref rob[n].vel, ref ImpulseVector);
getout:
}

/*
'Public Sub TieDragForces(n As Integer)  'for ties
''calculate drag on the ties as if the ties are cylinders
''radius of the tie should be stored in tie array
'Dim a As Long

''EricL 5/26/2006 Added for Perf
'If rob[n].numties = 0 Then GoTo getout

'For a = 0 To MAXTIES
'  If rob[n].Ties(a).pnt > 0 Then
'    'If rob[n].Ties(a).pnt > n Then TieDrag2 n, rob[n].Ties(a).pnt
'    TieDrag n, rob[n].Ties(a).pnt
'  End If
'Next a

'getout:
'End Sub

'Public Sub TieDrag3(n1 As Integer, n2 As Integer)
'  Dim pos As vector
'  Dim a As Single, b As Single, c As Single
'  Dim TorqueScalar

'  pos = VectorSub(rob(n2).pos, rob(n1).pos)

'  a = Cross(rob(n1).vel, pos)
'  b = Cross(rob(n2).vel, pos)
'  c = (a + b) * 0.5 * 0.0001

'  'pretend Cd is always 1
'  pos = VectorUnit(pos)
'  pos = VectorSet(-pos.Y, pos.X)
'  rob(n1).ImpulseRes = VectorSub(rob(n1).ImpulseRes, VectorScalar(pos, Abs(c)))

'End Sub

'Public Sub TieDrag2(ByVal n1 As Long, ByVal n2 As Long)
'  Dim pos As vector
'  Dim a As Single, b As Single, c As Single
'  Dim invlength As Single
'  Dim ForceScalar As Single

'  If rob(n1).mass = 0 Or rob(n2).mass = 0 Then GoTo getout

'  'cooperative or independant\
'  pos = VectorSub(rob(n1).pos, rob(n2).pos)
'  invlength = VectorInvMagnitude(pos)

'  'a and b are the two cross velocities
'  a = Cross(rob(n1).vel, pos)
'  b = Cross(rob(n2).vel, pos)

'  c = (a + b) * 0.5 * invlength 'the average cross velocity
'  'use c to find the Cd

'  Dim asquare As Single
'  Dim ab As Single
'  Dim bsquare As Single
'  Dim BigB As Single
'  Dim BigA As Single
'  Const TieRadius As Single = 30

'  asquare = a * a
'  ab = a * b
'  bsquare = b * b
'  BigB = rob(n2).mass / (rob(n1).mass + rob(n2).mass)
'  BigA = SimOpts.Density * TieRadius * CylinderCd(c, TieRadius) * 0.083333333333 * Sgn(c)

'  ForceScalar = BigA * invlength * (4 * BigB * '    (asquare - 5 * ab + bsquare) + asquare + 2 * ab + 3 * bsquare)
'  'divide the above be either B or 1-B (depending on which robot we're
'  'applying forces to) and multiply by the orthogonal unit component to pos
'  pos = VectorScalar(pos, invlength)
'  pos = VectorSet(-pos.Y, pos.X)

'  rob(n1).ImpulseRes = VectorAdd(rob(n1).ImpulseRes, '    VectorScalar(pos, ForceScalar * 0.5 / BigB))
'  'rob(N2).ForceRes = VectorAdd(rob(N2).ForceRes, '  '  VectorScalar(pos, ForceScalar * 0.5 / (1 - BigB)))
'getout:
'End Sub

'Public Sub TieDrag(n1 As Integer, n2 As Integer)
''Simple method:

''v1 = my velocity
''m1 = my mass
''p1 = my position
''v2 = other 's velocity
''m2 = other mass
''p2 = other position
''
''
''1.  Find unit vector for tie = u = (p2 - p1) / length(p2-p1)
''2.  a = v1 cross u  |  a and b are the cross velocities, that is, the velocity
''    b = vc cross u  |  perpindicular to the movement of the tie,
''                    |  which is the direction that causes drag
''    v(d) = a + (b-a)/length * d where d is distance from a
''3.  Force on either bots is: A/12*length^2(a^2+2ab+3b^2) in a direction perpindicular to u.
''A = density * radius * Cd
''4.  Find force of drag per unit length using c as the velocity <--- this is
''     the assumption that drag and velocity are linearly related.
''     For turbulent flows, using the average of two velocities is incorrect.
''     In the future, this should be solved.  This requires integrating the
''     Cd and drag force equations for length (with velocity depending linearly
''     on distance from either Mc or m1 of course) between 0 and distance from
''     m1 to Mc.
''5.  The torque (we just add it to resistive forces) applied to "me" =
''     Drag Force per length / 2, since it's the area of the triangle
''     formed by length and dragforces at Mc and m1, divided by the length
''     since we 're applying them all to m1.

'  Dim u As vector, vc As vector, a As Single, b As Single, c As Single
'  Dim Aconstant As Single
'  Dim DragScalar As Single
'  Dim Drag As vector
'  Dim radius As Single
'  Dim Length As Single

'  If n2 > UBound(rob) Then Exit Sub
'  If n1 > UBound(rob) Then Exit Sub


'  '1.  Find unit vector
'  u = VectorSub(rob(n2).pos, rob(n1).pos)
'  Length = VectorMagnitude(u)
'  u = VectorUnit(u)

'  a = Cross(rob(n1).vel, u)
'  b = Cross(rob(n2).vel, u)
'  c = (a * a + 2 * a * b + 3 * b * b)
'  '4.  Find drag using c:

'  '1.7 is a good radius.  What?  Whhhaaatttt?  It is.
'  'okay, it's because that's what 10 body, at 905 twips^3 each,
'  'stretched into a cylinder with a length of 1000 twips would be

'  If Length = 0 Then Length = 1    'EricL: 4/15/2006
'  radius = Sqr(9050 / Length / PI) ' EricL Possible divide by zero bug here when a bot is moved using the mouse.
'  Aconstant = radius * SimOpts.Density * 1

'  DragScalar = Aconstant * Length * Length / 12 * c
'  Drag = VectorScalar(VectorSet(-u.Y, u.X), DragScalar * 0.5)

'  '5:  apply drag to bot
'  'not working right :/
'  'rob(n1).ForceRes = VectorAdd(rob(n1).ForceRes, Drag)
'End Sub
*/
public static decimal SphereCd(decimal velocitymagnitude, decimal radius) {
  decimal SphereCd = 0;
//computes the coeficient of drag for a spehre given the unit reynolds in simopts
//totally ripped from an online drag calculator.  So sue me.

  dynamic _WithVar_7411;
  _WithVar_7411 = SimOpts;

    decimal Reynolds = 0;
    decimal y11 = 0;
    decimal y12 = 0;
    decimal y13 = 0;
    decimal y1 = 0;
    decimal y2 = 0;
    decimal alpha = 0;

    if (_WithVar_7411.Viscosity == 0) {
goto getout;
    }

    if (velocitymagnitude < 0.00001m) {
      velocitymagnitude = 0.00001m; // Overflow protection
    }
    Reynolds = radius * 2 * velocitymagnitude * _WithVar_7411.Density / _WithVar_7411.Viscosity;

    y11 = 24 / (3 * 10 ^ 5);
    y12 = 6 / (1 + Sqr(3 * 10 ^ 5));
    y13 = 0.4m;

    y1 = y11 + y12 + y13;
    y2 = 0.09m;

    alpha = (y2 - y1) * 50000 ^ -2;
    if (Reynolds == 0) {
      SphereCd = 0;
    } else if (Reynolds < 3 * 10 ^ 5) {
      SphereCd = 24 / Reynolds + 6 / (1 + Sqr(Reynolds)) + 0.4m;
    } else if (Reynolds < 3.5m * 10 ^ 5) {
      SphereCd = alpha * (Reynolds - (3 * 10 ^ 5)) ^ 2 + y1;
    } else if (Reynolds < 6 * 10 ^ 5) {
      SphereCd = 0.09m;
    } else if (Reynolds < 4 * 10 ^ 6) {
      SphereCd = (Reynolds / (6 * 10 ^ 5)) ^ 0.55m * y2;
    } else {
      SphereCd = 0.255m;
    }
getout:
  return SphereCd;
}

public static decimal CylinderCd(decimal velocitymagnitude, decimal radius) {
  decimal CylinderCd = 0;
  decimal sign = 0;


  dynamic _WithVar_278;
  _WithVar_278 = SimOpts;

    const decimal alpha = -3.6444444444444E-11m;

    if (velocitymagnitude < 0) {
      sign = -1;
      velocitymagnitude = -velocitymagnitude;
    } else {
      sign = 1;
    }

    if (_WithVar_278.Viscosity == 0) {
      CylinderCd = 0;
goto ;
    }

    decimal Reynolds = 0;

    Reynolds = radius * 2 * velocitymagnitude * _WithVar_278.Density / _WithVar_278.Viscosity;

    switch(Reynolds) {
      case 0:
        CylinderCd = 0;
        break;
// TODO: Cannot convert case: Is < 1
      case 0:
        CylinderCd = (8 * PI) / (Reynolds * (Log(8 / Reynolds) - 0.077216m));
        break;
// TODO: Cannot convert case: Is < 100000#
      case 0:
        CylinderCd = 1 + 10 / Reynolds ^ (2 / 3);
        break;
// TODO: Cannot convert case: Is < 250000#
      case 0:
        CylinderCd = alpha * (Reynolds - 100000) ^ 2 + 1;
        break;
// TODO: Cannot convert case: Is < 600000#
      case 0:
        CylinderCd = 0.18m;
        break;
// TODO: Cannot convert case: Is < 4000000
      case 0:
        CylinderCd = 0.18m * (Reynolds / 600000) ^ 0.63m;
        break;
// TODO: Cannot convert case: Is >= 4000000
      case 0:
        CylinderCd = 0.6m;
break;
}
getout:
  return CylinderCd;
}

public static void GravityForces(ref int n) { //Botsareus 2/2/2013 added bouy as part of y-gravity formula
  if ((SimOpts.Ygravity == 0 || !SimOpts.Pondmode || SimOpts.Updnconnected)) {
    if (rob[n].Bouyancy > 0) {
      if (!boylabldisp) {
        Form1.BoyLabl.Visible = true;
      }
      boylabldisp = true;
    }
    rob[n].ImpulseInd = VectorAdd(ref rob[n].ImpulseInd, ref VectorSet(0, SimOpts.Ygravity * rob[n].mass));
  } else {
    if (Form1.BoyLabl.Visible) {
      Form1.BoyLabl.Visible = false;
    }
//bouy costs energy (calculated from voluntery movment)
//importent PhysMoving is calculated into cost as it changes voluntary movement speeds as well
    if (rob[n].Bouyancy > 0) {
      dynamic _WithVar_437;
      _WithVar_437 = rob[n];
        _WithVar_437.nrg = _WithVar_437.nrg - (SimOpts.Ygravity / (SimOpts.PhysMoving) * IIf(_WithVar_437.mass > 192, 192, _WithVar_437.mass) * SimOpts.Costs(MOVECOST) * SimOpts.Costs(COSTMULTIPLIER)) * rob[n].Bouyancy;
    }
    if ((1 / BouyancyScaling - rob[n].pos.Y / SimOpts.FieldHeight) > rob[n].Bouyancy) {
      rob[n].ImpulseInd = VectorAdd(ref rob[n].ImpulseInd, ref VectorSet(0, SimOpts.Ygravity * rob[n].mass));
    } else {
      rob[n].ImpulseInd = VectorAdd(ref rob[n].ImpulseInd, ref VectorSet(0, -SimOpts.Ygravity * rob[n].mass));
    }
  }
}

public static void VoluntaryForces(ref int n) {
//calculates new acceleration and energy values from robot's
//.up/.dn/.sx/.dx vars
  decimal EnergyCost = 0;

  vector NewAccel = null;

  vector dir = null;

  decimal mult = 0;


  dynamic _WithVar_4478;
  _WithVar_4478 = rob[n];
//corpses are dead, they don't move around of their own volition
//If .Corpse Or .wall Or (Not .exist) Or ((.mem(dirup) = 0) And (.mem(dirdn) = 0) And (.mem(dirsx) = 0) And (.mem(dirdx) = 0)) Then goto getout
    if (_WithVar_4478.Corpse || _WithVar_4478.DisableMovementSysvars || _WithVar_4478.DisableDNA || (!.exist) || ((_WithVar_4478.mem(dirup) == 0) && (_WithVar_4478.mem(dirdn) == 0) && (_WithVar_4478.mem(dirsx) == 0) && (_WithVar_4478.mem(dirdx) == 0))) {
goto getout;
    }

    if (_WithVar_4478.NewMove == false) {
      mult = _WithVar_4478.mass;
    } else {
      mult = 1;
    }

//yes it's backwards, that's on purpose
    dir = VectorSet(CLng(_WithVar_4478.mem(dirup)) - CLng(_WithVar_4478.mem(dirdn)), CLng(_WithVar_4478.mem(dirsx)) - CLng(_WithVar_4478.mem(dirdx)));
    dir = VectorScalar(ref dir, ref mult);

    NewAccel = VectorSet(Dot(ref _WithVar_4478.aimvector, ref dir), Cross(ref _WithVar_4478.aimvector, ref dir));

//EricL 4/2/2006 Clip the magnitude of the acceleration vector to avoid an overflow crash
//Its possible to get some really high accelerations here when altzheimers sets in or if a mutation
//or venom or something writes some really high values into certain mem locations like .up, .dn. etc.
//This keeps things sane down the road.
    if (VectorMagnitude(ref NewAccel) > SimOpts.MaxVelocity) {
      NewAccel = VectorScalar(ref NewAccel, ref SimOpts.MaxVelocity / VectorMagnitude(ref NewAccel));
    }

//NewAccel is the impulse vector formed by the robot's internal "engine".
//Impulse is the integral of Force over time.

    _WithVar_4478.ImpulseInd = VectorAdd(ref _WithVar_4478.ImpulseInd, ref VectorScalar(ref NewAccel, ref SimOpts.PhysMoving));

    EnergyCost = VectorMagnitude(ref NewAccel) * SimOpts.Costs(MOVECOST) * SimOpts.Costs(COSTMULTIPLIER);

//EricL 4/4/2006 Clip the energy loss due to voluntary forces.  The total energy loss per cycle could be
//higher then this due to other nrg losses and this may be redundent with the magnitude clip above, but it
//helps keep things sane down the road and avoid crashing problems when .nrg goes hugely negative.
    if (EnergyCost > _WithVar_4478.nrg) {
      EnergyCost = _WithVar_4478.nrg;
    }

    if (EnergyCost < -1000) {
      EnergyCost = -1000;
    }

    _WithVar_4478.nrg = _WithVar_4478.nrg - EnergyCost;
getout:
}

public static void TieHooke(ref int n) {
//Handles Hooke forces of a tie.  That is, stretching and shrinking
//Force = -kx - bv
//from experiments, k and b should be less than .1 otherwise the forces
//become too great for a euler modelling (that is, the forces become too large
//for velocity = velocity + acceleration

//can be made less complex (from O(n^2) to Olog(n) by calculating forces only
//for robots less than current number and applying that force to both robots

  decimal Length = 0;

  decimal displacement = 0;

  decimal Impulse = 0;

  int k = 0;

  int t = 0;

  vector uv = null;

  vector vy = null;

  decimal deformation = 0;


//EricL 5/26/2006 Perf Test
  if (rob[n].numties == 0) {
goto getout;
  }

  deformation = 20; // Tie can stretch or contract this much and no forces are applied.
  dynamic _WithVar_2796;
  _WithVar_2796 = rob[n];

    k = 1;
    While(k <= MAXTIES && _WithVar_2796.Ties(k).pnt != 0);
//Botsareus 9/27/2014 Bug fix
//This may happen sometimes when the robot a tie points to did not teleport properly
    if (CheckRobot(_WithVar_2796.Ties(k).pnt)) {
      do {
//Simple Delete Tie
        if (k > 1) {
          _WithVar_2796.mem(TIEPRES) = _WithVar_2796.Ties(k - 1).Port;
        } else {
          _WithVar_2796.mem(TIEPRES) = 0; // no more ties
        }

        for(t=k; t<MAXTIES - 1; t++) {
          _WithVar_2796.Ties(t) = _WithVar_2796.Ties(t + 1);
          Next(t);

          _WithVar_2796.Ties(MAXTIES).pnt = 0;

        } while(!(!CheckRobot(_WithVar_2796.Ties(k).pnt));
      }

      uv = VectorSub(ref _WithVar_2796.pos, ref rob(_WithVar_2796.Ties(k).pnt).pos);
      Length = VectorMagnitude(ref uv);

//delete tie if length > 1000
//remember length is inverse squareroot
      if (Length - _WithVar_2796.radius - rob(_WithVar_2796.Ties(k).pnt).radius > 1000) {
        DeleteTie(n, _WithVar_2796.Ties(k).pnt);
//k = k - 1 ' Have to do this since deltie slides all the ties down
      } else {
        if (_WithVar_2796.Ties(k).last > 1) {
          _WithVar_2796.Ties(k).last = _WithVar_2796.Ties(k).last - 1; // Countdown to deleting tie
        }
        if (_WithVar_2796.Ties(k).last < 0) {
          _WithVar_2796.Ties(k).last = _WithVar_2796.Ties(k).last + 1; // Countup to hardening tie
        }

//EricL 5/7/2006 Following section stiffens ties after 20 cycles
        if (_WithVar_2796.Ties(k).last == 1) {
          DeleteTie(n, _WithVar_2796.Ties(k).pnt);
// k = k - 1 ' Have to do this since deltie slides all the ties down
        } else { // Stiffen the Tie, the bot is a multibot!
          if (_WithVar_2796.Ties(k).last == -1) {
            regang(n, k);
          }

          if (Length != 0) {
            uv = VectorScalar(ref uv, ref 1 / Length);

//first -kx
            displacement = _WithVar_2796.Ties(k).NaturalLength - Length;

            if (Abs(displacement) > deformation) {
              displacement = Sgn(displacement) * (Abs(displacement) - deformation);
              Impulse = _WithVar_2796.Ties(k).k * displacement;
              _WithVar_2796.ImpulseInd = VectorAdd(ref _WithVar_2796.ImpulseInd, ref VectorScalar(ref uv, ref Impulse));

//next -bv
              vy = VectorSub(ref _WithVar_2796.vel, ref rob(_WithVar_2796.Ties(k).pnt).vel);
              Impulse = Dot(ref vy, ref uv) * -.Ties(k).b;
              _WithVar_2796.ImpulseInd = VectorAdd(ref _WithVar_2796.ImpulseInd, ref VectorScalar(ref uv, ref Impulse));
            }
          }
        }
      }
      k = k + 1;
      Wend();
getout:

  }

/*
'Botsareus 9/30/2014 Returns true if robot does not exsist
*/
private static bool CheckRobot(int n) {
  bool CheckRobot = false;

  CheckRobot = false;

  if (n > UBound(rob)) {
    CheckRobot = true;
    return CheckRobot;

  }

  if (n == 0) {
    CheckRobot = false;
    return CheckRobot;

  }

  if (rob[n].exist == false) {
    CheckRobot = true;
  }
  return CheckRobot;
}

public static void PlanetEaters(ref int n) {
//this way is really, really slow, since we normalize the vector (yuck), - Botsareus I am no math wiz, but how to make faster? I think it is as good as it gets... unless Numsgil maybe?
//Botsareus 8/22/2014 Cap on mass to gravity
  int t = 0;

  decimal force = 0;

  vector PosDiff = null;

  decimal mag = 0;


  if (!SimOpts.PlanetEaters) {
goto getout;
  }
  if (rob[n].mass == 0) {
goto getout;
  }

  for(t=n + 1; t<MaxRobs; t++) {
    if (rob(t).mass == 0 || !rob(t).exist) {
goto Nextiteration;
    }

    PosDiff = VectorSub(ref rob(t).pos, ref rob[n].pos);
    mag = VectorMagnitude(ref PosDiff);
    if (mag == 0) {
goto Nextiteration;
    }

    force = (SimOpts.PlanetEatersG * IIf(rob[n].mass > 192, 192, rob[n].mass) * IIf(rob(t).mass > 192, 192, rob(t).mass)) / (mag * mag);
    PosDiff = VectorScalar(ref PosDiff, ref 1 / mag);
//Now set PosDiff to the vector for force along that line

    PosDiff = VectorScalar(ref PosDiff, ref force);

    rob[n].ImpulseInd = VectorAdd(ref rob[n].ImpulseInd, ref PosDiff);
    rob(t).ImpulseInd = VectorSub(ref rob(t).ImpulseInd, ref PosDiff);
Nextiteration:
    Next(t);
getout:
  }

/*
' calculates angle between (x1,y1) and (x2,y2)
*/
public static decimal angle(decimal x1, decimal y1, decimal x2, decimal y2) {
  decimal angle = 0;
  decimal an = 0;

  decimal dx = 0;

  decimal dy = 0;

  dx = x2 - x1;
  dy = y1 - y2;
  if (dx == 0) {
//an = 0
    an = PI / 2;
    if (dy < 0) {
      an = PI / 2 * 3;
    }
  } else {
    an = Atn(dy / dx);
    if (dx < 0) {
      an = an + PI;
    }
  }
  angle = an;
  return angle;
}

/*
' normalizes angle in 0,2pi
*/
public static decimal angnorm(decimal an) {
  decimal angnorm = 0;
  While(an < 0);
  an = an + 2 * PI;
  Wend();
  While(an > 2 * PI);
  an = an - 2 * PI;
  Wend();
  angnorm = an;
  return angnorm;
}

/*
' calculates difference between two angles
*/
public static decimal AngDiff(ref decimal a1, ref decimal a2) {
  decimal AngDiff = 0;
  decimal r = 0;

  r = a1 - a2;
  if (r > PI) {
    r = -(2 * PI - r);
  }
  if (r < -PI) {
    r = r + 2 * PI;
  }
  AngDiff = r;
  return AngDiff;
}

/*
'' calculates torque generated by all ties on robots
*/
public static void TieTorque(ref int t) {
//Dim check As Single
  decimal anl = 0;

  decimal dlo = 0;

  decimal dx = 0;

  decimal dy = 0;

  decimal dist = 0;

  int n = 0;

  int j = 0;

  decimal mt = 0;
  decimal mm = 0;
  decimal m = 0;

  decimal nax = 0;
  decimal nay = 0;

  vector TorqueVector = null;

  decimal angleslack = 0;// amount angle can vary without torque forces being applied

  int numOfTorqueTies = 0;


  angleslack = 5 * 2 * PI / 360; // 5 degrees

  j = 1;
  mt = 0;
  numOfTorqueTies = 0;
  dynamic _WithVar_3235;
  _WithVar_3235 = rob(t);
    if (_WithVar_3235.numties > 0) { //condition added to prevent parsing robots without ties.
      if (_WithVar_3235.Ties(1).pnt > 0) {
        While(_WithVar_3235.Ties(j).pnt > 0);
        if (_WithVar_3235.Ties(j).angreg) { //if angle is fixed.
          n = _WithVar_3235.Ties(j).pnt;
          anl = angle(_WithVar_3235.pos.X, _WithVar_3235.pos.Y, rob[n].pos.X, rob[n].pos.Y); //angle of tie in euclidian space
          dlo = AngDiff(ref anl, ref _WithVar_3235.aim); //difference of angle of tie and direction of robot
          mm = AngDiff(ref dlo, ref _WithVar_3235.Ties(j).ang + _WithVar_3235.Ties(j).bend); //difference of actual angle and requested angle

          _WithVar_3235.Ties(j).bend = 0; //reset bend command .tieang
//   .Ties(j).angreg = False ' reset angle request flag
          if (Abs(mm) > angleslack) {
            numOfTorqueTies = numOfTorqueTies + 1;
            mm = (Abs(mm) - angleslack) * Sgn(mm);
            m = mm * 0.1m; // Was .3
            dx = rob[n].pos.X - _WithVar_3235.pos.X;
            dy = _WithVar_3235.pos.Y - rob[n].pos.Y;
            dist = Sqr(dx ^ 2 + dy ^ 2);
            nax = -Sin(anl) * m * dist / 10;
            nay = -Cos(anl) * m * dist / 10;
//experimental limits to acceleration
            if (Abs(nax) > 100) {
              nax = 100 * Sgn(nax);
            }
            if (Abs(nay) > 100) {
              nay = 100 * Sgn(nax);
            }

//EricL 4/24/2006 This is the torque vector on robot t from it's movement of the tie
            TorqueVector = VectorSet(nax, nay);

            rob[n].ImpulseInd = VectorSub(ref rob[n].ImpulseInd, ref TorqueVector); //EricL Subtact the torque for bot n.
            _WithVar_3235.ImpulseInd = VectorAdd(ref _WithVar_3235.ImpulseInd, ref TorqueVector); //EricL Add the acceleration for bot t
            mt = mt + mm; //in other words mt = mm for 1 tie
//If t = 10 Then
//  mt = mt
//End If
          }
        }
        j = j + 1;
        Wend();
//If rob(t).absvel > 10 Then rob(30000).absvel = 1000000  'crash inducing line for debugging
        if (mt != 0) {
          if (Abs(mt) > 2 * PI) {
            _WithVar_3235.Ties(j).ang = dlo;
//    DeleteTie t, n ' break the tie if the torque is too much
          } else {
            if (Abs(mt) < PI / 4) {
              _WithVar_3235.ma = mt; //This is used later and zeroed each cycle in SetAimFunc
            } else {
              _WithVar_3235.ma = PI / 4 * Sgn(mt);
            }
          }


//.aim = angnorm(.aim + .ma)
//.aimvector = VectorSet(Cos(.aim), Sin(.aim))
        }
      }
    }
}

/*
'' calculates acceleration due to the medium action on the links
'' (used for swimming)
'Public Sub Swimming()
'  Dim vxle As Long
'  Dim vyle As Long
'  Dim nd As node
'  Dim t As Integer, p As Integer
'  Dim j As Byte
'  Dim anle As Single, anve As Single, ancm As Single
'  Dim vea As Long, lle As Long
'  Dim cnorm As Single
'  Dim Fx As Long, Fy As Long
'  Set nd = rlist.firstnode
'  While Not (nd Is rlist.last)
'    t = nd.robn
'    With rob(t)
'      If .Corpse = False And .Numties > 0 Then  'new conditions to prevent parsing corpses and robots without ties.
'        j = 1
'        While .Ties(j).pnt > 0
'          p = .Ties(j).pnt
'          vxle = (.vx + rob(p).vx) / 2  'average x velocity
'          vyle = (.vy + rob(p).vy) / 2  'average y velocity
'          anle = angle(.x, .y, rob(p).x, rob(p).y)  'Angle between robots
'          anve = angle(0, 0, vxle, vyle)            'Angle of vector velocity
'          ancm = anve - anle                        'Combined angle
'          vea = Sqr(vxle ^ 2 + vyle ^ 2)            'velocity along vector
'          lle = Sqr((.x - rob(p).x) ^ 2 + (.y - rob(p).y) ^ 2)  'distance between robots
'          cnorm = Sin(ancm) * vea * lle * SimOpts.PhysSwim  'Swim force
'          'cnorm = Average Cross velocity * distance between bots * PhysSwim
'          Fx = cnorm * Sin(anle) / 800
'          Fy = cnorm * Cos(anle) / 800
'          .ax = .ax + Fx
'          .ay = .ay + Fy
'          rob(p).ax = rob(p).ax + Fx
'          rob(p).ay = rob(p).ay + Fy
'          j = j + 1
'        Wend
'      End If
'    End With
'    Set nd = nd.pn
'  Wend
'End Sub
*/
public static void bordercolls(ref int t) {
//treat the borders as spongy ground
//that makes you bounce off.

//bottom = -1 for top, 1 for ground
//side = -1 for left, 1 for right

//Const k As Single = 0.1
//Const b As Single = 0.04

  const decimal k = 0.4m;
  const decimal b = 0.05m;

  vector dif = null;

  vector dist = null;

  decimal smudge = 0;


  dynamic _WithVar_7001;
  _WithVar_7001 = rob(t);
    if ((_WithVar_7001.pos.X > _WithVar_7001.radius) && (_WithVar_7001.pos.X < SimOpts.FieldWidth - _WithVar_7001.radius) && (_WithVar_7001.pos.Y > _WithVar_7001.radius) && (_WithVar_7001.pos.Y < SimOpts.FieldHeight - _WithVar_7001.radius)) {
goto getout;
    }

    _WithVar_7001.mem(214) = 0;

    smudge = _WithVar_7001.radius + smudgefactor;

    dif = VectorMin(ref VectorMax(ref _WithVar_7001.pos, ref VectorSet(smudge, smudge)), ref VectorSet(SimOpts.FieldWidth - smudge, SimOpts.FieldHeight - smudge));
    dist = VectorSub(ref dif, ref _WithVar_7001.pos);

    if (dist.X != 0) {
      if (SimOpts.Dxsxconnected == true) {
        if (dist.X < 0) {
          ReSpawn(t, smudge, _WithVar_7001.pos.Y);
        } else {
          ReSpawn(t, SimOpts.FieldWidth - smudge, _WithVar_7001.pos.Y);
        }
      } else {
        _WithVar_7001.mem(214) = 1;
//F-> = -k dist-> + v-> * b

// .ImpulseRes.x = .ImpulseRes.x + dist.x * -k
        if (_WithVar_7001.pos.X - _WithVar_7001.radius < 0) {
          _WithVar_7001.pos.X = _WithVar_7001.radius;
        }
        if (_WithVar_7001.pos.X + _WithVar_7001.radius > SimOpts.FieldWidth) {
          _WithVar_7001.pos.X = CSng(SimOpts.FieldWidth) - _WithVar_7001.radius;
        }
        _WithVar_7001.ImpulseRes.X = _WithVar_7001.ImpulseRes.X + _WithVar_7001.vel.X * b;
      }
    }

    if (dist.Y != 0) {
      if (SimOpts.Updnconnected) {
        if (dist.Y < 0) {
          ReSpawn(t, _WithVar_7001.pos.X, smudge);
        } else {
          ReSpawn(t, _WithVar_7001.pos.X, SimOpts.FieldHeight - smudge);
        }
      } else {
        rob(t).mem(214) = 1;
//F-> = -k dist-> + v-> * b

//   dif = VectorMin(VectorMax(.pos, VectorSet(smudge, smudge)), VectorSet(SimOpts.FieldWidth - smudge, SimOpts.FieldHeight - smudge))
//  dist = VectorSub(dif, .pos)

// .ImpulseRes.y = .ImpulseRes.y + dist.y * -k
        if (_WithVar_7001.pos.Y - _WithVar_7001.radius < 0) {
          _WithVar_7001.pos.Y = _WithVar_7001.radius;
        }
        if (_WithVar_7001.pos.Y + _WithVar_7001.radius > SimOpts.FieldHeight) {
          _WithVar_7001.pos.Y = CSng(SimOpts.FieldHeight) - _WithVar_7001.radius;
        }
        _WithVar_7001.ImpulseRes.Y = _WithVar_7001.ImpulseRes.Y + _WithVar_7001.vel.Y * b;
      }
    }
getout:
}

/*
'EricL - My attempt to back port 2.5 physics to address collision detection
'with a bunch of extra tweaks figurred out via trial and error.
*/
public static void Repel3(ref int rob1, ref int rob2) {
  vector normal = null;

  vector vy = null;

  decimal Length = 0;

  decimal force = 0;

  vector V1 = null;

  vector V1f = null;

  vector V1d = null;

  vector V2 = null;

  vector V2f = null;

  vector V2d = null;

  decimal M1 = 0;

  decimal M2 = 0;

  decimal currdist = 0;

  vector unit = null;

  vector vel1 = null;

  vector vel2 = null;

  decimal projection = 0;

  decimal e = 0;

  decimal fixedSep = 0;// the distance each fixed bots need to be separated

  vector fixedSepVector = null;

  decimal i = 0;// moment of interia

  decimal relVel = 0;

  decimal TotalMass = 0;


  e = SimOpts.CoefficientElasticity; // Set in the UI or loaded/defaulted in the sim load routines

//Botsareus 9/30/2014 More realisitic coefficient for massive robots 'Botsareus 6/18/2016 Disabled - Better idea using fixed logic below
//If e > 0 Then If rob(rob1).mass > 400 And rob(rob2).mass > 400 Then e = e * 10

  normal = VectorSub(ref rob(rob2).pos, ref rob(rob1).pos); // Vector pointing from bot 1 to bot 2
  currdist = VectorMagnitude(ref normal); // The current distance between the bots

//If both bots are fixed or not moving and they overlap, move their positions directly.  Fixed bots can overlap when shapes sweep them together
//or when they teleport or materialize on top of each other.  We move them directly apart as they are assumed to have no velocity
//by scaling the normal vector by the amount they need to be separated.  Each bot is moved half of the needed distance without taking into consideration
//mass or size.
  if (rob(rob1).Fixed && rob(rob2[.Fixed(Or(VectorMagnitude(rob(rob1).vel +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, +)] < 0.0001m && VectorMagnitude(ref rob(rob2).vel) < 0.0001m)) {
    fixedSep = ((rob(rob1).radius + rob(rob2).radius) - currdist) / 2;
    fixedSepVector = VectorScalar(ref VectorUnit(ref normal), ref fixedSep);
    rob(rob1).pos = VectorSub(ref rob(rob1).pos, ref fixedSepVector);
    rob(rob2).pos = VectorAdd(ref rob(rob2).pos, ref fixedSepVector);
  } else {
//Botsareus 6/18/2016 Still slowly move robots appart to cancel out compressive events
    TotalMass = rob(rob1).mass + rob(rob2).mass;
    fixedSep = ((rob(rob1).radius + rob(rob2).radius) - currdist);
    fixedSepVector = VectorScalar(ref VectorUnit(ref normal), ref fixedSep / (1 + 55 ^ (0.3m - e)));
    rob(rob1).pos = VectorSub(ref rob(rob1).pos, ref VectorScalar(ref fixedSepVector, ref rob(rob2).mass / TotalMass)); //Botsareus 7/4/2016 Factor in mass of robots (apply inverted)
    rob(rob2).pos = VectorAdd(ref rob(rob2).pos, ref VectorScalar(ref fixedSepVector, ref rob(rob1).mass / TotalMass));
  }


  if (VectorInvMagnitude(ref normal) != -1) { //vectorinvmagnitude = inverse magnitude.  Returns -1# if divide by zero
    M1 = rob(rob1).mass;
    M2 = rob(rob2).mass;

//If a bot is fixed, all the collision energy should be translated to the non-fixed bot so for
//the purposes of calculating the force applied to the non-fixed bot, treat the fixed one as if it is very massive
    if (rob(rob1).Fixed) {
      M1 = 32000;
    }
    if (rob(rob2).Fixed) {
      M2 = 32000;
    }

    unit = VectorUnit(ref normal); // Create a unit vector pointing from bot 1 to bot 2
    vel1 = rob(rob1).vel;
    vel2 = rob(rob2).vel;

//Project the bot's direction vector onto the unit vector and scale by velocity
//These represent vectors we subtract from the bot's velocity to push the bot in a direction
//appropriate to the collision.  This would be all we needed if the bots all massed the same.
//It's possible the bots are already moving away from each other having "collided" last cycle.  If so,
//we don't want to reverse them again and we don't want to add too much more further acceleration
    projection = Dot(ref vel1, ref unit) * 0.99m; // Try damping things down a little


    if (projection <= 0) { // bots are already moving away from one another
      projection = 0.000001m;
    }
    V1 = VectorScalar(ref unit, ref projection);

    projection = Dot(ref vel2, ref unit) * 0.99m; // try damping things down a little

    if (projection >= 0) { // bots are already moving away from one another
      projection = -0.000001m;
    }
    V2 = VectorScalar(ref unit, ref projection);

//Now we need to factor in the mass of the bots.  These vectors represent the resistance to movement due
//to the bot's mass
    V1f = VectorScalar(ref VectorAdd(ref VectorScalar(ref V2, ref (e + 1) * M2), ref VectorScalar(ref V1, ref (M1 - e * M2))), ref 1 / (M1 + M2));
    V2f = VectorScalar(ref VectorAdd(ref VectorScalar(ref V1, ref (e + 1) * M1), ref VectorScalar(ref V2, ref (M2 - e * M1))), ref 1 / (M1 + M2));

// V1 = VectorAdd(V1, V1f)
// V2 = VectorAdd(V2, V2f)

//Now we have to add in the angular momentum due to the collision
//Note that we should really do the collision force and the angular momentum force together since
//some of the collision rebound goes into rotation, but this will do for now.

//First we have to calculate the relative angular velocities of the bot surfaces where they touch
//Note that this is relative to bot 1
//relVel = rob(rob1).radius * rob(rob1).ma - rob(rob2).radius * rob(rob2).ma

//The angular velocity from the collision is

//  I = (2 / 5) * rob(rob1).radius * rob(rob1).radius * M1
//rob(rob1).ma = rob(rob1).ma + VectorMagnitude(V1)
//rob(rob2).ma = rob(rob2).ma + Dot(V2, unit) / rob(rob2).radius

//No reason to try to try to accelerate fixed bots
    if (!rob(rob1).Fixed) {
      rob(rob1).vel = VectorAdd(ref VectorSub(ref rob(rob1).vel, ref V1), ref V1f);
    }

    if (!rob(rob2).Fixed) {
      rob(rob2).vel = VectorAdd(ref VectorSub(ref rob(rob2).vel, ref V2), ref V2f);
    }


//Update the touch senses
    touch(rob1, rob(rob2).pos.X, rob(rob2).pos.Y);
    touch(rob2, rob(rob1).pos.X, rob(rob1).pos.Y);

//Update last touch variables
    rob(rob1).lasttch = rob2;
    rob(rob2).lasttch = rob1;

//Update the refvars to reflect touching bots.
    lookoccurr(rob1, rob2);
    lookoccurr(rob2, rob1);

  }
}
}
