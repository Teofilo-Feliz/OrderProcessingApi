using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApi.Application.Helpers
{
        public class Result
        {
            public bool IsSuccess { get; }
            public bool IsFailure => !IsSuccess;

            public IReadOnlyList<string> Errors { get; }

            protected Result(bool isSuccess, IEnumerable<string>? errors)
            {
                IsSuccess = isSuccess;
                Errors = errors?.ToList() ?? new List<string>();

                if (IsSuccess && Errors.Count > 0)
                {
                    throw new ArgumentException(
                        "No se puede crear un resultado exitoso que contenga errores.");
                }

                if (IsFailure && Errors.Count == 0)
                {
                    throw new ArgumentException(
                        "No se puede crear un resultado fallido sin especificar al menos un error.");
                }
            }

            public static Result Success()
            {
                return new Result(true, null);
            }

            public static Result Failure(string error)
            {
                return new Result(false, new[] { error });
            }

            public static Result Failure(IEnumerable<string> errors)
            {
                return new Result(false, errors);
            }
        }
}

