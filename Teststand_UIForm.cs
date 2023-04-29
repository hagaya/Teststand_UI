using IniParser;
using IniParser.Model;
using NationalInstruments.TestStand.Interop.API;
using NationalInstruments.TestStand.Utility;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.CheckedListBox;

namespace Teststand_UI
{


    public partial class Teststand_UIForm : Form
    {

        enum RunningMode { Running, Stopped, StoppedStartOver, Error }
        enum RunType { rtUndefined, rtTestBed, rtInputParameters, rtTests, rtCalibration, rtLabel }
        enum SequenceType { TestBed, InputParameters, Calibration }
        enum UserType { utUnknown, utAdministrator, utOperator }




        TeststandLogic TS = new TeststandLogic();
        FileIniDataParser iniParser = new FileIniDataParser();
        RunningMode runningMode;
        RunType runType;
        SequenceType sequenceType;
        UserType userType;
        bool EnableTracing;
        bool LoggedIn;
        List<int> CheckedArray = new List<int>();
        int TestProgress;
        List<string> StepList = new List<string>();
        int LoopIndex;
        List<CheckedStep> CheckedSteps = new List<CheckedStep>();


        public Teststand_UIForm()
        {
            InitializeComponent();

            runningMode = RunningMode.Stopped;
            runType = RunType.rtUndefined;
            EnableTracing = false;
            LoggedIn = false;
            userType = UserType.utUnknown;

            //LabelStatus.Caption = "";

            // Set Handle
            TS.AppMainWinHandle = this.Handle.ToInt32();

            // Set Events
            TS.ALoginEvent += LoginFinished;
            TS.ALoadSequenceEvent += SequenceLoaded;
            TS.AEndExecutionEvent += EndExecution;
            TS.AStartInteractiveExecutionEvent += StartInteractiveExecution;

            TS.ATraceEvent += TraceSteps2;

            TS.AProgressEvent += ProgressTest;
            TS.AProgressTextEvent += ProgressMessage;


            // Without this listBox1.DrawItem won't fire
            checkedListBox1.DrawMode = DrawMode.OwnerDrawVariable;

            // Subscribe event
            checkedListBox1.DrawItem += checkedListBox1_DrawItem;


            DrawListBoxPicture();

            SetButtons();

            // Start Login
            TS.StartLoginLogoutCallback(false, true);

        }

        private void checkedListBox1_DrawItem(object? sender, DrawItemEventArgs e)
        {

        }

        private void DrawListBoxPicture()
        {

        }

        private void ProgressMessage(string progressString)
        {

        }

        private void ProgressTest(double progressValue)
        {
            if (progressValue > ProgressBarTest.Maximum)
                ProgressBarTest.Value = ProgressBarTest.Maximum;
            else
                ProgressBarTest.Value = Convert.ToInt32(progressValue);
            //LabelProgressTest.Caption := IntToStr(ProgressBarTest.Position) + '%';
        }

        private void TraceSteps2(int currentStepIndex, string PrevioustStepStatus, int loopIndex)
        {
            if (EnableTracing)
            {
                // Set the Last Step Status
                if (checkedListBox1.SelectedIndex > -1 &&
                   currentStepIndex > checkedListBox1.SelectedIndex &&
                   currentStepIndex > -1)
                {
                    checkedListBox1.Items[checkedListBox1.SelectedIndex] = checkedListBox1.Items[checkedListBox1.SelectedIndex] + "  ->  " + PrevioustStepStatus;
                }

                else
                {
                    // Last Step Was Executed
                    if (currentStepIndex == -1 &&
                        checkedListBox1.SelectedIndex != -1 &&
                        checkedListBox1.SelectedIndex == CheckedSteps[CheckedSteps.Count - 1].StepIndex)
                    {
                        checkedListBox1.Items[checkedListBox1.SelectedIndex] = checkedListBox1.Items[checkedListBox1.SelectedIndex] + "  ->  " + PrevioustStepStatus;
                        ProgressTotal();
                        // End of run for Operator mode -> disable Print Label Tracing
                        if (userType != UserType.utAdministrator)
                            EnableTracing = false;
                    }
                }



                // Set New Loop Execution
                if (currentStepIndex == 0 &&
                   checkedListBox1.SelectedIndex == CheckedSteps[CheckedSteps.Count - 1].StepIndex &&
                   checkedListBox1.SelectedIndex > 0 &&
                   userType == UserType.utAdministrator)

                {
                    checkedListBox1.Items[checkedListBox1.SelectedIndex] = checkedListBox1.Items[checkedListBox1.SelectedIndex] + "  ->  " + PrevioustStepStatus;
                    InitiateListBox(true);

                    checkedListBox1.SelectedIndex = 0;
                    LoopIndex = LoopIndex + 1;
                    //LabelCurrentLoop.Caption := IntToStr(LoopIndex);
                    ProgressTotal();

                }

                // Set the Item Correct Index
                if (currentStepIndex > checkedListBox1.SelectedIndex) //OR (currentStepIndex == -1) then
                {
                    checkedListBox1.SelectedIndex = currentStepIndex;
                    if (currentStepIndex > 0)
                        ProgressTotal();

                    checkedListBox1.Refresh();


                }
            }
        }

