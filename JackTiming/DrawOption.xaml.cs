using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using JackTiming.MessageInfrastructure;

namespace JackTiming
{
    /// <summary>
    /// DrawOption.xaml 的互動邏輯
    /// </summary>
    public partial class DrawOption : Window
    {
        public DrawOption()
        {
            InitializeComponent();
        }

        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Messenger.Default.Send(new MessageToken()
            {
                TokenType = MessageTokenType.UpdateTimingDiagram,
                Message = null
            });
        }
    }

    public class DrawParamDto
    {
        public int UnitX
        {
            get { return DrawParam.UnitX; }
            set { DrawParam.UnitX = value; }
        }

        public int UnitY
        {
            get { return DrawParam.UnitY; }
            set { DrawParam.UnitY = value; }
        }

        public int VerticalSpace
        {
            get { return DrawParam.VerticalSpace; }
            set { DrawParam.VerticalSpace = value; }
        }

        public int MarkHeight
        {
            get { return DrawParam.MarkHeight; }
            set { DrawParam.MarkHeight = value; }
        }

        private static DrawParamDto _instance;

        public static DrawParamDto Instance => _instance?? (_instance=new DrawParamDto());
    }
}
