using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Exceptions;
using Grace.Logging;

namespace Grace.DependencyInjection.Impl.CompiledExport
{
	/// <summary>
	/// This class is used to create a delegate that creates an export and statisfies all it's imports
	/// </summary>
	public abstract class BaseCompiledExportDelegate
	{
		protected static readonly MethodInfo InjectionContextLocateByTypeMethod;
		protected static readonly MethodInfo InjectionContextLocateByNameMethod;
		protected static readonly MethodInfo ActivateValueProviderMethod;
		protected static readonly MethodInfo LocateByNameMethod;
		protected static readonly MethodInfo LocateByTypeMethod;
		protected static readonly MethodInfo InjectionScopeLocateAllMethod;
		protected static readonly MethodInfo CollectionActivateMethod;
		protected static readonly MethodInfo AddToDisposalScopeMethod;
		protected static readonly MethodInfo AddLocationInformationEntryMethod;
		protected static readonly MethodInfo ExecuteEnrichWithDelegateMethod;
		protected static readonly MethodInfo IncrementResolveDepth;
		protected static readonly MethodInfo DecrementResolveDepth;
		protected static readonly ConstructorInfo DisposalScopeMissingExceptionConstructor;
		protected static readonly ConstructorInfo MissingDependencyExceptionConstructor;
		protected static readonly ConstructorInfo LocationInformationEntryConstructor;
		protected static readonly Attribute[] EmptyAttributesArray = new Attribute[0];

		protected readonly IEnumerable<Attribute> activationTypeAttributes;
		protected readonly CompiledExportDelegateInfo exportDelegateInfo;
		protected readonly bool isRootObject;
		protected readonly ILog log = Logger.GetLogger<BaseCompiledExportDelegate>();
		protected readonly IInjectionScope owningScope;
		protected List<Expression> bodyExpressions;
		protected List<ExportStrategyDependency> dependencies;

		/// <summary>
		/// List of expressions for handling IDisposable
		/// </summary>
		protected List<Expression> disposalExpressions;

		/// <summary>
		/// The IInjectionScope parameter for the strategy
		/// </summary>
		protected ParameterExpression exportStrategyScopeParameter;

		/// <summary>
		/// List of expressions that loacte the import from the injection context before resolving
		/// </summary>
		protected List<Expression> importInjectionContextExpressions;

		/// <summary>
		/// The IInjectionContext parameter
		/// </summary>
		protected ParameterExpression injectionContextParameter;

		/// <summary>
		/// List of expressions used to construct the instance
		/// </summary>
		protected List<Expression> instanceExpressions;

		/// <summary>
		/// Variable that represents the instance being constructed
		/// </summary>
		protected ParameterExpression instanceVariable;

		/// <summary>
		/// List of expressions that test if an import is null
		/// </summary>
		protected List<Expression> isRequiredExpressions;

		protected List<ParameterExpression> localVariables;

		protected List<Expression> nonRootObjectImportExpressions;
		protected List<Expression> objectImportExpression;
		protected List<Expression> rootObjectImportExpressions;