        private void ProgressTotal()
        {



            if (ProgressBarTotal.Value + ProgressBarTotal.Step > ProgressBarTotal.Maximum)

                ProgressBarTotal.Value = ProgressBarTotal.Maximum;
            else
            {
                ProgressBarTotal.PerformStep();
                if (ProgressBarTotal.Value == (ProgressBarTotal.Step * CheckedSteps.Count * int.Parse(EditLoops.Text))) ;
                ProgressBarTotal.Value = ProgressBarTotal.Maximum;
            }
            //LabelProgressPresents.Caption := IntToStr(ProgressBarTotal.Position) + '%';
            ProgressTest(0);

        }

        private void StartInteractiveExecution()
        {
            SetupStartExecute();
        }

        private void EndExecution()
        {

            if (LoggedIn == true)
            {

                if (runningMode != RunningMode.StoppedStartOver)
                {
                    switch (runType)
                    {
                        case (RunType.rtTestBed):

                            // If passed TestBed execution

                            EnableTracing = false;
                            runType = RunType.rtInputParameters;
                            // Run The Default Sequence
                            sequenceType = SequenceType.InputParameters;
                            GetSequenceDetailsFromIni();
                            runningMode = RunningMode.Running;
                            //StatusBar1.Panels[1].Text := 'End TestBed';
                            TS.UsingModel = false;
                            try
                            {
                                TS.LoadSequence();
                                TS.Execute();
                            }
                            catch (Exception ex)
                            {
                                SetError(ex.Message);
                            }
                            break;
                        case (RunType.rtInputParameters):
                            // Or bring it from the station globals
                            runType = RunType.rtTests;
                            //StatusBar1.Panels[1].Text := 'End Input Parameters';
                            TS.ASequenceFileName = TS.GetSequenceName();
                            TS.ASequenceName = "MainSequence";
                            runningMode = RunningMode.Stopped;
                            TS.UsingModel = true;
                            try
                            {
                                TS.LoadSequence();
                            }
                            catch (Exception ex)
                            {
                                SetError(ex.Message);
                            }
                            break;
                        case (RunType.rtTests):
                            //StatusBar1.Panels[1].Text := "End Tests";
                            runType = RunType.rtTests; //rtUndefined;

                            if (runningMode == RunningMode.Running)
                            {
                                //LabelStatus.Caption := WinTS.ExecutionResult;
                            }
                            runningMode = RunningMode.Stopped;
                            EnableTracing = false;
                            break;
                        case (RunType.rtCalibration):
                            runType = RunType.rtTests;
                            EnableTracing = false;
                            runningMode = RunningMode.StoppedStartOver;
                            break;
                        case (RunType.rtLabel):
                            runType = RunType.rtUndefined;
                            EnableTracing = false;
                            runningMode = RunningMode.Stopped;
                            TS.ASequenceName = "MainSequence";
                            break;
                    }

                }


            }
            SetButtons();
        }

