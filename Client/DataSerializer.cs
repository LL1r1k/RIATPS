using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Client
{
    public class DataSerializer
    {
        public enum SerializationType
        {
            xml,
            json
        }

        static SerializationType Type { get; set; }

        public DataSerializer(string type)
        {
            Type = (SerializationType)Enum.Parse(typeof(SerializationType), type);
        }

        public Input Deserialize(string input)
        {
            Input deserialized;

            switch (Type)
            {
                case SerializationType.xml:

                    var serializer = new XmlSerializer(typeof(Input));

                    using (TextReader reader = new StringReader(input))
                    {
                        deserialized = (Input)serializer.Deserialize(reader);
                    }

                    break;
                case SerializationType.json:

                    deserialized = JsonConvert.DeserializeObject<Input>(input);

                    break;
                default:

                    return null;
            }

            return deserialized;
        }

        public string Serialize(Output input)
        {
            string serializedToString;

            switch (Type)
            {
                case SerializationType.xml:

                    using (var stringWriter = new StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(Output));

                        serializer.Serialize(stringWriter, input);
                        serializedToString = stringWriter.ToString();
                    }

                    break;
                case SerializationType.json:

                    serializedToString = JsonConvert.SerializeObject(input);

                    break;
                default:

                    return null;
            }

            return serializedToString;
        }

        public string GetAnswer(string serialized)
        {
            Input input = Deserialize(serialized);
            Output output = new Output(input);

            return Serialize(output);
        }
    }
}
