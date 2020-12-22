namespace VehicleTelemetrySimulator
{
    using System;
    using System.Runtime.Loader;
    using System.Text;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Shared;
    using Newtonsoft.Json;
    using Ganss.Excel;
    using Troschuetz.Random;
    using System.Linq;
    using Microsoft.ML;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Newtonsoft.Json.Linq;

    class Program
    {
        //TODO: 1 - set device connection string for the device client 
        //static string _deviceConnectionString = "<device connection string goes here>";

        //TODO: 2 - set the connection string for local blob storage (keep this text, just uncomment)
        //static string _storageConnectionString = "DefaultEndpointsProtocol=http;BlobEndpoint=http://azureblobstorageoniotedge:11002/edgestorage;AccountName=edgestorage;AccountKey=pM8cWFj0L8h+VKRfE8Fy3tVVtdfOR4bCIzX8N/sDiK1X0znhu8iatFwVfjzwjedDKe5ln+2cI7wpy+2eO1vvQQ==";
      
        static List<string> _boroughList = new List<string>() { "Northwind", "Contoso", "Tailwind" };
        static ExcelMapper _routeReader = new ExcelMapper("BusRouteData/routeInterpolated.xlsx");
        static int _secondsToTwinReportedPropertiesUpdate = 120;
        static List<RouteData> _routeData = null;
        static TRandom _random = new TRandom();
        static Timer _timer;

        static int _counter;

        static double _highOilProbabilityPower = 0.3;
        static double _lowOilProbabilityPower = 1.2;
        static double _highTirePressureProbabilityPower = 0.5;
        static double _lowTirePressureProbabilityPower = 1.7;
        static double _highOutsideTempProbabilityPower = 0.3;
        static double _lowOutsideTempProbabilityPower = 1.2;
        static double _highEngineTempProbabilityPower = 0.3;
        static double _lowEngineTempProbabilityPower = 1.2;
        static string _vin { get; set; }
        static string _borough { get; set; }
        static float _latitude { get; set; }
        static float _longitude { get; set; }
        static JObject _telemetryDefn {get;set;}

        static ModuleClient _vehicleTelemetryModuleClient;
        static DeviceClient _deviceClient;

        static CloudStorageAccount _storageAccount;
        static CloudBlobClient _blobClient;
        static CloudBlobContainer _blobContainer;

        static void Main(string[] args)
        {
            Init().Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();

            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();

        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init()
        {

            // read the route data
            _routeData = _routeReader.Fetch<RouteData>().ToList();

            // TODO: 11 - update reported properties at a specified time interval
            // _timer = new Timer(UpdateReportedProperties, null, TimeSpan.FromSeconds(_secondsToTwinReportedPropertiesUpdate), TimeSpan.FromSeconds(_secondsToTwinReportedPropertiesUpdate));

            // the module client is in charge of sending messages in the context of this module (VehicleTelemetry)
            // TODO: 3 - Create module client from container environment variable
            //_vehicleTelemetryModuleClient = await ModuleClient.CreateFromEnvironmentAsync(TransportType.Mqtt);            
            await _vehicleTelemetryModuleClient.OpenAsync();

            // the device client is responsible for the device twin reported properties updates
            // TODO: 4 - Create device client to obtain desired properties from Twin and update reported properties
            //_deviceClient = DeviceClient.CreateFromConnectionString(_deviceConnectionString);
            //await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(onDesiredPropertiesUpdateAsync, null);

            // Register callback to be called when a message is received by this module
            await _vehicleTelemetryModuleClient.SetInputMessageHandlerAsync("input1", PipeMessage, _vehicleTelemetryModuleClient);

            //initialize device with values obtained from the desired properties in the device twin     
            // TODO: 5 - initialize device instance with values obtained from the device twin desired properties       
            //var twin = await _deviceClient.GetTwinAsync();
            // var desired = twin.Properties.Desired;
            // await UpdateDeviceInstanceFromDesiredProperties(desired);

            // TODO: 6 - initialize iot edge storage 
            // _storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            // _blobClient = _storageAccount.CreateCloudBlobClient();
            // _blobContainer = _blobClient.GetContainerReference("telemetry");
            // if(!_blobContainer.Exists()){
            //     _blobContainer.CreateIfNotExists();
            // }
            
            //start generating telemetry data
            await GenerateTelemetry();
        }

        /// <summary>
        /// This method is called whenever the device twin desired properties change in the cloud 
        /// It updates the current device instance with the desired properties that were changed
        /// </summary>
        private static async Task onDesiredPropertiesUpdateAsync(TwinCollection desiredProperties, object userContext)
        {
            await UpdateDeviceInstanceFromDesiredProperties(desiredProperties);
        }


        /// <summary>
        /// This method simulated bus sensor telemetry 
        /// </summary>        
        private static async Task GenerateTelemetry()
        {
            string outputName = "output1";
            int routeIdx = 0;

            // TODO: 13 - Initialize machine learning prediction model infrastructure
            // var mlContext = new MLContext();
            // ITransformer mlModel = mlContext.Model.Load("BusMlModel/MLModel.zip", out var modelInputSchema);
            // var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            while (true)
            {
                try
                {
                    var currRouteData = _routeData[routeIdx];

                    _borough = GetBorough();
                    _latitude = currRouteData.Latitude;
                    _longitude = currRouteData.Longitude;

                    // TODO 14: Create input for the machine learning prediction engine by setting the 
                    //         device current latitude, longitude, and speed limit
                    // var mlInput = new ModelInput()
                    // {
                    //     Latitude = currRouteData.Latitude,
                    //     Longitude = currRouteData.Longitude,
                    //     BusSpeed = currRouteData.BusSpeed
                    // };

                    // TODO 15: Use this input model to have the prediction engine determine if the
                    //          current speed for the device is safe for the latitude and longitude location
                    // var mlOutput = predEngine.Predict(mlInput);

                    if (routeIdx == _routeData.Count - 1)
                    {
                        routeIdx = 0; //restart route
                    }
                    else
                    {
                        routeIdx++;
                    }

                    var info = new BusEvent()
                    {
                        vin = _vin,
                        outsideTemperature = GetOutsideTemp(_borough),
                        engineTemperature = GetEngineTemp(_borough),
                        speed = currRouteData.BusSpeed,
                        fuel = _random.Next(0, 40),
                        engineoil = GetOil(_borough),
                        tirepressure = GetTirePressure(_borough),
                        odometer = _random.Next(0, 200000),
                        accelerator_pedal_position = _random.Next(0, 100),
                        parking_brake_status = GetRandomBoolean(),
                        headlamp_status = GetRandomBoolean(),
                        brake_pedal_status = GetRandomBoolean(),
                        transmission_gear_position = GetGearPos(),
                        ignition_status = GetRandomBoolean(),
                        windshield_wiper_status = GetRandomBoolean(),
                        abs = GetRandomBoolean(),
                        timestamp = DateTime.UtcNow,

                        // TODO: 16 Populate the machine learning prediction into the telemetry data for upstream systems
                        // mlDetectedAggressiveDriving = mlOutput.Prediction
                    };
                    var serializedString = JsonConvert.SerializeObject(info);
                    Console.WriteLine($"{DateTime.Now} > Sending message: {serializedString}");
                    var message = new Message(Encoding.UTF8.GetBytes(serializedString));
                    message.ContentEncoding = "utf-8";
                    message.ContentType = "application/json";

                    // TODO: 17 - Have the ModuleClient send the event message asynchronously, using the specified output name
                    // await _vehicleTelemetryModuleClient.SendEventAsync(outputName, message);

                    // TODO: 18 - Send all telemetry to local blob storage
                    // var blockBlob = _blobContainer.GetBlockBlobReference($"telemetry_{info.timestamp.Ticks}.json");
                    // blockBlob.UploadText(serializedString);
                }
                catch (AggregateException ex)
                {
                    foreach (Exception exception in ex.InnerExceptions)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Error in sample: {0}", exception);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error in sample: {0}", ex);
                }

                await Task.Delay(200);
            }
            
        }

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        private static async Task<MessageResponse> PipeMessage(Message message, object userContext)
        {
            int counterValue = Interlocked.Increment(ref _counter);

            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine($"Received message: {counterValue}, Body: [{messageString}]");

            if (!string.IsNullOrEmpty(messageString))
            {
                var pipeMessage = new Message(messageBytes);
                foreach (var prop in message.Properties)
                {
                    pipeMessage.Properties.Add(prop.Key, prop.Value);
                }
                await moduleClient.SendEventAsync("output1", pipeMessage);
                Console.WriteLine("Received message sent");
            }
            return MessageResponse.Completed;
        }


        /// <summary>
        /// This method updates current device instance fields with values from a Device Twin
        /// </summary>
        private static async Task UpdateDeviceInstanceFromDesiredProperties(TwinCollection desired)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (desired["VIN"] != null)
                    {
                        // TODO: 7 - Set the vin to the value in the device twin
                        // _vin = desired["VIN"];
                    }
                    if (desired["Borough"] != null)
                    {
                        // TODO: 8 - Set the borough to the value in the device twin
                        // _borough = desired["Borough"];
                    }

                    if (desired["Latitude"] != null)
                    {
                        // TODO: 9 - Set the latitude to the value in the device twin
                        // _latitude = (float)desired["Latitude"];
                    }
                    if (desired["Longitude"] != null)
                    {
                        // TODO: 10 - Set the longitude to the value in the device twin
                        // _longitude = (float)desired["Longitude"];
                    }
                    if(desired["Telemetry"]!=null){
                        _telemetryDefn = desired["Telemetry"];
                    }
                }
                catch (AggregateException ex)
                {
                    foreach (Exception exception in ex.InnerExceptions)
                    {
                        Console.WriteLine("Error when receiving desired property: {0}", exception);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error when receiving desired property: {0}", ex.Message);
                }

            });



        }

        /// <summary>
        /// This method sends current device field 'state' to the cloud through reported properties
        /// </summary>
        private static void UpdateReportedProperties(Object stateInfo)
        {
            try
            {

                // TODO: 12 - update reported properties with the IoT Hub with most recent Lat/Long
                // patch the changed properties (Latitude, Longitude, Borough)
                // TwinCollection patch = new TwinCollection();
                // patch["Latitude"] = _latitude;
                // patch["Longitude"] = _longitude;
                // patch["Borough"] = _borough;
                // patch["Type"] = "Bus";
                // patch["Telemetry"] = _telemetryDefn;
                // Task.Run(async () => await _deviceClient.UpdateReportedPropertiesAsync(patch));

            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error when receiving desired property: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error when receiving desired property: {0}", ex.Message);
            }

        }

        /// <summary>
        /// Randomly chooses a borough for telemetry data
        /// </summary>
        static string GetBorough()
        {
            return _boroughList[_random.Next(0, _boroughList.Count)];
        }

        /// <summary>
        /// Randomly generates oil level for telemetry data
        /// </summary>
        static int GetOil(string borough)
        {
            if (borough.ToLower() == GetBorough().ToLower())
            {
                return GetRandomWeightedNumber(50, 0, _lowOilProbabilityPower);
            }
            return GetRandomWeightedNumber(50, 0, _highOilProbabilityPower);
        }

        /// <summary>
        /// Randomly generates tire pressure for telemetry data
        /// </summary>
        static int GetTirePressure(string borough)
        {
            if (borough.ToLower() == GetBorough().ToLower())
            {
                return GetRandomWeightedNumber(50, 0, _lowTirePressureProbabilityPower);
            }
            return GetRandomWeightedNumber(50, 0, _highTirePressureProbabilityPower);
        }

        /// <summary>
        /// Randomly generates engine temperature for telemetry data
        /// </summary>
        static int GetEngineTemp(string borough)
        {
            if (borough.ToLower() == GetBorough().ToLower())
            {
                return GetRandomWeightedNumber(500, 0, _highEngineTempProbabilityPower);
            }
            return GetRandomWeightedNumber(500, 0, _lowEngineTempProbabilityPower);
        }

        /// <summary>
        /// Randomly generates outside temperature for telemetry data
        /// </summary>
        static int GetOutsideTemp(string borough)
        {
            if (borough.ToLower() == GetBorough().ToLower())
            {
                return GetRandomWeightedNumber(100, 0, _lowOutsideTempProbabilityPower);
            }
            return GetRandomWeightedNumber(100, 0, _highOutsideTempProbabilityPower);
        }

        /// <summary>
        /// Helper method assisting with generation of random telemetry data
        /// </summary>
        private static int GetRandomWeightedNumber(int max, int min, double probabilityPower)
        {
            var randomDouble = _random.NextDouble();

            var result = Math.Floor(min + (max + 1 - min) * (Math.Pow(randomDouble, probabilityPower)));
            return (int)result;
        }

        /// <summary>
        /// Randomly assigns a gear position for telemetry data
        /// </summary>
        static string GetGearPos()
        {
            List<string> list = new List<string>() { "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eight" };
            return list[_random.Next(list.Count())];
        }

        /// <summary>
        /// Randomly provides a boolean value for telemetry data
        /// </summary>
        static bool GetRandomBoolean()
        {
            return _random.NextBoolean();
        }
    }
}