        private void SequenceLoaded(List<string> _stepList)
        {
            StepList = _stepList;
            InitiateListBox(false);
        }

        private void SetButtons()
        {
            switch (userType)
            {
                case UserType.utAdministrator:
                    btnPlay.Visible = true;
                    btnStop.Visible = true;
                    btnStartOver.Visible = true;
                    //ToolButtonReport.Visible = true;
                    //ToolButtonCalibration.Visible = true;
                    //ToolButtonPrinter.Visible = true;
                    //PanelAdministrator.Visible = true;
                    //PanelAdministrator.Enabled = !(runningMode == RunningMode.Running);
                    //PanelListBox.Enabled = !(runningMode == RunningMode.Running);
                    break;
                case UserType.utOperator:
                    btnPlay.Visible = true;
                    btnStop.Visible = true;
                    btnStartOver.Visible = true;
                    //ToolButtonReport.Visible = false;
                    //ToolButtonCalibration.Visible = false;
                    //ToolButtonPrinter.Visible = false;
                    //PanelAdministrator.Visible = false;
                    break;
                case UserType.utUnknown:
                    btnPlay.Visible = false;
                    btnStop.Visible = false;
                    btnStartOver.Visible = false;
                    //ToolButtonReport.Visible = false;
                    //ToolButtonCalibration.Visible = false;
                    //ToolButtonPrinter.Visible = false;
                    //PanelAdministrator.Visible = false;
                    break;
            }


            switch (runningMode)
            {
                case (RunningMode.Stopped):
                    btnPlay.Enabled = true;
                    btnStop.Enabled = false;
                    btnStartOver.Enabled = true;
                    //ToolButtonReport.Enabled = true;
                    //ToolButtonCalibration.Enabled = true;
                    //ToolButtonPrinter.Enabled = true;//RunType = rtTests;

                    checkedListBox1.SelectedIndex = -1;
                    break;
                case (RunningMode.Running):
                    btnPlay.Enabled = false;
                    btnStop.Enabled = true;
                    btnStartOver.Enabled = false;
                    //ToolButtonReport.Enabled = false;
                    //ToolButtonCalibration.Enabled = false;
                    //ToolButtonPrinter.Enabled = false;
                    //PanelLoopIndex.Visible = true;
                    break;
                case (RunningMode.StoppedStartOver):
                    btnPlay.Enabled = false;
                    btnStop.Enabled = false;
                    btnStartOver.Enabled = true;
                    //ToolButtonReport.Enabled = true;
                    //ToolButtonCalibration.Enabled = false;
                    //ToolButtonPrinter.Enabled = true;
                    break;
                case (RunningMode.Error):
                    btnPlay.Enabled = false;
                    btnStop.Enabled = false;
                    btnStartOver.Enabled = true;
                    //ToolButtonReport.Enabled = false;
                    //ToolButtonCalibration.Enabled = true;
                    //ToolButtonPrinter.Enabled = false;
                    break;
            }


        }


        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginToTeststand();
        }

        private void LoginToTeststand()
        {
            TS.StartLoginLogoutCallback(false, true);
        }

        public void LoginFinished(string UserName)
        {

            statusStrip1.Text = "User : " + UserName;

            if (UserName == null || UserName != "")
            {
                if (UserName.ToLower() == "administrator")
                    userType = UserType.utAdministrator;
                else
                    userType = UserType.utOperator;


                LoggedIn = true;
                //PanelListBox.Enabled = (userType == UserType.utAdministrator);
                sequenceType = SequenceType.TestBed;
                GetSequenceDetailsFromIni();

                runType = RunType.rtTestBed;
                // Load the Default (TestBed) Sequence
                // And RUN  Default Sequence
                TS.UsingModel = false;
                try
                {
                    TS.LoadSequence();
                    TS.Execute();

                }
                catch (Exception Ex)
                {
                    SetError(Ex.Message);
                }
            }
            else
            {
                TS.StartLoginLogoutCallback(false, true);
            }
        }

