using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Reflection;

namespace Collo.Cloud.Services.Libraries.Shared.Extensions
{
    /// <summary>
    ///Terrible reflection code since I haven't found a nicer way to do this...
    // For some reason the types are marked as internal
    // If there's code that will break in this sample,
    // it's probably here.
    /// </summary>
    public static class FunctionContextExtensions
    {
        public static (List<string> permittedChildrenIds, bool isAnyPermitted) GetPermittedObjects(this HttpRequestData req)
        {
            var permittedChildrenIds = (List<string>)req.FunctionContext.Items["permittedIds"];
            var isAnyPermitted = (bool)req.FunctionContext.Items["isAnyPermitted"];

            return (permittedChildrenIds, isAnyPermitted);
        }

        public static void SetHttpResponseStatusCode(this FunctionContext context, HttpStatusCode statusCode)
        {
            var coreAssembly = Assembly.Load("Microsoft.Azure.Functions.Worker.Core");
            var featureInterfaceName = "Microsoft.Azure.Functions.Worker.Context.Features.IFunctionBindingsFeature";
            var featureInterfaceType = coreAssembly.GetType(featureInterfaceName);
            var bindingsFeature = context.Features.Single(
                f => f.Key.FullName == featureInterfaceType.FullName).Value;
            var invocationResultProp = featureInterfaceType.GetProperty("InvocationResult");

            var grpcAssembly = Assembly.Load("Microsoft.Azure.Functions.Worker.Grpc");
            var responseDataType = grpcAssembly.GetType("Microsoft.Azure.Functions.Worker.GrpcHttpResponseData");
            var responseData = Activator.CreateInstance(responseDataType, context, statusCode);

            invocationResultProp.SetMethod.Invoke(bindingsFeature, new object[] { responseData });
        }

        public static void SetHttpResponseData(this FunctionContext funcContext, HttpResponseData httpResponseData)
        {
            var keyValue = funcContext?.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
            var funcBinding = keyValue?.Value;
            var type = funcBinding?.GetType();
            type?.GetProperties()
            .Single(o => o.Name == "InvocationResult")
            .SetValue(funcBinding, httpResponseData);
        }

        /// <summary>
        ///   // More terrible reflection code..
        // Would be nice if this was available out of the box on FunctionContext

        // This contains the fully qualified name of the method
        // E.g. IsolatedFunctionAuth.TestFunctions.ScopesAndAppRoles
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>

        public static MethodInfo GetTargetFunctionMethod(this FunctionContext context)
        {

            var entryPoint = context.FunctionDefinition.EntryPoint;

            var assemblyPath = context.FunctionDefinition.PathToAssembly;
            var assembly = Assembly.LoadFrom(assemblyPath);
            var typeName = entryPoint.Substring(0, entryPoint.LastIndexOf('.'));
            var type = assembly.GetType(typeName);
            var methodName = entryPoint.Substring(entryPoint.LastIndexOf('.') + 1);
            var method = type.GetMethod(methodName);

            return method;
        }

        /// <summary>
        ///  This method is for get request data.
        /// </summary>
        /// <param name="funcContext"></param>
        /// <returns></returns>

        public static HttpRequestData GetHttpRequestData(this FunctionContext funcContext)
        {
            var keyValue = funcContext?.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
            var funcBinding = keyValue?.Value;
            var type = funcBinding?.GetType();
            var inputData = type?.GetProperties().Single(p => p.Name == "InputData").GetValue(funcBinding) as IReadOnlyDictionary<string, object>;
            return inputData?.Values?.SingleOrDefault(obj => obj is HttpRequestData) as HttpRequestData;
        }

        /// <summary>
        ///  This method is for get qury data from request data.
        /// </summary>
        /// <param name="funcContext"></param>
        /// <returns></returns>

        public static string GetQuery(this FunctionContext funcContext, string key)
        {
            var request = funcContext.GetHttpRequestData();
            var query = System.Web.HttpUtility.ParseQueryString(request.Url.Query);
            if (string.IsNullOrEmpty(key)) return null;

            return query[key];
        }
    }
}
