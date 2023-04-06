using Microsoft.VisualBasic.ApplicationServices;
using NationalInstruments.TestStand.Interop.API;
using NationalInstruments.TestStand.Interop.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Teststand_UI.TeststandLogic;

namespace Teststand_UI
{
    public enum StepStatus
    {
        Unknown, Passed, Failed
    }

    public class CheckedStep
    {
        public string StepName;
        public int StepIndex;
        public StepStatus stepStatus;
    }


    internal class TeststandLogic
    {

        enum ShutDownType
        {
            ShutDown_NotShuttingDown = 0,
            ShutDown_LogoutAndExit = 1,
            ShutDown_LogoutOnly = 2,
            ShutDown_Exit = 3,
            ShutDown_LogoutOnlyComplete = 4,
            ShutDown_ExitPending = 5
        }

        // Global variables
        Engine engine;
        NationalInstruments.TestStand.Interop.API.Thread mFgThread;
        SequenceContext mCurrentContext;
        SequenceContext MainTestsContext;
        SequenceFile mCurrentModelSequence;
        SequenceFile mCurrentSequenceFile;

        bool MainTestsContextSet;

        string StationID;

        ShutDownType ShutdownPass;
        public int AppMainWinHandle;
        int mLoginLogoutExeId;

        bool mLoginLogoutCallbackRunning;
        bool mDoDeferredFinalExit;
        public bool UsingModel;

        Execution CurrentExecution;

        long mFrameId;

        public string ASequenceFileName;
        public string ASequenceName;
        string ALabelSequence;

        public bool ATestBedMode;
        public bool AInteractiveMode;

        int mPointedAtStepIndex;
        string mPointedStepName;
        string mPreviousStepResultStatus;
        StepGroups mExePhaseStepGroup;
        string mCurrUserName;
        List<string> mStepNames;
        string AExecutionStatus;
        string AExecutionResult;
        ExecutionRunStates ArunState;
        ExecutionTerminationStates AtermState;
        List<Sequence> mEntryPointSeqs;

        public delegate void TraceEvent(int pointedAtStepIndex, string previousStepResultStatus, int loopIndex);
        public event TraceEvent ATraceEvent;

        public delegate void ProgressEvent(double progressValue);
        public event ProgressEvent AProgressEvent;
        
        public delegate void ProgressTextEvent(string progressString);
        public event ProgressTextEvent AProgressTextEvent;
        
        public delegate void EndExecutionEvent();
        public event EndExecutionEvent AEndExecutionEvent;

        public delegate void StartInteractiveExecutionEvent();
        public event StartInteractiveExecutionEvent AStartInteractiveExecutionEvent;
        
        public delegate void LoadSequenceEvent(List<string> StepList);
        public event LoadSequenceEvent ALoadSequenceEvent;
        
        public delegate void LoginEvent(string userName);
        public event LoginEvent ALoginEvent;


        public TeststandLogic() 
        {
            engine = new Engine();
            engine.UIMessageEvent += Engine_UIMessageEvent;
            
            //engine.LoadTypePaletteFiles();

            mStepNames = new List<string>();
            mEntryPointSeqs = new List<Sequence> ();

            //engine.StationOptions.TracingEnabled = true;
            //engine.StationOptions.ExecutionMask += ExecutionMask.ExecMask_TracingEnabled;
            //engine.StationOptions.ExecutionMask += ExecutionMask.ExecMask_TraceWhileTerminating;
            
            //TestBedMode = true;
            AInteractiveMode = false; 
        }

        ~TeststandLogic()
        {
            CleanAll(true);
            engine.UnloadAllModules();
            engine.ShutDown(true);
        }

