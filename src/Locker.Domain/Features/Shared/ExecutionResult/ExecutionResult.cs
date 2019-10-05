namespace Locker.Domain.Features.Shared.ExecutionResult
{
    public class ExecutionResult
    {
        public ExecutionResultType ResultType { get; set; }

        public string Message { get; set; }

        public bool IsValid()
        {
            return (int)ResultType <= (int)ExecutionResultType.Ok;
        }
    }

    public class ExecutionResult<T> : ExecutionResult
    {
        public T Value { get; set; }
    }
}