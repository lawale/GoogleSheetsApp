using GoogleSheetsApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace GoogleSheetsApp.Validations
{
    public class ValidationBase : BindableBase, INotifyDataErrorInfo
    {
        #region Fields and Properties
        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        private readonly object sync = new object();

        public ValidationBase()
        {
            ErrorsChanged += OnErrorsChanged;
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HasErrors));
            RaisePropertyChanged(nameof(ErrorsList));
        }

        public bool HasErrors => errors.Any(x => x.Value.Any());

        public string GetFirstInvalidPropertyName => HasErrors ? errors.Select(x => x.Key).FirstOrDefault() : string.Empty;

        #endregion

        public IEnumerable GetErrors(string propertyName)
        {
            if(!string.IsNullOrEmpty(propertyName))
            {
                if (errors.ContainsKey(propertyName) && (errors[propertyName].Any()))
                    return errors[propertyName].ToList();
                return new List<string>();
            }
            return errors.SelectMany(x => x.Value.ToList()).ToList();
        }

        protected virtual void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock(sync)
            {
                var validationContext = new ValidationContext(this, null)
                {
                    MemberName = propertyName
                };

                var validationResults = new List<ValidationResult>();
                if(Validator.TryValidateProperty(value, validationContext, validationResults))
                    RemoveErrorsByPropertyName(propertyName);
                HandleValidationResults(validationResults);
            }
        }

        private void RemoveErrorsByPropertyName(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
                errors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }

        public void Validate(string property)
        {
            lock (sync)
            {
                var validationContext = new ValidationContext(this)
                {
                    MemberName = property
                };
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults);
                var propNames = errors.Keys.ToList();
                errors.Clear();

                foreach (var propertyName in propNames)
                {
                    OnErrorsChanged(propertyName);
                }
                HandleValidationResults(validationResults);
            }
        }

        private void HandleValidationResults(List<ValidationResult> validationResults)
        {
            var resultsByPropNames = from results in validationResults
                                     from memberNames in results.MemberNames
                                     group results by memberNames into groups
                                     select groups;

            foreach (var prop in resultsByPropNames)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                if (!errors.ContainsKey(prop.Key))
                    errors.Add(prop.Key, messages);
                else
                    errors[prop.Key] = messages;
                OnErrorsChanged(prop.Key);
            }
        }

        public IList<string> ErrorsList => GetErrors(string.Empty).Cast<string>().ToList();

        #region Events & Eventhandlers
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            var isNull = ErrorsChanged == null;
        }
        #endregion
    }
}