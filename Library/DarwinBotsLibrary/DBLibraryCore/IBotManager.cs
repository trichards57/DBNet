using System.Runtime.InteropServices;

namespace DBLibraryCore
{
    [ComVisible(true)]
    [Guid("0BB68F77-0CD9-4E87-990C-186509EF2195")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBotManager
    {
        /// <summary>
        /// Gets a reference to a new bot.
        /// </summary>
        /// <returns></returns>
        int GetNewBot();
    }
}
