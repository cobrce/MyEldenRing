namespace MyEldenRing
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var memman = new GameManagers.MemoryManager();
            //memman.Open(25796, "E8 ?? ?? ?? ?? 33 C0 C3 8B D4 8B 02 50", 1);
            //return;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}