        private void Engine_UIMessageEvent(UIMessage msg)
        {

            switch (msg.Event)
            {
                case UIMessageCodes.UIMsg_StartExecution:
                    // Nothing for now
                    break;

                case UIMessageCodes.UIMsg_StartInteractiveExecution:
                    // Nothing for now
                    AStartInteractiveExecutionEvent?.Invoke();
                    break;

                case UIMessageCodes.UIMsg_EndExecution:
                    AEndExecutionEvent?.Invoke();
                    // Check for the LoginLogout execution

                    if (mLoginLogoutCallbackRunning && msg.Execution.Id == mLoginLogoutExeId)
                    {
                        mLoginLogoutCallbackRunning = false;
                        // update current user name
                        mCurrUserName = GetLoginName();

                        switch (ShutdownPass)
                        {
                            case (ShutDownType.ShutDown_Exit):
                                // logout completed - request final shutdown
                                engine.ShutDown(true);
                                break;
                            case (ShutDownType.ShutDown_LogoutOnlyComplete):
                                // logout completed - request 2nd shutdown
                                engine.ShutDown(false);
                                break;
                            default:
                                ALoginEvent?.Invoke(mCurrUserName);
                                break;
                        }
                    }
                    else
                        GetExecutionState();
                    break;


                case UIMessageCodes.UIMsg_Trace:
                    UpdateSavedContextInfo(msg.Thread);

                    if (mExePhaseStepGroup == StepGroups.StepGroup_Main)
                    {
                        ATraceEvent?.Invoke(mPointedAtStepIndex, mPreviousStepResultStatus, mCurrentContext.LoopIndex);
                    }
                    break;



                case UIMessageCodes.UIMsg_ProgressPercent:
                    mCurrentContext.StationGlobals.SetValNumber("progress_bar", 0, Math.Truncate( msg.NumericData));
                    AProgressEvent?.Invoke(Math.Truncate(msg.NumericData));
                    break;


                case UIMessageCodes.UIMsg_ProgressText:
                    AProgressTextEvent?.Invoke(msg.StringData);
                    break;


                case UIMessageCodes.UIMsg_EndInteractiveExecution:
                    GetExecutionState();
                    if (ArunState == ExecutionRunStates.ExecRunState_Stopped)
                        AEndExecutionEvent.Invoke();
                    break;

                case UIMessageCodes.UIMsg_ShutDownComplete:

                    switch (ShutdownPass)
                    {
                        case (ShutDownType.ShutDown_LogoutAndExit):
                            ShutdownPass = ShutDownType.ShutDown_Exit;
                            if (!StartLoginLogoutCallback(true, false))
                            {
                                engine.ShutDown(true);
                            }


                            break;
                        case ShutDownType.ShutDown_LogoutOnly:

                            ShutdownPass = ShutDownType.ShutDown_LogoutOnlyComplete;
                            // start logout
                            if (! StartLoginLogoutCallback(true, false))
                            {
                                // couldnt run logout callback
                                // request shutdown now
                                engine.ShutDown(false);
                            }
                            break;
                        case ShutDownType.ShutDown_LogoutOnlyComplete:

                            ShutdownPass = ShutDownType.ShutDown_NotShuttingDown;
                            // bring up login dialog
                            StartLoginLogoutCallback(false, false);
                            break;
                        case ShutDownType.ShutDown_Exit:
                            // we can exit now
                            ShutdownPass = ShutDownType.ShutDown_ExitPending;
                            mDoDeferredFinalExit = true;    
                            break;
                    }
                    break;


                case UIMessageCodes.UIMsg_ShutDownCancelled:
                    ShutdownPass = ShutDownType.ShutDown_NotShuttingDown;
                    break;
            }
            msg.Acknowledge();


        }

        private void TerminateExecution ()
        {
            if (CurrentExecution.InInteractiveMode)
                CurrentExecution.TerminateInteractiveExecution();
            else
                CurrentExecution.Terminate();
        }

        private void DestroyCurrentExecution (bool Abort)
        {
            if (CurrentExecution != null)
            {
                if (Abort)
                {
                    CurrentExecution.Abort();
                }
                else
                {
                    TerminateExecution();
                }
                CurrentExecution = null;
            }
        }

        public void CleanAll (bool Abort)
        {
            DestroyCurrentExecution(Abort);
            if (mCurrentSequenceFile != null)
            {
                engine.ReleaseSequenceFileEx(mCurrentSequenceFile);
            }
            if (mCurrentModelSequence != null)
            {
                engine.ReleaseSequenceFileEx(mCurrentModelSequence);
            }
            if (Abort)
                engine.AbortAll();
            else
                engine.TerminateAll();

            mFgThread = null;
            mCurrentContext = null;
            MainTestsContext = null;
            mCurrentModelSequence = null;
            mCurrentSequenceFile = null;
            mEntryPointSeqs.Clear();
        }

        

