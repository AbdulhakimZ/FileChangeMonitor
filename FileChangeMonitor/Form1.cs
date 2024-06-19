using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileChangeMonitor
{
    public partial class Form1 : Form
    {
        private string targetFilePath;
        private FileSystemWatcher fileWatcher;
        private Timer batchTimer;
        private string previousContent;

        public Form1()
        {
            InitializeComponent();
            InitializeFileWatcher();
            InitializeBatchTimer();
        }
        private void InitializeFileWatcher()
        {
            fileWatcher = new FileSystemWatcher();
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.Changed += OnFileChanged;
        }
        private void InitializeBatchTimer()
        {
            batchTimer = new Timer();
            batchTimer.Interval = 15000; // 15 seconds
            batchTimer.Tick += OnBatchTimerTick;
            batchTimer.Start();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    targetFilePath = openFileDialog.FileName;
                    txtFilePath.Text = targetFilePath;
                    fileWatcher.Path = Path.GetDirectoryName(targetFilePath);
                    fileWatcher.Filter = Path.GetFileName(targetFilePath);
                    fileWatcher.EnableRaisingEvents = true;
                    previousContent = File.ReadAllText(targetFilePath);
                }
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            
        }

        private void OnBatchTimerTick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(targetFilePath))
                return;

            string currentContent = File.ReadAllText(targetFilePath);
            if (currentContent != previousContent)
            {
                string changes = GetChanges(previousContent, currentContent);
                ReportChanges(changes);
                previousContent = currentContent;
            }
        }

        private string GetChanges(string oldContent, string newContent)
        {
            return newContent;
        }

        private void ReportChanges(string changes)
        {
            lstChanges.Items.Add(changes);
        }
    }
}
