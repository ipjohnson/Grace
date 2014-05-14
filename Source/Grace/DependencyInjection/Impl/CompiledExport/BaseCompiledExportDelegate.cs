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
		protected static readonly MethodInfo ConvertNamedExportToPropertyTypeMethod;
		protected static readonly ConstructorInfo DisposalScopeMissingExceptionConstructor;
		protected static readonly ConstructorInfo MissingDependencyExceptionConstructor;
		protected static readonly ConstructorInfo LocationInformationEntryConstructor;
		protected static readonly ConstructorInfo GeneralLocateExceptionConstructor;
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

			MissingDependencyExceptionConstructor = typeof(MissingDependencyException).GetTypeInfo().DeclaredConstructors.First(x => x.GetParameters().Length == 3);

			DisposalScopeMissingExceptionConstructor =
				typeof(DisposalScopeMissingException).GetTypeInfo().DeclaredConstructors.First();

			LocationInformationEntryConstructor =
				typeof(LocationInformationEntry).GetTypeInfo().DeclaredConstructors.First();

			GeneralLocateExceptionConstructor =
				typeof(GeneralLocateException).GetTypeInfo().DeclaredConstructors.First();

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

			ConvertNamedExportToPropertyTypeMethod =
				typeof(BaseCompiledExportDelegate).GetRuntimeMethod("ConvertNamedExportToPropertyType",
					new[]
					{
						typeof(object), 
						typeof(Type)
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
			objectImportExpression = new List<Expression>();
			rootObjectImportExpressions = new List<Expression>();
			nonRootObjectImportExpressions = new List<Expression>();
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

			CreateCustomEnrichmentExpressions();

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

			InjectionTargetInfo targetInfo = null;

			if (importPropertyInfo.ImportName != null)
			{
				targetInfo = new InjectionTargetInfo(exportDelegateInfo.ActivationType,
																	activationTypeAttributes,
																	importPropertyInfo.Property,
																	attributes,
																	attributes,
																	importPropertyInfo.ImportName,
																	null);
			}
			else if (InjectionKernel.ImportTypeByName(importPropertyInfo.Property.PropertyType))
			{
				targetInfo = new InjectionTargetInfo(exportDelegateInfo.ActivationType,
																	activationTypeAttributes,
																	importPropertyInfo.Property,
																	attributes,
																	attributes,
																	importPropertyInfo.Property.Name,
																	null);
			}
			else
			{
				targetInfo = new InjectionTargetInfo(exportDelegateInfo.ActivationType,
																		activationTypeAttributes,
																		importPropertyInfo.Property,
																		attributes,
																		attributes,
																		null,
																		importPropertyInfo.Property.PropertyType);
			}

			ParameterExpression importVariable = null;

			List<Expression> expressionList = null;

			//// in the case of after construction we want to put the import statements in the body
			//if (importPropertyInfo.AfterConstruction)
			//{
			//	bodyExpressions.Add(
			//		Expression.Assign(Expression.PropertyOrField(injectionContextParameter, "Instance"), instanceVariable));

			//	expressionList = bodyExpressions;
			//}

			importVariable = CreateImportExpression(importPropertyInfo.Property.PropertyType,
																		targetInfo,
																		ExportStrategyDependencyType.Property,
																		importPropertyInfo.ImportName,
																		importPropertyInfo.Property.Name + "Import",
																		importPropertyInfo.IsRequired,
																		importPropertyInfo.ValueProvider,
																		importPropertyInfo.ExportStrategyFilter,
																		importPropertyInfo.ComparerObject,
																		expressionList);


			Expression assign = Expression.Assign(Expression.Property(instanceVariable, importPropertyInfo.Property),
				Expression.Convert(importVariable, importPropertyInfo.Property.PropertyType));

			bodyExpressions.Add(assign);

			if (importPropertyInfo.AfterConstruction)
			{
				bodyExpressions.Add(
					Expression.Assign(Expression.PropertyOrField(injectionContextParameter, "Instance"), Expression.Constant(null)));
			}
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
						InjectionTargetInfo injectionTargetInfo = new InjectionTargetInfo
																						(exportDelegateInfo.ActivationType,
																							activationTypeAttributes,
																							parameter,
																							parameterAttributes,
																							methodAttributes,
																							methodParamInfo.ImportName,
																							parameter.ParameterType);

						ParameterExpression importParameter =
							CreateImportExpression(parameter.ParameterType,
															injectionTargetInfo,
															ExportStrategyDependencyType.MethodParameter,
															methodParamInfo.ImportName,
															null,
															methodParamInfo.IsRequired,
															methodParamInfo.ValueProvider,
															methodParamInfo.Filter,
															methodParamInfo.Comparer,
															null);

						parameters.Add(Expression.Convert(importParameter, parameter.ParameterType));
					}
					else
					{
						InjectionTargetInfo injectionTargetInfo = new InjectionTargetInfo
																						(exportDelegateInfo.ActivationType,
																							activationTypeAttributes,
																							parameter,
																							parameterAttributes,
																							methodAttributes,
																							null,
																							parameter.ParameterType);

						ParameterExpression importParameter =
							CreateImportExpression(parameter.ParameterType,
															injectionTargetInfo,
															ExportStrategyDependencyType.MethodParameter,
															null,
															null,
															true,
															null,
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
						Expression.Constant(exportDelegateInfo.ActivationType), injectionContextParameter)));

			disposalExpressions = new List<Expression> { assignExpression, ifDisposalScopeNull };

			bodyExpressions.Add(Expression.Call(disposalScope,
				AddToDisposalScopeMethod,
				instanceVariable,
				Expression.Convert(Expression.Constant(exportDelegateInfo.CleanupDelegate),
					typeof(BeforeDisposalCleanupDelegate))));
		}


		/// <summary>
		/// Create all custom enrichment expressions
		/// </summary>
		protected virtual void CreateCustomEnrichmentExpressions()
		{
			if (exportDelegateInfo.EnrichmentExpressionProviders == null)
			{
				return;
			}

			CustomEnrichmentLinqExpressionContext context =
				new CustomEnrichmentLinqExpressionContext(exportDelegateInfo.ActivationType,
																		exportStrategyScopeParameter,
																		injectionContextParameter,
																		instanceVariable,
																		localVariables);

			foreach (ICustomEnrichmentLinqExpressionProvider customEnrichmentLinqExpressionProvider in exportDelegateInfo.EnrichmentExpressionProviders)
			{
				bodyExpressions.AddRange(customEnrichmentLinqExpressionProvider.ProvideExpressions(context));
			}
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
		/// <param name="expressionList"></param>
		/// <returns></returns>
		protected virtual ParameterExpression CreateImportExpression(Type importType,
			IInjectionTargetInfo targetInfo,
			ExportStrategyDependencyType dependencyType,
			string exportName,
			string variableName,
			bool isRequired,
			IExportValueProvider valueProvider,
			ExportStrategyFilter exportStrategyFilter,
			object comparerObject,
			List<Expression> expressionList)
		{
			bool canShortCut = true;
			ParameterExpression importVariable = Expression.Variable(typeof(object), variableName);
			string localExportName = exportName;

			if (string.IsNullOrEmpty(localExportName) && importType != null)
			{
				localExportName = InjectionKernel.ImportTypeByName(importType)
					? targetInfo.InjectionTargetName.ToLowerInvariant()
					: importType.FullName;
			}

			if (expressionList == null)
			{
				expressionList = objectImportExpression;
			}
			else
			{
				canShortCut = false;
			}

			localVariables.Add(importVariable);

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

			if (!ProcessSpecialType(importVariable,
											importType,
											targetInfo,
											exportName,
											valueProvider,
											exportStrategyFilter,
											comparerObject,
											expressionList))
			{
				if (exportStrategyFilter != null)
				{
					ImportFromRequestingScopeWithFilter(importType, targetInfo, exportName, importVariable, exportStrategyFilter, isRequired, expressionList);
				}
				else
				{
					// ImportForRootScope is a shortcut and can only be done for some types
					if (canShortCut &&
						 isRootObject &&
						 owningScope != null &&
						 importType != null &&
						 !importType.IsConstructedGenericType &&
						 importType.GetTypeInfo().BaseType != typeof(MulticastDelegate) &&
						 !InjectionKernel.ImportTypeByName(importType))
					{
						ImportForRootScope(importType, targetInfo, exportName, importVariable, isRequired);
					}
					else
					{
						ImportFromRequestingScope(importType, targetInfo, exportName, importVariable, isRequired, expressionList);
					}
				}
			}

			return importVariable;
		}

		private ConditionalExpression CreateRequiredStatement(Type importType,
			string exportName,
			ParameterExpression importVariable)
		{
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

			var requiredStatement = Expression.IfThen(testExpression, throwException);
			return requiredStatement;
		}

		private Expression CreateInjectionContextLocateStatement(Type importType,
			IInjectionTargetInfo targetInfo,
			string exportName,
			ParameterExpression importVariable)
		{
			Expression assignExpression;

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
						Expression.Call(ConvertNamedExportToPropertyTypeMethod,
							Expression.Call(injectionContextParameter,
								InjectionContextLocateByNameMethod,
								Expression.Constant(exportName.ToLowerInvariant())),
							Expression.Constant(targetInfo.InjectionTargetType)));
			}

			return assignExpression;
		}

		private void ImportForRootScope(Type importType,
			IInjectionTargetInfo targetInfo,
			string exportName,
			ParameterExpression importVariable,
			bool isRequired)
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

				Expression rootContextLocateExpression = CreateInjectionContextLocateStatement(importType,
																														targetInfo,
																														exportName,
																														importVariable);

				Expression requestContextLocateExpression = CreateInjectionContextLocateStatement(importType,
																														targetInfo,
																														exportName,
																														importVariable);


				Expression tryCatchRootExpression = CreateTryCatchUpdateException(exportName, importType, targetInfo, rootContextLocateExpression, rootIfExpression);
				Expression tryCatchRequestExpression = CreateTryCatchUpdateException(exportName, importType, targetInfo, requestContextLocateExpression, requestScopeIfExpression);

				rootObjectImportExpressions.Add(AddInjectionTargetInfo(targetInfo));
				rootObjectImportExpressions.Add(tryCatchRootExpression);

				nonRootObjectImportExpressions.Add(AddInjectionTargetInfo(targetInfo));
				nonRootObjectImportExpressions.Add(tryCatchRequestExpression);

				if (isRequired)
				{
					rootObjectImportExpressions.Add(CreateRequiredStatement(importType, exportName, importVariable));
					nonRootObjectImportExpressions.Add(CreateRequiredStatement(importType, exportName, importVariable));
				}
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

				Expression contextLocateExpression = CreateInjectionContextLocateStatement(importType,
																										targetInfo,
																										exportName,
																										importVariable);

				Expression tryCatchExpression = CreateTryCatchUpdateException(exportName, importType, targetInfo, contextLocateExpression, assignRoot);

				objectImportExpression.Add(AddInjectionTargetInfo(targetInfo));
				objectImportExpression.Add(tryCatchExpression);

				if (isRequired)
				{
					objectImportExpression.Add(CreateRequiredStatement(importType, exportName, importVariable));
				}
			}
		}

		private Expression CreateTryCatchUpdateException(string exportName, Type importType, IInjectionTargetInfo targetInfo, params Expression[] expressions)
		{
			CatchBlock catchBlock = CreateLocateExceptionCatchBlock(exportName, importType, targetInfo);

			CatchBlock generalCatchBlock = CreateGeneralExceptionCatchBlock(exportName, importType, targetInfo);

			return Expression.TryCatch(Expression.Block(typeof(void), expressions), catchBlock, generalCatchBlock);
		}

		private CatchBlock CreateLocateExceptionCatchBlock(string exportName,
			Type importType,
			IInjectionTargetInfo targetInfo)
		{
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

			ParameterExpression exceptionParameter = Expression.Parameter(typeof(LocateException));

			BlockExpression catchBody = Expression.Block(Expression.Call(exceptionParameter,
																			AddLocationInformationEntryMethod,
																			Expression.New(LocationInformationEntryConstructor,
																				exportNameExpression,
																				exportTypeExpression,
																				Expression.Constant(targetInfo))),
																		Expression.Rethrow());

			return Expression.Catch(exceptionParameter, catchBody); ;
		}

		private CatchBlock CreateGeneralExceptionCatchBlock(string exportName,
																					  Type importType,
																					  IInjectionTargetInfo targetInfo)
		{
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

			ParameterExpression exceptionParameter = Expression.Parameter(typeof(Exception));

			ParameterExpression generalException = Expression.Variable(typeof(GeneralLocateException));

			Expression newExpression = Expression.New(GeneralLocateExceptionConstructor,
																	exportNameExpression,
																	exportTypeExpression,
																	injectionContextParameter,
																	exceptionParameter);

			Expression addExpression = Expression.Call(generalException,
																	 AddLocationInformationEntryMethod,
																	 Expression.New(LocationInformationEntryConstructor,
																						 exportNameExpression,
																						 exportTypeExpression,
																						 Expression.Constant(targetInfo)));

			BlockExpression catchBody = Expression.Block(new[] { exceptionParameter, generalException },
																		Expression.Assign(generalException, newExpression),
																		addExpression,
																		Expression.Throw(generalException));

			return Expression.Catch(exceptionParameter, catchBody);
		}

		private void ImportFromRequestingScope(Type importType,
			IInjectionTargetInfo targetInfo,
			string exportName,
			ParameterExpression importVariable,
			bool isRequired,
			List<Expression> expressionList)
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
								Expression.Call(ConvertNamedExportToPropertyTypeMethod,
									Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
										LocateByNameMethod,
										Expression.Constant(exportName),
										injectionContextParameter,
										Expression.Convert(Expression.Constant(null), typeof(ExportStrategyFilter))),
									Expression.Constant(targetInfo.InjectionTargetType))));
				}

				Expression contextLocateExpression = CreateInjectionContextLocateStatement(importType,
																										targetInfo,
																										exportName,
																										importVariable);

				Expression tryCatchExpression = CreateTryCatchUpdateException(exportName, importType, targetInfo, contextLocateExpression, requestScopeIfExpression);

				expressionList.Add(AddInjectionTargetInfo(targetInfo));
				expressionList.Add(tryCatchExpression);

				if (isRequired)
				{
					expressionList.Add(CreateRequiredStatement(importType, exportName, importVariable));
				}
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
								Expression.Call(ConvertNamedExportToPropertyTypeMethod,
									Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
										LocateByNameMethod,
										Expression.Constant(exportName),
										injectionContextParameter,
										Expression.Convert(Expression.Constant(null), typeof(ExportStrategyFilter))),
									Expression.Constant(targetInfo.InjectionTargetType))));
				}

				Expression contextLocateExpression = CreateInjectionContextLocateStatement(importType,
																										targetInfo,
																										exportName,
																										importVariable);

				Expression tryCatchExpression = CreateTryCatchUpdateException(exportName, importType, targetInfo, contextLocateExpression, requestScopeIfExpression);

				expressionList.Add(AddInjectionTargetInfo(targetInfo));
				expressionList.Add(tryCatchExpression);

				if (isRequired)
				{
					expressionList.Add(CreateRequiredStatement(importType, exportName, importVariable));
				}
			}
		}

		private void ImportFromRequestingScopeWithFilter(Type importType,
			IInjectionTargetInfo targetInfo,
			string exportName,
			ParameterExpression importVariable,
			ExportStrategyFilter exportStrategyFilter,
			bool isRequired,
			List<Expression> expressionList)
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
								Expression.Call(ConvertNamedExportToPropertyTypeMethod,
									Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
										LocateByNameMethod,
										Expression.Constant(exportName),
										injectionContextParameter,
										Expression.Constant(exportStrategyFilter)),
									Expression.Constant(targetInfo.InjectionTargetType))));
				}

				Expression contextLocateExpression = CreateInjectionContextLocateStatement(importType,
																										targetInfo,
																										exportName,
																										importVariable);

				Expression tryCatchExpression = CreateTryCatchUpdateException(exportName, importType, targetInfo, contextLocateExpression, requestScopeIfExpression);

				expressionList.Add(AddInjectionTargetInfo(targetInfo));
				expressionList.Add(tryCatchExpression);

				if (isRequired)
				{
					expressionList.Add(CreateRequiredStatement(importType, exportName, importVariable));
				}
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
								Expression.Call(ConvertNamedExportToPropertyTypeMethod,
									Expression.Call(Expression.PropertyOrField(injectionContextParameter, "RequestingScope"),
										LocateByNameMethod,
										Expression.Constant(exportName),
										injectionContextParameter,
										Expression.Constant(exportStrategyFilter)),
									Expression.Constant(targetInfo.InjectionTargetType))));
				}

				Expression contextLocateExpression = CreateInjectionContextLocateStatement(importType,
																										targetInfo,
																										exportName,
																										importVariable);

				Expression tryCatchExpression = CreateTryCatchUpdateException(exportName, importType, targetInfo, contextLocateExpression, requestScopeIfExpression);

				expressionList.Add(AddInjectionTargetInfo(targetInfo));
				expressionList.Add(tryCatchExpression);

				if (isRequired)
				{
					expressionList.Add(CreateRequiredStatement(importType, exportName, importVariable));
				}
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
			object comparerObject,
			List<Expression> expressionList)
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

				expressionList.Add(AddInjectionTargetInfo(targetInfo));
				expressionList.Add(assignExpression);

				returnValue = true;
			}
			else if (importType == typeof(IDisposalScope))
			{
				expressionList.Add(
					Expression.Assign(importVariable, Expression.Property(injectionContextParameter, "DisposalScope")));

				returnValue = true;
			}
			else if (importType == typeof(IInjectionContext))
			{
				expressionList.Add(
					Expression.Assign(importVariable, injectionContextParameter));

				returnValue = true;
			}
			else if (importType == typeof(IInjectionScope) ||
						importType == typeof(IExportLocator))
			{
				expressionList.Add(
					exportDelegateInfo.IsTransient
						? Expression.Assign(importVariable, Expression.Property(injectionContextParameter, "RequestingScope"))
						: Expression.Assign(importVariable, exportStrategyScopeParameter));

				returnValue = true;
			}
			else if (importType == typeof(IDependencyInjectionContainer))
			{
				expressionList.Add(
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

					expressionList.Add(assign);

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

					expressionList.Add(assign);

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

					expressionList.Add(assign);

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

					expressionList.Add(assign);

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

					expressionList.Add(AddInjectionTargetInfo(targetInfo));
					expressionList.Add(Expression.Assign(importVariable, callExpression));

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

					expressionList.Add(AddInjectionTargetInfo(targetInfo));
					expressionList.Add(Expression.Assign(importVariable, newReadOnlyCollection));

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

					expressionList.Add(AddInjectionTargetInfo(targetInfo));
					expressionList.Add(assign);

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

					expressionList.Add(AddInjectionTargetInfo(targetInfo));
					expressionList.Add(assign);

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

				expressionList.Add(AddInjectionTargetInfo(targetInfo));
				expressionList.Add(Expression.Assign(importVariable, toArray));

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

		/// <summary>
		/// Converts a object value to a new type
		/// </summary>
		/// <param name="targetValue">value to convert</param>
		/// <param name="targetType">type to convert to</param>
		/// <returns>return value</returns>
		public static object ConvertNamedExportToPropertyType(object targetValue, Type targetType)
		{
			if (targetValue != null)
			{
				if (targetValue.GetType() != targetType)
				{
					return Convert.ChangeType(targetValue, targetType);
				}
			}

			return targetValue;
		}
	}
}