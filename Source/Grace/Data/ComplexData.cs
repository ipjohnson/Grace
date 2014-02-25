using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Data
{
	/// <summary>
	/// Intended to be used for modeling complex hierarchies of objects
	/// </summary>
	public class ComplexObject : IComplexObject, INotifyPropertyChanged
	{
		private IComplexObject parent;
		private ChildChangedHandler notifyUponchange;
		private List<IComplexObject> children;
		private string identityInParent;
		private bool isDirty = false;


		/// <summary>
		/// Parent of the object
		/// </summary>
		public IComplexObject Parent
		{
			get { return parent; }
		}

		/// <summary>
		/// Identity in the parent object (usually property name)
		/// </summary>
		public string IdentityInParent
		{
			get
			{
				if (string.IsNullOrEmpty(identityInParent))
				{
					return GetType().Name;
				}

				return identityInParent;
			}
			private set { identityInParent = value; }
		}

		/// <summary>
		/// Children of this complex object
		/// </summary>
		public virtual IEnumerable<IComplexObject> Children
		{
			get { return children; }
		}

		/// <summary>
		/// Is the object dirty
		/// </summary>
		public virtual bool IsDirty
		{
			get { return isDirty; }
			set
			{
				if (!value && isDirty)
				{
					isDirty = false;

					if (children != null)
					{
						foreach (IComplexObject child in children)
						{
							child.IsDirty = false;
						}
					}
				}
			}
		}

		/// <summary>
		/// Set the owning parent of the object
		/// </summary>
		/// <param name="newParent">new parent</param>
		/// <param name="identityInParent">name inside parent</param>
		/// <param name="notifyUponchange">handler to be notified upon change</param>
		/// <returns>
		/// returns true if the parent was set
		/// </returns>
		/// <exception cref="System.Exception">Object already has a parent. Remove frmo its previous heirarchy first.</exception>
		public bool SetParent(IComplexObject newParent,
									 string identityInParent,
									 ChildChangedHandler notifyUponchange)
		{
			if (parent == null)
			{
				this.notifyUponchange = notifyUponchange;

				parent = newParent;
				IdentityInParent = identityInParent;

				return true;
			}

			if (newParent == null)
			{
				this.notifyUponchange = null;

				parent = null;
				IdentityInParent = null;

				return true;
			}

			throw new Exception(
				"Object already has a parent. Remove frmo its previous heirarchy first.");
		}

		/// <summary>
		/// Data Changed event
		/// </summary>
		public event EventHandler<ComplexDataChangedArgs> DataChanged;

		/// <summary>
		/// Occurs when [property changed].
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Gets the children list.
		/// </summary>
		/// <value>
		/// The children list.
		/// </value>
		protected List<IComplexObject> ChildrenList
		{
			get
			{
				return children ?? (children = new List<IComplexObject>());
			}
		}

		/// <summary>
		/// Sets the property.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="storage">The storage.</param>
		/// <param name="value">The value.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns></returns>
		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
		{
			if (Equals(storage, value))
			{
				return false;
			}

			ComplexDataChangedArgs changedArgs =
				new ComplexDataChangedArgs(IdentityInParent, propertyName)
				{
					OldValue = storage,
					NewValue = value,
					OriginalSender = this
				};

			IComplexObject oldValue = storage as IComplexObject;

			if (oldValue != null)
			{
				ChildrenList.Remove(oldValue);

				oldValue.SetParent(null, null, null);
			}

			storage = value;

			isDirty = true;

			IComplexObject newValue = storage as IComplexObject;

			if (newValue != null)
			{
				ChildrenList.Add(newValue);

				newValue.SetParent(this, propertyName, ChildDataChange);
			}

			PropertyChangedEventHandler handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, changedArgs);
			}

			EventHandler<ComplexDataChangedArgs> dataChanged = DataChanged;

			if (dataChanged != null)
			{
				dataChanged(this, changedArgs);
			}

			if (notifyUponchange != null)
			{
				changedArgs.FullPropertyName =
					string.Concat(IdentityInParent, '.', changedArgs.FullPropertyName);
			}

			return true;
		}

		/// <summary>
		/// Child data change handler
		/// </summary>
		/// <param name="child">The child.</param>
		/// <param name="args">The arguments.</param>
		protected virtual void ChildDataChange(IComplexObject child,
															ComplexDataChangedArgs args)
		{
			args.FullPropertyName =
				string.Concat(IdentityInParent, '.', args.FullPropertyName);

			EventHandler<ComplexDataChangedArgs> dataChanged = DataChanged;

			isDirty = true;

			if (dataChanged != null)
			{
				dataChanged(this, args);
			}

			if (notifyUponchange != null)
			{
				notifyUponchange(this, args);
			}
		}
	}
}
