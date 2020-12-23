using System.Collections.Generic;

namespace Xbehave.Sdk
{
    /// <summary>
    /// Filters a list of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of items contained in a list.</typeparam>
    public interface IFilter<T>
    {
        /// <summary>
        /// Filters a list of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="source">The list to filter.</param>
        /// <returns>The filtered list.</returns>
        IEnumerable<T> Filter(IEnumerable<T> source);
    }
}
