Attribute VB_Name = "Physics"
'
'                 P H Y S I C S
'

'Important

Option Explicit

' experimental object: used to store a grid of environmental
' features
Public envgridpres As Boolean
Public nlink As Single         ' links physics constants
Public plink As Single
Public klink As Single
Public mlink As Single

Public Const smudgefactor As Single = 50 'just to keep the bots more likely to stay visible

Public boylabldisp As Boolean
Public BouyancyScaling As Single

Public Sub AddedMass(n As Integer)
  'added mass is a simple enough concept.
  'To move an object through a liquid, you must also move
  'that liquid out of the way.
  
  Const fourthirdspi As Single = 4.18879
  
  Const AddedMassCoefficientForASphere As Single = 0.5
  
  With rob(n)
    If simopts.Density = 0 Then
      .AddedMass = 0
    Else
      .AddedMass = AddedMassCoefficientForASphere * simopts.Density * fourthirdspi * .radius * .radius * .radius
    End If
  End With
End Sub

Public Sub FrictionForces(n As Integer)
  Dim Impulse As Single
  Dim mag As Single
  Dim ZGrav As Single
  
  With rob(n)
  
  If simopts.Zgravity = 0 Then GoTo getout

  ZGrav = simopts.Zgravity
  

    .ImpulseStatic = CSng(.mass * ZGrav * simopts.CoefficientStatic) ' * 1 cycle (timestep = 1)

    Impulse = CSng(.mass * ZGrav * simopts.CoefficientKinetic) ' * 1 cycle (timestep = 1)

    'Here we calculate the reduction in angular momentum due to friction
    If Abs(rob(n).ma) > 0 Then
      If Impulse < 48# Then
        rob(n).ma = rob(n).ma * (48 - Impulse) / 48
      Else
        rob(n).ma = 0
      End If
      If Abs(rob(n).ma) < 0.0000001 Then rob(n).ma = 0
    End If
    
    If Impulse > VectorMagnitude(.vel) Then Impulse = VectorMagnitude(.vel) ' EricL 5/3/2006 Added to insure friction only counteracts
    
    If Impulse < 0.0000001 Then GoTo getout ' Prevents the accumulation of very very low velocity in sims without density
      
    'EricL 5/7/2006 Changed to operate directly on velocity
    .vel = VectorSub(.vel, VectorScalar(VectorUnit(.vel), Impulse)) 'kinetic friction points in opposite direction of velocity
getout:
  End With
End Sub

Public Sub BrownianForces(n As Integer)
  If simopts.physBrown = 0 Then GoTo getout
  Dim Impulse As Single
  Dim RandomAngle As Single

    Impulse = simopts.physBrown * 0.5 * rndy
    
    RandomAngle = rndy * 2 * PI
    rob(n).ImpulseInd = VectorAdd(rob(n).ImpulseInd, VectorSet(Cos(RandomAngle) * Impulse, Sin(RandomAngle) * Impulse))
    rob(n).ma = rob(n).ma + (Impulse / 100) * (rndy - 0.5) ' turning motion due to brownian motion

getout:
End Sub

