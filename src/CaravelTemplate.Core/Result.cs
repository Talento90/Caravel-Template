using Caravel.Exceptions;

namespace CaravelTemplate.Core
{
    public sealed class Result<T>
    {
        public T Data { get; }
        public CaravelException? Exception { get; }
        
        public bool Success => Exception != null;
        
        private Result(T data)
        {
            Data = data;
        }
        
        private Result(CaravelException ex)
        {
            Data = default!;
            Exception = ex;
        }

        public static Result<T> Create(T data)
        {
            return new Result<T>(data);
        }
        
        public static Result<T> Create(CaravelException ex)
        {
            return new Result<T>(ex);
        }
    }
}