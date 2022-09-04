namespace Global.Patcher.Error;

using Global.Format;

public class SectionResult
{
    public enum ResultType
    {
        SUCCESS,
        GENERIC_FAILURE,
    }

    public Section? result {get;set;} = null;
    public bool success {get;set;} = false;

    public ResultType type {get;set;} = ResultType.GENERIC_FAILURE;

    public string message {get;set;} = "Unknown error";

    private SectionResult(Section? result, bool success)
    {
        this.result = result;
        this.success = success;
    }

    public static implicit operator bool (SectionResult result)
    {
        return result.success;
    }

    public static implicit operator Section (SectionResult result)
    {
        return result.result ?? throw new Exception("Section is invalid! reason: " + result.message);
    }

    public static SectionResult OK(Section value)
    {
        return new SectionResult(value, true);
    }

    public static SectionResult Error()
    {
        return new SectionResult(null, false);
    }

}