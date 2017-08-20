using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackTiming.ViewModel
{
    public interface IDialogService
    {
        string OpenFileDialog(string defaultPath);

        string SaveFileDialog();

        void ShowError(Exception Error, string Title);
        void ShowError(string Message, string Title);
        void ShowInfo(string Message, string Title);
        void ShowMessage(string Message, string Title);
        bool ShowQuestion(string Message, string Title);
        void ShowWarning(string Message, string Title);
    }
}
