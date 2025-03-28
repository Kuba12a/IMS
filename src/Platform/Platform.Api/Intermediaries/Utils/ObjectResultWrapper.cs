using Microsoft.AspNetCore.Mvc;

namespace Platform.Api.Intermediaries.Utils;

public class ObjectResultWrapper : ObjectResult
{
    public ExceptionLogType ExceptionLogType { get; set; }

    public ObjectResultWrapper(object value) : base(value)
    {
        ExceptionLogType = ExceptionLogType.None;
    }
}
