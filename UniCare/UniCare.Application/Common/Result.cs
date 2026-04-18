using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? Error { get; }

        private Result(T value) { IsSuccess = true; Value = value; }
        private Result(string error) { IsSuccess = false; Error = error; }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(string error) => new(error);
    }
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }

        private Result() { IsSuccess = true; }
        private Result(string error) { IsSuccess = false; Error = error; }

        public static Result Success() => new();
        public static Result Failure(string error) => new(error);
    }
}
