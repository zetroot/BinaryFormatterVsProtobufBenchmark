using BenchmarkDotNet.Running;
using System;

namespace BinaryFormatterVsProtobufBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            _ = BenchmarkRunner.Run<SerializationBench>();
            _ = BenchmarkRunner.Run<DeserializationBench>();
        }
    }
}
