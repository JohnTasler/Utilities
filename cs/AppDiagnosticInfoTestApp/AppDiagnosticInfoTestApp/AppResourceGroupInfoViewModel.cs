using System;
using System.ComponentModel;
using Tasler.ComponentModel;
using Windows.Foundation;
using Windows.System;
using Windows.System.Diagnostics;
using Windows.UI.Xaml.Data;

namespace AppDiagnosticInfoTestApp
{
    public class AppResourceGroupInfoViewModel : INotifyPropertyChanged
    {
        public AppResourceGroupInfoViewModel(AppResourceGroupInfo model)
        {
            this.Model = model;

            this.ProcessDiagnosticInfos.CurrentChanged += (s, e) =>
            {
                this.PropertyChanged.RaisePropertyChanged(this,
                    nameof(this.SelectedProcessDiagnosticInfo),
                    nameof(this.HasSelectedProcessDiagnosticInfo));
            };

            // Refresh values every 2 seconds
            _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.IsRepeating = true;
            _timer.Tick += (s, a) =>
            {
                this.PropertyChanged.RaisePropertyChanged(this,
                    nameof(this.Model),
                    nameof(this.MemoryReport),
                    nameof(this.ProcessDiagnosticInfos),
                    nameof(this.StateReport)
                    );
            };
            _timer.Start();
        }

        public AppResourceGroupInfo Model { get; }

        public AppResourceGroupMemoryReportViewModel MemoryReport
        {
            get
            {
                try
                {
                    return new AppResourceGroupMemoryReportViewModel(this.Model.GetMemoryReport());
                }
                catch
                {
                    return new AppResourceGroupMemoryReportViewModel(null);
                }
            }
        }

        public ICollectionView ProcessDiagnosticInfos
        {
            get
            {
                _processDiagnosticInfos.Source = this.Model.GetProcessDiagnosticInfos();
                return _processDiagnosticInfos.View;
            }
        }
        private readonly CollectionViewSource _processDiagnosticInfos = new CollectionViewSource();

        public int ProcessDiagnosticInfosCount => _processDiagnosticInfos.View.Count;

        public ProcessDiagnosticInfo SelectedProcessDiagnosticInfo => (ProcessDiagnosticInfo)_processDiagnosticInfos.View.CurrentItem;

        public bool HasSelectedProcessDiagnosticInfo => _processDiagnosticInfos.View.CurrentPosition >= 0;

        public AppResourceGroupStateReport StateReport
        {
            get
            {
                return this.Model.GetStateReport();
            }
        }

        public string OperationResult
        {
            get { return _operationResult; }
            private set { this.PropertyChanged.SetProperty(this, value, ref _operationResult, nameof(OperationResult), nameof(HasOperationResult)); }
        }
        private string _operationResult;

        public bool HasOperationResult => _operationResult != null;

        public void ExecuteSuspend()
        {
            this.DoOperation(a => a.StartSuspendAsync());
        }

        public void ExecuteResume()
        {
            this.DoOperation(a => a.StartResumeAsync());
        }

        public void ExecuteTerminate()
        {
            this.DoOperation(a => a.StartTerminateAsync());
        }

        public void ClearOperationResult() => this.OperationResult = null;

        private async void DoOperation(Func<AppResourceGroupInfo, IAsyncOperation<AppExecutionStateChangeResult>> asyncOperation)
        {
            var result = await asyncOperation(this.Model);
            this.OperationResult = result.ExtendedError != null ? result.ExtendedError.Message : "S_OK";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private DispatcherQueueTimer _timer;
    }
}
