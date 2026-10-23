using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApi.Application.Helpers
{
    public sealed class Result<T> : Result
    {
        public T? Value { get; }

        private Result(
            bool isSuccess,
            T? value,
            IEnumerable<string>? errors)
            : base(isSuccess, errors)
        {
            Value = value;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null);
        }

        public new static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, new[] { error });
        }

        public new static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false, default, errors);
        }
    }
}
