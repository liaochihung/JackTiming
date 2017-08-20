using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using JackTiming.MessageInfrastructure;
using JackTiming.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace JackTiming
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitDrawParam();

            MarkerDrawer.Canvas = myCanvas;
            TimingCharacterDrawer.Canvas = myCanvas;

            _markerDrawer = new MarkerDrawer();

            _timingDatas = null;

            Messenger.Default.Register<MessageToken>(this, (item) =>
            {
                switch (item.TokenType)
                {
                    case MessageTokenType.UpdateTimingDiagram:
                        DrawTiming(myCanvas);
                        break;

                    case MessageTokenType.CopyToClipboard:
                        GraphicHelper.CopyUiElementToClipboard(myCanvas);
                        break;

                    case MessageTokenType.SaveBitmap:
                        var dlg = new SaveFileDialog()
                        {
                            Filter = "Png files (*.png)|*.png"
                        };

                        if (dlg.ShowDialog() == false)
                            return;

                        GraphicHelper.CreateSaveBitmap(myCanvas, dlg.FileName);
                        break;
                }
            });
        }

        private void InitDrawParam()
        {
            DrawParam.StartX = 50;
            DrawParam.StartY = 15;
            DrawParam.UnitX = 10;
            DrawParam.UnitY = 30;
            DrawParam.Margin = 10;
            DrawParam.VerticalSpace = 12;
            DrawParam.MarkHeight = 6;
        }

        private List<TimingMap> _timingDatas;

        private void TxtData_OnKeyUp(object sender, KeyEventArgs e)
        {
            Messenger.Default.Send(new MessageToken()
            {
                TokenType = MessageTokenType.KeyChanged,
                Message = null
            });

            DrawTiming(myCanvas);
        }

        private MarkerDrawer _markerDrawer;

        private void DrawTiming(Canvas canvas)
        {
            canvas.Children.Clear();

            _timingDatas = TimingMapParser.Parse(txtData.Text);

            InitDrawParam();

            // determine the maximum width from all symbols
            var max = 0.0;
            foreach (var timingData in _timingDatas)
            {
                var x =GraphicHelper.MeasureTextWidth(timingData.Symbol, 14);
                if (max < x)
                    max = x;
            }

            // ensure we draw behind symbol
            DrawParam.StartX = (int)max;

            // determine the width of entire diagram 
            max = 0;
            foreach (var timingData in _timingDatas)
            {
                if (max < (int)timingData.Timing.Length)
                    max = (int)timingData.Timing.Length;
            }

            // re-calculate the size of canvas
            canvas.Width = DrawParam.StartX + (max * DrawParam.UnitX) + (DrawParam.Margin * 2);
            canvas.Height = _timingDatas.Count * (DrawParam.UnitY + DrawParam.VerticalSpace) + DrawParam.StartY;

            _markerDrawer.Draw((int)max);

            var lineIndex = 0;

            TimingCharacterDrawer marker = null;

            // draw content
            foreach (var timingMap in _timingDatas)
            {
                var drawer = new TimingCharacterDrawer(lineIndex, timingMap);

                if (drawer.IsMarker() == true)
                {
                    marker = drawer;

                    lineIndex++;
                    continue;
                }

                drawer.Draw();

                lineIndex++;
                DrawParam.StartY += DrawParam.VerticalSpace;
            }

            if (marker != null)
                marker.DrawMarker(lineIndex);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawTiming(myCanvas);
        }
    }
}
