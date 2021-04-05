using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Models.Api
{
    public class ErrorResult
    {
        public List<string> Errors { get; private set; }

        public ErrorResult(string errorMessage)
        {
            Errors = new List<string> { errorMessage };
        }

        public ErrorResult(List<string> errorMessages)
        {
            Errors = errorMessages;
        }

        public ErrorResult(Exception exception) : this(exception.Message)
        {
        }
    }
}
