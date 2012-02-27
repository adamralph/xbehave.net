using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;

namespace SubSpec
{
    internal abstract class TheoryDataProvider : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            return DataSource().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return DataSource().GetEnumerator();
        }

        protected abstract IEnumerable<object[]> DataSource();
    }

    internal abstract class SingleItemTheoryDataProvider : TheoryDataProvider
    {
        protected override sealed IEnumerable<object[]> DataSource()
        {
            return SingleItemDataSource().Select( x => new[] { x } );
        }

        protected abstract IEnumerable<object> SingleItemDataSource();
    }

    /// <summary>
    /// Use this class in conjunction with ContextFixture() method to return a disposable object containing a list of all 
    /// objects that need to be disposed that were created within your ContextFixture. 
    /// </summary>
    internal class CompositeFixture : List<IDisposable>, IDisposable
    {
        /// <summary>
        /// Disposes all registered items in reversed order than items have been added.
        /// </summary>
        public void Dispose()
        {
            var _exceptions = new List<Exception>();
            foreach (var disposable in Enumerable.Reverse( this ))
                try
                {
                    disposable.Dispose();
                }
                catch (Exception exception)
                {
                    _exceptions.Add( exception );
                }

            // Always throw AggregateException, prevents stack trace loss without us having
            // to resort on reflection hacks to preserve stack traces.
            if (_exceptions.Count > 0)
                throw new AggregateException( _exceptions );
        }

        // Nested as want to use one in TPL when on FX4
        internal class AggregateException : Exception
        {
            public AggregateException( IEnumerable<Exception> exceptions )
            {
                InnerExceptions = new ReadOnlyCollection<Exception>( exceptions.ToList() );
            }

            public ReadOnlyCollection<Exception> InnerExceptions { get; private set; }

            public override string ToString()
            {
                string splitter = Environment.NewLine + "\t";
                return "AggregateException:" + splitter + string.Join( splitter, InnerExceptions.Select( _ => _.ToString() ).ToArray() );
            }
        }
    }
}
