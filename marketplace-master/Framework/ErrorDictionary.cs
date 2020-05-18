using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public class ErrorDictionary : List<KeyValuePair<string, string>>
    {
        public bool HasErrors => Count > 0;

        public bool IsEmpty => Count == 0;

        public static ErrorDictionary Empty => new ErrorDictionary();

        public ErrorDictionary()
        {
        }
        
        public ErrorDictionary(string error)
        {
            Add("", error);
        }

        public ErrorDictionary(IEnumerable<string> errors)
        {
            Add("", errors.ToArray());
        }

        public ErrorDictionary(string key, string error)
        {
            Add(key, error);
        }

        public ErrorDictionary(string key, params string[] errors)
        {
            Add(key, errors);
        }

        public ErrorDictionary(ValidationResult validationResult)
        {
            Add(validationResult);
        }

        public static ErrorDictionary FromMany(IEnumerable<ErrorDictionary> errors)
        {
            var merged = new ErrorDictionary();

            if (errors != null)
                foreach (var error in errors)
                    merged.Merge(error);

            return merged;
        }

        public void Add(string key, string error)
        {
            var element = new KeyValuePair<string, string>(key, error);

            Add(element);
        }

        public void Add(string key, params string[] errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            foreach (var error in errors)
            {
                Add(key, error);
            }
        }

        public void Add(ValidationResult validationResult)
        {
            if (validationResult == null || validationResult.IsValid)
                return;

            foreach (var err in validationResult.Errors)
                Add(err.PropertyName, err.ErrorMessage);
        }

        public void Merge(ErrorDictionary dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            dict.ForEach(i =>
            {
                Add(i.Key, i.Value);
            });
        }

        public override string ToString()
        {
            var list = this.Select(error => error.Key + ": " + error.Value);

            return string.Join(Environment.NewLine, list);
        }

        public string ToSimpleString()
        {
            var list = this.Select(error => error.Value);

            return string.Join(Environment.NewLine, list);
        }
    }
}