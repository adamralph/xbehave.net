// <copyright file="ScenarioDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;
    using Guard = Xbehave.Sdk.Infrastructure.Guard;

    // TODO: get rid of this?
    public partial class ScenarioDefinition
    {
        private readonly IMethodInfo method;
        private readonly object[] args;
        private readonly ITestCommand[] backgroundCommands;
        private readonly ITestCommand scenarioCommand;
        private readonly object feature;

        public ScenarioDefinition(
            IMethodInfo method, IEnumerable<object> args, IEnumerable<ITestCommand> backgroundCommands, ITestCommand scenarioCommand, object feature)
        {
            Guard.AgainstNullArgument("args", args);
            Guard.AgainstNullArgument("backgroundCommands", backgroundCommands);

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (scenarioCommand == null)
            {
                throw new ArgumentNullException("scenarioCommand");
            }

            this.method = method;
            this.args = args.ToArray();
            this.backgroundCommands = backgroundCommands.ToArray();
            this.scenarioCommand = scenarioCommand;
            this.feature = feature;
        }

        public IMethodInfo Method
        {
            get { return this.method; }
        }

        public IEnumerable<object> Args
        {
            get { return this.args.Select(x => x); }
        }

        public IEnumerable<ITestCommand> BackgroundCommands
        {
            get { return this.backgroundCommands.Select(x => x); }
        }

        public ITestCommand ScenarioCommand
        {
            get { return this.scenarioCommand; }
        }

        public object Feature
        {
            get { return this.feature; }
        }
    }
}