Public Sub SphereDragForces(n As Integer)  'for bots
  Dim Impulse As Single
  Dim ImpulseVector  As vector
  Dim mag As Single
  
  'No Drag if no velocity or no density
  If (rob(n).vel.x = 0 And rob(n).vel.y = 0) Or simopts.Density = 0 Then GoTo getout
   
  'Here we calculate the reduction in angular momentum due to fluid density
  'I'm sure there there is a better calculation
  If Abs(rob(n).ma) > 0 Then
    If simopts.Density < 0.000001 Then
      rob(n).ma = rob(n).ma * (1# - (simopts.Density * 1000000))
    Else
      rob(n).ma = 0
    End If
    If Abs(rob(n).ma) < 0.0000001 Then rob(n).ma = 0
  End If
  
  mag = VectorMagnitude(rob(n).vel)
  
  If mag < 0.0000001 Then GoTo getout ' Prevents accumulation of really small velocities.
  
 
    
  Impulse = CSng(0.5 * SphereCd(mag, rob(n).radius) * _
    simopts.Density * mag * mag * (PI * rob(n).radius ^ 2))
   
  If Impulse > mag Then Impulse = mag * 0.99 ' Prevents the resistance force from exceeding the velocity!
  ImpulseVector = VectorScalar(VectorUnit(rob(n).vel), Impulse)
  'rob(n).ImpulseRes = VectorAdd(rob(n).ImpulseRes, ImpulseVector)
  rob(n).vel = VectorSub(rob(n).vel, ImpulseVector)
getout:
End Sub

Public Function SphereCd(ByVal velocitymagnitude As Single, ByVal radius As Single) As Single
  'computes the coeficient of drag for a spehre given the unit reynolds in simopts
  'totally ripped from an online drag calculator.  So sue me.
  
  With simopts
  
  Dim Reynolds As Single, y11 As Single, y12 As Single, y13 As Single, y1 As Single, y2 As Single, alpha As Single
  If .Viscosity = 0 Then GoTo getout
  
  If velocitymagnitude < 0.00001 Then velocitymagnitude = 0.00001 ' Overflow protection
  Reynolds = radius * 2 * velocitymagnitude * .Density / .Viscosity
  
  y11 = 24 / (3 * 10 ^ 5)
  y12 = 6 / (1 + Sqr(3 * 10 ^ 5))
  y13 = 0.4
  
  y1 = y11 + y12 + y13
  y2 = 0.09
  
  alpha = (y2 - y1) * 50000 ^ -2
  If Reynolds = 0 Then
    SphereCd = 0
  ElseIf Reynolds < 3 * 10 ^ 5 Then
    SphereCd = 24 / Reynolds + 6 / (1 + Sqr(Reynolds)) + 0.4
  ElseIf Reynolds < 3.5 * 10 ^ 5 Then
    SphereCd = alpha * (Reynolds - (3 * 10 ^ 5)) ^ 2 + y1
  ElseIf Reynolds < 6 * 10 ^ 5 Then
    SphereCd = 0.09
  ElseIf Reynolds < 4 * 10 ^ 6 Then
    SphereCd = (Reynolds / (6 * 10 ^ 5)) ^ 0.55 * y2
  Else
    SphereCd = 0.255
  End If
getout:
  End With
End Function

Public Function CylinderCd(ByVal velocitymagnitude As Single, ByVal radius As Single) As Single
  Dim sign As Single
  
  With simopts
  
  Const alpha As Single = -3.6444444444444E-11
  
  If velocitymagnitude < 0 Then
    sign = -1#
    velocitymagnitude = -velocitymagnitude
  Else
    sign = 1#
  End If
  
  If .Viscosity = 0 Then
    CylinderCd = 0
    GoTo getout
  End If
  
  Dim Reynolds As Single
  Reynolds = radius * 2 * velocitymagnitude * .Density / .Viscosity
  
  Select Case Reynolds
    Case 0
      CylinderCd = 0
    Case Is < 1
      CylinderCd = (8 * PI) / (Reynolds * (Log(8 / Reynolds) - 0.077216))
    Case Is < 100000#
      CylinderCd = 1 + 10 / Reynolds ^ (2 / 3)
    Case Is < 250000#
      CylinderCd = alpha * (Reynolds - 100000) ^ 2 + 1#
    Case Is < 600000#
      CylinderCd = 0.18
    Case Is < 4000000
      CylinderCd = 0.18 * (Reynolds / 600000) ^ 0.63
    Case Is >= 4000000
      CylinderCd = 0.6
  End Select
getout:
  End With
End Function

Public Sub GravityForces(n As Integer)  'Botsareus 2/2/2013 added bouy as part of y-gravity formula
If (simopts.Ygravity = 0 Or Not simopts.Pondmode Or simopts.Updnconnected) Then
    If rob(n).Bouyancy > 0 Then
        If Not boylabldisp Then Form1.BoyLabl.Visible = True
        boylabldisp = True
    End If
    rob(n).ImpulseInd = VectorAdd(rob(n).ImpulseInd, VectorSet(0, simopts.Ygravity * rob(n).mass))
Else
    If Form1.BoyLabl.Visible Then Form1.BoyLabl.Visible = False
    'bouy costs energy (calculated from voluntery movment)
    'importent PhysMoving is calculated into cost as it changes voluntary movement speeds as well
    If rob(n).Bouyancy > 0 Then
        With rob(n)
        .nrg = .nrg - (simopts.Ygravity / (simopts.physMoving) * IIf(.mass > 192, 192, .mass) * simopts.Costs(MOVECOST) * simopts.Costs(COSTMULTIPLIER)) * rob(n).Bouyancy
        End With
    End If
    If (1 / BouyancyScaling - rob(n).pos.y / simopts.fieldHeight) > rob(n).Bouyancy Then
       rob(n).ImpulseInd = VectorAdd(rob(n).ImpulseInd, VectorSet(0, simopts.Ygravity * rob(n).mass))
    Else
       rob(n).ImpulseInd = VectorAdd(rob(n).ImpulseInd, VectorSet(0, -simopts.Ygravity * rob(n).mass))
    End If
End If
End Sub

Public Sub VoluntaryForces(n As Integer)
  'calculates new acceleration and energy values from robot's
  '.up/.dn/.sx/.dx vars
  Dim EnergyCost As Single
  Dim NewAccel As vector
  Dim dir As vector
  Dim mult As Single
  
  With rob(n)
    'corpses are dead, they don't move around of their own volition
    If .Corpse Or .DisableMovementSysvars Or .DisableDNA Or (Not .exist) Or ((.mem(dirup) = 0) And (.mem(dirdn) = 0) And (.mem(dirsx) = 0) And (.mem(dirdx) = 0)) Then GoTo getout
    
    If .NewMove = False Then
        mult = .mass
    Else
        mult = 1
    End If
       
    'yes it's backwards, that's on purpose
    dir = VectorSet(CLng(.mem(dirup)) - CLng(.mem(dirdn)), CLng(.mem(dirsx)) - CLng(.mem(dirdx)))
    dir = VectorScalar(dir, mult)
    
    NewAccel = VectorSet(Dot(.aimvector, dir), Cross(.aimvector, dir))
    
    'EricL 4/2/2006 Clip the magnitude of the acceleration vector to avoid an overflow crash
    'Its possible to get some really high accelerations here when altzheimers sets in or if a mutation
    'or venom or something writes some really high values into certain mem locations like .up, .dn. etc.
    'This keeps things sane down the road.
    If VectorMagnitude(NewAccel) > simopts.maxVelocity Then
      NewAccel = VectorScalar(NewAccel, simopts.maxVelocity / VectorMagnitude(NewAccel))
    End If
        
    'NewAccel is the impulse vector formed by the robot's internal "engine".
    'Impulse is the integral of Force over time.
    
    .ImpulseInd = VectorAdd(.ImpulseInd, VectorScalar(NewAccel, simopts.physMoving))
    
    EnergyCost = VectorMagnitude(NewAccel) * simopts.Costs(MOVECOST) * simopts.Costs(COSTMULTIPLIER)
    
    'EricL 4/4/2006 Clip the energy loss due to voluntary forces.  The total energy loss per cycle could be
    'higher then this due to other nrg losses and this may be redundent with the magnitude clip above, but it
    'helps keep things sane down the road and avoid crashing problems when .nrg goes hugely negative.
    If EnergyCost > .nrg Then
      EnergyCost = .nrg
    End If
    
    If EnergyCost < -1000 Then
      EnergyCost = -1000
    End If
    
    .nrg = .nrg - EnergyCost
getout:
  End With
End Sub

'Botsareus 9/30/2014 Returns true if robot does not exsist
Private Function CheckRobot(ByVal n As Integer) As Boolean
'
CheckRobot = False
'
If n > UBound(rob) Then
CheckRobot = True
Exit Function
End If
'
If n = 0 Then
CheckRobot = False
Exit Function
End If
'
If rob(n).exist = False Then CheckRobot = True
End Function

Public Sub PlanetEaters(n As Integer)
'this way is really, really slow, since we normalize the vector (yuck), - Botsareus I am no math wiz, but how to make faster? I think it is as good as it gets... unless Numsgil maybe?
'Botsareus 8/22/2014 Cap on mass to gravity
  Dim t As Integer
  Dim force As Single
  Dim PosDiff As vector
  Dim mag As Single
  
  If Not simopts.PlanetEaters Then GoTo getout
  If rob(n).mass = 0 Then GoTo getout:
    
  For t = n + 1 To MaxRobs
    If rob(t).mass = 0 Or Not rob(t).exist Then GoTo Nextiteration
    
    PosDiff = VectorSub(rob(t).pos, rob(n).pos)
    mag = VectorMagnitude(PosDiff)
    If mag = 0 Then GoTo Nextiteration
    
    force = (simopts.PlanetEatersG * IIf(rob(n).mass > 192, 192, rob(n).mass) * IIf(rob(t).mass > 192, 192, rob(t).mass)) / (mag * mag)
    PosDiff = VectorScalar(PosDiff, 1 / mag)
    'Now set PosDiff to the vector for force along that line
        
    PosDiff = VectorScalar(PosDiff, force)
    
    rob(n).ImpulseInd = VectorAdd(rob(n).ImpulseInd, PosDiff)
    rob(t).ImpulseInd = VectorSub(rob(t).ImpulseInd, PosDiff)
Nextiteration:
  Next t
getout:
End Sub

' calculates angle between (x1,y1) and (x2,y2)
Public Function angle(ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single) As Single
  Dim an As Single
  Dim dx As Single
  Dim dy As Single
  dx = x2 - x1
  dy = y1 - y2
  If dx = 0 Then
    'an = 0
    an = PI / 2
    If dy < 0 Then an = PI / 2 * 3
  Else
    an = Atn(dy / dx)
    If dx < 0 Then
      an = an + PI
    End If
  End If
  angle = an
End Function

' normalizes angle in 0,2pi
Public Function angnorm(ByVal an As Single) As Single
  While an < 0
    an = an + 2 * PI
  Wend
  While an > 2 * PI
    an = an - 2 * PI
  Wend
  angnorm = an
End Function

' calculates difference between two angles
Public Function AngDiff(a1 As Single, a2 As Single) As Single
  Dim r As Single
  r = a1 - a2
  If r > PI Then
    r = -(2 * PI - r)
  End If
  If r < -PI Then
    r = r + 2 * PI
  End If
  AngDiff = r
End Function

Public Sub bordercolls(t As Integer)
  'treat the borders as spongy ground
  'that makes you bounce off.
  
  'bottom = -1 for top, 1 for ground
  'side = -1 for left, 1 for right
   
  Const k As Single = 0.4
  Const b As Single = 0.05
  
  Dim dif As vector
  Dim dist As vector
  Dim smudge As Single
  
  With rob(t)
    If (.pos.x > .radius) And (.pos.x < simopts.fieldWidth - .radius) And (.pos.y > .radius) And (.pos.y < simopts.fieldHeight - .radius) Then GoTo getout
  
    .mem(214) = 0
    
    smudge = .radius + smudgefactor
  
    dif = VectorMin(VectorMax(.pos, VectorSet(smudge, smudge)), VectorSet(simopts.fieldWidth - smudge, simopts.fieldHeight - smudge))
    dist = VectorSub(dif, .pos)
  
    If dist.x <> 0 Then
      If simopts.Dxsxconnected = True Then
        If dist.x < 0 Then
          ReSpawn t, smudge, .pos.y
        Else
          ReSpawn t, simopts.fieldWidth - smudge, .pos.y
        End If
      Else
        .mem(214) = 1
        'F-> = -k dist-> + v-> * b
      
       ' .ImpulseRes.x = .ImpulseRes.x + dist.x * -k
         If .pos.x - .radius < 0 Then .pos.x = .radius
         If .pos.x + .radius > simopts.fieldWidth Then .pos.x = CSng(simopts.fieldWidth) - .radius
        .ImpulseRes.x = .ImpulseRes.x + .vel.x * b
      End If
    End If
  
    If dist.y <> 0 Then
      If simopts.Updnconnected Then
        If dist.y < 0 Then
          ReSpawn t, .pos.x, smudge
        Else
          ReSpawn t, .pos.x, simopts.fieldHeight - smudge
        End If
      Else
        rob(t).mem(214) = 1
      'F-> = -k dist-> + v-> * b
      
        If .pos.y - .radius < 0 Then .pos.y = .radius
        If .pos.y + .radius > simopts.fieldHeight Then .pos.y = CSng(simopts.fieldHeight) - .radius
        .ImpulseRes.y = .ImpulseRes.y + .vel.y * b
      End If
    End If
getout:
  End With
End Sub

'EricL - My attempt to back port 2.5 physics to address collision detection
'with a bunch of extra tweaks figurred out via trial and error.
Public Sub Repel3(rob1 As Integer, rob2 As Integer)
  Dim normal As vector
  Dim vy As vector
  Dim length As Single
  Dim force As Single
  Dim V1 As vector
  Dim V1f As vector
  Dim V1d As vector
  Dim V2 As vector
  Dim V2f As vector
  Dim V2d As vector
  Dim M1 As Single
  Dim M2 As Single
  Dim currdist As Single
  Dim unit As vector
  Dim vel1 As vector
  Dim vel2 As vector
  Dim projection As Single
  Dim e As Single
  Dim fixedSep As Single ' the distance each fixed bots need to be separated
  Dim fixedSepVector As vector
  Dim i As Single ' moment of interia
  Dim relVel As Single
  Dim TotalMass As Single
  
  e = simopts.CoefficientElasticity ' Set in the UI or loaded/defaulted in the sim load routines
    
  normal = VectorSub(rob(rob2).pos, rob(rob1).pos) ' Vector pointing from bot 1 to bot 2
  currdist = VectorMagnitude(normal) ' The current distance between the bots
  
  'If both bots are fixed or not moving and they overlap, move their positions directly.  Fixed bots can overlap when shapes sweep them together
  'or when they teleport or materialize on top of each other.  We move them directly apart as they are assumed to have no velocity
  'by scaling the normal vector by the amount they need to be separated.  Each bot is moved half of the needed distance without taking into consideration
  'mass or size.
  If rob(rob1).Fixed And rob(rob2).Fixed Or _
    (VectorMagnitude(rob(rob1).vel) < 0.0001 And VectorMagnitude(rob(rob2).vel) < 0.0001) Then
    fixedSep = ((rob(rob1).radius + rob(rob2).radius) - currdist) / 2#
    fixedSepVector = VectorScalar(VectorUnit(normal), fixedSep)
    rob(rob1).pos = VectorSub(rob(rob1).pos, fixedSepVector)
    rob(rob2).pos = VectorAdd(rob(rob2).pos, fixedSepVector)
  Else
    'Botsareus 6/18/2016 Still slowly move robots appart to cancel out compressive events
    TotalMass = rob(rob1).mass + rob(rob2).mass
    fixedSep = ((rob(rob1).radius + rob(rob2).radius) - currdist)
    fixedSepVector = VectorScalar(VectorUnit(normal), fixedSep / (1 + 55 ^ (0.3 - e)))
    rob(rob1).pos = VectorSub(rob(rob1).pos, VectorScalar(fixedSepVector, rob(rob2).mass / TotalMass))  'Botsareus 7/4/2016 Factor in mass of robots (apply inverted)
    rob(rob2).pos = VectorAdd(rob(rob2).pos, VectorScalar(fixedSepVector, rob(rob1).mass / TotalMass))
  End If
  
                  
  If VectorInvMagnitude(normal) <> -1# Then 'vectorinvmagnitude = inverse magnitude.  Returns -1# if divide by zero
    M1 = rob(rob1).mass
    M2 = rob(rob2).mass
    
    'If a bot is fixed, all the collision energy should be translated to the non-fixed bot so for
    'the purposes of calculating the force applied to the non-fixed bot, treat the fixed one as if it is very massive
    If rob(rob1).Fixed Then M1 = 32000
    If rob(rob2).Fixed Then M2 = 32000
    
    unit = VectorUnit(normal) ' Create a unit vector pointing from bot 1 to bot 2
    vel1 = rob(rob1).vel
    vel2 = rob(rob2).vel
    
    'Project the bot's direction vector onto the unit vector and scale by velocity
    'These represent vectors we subtract from the bot's velocity to push the bot in a direction
    'appropriate to the collision.  This would be all we needed if the bots all massed the same.
    'It's possible the bots are already moving away from each other having "collided" last cycle.  If so,
    'we don't want to reverse them again and we don't want to add too much more further acceleration
    projection = Dot(vel1, unit) * 0.99 ' Try damping things down a little
    
      
    If projection <= 0 Then ' bots are already moving away from one another
       projection = 0.000001
    End If
    V1 = VectorScalar(unit, projection)
    
    projection = Dot(vel2, unit) * 0.99 ' try damping things down a little
          
    If projection >= 0 Then ' bots are already moving away from one another
       projection = -0.000001
    End If
    V2 = VectorScalar(unit, projection)
    
    'Now we need to factor in the mass of the bots.  These vectors represent the resistance to movement due
    'to the bot's mass
    V1f = VectorScalar(VectorAdd(VectorScalar(V2, (e + 1#) * M2), VectorScalar(V1, (M1 - e * M2))), 1 / (M1 + M2))
    V2f = VectorScalar(VectorAdd(VectorScalar(V1, (e + 1#) * M1), VectorScalar(V2, (M2 - e * M1))), 1 / (M1 + M2))
    
   ' V1 = VectorAdd(V1, V1f)
   ' V2 = VectorAdd(V2, V2f)
     
    'Now we have to add in the angular momentum due to the collision
    'Note that we should really do the collision force and the angular momentum force together since
    'some of the collision rebound goes into rotation, but this will do for now.
    
    'First we have to calculate the relative angular velocities of the bot surfaces where they touch
    'Note that this is relative to bot 1
    'relVel = rob(rob1).radius * rob(rob1).ma - rob(rob2).radius * rob(rob2).ma
    
    'The angular velocity from the collision is
    
    '  I = (2 / 5) * rob(rob1).radius * rob(rob1).radius * M1
    'rob(rob1).ma = rob(rob1).ma + VectorMagnitude(V1)
    'rob(rob2).ma = rob(rob2).ma + Dot(V2, unit) / rob(rob2).radius
    
    'No reason to try to try to accelerate fixed bots
    If Not rob(rob1).Fixed Then
      rob(rob1).vel = VectorAdd(VectorSub(rob(rob1).vel, V1), V1f)
    End If

    If Not rob(rob2).Fixed Then
      rob(rob2).vel = VectorAdd(VectorSub(rob(rob2).vel, V2), V2f)
    End If
      
    
    'Update the touch senses
    touch rob1, rob(rob2).pos.x, rob(rob2).pos.y
    touch rob2, rob(rob1).pos.x, rob(rob1).pos.y
    
    'Update last touch variables
    rob(rob1).lasttch = rob2
    rob(rob2).lasttch = rob1
    
    'Update the refvars to reflect touching bots.
    lookoccurr rob1, rob2
    lookoccurr rob2, rob1

  End If
End Sub
