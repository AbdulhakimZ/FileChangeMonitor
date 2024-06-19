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
        private string lastChangeType;

        private DateTime lastChangeTime;


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
                    lastChangeTime = DateTime.Now;
                }
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            System.Threading.Thread.Sleep(100);

            string currentContent = string.Empty;
            try
            {
                currentContent = File.ReadAllText(targetFilePath);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return;
            }
            lastChangeType = GetChangeType(e);
            lastChangeTime = DateTime.Now;
        }
        private void UpdateListView(ListViewItem item)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate {
                    UpdateListView(item);
                }));
                return;
            }

            lstChanges.Items.Add(item);
        }
        private string GetChangeType(FileSystemEventArgs e)
        {
            string changeType = string.Empty;
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                changeType = "File changed";
            }
            else if (e.ChangeType == WatcherChangeTypes.Created)
            {
                changeType = "File created";
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                changeType = "File deleted";
            }
            return changeType;
        }

        private void OnBatchTimerTick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(targetFilePath))
                return;

            string currentContent = File.ReadAllText(targetFilePath);
            if (currentContent != previousContent)
            {
                ListViewItem item = new ListViewItem(new[] {
                    lastChangeTime.ToString(),
                    lastChangeType,
                    previousContent,
                    currentContent
                });
                UpdateListView(item);
                previousContent = currentContent;
            }

        }
    }
}
