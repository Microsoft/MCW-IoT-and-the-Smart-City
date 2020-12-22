using System;

namespace VehicleTelemetrySimulator
{
    class BusEvent
    {
        public string vin { get; set; }
        public int outsideTemperature { get; set; }
        public int engineTemperature { get; set; }
        public double speed { get; set; }
        public int fuel { get; set; }
        public int engineoil { get; set; }
        public int tirepressure { get; set; }
        public int odometer { get; set; }
        public int accelerator_pedal_position { get; set; }
        public bool parking_brake_status { get; set; }
        public bool brake_pedal_status { get; set; }
        public bool headlamp_status { get; set; }
        public string transmission_gear_position { get; set; }
        public bool ignition_status { get; set; }
        public bool windshield_wiper_status { get; set; }
        public bool abs { get; set; }
        public bool mlDetectedAggressiveDriving {get;set;}
        public DateTime timestamp { get; set; }

    }
}