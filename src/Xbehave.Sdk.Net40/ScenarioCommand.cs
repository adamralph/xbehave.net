using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Xunit.Sdk;

namespace Xunit.Extensions
{
    public class ScenarioCommand : TestCommand
    {
        public ScenarioCommand(IMethodInfo testMethod, object[] parameters)
            : this(testMethod, parameters, null) { }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "An immediate NullReferenceException is fine.")]
        public ScenarioCommand(IMethodInfo testMethod, object[] parameters, Type[] genericTypes)
            : base(testMethod, MethodUtility.GetDisplayName(testMethod), MethodUtility.GetTimeoutParameter(testMethod))
        {
            int idx;

            Parameters = parameters ?? new object[0];

            if (genericTypes != null && genericTypes.Length > 0)
            {
                string[] typeNames = new string[genericTypes.Length];
                for (idx = 0; idx < genericTypes.Length; idx++)
                    typeNames[idx] = ConvertToSimpleTypeName(genericTypes[idx]);

                DisplayName = String.Format("{0}<{1}>", DisplayName, string.Join(", ", typeNames));
            }

            ParameterInfo[] parameterInfos = testMethod.MethodInfo.GetParameters();
            string[] displayValues = new string[Math.Max(Parameters.Length, parameterInfos.Length)];

            for (idx = 0; idx < Parameters.Length; idx++)
                displayValues[idx] = ParameterToDisplayValue(GetParameterName(parameterInfos, idx), Parameters[idx]);

            for (; idx < parameterInfos.Length; idx++)  // Fill-in any missing parameters with "???"
                displayValues[idx] = parameterInfos[idx].Name + ": ???";

            DisplayName = String.Format("{0}({1})", DisplayName, string.Join(", ", displayValues));
        }

        public object[] Parameters { get; protected set; }

        static string ConvertToSimpleTypeName(Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            Type[] genericTypes = type.GetGenericArguments();
            string[] simpleNames = new string[genericTypes.Length];

            for (int idx = 0; idx < genericTypes.Length; idx++)
                simpleNames[idx] = ConvertToSimpleTypeName(genericTypes[idx]);

            string baseTypeName = type.Name;
            int backTickIdx = type.Name.IndexOf('`');

            return baseTypeName.Substring(0, backTickIdx) + "<" + String.Join(", ", simpleNames) + ">";
        }

        public override MethodResult Execute(object testClass)
        {
            try
            {
                ParameterInfo[] parameterInfos = testMethod.MethodInfo.GetParameters();
                if (parameterInfos.Length != Parameters.Length)
                    throw new InvalidOperationException(string.Format("Expected {0} parameters, got {1} parameters", parameterInfos.Length, Parameters.Length));

                testMethod.Invoke(testClass, Parameters);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            }

            return new PassedResult(testMethod, DisplayName);
        }

        static string GetParameterName(ParameterInfo[] parameters, int index)
        {
            if (index >= parameters.Length)
                return "???";

            return parameters[index].Name;
        }

        static string ParameterToDisplayValue(object parameterValue)
        {
            if (parameterValue == null)
                return "null";

            if (parameterValue is char)
                return "'" + parameterValue + "'";

            string stringParameter = parameterValue as string;
            if (stringParameter != null)
            {
                if (stringParameter.Length > 50)
                    return "\"" + stringParameter.Substring(0, 50) + "\"...";

                return "\"" + stringParameter + "\"";
            }

            return Convert.ToString(parameterValue, CultureInfo.InvariantCulture);
        }

        static string ParameterToDisplayValue(string parameterName, object parameterValue)
        {
            return parameterName + ": " + ParameterToDisplayValue(parameterValue);
        }
    }
}