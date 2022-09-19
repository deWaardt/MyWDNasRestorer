using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WD_File_Recovery;
//using Push;

namespace My_WD_File_Recovery
{
    public partial class MainWindow : Window
    {
        bool firstBtnPressed = false;
        bool secondBtnPressed = false;
        bool thirdBtnPressed = false;
        System.Timers.Timer? uiUpdateTimer;
        System.Timers.Timer? timeTimer;

        string a = "C:\\Users\\domin\\Desktop\\index.db";
        string b = "P:\\restsdk\\data\\files";
        string c = "F:\\BackupDir";

        int timePassed = 0;
        TimeSpan timePassedSpan;
        int calcTimeInt = 0;
        float itemMark1 = 0;
        float itemMark2 = 0;
        DateTime mark1;
        DateTime mark2;

        List<float> avgCalcList = new List<float>();
        private Window _mainw;

        public MainWindow()
        {
            InitializeComponent();
            _mainw = this;
        }

        public async void dbFileSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDbFileDialog = new OpenFileDialog();

            string[] result = await openDbFileDialog.ShowAsync(_mainw);

            if (result != null)
            {
                Session.dbFilePath = openDbFileDialog.InitialFileName;
                dbFileSelectBtn.Content = Session.dbFilePath;
                DBController db = new DBController(Session.dbFilePath);
                Session.allItems = db.GetAll();
                filesDiscoveredLabel.Content = $"{Session.allItems.Count} files/folders discovered";
                firstBtnPressed = true;
            }
        }

        private async void fileFolderSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            var openDbFileDialog = new OpenFolderDialog();
            string result = await openDbFileDialog.ShowAsync(_mainw);

