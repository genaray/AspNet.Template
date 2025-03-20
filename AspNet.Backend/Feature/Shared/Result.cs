using Microsoft.AspNetCore.Mvc;

namespace AspNet.Backend.Feature.Shared;

/// <summary>
/// The <see cref="Result"/> struct
/// acts as a response or result from a service class that contains its success and error messages. 
/// </summary>
public struct Result
{
    public bool Success { get; set; }
    public AppException? Error { get; set; }

    public override string ToString()
    {
        return $"{nameof(Success)}: {Success}, {nameof(Error)}: {Error}";
    }
}

/// <summary>
/// The <see cref="Response{T}"/> struct
/// acts as a response with <see cref="Result"/> and data.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
public struct Response<T>
{
    public required Result Result { get; set; }
    public T? Data { get; set; }
    
    // Deconstructor
    public void Deconstruct(out Result result, out T? data)
    {
        result = Result;
        data = Data;
    }

    public override string ToString()
    {
        return $"{nameof(Result)}: {Result}, {nameof(Data)}: {Data}";
    }
}