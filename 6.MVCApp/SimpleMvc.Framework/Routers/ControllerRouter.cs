﻿using SimpleMvc.Framework.Attributes.Methods;
using SimpleMvc.Framework.Contracts;
using SimpleMvc.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebServer.Contracts;
using WebServer.Enums;
using WebServer.Exceptions;
using WebServer.Http.Contracts;
using WebServer.Http.Response;

namespace SimpleMvc.Framework.Routers
{
    public class ControllerRouter : IHandleable
    {
        private IDictionary<string, string> getParameters;
        private IDictionary<string, string> postParameters;
        private string controllerName;
        private object controllerInstance;
        private string actionName;
        private string requestMethod;
        private object[] methodParams;

        public IHttpResponse Handle(IHttpRequest request)
        {
            this.getParameters = new Dictionary<string, string>(request.UrlParameters);
            this.postParameters = new Dictionary<string, string>(request.FormData);
            this.requestMethod = request.Method.ToString().ToUpper();

            this.PrepareControllerAndActionNames(request);

            var methodInfo = this.GetActionForExecution();

            if (methodInfo == null)
            {
                return new NotFoundResponse();
            }

            this.PrepareMethodParameters(methodInfo);

            var actionResult = (IInvocable)methodInfo.Invoke(this.GetControllerInstace(),this.methodParams);

            string content = actionResult.Invoke();

            return new ContentResponse(HttpStatusCode.Ok,content);
        }

        private void PrepareControllerAndActionNames(IHttpRequest request)
        {
            var pathParts = request.Path.Split(new[] { '?', '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (pathParts.Length < 2)
            {
                BadRequestException.ThrowFromInvalidRequest();
            }

            this.controllerName = $"{pathParts[0].Capitalize()}{MvcContext.Get.ControllersSuffix}";
            this.actionName = pathParts[1].Capitalize();

        }

        private MethodInfo GetActionForExecution()
        {
            foreach (var method in this.GetSuitableMethods())
            {
                var httpMethodAttributes = method.GetCustomAttributes()
                    .Where(a => a is HttpMethodAttribute)
                    .Cast<HttpMethodAttribute>();

                if(!httpMethodAttributes.Any() && requestMethod == "GET")
                {
                    return method;
                }

                foreach (var httpMethodAttribute in httpMethodAttributes)
                {
                    if (httpMethodAttribute.IsValid(this.requestMethod))
                    {
                        return method;
                    }

                }
            }

            return null;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods()
        {
            var controller = this.GetControllerInstace();

            if (controller == null)
            {
                return new MethodInfo[0];
            }

            return controller
                .GetType()
                .GetMethods()
                .Where(m => m.Name == actionName);
        }

        private object GetControllerInstace()
        {
            if (controllerInstance != null)
            {
                return controllerInstance;
            }

            var controllerFullQualifiedName = string.Format(
                "{0}.{1}.{2}, {0}",
                MvcContext.Get.AssemblyName,
                MvcContext.Get.ControllersFolders,
                this.controllerName);

            var controllerType = Type.GetType(controllerFullQualifiedName);

            if (controllerType == null)
            {
                return null;
            }

            controllerInstance = Activator.CreateInstance(controllerType);

            return controllerInstance;
        }

        private void PrepareMethodParameters(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            this.methodParams = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                if (parameter.ParameterType.IsPrimitive || parameter.ParameterType == typeof(string))
                {
                    var getParamValue = this.getParameters[parameter.Name];

                    var value = Convert.ChangeType(getParamValue, parameter.ParameterType);

                    this.methodParams[i] = value;

                }
                else
                {
                    var modelType = parameter.ParameterType;

                    var modelInstance = Activator.CreateInstance(modelType);

                    var modelProperties = modelType.GetProperties();

                    foreach (var modelProperty in modelProperties)
                    {
                        var postParamValue = this.postParameters[modelProperty.Name];

                        var value = Convert.ChangeType(postParamValue, modelProperty.PropertyType);

                        modelProperty.SetValue(modelInstance, value);              
                    }
                    this.methodParams[i] = Convert.ChangeType(modelInstance, modelType);
                }
            }
        }
    }
}
