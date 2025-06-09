using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.Properties;

namespace Tuick
{
	public class Bindable<T> : INotifyBindablePropertyChanged
	{
		private T _value;

		// UXMLからのバインディングターゲットとなるプロパティ
		[CreateProperty]
		public T Value
		{
			get => _value;
			set
			{
				if (EqualityComparer<T>.Default.Equals(_value, value))
					return;

				_value = value;
				// "Value"プロパティが変更されたことを通知する
				propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(nameof(Value)));
			}
		}

		public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

		public Bindable(T initialValue = default)
		{
			_value = initialValue;
		}

		// 暗黙の変換
		// public static implicit operator T(Bindable<T> b) => b.Value;
	}
}