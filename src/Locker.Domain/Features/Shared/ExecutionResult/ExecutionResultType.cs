namespace Locker.Domain.Features.Shared.ExecutionResult
{
    public enum ExecutionResultType
    {
        Ok = 10,
        NotFound = 11,
        ValidationError = 12,
        UpstreamError = 13,
        UpstreamErrorNoResponse = 14,
        Unauthorized = 15,
        Forbidden = 16,
        Conflict = 17,
        Exception = 20,
        NotImplemented = 23,
        NoContent = 25,
    }
}