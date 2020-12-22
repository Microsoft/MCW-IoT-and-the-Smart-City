//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace VehicleTelemetrySimulator
{
    public class ModelInput
    {
        [ColumnName("Latitude"), LoadColumn(0)]
        public float Latitude { get; set; }


        [ColumnName("Longitude"), LoadColumn(1)]
        public float Longitude { get; set; }


        [ColumnName("BusSpeed"), LoadColumn(2)]
        public float BusSpeed { get; set; }


        [ColumnName("IsDangerous"), LoadColumn(3)]
        public bool IsDangerous { get; set; }


    }
}
