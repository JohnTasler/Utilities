namespace DataObjectViewer.ComponentModel.Mvvm
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;

	/// <summary>
	/// This is the abstract base class for any object that provides property change notifications.  
	/// </summary>
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableObject"/> class.
		/// </summary>
		protected ObservableObject()
		{
		}

		#endregion Constructor

		#region RaisePropertyChanged

		/// <summary>
		/// Raises this object's PropertyChanged event for the specified <paramref name="propertyName"/>.
		/// </summary>
		/// <param name="propertyName">The name of the property that has changed.</param>
		protected void RaisePropertyChanged(string propertyName)
		{
			this.VerifyPropertyName(propertyName);

			var handler = this.PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}


		/// <summary>
		/// Raises this object's PropertyChanged event for the specified <paramref name="propertyNames"/>.
		/// </summary>
		/// <param name="propertyName">The names of the properties that have changed.</param>
		protected void RaisePropertyChanged(params string[] propertyNames)
		{
			if (propertyNames == null)
				throw new ArgumentNullException("propertyNames");

			#if DEBUG
				foreach (var propertyName in propertyNames)
					this.VerifyPropertyName(propertyName);
			#endif // DEBUG

			var handler = this.PropertyChanged;
			if (handler != null)
			{
				foreach (var propertyName in propertyNames)
					handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion RaisePropertyChanged

		#region Property Setter Methods

		/// <summary>
		/// Compares a field value to the specified value and, if not equal, sets the field to the new value and invokes
		/// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
		/// </summary>
		/// <typeparam name="T">The type of the field being compared. This is inferred from the <paramref name="field"/>
		/// and <paramref name="newValue"/> parameters, and does not normally need to be specified explicitly.</typeparam>
		/// <param name="field">A reference to the member field that backs the property being set.</param>
		/// <param name="newValue">The new value the property is being set to.</param>
		/// <param name="propertyName">The name of the property being set.</param>
		/// <returns><b>true</b> if the <paramref name="field"/> value was changed to <paramref name="newValue"/>;
		/// otherwise <b>false</b>.</returns>
		protected bool SetProperty<T>(ref T field, T newValue, string propertyName)
		{
			return this.SetProperty(ref field, newValue, EqualityComparer<T>.Default, propertyName);
		}

		/// <summary>
		/// Compares a field value to the specified value and, if not equal, sets the field to the new value and invokes
		/// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
		/// </summary>
		/// <typeparam name="T">The type of the field being compared. This is inferred from the <paramref name="field"/>
		/// and <paramref name="newValue"/> parameters, and does not normally need to be specified explicitly.</typeparam>
		/// <param name="field">A reference to the member field that backs the property being set.</param>
		/// <param name="newValue">The new value the property is being set to.</param>
		/// <param name="equalityComparer">An object that implements the <see cref="IEqualityComparer{T}"/> interface, used
		/// to test for the equality of the <paramref name="field"/> and <paramref name="newValue"/> parameters. If not
		/// specified, then <see cref="EqualityComparer{T}"/> is used.</param>
		/// <param name="propertyName">The name of the property being set.</param>
		/// <returns><b>true</b> if the <paramref name="field"/> value was changed to <paramref name="newValue"/>;
		/// otherwise <b>false</b>.</returns>
		protected bool SetProperty<T>(ref T field, T newValue, IEqualityComparer<T> equalityComparer, string propertyName)
		{
			bool hasChanged = !equalityComparer.Equals(field, newValue);
			if (hasChanged)
			{
				field = newValue;
				this.RaisePropertyChanged(propertyName);
			}
			return hasChanged;
		}

		/// <summary>
		/// Compares a field value to the specified value and, if not equal, sets the field to the new value and invokes
		/// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
		/// </summary>
		/// <typeparam name="T">The type of the field being compared. This is inferred from the <paramref name="field"/>
		/// and <paramref name="newValue"/> parameters, and does not normally need to be specified explicitly.</typeparam>
		/// <param name="field">A reference to the member field that backs the property being set.</param>
		/// <param name="newValue">The new value the property is being set to.</param>
		/// <param name="propertyNames">The names of zero or more properties affected by the changed property value.</param>
		/// <returns><b>true</b> if the <paramref name="field"/> value was changed to <paramref name="newValue"/>;
		/// otherwise <b>false</b>.</returns>
		protected bool SetProperty<T>(ref T field, T newValue, params string[] propertyNames)
		{
			return this.SetProperty(ref field, newValue, EqualityComparer<T>.Default, propertyNames);
		}

		/// <summary>
		/// Compares a field value to the specified value and, if not equal, sets the field to the new value and invokes
		/// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
		/// </summary>
		/// <typeparam name="T">The type of the field being compared. This is inferred from the <paramref name="field"/>
		/// and <paramref name="newValue"/> parameters, and does not normally need to be specified explicitly.</typeparam>
		/// <param name="field">A reference to the member field that backs the property being set.</param>
		/// <param name="newValue">The new value the property is being set to.</param>
		/// <param name="equalityComparer">An object that implements the <see cref="IEqualityComparer{T}"/> interface, used
		/// to test for the equality of the <paramref name="field"/> and <paramref name="newValue"/> parameters. If not
		/// specified, then <see cref="EqualityComparer{T}"/> is used.</param>
		/// <param name="propertyNames">The names of zero or more properties affected by the changed property value.</param>
		/// <returns><b>true</b> if the <paramref name="field"/> value was changed to <paramref name="newValue"/>;
		/// otherwise <b>false</b>.</returns>
		protected bool SetProperty<T>(ref T field, T newValue, IEqualityComparer<T> equalityComparer, params string[] propertyNames)
		{
			bool hasChanged = !equalityComparer.Equals(field, newValue);
			if (hasChanged)
			{
				field = newValue;
				this.RaisePropertyChanged(propertyNames);
			}
			return hasChanged;
		}

		#endregion Property Setter Methods

		#region Debugging Aides

		/// <summary>
		/// Warns the developer if this object does not have
		/// a public property with the specified name. This 
		/// method does not exist in a Release build.
		/// </summary>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			// If you raise PropertyChanged and do not specify a property name,
			// all properties on the object are considered to be changed by the binding system.
			if (string.IsNullOrEmpty(propertyName))
				return;

			// Verify that the property name matches a real,  
			// public, instance property on this object.
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				string message = "Invalid property name: " + propertyName;

				if (this.ThrowOnInvalidPropertyName)
					throw new ArgumentException(message);
				else
					Debug.Fail(message);
			}
		}

		/// <summary>
		/// Returns whether an exception is thrown, or if a Debug.Fail() is used
		/// when an invalid property name is passed to the VerifyPropertyName method.
		/// The default value is false, but subclasses used by unit tests might 
		/// override this property's getter to return true.
		/// </summary>
		protected virtual bool ThrowOnInvalidPropertyName
		{
			get;
			private set;
		}

		#endregion Debugging Aides

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Raised when a property on this object has a new value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion INotifyPropertyChanged Members
	}
}

