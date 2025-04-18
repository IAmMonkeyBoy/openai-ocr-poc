namespace OcrDemo.Core.Utils;

public static class ImageDataExtensions
{
  public static string InferImageTypeFromBytes(this byte[] bytes)
  {
    if (bytes.Length < 4)
      throw new ArgumentException("Byte array is too small to determine the image type.");

    // Check for PNG
    if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
      return "image/png";

    // Check for JPEG
    if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
      return "image/jpeg";

    // Check for GIF
    if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
      return "image/gif";

    // Check for WEBP
    if (bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46)
      return "image/webp";

    throw new NotSupportedException("Unsupported image type.");
  }
}
