using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;

namespace DysphagiaAssessment
{
    public sealed partial class MainPage : Page
    {
        private SerialDevice serialPort;
        private DataReader dataReader;
        private Random random = new Random();
        double[] sensorData = { 0, 0, 0, 0 };
        private StreamWriter csvWriter;

        enum ForceSensor { SENSOR1, SENSOR2, SENSOR3, SENSOR4, SENSOR5, SENSOR6, SENSOR7};

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ConnectToSerialPort();
            // await InitializeCsvFile();
        }

        private async void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            await CloseCsvFile();
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
        private async Task ConnectToSerialPort()
        {
            string portId = "COM11"; // Use the correct COM port assigned to your ESP32

            var selector = SerialDevice.GetDeviceSelector(portId);
            var devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Count > 0)
            {
                var deviceInfo = devices[0];
                serialPort = await SerialDevice.FromIdAsync(deviceInfo.Id);

                if (serialPort != null)
                {
                    // Configure serial port settings if needed
                    serialPort.BaudRate = 9600;
                    serialPort.DataBits = 8;
                    serialPort.Parity = SerialParity.None;
                    serialPort.StopBits = SerialStopBitCount.One;

                    dataReader = new DataReader(serialPort.InputStream);
                    ReadSerialData();
                }
            }
            else
            {
                Console.WriteLine($"No serial devices found on {portId}.");
            }
        }

        private async Task InitializeCsvFile()
        {
            try
            {
                // Create or open the CSV file in the LocalFolder
                string fileName = $"sensor_data_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile csvFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                csvWriter = new StreamWriter(await csvFile.OpenStreamForWriteAsync());

                // Write the header
                await csvWriter.WriteLineAsync("Midline Anterior, Midline Posterior, Right Anterior, Left Anterior, Left Posterior, Midline Center, Right Posterior");

                // Flush the writer to ensure data is written to the file
                await csvWriter.FlushAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions, log, or display an error message
                Console.WriteLine($"Error initializing CSV file: {ex.Message}");
            }
        }

        private async Task CloseCsvFile()
        {
            if (csvWriter != null)
            {
                await csvWriter.FlushAsync();
                csvWriter.Dispose();
                csvWriter = null;
            }
        }

        private async void ReadSerialData()
        {
            try
            {
                // Ensure csvWriter is initialized
                if (csvWriter == null)
                {
                    await InitializeCsvFile();
                }
                
                while (true)
                {
                    await dataReader.LoadAsync(sizeof(float) * 7);
                    byte[] data = new byte[sizeof(float) * 7];
                    dataReader.ReadBytes(data);

                    var rawData = $"Received raw data: {BitConverter.ToString(data)}";

                    var forceValue1 = BitConverter.ToSingle(data, 0).ToString("0.00");
                    var forceValue2 = BitConverter.ToSingle(data, sizeof(float)).ToString("0.00");
                    var forceValue3 = BitConverter.ToSingle(data, sizeof(float) * 2).ToString("0.00");
                    var forceValue4 = BitConverter.ToSingle(data, sizeof(float) * 3).ToString("0.00");
                    var forceValue5 = BitConverter.ToSingle(data, sizeof(float) * 4).ToString("0.00");
                    var forceValue6 = BitConverter.ToSingle(data, sizeof(float) * 5).ToString("0.00");
                    var forceValue7 = BitConverter.ToSingle(data, sizeof(float) * 6).ToString("0.00");

                    // Write force values to the CSV file
                    await csvWriter.WriteLineAsync($"{forceValue1},{forceValue2},{forceValue3},{forceValue4},{forceValue5},{forceValue6},{forceValue7}");

                    // Flush the writer to ensure data is written to the file
                    await csvWriter.FlushAsync();

                    // Process force values as needed, update UI, etc.
                    Console.WriteLine($"Received force values: {forceValue1}, {forceValue2}, {forceValue3}, {forceValue4},{forceValue5},{forceValue6},{forceValue7}");

                    retainerUserControl.ForceSensor1Value = forceValue1;
                    retainerUserControl.ForceSensor2Value = forceValue2;
                    retainerUserControl.ForceSensor3Value = forceValue3;
                    retainerUserControl.ForceSensor4Value = forceValue4;
                    retainerUserControl.ForceSensor5Value = forceValue5;
                    retainerUserControl.ForceSensor6Value = forceValue6;
                    retainerUserControl.ForceSensor7Value = forceValue7;

                    //retainerUserControl.UpdateVisualization();

                    await Task.Delay(100); // Adjust delay as needed
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Console.WriteLine($"Error reading serial data: {ex.Message}");
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
