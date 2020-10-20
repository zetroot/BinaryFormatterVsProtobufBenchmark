using BenchmarkDotNet.Attributes;
using BinaryFormatterVsProtobufBenchmark.Protobuf;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BinaryFormatterVsProtobufBenchmark
{
    public class SerializationBench
    {
        private static Random random = new Random();
        private List<POCOClass> pocoList = new List<POCOClass>();
        private List<ProtoClass> protoList = new List<ProtoClass>();

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
            
            for(int i =0; i<ArrSize; ++i)
            {
                int intData = random.Next(int.MaxValue);
                string strData = RandomString(ObjSize);
                var doubles = Enumerable.Range(0, ObjSize).Select(_ => random.NextDouble()).ToList();
                pocoList.Add(new POCOClass { IntegerData = intData, StringData = strData, DoubleArr = doubles.ToList() });
                protoList.Add(new ProtoClass { IntegerData = intData, StringData = strData, DoubleArr = { doubles } });
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            pocoList.Clear();
            protoList.Clear();
        }

        [Benchmark]
        public List<byte[]> BinaryFormatterS11n()
        {
            var result = new List<byte[]>();
            var formatter = new BinaryFormatter();
            foreach(var inst in pocoList)
            {
                using var stream = new MemoryStream();
                formatter.Serialize(stream, inst);
                result.Add(stream.ToArray());
            }
            return result;
        }

        [Benchmark]
        public List<byte[]> ProtocolBuffersS11n()
        {
            var result = new List<byte[]>();            
            foreach (var inst in protoList)
            {                
                result.Add(inst.ToByteArray());
            }
            return result;
        }
    }
}
