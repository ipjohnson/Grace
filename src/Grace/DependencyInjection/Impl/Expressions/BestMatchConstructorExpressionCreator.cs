using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;

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
        protected override ConstructorInfo PickConstructor(
            IInjectionScope injectionScope, 
            TypeActivationConfiguration configuration,
            IActivationExpressionRequest request, 
            ConstructorInfo[] constructors)
        {
            var matchInfos = new List<MatchInfo>();

            foreach (var constructor in constructors.OrderByDescending(c => c.GetParameters().Length))
            {
                var matchInfo = new MatchInfo { ConstructorInfo = constructor };

                foreach (var parameter in constructor.GetParameters())
                {
                    if (parameter.IsOptional ||
                        parameter.ParameterType.IsGenericParameter ||
                        CanGetValueFromInfo(configuration, parameter, out var configKey) ||
                        CanGetValueFromKnownValues(request, parameter) ||
                        injectionScope.CanLocate(parameter.ParameterType, null, configKey ?? GetKey(injectionScope, parameter)))
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
                    return constructor;
                }

                matchInfos.Add(matchInfo);
            }

            #if NET6_0_OR_GREATER

            return matchInfos.MaxBy(x => x.Matched - x.Missing).ConstructorInfo;
            
            #else
            
            var comparer = Comparer<int>.Default;
            matchInfos.Sort((x, y) => comparer.Compare(x.Matched - x.Missing, y.Matched - y.Missing));
            return matchInfos.Last().ConstructorInfo;
            
            #endif
        }

        private object GetKey(IInjectionScope scope, ParameterInfo parameter)
        {
            var importInfo = ImportAttributeInfo.For(parameter, parameter.ParameterType, parameter.Name);
            if (importInfo != null)
            {
                return importInfo.ImportKey;
            }
            
            if (scope.ScopeConfiguration.Behaviors.KeyedTypeSelector(parameter.ParameterType))
            {
                return parameter.Name;
            }
                
            return null;
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
