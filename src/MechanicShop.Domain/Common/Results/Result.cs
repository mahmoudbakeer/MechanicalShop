using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace MechanicShop.Domain.Common.Results;



public static class Result
{
    public static Success Success => default;
    public static Created Created => default;
    public static Updated Updated => default;
    public static Deleted Deleted => default;
}

public sealed class Result<TValue> : IResult<TValue>
{
    private readonly List<Error>? _errors = null;
    private readonly TValue? _value = default;


    public bool IsSuccess { get; }
    public bool IsError => !IsSuccess;

    public TValue Value => IsSuccess ? _value! : default!;

    public List<Error>? Errors => IsError ? _errors : [];


    public Error TopError => (_errors?.Count > 0) ? _errors[0] : default;


    private Result(Error error)
    {
        _errors = [error];
    }

    [JsonConstructor]
    [Obsolete("For Json Serialization only.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Result(TValue? value, List<Error>? errors, bool isSuccess)
    {
        if (isSuccess)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
            _errors = default;
            IsSuccess = isSuccess;
        }
        else
        {
            if (errors is null || errors.Count == 0)
            {
                throw new ArgumentException("Cannot Create An ErrorOr<TValue> from an empty collection of errors. Please provide at least one error.",
                nameof(errors));
            }
            _errors = errors;
            _value = default;
            IsSuccess = isSuccess;
        }

    }
    private Result(List<Error>? errors)
    {
        if (errors is null || errors.Count == 0)
        {
            throw new ArgumentException("Cannot Create An ErrorOr<TValue> from an empty collection of errors. Please provide at least one error.",
            nameof(errors));
        }
        _errors = errors;
        IsSuccess = false;
    }

    private Result(TValue value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        _value = value;
        IsSuccess = true;
    }

    public TNextValue Match<TNextValue>(Func<TValue, TNextValue> OnValue, Func<List<Error>?, TNextValue> OnError)
           => IsSuccess ? OnValue(_value!) : OnError(_errors);
    public static implicit operator Result<TValue>(TValue value)
           => new(value);
    public static implicit operator Result<TValue>(Error error)
           => new(error);
    public static implicit operator Result<TValue>(List<Error> errors)
           => new(errors);

}

public readonly record struct Success;
public readonly record struct Created;
public readonly record struct Updated;
public readonly record struct Deleted;