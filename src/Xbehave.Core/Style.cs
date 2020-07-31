namespace Xbehave
{
    /// <summary>
    /// A simple class to follow BDD style.
    /// </summary>
    public static class Style
    {
#pragma warning disable CA1062 // Validate arguments of public methods
        /// <summary>
        /// The Given part describes the state of the world before you begin the behavior you're specifying in this scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="prefix">The prefix of the step text.</param>
        /// <param name="postfix">The postfix of the step text.</param>
        /// <returns></returns>
        public static string Given(string text, string prefix = "Given", string postfix = "") => $"{prefix} {text.Trim()} {postfix}".Trim();

        /// <summary>
        /// The When section is that behavior that you're specifying.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="prefix">The prefix of the step text.</param>
        /// <param name="postfix">The postfix of the step text.</param>
        /// <returns></returns>
        public static string When(string text, string prefix = "When", string postfix = "") => $"{prefix} {text.Trim()} {postfix}".Trim();

        /// <summary>
        ///  The Then section describes the changes you expect due to the specified behavior. 
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="prefix">The prefix of the step text.</param>
        /// <param name="postfix">The postfix of the step text.</param>
        /// <returns></returns>
        public static string Then(string text, string prefix = "Then", string postfix = "") => $"{prefix} {text.Trim()} {postfix}".Trim();

        /// <summary>
        /// The And helps to use behaviours together.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="prefix">The prefix of the step text.</param>
        /// <param name="postfix">The postfix of the step text.</param>
        /// <returns></returns>
        public static string And(string text, string prefix = "And", string postfix = "") => $"{prefix} {text.Trim()} {postfix}".Trim();
#pragma warning restore CA1062 // Validate arguments of public methods
    }
}
