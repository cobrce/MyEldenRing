namespace MyEldenRing
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtSaveFile = new TextBox();
            btnSelectSaveFile = new Button();
            label1 = new Label();
            folderBrowserDialog1 = new FolderBrowserDialog();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            btnBackup = new Button();
            btnRestore = new Button();
            chkQuitout = new CheckBox();
            chkBackup = new CheckBox();
            chkQuitRestore = new CheckBox();
            openFileDialog1 = new OpenFileDialog();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // txtSaveFile
            // 
            txtSaveFile.Location = new Point(83, 10);
            txtSaveFile.Name = "txtSaveFile";
            txtSaveFile.ReadOnly = true;
            txtSaveFile.Size = new Size(330, 27);
            txtSaveFile.TabIndex = 0;
            txtSaveFile.TextChanged += txtSaveFile_TextChanged;
            // 
            // btnSelectSaveFile
            // 
            btnSelectSaveFile.Location = new Point(419, 8);
            btnSelectSaveFile.Name = "btnSelectSaveFile";
            btnSelectSaveFile.Size = new Size(47, 29);
            btnSelectSaveFile.TabIndex = 1;
            btnSelectSaveFile.Text = "...";
            btnSelectSaveFile.UseVisualStyleBackColor = true;
            btnSelectSaveFile.Click += btnSelectSaveFile_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(65, 20);
            label1.TabIndex = 2;
            label1.Text = "Save file";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip1.Location = new Point(0, 166);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(477, 26);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(171, 20);
            lblStatus.Text = "Game process not found";
            // 
            // btnBackup
            // 
            btnBackup.Location = new Point(12, 43);
            btnBackup.Name = "btnBackup";
            btnBackup.Size = new Size(225, 29);
            btnBackup.TabIndex = 9;
            btnBackup.Text = "Create backup";
            btnBackup.UseVisualStyleBackColor = true;
            btnBackup.Click += btnBackup_Click;
            // 
            // btnRestore
            // 
            btnRestore.Enabled = false;
            btnRestore.Location = new Point(240, 43);
            btnRestore.Name = "btnRestore";
            btnRestore.Size = new Size(225, 29);
            btnRestore.TabIndex = 9;
            btnRestore.Text = "Restore";
            btnRestore.UseVisualStyleBackColor = true;
            btnRestore.Click += btnRestore_Click;
            // 
            // chkQuitout
            // 
            chkQuitout.AutoSize = true;
            chkQuitout.Location = new Point(12, 78);
            chkQuitout.Name = "chkQuitout";
            chkQuitout.Size = new Size(150, 24);
            chkQuitout.TabIndex = 10;
            chkQuitout.Text = "ALT+Q to quit out";
            chkQuitout.UseVisualStyleBackColor = true;
            chkQuitout.CheckedChanged += chkQuitout_CheckedChanged;
            // 
            // chkBackup
            // 
            chkBackup.AutoSize = true;
            chkBackup.Location = new Point(12, 108);
            chkBackup.Name = "chkBackup";
            chkBackup.Size = new Size(143, 24);
            chkBackup.TabIndex = 10;
            chkBackup.Text = "ALT+S to backup";
            chkBackup.UseVisualStyleBackColor = true;
            chkBackup.CheckedChanged += chkBackup_CheckedChanged;
            // 
            // chkQuitRestore
            // 
            chkQuitRestore.AutoSize = true;
            chkQuitRestore.Location = new Point(12, 138);
            chkQuitRestore.Name = "chkQuitRestore";
            chkQuitRestore.Size = new Size(282, 24);
            chkQuitRestore.TabIndex = 10;
            chkQuitRestore.Text = "CTRL+SHIFT+Q to quitout and restore";
            chkQuitRestore.UseVisualStyleBackColor = true;
            chkQuitRestore.CheckedChanged += chkQuitRestore_CheckedChanged;
            // 
            // openFileDialog1
            // 
            openFileDialog1.Filter = "Save file|*.sl2|Seamless coop|*.co2|All files|*.*";
            openFileDialog1.Title = "Select save file";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(477, 192);
            Controls.Add(chkQuitRestore);
            Controls.Add(chkBackup);
            Controls.Add(chkQuitout);
            Controls.Add(btnRestore);
            Controls.Add(btnBackup);
            Controls.Add(statusStrip1);
            Controls.Add(label1);
            Controls.Add(btnSelectSaveFile);
            Controls.Add(txtSaveFile);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MyEldenRing";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            Shown += Form1_Shown;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSaveFile;
        private Button btnSelectSaveFile;
        private Label label1;
        private FolderBrowserDialog folderBrowserDialog1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
        private Button btnBackup;
        private Button btnRestore;
        private CheckBox chkQuitout;
        private CheckBox chkBackup;
        private CheckBox chkQuitRestore;
        private OpenFileDialog openFileDialog1;
    }
}
