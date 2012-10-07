using System;

namespace NReadability
{
  public class WebTranscodingInput
  {
    private DomSerializationParams _domSerializationParams;

    public WebTranscodingInput(string url, string css=null)
    {
      if (string.IsNullOrEmpty(url))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "url");
      }

      Url = url;
      //  Css = css;
    }

    public string Url { get; private set; }
   // public string Css { get; private set; }

    public DomSerializationParams DomSerializationParams
    {
      get { return _domSerializationParams ?? (_domSerializationParams = DomSerializationParams.CreateDefault()); }
      set { _domSerializationParams = value; }
    }
  }
}
