// <copyright file="MethodCall.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    public class MethodCall
    {
        private readonly IMethodInfo method;
        private readonly Argument[] arguments;
        private readonly Type[] typeArguments;

        public MethodCall(IMethodInfo method, IEnumerable<Argument> arguments, IEnumerable<Type> typeArguments)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            this.method = method;

            if (arguments != null)
            {
                this.arguments = arguments.ToArray();
                if (this.arguments.Any(argument => argument == null))
                {
                    throw new ArgumentException("The arguments contain at least one null value.", "arguments");
                }
            }
            else
            {
                this.arguments = new Argument[0];
            }

            if (typeArguments != null)
            {
                this.typeArguments = typeArguments.ToArray();
                if (this.typeArguments.Any(typeArgument => typeArgument == null))
                {
                    throw new ArgumentException("The type arguments contain at least one null value.", "typeArguments");
                }
            }
            else
            {
                this.typeArguments = new Type[0];
            }
        }

        public IMethodInfo Method
        {
            get { return this.method; }
        }

        public IEnumerable<Argument> Arguments
        {
            get { return this.arguments.Select(argument => argument); }
        }

        public IEnumerable<Type> TypeArguments
        {
            get { return this.typeArguments.Select(typeArgument => typeArgument); }
        }
    }
}
