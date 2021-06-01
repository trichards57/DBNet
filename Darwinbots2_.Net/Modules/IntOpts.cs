using Iersera.Model;

internal static class IntOpts
{
    public const int MAXINTERNETSIMS = 100;
    public const int MAXINTERNETSPECIES = 500;
    public static bool Active { get; set; }
    public static string IName { get; set; } = string.Empty;
    public static string InboundPath { get; set; } = string.Empty;
    public static bool InternetMode { get; set; }
    public static Species[] InternetSpecies { get; set; } = new Species[(MAXINTERNETSPECIES + 1)];
    public static int numInternetSpecies { get; set; }
    public static string OutboundPath { get; set; } = string.Empty;
    public static int pid { get; set; }
    public static string ServIP { get; set; } = string.Empty;
    public static string ServPort { get; set; } = string.Empty;
    public static bool StartInInternetMode { get; set; }
}
