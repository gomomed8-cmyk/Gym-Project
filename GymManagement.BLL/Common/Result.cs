using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Common
{
    public sealed record Result(bool success,string? erorr=null,ResultKind Kind=ResultKind.OK)
    {
        public static Result OK() => new (true);
        public static Result Fail(string erorr, ResultKind kind = ResultKind.Conflict) => new(false, erorr, kind);
        public static Result NotFound(string erorr = "NotFound") => new(false,erorr,ResultKind.NotFound);
        public static Result Vaildation(string error) => new(false,error,ResultKind.VaildationFailed);
    }
    public sealed record Result<T>(bool success , T? value, string? error=null,ResultKind Kind=ResultKind.OK)
    {
        public static Result<T> Ok(T value)=>new (true,value);
        public static Result<T> Fail(string error ,ResultKind kind=ResultKind.Conflict) => new(false,default,error,kind);
        public static Result<T> NotFound(string error="NotFound")=>new(false,default,error,ResultKind.NotFound);
    }
}
