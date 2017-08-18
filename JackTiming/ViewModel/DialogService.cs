using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace JackTiming.ViewModel
{
    public class DialogService : IDialogService
    {
        public string OpenFileDialog(string defaultPath)
        {
            var dlg = new OpenFileDialog()
            {
                Title = "Open Timing Diagram",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Timing files (*.atd)|*.atd"
            };

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        public string SaveFileDialog()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Timing files (*.atd)|*.atd"
            };

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        public void ShowError(Exception Error, string Title)
        {
            MessageBox.Show(Error.ToString(), Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowError(string Message, string Title)
        {
            MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfo(string Message, string Title)
        {
            MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowMessage(string Message, string Title)
        {
            MessageBox.Show(Message, Title, MessageBoxButton.OK);
        }

        public bool ShowQuestion(string Message, string Title)
        {
            return MessageBox.Show(Message, Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public void ShowWarning(string Message, string Title)
        {
            MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