		static BaseCompiledExportDelegate()
		{
			CollectionActivateMethod = typeof(IExportStrategyCollection).GetRuntimeMethod("Activate",
				new[]
				{
					typeof(string),
					typeof(Type),
					typeof(IInjectionContext),
					typeof(ExportStrategyFilter)
				});

			LocateByTypeMethod = typeof(IExportLocator).GetRuntimeMethod("Locate",
				new[]
				{
					typeof(Type),
					typeof(IInjectionContext),
					typeof(ExportStrategyFilter)
				});

			LocateByNameMethod = typeof(IExportLocator).GetRuntimeMethod("Locate",
				new[]
				{
					typeof(string),
					typeof(IInjectionContext),
					typeof(ExportStrategyFilter)
				});

			InjectionScopeLocateAllMethod =
				typeof(IExportLocator).GetRuntimeMethods()
					.First(f => f.Name == "LocateAll" && f.GetParameters().First().ParameterType == typeof(IInjectionContext));

			InjectionContextLocateByTypeMethod = typeof(IInjectionContext).GetRuntimeMethod("Locate", new[] { typeof(Type) });

			InjectionContextLocateByNameMethod = typeof(IInjectionContext).GetRuntimeMethod("Locate", new[] { typeof(string) });

			IncrementResolveDepth = typeof(IInjectionContext).GetRuntimeMethod("IncrementResolveDepth", new Type[0]);

			DecrementResolveDepth = typeof(IInjectionContext).GetRuntimeMethod("DecrementResolveDepth", new Type[0]);

			ActivateValueProviderMethod = typeof(IExportValueProvider).GetRuntimeMethod("Activate",
				new[]
				{
					typeof(IInjectionScope),
					typeof(IInjectionContext),
					typeof(ExportStrategyFilter)
				});

			MissingDependencyExceptionConstructor = typeof(MissingDependencyException).GetTypeInfo().DeclaredConstructors.First();

			DisposalScopeMissingExceptionConstructor =
				typeof(DisposalScopeMissingException).GetTypeInfo().DeclaredConstructors.First();

			LocationInformationEntryConstructor =
				typeof(LocationInformationEntry).GetTypeInfo().DeclaredConstructors.First();

			AddToDisposalScopeMethod = typeof(IDisposalScope).GetRuntimeMethod("AddDisposable",
				new[]
				{
					typeof(IDisposable),
					typeof(BeforeDisposalCleanupDelegate)
				});

			ExecuteEnrichWithDelegateMethod = typeof(BaseCompiledExportDelegate).GetRuntimeMethod("ExecuteEnrichWithDelegate",
				new[]
				{
					typeof(EnrichWithDelegate),
					typeof(IInjectionScope),
					typeof(IInjectionContext),
					typeof(object)
				});

			AddLocationInformationEntryMethod = typeof(LocateException).GetRuntimeMethod("AddLocationInformationEntry",
				new[]
				{
					typeof(LocationInformationEntry)
				});
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportDelegateInfo">information for compiling the delegate</param>
		/// <param name="owningScope">the owning scope</param>
		protected BaseCompiledExportDelegate(CompiledExportDelegateInfo exportDelegateInfo,
			IInjectionScope owningScope = null)
		{
			if (exportDelegateInfo == null)
			{
				throw new ArgumentNullException("exportDelegateInfo");
			}

			if (exportDelegateInfo.Attributes == null)
			{
				throw new ArgumentNullException("exportDelegateInfo", "Attributes can't be null on exportDelegateInfo");
			}

			this.exportDelegateInfo = exportDelegateInfo;

			this.owningScope = owningScope;

			if (owningScope != null)
			{
				isRootObject = owningScope.ParentScope == null;
			}

			Attribute[] classAttributes = exportDelegateInfo.Attributes.ToArray();

			activationTypeAttributes = classAttributes.Length == 0 ? EmptyAttributesArray : classAttributes;

			dependencies = new List<ExportStrategyDependency>();
		}

		/// <summary>
		/// Dependencies for this compiled delegate
		/// </summary>
		public List<ExportStrategyDependency> Dependencies
		{
			get { return dependencies; }
		}

		/// <summary>
		/// Compiles the export delegate
		/// </summary>
		/// <returns></returns>
		public ExportActivationDelegate CompileDelegate()
		{
			Initialize();

			return GenerateDelegate();
		}

		/// <summary>
		/// Initialize the delegate
		/// </summary>
		protected virtual void Initialize()
		{
			localVariables = new List<ParameterExpression>();
			importInjectionContextExpressions = new List<Expression>();
			objectImportExpression = new List<Expression>();
			rootObjectImportExpressions = new List<Expression>();
			nonRootObjectImportExpressions = new List<Expression>();
			isRequiredExpressions = new List<Expression>();
			instanceExpressions = new List<Expression>();
			bodyExpressions = new List<Expression>();
		}

		/// <summary>
		/// This method generates the compiled delegate
		/// </summary>
		/// <returns></returns>
		protected virtual ExportActivationDelegate GenerateDelegate()
		{
			SetUpParameterExpressions();

			SetUpInstanceVariableExpression();

			CreateInstantiationExpression();

			CreateCustomInitializeExpressions();

			CreatePropertyImportExpressions();

			CreateMethodImportExpressions();

			CreateActivationMethodExpressions();

			if (exportDelegateInfo.TrackDisposable)
			{
				CreateDisposableMethodExpression();
			}

			bodyExpressions.Add(Expression.Call(injectionContextParameter, DecrementResolveDepth));

			if (!CreateEnrichmentExpression())
			{
				// only add the return expression if there was no enrichment
				CreateReturnExpression();
			}

			List<Expression> methodExpressions = new List<Expression>();

			if (disposalExpressions != null)
			{
				methodExpressions.AddRange(disposalExpressions);
			}

			methodExpressions.Add(Expression.Call(injectionContextParameter, IncrementResolveDepth));

			methodExpressions.AddRange(GetImportExpressions());
			methodExpressions.AddRange(instanceExpressions);
			methodExpressions.AddRange(bodyExpressions);

			BlockExpression body = Expression.Block(localVariables, methodExpressions);

			return Expression.Lambda<ExportActivationDelegate>(body,
				exportStrategyScopeParameter,
				injectionContextParameter).Compile();
		}

		protected IEnumerable<Expression> GetImportExpressions()
		{
			foreach (Expression importInjectionContextExpression in importInjectionContextExpressions)
			{
				yield return importInjectionContextExpression;
			}

			foreach (Expression expression in objectImportExpression)
			{
				yield return expression;
			}

			if (rootObjectImportExpressions.Count > 0 && nonRootObjectImportExpressions.Count > 0)
			{
				Expression rootScopeBlock = Expression.Block(rootObjectImportExpressions);
				Expression nonRootScopeBlock = Expression.Block(nonRootObjectImportExpressions);

				Expression testExpression =
					Expression.Equal(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
						exportStrategyScopeParameter);

				yield return Expression.IfThenElse(testExpression, rootScopeBlock, nonRootScopeBlock);
			}

			foreach (Expression isRequiredExpression in isRequiredExpressions)
			{
				yield return isRequiredExpression;
			}
		}

		/// <summary>
		/// Sets up the parameters for the delegate
		/// </summary>
		protected virtual void SetUpParameterExpressions()
		{
			exportStrategyScopeParameter = Expression.Parameter(typeof(IInjectionScope), "exportStrategyScope");

			injectionContextParameter =
				Expression.Parameter(typeof(IInjectionContext), "injectionContext");
		}

		/// <summary>
		/// Sets up a local variable that hold the instance that will be injected
		/// </summary>
		protected virtual void SetUpInstanceVariableExpression()
		{
			instanceVariable = Expression.Variable(exportDelegateInfo.ActivationType, "instance");

			localVariables.Add(instanceVariable);
		}

		/// <summary>
		/// This method creates the expression that calls the constructor or function to create a new isntance that will be returned
		/// </summary>
		protected abstract void CreateInstantiationExpression();

		/// <summary>
		/// This method creates expressions that call Initialization methods (i.e. methods that take IInjectionContext and are called after
		/// construction and before properties are injected)
		/// </summary>
		protected virtual void CreateCustomInitializeExpressions()
		{
		}

		/// <summary>
		/// Creates all the import expressions for properties that are being injected
		/// </summary>
		protected virtual void CreatePropertyImportExpressions()
		{
			if (exportDelegateInfo.ImportProperties == null)
			{
				return;
			}

			foreach (ImportPropertyInfo importPropertyInfo in exportDelegateInfo.ImportProperties)
			{
				CreatePropertyImportExpression(importPropertyInfo);
			}
		}

		protected virtual void CreatePropertyImportExpression(ImportPropertyInfo importPropertyInfo)
		{
			Attribute[] attributes = importPropertyInfo.Property.GetCustomAttributes(true).ToArray();

			if (attributes.Length == 0)
			{
				attributes = EmptyAttributesArray;
			}

			InjectionTargetInfo targetInfo =
				new InjectionTargetInfo(exportDelegateInfo.ActivationType,
					activationTypeAttributes,
					importPropertyInfo.Property,
					attributes,
					attributes);

			ParameterExpression importVariable =
				CreateImportExpression(importPropertyInfo.Property.PropertyType,
					targetInfo,
					ExportStrategyDependencyType.Property,
					importPropertyInfo.ImportName,
					importPropertyInfo.Property.Name + "Import",
					importPropertyInfo.IsRequired,
					importPropertyInfo.ValueProvider,
					importPropertyInfo.ExportStrategyFilter,
					importPropertyInfo.ComparerObject);

			Expression assign = Expression.Assign(Expression.Property(instanceVariable, importPropertyInfo.Property),
				Expression.Convert(importVariable, importPropertyInfo.Property.PropertyType));

			bodyExpressions.Add(assign);
		}

		/// <summary>
		/// Creates all method import expressions for the export
		/// </summary>
		protected virtual void CreateMethodImportExpressions()
		{
			if (exportDelegateInfo.ImportMethods == null)
			{
				return;
			}

			foreach (ImportMethodInfo importMethod in exportDelegateInfo.ImportMethods)
			{
				List<Expression> parameters = new List<Expression>();
				Attribute[] methodAttributes = importMethod.MethodToImport.GetCustomAttributes(true).ToArray();

				if (methodAttributes.Length == 0)
				{
					methodAttributes = EmptyAttributesArray;
				}

				foreach (ParameterInfo parameter in importMethod.MethodToImport.GetParameters())
				{
					MethodParamInfo methodParamInfo = null;
					Attribute[] parameterAttributes = parameter.GetCustomAttributes(true).ToArray();

					InjectionTargetInfo injectionTargetInfo = new InjectionTargetInfo
						(exportDelegateInfo.ActivationType,
							activationTypeAttributes,
							parameter,
							parameterAttributes,
							methodAttributes);

					if (importMethod.MethodParamInfos != null)
					{
						foreach (MethodParamInfo paramInfo in importMethod.MethodParamInfos)
						{
							if (paramInfo.ParameterName == parameter.Name)
							{
								methodParamInfo = paramInfo;
								break;
							}
						}

						if (methodParamInfo == null)
						{
							foreach (MethodParamInfo paramInfo in importMethod.MethodParamInfos)
							{
								if (parameter.ParameterType != null &&
									 paramInfo.ParameterType.GetTypeInfo().IsAssignableFrom(parameter.ParameterType.GetTypeInfo()))
								{
									methodParamInfo = paramInfo;
									break;
								}
							}
						}
					}

					if (methodParamInfo != null)
					{
						ParameterExpression importParameter =
							CreateImportExpression(parameter.ParameterType,
								injectionTargetInfo,
								ExportStrategyDependencyType.MethodParameter,
								methodParamInfo.ImportName,
								null,
								methodParamInfo.IsRequired,
								methodParamInfo.ValueProvider,
								methodParamInfo.Filter,
								methodParamInfo.Comparer);

						parameters.Add(Expression.Convert(importParameter, parameter.ParameterType));
					}
					else
					{
						ParameterExpression importParameter =
							CreateImportExpression(parameter.ParameterType,
								injectionTargetInfo,
								ExportStrategyDependencyType.MethodParameter,
								null,
								null,
								true,
								null,
								null,
								null);

						parameters.Add(Expression.Convert(importParameter, parameter.ParameterType));
					}
				}

				Expression callExpression = Expression.Call(instanceVariable, importMethod.MethodToImport, parameters);

				bodyExpressions.Add(callExpression);
			}
		}

		/// <summary>
		/// Create all the expressions for activatition methods
		/// </summary>
		protected virtual void CreateActivationMethodExpressions()
		{
			if (exportDelegateInfo.ActivationMethodInfos == null)
			{
				return;
			}

			foreach (MethodInfo activationMethodInfo in exportDelegateInfo.ActivationMethodInfos)
			{
				ParameterInfo[] infos = activationMethodInfo.GetParameters();

				if (infos.Length == 0)
				{
					bodyExpressions.Add(Expression.Call(instanceVariable, activationMethodInfo));
				}
				else if (infos.Length == 1 && infos[0].ParameterType == typeof(IInjectionContext))
				{
					bodyExpressions.Add(Expression.Call(instanceVariable, activationMethodInfo, injectionContextParameter));
				}
				else
				{
					log.ErrorFormat("{0}.{1} can't be used for activation, must take no parameters or IInjectionContext",
						activationMethodInfo.DeclaringType.FullName,
						activationMethodInfo.Name);
				}
			}
		}

		/// <summary>
		/// Create expressions for disposable objects
		/// </summary>
		protected virtual void CreateDisposableMethodExpression()
		{
			if (!typeof(IDisposable).GetTypeInfo().IsAssignableFrom(exportDelegateInfo.ActivationType.GetTypeInfo()))
			{
				return;
			}

			ParameterExpression disposalScope = Expression.Variable(typeof(IDisposalScope), "disposalScope");

			localVariables.Add(disposalScope);

			Expression assignExpression = Expression.Assign(disposalScope,
				Expression.PropertyOrField(injectionContextParameter, "DisposalScope"));

			Expression ifDisposalScopeNull =
				Expression.IfThen(Expression.Equal(disposalScope, Expression.Constant(null)),
					Expression.Throw(Expression.New(DisposalScopeMissingExceptionConstructor,
						Expression.Constant(exportDelegateInfo.ActivationType))));

			disposalExpressions = new List<Expression> { assignExpression, ifDisposalScopeNull };

			bodyExpressions.Add(Expression.Call(disposalScope,
				AddToDisposalScopeMethod,
				instanceVariable,
				Expression.Convert(Expression.Constant(exportDelegateInfo.CleanupDelegate),
					typeof(BeforeDisposalCleanupDelegate))));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>true if there was an enrichment expression created</returns>
		protected virtual bool CreateEnrichmentExpression()
		{
			if (exportDelegateInfo.EnrichmentDelegates == null)
			{
				return false;
			}

			Expression valueExpression = instanceVariable;

			foreach (EnrichWithDelegate enrichmentDelegate in exportDelegateInfo.EnrichmentDelegates)
			{
				valueExpression = Expression.Call(ExecuteEnrichWithDelegateMethod,
					Expression.Constant(enrichmentDelegate),
					exportStrategyScopeParameter,
					injectionContextParameter,
					valueExpression);
			}

			bodyExpressions.Add(valueExpression);

			return true;
		}

		/// <summary>
		/// This method is used to execute an EnrichWithDelegate
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		/// <param name="scope"></param>
		/// <param name="context"></param>
		/// <param name="injectObject"></param>
		/// <returns></returns>
		public static object ExecuteEnrichWithDelegate(EnrichWithDelegate enrichWithDelegate,
			IInjectionScope scope,
			IInjectionContext context,
			object injectObject)
		{
			return enrichWithDelegate(scope, context, injectObject);
		}

		/// <summary>
		/// Creates a return expression 
		/// </summary>
		protected virtual void CreateReturnExpression()
		{
			if (exportDelegateInfo.ActivationType.IsByRef)
			{
				bodyExpressions.Add(instanceVariable);
			}
			else
			{
				bodyExpressions.Add(Expression.Convert(instanceVariable, typeof(object)));
			}
		}

		/// <summary>
		/// Creates a local variable an then tries to import the value
		/// </summary>
		/// <param name="importType"></param>
		/// <param name="targetInfo"></param>
		/// <param name="dependencyType"></param>
		/// <param name="exportName"></param>
		/// <param name="variableName"></param>
		/// <param name="isRequired"></param>
		/// <param name="valueProvider"></param>
		/// <param name="exportStrategyFilter"></param>
		/// <param name="comparerObject"></param>
		/// <returns></returns>
		protected virtual ParameterExpression CreateImportExpression(Type importType,
			IInjectionTargetInfo targetInfo,
			ExportStrategyDependencyType dependencyType,
			string exportName,
			string variableName,
			bool isRequired,
			IExportValueProvider valueProvider,
			ExportStrategyFilter exportStrategyFilter,
			object comparerObject)
		{
			ParameterExpression importVariable = Expression.Variable(typeof(object), variableName);
			string localExportName = exportName;

			if (string.IsNullOrEmpty(localExportName) && importType != null)
			{
				localExportName = InjectionKernel.ImportTypeByName(importType)
					? targetInfo.InjectionTargetName.ToLowerInvariant()
					: importType.FullName;
			}

			localVariables.Add(importVariable);

			if (exportDelegateInfo.IsTransient)
			{
				Expression assignExpression = null;

				if (importType != null &&
					string.IsNullOrEmpty(exportName) &&
					!InjectionKernel.ImportTypeByName(importType))
				{
					assignExpression =
						Expression.Assign(importVariable,
							Expression.Call(injectionContextParameter,
												 InjectionContextLocateByTypeMethod,
												 Expression.Constant(importType)));
				}
				else
				{
					assignExpression =
						Expression.Assign(importVariable,
							Expression.Call(injectionContextParameter,
												 InjectionContextLocateByNameMethod,
												 Expression.Constant(localExportName)));
				}


				importInjectionContextExpressions.Add(assignExpression);
			}

			string dependencyName = localExportName;

			if (importType != null && dependencyName == importType.FullName)
			{
				dependencyName = null;
			}

			dependencies.Add(new ExportStrategyDependency
								  {
									  DependencyType = dependencyType,
									  HasValueProvider = valueProvider != null,
									  ImportName = dependencyName,
									  ImportType = importType,
									  TargetName = targetInfo.InjectionTargetName
								  });

			if (
				!ProcessSpecialType(importVariable,
					importType,
					targetInfo,
					exportName,
					valueProvider,
					exportStrategyFilter,
					comparerObject))
			{
				if (exportStrategyFilter != null)
				{
					ImportFromRequestingScopeWithFilter(importType, targetInfo, exportName, importVariable, exportStrategyFilter);
				}
				else
				{
					// ImportForRootScope is a shortcut and can only be done for some types
					if (isRootObject &&
						 owningScope != null &&
						 importType != null &&
						 !importType.IsConstructedGenericType &&
						 importType.GetTypeInfo().BaseType != typeof(MulticastDelegate) &&
						 !InjectionKernel.ImportTypeByName(importType))
					{
						ImportForRootScope(importType, targetInfo, exportName, importVariable);
					}
					else
					{
						ImportFromRequestingScope(importType, targetInfo, exportName, importVariable);
					}
				}
			}

			if (isRequired)
			{
				ParameterInfo parameterInfo = targetInfo.InjectionTarget as ParameterInfo;

				Expression exportNameExpression = Expression.Constant(exportName);

				if (exportName == null)
				{
					exportNameExpression = Expression.Convert(exportNameExpression, typeof(string));
				}

				Expression exportTypeExpression = Expression.Constant(importType);

				if (importType == null)
				{
					exportTypeExpression = Expression.Convert(exportTypeExpression, typeof(Type));
				}

				Expression throwException = Expression.Throw(
					Expression.New(MissingDependencyExceptionConstructor,
						exportNameExpression,
						exportTypeExpression,
						injectionContextParameter));

				Expression testExpression = Expression.Equal(importVariable, Expression.Constant(null));

				isRequiredExpressions.Add(Expression.IfThen(testExpression, throwException));
			}

			return importVariable;
		}

		private void ImportForRootScope(Type importType,
			IInjectionTargetInfo targetInfo,
			string exportName,
			ParameterExpression importVariable)
		{
			Expression importTypeExpression = Expression.Constant(importType);
			Expression exportNameExpression = Expression.Constant(exportName);

			if (exportName == null)
			{
				exportNameExpression = Expression.Convert(exportNameExpression, typeof(string));
			}

			IExportStrategyCollection collection =
				owningScope.GetStrategyCollection(importType);

			if (exportDelegateInfo.IsTransient)
			{
				Expression rootIfExpression = Expression.IfThen(
					Expression.Equal(importVariable, Expression.Constant(null)),
					Expression.Assign(importVariable,
						Expression.Call(Expression.Constant(collection),
							CollectionActivateMethod,
							exportNameExpression,
							importTypeExpression,
							injectionContextParameter,
							Expression.Convert(Expression.Constant(null), typeof(ExportStrategyFilter)))));

				Expression requestScopeIfExpression;

				if (string.IsNullOrEmpty(exportName))
				{
					requestScopeIfExpression
						= Expression.IfThen(
							Expression.Equal(importVariable, Expression.Constant(null)),
							Expression.Assign(importVariable,
								Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
									LocateByTypeMethod,
									Expression.Constant(importType),
									injectionContextParameter,
									Expression.Convert(Expression.Constant(null), typeof(ExportStrategyFilter)))));
				}
				else
				{
					Expression assignStatementExpression =
						Expression.Assign(importVariable,
							Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
								LocateByNameMethod,
								injectionContextParameter,
								Expression.Constant(exportName)));

					requestScopeIfExpression
						= Expression.IfThen(
							Expression.Equal(importVariable, Expression.Constant(null)), assignStatementExpression);
				}

				//Expression tryCatchRootExpression = CreateTryCatchUpdateException(exportName, importType, rootIfExpression,targetInfo);
				//Expression tryCatchRequestExpression = CreateTryCatchUpdateException(exportName, importType, requestScopeIfExpression, targetInfo);

				rootObjectImportExpressions.Add(AddInjectionTargetInfo(targetInfo));
				rootObjectImportExpressions.Add(rootIfExpression);

				nonRootObjectImportExpressions.Add(AddInjectionTargetInfo(targetInfo));
				nonRootObjectImportExpressions.Add(requestScopeIfExpression);
			}
			else
			{
				Expression assignRoot =
					Expression.Assign(importVariable,
						Expression.Call(Expression.Constant(collection),
							CollectionActivateMethod,
							exportNameExpression,
							importTypeExpression,
							injectionContextParameter,
							Expression.Convert(Expression.Constant(null), typeof(ExportStrategyFilter))));

				//Expression tryCatchExpression = CreateTryCatchUpdateException(exportName, importType, assignRoot, targetInfo);

				objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
				objectImportExpression.Add(assignRoot);
			}
		}

