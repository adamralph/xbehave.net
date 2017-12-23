// <copyright file="GetStepDisplayText.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    /// <summary>
    /// Gets the display text for a step.
    /// </summary>
    /// <param name="stepText">The step text.</param>
    /// <param name="isBackgroundStep">A value indidcating whether the step is a background step.</param>
    /// <returns>A string representing the display text for the step.</returns>
    public delegate string GetStepDisplayText(string stepText, bool isBackgroundStep);
}
