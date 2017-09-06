using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Constructor expression creator that picks it's constructor based on available exports
    /// </summary>
    public class BestMatchConstructorExpressionCreator : ConstructorExpressionCreator
    {
        /// <summary>
        /// This method is called when there are multiple constructors
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="configuration"></param>
        /// <param name="request"></param>
        /// <param name="constructors"></param>
        /// <returns></returns>
        protected override ConstructorInfo PickConstructor(IInjectionScope injectionScope, TypeActivationConfiguration configuration,
            IActivationExpressionRequest request, ConstructorInfo[] constructors)
        {
            ConstructorInfo returnConstructor = null;
            var matchInfos = new List<MatchInfo>();

            foreach (var constructor in constructors)
            {
                var matchInfo = new MatchInfo { ConstructorInfo = constructor };

                foreach (var parameter in constructor.GetParameters())
                {
                    object key = null;

                    if (injectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(parameter.ParameterType))
                    {
                        key = parameter.Name;
                    }

                    if (parameter.IsOptional ||
                        parameter.ParameterType.IsGenericParameter ||
                        CanGetValueFromInfo(configuration, parameter) ||
                        CanGetValueFromKnownValues(request, parameter) ||
                        injectionScope.CanLocate(parameter.ParameterType, null, key))
                    {
                        matchInfo.Matched++;
                    }
                    else
                    {
                        matchInfo.Missing++;
                    }
                }

                if (matchInfo.Missing == 0)
                {
                    returnConstructor = constructor;
                    break;
                }

                matchInfos.Add(matchInfo);
            }

            if (returnConstructor == null)
            {
                var comparer = Comparer<int>.Default;

                matchInfos.Sort((x, y) => comparer.Compare(x.Matched - x.Missing, y.Matched - y.Missing));

                returnConstructor = matchInfos.Last().ConstructorInfo;
            }

            return returnConstructor;
        }

        private bool CanGetValueFromKnownValues(IActivationExpressionRequest request, ParameterInfo parameter)
        {
            return request.KnownValueExpressions.Any(e => e.ActivationType == parameter.ParameterType);
        }
        
        /// <summary>
        /// Class used for keeping track of information about a constructor
        /// </summary>
        protected struct MatchInfo
        {
            /// <summary>
            /// how many parameters dependencies were satisfied
            /// </summary>
            public int Matched;

            /// <summary>
            /// how many parameters dependencies could not be found
            /// </summary>
            public int Missing;

            /// <summary>
            /// Constructor
            /// </summary>
            public ConstructorInfo ConstructorInfo;
        }
    }
}
