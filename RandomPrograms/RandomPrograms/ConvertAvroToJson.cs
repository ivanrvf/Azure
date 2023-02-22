using System;
using System.Collections.Generic;
using System.Text.Json;
using SolTechnology.Avro;
using SolTechnology.Avro.AvroObjectServices.Schemas;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Text;
using SolTechnology.Avro.Infrastructure.Attributes;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Azure.Messaging.EventHubs;
using Avro.Generic;
using Avro.File;
using Avro.Specific;
using Avro;
using System.Configuration;

namespace RandomPrograms
{
    class ConvertAvroToJson
    {
        static void Main(string[] args)
        {
            var avroBytes = File.ReadAllBytes(ConfigurationManager.AppSettings["AvroFileLocation"].ToString()); // Read avro file. 
            var schema = AvroConvert.GetSchema(avroBytes); // Get avro schema using AvroConvert library -> Nuget Link: https://www.nuget.org/packages/AvroConvert#readme-body-tab
                                                           // Github link: https://github.com/AdrianStrugala/AvroConvert/blob/master/docs/Documentation.md
            System.Diagnostics.Debug.WriteLine("Schema: " + schema.ToString()); // Printing the schema in Debug output window of Visual Studio.
            System.Diagnostics.Debug.WriteLine("Expected Model: " + AvroConvert.GenerateModel(schema)); // Generating model class and property based on the schema we got in previous statement. Printing the model.
            try
            {
                
                var result = AvroConvert.Avro2Json(avroBytes);
                JsonDocument jd = JsonDocument.Parse(result.ToString()); // Parse the string return type of avro2Json to JsonDocument using System.Text.Json package. link: https://www.nuget.org/packages/System.Text.Json
                int i = 1;
                if (jd.RootElement.GetArrayLength() > 0) // We expect the avro file to have multiple records. So get the length of the parsed avro file to Json to ensure data is present. 
                {
                    foreach (JsonElement e in jd.RootElement.EnumerateArray())
                    {
                        byte[] body = e.GetProperty("Body").GetBytesFromBase64(); // Body is in base64 string format. Convert it into byte[]. 
                        Int64 SeqNum = e.GetProperty("SequenceNumber").GetInt64();
                        string Offset = e.GetProperty("Offset").GetString();
                        string TimeUtc = e.GetProperty("EnqueuedTimeUtc").GetString();
                        JsonElement SystemProperties = e.GetProperty("SystemProperties");
                        JsonElement Properties = e.GetProperty("Properties");
                        System.Diagnostics.Debug.WriteLine("Event number: " + i);
                        System.Diagnostics.Debug.WriteLine("Body: " + Encoding.Default.GetString(body)); // The body of event in EventHub after parsing from avro would be base64 format. We need to get that body into byte array and use System.Text.Encoding lib to convert it into format you expect the event data to be in. 
                        System.Diagnostics.Debug.WriteLine("Sequence number: " + SeqNum.ToString());
                        System.Diagnostics.Debug.WriteLine("EnqueuedTimeUTC: " + TimeUtc);
                        System.Diagnostics.Debug.WriteLine("Offset: " + Offset);
                        System.Diagnostics.Debug.WriteLine("SystemProperties: " + SystemProperties.ToString()); 
                        System.Diagnostics.Debug.WriteLine("Properties: " + Properties.ToString());
                        System.Diagnostics.Debug.WriteLine("");
                        
                        /*
                         Output: 
                        Schema: {"type":"record","name":"EventData","namespace":"Microsoft.ServiceBus.Messaging","fields":[{"name":"SequenceNumber","type":"long"},{"name":"Offset","type":"string"},{"name":"EnqueuedTimeUtc","type":"string"},{"name":"SystemProperties","type":{"type":"map","values":["long","double","string","bytes"]}},{"name":"Properties","type":{"type":"map","values":["long","double","string","bytes","null"]}},{"name":"Body","type":["null","bytes"]}]}
                        Length: 11730
                        Event number: 11730
                        Body: Event 5 at 2/6/2023 6:29:45 PM
                        Sequence number: 176265
                        EnqueuedTimeUTC: 2/6/2023 6:29:45 PM
                        Offset: 154619760976
                        .
                        .
                        .
                        Event number: 11728
                        Body: Event 3 at 2/6/2023 6:29:45 PM
                        Sequence number: 176263
                        EnqueuedTimeUTC: 2/6/2023 6:29:45 PM
                        Offset: 154619760816

                        Event number: 11729
                        Body: Event 4 at 2/6/2023 6:29:45 PM
                        Sequence number: 176264
                        EnqueuedTimeUTC: 2/6/2023 6:29:45 PM
                        Offset: 154619760896
                         */
                        i++;
                    }
                }
                else {
                    System.Diagnostics.Debug.WriteLine("No data in avro file");
                }

                // Testing Avro library
                //RecordSchema<EventData> rs = new 
                //DatumReader<EventData> eventDataDatumReader = new SpecificDatumReader<EventData>(RecordSchema.Create("EventData", new List<Avro.Field>()), RecordSchema.Create("EventData", new List<Avro.Field>()));
               /* DataFileReader<EventData> empDatumReader = (DataFileReader<EventData>)DataFileReader<EventData>.OpenReader("C:/Users/ivferna/Downloads/34.avro", RecordSchema.Create( "EventData", new List<Avro.Field>()));
                EventData test = empDatumReader.Next();
               */

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
                Exception innerException = e.InnerException;
                while(innerException != null)
                {
                    System.Diagnostics.Debug.WriteLine(innerException.Message);
                    System.Diagnostics.Debug.WriteLine(innerException.StackTrace);
                    System.Diagnostics.Debug.WriteLine(innerException.Data);
                    System.Diagnostics.Debug.WriteLine("End of "+innerException.ToString());
                    innerException = innerException.InnerException;
                }
                    
            }
        }
    }
    /*
    public class EventData
    {
        public long SequenceNumber { get; set; }
        public string Offset { get; set; }
        public string EnqueuedTimeUtc { get; set; }

        //[NullableSchema]
        //[DefaultValue("['long','double','string','bytes'")]
        public ReadOnlyDictionary<string, object> SystemProperties { get; set; }

        //[NullableSchema]
        //[DefaultValue("['long','double','string','bytes'")]
        public Dictionary<string, object> Properties { get; set; }
        
        [DefaultValue("test")]
        public string Body { get; set; }
    }
    public class map<dynamic> { 
        public dynamic root { get; set; }
    }
    public struct MyValue
    {
        public string Value1;
        public double Value2;
        public long Value3;
        public byte[] Value4;
    }*/
}