            if (result != null)
            {
                fileFolderSelectBtn.Content = "[ Processing... ]";
                fileFolderSelectBtn.IsEnabled = false;
                dbFileSelectBtn.Content = "[ Scanning... ]";
                fileFolderSelectBtn.IsEnabled = false;
                startBtn.IsEnabled = false;
                new Thread(() =>
                {
                    //Session.filesPath = openDbFileDialog.SelectedPath + "restsdk\\data\\files";
                    //Session.dbFilePath = openDbFileDialog.SelectedPath + "restsdk\\data\\db\\index.db";
                    Session.filesPath = result + "restsdk\\data\\files";
                    Session.dbFilePath = result + "restsdk\\data\\db\\index.db";
                    File.Copy(Session.dbFilePath, "index.db", overwrite: true);
                    //this.Dispatcher.Invoke(() => { dbFileSelectBtn.Content = Session.dbFilePath; });
                    dbFileSelectBtn.Content = Session.dbFilePath;
                    Session.dbFilePath = "index.db";
                    DBController db = new DBController(Session.dbFilePath);
                    Session.allItems = db.GetAll();
                    //this.Dispatcher.Invoke(() =>
                    //{
                        fileFolderSelectBtn.Content = Session.filesPath;
                        filesDiscoveredLabel.Content = $"{Session.allItems.Count} files/folders discovered";
                        startBtn.IsEnabled = true;
                    //});

                    firstBtnPressed = true;
                    secondBtnPressed = true;
                }).Start();
            }
        }

        private async void outputFolderSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            var openDbFileDialog = new OpenFolderDialog();
            string result = await openDbFileDialog.ShowAsync(_mainw);

            if (result != null)
            {
                Session.outputPath = result;
                outputFolderSelectBtn.Content = Session.outputPath;
                thirdBtnPressed = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (firstBtnPressed && secondBtnPressed && thirdBtnPressed)
            {
                Session.current = 1;

                dbFileSelectBtn.IsEnabled = false;
                fileFolderSelectBtn.IsEnabled = false;
                outputFolderSelectBtn.IsEnabled = false;
                startBtn.IsEnabled = false;
                debugStartBtn.IsVisible = false;
                manualSelectBtn.IsEnabled = false;

                uiUpdateTimer = new System.Timers.Timer(20);
                uiUpdateTimer.Elapsed += ATimer_Elapsed;
                uiUpdateTimer.Start();

                timeTimer = new System.Timers.Timer(1000);
                timeTimer.Elapsed += TimeTimer_Elapsed;
                timeTimer.Start();

                timeLeftLbl.Content = "Calculating... Takes a few minutes";

                statusBox.Text = "";

                Thread thread1 = new Thread(ThreadWork.doRecovery);
                thread1.Start();

                startBtn.Content = "Operation started!";
                statusBox.Text = "Preparing...";
            }
            else { startBtn.Content = "Select files and folders fist"; }
        }

        private void TimeTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Session.ready) { return; }
            timePassed += 1;
            calcTimeInt += 1;
            Debug.WriteLine(Session.filesProcessed.Length);



            if (calcTimeInt == 0)
            {
                mark1 = DateTime.Now;
                itemMark1 = (float)Session.current;
            }

            if (calcTimeInt == 1)
            {
                mark2 = DateTime.Now;
                //TimeSpan diff = mark2 - mark1;

                itemMark2 = (float)Session.current;
                float diff = itemMark2 - itemMark1;

                float time = (float)timePassed * (float)Session.totalCount;
                time = time / diff;
                //Debug.WriteLine(diff + " | "+time);

                avgCalcList.Add(time);
                float avgTotal = 0;
                foreach (float u in avgCalcList) { avgTotal += u; }
                float avg = avgTotal / (float)avgCalcList.Count;

                //TimeSpan remaining = new TimeSpan(0, 0, (int)time) - timePassedSpan;
                TimeSpan remaining = new TimeSpan(0, 0, (int)avg) - timePassedSpan;
                if (timePassed > 120)
                {
                    //this.Dispatcher.Invoke(() =>
                    //{
                        timeLeftLbl.Content = remaining.ToString();
                    //});
                }

                if (avgCalcList.Count > 100) { avgCalcList.RemoveRange(0, 50); }

                //Debug.WriteLine(avgCalcList.Count);

                calcTimeInt = 0;

            }

            timePassedSpan = new TimeSpan(0, 0, timePassed);

            //this.Dispatcher.Invoke(() =>
            //{
                elapsedTimeLbl.Content = timePassedSpan.ToString();
            //});

        }

        private void ATimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Session.ready) { return; }
            //if(timePassed >= 1) { Debug.WriteLine(Session.filesProcessed.Length); }

            if (timePassed > 2)
            {
                if (Session.filesProcessed.Length > 2000) { Session.filesProcessed = Session.filesProcessed.Remove(0, 100); }
            }

            //this.Dispatcher.Invoke(() =>
            //{
                decimal progression = (decimal)Session.current / (decimal)Session.totalCount;
                progression = progression * 100;
                progression = Math.Round(progression, 2);
                progressBar.Value = (float)progression;
                percentageLbl.Content = $"{progression}%";
                statusBox.Text = Session.filesProcessed;
                statusBoxScoller.ScrollToEnd();
                amountProcessed.Content = $"Processed: {Session.current}/{Session.totalCount}";
            //});

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            firstBtnPressed = true;
            secondBtnPressed = true;
            thirdBtnPressed = true;
            Session.dbFilePath = a;
            Session.filesPath = b;
            Session.outputPath = c;

            DBController db = new DBController(Session.dbFilePath);
            Session.allItems = db.GetAll();

            Button_Click(sender, e);
        }
    }


    public class ThreadWork
    {
        public static void doRecovery()
        {
            Session.wdddirFiles = Directory.GetFiles(Session.filesPath, "*", SearchOption.AllDirectories);
            Session.totalCount = Session.allItems.Count;
            Session.ready = true;

            foreach (Item i in Session.allItems)
            {
                if (i.id == Session.allItems[0].id || i.id == Session.allItems[1].id) { continue; }
                List<string> PathHistory = new List<string>();

                findParent(i, PathHistory, i);
                Session.current++;
            }
        }


        static void findParent(Item item, List<string> history, Item initialItem)
        {
            if (item.id == Session.allItems[1].id)
            {
                history.Reverse();
                string currentPath = "";
                try
                {
                    history[0] = "My WD Cloud";
                }
                catch { }
                foreach (string i in history) { currentPath += (i + "\\"); }

                if (initialItem.type != "application/x.wd.dir")
                {
                    foreach (string z in Session.wdddirFiles)
                    {
                        List<string> splitString = new List<string>();
                        splitString.AddRange(z.Split(new char[] { '\\' }));
                        if (splitString[splitString.Count - 1] == initialItem.contentID)
                        {
                            Session.currentFile = Session.outputPath + "\\" + currentPath + initialItem.filename;
                            Session.filesProcessed += Session.outputPath + "\\" + currentPath + initialItem.filename + "\n";
                            Directory.CreateDirectory(Session.outputPath + "\\" + currentPath);
                            File.Copy(z, Session.outputPath + "\\" + currentPath + initialItem.filename, overwrite: true);
                        }
                    }
                }
            }
            foreach (Item cur in Session.allItems)
            {
                if (cur.id == item.parentID)
                {
                    history.Add(cur.filename);
                    findParent(cur, history, initialItem);
                    continue;
                }

            }
        }
    }
}