using System.Linq.Expressions;
using System.Reflection;

namespace Meziantou.Framework;

/// <summary>
/// Helper for detecting whether a given type is FSharpAsync`1, and if so, supplying
/// an <see cref="Expression"/> for mapping instances of that type to a C# awaitable.
/// </summary>
/// <remarks>
/// The main design goal here is to avoid taking a compile-time dependency on
/// FSharp.Core.dll, because non-F# applications wouldn't use it. So all the references
/// to FSharp types have to be constructed dynamically at runtime.
/// </remarks>
internal static class ObjectMethodExecutorFSharpSupport
{
    private static readonly Lock FsharpValuesCacheLock = new();
    private static Assembly? s_fsharpCoreAssembly;
    private static MethodInfo? s_fsharpAsyncStartAsTaskGenericMethod;
    private static PropertyInfo? s_fsharpOptionOfTaskCreationOptionsNoneProperty;
    private static PropertyInfo? s_fsharpOptionOfCancellationTokenNoneProperty;

    [UnconditionalSuppressMessage("Trimmer", "IL2060", Justification = "Reflecting over the async FSharpAsync<> contract.")]
    public static bool TryBuildCoercerFromFSharpAsyncToAwaitable(
        Type possibleFSharpAsyncType,
        [NotNullWhen(true)] out Expression? coerceToAwaitableExpression,
        [NotNullWhen(true)] out Type? awaitableType)
    {
        var methodReturnGenericType = possibleFSharpAsyncType.IsGenericType
            ? possibleFSharpAsyncType.GetGenericTypeDefinition()
            : null;

        if (!IsFSharpAsyncOpenGenericType(methodReturnGenericType))
        {
            coerceToAwaitableExpression = null;
            awaitableType = null;
            return false;
        }

        var awaiterResultType = possibleFSharpAsyncType.GetGenericArguments().Single();
        awaitableType = typeof(Task<>).MakeGenericType(awaiterResultType);

        // coerceToAwaitableExpression = (object fsharpAsync) =>
        // {
        //     return (object)FSharpAsync.StartAsTask<TResult>(
        //         (Microsoft.FSharp.Control.FSharpAsync<TResult>)fsharpAsync,
        //         FSharpOption<TaskCreationOptions>.None,
        //         FSharpOption<CancellationToken>.None);
        // };
        var startAsTaskClosedMethod = s_fsharpAsyncStartAsTaskGenericMethod!
            .MakeGenericMethod(awaiterResultType);
        var coerceToAwaitableParam = Expression.Parameter(typeof(object));
        coerceToAwaitableExpression = Expression.Lambda(
            Expression.Convert(
                Expression.Call(
                    startAsTaskClosedMethod,
                    Expression.Convert(coerceToAwaitableParam, possibleFSharpAsyncType),
                    Expression.MakeMemberAccess(expression: null, s_fsharpOptionOfTaskCreationOptionsNoneProperty!),
                    Expression.MakeMemberAccess(expression: null, s_fsharpOptionOfCancellationTokenNoneProperty!)),
                typeof(object)),
            coerceToAwaitableParam);

        return true;
    }

    private static bool IsFSharpAsyncOpenGenericType(Type? possibleFSharpAsyncGenericType)
    {
        if (possibleFSharpAsyncGenericType == null)
            return false;

        var typeFullName = possibleFSharpAsyncGenericType.FullName;
        if (typeFullName != "Microsoft.FSharp.Control.FSharpAsync`1")
            return false;

        lock (FsharpValuesCacheLock)
        {
            if (s_fsharpCoreAssembly != null)
            {
                // Since we've already found the real FSharpAsync.Core assembly, we just have
                // to check that the supplied FSharpAsync`1 type is the one from that assembly.
                return possibleFSharpAsyncGenericType.Assembly == s_fsharpCoreAssembly;
            }
            else
            {
                // We'll keep trying to find the FSharp types/values each time any type called
                // FSharpAsync`1 is supplied.
                return TryPopulateFSharpValueCaches(possibleFSharpAsyncGenericType);
            }
        }
    }

    [UnconditionalSuppressMessage("Trimmer", "IL2026", Justification = "Reflecting over the async FSharpAsync<> contract")]
    [UnconditionalSuppressMessage("Trimmer", "IL2055", Justification = "Reflecting over the async FSharpAsync<> contract")]
    [UnconditionalSuppressMessage("Trimmer", "IL2072", Justification = "Reflecting over the async FSharpAsync<> contract")]
    private static bool TryPopulateFSharpValueCaches(Type possibleFSharpAsyncGenericType)
    {
        var assembly = possibleFSharpAsyncGenericType.Assembly;
        var fsharpOptionType = assembly.GetType("Microsoft.FSharp.Core.FSharpOption`1");
        var fsharpAsyncType = assembly.GetType("Microsoft.FSharp.Control.FSharpAsync");

        if (fsharpOptionType == null || fsharpAsyncType == null)
            return false;

        // Get a reference to FSharpOption<TaskCreationOptions>.None
        var fsharpOptionOfTaskCreationOptionsType = fsharpOptionType.MakeGenericType(typeof(TaskCreationOptions));
        s_fsharpOptionOfTaskCreationOptionsNoneProperty = fsharpOptionOfTaskCreationOptionsType.GetRuntimeProperty("None");

        // Get a reference to FSharpOption<CancellationToken>.None
        var fsharpOptionOfCancellationTokenType = fsharpOptionType.MakeGenericType(typeof(CancellationToken));
        s_fsharpOptionOfCancellationTokenNoneProperty = fsharpOptionOfCancellationTokenType.GetRuntimeProperty("None");

        // Get a reference to FSharpAsync.StartAsTask<>
        var fsharpAsyncMethods = fsharpAsyncType.GetRuntimeMethods().Where(m => m.Name == "StartAsTask");
        foreach (var candidateMethodInfo in fsharpAsyncMethods)
        {
            var parameters = candidateMethodInfo.GetParameters();
            if (parameters.Length == 3
                && TypesHaveSameIdentity(parameters[0].ParameterType, possibleFSharpAsyncGenericType)
                && parameters[1].ParameterType == fsharpOptionOfTaskCreationOptionsType
                && parameters[2].ParameterType == fsharpOptionOfCancellationTokenType)
            {
                // This really does look like the correct method (and hence assembly).
                s_fsharpAsyncStartAsTaskGenericMethod = candidateMethodInfo;
                s_fsharpCoreAssembly = assembly;
                break;
            }
        }

        return s_fsharpCoreAssembly != null;
    }

    private static bool TypesHaveSameIdentity(Type type1, Type type2)
    {
        return type1.Assembly == type2.Assembly
            && string.Equals(type1.Namespace, type2.Namespace, StringComparison.Ordinal)
            && string.Equals(type1.Name, type2.Name, StringComparison.Ordinal);
    }
}