		private Expression CreateTryCatchUpdateException(string exportName, Type importType, Expression expression, IInjectionTargetInfo targetInfo)
		{
			ParameterExpression exceptionParameter = Expression.Parameter(typeof(LocateException));

			Expression exportNameExpression = Expression.Constant(exportName);

			if (exportName == null)
			{
				exportNameExpression = Expression.Convert(exportNameExpression, typeof(string));
			}

			Expression exportTypeExpression = Expression.Constant(importType);

			if (importType == null)
			{
				exportTypeExpression = Expression.Convert(exportTypeExpression, typeof(Type));
			}

			BlockExpression catchBody = Expression.Block(new[] { exceptionParameter },
																		Expression.Call(exceptionParameter,
																							 AddLocationInformationEntryMethod, 
																							 Expression.New(LocationInformationEntryConstructor,
																												exportNameExpression,
																												exportTypeExpression, 
																												Expression.Constant(targetInfo))),
																		Expression.Rethrow());

			CatchBlock catchBlock = Expression.Catch(exceptionParameter, catchBody);

			return Expression.TryCatch(Expression.Block(typeof(void), expression), catchBlock);
		}

		private void ImportFromRequestingScope(Type importType,
			IInjectionTargetInfo targetInfo,
			string exportName,
			ParameterExpression importVariable)
		{
			// for cases where we are importing a string or a primitive
			// import by name rather than type
			if (string.IsNullOrEmpty(exportName) &&
				 (InjectionKernel.ImportTypeByName(importType)))
			{
				exportName = targetInfo.InjectionTargetName.ToLowerInvariant();
			}

			if (exportDelegateInfo.IsTransient)
			{
				Expression requestScopeIfExpression;

				if (string.IsNullOrEmpty(exportName))
				{
					requestScopeIfExpression
						= Expression.IfThen(
							Expression.Equal(importVariable, Expression.Constant(null)),
							Expression.Assign(importVariable,
								Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
									LocateByTypeMethod,
									Expression.Constant(importType),
									injectionContextParameter,
									Expression.Convert(Expression.Constant(null), typeof(ExportStrategyFilter)))));
				}
				else
				{
					requestScopeIfExpression
						= Expression.IfThen(
							Expression.Equal(importVariable, Expression.Constant(null)),
							Expression.Assign(importVariable,
								Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
									LocateByNameMethod,
									Expression.Constant(exportName),
									injectionContextParameter,
									Expression.Convert(Expression.Constant(null), typeof(ExportStrategyFilter)))));
				}

				objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
				objectImportExpression.Add(requestScopeIfExpression);
			}
			else
			{
				Expression requestScopeIfExpression;

				if (string.IsNullOrEmpty(exportName))
				{
					requestScopeIfExpression
						= Expression.IfThen(
							Expression.Equal(importVariable, Expression.Constant(null)),
							Expression.Assign(importVariable,
								Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
									LocateByTypeMethod,
									Expression.Constant(importType),
									injectionContextParameter,
									Expression.Convert(Expression.Constant(null), typeof(ExportStrategyFilter)))));
				}
				else
				{
					requestScopeIfExpression
						= Expression.IfThen(
							Expression.Equal(importVariable, Expression.Constant(null)),
							Expression.Assign(importVariable,
								Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
									LocateByNameMethod,
									Expression.Constant(exportName),
									injectionContextParameter,
									Expression.Convert(Expression.Constant(null), typeof(ExportStrategyFilter)))));
				}

				objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
				objectImportExpression.Add(requestScopeIfExpression);
			}
		}

		private void ImportFromRequestingScopeWithFilter(Type importType,
			IInjectionTargetInfo targetInfo,
			string exportName,
			ParameterExpression importVariable,
			ExportStrategyFilter exportStrategyFilter)
		{
			if (exportDelegateInfo.IsTransient)
			{
				Expression requestScopeIfExpression;

				if (string.IsNullOrEmpty(exportName))
				{
					requestScopeIfExpression
						= Expression.IfThen(
							Expression.Equal(importVariable, Expression.Constant(null)),
							Expression.Assign(importVariable,
								Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
									LocateByTypeMethod,
									Expression.Constant(importType),
									injectionContextParameter,
									Expression.Constant(exportStrategyFilter))));
				}
				else
				{
					requestScopeIfExpression
						= Expression.IfThen(
							Expression.Equal(importVariable, Expression.Constant(null)),
							Expression.Assign(importVariable,
								Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
									LocateByNameMethod,
									Expression.Constant(exportName),
									injectionContextParameter,
									Expression.Constant(exportStrategyFilter))));
				}

				objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
				objectImportExpression.Add(requestScopeIfExpression);
			}
			else
			{
				Expression requestScopeIfExpression;

				if (string.IsNullOrEmpty(exportName))
				{
					requestScopeIfExpression
						= Expression.IfThen(
							Expression.Equal(importVariable, Expression.Constant(null)),
							Expression.Assign(importVariable,
								Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
									LocateByTypeMethod,
									Expression.Constant(importType),
									injectionContextParameter,
									Expression.Constant(exportStrategyFilter))));
				}
				else
				{
					requestScopeIfExpression
						= Expression.IfThen(
							Expression.Equal(importVariable, Expression.Constant(null)),
							Expression.Assign(importVariable,
								Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
									LocateByNameMethod,
									Expression.Constant(exportName),
									injectionContextParameter,
									Expression.Constant(exportStrategyFilter))));
				}

				objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
				objectImportExpression.Add(requestScopeIfExpression);
			}
		}

		private Expression AddInjectionTargetInfo(IInjectionTargetInfo targetInfo)
		{
			return Expression.Assign(Expression.Property(injectionContextParameter, "TargetInfo"),
				Expression.Constant(targetInfo));
		}

		private bool ProcessSpecialType(ParameterExpression importVariable,
			Type importType,
			IInjectionTargetInfo targetInfo,
			string exportName,
			IExportValueProvider valueProvider,
			ExportStrategyFilter exportStrategyFilter,
			object comparerObject)
		{
			bool returnValue = false;

			if (valueProvider != null)
			{
				Expression callValueProvider =
					Expression.Call(Expression.Constant(valueProvider),
						ActivateValueProviderMethod,
						exportStrategyScopeParameter,
						injectionContextParameter,
						Expression.Convert(Expression.Constant(null), typeof(ExportStrategyFilter)));

				Expression assignExpression =
					Expression.Assign(importVariable, callValueProvider);

				objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
				objectImportExpression.Add(assignExpression);

				returnValue = true;
			}
			else if (importType == typeof(IDisposalScope))
			{
				objectImportExpression.Add(
					Expression.Assign(importVariable, Expression.Property(injectionContextParameter, "DisposalScope")));

				returnValue = true;
			}
			else if (importType == typeof(IInjectionScope) ||
						importType == typeof(IExportLocator))
			{
				if (exportDelegateInfo.IsTransient)
				{
					objectImportExpression.Add(
						Expression.Assign(importVariable, Expression.Property(injectionContextParameter, "RequestingScope")));
				}
				else
				{
					objectImportExpression.Add(
						Expression.Assign(importVariable, exportStrategyScopeParameter));
				}

				returnValue = true;
			}
			else if (importType == typeof(IDependencyInjectionContainer))
			{
				objectImportExpression.Add(
					Expression.Assign(importVariable, Expression.Property(exportStrategyScopeParameter, "Container")));

				returnValue = true;
			}
			else if (importType.IsConstructedGenericType)
			{
				Type genericType = importType.GetGenericTypeDefinition();

				if (genericType == typeof(Func<>))
				{
					MethodInfo createFuncMethod = typeof(BaseCompiledExportDelegate).GetRuntimeMethod("CreateFunc",
						new[]
						{
							typeof(IInjectionContext),
							typeof(ExportStrategyFilter)
						});

					MethodInfo closedMethod = createFuncMethod.MakeGenericMethod(importType.GenericTypeArguments);

					Expression assign = Expression.Assign(importVariable,
						Expression.Call(closedMethod,
							injectionContextParameter,
							Expression.Convert(Expression.Constant(exportStrategyFilter),
								typeof(ExportStrategyFilter))));

					objectImportExpression.Add(assign);

					returnValue = true;
				}
				else if (genericType == typeof(Func<,>) &&
							importType.GenericTypeArguments[0] == typeof(IInjectionContext))
				{
					MethodInfo createFuncMethod = typeof(BaseCompiledExportDelegate).GetRuntimeMethod("CreateFuncWithContext",
						new[]
						{
							typeof(IInjectionScope),
							typeof(ExportStrategyFilter)
						});

					MethodInfo closedMethod = createFuncMethod.MakeGenericMethod(importType.GenericTypeArguments);

					Expression assign = Expression.Assign(importVariable,
						Expression.Call(closedMethod,
							Expression.Property(injectionContextParameter,
								"RequestingScope"),
							Expression.Convert(Expression.Constant(exportStrategyFilter),
								typeof(ExportStrategyFilter))));

					objectImportExpression.Add(assign);

					returnValue = true;
				}
				else if (genericType == typeof(Func<Type, object>))
				{
					MethodInfo funcMethodInfo = typeof(BaseCompiledExportDelegate).GetRuntimeMethod("CreateFuncType",
						new[]
						{
							typeof(IInjectionScope),
							typeof(ExportStrategyFilter)
						});

					Expression assign = Expression.Assign(importVariable,
						Expression.Call(funcMethodInfo,
							Expression.Property(injectionContextParameter,
								"RequestingScope"),
							Expression.Convert(Expression.Constant(exportStrategyFilter),
								typeof(ExportStrategyFilter))));

					objectImportExpression.Add(assign);

					returnValue = true;
				}
				else if (genericType == typeof(Func<Type, IInjectionContext, object>))
				{
					MethodInfo funcMethodInfo = typeof(BaseCompiledExportDelegate).GetRuntimeMethod("CreateFuncTypeWithContext",
						new[]
						{
							typeof(IInjectionScope),
							typeof(ExportStrategyFilter)
						});

					Expression assign = Expression.Assign(importVariable,
						Expression.Call(funcMethodInfo,
							Expression.Property(injectionContextParameter,
								"RequestingScope"),
							Expression.Convert(Expression.Constant(exportStrategyFilter),
								typeof(ExportStrategyFilter))));

					objectImportExpression.Add(assign);

					returnValue = true;
				}
				else if (genericType == typeof(IEnumerable<>) ||
							genericType == typeof(ICollection<>) ||
							genericType == typeof(IList<>) ||
							genericType == typeof(List<>))
				{
					MethodInfo closeMethod = InjectionScopeLocateAllMethod.MakeGenericMethod(importType.GenericTypeArguments);
					Type comparerType = typeof(IComparer<>).MakeGenericType(importType.GenericTypeArguments);

					Expression callExpression =
						Expression.Call(Expression.Property(injectionContextParameter, "RequestingScope"),
							closeMethod,
							injectionContextParameter,
							Expression.Convert(Expression.Constant(exportStrategyFilter), typeof(ExportStrategyFilter)),
							Expression.Convert(Expression.Constant(comparerObject), comparerType));

					objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
					objectImportExpression.Add(Expression.Assign(importVariable, callExpression));

					returnValue = true;
				}
				else if (genericType == typeof(ReadOnlyCollection<>) ||
							genericType == typeof(IReadOnlyCollection<>) ||
							genericType == typeof(IReadOnlyList<>))
				{
					Type closedType = typeof(ReadOnlyCollection<>).MakeGenericType(importType.GenericTypeArguments);
					MethodInfo closeMethod = InjectionScopeLocateAllMethod.MakeGenericMethod(importType.GenericTypeArguments);
					Type comparerType = typeof(IComparer<>).MakeGenericType(importType.GenericTypeArguments);

					Expression callExpression =
						Expression.Call(Expression.Property(injectionContextParameter, "RequestingScope"),
							closeMethod,
							injectionContextParameter,
							Expression.Convert(Expression.Constant(exportStrategyFilter), typeof(ExportStrategyFilter)),
							Expression.Convert(Expression.Constant(comparerObject), comparerType));

					Expression newReadOnlyCollection =
						Expression.New(closedType.GetTypeInfo().DeclaredConstructors.First(), callExpression);

					objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
					objectImportExpression.Add(Expression.Assign(importVariable, newReadOnlyCollection));

					returnValue = true;
				}
				else if (genericType == typeof(Lazy<>))
				{
					MethodInfo methodInfo =
						typeof(BaseCompiledExportDelegate).GetRuntimeMethod("CreateLazy",
							new[]
							{
								typeof(IInjectionContext),
								typeof(ExportStrategyFilter)
							});

					MethodInfo closedMethod = methodInfo.MakeGenericMethod(importType.GenericTypeArguments);

					Expression assign = Expression.Assign(importVariable,
						Expression.Call(closedMethod,
							injectionContextParameter,
							Expression.Convert(Expression.Constant(exportStrategyFilter),
								typeof(ExportStrategyFilter))));

					objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
					objectImportExpression.Add(assign);

					returnValue = true;
				}
				else if (genericType == typeof(Owned<>))
				{
					MethodInfo methodInfo =
						typeof(BaseCompiledExportDelegate).GetRuntimeMethod("CreateOwned",
							new[]
							{
								typeof(IInjectionContext),
								typeof(ExportStrategyFilter)
							});

					MethodInfo closedMethod = methodInfo.MakeGenericMethod(importType.GenericTypeArguments);

					Expression assign = Expression.Assign(
						importVariable,
						Expression.Call(closedMethod,
							injectionContextParameter,
							Expression.Convert(Expression.Constant(exportStrategyFilter), typeof(ExportStrategyFilter))));

					objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
					objectImportExpression.Add(assign);

					returnValue = true;
				}
			}
			else if (importType.IsArray)
			{
				Type closedList = typeof(List<>).MakeGenericType(importType.GetElementType());
				MethodInfo closeMethod = InjectionScopeLocateAllMethod.MakeGenericMethod(importType.GetElementType());
				Type comparerType = typeof(IComparer<>).MakeGenericType(importType.GetElementType());

				Expression callExpression =
					Expression.Call(Expression.Property(injectionContextParameter, "RequestingScope"),
						closeMethod,
						injectionContextParameter,
						Expression.Convert(Expression.Constant(exportStrategyFilter), typeof(ExportStrategyFilter)),
						Expression.Convert(Expression.Constant(comparerObject), comparerType));

				Expression toArray =
					Expression.Call(callExpression, closedList.GetRuntimeMethod("ToArray", new Type[0]));

				objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
				objectImportExpression.Add(Expression.Assign(importVariable, toArray));

				returnValue = true;
			}

			return returnValue;
		}

		/// <summary>
		/// Creates a new Func(T)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context"></param>
		/// <param name="exportStrategyFilter"></param>
		/// <returns></returns>
		public static Func<T> CreateFunc<T>(IInjectionContext context, ExportStrategyFilter exportStrategyFilter)
		{
			IInjectionScope scope = context.RequestingScope;
			IDisposalScope disposal = context.DisposalScope;
			IInjectionTargetInfo targetInfo = context.TargetInfo;

			return () =>
					 {
						 IInjectionContext injectionContext = context.Clone();

						 injectionContext.RequestingScope = scope;
						 injectionContext.DisposalScope = disposal;
						 injectionContext.TargetInfo = targetInfo;

						 return scope.Locate<T>(injectionContext, exportStrategyFilter);
					 };
		}

		/// <summary>
		/// Creates a new Func(IInjectionContext,T)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="injectionScope"></param>
		/// <param name="exportStrategyFilter"></param>
		/// <returns></returns>
		public static Func<IInjectionContext, T> CreateFuncWithContext<T>(IInjectionScope injectionScope,
			ExportStrategyFilter exportStrategyFilter)
		{
			return context => injectionScope.Locate<T>(context, exportStrategyFilter);
		}

		/// <summary>
		/// Creates a new Func(Type,object) for resolving type without having to access container
		/// </summary>
		/// <param name="context">injection context to use</param>
		/// <param name="exportStrategyFilter">export filter to use when locating</param>
		/// <returns>new Func(Type,object)</returns>
		public static Func<Type, object> CreateFuncType(IInjectionContext context,
			ExportStrategyFilter exportStrategyFilter)
		{
			IInjectionScope scope = context.RequestingScope;
			IDisposalScope disposal = context.DisposalScope;
			IInjectionTargetInfo targetInfo = context.TargetInfo;

			return type =>
					 {
						 IInjectionContext newContext = context.Clone();

						 newContext.DisposalScope = disposal;
						 newContext.RequestingScope = scope;
						 newContext.TargetInfo = targetInfo;

						 return scope.Locate(type, newContext, exportStrategyFilter);
					 };
		}

		/// <summary>
		/// Create a new Func(Type, IInjectionContext, object) for resolving type without having to access container
		/// </summary>
		/// <param name="injectionScope"></param>
		/// <param name="exportStrategyFilter"></param>
		/// <returns></returns>
		public static Func<Type, IInjectionContext, object> CreateFuncTypeWithContext(IInjectionScope injectionScope,
			ExportStrategyFilter exportStrategyFilter)
		{
			return (type, context) => injectionScope.Locate(type, context, exportStrategyFilter);
		}

		/// <summary>
		/// Creates a new Owned(T)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="injectionContext"></param>
		/// <param name="exportStrategyFilter"></param>
		/// <returns></returns>
		public static Owned<T> CreateOwned<T>(IInjectionContext injectionContext, ExportStrategyFilter exportStrategyFilter)
			where T : class
		{
			Owned<T> newT = new Owned<T>();

			IDisposalScope disposalScope = injectionContext.DisposalScope;

			injectionContext.DisposalScope = newT;

			T returnT = injectionContext.RequestingScope.Locate<T>(injectionContext, exportStrategyFilter);

			newT.SetValue(returnT);

			injectionContext.DisposalScope = disposalScope;

			return newT;
		}

		/// <summary>
		/// Create a new Lazy(T) 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="injectionContext"></param>
		/// <param name="exportStrategyFilter"></param>
		/// <returns></returns>
		public static Lazy<T> CreateLazy<T>(IInjectionContext injectionContext, ExportStrategyFilter exportStrategyFilter)
		{
			IInjectionScope scope = injectionContext.RequestingScope;
			IDisposalScope disposal = injectionContext.DisposalScope;
			IInjectionTargetInfo targetInfo = injectionContext.TargetInfo;

			return new Lazy<T>(() =>
									 {
										 IInjectionContext clonedContext = injectionContext.Clone();

										 clonedContext.RequestingScope = scope;
										 clonedContext.DisposalScope = disposal;
										 clonedContext.TargetInfo = targetInfo;

										 return scope.Locate<T>(clonedContext, exportStrategyFilter);
									 });
		}
	}
}