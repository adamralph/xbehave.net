// <copyright file="ScenarioOutlineRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Execution.Shims;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioOutlineRunner : XunitTestCaseRunner
    {
        private static readonly object[] noArguments = new object[0];
        private static readonly ITypeInfo objectTypeInfo = Reflector.Wrap(typeof(object));

        public ScenarioOutlineRunner(
            IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                displayName,
                skipReason,
                constructorArguments,
                noArguments,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            var scenarioRunners = new List<ScenarioRunner>();
            var disposables = new List<IDisposable>();

            var scenarioNumber = 1;
            try
            {
                var dataAttributes = TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute)).ToList();
                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                    var discovererArguments = discovererAttribute.GetConstructorArguments().Cast<string>().ToList();
                    var discovererType = Reflector.GetType(discovererArguments[1], discovererArguments[0]);
                    var discoverer = ExtensibilityPointFactory.GetDataDiscoverer(discovererType);

                    foreach (var dataRow in discoverer.GetData(dataAttribute, TestCase.TestMethod.Method))
                    {
                        scenarioRunners.Add(this.CreateRunner(disposables, dataRow, scenarioNumber++));
                    }
                }
            }
            catch (Exception ex)
            {
                var test = new XunitTest(TestCase, DisplayName);

                if (!MessageBus.QueueMessage(new TestStarting(test)))
                {
                    CancellationTokenSource.Cancel();
                }
                else
                {
                    if (!MessageBus.QueueMessage(new TestFailed(test, 0, null, ex.Unwrap())))
                    {
                        CancellationTokenSource.Cancel();
                    }
                }

                if (!MessageBus.QueueMessage(new TestFinished(test, 0, null)))
                {
                    CancellationTokenSource.Cancel();
                }

                return new RunSummary { Total = 1, Failed = 1 };
            }

            if (!scenarioRunners.Any())
            {
                scenarioRunners.Add(this.CreateRunner(disposables, new object[0], 1));
            }

            var summary = new RunSummary();
            foreach (var scenarioRunner in scenarioRunners)
            {
                summary.Aggregate(await scenarioRunner.RunAsync());
            }

            var timer = new ExecutionTimer();
            var aggregator = new ExceptionAggregator();

            foreach (var disposable in disposables)
            {
                timer.Aggregate(() => aggregator.Run(() => disposable.Dispose()));
            }

            summary.Time += timer.Total;
            return summary;
        }

        private static IEnumerable<ITypeInfo> ResolveTypeArguments(IMethodInfo method, IList<object> argumentValues)
        {
            var parameters = method.GetParameters().ToArray();
            return method.GetGenericArguments()
                .Select(typeParameter => ResolveTypeArgument(typeParameter, parameters, argumentValues));
        }

        private static ITypeInfo ResolveTypeArgument(
            ITypeInfo typeParameter, IList<IParameterInfo> parameters, IList<object> argumentValues)
        {
            var sawNullValue = false;
            ITypeInfo type = null;
            for (var index = 0; index < Math.Min(parameters.Count, argumentValues.Count); ++index)
            {
                var parameterType = parameters[index].ParameterType;
                if (parameterType.IsGenericParameter && parameterType.Name == typeParameter.Name)
                {
                    var argumentValue = argumentValues[index];
                    if (argumentValue == null)
                    {
                        sawNullValue = true;
                    }
                    else if (type == null)
                    {
                        type = Reflector.Wrap(argumentValue.GetType());
                    }
                    else if (type.Name != argumentValue.GetType().FullName)
                    {
                        return objectTypeInfo;
                    }
                }
            }

            if (type == null)
            {
                return objectTypeInfo;
            }

            return sawNullValue && type.IsValueType ? objectTypeInfo : type;
        }

        private static string GetDisplayName(
            IMethodInfo method, string baseDisplayName, Argument[] arguments, ITypeInfo[] typeArguments)
        {
            if (typeArguments.Length > 0)
            {
                baseDisplayName = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}<{1}>",
                    baseDisplayName,
                    string.Join(", ", typeArguments.Select(typeArgument => typeArgument.ToSimpleString())));
            }

            var parameterTokens = new List<string>();
            var parameters = method.GetParameters().ToArray();
            int parameterIndex;
            for (parameterIndex = 0; parameterIndex < arguments.Length; parameterIndex++)
            {
                if (arguments[parameterIndex].IsGeneratedDefault)
                {
                    continue;
                }

                parameterTokens.Add(string.Concat(
                    parameterIndex >= parameters.Length ? "???" : parameters[parameterIndex].Name,
                    ": ",
                    arguments[parameterIndex].ToString()));
            }

            for (; parameterIndex < parameters.Length; parameterIndex++)
            {
                parameterTokens.Add(parameters[parameterIndex].Name + ": ???");
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}({1})", baseDisplayName, string.Join(", ", parameterTokens));
        }

        private ScenarioRunner CreateRunner(List<IDisposable> disposables, object[] argumentValues, int scenarioNumber)
        {
            disposables.AddRange(argumentValues.OfType<IDisposable>());

            var typeArguments = new ITypeInfo[0];
            var closedMethod = TestMethod;
            if (closedMethod.IsGenericMethodDefinition)
            {
                typeArguments = ResolveTypeArguments(TestCase.TestMethod.Method, argumentValues.ToArray()).ToArray();

                closedMethod =
                    closedMethod.MakeGenericMethod(typeArguments.Select(t => ((IReflectionTypeInfo)t).Type).ToArray());
            }

            var parameterTypes = closedMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var convertedArgumentValues = Reflector.ConvertArguments(argumentValues, parameterTypes);

            var parameters = TestCase.TestMethod.Method.GetParameters().ToArray();
            var generatedArguments = new List<Argument>();
            for (var missingArgumentIndex = argumentValues.Length; missingArgumentIndex < parameters.Length; ++missingArgumentIndex)
            {
                var parameterType = parameters[missingArgumentIndex].ParameterType;
                if (parameterType.IsGenericParameter)
                {
                    ITypeInfo concreteType = null;
                    var typeParameters = TestCase.TestMethod.Method.GetGenericArguments().ToArray();
                    for (var typeParameterIndex = 0; typeParameterIndex < typeParameters.Length; ++typeParameterIndex)
                    {
                        var typeParameter = typeParameters[typeParameterIndex];
                        if (typeParameter.Name == parameterType.Name)
                        {
                            concreteType = typeArguments[typeParameterIndex];
                            break;
                        }
                    }

                    if (concreteType == null)
                    {
                        var message = string.Format(
                            CultureInfo.CurrentCulture, "The type of parameter \"{0}\" cannot be resolved.", parameters[missingArgumentIndex].Name);
                        throw new InvalidOperationException(message);
                    }

                    parameterType = concreteType;
                }

                generatedArguments.Add(new Argument(((IReflectionTypeInfo)parameterType).Type));
            }

            var arguments = convertedArgumentValues
                .Select(value => new Argument(value))
                .Concat(generatedArguments)
                .ToArray();

            var displayName = GetDisplayName(TestCase.TestMethod.Method, this.DisplayName, arguments, typeArguments);

            return new ScenarioRunner(
                closedMethod,
                TestCase,
                displayName,
                scenarioNumber,
                SkipReason,
                ConstructorArguments,
                arguments.Select(argument => argument.Value).ToArray(),
                MessageBus,
                Aggregator,
                CancellationTokenSource);
        }
    }
}
