
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Meziantou.Framework;

partial class HttpClientMock
{

    private static readonly string[] GetVerb = ["GET"];

    private static readonly string[] PostVerb = ["POST"];

    private static readonly string[] PutVerb = ["PUT"];

    private static readonly string[] PatchVerb = ["PATCH"];

    private static readonly string[] DeleteVerb = ["DELETE"];

    private static readonly string[] HeadVerb = ["HEAD"];

    private static readonly string[] OptionsVerb = ["OPTIONS"];


    public IEndpointConventionBuilder MapGet(string url, RequestDelegate handler) => Map(GetVerb, url, handler);
    public IEndpointConventionBuilder MapGet(string url, Delegate handler) => Map(GetVerb, url, handler);

    public IEndpointConventionBuilder MapPost(string url, RequestDelegate handler) => Map(PostVerb, url, handler);
    public IEndpointConventionBuilder MapPost(string url, Delegate handler) => Map(PostVerb, url, handler);

    public IEndpointConventionBuilder MapPut(string url, RequestDelegate handler) => Map(PutVerb, url, handler);
    public IEndpointConventionBuilder MapPut(string url, Delegate handler) => Map(PutVerb, url, handler);

    public IEndpointConventionBuilder MapPatch(string url, RequestDelegate handler) => Map(PatchVerb, url, handler);
    public IEndpointConventionBuilder MapPatch(string url, Delegate handler) => Map(PatchVerb, url, handler);

    public IEndpointConventionBuilder MapDelete(string url, RequestDelegate handler) => Map(DeleteVerb, url, handler);
    public IEndpointConventionBuilder MapDelete(string url, Delegate handler) => Map(DeleteVerb, url, handler);

    public IEndpointConventionBuilder MapHead(string url, RequestDelegate handler) => Map(HeadVerb, url, handler);
    public IEndpointConventionBuilder MapHead(string url, Delegate handler) => Map(HeadVerb, url, handler);

    public IEndpointConventionBuilder MapOptions(string url, RequestDelegate handler) => Map(OptionsVerb, url, handler);
    public IEndpointConventionBuilder MapOptions(string url, Delegate handler) => Map(OptionsVerb, url, handler);
}