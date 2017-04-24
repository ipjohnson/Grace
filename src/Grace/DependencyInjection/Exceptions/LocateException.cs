using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Grace.DependencyInjection.Exceptions
{
    /// <summary>
    /// Default locate exception for the container
    /// </summary>
    public class LocateException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">static context required</param>
        /// <param name="message">message, this is not required</param>
        public LocateException(StaticInjectionContext context, string message = null) : base(CreateMessage(context, message))
        {
            Context = context;
        }

        /// <summary>
        /// Constructor that takes an inner exception
        /// </summary>
        /// <param name="context">static context</param>
        /// <param name="innerException">inner exception</param>
        /// <param name="message">message for exception</param>
        public LocateException(StaticInjectionContext context, Exception innerException, string message = null) : base(CreateMessage(context, message), innerException)
        {
            Context = context;
        }

        /// <summary>
        /// Context information associated with the exception
        /// </summary>
        public StaticInjectionContext Context { get; }

        private static string CreateMessage(StaticInjectionContext context, string message = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var infoStack = new List<InjectionTargetInfo>(context.InjectionStack.Reverse());
            var builder = new StringBuilder();

            builder.AppendLine(message ?? $"Could not locate Type {context.ActivationType}");

            if (infoStack.Count > 40)
            {
                for (var i = 0; i < 20; i++)
                {
                    CreateMessageForTargetInfo(builder, infoStack[i], i + 1);
                }

                builder.AppendLine();
                builder.AppendLine($"Dropped {infoStack.Count - 40} entries");
                builder.AppendLine();

                for (var i = infoStack.Count - 20; i < infoStack.Count; i++)
                {
                    CreateMessageForTargetInfo(builder, infoStack[i], i + 1);
                }
            }
            else
            { 
                for (var i = 0; i < infoStack.Count; i++)
                {
                    CreateMessageForTargetInfo(builder, infoStack[i], i + 1);
                }
            }

            return builder.ToString();
        }

        private static void CreateMessageForTargetInfo(StringBuilder builder, InjectionTargetInfo info, int stepIndex)
        {
            builder.AppendFormat("{0} Importing {1} ", stepIndex, info.LocateType);

            var parameter = info.InjectionTarget as ParameterInfo;

            if (parameter != null)
            {
                var method = parameter.Member as MethodInfo;

                if (method != null)
                {
                    builder.AppendFormat(" for method {0} parameter {1}", method.Name, parameter.Name);
                }
                else
                {
                    builder.AppendFormat(" for constructor parameter {0}", parameter.Name);
                }
            }
            else if (info.InjectionTarget is PropertyInfo)
            {
                builder.AppendFormat(" for property {0}", ((PropertyInfo)info.InjectionTarget).Name);
            }

            builder.AppendLine();
        }
    }
}
