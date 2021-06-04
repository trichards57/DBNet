namespace Iersera.DataModel
{
    public class RestrictionsPresetFile
    {
        public bool DisableChloroplastsNonVeg { get; set; }
        public bool DisableDnaNonVeg { get; set; }
        public bool DisableDnaVeg { get; set; }
        public bool DisableMotionNonVeg { get; set; }
        public bool DisableMotionVeg { get; set; }
        public bool DisableMutationsNonVeg { get; set; }
        public bool DisableMutationsVeg { get; set; }
        public bool DisableReproductionNonVeg { get; set; }
        public bool DisableReproductionVeg { get; set; }
        public bool DisableVisionNonVeg { get; set; }
        public bool DisableVisionVeg { get; set; }
        public bool FixedInPlaceNonVeg { get; set; }
        public bool FixedInPlaceVeg { get; set; }
        public bool KillNonMultibotNonVeg { get; set; }
        public bool KillNonMultibotVeg { get; set; }
        public bool VirusImmuneNonVeg { get; set; }
        public bool VirusImmuneVeg { get; set; }
    }
}
