using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	/// <summary>
	/// Non Generic rule execution context
	/// </summary>
	public interface IRuleExecutionContext
	{
		/// <summary>
		/// Validation state for rule execution
		/// </summary>
		ValidationState State { get; set; }

		/// <summary>
		/// Validation message
		/// </summary>
		string Message { get; set; }

		/// <summary>
		/// Object being validated
		/// </summary>
		object ValidationObject { get; }

		/// <summary>
		/// Add a validation part to the rule
		/// </summary>
		/// <param name="part">validation part</param>
		void AddValidationPart(IValidationRuleResultPart part);

		/// <summary>
		/// Adds required message
		/// </summary>
		/// <param name="partName">part that is required</param>
		/// <param name="message">required message</param>
		void AddRequired(string partName, string message);

		/// <summary>
		/// Adds an error message to the context
		/// </summary>
		/// <param name="partName">part name that is invalid</param>
		/// <param name="message">error message</param>
		void AddError(string partName, string message);


		/// <summary>
		/// Add a warning message
		/// </summary>
		/// <param name="partName">part name that has warning</param>
		/// <param name="message">warning message</param>
		void AddWarning(string partName, string message);
	}

	/// <summary>
	/// Hold the information for rule execution
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRuleExecutionContext<out T>
	{
		/// <summary>
		/// Current state
		/// </summary>
		ValidationState State { get; set; }

		/// <summary>
		/// Message for rule
		/// </summary>
		string Message { get; set; }

		/// <summary>
		/// Object to validate
		/// </summary>
		T ValidationObject { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="part"></param>
		void AddValidationPart(IValidationRuleResultPart part);

		void AddRequired(string partName, string message);

		void AddError(string partName, string message);

		void AddWarning(string partName, string message);
	}
}
