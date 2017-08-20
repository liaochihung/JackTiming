using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using JackTiming.MessageInfrastructure;

namespace JackTiming.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private IDialogService _dialogService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDialogService dialogService)
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

            ApplicationTitle = nameof(JackTiming);

            FileName = InitFileName;
            TimingData = InitTimingData;
            MarkerData = InitMarkerData;

            EditStatusString = EditStatus.Unchanged.ToString();

            _dialogService = dialogService;

            TimingCharacters =
                " Characters " + Environment.NewLine + Environment.NewLine +
                " - = Tristate" + Environment.NewLine +
                " ~ = Hi edge" + Environment.NewLine +
                " _ = Lo edge" + Environment.NewLine +
                " / = Hi edge slow" + Environment.NewLine +
                " \\ = Lo edge slow" + Environment.NewLine +
                " [ = Data begin" + Environment.NewLine +
                " ] = Data end" + Environment.NewLine +
                " * = Data cross over" + Environment.NewLine +
                " < = Data begin slow" + Environment.NewLine +
                " > = Data end slow" + Environment.NewLine +
                " : = Break" + Environment.NewLine +
                " | = Marker";

            Messenger.Default.Register<MessageToken>(this, (item) =>
            {
                if (item.TokenType == MessageTokenType.KeyChanged)
                    EditStatus = EditStatus.Modified;
            });

            // doesn't work!
            //TestDialogServiceCommand = new RelayCommand(() =>
            //{
            //    dialogService.ShowInfo("test", "title");
            //});

            NewFileCommand = new RelayCommand(() =>
            {
                if (EditStatus == EditStatus.Unchanged)
                {
                    FileName = InitFileName;
                    TimingData = InitTimingData;
                }
                else
                {
                    _dialogService.ShowWarning("File has been changed, you need to save first!", "File changed");

                    return;
                }
                UpdateTimingDiagram();
            });

            OpenFileCommand = new RelayCommand(() =>
            {
                _fullPathFileName = _dialogService.OpenFileDialog(".\\");
                if (_fullPathFileName == string.Empty)
                    return;

                TimingData = File.ReadAllText(_fullPathFileName);
                FilePath = System.IO.Path.GetDirectoryName(_fullPathFileName);
                FileName = System.IO.Path.GetFileName(_fullPathFileName);

                UpdateTimingDiagram();

                EditStatus = EditStatus.Unchanged;
            });

            SaveCommand = new RelayCommand(() =>
            {
                if (FilePath == null || !File.Exists(FilePath))
                {
                    SaveAs();
                    return;
                }

                File.WriteAllText(FilePath, TimingData);
                EditStatus = EditStatus.Unchanged;
            });

            SaveAsCommand = new RelayCommand(SaveAs);

            CopyImageCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new MessageToken()
                {
                    TokenType = MessageTokenType.CopyToClipboard,
                    Message = null
                });
            });

            ExportImageCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new MessageToken()
                {
                    TokenType = MessageTokenType.SaveBitmap,
                    Message = System.IO.Path.ChangeExtension(_fullPathFileName, ".png")
                });
            });

            CloseWindowCommand = new RelayCommand<Window>((window) =>
            {
                if (EditStatus == EditStatus.Modified)
                {
                    if (_dialogService.ShowQuestion("File has been modified, are you sure to exit?",
                            "File modified.") == false)
                        return;
                }
                window.Close();
            });

            UpdateTimingDiagram();
        }

        private void SaveAs()
        {
            var fn = _dialogService.SaveFileDialog();
            if (fn != string.Empty)
            {
                File.WriteAllText(fn, TimingData);

                _fullPathFileName = fn;

                FilePath = System.IO.Path.GetDirectoryName(_fullPathFileName);
                FileName = System.IO.Path.GetFileName(_fullPathFileName);

                EditStatus = EditStatus.Unchanged;
            }
            else
            {
                return;
            }
        }

        private void UpdateTimingDiagram()
        {
            Messenger.Default.Send(new MessageToken()
            {
                TokenType = MessageTokenType.UpdateTimingDiagram,
                Message = null
            });
        }

        private const string InitTimingData = "Marker=  | |  \r\nTest=__~~__~~__";
        private const string InitFileName = "untitled.atd";
        private const string InitMarkerData = "  |    |";

        private EditStatus _editStatus = EditStatus.Unchanged;

        public EditStatus EditStatus
        {
            get { return _editStatus; }
            private set
            {
                _editStatus = value;
                EditStatusString = _editStatus.ToString();
            }
        }

        public string ApplicationTitle { get; set; }
        public string FileName { get; private set; }
        public string FilePath { get; private set; }

        public string TimingData { get; set; }
        public string TimingCharacters { get; set; }
        public string MarkerData { get; set; }

        // lazy for create converter :)
        public string EditStatusString { get; set; }

        public RelayCommand TestDialogServiceCommand { get; private set; }

        public RelayCommand NewFileCommand { get; private set; }
        public RelayCommand OpenFileCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand SaveAsCommand { get; private set; }

        public RelayCommand ExportImageCommand { get; private set; }
        public RelayCommand CopyImageCommand { get; private set; }

        public RelayCommand AddCommand { get; private set; }

        public RelayCommand<Window> CloseWindowCommand { get; private set; }

        private bool _isFileModified;
        private string _fullPathFileName;
    }

    public enum EditStatus
    {
        Modified,
        Unchanged
    }
}