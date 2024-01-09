using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DysphagiaAssessment
{
    public sealed partial class RetainerUserControl : UserControl
    {
        private readonly int MIN_OK_PRESSURE = 43;
        private readonly int MAX_OK_PRESSURE = 78;

        public RetainerUserControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public string ForceSensor1Value
        {
            get { return (string)GetValue(ForceSensor1ValueProperty); }
            set 
            { 
                SetValue(ForceSensor1ValueProperty, value);
                fsr1.Fill = GetBrushColor(value);
            }
        }

        public static readonly DependencyProperty ForceSensor1ValueProperty =
            DependencyProperty.Register("ForceSensor1Value", typeof(string), typeof(RetainerUserControl), new PropertyMetadata(0.0));

        public string ForceSensor2Value
        {
            get { return (string)GetValue(ForceSensor2ValueProperty); }
            set 
            { 
                SetValue(ForceSensor2ValueProperty, value);
                fsr2.Fill = GetBrushColor(value);
            }
        }

        public static readonly DependencyProperty ForceSensor2ValueProperty =
            DependencyProperty.Register("ForceSensor2Value", typeof(string), typeof(RetainerUserControl), new PropertyMetadata(0.0));

        public string ForceSensor3Value
        {
            get { return (string)GetValue(ForceSensor3ValueProperty); }
            set 
            { 
                SetValue(ForceSensor3ValueProperty, value);
                fsr3.Fill = GetBrushColor(value);
            }
        }

        public static readonly DependencyProperty ForceSensor3ValueProperty =
            DependencyProperty.Register("ForceSensor3Value", typeof(string), typeof(RetainerUserControl), new PropertyMetadata(0.0));

        public string ForceSensor4Value
        {
            get { return (string)GetValue(ForceSensor4ValueProperty); }
            set 
            { 
                SetValue(ForceSensor4ValueProperty, value);
                fsr4.Fill = GetBrushColor(value);
            }
        }

        public static readonly DependencyProperty ForceSensor4ValueProperty =
            DependencyProperty.Register("ForceSensor4Value", typeof(string), typeof(RetainerUserControl), new PropertyMetadata(0.0));

        public string ForceSensor5Value
        {
            get { return (string)GetValue(ForceSensor5ValueProperty); }
            set
            {
                SetValue(ForceSensor5ValueProperty, value);
                fsr5.Fill = GetBrushColor(value);
            }
        }

        public static readonly DependencyProperty ForceSensor5ValueProperty =
            DependencyProperty.Register("ForceSensor5Value", typeof(string), typeof(RetainerUserControl), new PropertyMetadata(0.0));

        public string ForceSensor6Value
        {
            get { return (string)GetValue(ForceSensor6ValueProperty); }
            set
            {
                SetValue(ForceSensor6ValueProperty, value);
                fsr6.Fill = GetBrushColor(value);
            }
        }

        public static readonly DependencyProperty ForceSensor6ValueProperty =
            DependencyProperty.Register("ForceSensor6Value", typeof(string), typeof(RetainerUserControl), new PropertyMetadata(0.0));

        public string ForceSensor7Value
        {
            get { return (string)GetValue(ForceSensor7ValueProperty); }
            set
            {
                SetValue(ForceSensor7ValueProperty, value);
                fsr7.Fill = GetBrushColor(value);
            }
        }

        public static readonly DependencyProperty ForceSensor7ValueProperty =
            DependencyProperty.Register("ForceSensor7Value", typeof(string), typeof(RetainerUserControl), new PropertyMetadata(0.0));


        public void UpdateVisualization()
        {
            // Update the visual elements based on sensor data
            fsr1.Fill = int.Parse(ForceSensor1Value) > 78 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            fsr2.Fill = int.Parse(ForceSensor2Value) > 78 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green );

            fsr3.Fill = int.Parse(ForceSensor3Value) > 78 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            fsr4.Fill = int.Parse(ForceSensor4Value) > 78 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            fsr5.Fill = int.Parse(ForceSensor5Value) > 78 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            fsr6.Fill = int.Parse(ForceSensor6Value) > 78 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            fsr7.Fill = int.Parse(ForceSensor7Value) > 78 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
        }

        private SolidColorBrush GetBrushColor(string value)
        {
            var pressure = double.Parse(value);
            if(pressure == 0.00)
            {
                return new SolidColorBrush(Colors.Gray);
            }
            
            return (pressure < MIN_OK_PRESSURE || pressure > MAX_OK_PRESSURE) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
        }
    }
}