        private void SetError(string ErrMSG)
        {
            TS.CleanAll(false);
            //Engine1.TerminateAll;
            MessageBox.Show(ErrMSG);
            runningMode = RunningMode.Error;
            SetButtons();
        }

        private void GetSequenceDetailsFromIni()
        {
            string SectionName = "";
            switch (sequenceType)
            {
                case (SequenceType.TestBed):
                    SectionName = "TESTBED SEQUENCE";
                    break;
                case (SequenceType.InputParameters):
                    SectionName = "INPUT PARAMETERS SEQUENCE";
                    break;
                case (SequenceType.Calibration):
                    SectionName = "CALIBRATION SEQUENCE";
                    break;

            }

            string iniFilename = Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".ini";


            try
            {
                IniData sectionsData = iniParser.ReadFile(iniFilename);
                TS.ASequenceFileName = sectionsData.Sections[SectionName]["FILE NAME"];
                TS.ASequenceName = sectionsData.Sections[SectionName]["SEQUENCE NAME"];
            }
            catch (Exception Ex)
            {
                SetError(Ex.Message);
            }

        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            Execute();
        }


        private void SetupStartExecute()
        {
            InitiateListBox(true);
            EnableTracing = true;


            //LabelStatus.Caption = "Running";
            checkedListBox1.SelectedIndex = -1;
            ProgressBarTotal.Value = ProgressBarTotal.Minimum;
            //LabelProgressPresents.Caption = "0%";
            ProgressTest(0);
            TS.ATestBedMode = false;

            TS.AInteractiveMode = true;
            CheckedSteps.Clear();
            LoopIndex = 1;
            //LabelCurrentLoop.Caption = LoopIndex.ToString();


            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i) == true)
                {
                    CheckedStep NewCheckedStep = new CheckedStep();
                    NewCheckedStep.StepIndex = i;
                    NewCheckedStep.StepName = checkedListBox1.Items[i].ToString();
                    //NewCheckedStep.stepStatus = StepStatus.Unknown;
                    CheckedSteps.Add(NewCheckedStep);
                }
            }
            if (CheckedSteps.Count > 0)
            {
                ProgressBarTotal.Step = 100 / (CheckedSteps.Count * int.Parse(EditLoops.Text));
            }
            else
            {
                ProgressBarTotal.Step = 100;
            }

        }


        private void Execute()
        {
            runningMode = RunningMode.Running;
            SetButtons();
            TS.ExecuteInterActive(int.Parse(EditLoops.Text), CheckedSteps);

        }

        private void InitiateListBox(bool PreserveChecks)
        {
            if (runType == RunType.rtTests || runType == RunType.rtCalibration)
            {
                if (PreserveChecks)
                {
                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        checkedListBox1.Items[i] = StepList[i];
                    }
                }
                else
                {
                    checkedListBox1.Items.Clear();
                    checkedListBox1.Items.AddRange(StepList.ToArray());
                    CheckAllTheList(CheckState.Checked);
                }

            }
        }

        private void CheckAllTheList(CheckState value)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemCheckState(i, value);
            }

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            EnableTracing = false;
            runningMode = RunningMode.StoppedStartOver;
            //Engine1.TerminateAll();
            checkedListBox1.SelectedIndex = -1;
            //StatusBar1.Panels[1].Text = 'Terminated';

            //LabelStatus.Caption := 'Terminated';
            SetButtons();
        }

        private void btnStartOver_Click(object sender, EventArgs e)
        {
            //TS.TestBedMode = true;
            sequenceType = SequenceType.InputParameters;
            GetSequenceDetailsFromIni();
            runningMode = RunningMode.Stopped;
            //LabelStatus.Caption = '';
            ProgressBarTotal.Value = ProgressBarTotal.Minimum;
            checkedListBox1.Items.Clear();
            //DrawListBoxPicture;
            //EditLoops.Visible = false;

            runType = RunType.rtInputParameters;
            // Load the Default (TestBed) Sequence
            // And RUN  Default Sequence
            TS.UsingModel = false;
            try
            {
                TS.LoadSequence();
                TS.Execute();
            }
            catch (Exception Ex)
            {
                SetError(Ex.Message);
            }
        }
    }
}