using System;
using System.Collections.Generic;
using System.Threading;

using Explorip.HookFileOperations.Models;

namespace ExploripCopy.ViewModels
{
    internal class MainViewModels : ViewModelBase
    {
        private static MainViewModels _instance;
        private readonly Thread _mainThread;

        public static MainViewModels Instance
        {
            get { return _instance ??= new MainViewModels(); }
        }

        internal MainViewModels() : base()
        {
            _listWait = new();
            _mainThread = new Thread(new ThreadStart(ThreadFileOpWaiting));
            _mainThread.Start();
        }

        private bool _isMaximized;
        public bool WindowMaximized
        {
            get { return _isMaximized; }
            set
            {
                if (value != _isMaximized)
                {
                    _isMaximized = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _currentFile;
        public string CurrentFile
        {
            get { return _currentFile; }
            set
            {
                _currentFile = value;
                OnPropertyChanged();
            }
        }

        private string _globalReport;
        public string GlobalReport
        {
            get { return _globalReport; }
            set
            {
                _globalReport = value;
                OnPropertyChanged();
            }
        }

        private double _reportProgress;
        public double CurrentProgress
        {
            get { return _reportProgress; }
            set
            {
                _reportProgress = value;
                OnPropertyChanged();
            }
        }

        private double _globalProgress;
        public double GlobalProgress
        {
            get { return _globalProgress; }
            set
            {
                _globalProgress = value;
                OnPropertyChanged();
            }
        }

        private List<OneFileOperation> _listWait;
        public List<OneFileOperation> ListWaiting
        {
            get { return _listWait; }
            set
            {
                _listWait = value;
                OnPropertyChanged();
            }
        }

        private void ThreadFileOpWaiting()
        {
            while (true)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(_currentFile) && _listWait.Count > 0)
                    {
                    }
                    Thread.Sleep(10);
                }
                catch (ThreadAbortException) { break; }
                catch (Exception) { /* Ignore errors */ }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _mainThread.Abort();
        }
    }
}
