using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Sdk;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace SubSpec
{
    /// <remarks>
    /// We declare our own delegate instead of using Func 
    /// because we can't use implicit covariance on the generic type parameter in .net 3.5"/>
    /// </remarks>
    public delegate IDisposable ContextDelegate();

    public class DisposableAction : IDisposable
    {
        public static readonly DisposableAction None = new DisposableAction( () => { } );

        private readonly Action _action;

        public DisposableAction( Action action )
        {
            if (action == null)
                throw new ArgumentNullException( "action" );

            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }

    public interface ISpecificationPrimitive
    {
        /// <summary>
        /// Indicate that execution of this delegate should be canceled after a specified timeout.
        /// </summary>
        /// <param name="timeoutMs">The timeout in milliseconds.</param>
        ISpecificationPrimitive WithTimeout( int timeoutMs );
    }

    internal static class Core
    {
        private class SpecificationPrimitive<T> : ISpecificationPrimitive
        {
            private readonly string _message;
            private readonly T _action;
            private int _timeoutMs;

            /// <summary>
            /// Initializes a new instance of the Context class.
            /// </summary>
            /// <param name="message"></param>
            /// <param name="action"></param>
            public SpecificationPrimitive( string message, T action )
            {
                if (message == null)
                    throw new ArgumentNullException( "message" );
                if (action == null)
                    throw new ArgumentNullException( "action" );

                _message = message;
                _action = action;
            }

            public string Message
            {
                get { return _message; }
            }
            public T ActionDelegate
            {
                get { return _action; }
            }
            public int TimeoutMs
            {
                get { return _timeoutMs; }
            }

            public ISpecificationPrimitive WithTimeout( int timeoutMs )
            {
                _timeoutMs = timeoutMs;
                return this;
            }
        }

        private static class SpecificationPrimitiveExecutor
        {
            public static void Execute( SpecificationPrimitive<Action> primitive )
            {
                Debug.Assert( primitive.ActionDelegate != null );

                if (primitive.TimeoutMs > 0)
                {
#if SILVERLIGHT
                IAsyncResult asyncResult = AsyncDelegatesCompatibility.WorkingBeginInvoke( primitive.ActionDelegate, null, null );
#else
                    IAsyncResult asyncResult = primitive.ActionDelegate.BeginInvoke( null, null );
#endif

                    if (!asyncResult.AsyncWaitHandle.WaitOne( primitive.TimeoutMs ))
                        throw new Xunit.Sdk.TimeoutException( primitive.TimeoutMs );
                    else
                    {
#if SILVERLIGHT
                    primitive.ActionDelegate.WorkingEndInvoke( asyncResult );
#else
                        primitive.ActionDelegate.EndInvoke( asyncResult );
#endif

                    }
                }
                else
                    primitive.ActionDelegate();
            }

            public static IDisposable Execute( SpecificationPrimitive<ContextDelegate> primitive )
            {
                Debug.Assert( primitive.ActionDelegate != null );

                if (primitive.TimeoutMs > 0)
                {
#if SILVERLIGHT
                IAsyncResult asyncResult = AsyncDelegatesCompatibility.WorkingBeginInvoke( primitive.ActionDelegate, null, null );
#else
                    IAsyncResult asyncResult = primitive.ActionDelegate.BeginInvoke( null, null );
#endif

                    if (!asyncResult.AsyncWaitHandle.WaitOne( primitive.TimeoutMs ))
                        throw new Xunit.Sdk.TimeoutException( primitive.TimeoutMs );
                    else
                    {
#if SILVERLIGHT
                    return (IDisposable)primitive.ActionDelegate.WorkingEndInvoke( asyncResult );
#else
                        return primitive.ActionDelegate.EndInvoke( asyncResult );
#endif

                    }
                }
                else
                    return primitive.ActionDelegate();
            }
        }

        private class ActionTestCommand : TestCommand, ITestCommand
        {
            private readonly Action _action;

            public ActionTestCommand( IMethodInfo method, string name, int timeout, Action action )
                : base( method, name, timeout )
            {
                _action = action;
            }

            public override MethodResult Execute( object testClass )
            {
                try
                {
                    _action();
                    return new PassedResult( testMethod, DisplayName );
                }
                catch (Exception ex)
                {
                    return new FailedResult( testMethod, ex, DisplayName );
                }
            }
        }

        private class ExceptionTestCommand : ActionTestCommand
        {
            public ExceptionTestCommand( IMethodInfo method, Xunit.Assert.ThrowsDelegate action )
                : base( method, null, 0, () => action() )
            {
            }

            public override bool ShouldCreateInstance
            {
                get { return false; }
            }
        }

        private class AssertExecutor
        {
            private readonly List<SpecificationPrimitive<Action>> _asserts;
            private readonly SpecificationPrimitive<ContextDelegate> _context;
            private readonly SpecificationPrimitive<Action> _do;

            public AssertExecutor( SpecificationPrimitive<ContextDelegate> context, SpecificationPrimitive<Action> @do, List<SpecificationPrimitive<Action>> asserts )
            {
                _asserts = asserts;
                _context = context;
                _do = @do;
            }

            public IEnumerable<ITestCommand> AssertCommands( string name, IMethodInfo method )
            {
                foreach (var assertion in _asserts)
                {
                    // do not capture the iteration variable because 
                    // all tests would point to the same assertion
                    var capturableAssertion = assertion;
                    Action test =
                        () =>
                        {
                            using (SpecificationPrimitiveExecutor.Execute( _context ))
                            {
                                if (_do != null)
                                    SpecificationPrimitiveExecutor.Execute( _do );

                                SpecificationPrimitiveExecutor.Execute( capturableAssertion );
                            }
                        };

                    string testDescription = String.Format( "{0}, {1}", name, assertion.Message );

                    yield return new ActionTestCommand( method, testDescription, MethodUtility.GetTimeoutParameter( method ), test );
                }
            }
        }

        private class ObservationExecutor
        {
            private readonly IEnumerable<SpecificationPrimitive<Action>> _observations;
            private readonly SpecificationPrimitive<ContextDelegate> _context;
            private readonly SpecificationPrimitive<Action> _do;

            public ObservationExecutor( SpecificationPrimitive<ContextDelegate> context, SpecificationPrimitive<Action> @do, IEnumerable<SpecificationPrimitive<Action>> observations )
            {
                _observations = observations;
                _context = context;
                _do = @do;
            }

            public IEnumerable<ITestCommand> ObservationCommands( string name, IMethodInfo method )
            {
                if (_observations.Count() == 0)
                    yield break;

                bool setupExceptionOccurred = false;
                IDisposable systemUnderTest = default( IDisposable );

                Action setupAction = () =>
                {
                    try
                    {
                        systemUnderTest = SpecificationPrimitiveExecutor.Execute( _context );

                        if (_do != null)
                            SpecificationPrimitiveExecutor.Execute( _do );
                    }
                    catch (Exception)
                    {
                        setupExceptionOccurred = true;
                        throw;
                    }
                };

                yield return new ActionTestCommand( method, "{ " + name, 0, setupAction );

                foreach (var observation in _observations)
                {
                    // do not capture the iteration variable because 
                    // all tests would point to the same observation
                    var capturableObservation = observation;
                    Action perform = () =>
                    {
                        if (setupExceptionOccurred)
                            throw new ContextSetupFailedException( "Setting up Context failed" );

                        SpecificationPrimitiveExecutor.Execute( capturableObservation );
                    };

                    yield return new ActionTestCommand( method, "\t- " + capturableObservation.Message, 0, perform );
                }

                Action tearDownAction = () =>
                {
                    if (systemUnderTest != null)
                        systemUnderTest.Dispose();

                    if (setupExceptionOccurred)
                        throw new ContextSetupFailedException( "Setting up Context failed, but Fixtures were disposed." );
                };

                yield return new ActionTestCommand( method, "} " + name, 0, tearDownAction );
            }
        }

        /// <summary>
        /// An exception that is thrown from Observations or their teardown whenever the corresponding setup failed.
        /// </summary>
        private class ContextSetupFailedException : Exception
        {
            public ContextSetupFailedException( string message ) : base( message ) { }
        }

        internal static class SpecificationContext
        {
            [ThreadStatic]
            private static bool _threadStaticInitialized;
            [ThreadStatic]
            private static SpecificationPrimitive<ContextDelegate> _context;
            [ThreadStatic]
            private static SpecificationPrimitive<Action> _do;
            [ThreadStatic]
            private static List<SpecificationPrimitive<Action>> _asserts;
            [ThreadStatic]
            private static List<SpecificationPrimitive<Action>> _observations;
            [ThreadStatic]
            private static List<SpecificationPrimitive<Action>> _skips;
            [ThreadStatic]
            private static List<Action> _exceptions;

            private static void Reset()
            {
                _exceptions = new List<Action>();
                _context = null;
                _do = null;
                _asserts = new List<SpecificationPrimitive<Action>>();
                _observations = new List<SpecificationPrimitive<Action>>();
                _skips = new List<SpecificationPrimitive<Action>>();
            }

            private static void EnsureThreadStaticInitialized()
            {
                if (_threadStaticInitialized)
                    return;

                Reset();
                _threadStaticInitialized = true;
            }

            public static ISpecificationPrimitive Context( string message, ContextDelegate arrange )
            {
                EnsureThreadStaticInitialized();

                if (_context == null)
                    _context = new SpecificationPrimitive<ContextDelegate>( message, arrange );
                else
                    _exceptions.Add( () => { throw new InvalidOperationException( "Cannot have more than one Context statement in a specification" ); } );

                return _context;
            }

            public static ISpecificationPrimitive Do( string message, Action doAction )
            {
                EnsureThreadStaticInitialized();

                if (_do == null)
                    _do = new SpecificationPrimitive<Action>( message, doAction );
                else
                    _exceptions.Add( () => { throw new InvalidOperationException( "Cannot have more than one Do statement in a specification" ); } );

                return _do;
            }

            public static ISpecificationPrimitive Assert( string message, Action assertAction )
            {
                EnsureThreadStaticInitialized();

                SpecificationPrimitive<Action> assert = new SpecificationPrimitive<Action>( message, assertAction );
                _asserts.Add( assert );

                return assert;
            }

            public static ISpecificationPrimitive Observation( string message, Action observationAction )
            {
                EnsureThreadStaticInitialized();

                var observation = new SpecificationPrimitive<Action>( message, observationAction );
                _observations.Add( observation );

                return observation;
            }

            public static ISpecificationPrimitive Todo( string message, Action skippedAction )
            {
                EnsureThreadStaticInitialized();

                SpecificationPrimitive<Action> skip = new SpecificationPrimitive<Action>( message, skippedAction );
                _skips.Add( skip );

                return skip;
            }

            private static string PrepareSetupDescription()
            {
                string name = _context.Message;

                if (_do != null)
                    name += " " + _do.Message;
                return name;
            }

            public static IEnumerable<ITestCommand> SafelyEnumerateTestCommands( IMethodInfo method, Action<IMethodInfo> registerPrimitives )
            {
                try
                {
                    registerPrimitives( method );

                    IEnumerable<ITestCommand> testCommands = SpecificationContext.BuildCommandsFromRegisteredPrimitives( method );

                    return testCommands;
                }
                catch (Exception ex)
                {
                    return new ITestCommand[] {
                        new SubSpec.Core.ExceptionTestCommand(method, () =>
                          {
                                              		throw new InvalidOperationException(string.Format("An exception was thrown while building tests from Specification {0}.{1}:\r\n" + ex.ToString(), 
                                                                                            method.TypeName, 
                                                                                            method.Name));
                                              })};
                }
            }

            private static IEnumerable<ITestCommand> BuildCommandsFromRegisteredPrimitives( IMethodInfo method )
            {
                EnsureThreadStaticInitialized();

                try
                {
                    var validationException = ValidateSpecification( method );
                    if (validationException != null)
                    {
                        yield return validationException;
                        yield break;
                    }

                    int testsReturned = 0;
                    string name = PrepareSetupDescription();

                    var ax = new AssertExecutor( _context, _do, _asserts );
                    foreach (var item in ax.AssertCommands( name, method ))
                    {
                        yield return item;
                        testsReturned++;
                    }

                    var ox = new ObservationExecutor( _context, _do, _observations );
                    foreach (var item in ox.ObservationCommands( name, method ))
                    {
                        yield return item;
                        testsReturned++;
                    }
                                                                                                                                     
                    foreach (var item in SkipCommands( name, method ))
                    {
                        yield return item;
                        testsReturned++;
                    }

                    if (testsReturned == 0)
                        yield return new ExceptionTestCommand( method, () => { throw new InvalidOperationException( "Must have at least one Assert or Observation in each specification" ); } );
                }
                finally
                {
                    Reset();
                }
            }

            private static ExceptionTestCommand ValidateSpecification( IMethodInfo method )
            {
                if (_context == null)
                    _exceptions.Add( () => { throw new InvalidOperationException( "Must have a Context in each specification" ); } );

                // check if we have any recorded exceptions
                if (_exceptions.Count > 0)
                {
                    // throw the first recorded exception, preserves stacktraces nicely.
                    return new ExceptionTestCommand( method, () => _exceptions[0]() );
                }

                return null;
            }

            private static IEnumerable<ITestCommand> SkipCommands( string name, IMethodInfo method )
            {
                foreach (var kvp in _skips)
                    yield return new SkipCommand( method, name + ", " + kvp.Message, "Action is Todo (instead of Observation or Assert)" );
            }
        }
    }

    [AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = true )]
    public class SpecificationAttribute : FactAttribute
    {
        protected override IEnumerable<ITestCommand> EnumerateTestCommands( IMethodInfo method )
        {
            return SubSpec.Core.SpecificationContext.SafelyEnumerateTestCommands( method, RegisterSpecificationPrimitives);
        }

        public static IEnumerable<ITestCommand> FtoEnumerateTestCommands( IMethodInfo method )
        {
            return SubSpec.Core.SpecificationContext.SafelyEnumerateTestCommands( method, RegisterSpecificationPrimitives );
        }

        private static void RegisterSpecificationPrimitives( IMethodInfo method )
        {
            if (method.IsStatic)
                method.Invoke( null, null );
            else
            {
                ConstructorInfo defaultConstructor = method.MethodInfo.ReflectedType.GetConstructor( Type.EmptyTypes );

                if (defaultConstructor == null)
                    throw new InvalidOperationException( "Specification class does not have a default constructor" );

                object instance = defaultConstructor.Invoke( null );
                method.Invoke( instance, null );
            }
        }
    }

    /// <summary>
    /// Provides extensions for a fluent specification syntax
    /// </summary>
    public static class SpecificationExtensions
    {
        private const string IDisposableHintMessaage = "Your context implements IDisposable. Use ContextFixture() to have its lifecycle managed by SubSpec.";

        /// <summary>
        /// Records a context setup for this specification.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="arrange">The action that will establish the context.</param>
        public static ISpecificationPrimitive Context( this string message, Action arrange )
        {
            return SubSpec.Core.SpecificationContext.Context( message,
                                          () =>
                                          {
                                              arrange();
                                              return DisposableAction.None;
                                          } );
        }

        /// <summary>
        /// Trap for using contexts implementing IDisposable with the wrong overload.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="arrange">The action that will establish and return the context for this test.</param>
        [Obsolete( IDisposableHintMessaage )]
        public static void Context( this string message, ContextDelegate arrange )
        {
            throw new InvalidOperationException( IDisposableHintMessaage );
        }

        /// <summary>
        /// Records a disposable context for this specification. The context lifecycle will be managed by SubSpec.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="arrange">The action that will establish and return the context for this test.</param>
        public static ISpecificationPrimitive ContextFixture( this string message, ContextDelegate arrange )
        {
            return SubSpec.Core.SpecificationContext.Context( message, arrange );
        }

        /// <summary>
        /// Records an action to be performed on the context for this specification.
        /// </summary>
        /// <param name="message">A message describing the action.</param>
        /// <param name="arrange">The action to perform.</param>
        public static ISpecificationPrimitive Do( this string message, Action act )
        {
            return SubSpec.Core.SpecificationContext.Do( message, act );
        }

        /// <summary>
        /// Records an assertion for this specification.
        /// Each assertion is executed on an isolated context.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="assert">The action that will verify the expectation.</param>
        public static ISpecificationPrimitive Assert( this string message, Action assert )
        {
            return SubSpec.Core.SpecificationContext.Assert( message, assert );
        }

        /// <summary>
        /// Records an observation for this specification.
        /// All observations are executed on the same context.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="observation">The action that will verify the expectation.</param>
        public static ISpecificationPrimitive Observation( this string message, Action observation )
        {
            return SubSpec.Core.SpecificationContext.Observation( message, observation );
        }

        /// <summary>
        /// Records a skipped assertion for this specification.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="skippedAction">The action that will verify the expectation.</param>
        public static ISpecificationPrimitive Todo( this string message, Action skippedAction )
        {
            return SubSpec.Core.SpecificationContext.Todo( message, skippedAction );
        }
    }
}