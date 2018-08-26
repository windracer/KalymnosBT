using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace KalymnosBT.MVVMBase
{
    public class ObservableObject : IChangeTracking, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected ObservableObject()
        {
            PropertyChanged += new PropertyChangedEventHandler(OnNotifiedOfPropertyChanged);
        }

        private void OnNotifiedOfPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e != null && !String.Equals(e.PropertyName, "IsChanged", StringComparison.Ordinal))
            {
                IsChanged = true;
            }
        }

        private bool _thisObjectWasChanged;
        private readonly object _notifyingObjectIsChangedSyncRoot = new object();

        [JsonIgnore]
        public bool IsChanged
        {
            get
            {
                lock (_notifyingObjectIsChangedSyncRoot)
                {
                    return _thisObjectWasChanged;
                }
            }
            protected set
            {
                lock (_notifyingObjectIsChangedSyncRoot)
                {
                    if (!Boolean.Equals(_thisObjectWasChanged, value))
                    {
                        _thisObjectWasChanged = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public void AcceptChanges()
        {
            IsChanged = false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Assert(GetType().GetProperty(propertyName) != null);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propName);
                return true;
            }
            return false;
        }

        protected bool SetProperty<T>(ref T field, T value, Expression<Func<T>> expr)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var lambda = (LambdaExpression)expr;
                MemberExpression memberExpr;
                if (lambda.Body is UnaryExpression)
                {
                    var unaryExpr = (UnaryExpression)lambda.Body;
                    memberExpr = (MemberExpression)unaryExpr.Operand;
                }
                else
                {
                    memberExpr = (MemberExpression)lambda.Body;
                }

                OnPropertyChanged(memberExpr.Member.Name);
                return true;
            }
            return false;
        }
    }

}
