namespace OcrDemo.Core.Utils;

public static class StreamExtensions
{
  public static byte[] ToByteArray(this Stream stream)
  {
    using var memoryStream = new MemoryStream();
    stream.CopyTo(memoryStream);
    return memoryStream.ToArray();
  }
}
