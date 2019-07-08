using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace cats_api
{

    internal class AwsSecretManagerConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AwsSecretManagerConfigurationProvider();
        }
    }

    internal class AwsSecretManagerConfigurationProvider : ConfigurationProvider
    {
        public string RawData { get; private set; }

        public override void Load()
        {
            var rawData = RawData;
            RawData = JsonConvert.SerializeObject(new { ConnectionString = "whatever the value should be ..." });
            Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(RawData);
        }

    }
}