        private void GetModelEntryPoints (SequenceFile modelSeqFile, bool forExeDisplay, bool seqFileHasModel)
        {
            Sequence currentSeq;
            int numSeqs = 0;
            if (modelSeqFile != null)
            {
                numSeqs = modelSeqFile.NumSequences;
            }
    
            if (numSeqs > 0)
            {
                for (int i=0; i<numSeqs;i++)
                {
                    currentSeq = modelSeqFile.GetSequence(i);
                    if (currentSeq.Type == SequenceTypes.SeqType_ExeEntryPoint
                        &&
                        ! currentSeq.ShowEntryPointForEditorOnly
                        &&
                        (currentSeq.ShowEntryPointForFileWindow || currentSeq.ShowEntryPointForAllWindows) )
                    {
                        mEntryPointSeqs.Add(currentSeq);
                    }
                }
            }

        }

        private SequenceFile GetModelSeqFile(SequenceFile mCurrentSeqFile)
        {
            string modelDesc = "";
            SequenceFile Result = null;
            if (mCurrentSeqFile != null) 
                if (mCurrentSeqFile.HasModel)
                    Result = mCurrentSeqFile.GetModelSequenceFile(out modelDesc);

            if (Result == null)
                    Result = engine.GetStationModelSequenceFile(out modelDesc);

            return Result;
        }


        private double GetRunID ()
        {
            PropertyObject ATEGlobals;

            if (mCurrentContext != null)
            {
                ATEGlobals = mCurrentContext.StationGlobals.GetPropertyObject("ATEGlobals", 0);
                if (ATEGlobals != null)
                    return ATEGlobals.GetValNumber("RunID", 0);
            }
            return -1.0;

        }

        public string GetSequenceName ()
        {
            if (mCurrentContext != null)
                return mCurrentContext.StationGlobals.GetValString("sequenceName", 0);
            else
                return "";
        }

        private bool IsBaseSequence()
        {
            int frameID = 0;
            SequenceContext CurrentSequenceContext = mFgThread.GetSequenceContext(0, out frameID);
            mFrameId = frameID;
            return mCurrentContext.Id == CurrentSequenceContext.Id;
        }

        private bool IsSupportedEntryPointSeq(Sequence EntryPtSeq)
        {
            return 
                (EntryPtSeq.Type == SequenceTypes.SeqType_CfgEntryPoint || EntryPtSeq.Type == SequenceTypes.SeqType_ExeEntryPoint) 
                &&
                (! EntryPtSeq.ShowEntryPointForEditorOnly);
        }

        public void LoadSequence ()
        {
            if (mCurrentSequenceFile != null)
                engine.ReleaseSequenceFileEx(mCurrentSequenceFile);
            mCurrentSequenceFile = null;
            
            mCurrentSequenceFile = engine.GetSequenceFileEx(ASequenceFileName, GetSeqFileOptions.GetSeqFile_OperatorInterfaceFlags);

            if (mCurrentSequenceFile != null)
              ASequenceFileName = mCurrentSequenceFile.Path;

            if (UsingModel == true)
              mCurrentModelSequence = GetModelSeqFile(mCurrentSequenceFile);

            Sequence AMainSequence = mCurrentSequenceFile.GetSequenceByName(ASequenceName);

            int StepNumber = AMainSequence.GetNumSteps(StepGroups.StepGroup_Main);

            mStepNames.Clear();
            for (int i = 0; i<StepNumber; i++)        
              mStepNames.Add(AMainSequence.GetStep(i, StepGroups.StepGroup_Main).Name);

            ALoadSequenceEvent?.Invoke(mStepNames);

        }

        private void ReleaseExecution ()
        {
            DestroyCurrentExecution(false);
            mFgThread = null;
            mCurrentContext = null;
            MainTestsContext = null;
            CurrentExecution = null;
        }


        private void SetAppMainWinHandle (int winHandler)
        {
            AppMainWinHandle = winHandler;
            engine.AppMainHwnd = AppMainWinHandle;
        }

        public void Execute()
        {
            SequenceFile nullSequence = null;
            CurrentExecution = engine.NewExecution(mCurrentSequenceFile,
                                                  ASequenceName,
                                                  null,
                                                  false,
                                                  ExecutionTypeMask.ExecTypeMask_Normal);
            CurrentExecution.Resume();
            mFgThread = CurrentExecution.GetThread(0);
            int frameID = 0;
            mCurrentContext = mFgThread.GetSequenceContext(0, out frameID);

            mFrameId = frameID;


        }

