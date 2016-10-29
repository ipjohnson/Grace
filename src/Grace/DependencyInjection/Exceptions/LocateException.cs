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
        private StaticInjectionContext _context;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">static context required</param>
        /// <param name="message">message, this is not required</param>
        public LocateException(StaticInjectionContext context, string message = null) : base(CreateMessage(context, message))
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        /// <summary>
        /// Constructor that takes an inner exception
        /// </summary>
        /// <param name="context">static context</param>
        /// <param name="innerException">inner exception</param>
        /// <param name="message">message for exception</param>
        public LocateException(StaticInjectionContext context, Exception innerException, string message = null) : base(CreateMessage(context, message), innerException)
        {
            _context = context;
        }

        private static string CreateMessage(StaticInjectionContext context, string message = null)
        {
            var infoStack = new List<InjectionTargetInfo>(context.InjectionStack.Reverse());
            var builder = new StringBuilder();

            if (message == null)
            {
                builder.AppendLine($"Could not locate Type {context.ActivationType}");
            }
            else
            {
                builder.AppendLine(message);
            }

            for (int i = 0; i < infoStack.Count; i++)
            {
                CreateMessageForTargetInfo(builder, infoStack[i], i + 1);
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
