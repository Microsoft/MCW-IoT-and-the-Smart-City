// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Globalization;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.WebService.v1.Models
{
    public class ValueApiModel
    {
        [JsonProperty("Key")]
        public string Key { get; set; }

        [JsonProperty("Data")]
        public string Data { get; set; }

        [JsonProperty("ETag")]
        public string ETag { get; set; }

        [JsonProperty("$metadata")]
        public Dictionary<string, string> Metadata;

        public ValueApiModel(ValueServiceModel model)
        {
            this.Key = model.Key;
            this.Data = model.Data;
            this.ETag = model.ETag;

            this.Metadata = new Dictionary<string, string>
            {
                { "$type", $"Value;{Version.NUMBER}" },
                { "$modified", model.Timestamp.ToString(CultureInfo.InvariantCulture) },
                { "$uri", $"/{Version.PATH}/collections/{model.CollectionId}/values/{model.Key}" }
            };
        }
    }
}
