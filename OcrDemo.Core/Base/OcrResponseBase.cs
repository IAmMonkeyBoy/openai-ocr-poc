namespace OcrDemo.Core.Base;

public class OcrResponseBase<T>
{
    public OcrResponseBase(T document)
    {
        Document = document;
    }
    public T Document { get; set; }
}