using MyEldenRing.GameManagers;
using NHotkey.WindowsForms;
using System.Configuration;
using System.Diagnostics;

namespace MyEldenRing
{
    public partial class Form1 : Form
    {
        const string QuitOutHotkey = "QuitOut";
        const string BackupHotkey = "Backup";
        const string RestoreHotkey = "Restore";

        const string Settings_SaveFileKey = "SaveFile";
        const string Settings_QuitOutKey = "QuitOutHotkey";
        const string Settings_QuitRestoreKey = "QuitRestoreHotkey";
        const string Settings_BackupKey = "BackupHotkey";

        readonly MemoryManager memoryManager = new();
        readonly SaveManager saveManager = new();
        private readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        Process? _gameProcess;
        private bool closed = false;

        public Process? GameProcess
        {
            get { return _gameProcess; }
            private set
            {
                ClosePreviousProcess();
                _gameProcess = value;
                if (!OpenProcess(value)) // either the process couldn't be open or pattern not found
                    _gameProcess = null; // make this null to tell the loop to keep looking

            }
        }

        public Color LblStatusPreviousColor { get; private set; }
        public Color LblSTatusPreviousBackColor { get; private set; }
        public Color StatusStripPreviousBackColor { get; private set; }
        public string? LblStatusPreviousText { get; private set; }

        private bool OpenProcess(Process? gameProcess)
        {
            if (gameProcess == null)
                return false;
            if (memoryManager.Open(gameProcess))
            {
                this.Invoke(() =>
                {
                    lblStatus.Text = $"Process found, ID : {gameProcess.Id}";
                    lblStatus.ForeColor = Color.Gold;
                    statusStrip1.BackColor = Color.Black;
                    lblStatus.BackColor = Color.Black;
                });
                return true;
            }
            return false;

        }

        private void ClosePreviousProcess()
        {
            if (IsHandleCreated && !closed)
                this.Invoke(() =>
                {
                    lblStatus.BackColor = LblSTatusPreviousBackColor;
                    lblStatus.ForeColor = LblStatusPreviousColor;
                    lblStatus.Text = LblStatusPreviousText;
                    statusStrip1.BackColor = StatusStripPreviousBackColor;
                });
            memoryManager.Close();
        }

        public Form1()
        {
            InitializeComponent();
            LblStatusPreviousColor = lblStatus.ForeColor;
            LblSTatusPreviousBackColor = lblStatus.BackColor;
            LblStatusPreviousText = lblStatus.Text;
            StatusStripPreviousBackColor = statusStrip1.BackColor;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtSaveFile.Text = ReadSettings(Settings_SaveFileKey)?.ToString();

            bool.TryParse(ReadSettings(Settings_BackupKey), out bool chkBackupChecked);
            bool.TryParse(ReadSettings(Settings_QuitRestoreKey), out bool chkQuitRestoreChecked);
            bool.TryParse(ReadSettings(Settings_QuitOutKey), out bool chkQuitChecked);

            chkBackup.Checked = chkBackupChecked;
            chkQuitRestore.Checked = chkQuitRestoreChecked;
            chkQuitout.Checked = chkQuitChecked;

        }


        private string? ReadSettings(string key)
        {
            var settings = config.AppSettings.Settings;
            if (settings[key] != null)
                return settings[key].Value;

            return null;
        }


        private void SearchForGame_Loop()
        {
            while (!closed)
            {
                SearchForProcess_Loop();
                WaitForGameToExit();
                ClosePreviousProcess();
            }
        }

        private void WaitForGameToExit()
        {
            while (GameProcess != null && !closed)
                if (GameProcess?.WaitForExit(100) == true)
                    return;
        }

        private void SearchForProcess_Loop()
        {
            Process? process;
            while (true)
            {
                if (closed) return;
                process = Process.GetProcessesByName("eldenring")
                                        .FirstOrDefault();
                if (process != null)
                    break;

                Thread.Sleep(100);
            }
            Invoke(() =>
            {
                this.GameProcess = process;
            });
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            var processLoopThread = new Thread(SearchForGame_Loop);
            processLoopThread.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.closed = true;
        }

        private static void ToggleHotkey(string hotkeyname, Keys keys, EventHandler<NHotkey.HotkeyEventArgs> handler, bool enabled)
        {
            if (enabled)
                HotkeyManager.Current.AddOrReplace(hotkeyname, keys, handler);
            else
                HotkeyManager.Current.Remove(hotkeyname);
        }

        private void chkQuitout_CheckedChanged(object sender, EventArgs e)
        {
            ToggleHotkey(QuitOutHotkey, Keys.Alt | Keys.Q, OnQuitOut, chkQuitout.Checked);
            SaveSettings(Settings_QuitOutKey, chkQuitout.Checked.ToString());
        }
        private void OnQuitOut(object? sender, NHotkey.HotkeyEventArgs e)
        {
            e.Handled = true;
            HotkeyManager.Current.IsEnabled = false;
            memoryManager.QuitOutGame();
            HotkeyManager.Current.IsEnabled = true;
        }

        private void chkBackup_CheckedChanged(object sender, EventArgs e)
        {
            ToggleHotkey(BackupHotkey, Keys.Alt | Keys.S, OnBackup, chkBackup.Checked);
            SaveSettings(Settings_BackupKey, chkBackup.Checked.ToString());
        }
        private void OnBackup(object? sender, NHotkey.HotkeyEventArgs e)
        {
            e.Handled = true;
            saveManager.CreateBackup();
            UpdateBtnRestore();
        }

        private void chkQuitRestore_CheckedChanged(object sender, EventArgs e)
        {
            ToggleHotkey(RestoreHotkey, Keys.Control | Keys.Shift | Keys.Q, OnRestore, chkQuitRestore.Checked);
            SaveSettings(Settings_QuitRestoreKey, chkQuitRestore.Checked.ToString());
        }
        private void OnRestore(object? sender, NHotkey.HotkeyEventArgs e)
        {
            e.Handled = true;
            HotkeyManager.Current.IsEnabled = false;
            memoryManager.QuitOutGame();
            Thread.Sleep(100);
            saveManager.RestoreBackup();
            HotkeyManager.Current.IsEnabled = true;
        }


        private void txtSaveFile_TextChanged(object sender, EventArgs e)
        {
            if (!File.Exists(txtSaveFile.Text))
                return;

            saveManager.SaveFile = txtSaveFile.Text;

            UpdateBtnRestore();

        }

        private bool UpdateBtnRestore()
        {
            return (btnRestore.Enabled = saveManager.BackupExists);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (!UpdateBtnRestore())
                return;

            saveManager.RestoreBackup();
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            saveManager.CreateBackup();
            UpdateBtnRestore();
        }

        private void btnSelectSaveFile_Click(object sender, EventArgs e)
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var ERdir = Path.Combine(appdata, "EldenRing").ToString();
            openFileDialog1.InitialDirectory = Directory.Exists(ERdir) ? ERdir : appdata;
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            txtSaveFile.Text = openFileDialog1.FileName;
            SaveSaveFileSettings(openFileDialog1.FileName);
            UpdateBtnRestore();

        }


        private void SaveSaveFileSettings(string filename)
        {
            SaveSettings(Settings_SaveFileKey, filename);
        }

        private void SaveSettings(string key, string value)
        {
            var settings = config.AppSettings.Settings;
            if (settings[key] == null)
                settings.Add(key, value);
            else
                settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }
    }
}
