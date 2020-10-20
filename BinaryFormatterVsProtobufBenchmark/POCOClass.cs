using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryFormatterVsProtobufBenchmark
{
    [Serializable]
    public class POCOClass
    {
        public int IntegerData { get; set; }
        public string StringData { get; set; }
        public List<double> DoubleArr { get; set; } = new List<double>();
    }
}
