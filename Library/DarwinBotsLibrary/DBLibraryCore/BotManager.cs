using System;
using System.Runtime.InteropServices;

namespace DBLibraryCore
{
    [ComVisible(true)]
    [Guid("1A6DD3A6-3A38-4057-B793-C10970BB8800")]
    public class BotManager : IBotManager
    {
        /// <summary>
        /// Gets a reference to a new bot.
        /// </summary>
        /// <returns></returns>
        public int GetNewBot()
        {
            throw new NotImplementedException();
        }
    }
}
