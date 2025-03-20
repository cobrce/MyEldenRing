namespace MyEldenRing.GameManagers
{
    internal class SaveManager
    {
        private const string BackupExtension = ".mybackup";
        public SaveManager()
        {

        }
        private static bool TryCopy(string source, string destination)
        {
            if (!File.Exists(source))
                return false;
            try { File.Copy(source, destination, true); }
            catch { return false; }
            return true;
        }

        public bool CreateBackup()
        {
            return TryCopy(SaveFile, BackupFile);
        }

        public bool RestoreBackup()
        {
            return TryCopy(BackupFile, SaveFile);
        }

        public bool BackupExists { get => File.Exists(BackupFile); }
        public string BackupFile { get => SaveFile + BackupExtension; }
        public string SaveFile { get; set; } = "";

    }
}
