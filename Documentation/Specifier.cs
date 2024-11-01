using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Documentation;

public class Specifier<T> : ISpecifier
{
    private Type targetType = typeof(T);
    public string GetApiDescription()
    {
        var apiDescriptionAttribute = targetType.GetCustomAttributes()
            .OfType<ApiDescriptionAttribute>().FirstOrDefault();
        return apiDescriptionAttribute?.Description;
    }

    public string[] GetApiMethodNames()
    {
        return targetType.GetMethods()
            .Where(method => method.GetCustomAttributes().OfType<ApiMethodAttribute>().Any())
            .Select(method => method.Name).ToArray();
    }

    public string GetApiMethodDescription(string methodName)
    {
        var methodInfo = targetType.GetMethod(methodName);
        if (methodInfo == null || !methodInfo.GetCustomAttributes().OfType<ApiMethodAttribute>().Any())
            return null;
        var methodDescriptionAttribute = methodInfo.GetCustomAttributes()
            .OfType<ApiDescriptionAttribute>().FirstOrDefault();
        return methodDescriptionAttribute?.Description;
    }

    public string[] GetApiMethodParamNames(string methodName)
    {
        var methodInfo = targetType.GetMethod(methodName);
        if (methodInfo == null || !methodInfo.GetCustomAttributes().OfType<ApiMethodAttribute>().Any())
            return null;
        return methodInfo.GetParameters()
            .Select(param => param.Name).ToArray();
    }

    public string GetApiMethodParamDescription(string methodName, string parameterName)
    {
        var methodInfo = targetType.GetMethod(methodName);
        if (methodInfo == null || !methodInfo.GetCustomAttributes().OfType<ApiMethodAttribute>().Any())
            return null;
        var matchingParameter = methodInfo.GetParameters().Where(param => param.Name == parameterName);
        if (!matchingParameter.Any())
            return null;
        var apiDescriptionAttribute = matchingParameter.FirstOrDefault()
            .GetCustomAttributes().OfType<ApiDescriptionAttribute>().FirstOrDefault();
        return apiDescriptionAttribute?.Description;
    }

    public ApiParamDescription GetApiMethodParamFullDescription(string methodName, string parameterName)
    {
        var apiParamDescription = new ApiParamDescription { ParamDescription = new CommonDescription(parameterName) };
        var methodInfo = targetType.GetMethod(methodName);
        if (methodInfo == null || !methodInfo.GetCustomAttributes().OfType<ApiMethodAttribute>().Any()) 
            return apiParamDescription;
        var matchingParameter = methodInfo.GetParameters().Where(param => param.Name == parameterName);
        return !matchingParameter
            .Any() ? apiParamDescription : FillFullParamsDescription(matchingParameter, apiParamDescription);
    }

    /// <summary>
    /// ��������� ������ �������� ���������� �� ������ ���������, ����������� � ���������� ������.
    /// </summary>
    /// <param name="parameter">��������� �������� ParameterInfo, �������������� ��������� ������.</param>
    /// <param name="result">������ ApiParamDescription, ������� ����� �������� ��������� ����������.</param>
    /// <returns>����������� ������ ApiParamDescription.</returns>
    private ApiParamDescription FillFullParamsDescription(IEnumerable<ParameterInfo> parameter, 
        ApiParamDescription result)
    {
        var apiDescriptionAttribute = parameter.FirstOrDefault()
            .GetCustomAttributes().OfType<ApiDescriptionAttribute>()
            .FirstOrDefault();
        if (apiDescriptionAttribute != null)
            result.ParamDescription.Description = apiDescriptionAttribute.Description;
        var apiIntValidationAttribute = parameter.FirstOrDefault()
            .GetCustomAttributes().OfType<ApiIntValidationAttribute>()
            .FirstOrDefault();
        if (apiIntValidationAttribute != null)
        {
            result.MinValue = apiIntValidationAttribute.MinValue;
            result.MaxValue = apiIntValidationAttribute.MaxValue;
        }
        var apiRequiredAttribute = parameter.First()
            .GetCustomAttributes().OfType<ApiRequiredAttribute>().FirstOrDefault();
        if (apiRequiredAttribute != null)
            result.Required = apiRequiredAttribute.Required;
        return result;
    }

