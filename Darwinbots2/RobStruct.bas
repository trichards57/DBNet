Attribute VB_Name = "RobStruct"
Type block
  tipo As Integer
  value As Integer
End Type


' robot structure
Public Type robot

  exist As Boolean        ' the robot exists?
  radius As Single
  
  Veg As Boolean          ' is it a vegetable?
  NoChlr As Boolean       ' no chloroplasts?
  
  wall As Boolean         ' is it a wall?
  Corpse As Boolean
  Fixed As Boolean        ' is it blocked?
  
  View As Boolean         ' has this bot ever tried to see?
  NewMove As Boolean      ' is the bot using the new physics paradigms?
  
  ' physics
  pos As vector
  BucketPos As vector
  
  vel As vector
  actvel As vector 'Botsareus 6/22/2016 Robots actual velocity if it hits something
  opos As vector 'Used to calculate actvel
   
  ImpulseInd As vector      ' independant forces vector
  ImpulseRes As vector      ' Resistive forces vector
  ImpulseStatic As Single   ' static force scalar (always opposes current forces)
    
  AddedMass As Single     'From fluid displacement
  
  aim As Single           ' aim angle
  aimvector As vector     ' the unit vector for aim
  
  oaim As Single          ' old aim angle
  ma As Single              ' angular momentum
  mt As Single              ' torch
  Ties(10) As tie           ' array of ties
  order As Integer          'order in the roborder array
    
  occurr(20) As Integer     ' array with the ref* values
  nrg As Single             ' energy
  onrg As Single            ' old energy

  
  chloroplasts As Single    'Panda 8/11/2013 number of chloroplasts
  
  body As Single            ' Body points. A corpse still has a body after all
  obody As Single           ' old body points, for use with pain pleas versions for body
  vbody As Single           ' Virtual Body used to calculate body functions of MBs
    
  mass As Single            ' mass of robot
  
  shell As Single          ' Hard shell. protection from shots 1-100 reduces each cycle
  Slime As Single          ' slime layer. protection from ties 1-100 reduces each cycle
  Waste As Single          ' waste buildup in a robot. Too much and he dies. some can be removed by various methods
  Pwaste As Single         ' Permanent waste. cannot be removed. Builds up as waste is removed.
  poison As Single         ' How poisonous is robot
  venom As Single          ' How venomous is robot
    
  Paralyzed As Boolean      ' true when robot is paralyzed
  Paracount As Single       ' countdown until paralyzed robot is free to move again
  
  numties As Single         ' the number of ties attached to a robot
  Multibot As Boolean       ' Is robot part of a multi-bot
  TieAngOverwrite(3) As Boolean 'Botsareus 3/22/2013 allowes program to handle tieang...tielen 1...4 as input
  TieLenOverwrite(3) As Boolean
  
  Poisoned As Boolean       ' Is robot poisoned and confused
  Poisoncount As Single     ' Countdown till poison is out of his system
  
  Bouyancy As Single        ' Does robot float or sink
  DecayTimer As Integer     ' controls decay cycle
  Kills As Long             ' How many other robots has it killed? Might not work properly
  Dead As Boolean           ' Allows program to define a robot as dead after a certain operation
  Ploc As Integer           ' Location for custom poison to strike
  Pval As Integer           ' Value to insert into venom location
  Vloc As Integer           ' Location for custom venom to strike
  Vval As Integer           ' Value to insert into venom location
  Vtimer As Long            ' Count down timer to produce a virus
  
  vars(1000) As var
  vnum As Integer           '| about private variables
  maxusedvars As Integer    '|
  usedvars(1000) As Integer '| used memory cells
  
  ' virtual machine
  epimem(14) As Integer
  mem(1000) As Integer      ' memory array
  dna() As block            ' program array
  
  lastopp As Long           ' Index of last object in the focus eye.  Could be a bot or shape or something else.
  lastopptype As Integer    ' Indicates the type of lastopp.
                            ' 0 - bot
                            ' 1 - shape
                            ' 2 - edge of the playing field
  lastopppos As vector      ' the position of the closest portion of the viewed object
  
  lasttch As Long           ' Botsareus 11/26/2013 The robot who is touching our robot.
  
  AbsNum As Long            ' absolute robot number
  sim As Long               ' GUID of sim in which this bot was born
    
  'Mutation related
  Mutables As mutationprobs
   
  PointMutCycle As Long     ' Next cycle to point mutate (expressed in cycles since birth.  ie: age)
  PointMutBP As Long        ' the base pair to mutate
  Point2MutCycle As Long    ' Botsareus 12/10/2013 The new point2 cycle
  
  condnum As Integer        ' number of conditions (used for cost calculations)
  console As Consoleform    ' console object associated to the robot
  
  ' informative
  SonNumber As Integer      ' number of sons
  
  Mutations As Long         ' total mutations
  OldMutations As Long      ' total mutations from dna file
  
  
  GenMut As Single          ' figure out how many mutations before the next genetic test
  OldGD As Single           ' our old genetic distance
  LastMut As Long           ' last mutations
  MutEpiReset As Double     ' how many mutations until epigenetic reset
  
  parent As Long            ' parent absolute number
  age As Long               ' age in cycles
  newage As Long            ' age this simulation
  BirthCycle As Long        ' birth cycle
  genenum As Integer        ' genes number
  generation As Integer     ' generation
  ''TODO Look at this
  LastOwner As String       ' last internet owner's name
  FName As String           ' species name
  DnaLen As Integer         ' dna length
  LastMutDetail As String   ' description of last mutations
  
  ' aspetto
  Skin(13) As Integer       ' skin definition
  OSkin(13) As Integer      ' Old skin definition
  color As Long             ' colour
  highlight As Boolean      ' is it highlighted?
  flash As Integer          ' EricL - used for blinking the entire bot a specific color for 1 cycle when various things happen
  
  'These store the last direction values the bot stored for voluntary movement.  Used to display movement vectors.
  lastup As Integer
  lastdown As Integer
  lastleft As Integer
  lastright As Integer
  
  virusshot As Long                 ' the viral shot being stored
  ga() As Boolean                   ' EricL March 15, 2006  Used to store gene activation state
  oldBotNum As Integer              ' EricL New for 2.42.8 - used only for remapping ties when loading multi-cell organisms
  reproTimer As Integer             ' New for 2.42.9 - the time in cycles before the act of reproduction is free
  CantSee As Boolean                ' Indicates whether bot's eyes get populated
  DisableDNA As Boolean             ' Indicates whether bot's DNA should be executed
  DisableMovementSysvars As Boolean ' Indicates whether movement sysvars for this bot should be disabled.
  CantReproduce As Boolean          ' Indicates whether reproduction for this robot has been disabled
  VirusImmune As Boolean            ' Indicates whether this robot is immune to viruses
  SubSpecies As Integer             ' Indicates this bot's subspecies.  Changed when mutation or virus infection occurs
'  Ancestors(500) As ancestorType    ' Orderred list of ancestor bot numbers.
'  AncestorIndex As Integer          ' Index into the Ancestors array.  Points to the bot's immediate parent.  Older ancestors have lower numbers then wrap.
  
  fertilized As Integer             ' If non-zero, indicates this bot has been fertilized via a sperm shot.  This bot can choose to sexually reproduce
                                    ' with this DNA until the counter hits 0.  Will be zero if unfertilized.
  spermDNA() As block               ' Contains the DNA this bot has been fertilized with.
  spermDNAlen As Integer
  
  tag As String
  
  monitor_r As Integer
  monitor_g As Integer
  monitor_b As Integer
  
  multibot_time As Byte
  Chlr_Share_Delay As Byte
  dq As Byte
  
  dbgstring As String

End Type