        public void ExecuteInterActive(int Loops, List<CheckedStep> Steps)
        {
            mEntryPointSeqs = new List<Sequence> ();

            GetModelEntryPoints(mCurrentModelSequence, true, mCurrentSequenceFile.HasModel);

            InteractiveArgs interExeArgs = engine.NewInteractiveArgs();
            EditArgs EditArgsObj = engine.NewEditArgs();

            EditArgsObj.SetSelectedSequenceFile(mCurrentSequenceFile);
            Sequence AMainSequence = mCurrentSequenceFile.GetSequence(0);

            if (AMainSequence == null) // No Main Sequence
                return;

            EditArgsObj.AddSelectedSequence(AMainSequence);
            EditArgsObj.SetSelectedStepGroup(StepGroups.StepGroup_Main);

            int StepNum = AMainSequence.GetNumSteps(StepGroups.StepGroup_Main);

            Step AStep;
            for (int i=0;i<StepNum;i++)
            {
                AStep = AMainSequence.GetStep(i, StepGroups.StepGroup_Main);
                EditArgsObj.AddSelectedStep(AStep);
            }


            interExeArgs.Sequence = AMainSequence;
            //set the loop count
            interExeArgs.LoopCount = Loops;
            // set the step group
            interExeArgs.StepGroup = StepGroups.StepGroup_Main; // ????  StepGroup_Main


            if (Steps != null && Steps.Count > 0)
            {
                for (int i=0; i<Steps.Count; i++)
                {
                    interExeArgs.AddStepIndex(Steps[i].StepIndex);
                }
            }
            else
            {
                for (int i = 0; i < StepNum; i++)
                {
                    interExeArgs.AddStepIndex(i);
                }

            }

            AMainSequence.ShowEntryPointForEditorOnly = false;

            PropertyObject Container = engine.NewPropertyObject(PropertyValueTypes.PropValType_Container,false,"",0);
            Container.SetValString("Sequence", PropertyOptions.PropOption_InsertIfMissing, AMainSequence.Name);

            string EntryPointSeqName = "";
            if (mEntryPointSeqs.Count > 0)
                EntryPointSeqName = mEntryPointSeqs[0].Name;
            else
                EntryPointSeqName = ASequenceName;
            
            CurrentExecution = engine.NewExecution(mCurrentSequenceFile,
                                        EntryPointSeqName,   
                                        mCurrentModelSequence,
                                        false,
                                        ExecutionTypeMask.ExecTypeMask_Normal,
                                        Container,
                                        EditArgsObj,
                                        interExeArgs);

            mFgThread = CurrentExecution.GetThread(0);
            int frameID = 0;
            mCurrentContext = mFgThread.GetSequenceContext(0, out frameID);
            mCurrentContext.Tracing = true;

            mFrameId = frameID;
        }


        private string GetExecutionResult()
        {
            return CurrentExecution.ResultStatus;
        }


        public bool StartLoginLogoutCallback(bool logoutOnly, bool isInitialLogin)
        {
            PropertyObject args;

            try
            {
                args = engine.NewPropertyObject(PropertyValueTypes.PropValType_Container, false, "", 0);
                args.SetValBoolean("logoutOnly", PropertyOptions.PropOption_InsertIfMissing, logoutOnly);
                args.SetValBoolean("isInitialLogin", PropertyOptions.PropOption_InsertIfMissing, isInitialLogin);
                //mLoginLogoutExeId = engine.CallFrontEndCallbackEx("LoginLogout", args).Id;
                NationalInstruments.TestStand.Interop.API.User userObject;
                bool OK = engine.DisplayLoginDialog("Login to Fabric Tester", "Fabric Operator", "", true, out userObject);
                mLoginLogoutCallbackRunning = true;
                mCurrUserName = "";
                if (OK)
                    mCurrUserName = userObject.LoginName;

                ALoginEvent?.Invoke(mCurrUserName);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }


        }

        private string GetLoginName()
        {
            if (engine.CurrentUser == null)
                return "";
            else
                return engine.CurrentUser.LoginName;
        }

