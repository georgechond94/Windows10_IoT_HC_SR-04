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

using Windows.Devices.Gpio;
using System.Threading.Tasks;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTUltrasonicApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int ECHO_PIN = 23;
        private const int TRIGGER_PIN = 18;
        private GpioPin pinEcho;
        private GpioPin pinTrigger;
        private DispatcherTimer timer;
        private Stopwatch sw;

        public MainPage()
        {
            this.InitializeComponent();


            InitGPIO();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(400);
            timer.Tick += Timer_Tick;
            if (pinEcho != null && pinTrigger != null)
            {
                timer.Start();
            }


        }

        private async void Timer_Tick(object sender, object e)
        {
            pinTrigger.Write(GpioPinValue.High);
            await Task.Delay(10);
            pinTrigger.Write(GpioPinValue.Low);
            while (pinEcho.Read() == GpioPinValue.Low)
            {
                sw.Restart();

            }

            while (pinEcho.Read() == GpioPinValue.High)
            {
            }
            sw.Stop();

            var elapsed = sw.Elapsed.TotalSeconds;
            var distance = elapsed * 34000;

            distance /= 2;
            distancetb.Text = "Distance: " + distance + " cm";

        }
        private async void InitGPIO()
        {
            var gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                pinEcho = null;
                pinTrigger = null;
                gpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            pinEcho = gpio.OpenPin(ECHO_PIN);
            pinTrigger = gpio.OpenPin(TRIGGER_PIN);


            pinTrigger.SetDriveMode(GpioPinDriveMode.Output);
            pinEcho.SetDriveMode(GpioPinDriveMode.Input);

            gpioStatus.Text = "GPIO controller and pins initialized successfully.";

            pinTrigger.Write(GpioPinValue.Low);

            await Task.Delay(100);
        }
    }

}
