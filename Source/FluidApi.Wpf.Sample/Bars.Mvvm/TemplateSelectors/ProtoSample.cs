using ProtoBuf;
using ProtobufSourceGenerator;

namespace Bars.Mvvm.FluidGenerator.Sample;

[GeneratorOptions(PropertyAttributeType = typeof(string))]
[ProtoContract]
public class ProtoSample
{
    public int Number { get; set; }
}