        private string GetExecutionState()
        {
            string item = "PANEL_TITLE_STATUS_COMPLETED";


            if (CurrentExecution != null)
            {
                CurrentExecution.GetStates(out ArunState, out AtermState);

                switch (ArunState)
                {
                    case (ExecutionRunStates.ExecRunState_Paused):
                        item = "PANEL_TITLE_STATUS_PAUSED";
                        break;

                    case (ExecutionRunStates.ExecRunState_Running):
                        switch (AtermState)
                        {
                            case ExecutionTerminationStates.ExecTermState_Normal:
                                item = "PANEL_TITLE_STATUS_RUNNING";
                                break;

                            case ExecutionTerminationStates.ExecTermState_Terminating:
                                item = "PANEL_TITLE_STATUS_TERMINATING";
                                break;

                            case ExecutionTerminationStates.ExecTermState_Aborting:
                                item = "PANEL_TITLE_STATUS_ABORTING";
                                break;

                            case ExecutionTerminationStates.ExecTermState_KillingThreads:
                                item = "PANEL_TITLE_STATUS_KILLING_THREADS";
                                break;
                        }
                        break;


                    case (ExecutionRunStates.ExecRunState_Stopped):
                        switch (AtermState)
                        {
                            case ExecutionTerminationStates.ExecTermState_Normal:
                                item = "PANEL_TITLE_STATUS_COMPLETED";
                                break;

                            case ExecutionTerminationStates.ExecTermState_Terminating:
                                item = "PANEL_TITLE_STATUS_TERMINATED";
                                break;

                            case ExecutionTerminationStates.ExecTermState_Aborting:
                                item = "PANEL_TITLE_STATUS_ABORTED";
                                break;

                            case ExecutionTerminationStates.ExecTermState_KillingThreads:
                                item = "PANEL_TITLE_STATUS_THREADS_KILLED";
                                break;
                        }
                        break;
                }

            }
            object unused;
            item = engine.GetResourceString("TSUI_OI_MAIN_PANEL", "OP_INT_EXE_DISP_PANEL", "", out unused);
            return item;
        }

        private void UpdateSavedContextInfo(NationalInstruments.TestStand.Interop.API.Thread thread)
        {

            bool fullUpdateNeeded = false;
            SequenceContext newContext;
            int newFrameId;
            Step? mCurrentStep;

            newContext = mFgThread.GetSequenceContext(0, out newFrameId);

            // Update content
            if (mCurrentContext == null || MainTestsContext == null || mFrameId != newFrameId)
            {
                // A full update is needed
                string seqFilePath = newContext.SequenceFile.Path;
                string seqName = newContext.Sequence.Name;
                if (seqFilePath == ASequenceFileName && seqName == ASequenceName)
                {
                    MainTestsContext = newContext;
                    MainTestsContextSet = true;
                }

                mCurrentContext = newContext;
                mFrameId = newFrameId;
                //UpdateSavedContextInfo;
                fullUpdateNeeded = true;
            }

            if (mCurrentContext == MainTestsContext)
            {
                mExePhaseStepGroup = mCurrentContext.StepGroup;
                if (mExePhaseStepGroup == StepGroups.StepGroup_Main)
                {
                    mPointedAtStepIndex = mCurrentContext.StepIndex;
                    if (mPointedAtStepIndex == -1)
                    {
                        if (mCurrentContext.NextStepIndex > -1)
                        {
                            mPointedAtStepIndex = mCurrentContext.NextStepIndex;
                            mPointedStepName = mCurrentContext.Sequence.GetStep(mPointedAtStepIndex, mCurrentContext.StepGroup).Name;
                        }
                        mPointedStepName = "???";
                    }
                    else
                    {
                        mPointedStepName = mCurrentContext.Sequence.GetStep(mPointedAtStepIndex, mCurrentContext.StepGroup).Name;
                    }

                    if (mCurrentContext.PreviousStepIndex > -1)
                    {
                        mCurrentStep = mCurrentContext.Sequence.GetStep(mCurrentContext.PreviousStepIndex, StepGroups.StepGroup_Main);
                        mPreviousStepResultStatus = mCurrentStep.ResultStatus;
                    }
                    else
                    {
                        mPreviousStepResultStatus = "";
                    }
                }


            }

            mCurrentStep = null;
        }



    }
}
