using DarwinBots.Modules;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DarwinBots.Model
{
    internal class Robot
    {
        public const int RobSize = 120;
        private const double AddedMassCoefficientForASphere = 0.5;
        private const double EnergyPerBody = 10.0;
        private readonly IBucketManager _bucketManager;
        private int _age;
        private double _body;
        private double _energy;
        private double _mass;
        private int _paralyzedCountdown;
        private double _poison;
        private int _poisonCountdown;
        private double _slime;
        private double _venom;
        private double _waste;

        public Robot(IBucketManager bucketManager)
        {
            _bucketManager = bucketManager;
        }

        public int AbsNum { get; set; }

        public DoubleVector ActualVelocity { get; set; }

        public int Age
        {
            get => _age;
            private set
            {
                _age = value;
                Memory[MemoryAddresses.robage] = Math.Min(_age, 32000);
            }
        }

        public double Aim { get; set; }
        public DoubleVector AimVector => new(Math.Cos(Aim), Math.Sin(Aim));
        public double AngularMomentum { get; set; }
        public int BirthCycle { get; set; }

        public double Body
        {
            get => _body;
            set
            {
                _body = Math.Clamp(value, 0, 32000);
                Memory[MemoryAddresses.body] = (int)_body;
            }
        }

        public double Bouyancy { get; set; }
        public IntVector BucketPosition { get; set; }
        public bool CantReproduce { get; set; }
        public bool CantSee { get; set; }
        public double Chloroplasts { get; set; }
        public bool ChloroplastsDisabled { get; set; }
        public byte ChloroplastsShareDelay { get; set; }
        public Color Color { get; set; }
        public int DecayTimer { get; set; }
        public List<DnaBlock> Dna { get; set; } = new();
        public bool DnaDisabled { get; set; }

        public double Energy
        {
            get => _energy;
            set => _energy = Math.Clamp(value, -32000, 32000);
        }

        public int[] EpigeneticMemory { get; } = new int[14];
        public bool Exists { get; private set; } = true;
        public int Fertilized { get; set; }
        public string FName { get; set; }
        public int Generation { get; set; }
        public double GenMut { get; set; }
        public DoubleVector IndependentImpulse { get; set; }
        public bool IsCorpse { get; set; }
        public bool IsDead { get; set; }
        public bool IsFixed { get; set; }
        public bool IsMultibot { get; set; }
        public bool IsParalyzed { get; set; }
        public bool IsPoisoned { get; set; }
        public bool IsVegetable { get; set; }
        public bool IsVirusImmune { get; set; }
        public int Kills { get; set; }
        public int LastMutation { get; set; }
        public List<string> LastMutationDetail { get; } = new List<string>();
        public object LastSeenObject { get; set; }
        public DoubleVector LastSeenObjectPosition { get; set; }
        public Robot LastTouched { get; set; }

        public double Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                Memory[MemoryAddresses.masssys] = (int)Mass;
            }
        }

        public int[] Memory { get; } = new int[1000];
        public bool MovementSysvarsDisabled { get; set; }
        public int MultibotTimer { get; set; }
        public MutationProbabilities MutationProbabilities { get; set; }
        public int Mutations { get; set; }

        /// <summary>
        /// Age this simulation.
        /// </summary>
        /// <remarks>
        /// Used by the Tie code.
        /// </remarks>
        public int NewAge { get; private set; }

        public int NumberOfGenes { get; set; }
        public int[] occurr { get; } = new int[20];
        public DoubleVector OffsetPosition => Position - Velocity + ActualVelocity;
        public double OldBody { get; set; }

        public double OldEnergy { get; set; }

        public int OldMutations { get; set; }

        public DoubleVector OldPosition { get; set; }

        public int ParalyzedCountdown
        {
            get => _paralyzedCountdown;
            set
            {
                _paralyzedCountdown = value;
                Memory[MemoryAddresses.ParalyzedCountdown] = _paralyzedCountdown;
            }
        }

        public Robot Parent { get; set; }

        public double PermanentWaste { get; set; }

        public int PointMutationBasePair { get; set; }

        public int PointMutationCycle { get; set; }

        public double Poison
        {
            get => _poison;
            set
            {
                _poison = value >= 0.5 ? Math.Min(value, 32000) : 0;
                Memory[MemoryAddresses.poison] = (int)_poison;
            }
        }

        public int PoisonCountdown
        {
            get => _poisonCountdown;
            set
            {
                _poisonCountdown = value;
                Memory[MemoryAddresses.PoisonCountdown] = _poisonCountdown;
            }
        }

        public int PoisonLocation { get; set; }

        public int PoisonValue { get; set; }

        public DoubleVector Position { get; set; }

        public DoubleVector ResistiveImpulse { get; set; }

        public double Shell { get; set; }

        public int[] Skin { get; } = new int[13];

        public double Slime
        {
            get => _slime;
            set
            {
                _slime = value >= 0.5 ? value : 0;
                Memory[MemoryAddresses.Slime] = (int)_slime;
            }
        }

        public int SonNumber { get; set; }

        public List<DnaBlock> SpermDna { get; set; } = new();

        public double StaticImpulse { get; set; }

        public int SubSpecies { get; set; }

        public bool[] TieAngleOverwrite { get; } = new bool[3];

        public bool[] TieLengthOverwrite { get; } = new bool[3];

        public List<Tie> Ties { get; } = new();

        public List<Variable> Variables { get; } = new();

        public double vbody { get; set; }

        public DoubleVector Velocity { get; set; }

        public double Venom
        {
            get => _venom;
            set => _venom = value >= 0.5 ? Math.Min(value, 32000) : 0;
        }

        public int VirusLocation { get; set; }

        public Shot VirusShot { get; set; }

        public int VirusTimer { get; set; }

        public int VirusValue { get; set; }

        public double Waste
        {
            get => _waste;
            set => _waste = Math.Min(value, 32000);
        }

        public void Ageing()
        {
            if (IsCorpse || !Exists)
                return;

            Age++;
            NewAge++;

            Memory[MemoryAddresses.timersys] += 1; // update epigenetic timer

            if (Memory[MemoryAddresses.timersys] > 32000)
                Memory[MemoryAddresses.timersys] = -32000;
        }

        public void Alzheimer(int badWasteLevel)
        {
            if (PermanentWaste + Waste <= badWasteLevel)
                return;

            // Makes robots with high waste act in a bizarre fashion.
            var loops = (PermanentWaste + Waste - badWasteLevel) / 4;

            for (var t = 0; t < loops; t++)
            {
                int loc;

                do
                {
                    loc = ThreadSafeRandom.Local.Next(1, 1000);
                } while (loc is MemoryAddresses.mkchlr or MemoryAddresses.rmchlr);

                Memory[loc] = ThreadSafeRandom.Local.Next(-32000, 32000);
            }
        }

        public void CleanUp(IShotManager shotManager, IBucketManager bucketManager)
        {
            Dna?.Clear();
            Dna = null;
            LastSeenObject = null;
            LastTouched = null;
            MutationProbabilities = null;
            Parent = null;
            SpermDna?.Clear();
            SpermDna = null;

            Modules.Ties.DeleteAllTies(this);

            Ties.Clear();
            Variables.Clear();
            if (VirusShot != null)
            {
                shotManager?.Shots.Remove(VirusShot);
                VirusShot.CleanUp();
                VirusShot = null;
            }

            Exists = false;

            bucketManager?.UpdateBotBucket(this);
        }

        public void DoGeneticMemory()
        {
            if (Age >= EpigeneticMemory.Length)
                return;

            if (Ties.Count <= 0 || Ties[0].Last <= 0)
                return;

            var loc = 976 + Age;
            if (Memory[loc] == 0 & EpigeneticMemory[Age] != 0)
                Memory[loc] = EpigeneticMemory[Age];
        }

        public void FireTies()
        {
            if (Memory[MemoryAddresses.mtie] == 0 || SimOpt.SimOpts.DisableTies)
                return;

            Robot lastSeen;

            switch (LastSeenObject)
            {
                case Robot r:
                    lastSeen = r;
                    break;

                case null when Age < 2 && Parent is { Exists: true }:
                    lastSeen = Parent;
                    break;

                case null when LastTouched is { Exists: true }:
                    lastSeen = LastTouched;
                    break;

                default:
                    return;
            }

            var length = (lastSeen.Position - Position).Magnitude();
            var maxLength = RobSize * 4 + GetRadius(SimOpt.SimOpts.FixedBotRadii) + lastSeen.GetRadius(SimOpt.SimOpts.FixedBotRadii);

            if (length <= maxLength)
                Modules.Ties.MakeTie(this, lastSeen, (int)(GetRadius(SimOpt.SimOpts.FixedBotRadii) + lastSeen.GetRadius(SimOpt.SimOpts.FixedBotRadii) + RobSize * 2), -20, Memory[MemoryAddresses.mtie]);

            Memory[MemoryAddresses.mtie] = 0;
        }

        public double GetRadius(bool isFixed)
        {
            if (isFixed)
                return RobSize / 2.0;

            var bodypoints = Math.Max(Body, 1);
            var r = Math.Pow(Math.Log(bodypoints) * bodypoints * RobotsManager.CubicTwipPerBody * 3 * 0.25 / Math.PI, 1.0 / 3);
            r += (415 - r) * Chloroplasts / 32000;

            if (r < 1)
                r = 1;

            return r;
        }

        public void HandleWaste(IShotManager shotManager)
        {
            if (Waste > 0 && Chloroplasts > 0)
                Vegs.feedveg2(this);

            if (SimOpt.SimOpts.BadWasteLevel == 0)
                SimOpt.SimOpts.BadWasteLevel = 400;

            Alzheimer(SimOpt.SimOpts.BadWasteLevel);

            if (Waste > 32000)
                shotManager.Defecate(this);

            if (PermanentWaste > 32000)
                PermanentWaste = 32000;

            if (Waste < 0)
                Waste = 0;

            Memory[828] = (int)Waste;
            Memory[829] = (int)PermanentWaste;
        }

        public void LogMutation(string strmut)
        {
            if (SimOpt.SimOpts.TotRunCycle == 0)
                return;

            if (LastMutationDetail.Count > 5)
                LastMutationDetail.Remove(LastMutationDetail[0]);

            LastMutationDetail.Add(strmut);
        }

        public void MakeStuff(Costs costs)
        {
            StoreVenom(costs.VenomCost * costs.CostMultiplier);
            StorePoison(costs.PoisonCost * costs.CostMultiplier);
            MakeShell(costs.ShellCost * costs.CostMultiplier);
            MakeSlime(costs.SlimeCost * costs.CostMultiplier);
        }

        public void ManageBody()
        {
            if (Memory[MemoryAddresses.strbody] == 0 && Memory[MemoryAddresses.fdbody] == 0)
                return;

            var energyChange = Math.Min(Memory[MemoryAddresses.strbody], 100) - Math.Min(Memory[MemoryAddresses.fdbody], 100);

            Energy -= energyChange;
            Body += energyChange / EnergyPerBody;

            if (Energy > 32000)
                Energy = 32000;

            Memory[MemoryAddresses.strbody] = 0;
            Memory[MemoryAddresses.fdbody] = 0;
        }

        public void ManageBouyancy()
        {
            if (Memory[MemoryAddresses.setboy] == 0)
                return;

            Bouyancy += (double)Memory[MemoryAddresses.setboy] / 32000;
            Bouyancy = Math.Clamp(Bouyancy, 0, 1);

            Memory[MemoryAddresses.rdboy] = (int)(Bouyancy * 32000);
            Memory[MemoryAddresses.setboy] = 0;
        }

        public void ManageChlr(int totalChloroplasts, double chloroplastCost, int maxPopulation)
        {
            if (ChloroplastsDisabled)
                return;

            ChangeChlr(totalChloroplasts, chloroplastCost, maxPopulation);

            Chloroplasts -= 0.5 / Math.Pow(100, Chloroplasts / 16000);

            Chloroplasts = Math.Clamp(Chloroplasts, 0, 32000);

            Memory[MemoryAddresses.chlr] = (int)Chloroplasts;
            Memory[MemoryAddresses.light] = (int)(32000 - Vegs.LightAval * 32000);
        }

        public void ManageFixed()
        {
            IsFixed = Memory[MemoryAddresses.Fixed] > 0;
        }

        public void Poisons()
        {
            if (IsCorpse || DnaDisabled)
                return;

            if (IsParalyzed)
            {
                Memory[VirusLocation] = VirusValue;

                ParalyzedCountdown--;

                if (ParalyzedCountdown < 1)
                {
                    IsParalyzed = false;
                    VirusLocation = 0;
                    VirusValue = 0;
                }
            }

            if (IsPoisoned)
            {
                Memory[PoisonLocation] = PoisonValue;

                PoisonCountdown--;
                if (PoisonCountdown < 1)
                {
                    IsPoisoned = false;
                    PoisonLocation = 0;
                    PoisonValue = 0;
                }
            }
        }

        public void ReadTie()
        {
            if (NewAge < 2)
                return;

            if (Ties.Any())
            {
                // If there is a value in .readtie then get the trefvars from that tie else read the trefvars from the last tie created
                var tn = Memory[MemoryAddresses.readtiesys] != 0 ? Memory[MemoryAddresses.readtiesys] : Memory[MemoryAddresses.TIEPRES];

                var tie = Ties.LastOrDefault(t => t.Port == tn);

                if (tie != null)
                    ReadTRefVars(tie);
            }
            EraseTRefVars();
        }

        public void ReadTRefVars(Tie tie)
        {
            if (tie.OtherBot.Energy < 32000 & tie.OtherBot.Energy > -32000)
                Memory[MemoryAddresses.trefnrg] = (int)tie.OtherBot.Energy; //copies tied robot's energy into memory cell *trefnrg

            if (tie.OtherBot.Age < 32000)
                Memory[465] = tie.OtherBot.Age + 1; //copies age of tied robot into *refvar
            else
                Memory[465] = 32000;

            if (tie.OtherBot.Body < 32000 & tie.OtherBot.Body > -32000)
                Memory[MemoryAddresses.trefbody] = (int)tie.OtherBot.Body; //copies tied robot's body value
            else
                Memory[MemoryAddresses.trefbody] = 32000;

            for (var l = 1; l < 8; l++)
                Memory[455 + l] = tie.OtherBot.occurr[l];

            if (Memory[476] > 0 & Memory[476] <= 1000)
            {
                //tmemval and tmemloc couple used to read a specific memory value from tied robot.
                Memory[475] = tie.OtherBot.Memory[Memory[476]];
            }

            Memory[478] = tie.OtherBot.IsFixed ? 1 : 0;
            Memory[479] = tie.OtherBot.Memory[MemoryAddresses.AimSys];
            Memory[MemoryAddresses.trefxpos] = tie.OtherBot.Memory[219];
            Memory[MemoryAddresses.trefypos] = tie.OtherBot.Memory[217];
            Memory[MemoryAddresses.trefvelyourup] = tie.OtherBot.Memory[MemoryAddresses.velup];
            Memory[MemoryAddresses.trefvelyourdn] = tie.OtherBot.Memory[MemoryAddresses.veldn];
            Memory[MemoryAddresses.trefvelyoursx] = tie.OtherBot.Memory[MemoryAddresses.velsx];
            Memory[MemoryAddresses.trefvelyourdx] = tie.OtherBot.Memory[MemoryAddresses.veldx];

            tie.OtherBot.Velocity = DoubleVector.Clamp(tie.OtherBot.Velocity, -16000, 16000);

            Memory[MemoryAddresses.trefvelmyup] = (int)(tie.OtherBot.Velocity.X * Math.Cos(Aim) + Math.Sin(Aim) * tie.OtherBot.Velocity.Y * -1 - Memory[MemoryAddresses.velup]); //gives velocity from mybots frame of reference
            Memory[MemoryAddresses.trefvelmydn] = Memory[MemoryAddresses.trefvelmyup] * -1;
            Memory[MemoryAddresses.trefvelmydx] = (int)(tie.OtherBot.Velocity.Y * Math.Cos(Aim) + Math.Sin(Aim) * tie.OtherBot.Velocity.X - Memory[MemoryAddresses.veldx]);
            Memory[MemoryAddresses.trefvelmysx] = Memory[MemoryAddresses.trefvelmydx] * -1;
            Memory[MemoryAddresses.trefvelscalar] = tie.OtherBot.Memory[MemoryAddresses.velscalar];
            Memory[MemoryAddresses.trefshell] = (int)tie.OtherBot.Shell;

            //These are the tie in/pairs
            for (var l = 410; l < 419; l++)
                Memory[l + 10] = tie.OtherBot.Memory[l];
        }

        public void SetAim()
        {
            double aim;
            double diff2 = 0;
            var diff = Physics.IntToRadians(Memory[MemoryAddresses.aimsx] - Memory[MemoryAddresses.aimdx]);

            if (Memory[MemoryAddresses.SetAim] == Physics.RadiansToInt(Aim))
            {
                aim = Aim + diff;
            }
            else
            {
                // .setaim overrides .aimsx and .aimdx
                aim = Physics.IntToRadians(Memory[MemoryAddresses.SetAim]); // this is where .aim needs to be
                diff = -Physics.AngDiff(Aim, aim); // this is the diff to get there 'Botsareus 6/18/2016 Added angnorm
                diff2 = Math.Abs(Math.Round((Aim - aim) / (2 * Math.PI), 0) * (2 * Math.PI)) * Math.Sign(diff); // this is how much we add to momentum
            }

            //diff + diff2 is now the amount, positive or negative to turn.
            Energy -= Math.Abs(Math.Round(diff + diff2, 3) * SimOpt.SimOpts.Costs.RotationCost * SimOpt.SimOpts.Costs.CostMultiplier);

            aim = Physics.NormaliseAngle(aim);

            //Overflow Protection
            while (AngularMomentum > 2 * Math.PI)
                AngularMomentum -= 2 * Math.PI;

            while (AngularMomentum < -2 * Math.PI)
                AngularMomentum += 2 * Math.PI;

            Aim = aim + AngularMomentum; // Add in the angular momentum

            // Voluntary rotation can reduce angular momentum but does not add to it.

            if (AngularMomentum > 0 & diff < 0)
            {
                AngularMomentum += (diff + diff2);
                if (AngularMomentum < 0)
                    AngularMomentum = 0;
            }
            if (AngularMomentum < 0 & diff > 0)
            {
                AngularMomentum += (diff + diff2);
                if (AngularMomentum > 0)
                    AngularMomentum = 0;
            }

            Memory[MemoryAddresses.aimsx] = 0;
            Memory[MemoryAddresses.aimdx] = 0;
            Memory[MemoryAddresses.AimSys] = Physics.RadiansToInt(Aim);
            Memory[MemoryAddresses.SetAim] = Physics.RadiansToInt(Aim);
        }

        public void ShareChloroplasts(Tie tie)
        {
            ShareResource(tie, MemoryAddresses.sharechlr, r => r.Chloroplasts, (r, s) => r.Chloroplasts = s);
        }

        public void ShareEnergy(Tie tie)
        {
            //This is an order of operation thing.  A bot earlier in the rob array might have taken all your nrg, taking your
            //nrg to 0.  You should still be able to take some back.
            if (Energy < 0 || tie.OtherBot.Energy < 0)
                return;

            //.mem(830) is the percentage of the total nrg this bot wants to receive
            //has to be positive to come here, so no worries about changing the .mem location here
            if (Memory[830] <= 0)
                Memory[830] = 0;
            else
            {
                Memory[830] = Memory[830] % 100;
                if (Memory[830] == 0)
                    Memory[830] = 100;
            }

            //Total nrg of both bots combined
            var totnrg = Energy + tie.OtherBot.Energy;

            var portionThatsMine = totnrg * ((double)Memory[830] / 100); // This is what the bot wants to have of the total
            if (portionThatsMine > 32000)
                portionThatsMine = 32000; // Can't want more than the max a bot can have

            var myChangeInNrg = portionThatsMine - Energy; // This is what the bot's change in nrg would be

            //If the bot is taking nrg, then he can't take more than that represented by his own body.  If giving nrg away, same thing.  The bot
            //can't give away more than that represented by his body.  Should make it so that larger bots win tie feeding battles.
            if (Math.Abs(myChangeInNrg) > Body)
                myChangeInNrg = Math.Sign(myChangeInNrg) * Body;

            if (Energy + myChangeInNrg > 32000)
                myChangeInNrg = 32000 - Energy; //Limit change if it would put bot over the limit

            if (Energy + myChangeInNrg < 0)
                myChangeInNrg = -Energy; // Limit change if it would take the bot below 0

            //Now we have to check the limits on the other bot
            //sign is negative since the negative of myChangeinNrg is what the other bot is going to get/recevie
            if (tie.OtherBot.Energy - myChangeInNrg > 32000)
                myChangeInNrg = -(32000 - tie.OtherBot.Energy); //Limit change if it would put bot over the limit

            if (tie.OtherBot.Energy - myChangeInNrg < 0)
                myChangeInNrg = tie.OtherBot.Energy; // limit change if it would take the bot below 0

            //Do the actual nrg exchange
            Energy += myChangeInNrg;
            tie.OtherBot.Energy -= myChangeInNrg;

            //Transferring nrg costs nrg.  1% of the transfer gets deducted from the bot iniating the transfer
            Energy -= Math.Abs(myChangeInNrg) * 0.01;

            //Bots with 32000 nrg can still take or receive nrg, but everything over 32000 disappears
            if (Energy > 32000)
                Energy = 32000;

            if (tie.OtherBot.Energy > 32000)
                tie.OtherBot.Energy = 32000;
        }

        public void ShareShell(Tie tie)
        {
            ShareResource(tie, 832, r => r.Shell, (r, s) => r.Shell = s);
        }

        public void ShareSlime(Tie tie)
        {
            ShareResource(tie, 833, r => r.Slime, (r, s) => r.Slime = s);
        }

        public void ShareWaste(Tie tie)
        {
            ShareResource(tie, 831, r => r.Waste, (r, s) => r.Waste = s);
        }

        public void Shock()
        {
            if (IsVegetable || Energy <= 3000)
                return;

            var temp = OldEnergy - Energy;

            if (!(temp > OldEnergy / 2))
                return;

            Energy = 0;
            Body += Energy / 10;
        }

        public void TiePortCommunication()
        {
            if (!(Memory[455] != 0 & Ties.Count > 0 & Memory[MemoryAddresses.tieloc] > 0))
                return;

            if (Memory[MemoryAddresses.tieloc] > 0 & Memory[MemoryAddresses.tieloc] < 1001)
            {
                //.tieloc value
                var tie = Ties.LastOrDefault(t => t.Port == Memory[MemoryAddresses.TIENUM]);

                if (tie == null)
                    return;

                tie.OtherBot.Memory[Memory[MemoryAddresses.tieloc]] = Memory[MemoryAddresses.tieval]; //stores a value in tied robot memory location (.tieloc) specified in .tieval

                if (!tie.BackTie)
                    tie.InfoUsed = true; //draws tie white
                else
                    tie.ReverseTie.InfoUsed = true;

                Memory[MemoryAddresses.tieval] = 0;
                Memory[MemoryAddresses.tieloc] = 0;
            }
        }

        public void UpdateMass()
        {
            Mass = Math.Clamp(Body / 1000 + Shell / 200 + Chloroplasts / 32000 * 31680, 1, 32000);
        }

        public void UpdatePosition(double maxVelocity, bool noMomentum, double density, bool radiusFixed)
        {
            //Following line commented since mass is set earlier in CalcMass
            if (Mass + GetAddedMass(density, radiusFixed) < 0.25)
                Mass = 0.25 - GetAddedMass(density, radiusFixed); // a fudge since Euler approximation can't handle it when mass -> 0

            double vt = 0;

            if (!IsFixed)
            {
                // speed normalization
                Velocity += IndependentImpulse * (1 / (Mass + GetAddedMass(density, radiusFixed)));

                vt = Velocity.MagnitudeSquare();
                if (vt > maxVelocity * maxVelocity)
                {
                    Velocity = Velocity.Unit() * maxVelocity;
                }

                Position += Velocity;
                _bucketManager.UpdateBotBucket(this);
            }
            else
                Velocity = DoubleVector.Zero;

            //Have to do these here for both fixed and unfixed bots to avoid build up of forces in case fixed bots become unfixed.
            IndependentImpulse = DoubleVector.Zero;
            ResistiveImpulse = DoubleVector.Zero;
            StaticImpulse = 0;

            if (noMomentum)
                Velocity = DoubleVector.Zero;

            Memory[MemoryAddresses.dirup] = 0;
            Memory[MemoryAddresses.dirdn] = 0;
            Memory[MemoryAddresses.dirdx] = 0;
            Memory[MemoryAddresses.dirsx] = 0;

            Memory[MemoryAddresses.velscalar] = (int)Math.Clamp(Math.Sqrt(vt), -32000, 32000);
            Memory[MemoryAddresses.vel] = (int)Math.Clamp(Math.Cos(Aim) * Velocity.X + Math.Sin(Aim) * Velocity.Y * -1, -32000, 32000);
            Memory[MemoryAddresses.veldn] = Memory[MemoryAddresses.vel] * -1;
            Memory[MemoryAddresses.veldx] = (int)Math.Clamp(Math.Sin(Aim) * Velocity.X + Math.Cos(Aim) * Velocity.Y, -32000, 32000);
            Memory[MemoryAddresses.velsx] = Memory[MemoryAddresses.veldx] * -1;

            Memory[MemoryAddresses.maxvelsys] = (int)maxVelocity;
        }

        public void UpdateTieAngles()
        {
            // Zero these incase no ties or tienum is non-zero, but does not refer to a tieport, etc.
            Memory[MemoryAddresses.TIEANG] = 0;
            Memory[MemoryAddresses.TIELEN] = 0;

            //No point in setting the length and angle if no ties!
            if (Ties.Count == 0)
                return;

            //Figure if .tienum has a value.  If it's zero, use .tiepres
            var whichTie = Memory[MemoryAddresses.TIENUM] != 0 ? Memory[MemoryAddresses.TIENUM] : Memory[MemoryAddresses.TIEPRES];

            if (whichTie == 0)
                return;

            //Now find the tie that corrosponds to either .tienum or .tiepres and set .tieang and .tielen accordingly
            //We count down through the ties to find the most recent tie with the specified tieport since more than one tie
            //can potentially have the same tieport and we want the most recent, which will be the one with the highest k.
            var tie = Ties.LastOrDefault(t => t.Port == whichTie);

            if (tie != null)
            {
                var tieAngle = Physics.Angle(Position.X, Position.Y, tie.OtherBot.Position.X, tie.OtherBot.Position.Y);
                var dist = (Position - tie.OtherBot.Position).Magnitude();
                //Overflow prevention.  Very long ties can happen for one cycle when bots wrap in torridal fields
                if (dist > 32000)
                    dist = 32000;

                Memory[MemoryAddresses.TIEANG] = (int)-(Physics.AngDiff(Physics.NormaliseAngle(tieAngle), Physics.NormaliseAngle(Aim)) * 200);
                Memory[MemoryAddresses.TIELEN] = (int)(dist - GetRadius(SimOpt.SimOpts.FixedBotRadii) - tie.OtherBot.GetRadius(SimOpt.SimOpts.FixedBotRadii));
            }
        }

        public void Upkeep(Costs costs)
        {
            if (IsCorpse)
                return;

            double cost;

            // Age Cost
            var ageDelta = Age - costs.AgeCostBeginAge;

            if (ageDelta > 0 & Age > 0)
            {
                if (costs.EnableAgeCostIncreaseLog)
                    cost = costs.AgeCost * Math.Log(ageDelta);
                else if (costs.EnableAgeCostIncreasePerCycle)
                    cost = costs.AgeCost + ageDelta * costs.AgeCostIncreasePerCycle;
                else
                    cost = costs.AgeCost;

                Energy -= cost * costs.CostMultiplier;
            }

            // Body Upkeep
            cost = Body * costs.BodyUpkeepCost * costs.CostMultiplier;
            Energy -= cost;

            // DNA upkeep cost
            cost = (Dna.Count - 1) * costs.DnaUpkeepCost * costs.CostMultiplier;
            Energy -= cost;

            // Degrade slime
            Slime *= 0.98;

            // Degrade poison
            Poison *= 0.98;
        }

        private void ChangeChlr(int totalChloroplasts, double chloroplastCost, int maxPopulation)
        {
            if (Memory[MemoryAddresses.mkchlr] == 0 && Memory[MemoryAddresses.rmchlr] == 0)
                return;

            var tmpchlr = Chloroplasts;

            // add chloroplasts
            Chloroplasts += Memory[MemoryAddresses.mkchlr];

            // remove chloroplasts
            Chloroplasts -= Memory[MemoryAddresses.rmchlr];

            if (tmpchlr < Chloroplasts)
            {
                var newnrg = Energy - (Chloroplasts - tmpchlr) * chloroplastCost;

                if (totalChloroplasts > maxPopulation && IsVegetable || newnrg < 100)
                    Chloroplasts = tmpchlr;
                else
                    Energy = newnrg;
            }

            Memory[MemoryAddresses.mkchlr] = 0;
            Memory[MemoryAddresses.rmchlr] = 0;
        }

        private void EraseTRefVars()
        {
            // Zero the trefvars as all ties have gone.  Perf -> Could set a flag to not do this everytime
            for (var counter = 456; counter < 465; counter++)
                Memory[counter] = 0;

            Memory[MemoryAddresses.trefbody] = 0;
            Memory[475] = 0;
            Memory[478] = 0;
            Memory[479] = 0;
            for (var counter = 0; counter < 10; counter++)
                Memory[MemoryAddresses.trefxpos + counter] = 0;

            //These are .tin trefvars
            for (var counter = 420; counter < 429; counter++)
                Memory[counter] = 0;
        }

        private double GetAddedMass(double density, bool radiusFixed)
        {
            return AddedMassCoefficientForASphere * density * (Math.PI * 4 / 3) * Math.Pow(GetRadius(radiusFixed), 3);
        }

        private void MakeShell(double cost)
        {
            if (Memory[MemoryAddresses.MakeShell] == 0)
                return;

            const double shellNrgConvRate = 0.1;

            StoreResource(MemoryAddresses.MakeShell, MemoryAddresses.Shell, shellNrgConvRate, cost, r => r.Shell, (r, s) => r.Shell = s, true);
        }

        private void MakeSlime(double cost)
        {
            if (Memory[MemoryAddresses.MakeSlime] == 0)
                return;

            const double slimeNrgConvRate = 0.1;

            StoreResource(MemoryAddresses.MakeSlime, MemoryAddresses.Slime, slimeNrgConvRate, cost, r => r.Slime, (r, s) => r.Slime = s, true);
        }

        private void ShareResource(Tie tie, int address, Func<Robot, double> getValue, Action<Robot, double> setValue)
        {
            if (Memory[address] > 99)
                Memory[address] = 99;
            if (Memory[address] < 0)
                Memory[address] = 0;

            var total = getValue(this) + getValue(tie.OtherBot);

            setValue(tie.OtherBot, Math.Min(total * ((100 - (double)Memory[address]) / 100), 32000));
            setValue(this, Math.Min(total * ((double)Memory[address] / 100), 32000));

            Memory[address] = (int)getValue(this); // update the .shell sysvar
            tie.OtherBot.Memory[address] = (int)getValue(tie.OtherBot);
        }

        private void StorePoison(double cost)
        {
            if (Memory[MemoryAddresses.StorePoison] == 0)
                return;

            const double poisonNrgConvRate = 0.25; // Make 4 poison for 1 nrg

            StoreResource(MemoryAddresses.StorePoison, MemoryAddresses.poison, poisonNrgConvRate, cost, r => r.Poison, (r, s) => r.Poison = s);
        }

        private void StoreResource(int storeAddress, int levelAddress, double conversionRate, double resourceCost, Func<Robot, double> getValue, Action<Robot, double> setValue, bool multiBotDiscount = false)
        {
            if (Energy <= 0)
                return;

            double delta = Math.Clamp(Memory[storeAddress], -32000, 32000);
            delta = Math.Clamp(delta, -Energy / conversionRate, Energy / conversionRate);
            delta = Math.Clamp(delta, -100, 100);

            if (getValue(this) + delta > 32000)
                delta = 32000 - getValue(this);

            if (getValue(this) + delta < 0)
                delta = -getValue(this);

            setValue(this, getValue(this) + delta);

            if (IsMultibot && multiBotDiscount)
                Energy -= Math.Abs(delta) * conversionRate / (Ties.Count + 1);
            else
                Energy -= Math.Abs(delta) * conversionRate;

            //This is the transaction cost
            var cost = Math.Abs(delta) * resourceCost;

            Energy -= cost;
            Waste += cost;

            Memory[storeAddress] = 0;
            Memory[levelAddress] = (int)getValue(this);
        }

        private void StoreVenom(double cost)
        {
            const double venomNrgConvRate = 1.0; // Make 1 venom for 1 nrg

            if (Memory[MemoryAddresses.StoreVenom] == 0)
                return;

            StoreResource(MemoryAddresses.StoreVenom, MemoryAddresses.Venom, venomNrgConvRate, cost, r => r.Venom, (r, s) => r.Venom = s);
        }
    }
}
