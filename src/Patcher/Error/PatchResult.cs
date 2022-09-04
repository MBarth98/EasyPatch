namespace Global.Patcher.Error;

using Global.Format;

public class PatchResult
{
    public enum ResultType
    {
        SUCCESS,
        GENERIC_FAILURE,
    }

    public Patch? result {get;set;} = null;
    public bool success {get;set;} = false;

    public ResultType type {get;set;} = ResultType.GENERIC_FAILURE;

    public string message {get;set;} = "Unknown error";

    private PatchResult(Patch? result, bool success)
    {
        this.result = result;
        this.success = success;
    }

    public static implicit operator bool (PatchResult result)
    {
        return result.success;
    }

    public static implicit operator Patch (PatchResult result)
    {
        return result.result ?? throw new Exception("Patch is invalid! reason: " + result.message);
    }

    public static PatchResult OK(Patch value)
    {
        return new PatchResult(value, true);
    }

    public static PatchResult Error()
    {
        return new PatchResult(null, false);
    }

    public static PatchResult Error(string message, string file)
    {
        return new PatchResult(null, false);
    }
}
