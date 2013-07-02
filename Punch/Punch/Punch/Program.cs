using System;

namespace Punch
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Punch game = new Punch())
            {
                game.Run();
            }
        }
    }
#endif
}

