using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DBLibrary
{
	internal partial class RobotHost
	{
		public List<int> AbsoluteNumber { get; } = new List<int>();
		public List<Vector> ActualVelocity { get; } = new List<Vector>();
		public List<float> AddedMass { get; } = new List<float>();
		public List<int> Age { get; } = new List<int>();
		public List<float> Aim { get; } = new List<float>();
		public List<Vector> AimVector { get; } = new List<Vector>();
		public List<int> AncestorIndex { get; } = new List<int>();
		public List<AncestorType[]> Ancestors { get; } = new List<AncestorType[]>();
		public List<float> AngularMomentum { get; } = new List<float>();
		public List<int> BirthCycle { get; } = new List<int>();
		public List<float> Body { get; } = new List<float>();
		public List<VectorInt> BucketPosition { get; } = new List<VectorInt>();
		public List<bool> CantReproduce { get; } = new List<bool>();
		public List<bool> CantSee { get; } = new List<bool>();
		public List<int> ChlorophylShareDelay { get; } = new List<int>();
		public List<float> Chloroplasts { get; } = new List<float>();
		public List<int> Color { get; } = new List<int>();
		public List<int> ConditionNumber { get; } = new List<int>();
		public List<int> DecayTimer { get; } = new List<int>();
		public List<bool> DisableDNA { get; } = new List<bool>();
		public List<bool> DisableMovementSysVars { get; } = new List<bool>();
		public List<DnaBlock[]> Dna { get; } = new List<DnaBlock[]>();
		public List<int> DNALength { get; } = new List<int>();
		public List<int> Dq { get; } = new List<int>();
		public List<float> Energy { get; } = new List<float>();
		public List<int[]> EpiMem { get; } = new List<int[]>();
		public List<bool> Exists { get; } = new List<bool>();
		public List<int> Fertilized { get; } = new List<int>();
		public List<int> Flash { get; } = new List<int>();
		public List<string> FName { get; } = new List<string>();
		public List<bool[]> GeneActivations { get; } = new List<bool[]>();
		public List<int> GeneNumber { get; } = new List<int>();
		public List<int> Generation { get; } = new List<int>();
		public List<float> GenMut { get; } = new List<float>();
		public List<bool> HasViewed { get; } = new List<bool>();
		public List<bool> Highlight { get; } = new List<bool>();
		public List<Vector> ImpulseInd { get; } = new List<Vector>();
		public List<Vector> ImpulseRes { get; } = new List<Vector>();
		public List<float> ImpulseStatic { get; } = new List<float>();
		public List<bool> IsCorpse { get; } = new List<bool>();
		public List<bool> IsDead { get; } = new List<bool>();
		public List<bool> IsMultiBot { get; } = new List<bool>();
		public List<bool> IsParalyzed { get; } = new List<bool>();
		public List<bool> IsPoisoned { get; } = new List<bool>();
		public List<bool> IsVegetable { get; } = new List<bool>();
		public List<bool> IsWall { get; } = new List<bool>();
		public List<int> Kills { get; } = new List<int>();
		public List<int> LastDown { get; } = new List<int>();
		public List<int> LastLeft { get; } = new List<int>();
		public List<int> LastMutation { get; } = new List<int>();
		public List<string> LastMutationDetail { get; } = new List<string>();
		public List<int> LastObject { get; } = new List<int>();
		public List<Vector> LastObjectPosition { get; } = new List<Vector>();
		public List<int> LastObjectType { get; } = new List<int>();
		public List<string> LastOwner { get; } = new List<string>();
		public List<int> LastRight { get; } = new List<int>();
		public List<int> LastTouchedRobot { get; } = new List<int>();
		public List<int> LastUp { get; } = new List<int>();
		public List<float> Mass { get; } = new List<float>();
		public List<int> MaxedUsedVariables { get; } = new List<int>();
		public List<int[]> Memory { get; } = new List<int[]>();
		public List<int> MonitorB { get; } = new List<int>();
		public List<int> MonitorG { get; } = new List<int>();
		public List<int> MonitorR { get; } = new List<int>();
		public List<float> MT { get; } = new List<float>();
		public List<int> MultibotTime { get; } = new List<int>();
		public List<MutationProbs> Mutables { get; } = new List<MutationProbs>();
		public List<int> Mutations { get; } = new List<int>();
		public List<float> MutEpiReset { get; } = new List<float>();
		public List<int> NewAge { get; } = new List<int>();
		public List<bool> NoChloroplasts { get; } = new List<bool>();
		public List<float> NumberOfTies { get; } = new List<float>();
		public List<int[]> Occurr { get; } = new List<int[]>();
		public List<float> OldAim { get; } = new List<float>();
		public List<float> OldBody { get; } = new List<float>();
		public List<int> OldBotNumber { get; } = new List<int>();
		public List<float> OldEnergy { get; } = new List<float>();
		public List<float> OldGD { get; } = new List<float>();
		public List<int> OldMutations { get; } = new List<int>();
		public List<Vector> OldPosition { get; } = new List<Vector>();
		public List<int[]> OldSkin { get; } = new List<int[]>();
		public List<int> Order { get; } = new List<int>();
		public List<float> ParalyzedCount { get; } = new List<float>();
		public List<int> Parent { get; } = new List<int>();
		public List<float> PermanentWaste { get; } = new List<float>();
		public List<int> Point2MutationCycle { get; } = new List<int>();
		public List<int> PointMutationBasePair { get; } = new List<int>();
		public List<int> PointMutationCycle { get; } = new List<int>();
		public List<float> Poison { get; } = new List<float>();
		public List<float> PoisonCount { get; } = new List<float>();
		public List<int> PoisonLocation { get; } = new List<int>();
		public List<int> PosionValue { get; } = new List<int>();
		public List<Vector> Position { get; } = new List<Vector>();
		public List<float> Radius { get; } = new List<float>();
		public List<int> ReptoTimer { get; } = new List<int>();
		public List<float> Shell { get; } = new List<float>();
		public List<Guid> Sim { get; } = new List<Guid>();
		public List<int[]> Skin { get; } = new List<int[]>();
		public List<float> Slime { get; } = new List<float>();
		public List<int> SonNumber { get; } = new List<int>();
		public List<DnaBlock[]> SpermDna { get; } = new List<DnaBlock[]>();
		public List<int> SpermDnaLength { get; } = new List<int>();
		public List<int> SubSpecies { get; } = new List<int>();
		public List<string> Tag { get; } = new List<string>();
		public List<bool[]> TieAngleOverwrite { get; } = new List<bool[]>();
		public List<bool[]> TieLengthOverwrite { get; } = new List<bool[]>();
		public List<Tie[]> Ties { get; } = new List<Tie[]>();
		public List<int[]> UsedVariables { get; } = new List<int[]>();
		public List<bool> UsingNewMove { get; } = new List<bool>();
		public List<int> VariableNumber { get; } = new List<int>();
		public List<Variable[]> Variables { get; } = new List<Variable[]>();
		public List<Vector> Velocity { get; } = new List<Vector>();
		public List<int> VenomLocation { get; } = new List<int>();
		public List<int> VenomValue { get; } = new List<int>();
		public List<float> VirtualBody { get; } = new List<float>();
		public List<bool> VirusImmune { get; } = new List<bool>();
		public List<int> VirusShot { get; } = new List<int>();
		public List<int> VirusTimer { get; } = new List<int>();
		public List<float> Waste { get; } = new List<float>();
	}

    [ComVisible(true)]
	public partial class BotManager
	{
		internal RobotHost Host { get; } = new RobotHost();
		public int GetAbsoluteNumber(int id) {
			if (id < Host.AbsoluteNumber.Count)
				return Host.AbsoluteNumber[id];
			return default;
		}

		public void SetAbsoluteNumber(int id, int value) {
			if (id < Host.AbsoluteNumber.Count)
				Host.AbsoluteNumber[id] = value;
		}

		public Vector GetActualVelocity(int id) {
			if (id < Host.ActualVelocity.Count)
				return Host.ActualVelocity[id];
			return default;
		}

		public void SetActualVelocity(int id, Vector value) {
			if (id < Host.ActualVelocity.Count)
				Host.ActualVelocity[id] = value;
		}

		public float GetAddedMass(int id) {
			if (id < Host.AddedMass.Count)
				return Host.AddedMass[id];
			return default;
		}

		public void SetAddedMass(int id, float value) {
			if (id < Host.AddedMass.Count)
				Host.AddedMass[id] = value;
		}

		public int GetAge(int id) {
			if (id < Host.Age.Count)
				return Host.Age[id];
			return default;
		}

		public void SetAge(int id, int value) {
			if (id < Host.Age.Count)
				Host.Age[id] = value;
		}

		public float GetAim(int id) {
			if (id < Host.Aim.Count)
				return Host.Aim[id];
			return default;
		}

		public void SetAim(int id, float value) {
			if (id < Host.Aim.Count)
				Host.Aim[id] = value;
		}

		public Vector GetAimVector(int id) {
			if (id < Host.AimVector.Count)
				return Host.AimVector[id];
			return default;
		}

		public void SetAimVector(int id, Vector value) {
			if (id < Host.AimVector.Count)
				Host.AimVector[id] = value;
		}

		public int GetAncestorIndex(int id) {
			if (id < Host.AncestorIndex.Count)
				return Host.AncestorIndex[id];
			return default;
		}

		public void SetAncestorIndex(int id, int value) {
			if (id < Host.AncestorIndex.Count)
				Host.AncestorIndex[id] = value;
		}

		public AncestorType[] GetAncestors(int id) {
			if (id < Host.Ancestors.Count)
				return Host.Ancestors[id];
			return default;
		}

		public void SetAncestors(int id, AncestorType[] value) {
			if (id < Host.Ancestors.Count)
				Host.Ancestors[id] = value;
		}

		public float GetAngularMomentum(int id) {
			if (id < Host.AngularMomentum.Count)
				return Host.AngularMomentum[id];
			return default;
		}

		public void SetAngularMomentum(int id, float value) {
			if (id < Host.AngularMomentum.Count)
				Host.AngularMomentum[id] = value;
		}

		public int GetBirthCycle(int id) {
			if (id < Host.BirthCycle.Count)
				return Host.BirthCycle[id];
			return default;
		}

		public void SetBirthCycle(int id, int value) {
			if (id < Host.BirthCycle.Count)
				Host.BirthCycle[id] = value;
		}

		public float GetBody(int id) {
			if (id < Host.Body.Count)
				return Host.Body[id];
			return default;
		}

		public void SetBody(int id, float value) {
			if (id < Host.Body.Count)
				Host.Body[id] = value;
		}

		public VectorInt GetBucketPosition(int id) {
			if (id < Host.BucketPosition.Count)
				return Host.BucketPosition[id];
			return default;
		}

		public void SetBucketPosition(int id, VectorInt value) {
			if (id < Host.BucketPosition.Count)
				Host.BucketPosition[id] = value;
		}

		public bool GetCantReproduce(int id) {
			if (id < Host.CantReproduce.Count)
				return Host.CantReproduce[id];
			return default;
		}

		public void SetCantReproduce(int id, bool value) {
			if (id < Host.CantReproduce.Count)
				Host.CantReproduce[id] = value;
		}

		public bool GetCantSee(int id) {
			if (id < Host.CantSee.Count)
				return Host.CantSee[id];
			return default;
		}

		public void SetCantSee(int id, bool value) {
			if (id < Host.CantSee.Count)
				Host.CantSee[id] = value;
		}

		public int GetChlorophylShareDelay(int id) {
			if (id < Host.ChlorophylShareDelay.Count)
				return Host.ChlorophylShareDelay[id];
			return default;
		}

		public void SetChlorophylShareDelay(int id, int value) {
			if (id < Host.ChlorophylShareDelay.Count)
				Host.ChlorophylShareDelay[id] = value;
		}

		public float GetChloroplasts(int id) {
			if (id < Host.Chloroplasts.Count)
				return Host.Chloroplasts[id];
			return default;
		}

		public void SetChloroplasts(int id, float value) {
			if (id < Host.Chloroplasts.Count)
				Host.Chloroplasts[id] = value;
		}

		public int GetColor(int id) {
			if (id < Host.Color.Count)
				return Host.Color[id];
			return default;
		}

		public void SetColor(int id, int value) {
			if (id < Host.Color.Count)
				Host.Color[id] = value;
		}

		public int GetConditionNumber(int id) {
			if (id < Host.ConditionNumber.Count)
				return Host.ConditionNumber[id];
			return default;
		}

		public void SetConditionNumber(int id, int value) {
			if (id < Host.ConditionNumber.Count)
				Host.ConditionNumber[id] = value;
		}

		public int GetDecayTimer(int id) {
			if (id < Host.DecayTimer.Count)
				return Host.DecayTimer[id];
			return default;
		}

		public void SetDecayTimer(int id, int value) {
			if (id < Host.DecayTimer.Count)
				Host.DecayTimer[id] = value;
		}

		public bool GetDisableDNA(int id) {
			if (id < Host.DisableDNA.Count)
				return Host.DisableDNA[id];
			return default;
		}

		public void SetDisableDNA(int id, bool value) {
			if (id < Host.DisableDNA.Count)
				Host.DisableDNA[id] = value;
		}

		public bool GetDisableMovementSysVars(int id) {
			if (id < Host.DisableMovementSysVars.Count)
				return Host.DisableMovementSysVars[id];
			return default;
		}

		public void SetDisableMovementSysVars(int id, bool value) {
			if (id < Host.DisableMovementSysVars.Count)
				Host.DisableMovementSysVars[id] = value;
		}

		public DnaBlock[] GetDna(int id) {
			if (id < Host.Dna.Count)
				return Host.Dna[id];
			return default;
		}

		public void SetDna(int id, DnaBlock[] value) {
			if (id < Host.Dna.Count)
				Host.Dna[id] = value;
		}

		public int GetDNALength(int id) {
			if (id < Host.DNALength.Count)
				return Host.DNALength[id];
			return default;
		}

		public void SetDNALength(int id, int value) {
			if (id < Host.DNALength.Count)
				Host.DNALength[id] = value;
		}

		public int GetDq(int id) {
			if (id < Host.Dq.Count)
				return Host.Dq[id];
			return default;
		}

		public void SetDq(int id, int value) {
			if (id < Host.Dq.Count)
				Host.Dq[id] = value;
		}

		public float GetEnergy(int id) {
			if (id < Host.Energy.Count)
				return Host.Energy[id];
			return default;
		}

		public void SetEnergy(int id, float value) {
			if (id < Host.Energy.Count)
				Host.Energy[id] = value;
		}

		public int[] GetEpiMem(int id) {
			if (id < Host.EpiMem.Count)
				return Host.EpiMem[id];
			return default;
		}

		public void SetEpiMem(int id, int[] value) {
			if (id < Host.EpiMem.Count)
				Host.EpiMem[id] = value;
		}

		public bool GetExists(int id) {
			if (id < Host.Exists.Count)
				return Host.Exists[id];
			return default;
		}

		public void SetExists(int id, bool value) {
			if (id < Host.Exists.Count)
				Host.Exists[id] = value;
		}

		public int GetFertilized(int id) {
			if (id < Host.Fertilized.Count)
				return Host.Fertilized[id];
			return default;
		}

		public void SetFertilized(int id, int value) {
			if (id < Host.Fertilized.Count)
				Host.Fertilized[id] = value;
		}

		public int GetFlash(int id) {
			if (id < Host.Flash.Count)
				return Host.Flash[id];
			return default;
		}

		public void SetFlash(int id, int value) {
			if (id < Host.Flash.Count)
				Host.Flash[id] = value;
		}

		public string GetFName(int id) {
			if (id < Host.FName.Count)
				return Host.FName[id];
			return default;
		}

		public void SetFName(int id, string value) {
			if (id < Host.FName.Count)
				Host.FName[id] = value;
		}

		public bool[] GetGeneActivations(int id) {
			if (id < Host.GeneActivations.Count)
				return Host.GeneActivations[id];
			return default;
		}

		public void SetGeneActivations(int id, bool[] value) {
			if (id < Host.GeneActivations.Count)
				Host.GeneActivations[id] = value;
		}

		public int GetGeneNumber(int id) {
			if (id < Host.GeneNumber.Count)
				return Host.GeneNumber[id];
			return default;
		}

		public void SetGeneNumber(int id, int value) {
			if (id < Host.GeneNumber.Count)
				Host.GeneNumber[id] = value;
		}

		public int GetGeneration(int id) {
			if (id < Host.Generation.Count)
				return Host.Generation[id];
			return default;
		}

		public void SetGeneration(int id, int value) {
			if (id < Host.Generation.Count)
				Host.Generation[id] = value;
		}

		public float GetGenMut(int id) {
			if (id < Host.GenMut.Count)
				return Host.GenMut[id];
			return default;
		}

		public void SetGenMut(int id, float value) {
			if (id < Host.GenMut.Count)
				Host.GenMut[id] = value;
		}

		public bool GetHasViewed(int id) {
			if (id < Host.HasViewed.Count)
				return Host.HasViewed[id];
			return default;
		}

		public void SetHasViewed(int id, bool value) {
			if (id < Host.HasViewed.Count)
				Host.HasViewed[id] = value;
		}

		public bool GetHighlight(int id) {
			if (id < Host.Highlight.Count)
				return Host.Highlight[id];
			return default;
		}

		public void SetHighlight(int id, bool value) {
			if (id < Host.Highlight.Count)
				Host.Highlight[id] = value;
		}

		public Vector GetImpulseInd(int id) {
			if (id < Host.ImpulseInd.Count)
				return Host.ImpulseInd[id];
			return default;
		}

		public void SetImpulseInd(int id, Vector value) {
			if (id < Host.ImpulseInd.Count)
				Host.ImpulseInd[id] = value;
		}

		public Vector GetImpulseRes(int id) {
			if (id < Host.ImpulseRes.Count)
				return Host.ImpulseRes[id];
			return default;
		}

		public void SetImpulseRes(int id, Vector value) {
			if (id < Host.ImpulseRes.Count)
				Host.ImpulseRes[id] = value;
		}

		public float GetImpulseStatic(int id) {
			if (id < Host.ImpulseStatic.Count)
				return Host.ImpulseStatic[id];
			return default;
		}

		public void SetImpulseStatic(int id, float value) {
			if (id < Host.ImpulseStatic.Count)
				Host.ImpulseStatic[id] = value;
		}

		public bool GetIsCorpse(int id) {
			if (id < Host.IsCorpse.Count)
				return Host.IsCorpse[id];
			return default;
		}

		public void SetIsCorpse(int id, bool value) {
			if (id < Host.IsCorpse.Count)
				Host.IsCorpse[id] = value;
		}

		public bool GetIsDead(int id) {
			if (id < Host.IsDead.Count)
				return Host.IsDead[id];
			return default;
		}

		public void SetIsDead(int id, bool value) {
			if (id < Host.IsDead.Count)
				Host.IsDead[id] = value;
		}

		public bool GetIsMultiBot(int id) {
			if (id < Host.IsMultiBot.Count)
				return Host.IsMultiBot[id];
			return default;
		}

		public void SetIsMultiBot(int id, bool value) {
			if (id < Host.IsMultiBot.Count)
				Host.IsMultiBot[id] = value;
		}

		public bool GetIsParalyzed(int id) {
			if (id < Host.IsParalyzed.Count)
				return Host.IsParalyzed[id];
			return default;
		}

		public void SetIsParalyzed(int id, bool value) {
			if (id < Host.IsParalyzed.Count)
				Host.IsParalyzed[id] = value;
		}

		public bool GetIsPoisoned(int id) {
			if (id < Host.IsPoisoned.Count)
				return Host.IsPoisoned[id];
			return default;
		}

		public void SetIsPoisoned(int id, bool value) {
			if (id < Host.IsPoisoned.Count)
				Host.IsPoisoned[id] = value;
		}

		public bool GetIsVegetable(int id) {
			if (id < Host.IsVegetable.Count)
				return Host.IsVegetable[id];
			return default;
		}

		public void SetIsVegetable(int id, bool value) {
			if (id < Host.IsVegetable.Count)
				Host.IsVegetable[id] = value;
		}

		public bool GetIsWall(int id) {
			if (id < Host.IsWall.Count)
				return Host.IsWall[id];
			return default;
		}

		public void SetIsWall(int id, bool value) {
			if (id < Host.IsWall.Count)
				Host.IsWall[id] = value;
		}

		public int GetKills(int id) {
			if (id < Host.Kills.Count)
				return Host.Kills[id];
			return default;
		}

		public void SetKills(int id, int value) {
			if (id < Host.Kills.Count)
				Host.Kills[id] = value;
		}

		public int GetLastDown(int id) {
			if (id < Host.LastDown.Count)
				return Host.LastDown[id];
			return default;
		}

		public void SetLastDown(int id, int value) {
			if (id < Host.LastDown.Count)
				Host.LastDown[id] = value;
		}

		public int GetLastLeft(int id) {
			if (id < Host.LastLeft.Count)
				return Host.LastLeft[id];
			return default;
		}

		public void SetLastLeft(int id, int value) {
			if (id < Host.LastLeft.Count)
				Host.LastLeft[id] = value;
		}

		public int GetLastMutation(int id) {
			if (id < Host.LastMutation.Count)
				return Host.LastMutation[id];
			return default;
		}

		public void SetLastMutation(int id, int value) {
			if (id < Host.LastMutation.Count)
				Host.LastMutation[id] = value;
		}

		public string GetLastMutationDetail(int id) {
			if (id < Host.LastMutationDetail.Count)
				return Host.LastMutationDetail[id];
			return default;
		}

		public void SetLastMutationDetail(int id, string value) {
			if (id < Host.LastMutationDetail.Count)
				Host.LastMutationDetail[id] = value;
		}

		public int GetLastObject(int id) {
			if (id < Host.LastObject.Count)
				return Host.LastObject[id];
			return default;
		}

		public void SetLastObject(int id, int value) {
			if (id < Host.LastObject.Count)
				Host.LastObject[id] = value;
		}

		public Vector GetLastObjectPosition(int id) {
			if (id < Host.LastObjectPosition.Count)
				return Host.LastObjectPosition[id];
			return default;
		}

		public void SetLastObjectPosition(int id, Vector value) {
			if (id < Host.LastObjectPosition.Count)
				Host.LastObjectPosition[id] = value;
		}

		public int GetLastObjectType(int id) {
			if (id < Host.LastObjectType.Count)
				return Host.LastObjectType[id];
			return default;
		}

		public void SetLastObjectType(int id, int value) {
			if (id < Host.LastObjectType.Count)
				Host.LastObjectType[id] = value;
		}

		public string GetLastOwner(int id) {
			if (id < Host.LastOwner.Count)
				return Host.LastOwner[id];
			return default;
		}

		public void SetLastOwner(int id, string value) {
			if (id < Host.LastOwner.Count)
				Host.LastOwner[id] = value;
		}

		public int GetLastRight(int id) {
			if (id < Host.LastRight.Count)
				return Host.LastRight[id];
			return default;
		}

		public void SetLastRight(int id, int value) {
			if (id < Host.LastRight.Count)
				Host.LastRight[id] = value;
		}

		public int GetLastTouchedRobot(int id) {
			if (id < Host.LastTouchedRobot.Count)
				return Host.LastTouchedRobot[id];
			return default;
		}

		public void SetLastTouchedRobot(int id, int value) {
			if (id < Host.LastTouchedRobot.Count)
				Host.LastTouchedRobot[id] = value;
		}

		public int GetLastUp(int id) {
			if (id < Host.LastUp.Count)
				return Host.LastUp[id];
			return default;
		}

		public void SetLastUp(int id, int value) {
			if (id < Host.LastUp.Count)
				Host.LastUp[id] = value;
		}

		public float GetMass(int id) {
			if (id < Host.Mass.Count)
				return Host.Mass[id];
			return default;
		}

		public void SetMass(int id, float value) {
			if (id < Host.Mass.Count)
				Host.Mass[id] = value;
		}

		public int GetMaxedUsedVariables(int id) {
			if (id < Host.MaxedUsedVariables.Count)
				return Host.MaxedUsedVariables[id];
			return default;
		}

		public void SetMaxedUsedVariables(int id, int value) {
			if (id < Host.MaxedUsedVariables.Count)
				Host.MaxedUsedVariables[id] = value;
		}

		public int[] GetMemory(int id) {
			if (id < Host.Memory.Count)
				return Host.Memory[id];
			return default;
		}

		public void SetMemory(int id, int[] value) {
			if (id < Host.Memory.Count)
				Host.Memory[id] = value;
		}

		public int GetMonitorB(int id) {
			if (id < Host.MonitorB.Count)
				return Host.MonitorB[id];
			return default;
		}

		public void SetMonitorB(int id, int value) {
			if (id < Host.MonitorB.Count)
				Host.MonitorB[id] = value;
		}

		public int GetMonitorG(int id) {
			if (id < Host.MonitorG.Count)
				return Host.MonitorG[id];
			return default;
		}

		public void SetMonitorG(int id, int value) {
			if (id < Host.MonitorG.Count)
				Host.MonitorG[id] = value;
		}

		public int GetMonitorR(int id) {
			if (id < Host.MonitorR.Count)
				return Host.MonitorR[id];
			return default;
		}

		public void SetMonitorR(int id, int value) {
			if (id < Host.MonitorR.Count)
				Host.MonitorR[id] = value;
		}

		public float GetMT(int id) {
			if (id < Host.MT.Count)
				return Host.MT[id];
			return default;
		}

		public void SetMT(int id, float value) {
			if (id < Host.MT.Count)
				Host.MT[id] = value;
		}

		public int GetMultibotTime(int id) {
			if (id < Host.MultibotTime.Count)
				return Host.MultibotTime[id];
			return default;
		}

		public void SetMultibotTime(int id, int value) {
			if (id < Host.MultibotTime.Count)
				Host.MultibotTime[id] = value;
		}

		public MutationProbs GetMutables(int id) {
			if (id < Host.Mutables.Count)
				return Host.Mutables[id];
			return default;
		}

		public void SetMutables(int id, MutationProbs value) {
			if (id < Host.Mutables.Count)
				Host.Mutables[id] = value;
		}

		public int GetMutations(int id) {
			if (id < Host.Mutations.Count)
				return Host.Mutations[id];
			return default;
		}

		public void SetMutations(int id, int value) {
			if (id < Host.Mutations.Count)
				Host.Mutations[id] = value;
		}

		public float GetMutEpiReset(int id) {
			if (id < Host.MutEpiReset.Count)
				return Host.MutEpiReset[id];
			return default;
		}

		public void SetMutEpiReset(int id, float value) {
			if (id < Host.MutEpiReset.Count)
				Host.MutEpiReset[id] = value;
		}

		public int GetNewAge(int id) {
			if (id < Host.NewAge.Count)
				return Host.NewAge[id];
			return default;
		}

		public void SetNewAge(int id, int value) {
			if (id < Host.NewAge.Count)
				Host.NewAge[id] = value;
		}

		public bool GetNoChloroplasts(int id) {
			if (id < Host.NoChloroplasts.Count)
				return Host.NoChloroplasts[id];
			return default;
		}

		public void SetNoChloroplasts(int id, bool value) {
			if (id < Host.NoChloroplasts.Count)
				Host.NoChloroplasts[id] = value;
		}

		public float GetNumberOfTies(int id) {
			if (id < Host.NumberOfTies.Count)
				return Host.NumberOfTies[id];
			return default;
		}

		public void SetNumberOfTies(int id, float value) {
			if (id < Host.NumberOfTies.Count)
				Host.NumberOfTies[id] = value;
		}

		public int[] GetOccurr(int id) {
			if (id < Host.Occurr.Count)
				return Host.Occurr[id];
			return default;
		}

		public void SetOccurr(int id, int[] value) {
			if (id < Host.Occurr.Count)
				Host.Occurr[id] = value;
		}

		public float GetOldAim(int id) {
			if (id < Host.OldAim.Count)
				return Host.OldAim[id];
			return default;
		}

		public void SetOldAim(int id, float value) {
			if (id < Host.OldAim.Count)
				Host.OldAim[id] = value;
		}

		public float GetOldBody(int id) {
			if (id < Host.OldBody.Count)
				return Host.OldBody[id];
			return default;
		}

		public void SetOldBody(int id, float value) {
			if (id < Host.OldBody.Count)
				Host.OldBody[id] = value;
		}

		public int GetOldBotNumber(int id) {
			if (id < Host.OldBotNumber.Count)
				return Host.OldBotNumber[id];
			return default;
		}

		public void SetOldBotNumber(int id, int value) {
			if (id < Host.OldBotNumber.Count)
				Host.OldBotNumber[id] = value;
		}

		public float GetOldEnergy(int id) {
			if (id < Host.OldEnergy.Count)
				return Host.OldEnergy[id];
			return default;
		}

		public void SetOldEnergy(int id, float value) {
			if (id < Host.OldEnergy.Count)
				Host.OldEnergy[id] = value;
		}

		public float GetOldGD(int id) {
			if (id < Host.OldGD.Count)
				return Host.OldGD[id];
			return default;
		}

		public void SetOldGD(int id, float value) {
			if (id < Host.OldGD.Count)
				Host.OldGD[id] = value;
		}

		public int GetOldMutations(int id) {
			if (id < Host.OldMutations.Count)
				return Host.OldMutations[id];
			return default;
		}

		public void SetOldMutations(int id, int value) {
			if (id < Host.OldMutations.Count)
				Host.OldMutations[id] = value;
		}

		public Vector GetOldPosition(int id) {
			if (id < Host.OldPosition.Count)
				return Host.OldPosition[id];
			return default;
		}

		public void SetOldPosition(int id, Vector value) {
			if (id < Host.OldPosition.Count)
				Host.OldPosition[id] = value;
		}

		public int[] GetOldSkin(int id) {
			if (id < Host.OldSkin.Count)
				return Host.OldSkin[id];
			return default;
		}

		public void SetOldSkin(int id, int[] value) {
			if (id < Host.OldSkin.Count)
				Host.OldSkin[id] = value;
		}

		public int GetOrder(int id) {
			if (id < Host.Order.Count)
				return Host.Order[id];
			return default;
		}

		public void SetOrder(int id, int value) {
			if (id < Host.Order.Count)
				Host.Order[id] = value;
		}

		public float GetParalyzedCount(int id) {
			if (id < Host.ParalyzedCount.Count)
				return Host.ParalyzedCount[id];
			return default;
		}

		public void SetParalyzedCount(int id, float value) {
			if (id < Host.ParalyzedCount.Count)
				Host.ParalyzedCount[id] = value;
		}

		public int GetParent(int id) {
			if (id < Host.Parent.Count)
				return Host.Parent[id];
			return default;
		}

		public void SetParent(int id, int value) {
			if (id < Host.Parent.Count)
				Host.Parent[id] = value;
		}

		public float GetPermanentWaste(int id) {
			if (id < Host.PermanentWaste.Count)
				return Host.PermanentWaste[id];
			return default;
		}

		public void SetPermanentWaste(int id, float value) {
			if (id < Host.PermanentWaste.Count)
				Host.PermanentWaste[id] = value;
		}

		public int GetPoint2MutationCycle(int id) {
			if (id < Host.Point2MutationCycle.Count)
				return Host.Point2MutationCycle[id];
			return default;
		}

		public void SetPoint2MutationCycle(int id, int value) {
			if (id < Host.Point2MutationCycle.Count)
				Host.Point2MutationCycle[id] = value;
		}

		public int GetPointMutationBasePair(int id) {
			if (id < Host.PointMutationBasePair.Count)
				return Host.PointMutationBasePair[id];
			return default;
		}

		public void SetPointMutationBasePair(int id, int value) {
			if (id < Host.PointMutationBasePair.Count)
				Host.PointMutationBasePair[id] = value;
		}

		public int GetPointMutationCycle(int id) {
			if (id < Host.PointMutationCycle.Count)
				return Host.PointMutationCycle[id];
			return default;
		}

		public void SetPointMutationCycle(int id, int value) {
			if (id < Host.PointMutationCycle.Count)
				Host.PointMutationCycle[id] = value;
		}

		public float GetPoison(int id) {
			if (id < Host.Poison.Count)
				return Host.Poison[id];
			return default;
		}

		public void SetPoison(int id, float value) {
			if (id < Host.Poison.Count)
				Host.Poison[id] = value;
		}

		public float GetPoisonCount(int id) {
			if (id < Host.PoisonCount.Count)
				return Host.PoisonCount[id];
			return default;
		}

		public void SetPoisonCount(int id, float value) {
			if (id < Host.PoisonCount.Count)
				Host.PoisonCount[id] = value;
		}

		public int GetPoisonLocation(int id) {
			if (id < Host.PoisonLocation.Count)
				return Host.PoisonLocation[id];
			return default;
		}

		public void SetPoisonLocation(int id, int value) {
			if (id < Host.PoisonLocation.Count)
				Host.PoisonLocation[id] = value;
		}

		public int GetPosionValue(int id) {
			if (id < Host.PosionValue.Count)
				return Host.PosionValue[id];
			return default;
		}

		public void SetPosionValue(int id, int value) {
			if (id < Host.PosionValue.Count)
				Host.PosionValue[id] = value;
		}

		public Vector GetPosition(int id) {
			if (id < Host.Position.Count)
				return Host.Position[id];
			return default;
		}

		public void SetPosition(int id, Vector value) {
			if (id < Host.Position.Count)
				Host.Position[id] = value;
		}

		public float GetRadius(int id) {
			if (id < Host.Radius.Count)
				return Host.Radius[id];
			return default;
		}

		public void SetRadius(int id, float value) {
			if (id < Host.Radius.Count)
				Host.Radius[id] = value;
		}

		public int GetReptoTimer(int id) {
			if (id < Host.ReptoTimer.Count)
				return Host.ReptoTimer[id];
			return default;
		}

		public void SetReptoTimer(int id, int value) {
			if (id < Host.ReptoTimer.Count)
				Host.ReptoTimer[id] = value;
		}

		public float GetShell(int id) {
			if (id < Host.Shell.Count)
				return Host.Shell[id];
			return default;
		}

		public void SetShell(int id, float value) {
			if (id < Host.Shell.Count)
				Host.Shell[id] = value;
		}

		public Guid GetSim(int id) {
			if (id < Host.Sim.Count)
				return Host.Sim[id];
			return default;
		}

		public void SetSim(int id, Guid value) {
			if (id < Host.Sim.Count)
				Host.Sim[id] = value;
		}

		public int[] GetSkin(int id) {
			if (id < Host.Skin.Count)
				return Host.Skin[id];
			return default;
		}

		public void SetSkin(int id, int[] value) {
			if (id < Host.Skin.Count)
				Host.Skin[id] = value;
		}

		public float GetSlime(int id) {
			if (id < Host.Slime.Count)
				return Host.Slime[id];
			return default;
		}

		public void SetSlime(int id, float value) {
			if (id < Host.Slime.Count)
				Host.Slime[id] = value;
		}

		public int GetSonNumber(int id) {
			if (id < Host.SonNumber.Count)
				return Host.SonNumber[id];
			return default;
		}

		public void SetSonNumber(int id, int value) {
			if (id < Host.SonNumber.Count)
				Host.SonNumber[id] = value;
		}

		public DnaBlock[] GetSpermDna(int id) {
			if (id < Host.SpermDna.Count)
				return Host.SpermDna[id];
			return default;
		}

		public void SetSpermDna(int id, DnaBlock[] value) {
			if (id < Host.SpermDna.Count)
				Host.SpermDna[id] = value;
		}

		public int GetSpermDnaLength(int id) {
			if (id < Host.SpermDnaLength.Count)
				return Host.SpermDnaLength[id];
			return default;
		}

		public void SetSpermDnaLength(int id, int value) {
			if (id < Host.SpermDnaLength.Count)
				Host.SpermDnaLength[id] = value;
		}

		public int GetSubSpecies(int id) {
			if (id < Host.SubSpecies.Count)
				return Host.SubSpecies[id];
			return default;
		}

		public void SetSubSpecies(int id, int value) {
			if (id < Host.SubSpecies.Count)
				Host.SubSpecies[id] = value;
		}

		public string GetTag(int id) {
			if (id < Host.Tag.Count)
				return Host.Tag[id];
			return default;
		}

		public void SetTag(int id, string value) {
			if (id < Host.Tag.Count)
				Host.Tag[id] = value;
		}

		public bool[] GetTieAngleOverwrite(int id) {
			if (id < Host.TieAngleOverwrite.Count)
				return Host.TieAngleOverwrite[id];
			return default;
		}

		public void SetTieAngleOverwrite(int id, bool[] value) {
			if (id < Host.TieAngleOverwrite.Count)
				Host.TieAngleOverwrite[id] = value;
		}

		public bool[] GetTieLengthOverwrite(int id) {
			if (id < Host.TieLengthOverwrite.Count)
				return Host.TieLengthOverwrite[id];
			return default;
		}

		public void SetTieLengthOverwrite(int id, bool[] value) {
			if (id < Host.TieLengthOverwrite.Count)
				Host.TieLengthOverwrite[id] = value;
		}

		public Tie[] GetTies(int id) {
			if (id < Host.Ties.Count)
				return Host.Ties[id];
			return default;
		}

		public void SetTies(int id, Tie[] value) {
			if (id < Host.Ties.Count)
				Host.Ties[id] = value;
		}

		public int[] GetUsedVariables(int id) {
			if (id < Host.UsedVariables.Count)
				return Host.UsedVariables[id];
			return default;
		}

		public void SetUsedVariables(int id, int[] value) {
			if (id < Host.UsedVariables.Count)
				Host.UsedVariables[id] = value;
		}

		public bool GetUsingNewMove(int id) {
			if (id < Host.UsingNewMove.Count)
				return Host.UsingNewMove[id];
			return default;
		}

		public void SetUsingNewMove(int id, bool value) {
			if (id < Host.UsingNewMove.Count)
				Host.UsingNewMove[id] = value;
		}

		public int GetVariableNumber(int id) {
			if (id < Host.VariableNumber.Count)
				return Host.VariableNumber[id];
			return default;
		}

		public void SetVariableNumber(int id, int value) {
			if (id < Host.VariableNumber.Count)
				Host.VariableNumber[id] = value;
		}

		public Variable[] GetVariables(int id) {
			if (id < Host.Variables.Count)
				return Host.Variables[id];
			return default;
		}

		public void SetVariables(int id, Variable[] value) {
			if (id < Host.Variables.Count)
				Host.Variables[id] = value;
		}

		public Vector GetVelocity(int id) {
			if (id < Host.Velocity.Count)
				return Host.Velocity[id];
			return default;
		}

		public void SetVelocity(int id, Vector value) {
			if (id < Host.Velocity.Count)
				Host.Velocity[id] = value;
		}

		public int GetVenomLocation(int id) {
			if (id < Host.VenomLocation.Count)
				return Host.VenomLocation[id];
			return default;
		}

		public void SetVenomLocation(int id, int value) {
			if (id < Host.VenomLocation.Count)
				Host.VenomLocation[id] = value;
		}

		public int GetVenomValue(int id) {
			if (id < Host.VenomValue.Count)
				return Host.VenomValue[id];
			return default;
		}

		public void SetVenomValue(int id, int value) {
			if (id < Host.VenomValue.Count)
				Host.VenomValue[id] = value;
		}

		public float GetVirtualBody(int id) {
			if (id < Host.VirtualBody.Count)
				return Host.VirtualBody[id];
			return default;
		}

		public void SetVirtualBody(int id, float value) {
			if (id < Host.VirtualBody.Count)
				Host.VirtualBody[id] = value;
		}

		public bool GetVirusImmune(int id) {
			if (id < Host.VirusImmune.Count)
				return Host.VirusImmune[id];
			return default;
		}

		public void SetVirusImmune(int id, bool value) {
			if (id < Host.VirusImmune.Count)
				Host.VirusImmune[id] = value;
		}

		public int GetVirusShot(int id) {
			if (id < Host.VirusShot.Count)
				return Host.VirusShot[id];
			return default;
		}

		public void SetVirusShot(int id, int value) {
			if (id < Host.VirusShot.Count)
				Host.VirusShot[id] = value;
		}

		public int GetVirusTimer(int id) {
			if (id < Host.VirusTimer.Count)
				return Host.VirusTimer[id];
			return default;
		}

		public void SetVirusTimer(int id, int value) {
			if (id < Host.VirusTimer.Count)
				Host.VirusTimer[id] = value;
		}

		public float GetWaste(int id) {
			if (id < Host.Waste.Count)
				return Host.Waste[id];
			return default;
		}

		public void SetWaste(int id, float value) {
			if (id < Host.Waste.Count)
				Host.Waste[id] = value;
		}

	}
}