using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace DysphagiaAssessment
{
    public sealed partial class MainPage : Page
    {
        private BluetoothLEDevice bleDevice;
        private GattCharacteristic forceSensor1Characteristic;
        private GattCharacteristic forceSensor2Characteristic;
        private GattCharacteristic forceSensor3Characteristic;
        private GattCharacteristic forceSensor4Characteristic;
        private Random random = new Random();
        double[] sensorData = { 0, 0, 0, 0 };

        enum ForceSensor { SENSOR1, SENSOR2, SENSOR3, SENSOR4};

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
            //EnumerateBLEDevices();
            ConnectToBLEDevice();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void RetainerUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Simulate sensor data updates
            /*while (true)
            {
                double[] sensorData = GenerateRandomSensorData();
                retainerUserControl.UpdateVisualization(sensorData);
                await Task.Delay(1000);
            }*/
        }


        private double[] GenerateRandomSensorData()
        {
            double[] sensorData = new double[4];
            for (int i = 0; i < 4; i++)
            {
                sensorData[i] = random.Next(0, 100);
            }
            return sensorData;
        }

        private async void EnumerateBLEDevices()
        {
            var selector = BluetoothLEDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Count > 0)
            {
                foreach (var device in devices)
                {
                    Console.WriteLine($"Found BLE device: {device.Name}, Id: {device.Id}");
                }
            }
            else
            {
                Console.WriteLine("No BLE devices found.");
            }
        }


        private async void ConnectToBLEDevice()
        {
            //var selector = GattDeviceService.GetDeviceSelectorFromUuid(new Guid("D9AC5674-89B2-F9A1-D684-B701BE8BEBAD"));
            var selector = GattDeviceService.GetDeviceSelectorFromUuid(new Guid("A0:B7:65:CC:3F:88"));
            //var selector = BluetoothLEDevice.GetDeviceSelectorFromDeviceName("ForceSensors");
            var devices = await DeviceInformation.FindAllAsync(selector);

            //f(device != null)
            //{
              //  bleDevice = await BluetoothLEDevice.FromIdAsync(device.Id);

            if (devices.Count > 0)
            {
                bleDevice = await BluetoothLEDevice.FromIdAsync(devices[0].Id);
                if (bleDevice != null)
                {
                    var service = bleDevice.GetGattService(new Guid("0000"));

                    if (service != null)
                    {
                        forceSensor1Characteristic = service.GetCharacteristics(new Guid("0001"))[0];
                        forceSensor2Characteristic = service.GetCharacteristics(new Guid("0002"))[0];
                        forceSensor3Characteristic = service.GetCharacteristics(new Guid("0003"))[0];
                        forceSensor4Characteristic = service.GetCharacteristics(new Guid("0004"))[0];

                        await forceSensor1Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);

                        await forceSensor2Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);

                        await forceSensor3Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);

                        await forceSensor4Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);

                        forceSensor1Characteristic.ValueChanged += ForceSensor1Characteristic_ValueChanged;
                        forceSensor2Characteristic.ValueChanged += ForceSensor2Characteristic_ValueChanged;
                        forceSensor3Characteristic.ValueChanged += ForceSensor3Characteristic_ValueChanged;
                        forceSensor4Characteristic.ValueChanged += ForceSensor4Characteristic_ValueChanged;
                    }
                }
            }
        }

        private void ForceSensor1Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            ReadForceValue(sender, ForceSensor.SENSOR1);
        }

        private void ForceSensor2Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            ReadForceValue(sender, ForceSensor.SENSOR2);
        }

        private void ForceSensor3Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            ReadForceValue(sender, ForceSensor.SENSOR3);
        }

        private void ForceSensor4Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            ReadForceValue(sender, ForceSensor.SENSOR4);
        }

        private async void ReadForceValue(GattCharacteristic characteristic, ForceSensor forceSensor)
        {
            var result = await characteristic.ReadValueAsync();

            using (var reader = DataReader.FromBuffer(result.Value))
            {
                byte[] data = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(data);

                if (data.Length == 4)
                {
                    var forceValue = BitConverter.ToSingle(data, 0).ToString("0.00");

                    switch (forceSensor)
                    {
                        case ForceSensor.SENSOR1:
                            retainerUserControl.ForceSensor1Value = forceValue;
                            break;
                        case ForceSensor.SENSOR2:
                            retainerUserControl.ForceSensor2Value = forceValue;
                            break;
                        case ForceSensor.SENSOR3:
                            retainerUserControl.ForceSensor3Value = forceValue;
                            break;
                        case ForceSensor.SENSOR4:
                            retainerUserControl.ForceSensor4Value = forceValue;
                            break;
                    }
                }
            }
        }

        /*private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var selector = GattDeviceService.GetDeviceSelectorFromUuid(new Guid("0000"));
            var devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Count > 0)
            {
                bleDevice = await BluetoothLEDevice.FromIdAsync(devices[0].Id);
                if (bleDevice != null)
                {
                    var service = bleDevice.GetGattService(new Guid("0000"));

                    if (service != null)
                    {
                        forceSensorCharacteristic = service.GetCharacteristics(new Guid("0001"))[0];

                        await forceSensorCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);

                        forceSensorCharacteristic.ValueChanged += ForceSensorCharacteristic_ValueChanged;
                    }
                }
            }
        }

        private void ForceSensorCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);

            if (data.Length == 16)
            {
                double[] values = new double[4];
                for (int i = 0; i < 4; i++)
                {
                    values[i] = BitConverter.ToDouble(data, i * 8);
                }

                // Update your retainer visualization with the received sensor data
                UpdateVisualization(values);
            }
        }

        private async void UpdateVisualization(double[] values)
        {
            // Implement your visualization logic here, similar to the previous example
            // Use shapes or other elements to display the retainer and measurements
            // For example, you can create Ellipse elements for FSRs and update their colors or sizes based on the values.
        }*/
    }
}