    public ApiMethodDescription GetApiMethodFullDescription(string methodName)
    {
        var method = targetType.GetMethod(methodName);
        if (method == null || !method.GetCustomAttributes().OfType<ApiMethodAttribute>().Any())
            return null;
        var apiMethodDescription = new ApiMethodDescription
        {
            MethodDescription = new CommonDescription(methodName, GetApiMethodDescription(methodName)),
            ParamDescriptions = GetApiMethodParamNames(methodName)
                .Select(param => GetApiMethodParamFullDescription(methodName, param))
                .ToArray()
        };

        return FillFullMethodDescription(method.ReturnParameter, apiMethodDescription);
    }

    /// <summary>
    /// ��������� ������ �������� ������ �� ������ ���������, ����������� � ������������� ��������� ������.
    /// </summary>
    /// <param name="returnParameter">������ ParameterInfo, �������������� ������������ �������� ������.</param>
    /// <param name="result">������ ApiMethodDescription, ������� ����� �������� ��������� ������.</param>
    /// <returns>����������� ������ ApiMethodDescription.</returns>
    private ApiMethodDescription FillFullMethodDescription(ParameterInfo returnParameter, ApiMethodDescription result)
    {
        var returnedParamDescription = new ApiParamDescription { ParamDescription = new CommonDescription() };
        FillDescriptionAttribute(returnParameter, returnedParamDescription);
        FillValidationAttribute(returnParameter, returnedParamDescription);
        var isReturned = FillRequiredAttribute(returnParameter, returnedParamDescription);
        if (isReturned)
            result.ReturnDescription = returnedParamDescription;
        return result;
    }

    /// <summary>
    /// ��������� �������� ������������� ��������� �� ������ �������� ApiDescriptionAttribute.
    /// </summary>
    /// <param name="returnParameter">������ ParameterInfo, �������������� ������������ �������� ������.</param>
    /// <param name="returnedDescription">������ ApiParamDescription, 
    /// ������� ����� �������� ��������� ���������.</param>
    private void FillDescriptionAttribute
        (ParameterInfo returnParameter, ApiParamDescription returnedDescription)
    {
        var apiDescriptionAttribute = returnParameter
            .GetCustomAttributes().OfType<ApiDescriptionAttribute>().FirstOrDefault();
        if (apiDescriptionAttribute != null)
            returnedDescription.ParamDescription.Description = apiDescriptionAttribute.Description;
    }

    /// <summary>
    /// ��������� �������� ��������� ������������� ��������� �� ������ �������� ApiIntValidationAttribute.
    /// </summary>
    /// <param name="returnParameter">������ ParameterInfo, �������������� ������������ �������� ������.</param>
    /// <param name="returnedDescription">������ ApiParamDescription, 
    /// ������� ����� �������� ��������� ��������� ���������.</param>
    private void FillValidationAttribute
        (ParameterInfo returnParameter, ApiParamDescription returnedDescription)
    {
        var apiIntValidationAttribute = returnParameter.GetCustomAttributes()
            .OfType<ApiIntValidationAttribute>().FirstOrDefault();
        if (apiIntValidationAttribute != null)
        {
            returnedDescription.MinValue = apiIntValidationAttribute.MinValue;
            returnedDescription.MaxValue = apiIntValidationAttribute.MaxValue;
        }
    }

    /// <summary>
    /// ��������� ���������� � ���, �������� �� ������������ �������� ������������, 
    /// �� ������ �������� ApiRequiredAttribute.
    /// </summary>
    /// <param name="returnParameter">������ ParameterInfo, �������������� ������������ �������� ������.</param>
    /// <param name="returnedDescription">������ ApiParamDescription, 
    /// ������� ����� �������� ����������� �� �������������� ���������.</param>
    /// <returns>true, ���� �������� �������� ������������, ����� false.</returns>
    private bool FillRequiredAttribute
        (ParameterInfo returnParameter, ApiParamDescription returnedDescription)
    {
        var apiRequiredAttribute = returnParameter
            .GetCustomAttributes().OfType<ApiRequiredAttribute>().FirstOrDefault();
        if (apiRequiredAttribute != null)
        {
            returnedDescription.Required = apiRequiredAttribute.Required;
            return true;
        }
        return false;
    }
}