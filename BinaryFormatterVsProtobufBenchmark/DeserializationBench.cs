using BenchmarkDotNet.Attributes;
using BinaryFormatterVsProtobufBenchmark.Protobuf;
using Google.Protobuf;
using Iced.Intel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BinaryFormatterVsProtobufBenchmark
{
    public class DeserializationBench
    {
        private static Random random = new Random();
        private List<byte[]> pocoList = new List<byte[]>();
        private List<byte[]> protoList = new List<byte[]>();

        [Params(100, 1000)]
        public int ArrSize { get; set; }

        [Params(80, 8000, 80000)]
        public int ObjSize { get; set; }

        private static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrtsuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [GlobalSetup]
        public void Setup()
        {
            pocoList.Clear();
            protoList.Clear();
            var formatter = new BinaryFormatter();
            for (int i = 0; i < ArrSize; ++i)
            {
                int intData = random.Next(int.MaxValue);
                string strData = RandomString(ObjSize);
                var doubles = Enumerable.Range(0, ObjSize).Select(_ => random.NextDouble()).ToList();

                var poco = new POCOClass { IntegerData = intData, StringData = strData, DoubleArr = doubles.ToList() };
                var proto = new ProtoClass { IntegerData = intData, StringData = strData, DoubleArr = { doubles } };

                using var stream = new MemoryStream();
                formatter.Serialize(stream, poco);
                pocoList.Add(stream.ToArray());

                protoList.Add(proto.ToByteArray());                
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            pocoList.Clear();
            protoList.Clear();
        }

        [Benchmark]
        public List<POCOClass> BinaryD12on()
        {
            var result = new List<POCOClass>();
            var formatter = new BinaryFormatter();
            foreach (var bytes in pocoList)
            {
                using var stream = new MemoryStream(bytes);
                result.Add((POCOClass)formatter.Deserialize(stream));
            }
            return result;
        }

        [Benchmark]
        public List<ProtoClass> ProtocolBuffersD12on()
        {
            var result = new List<ProtoClass>();
            foreach (var inst in protoList)
            {
                var proto = new ProtoClass();
                proto.MergeFrom(inst);
                result.Add(proto);
            }
            return result;
        }
    }
}
