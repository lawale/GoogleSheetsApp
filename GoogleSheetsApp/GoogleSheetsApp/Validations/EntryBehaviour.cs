using GoogleSheetsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using XF.Material.Forms.UI;

namespace GoogleSheetsApp.Validations
{
    public class EntryBehaviour : Behavior<MaterialTextField>
    {
        public string PropertyName { get; set; }

        public EntryBehaviour()
        {
            
        }


        protected override void OnAttachedTo(MaterialTextField entry)
        {
            base.OnAttachedTo(entry);
            entry.Completed += OnCompleted;
        }

        private void OnCompleted(object sender, EventArgs e)
        {
            if (sender is MaterialTextField entry && entry.BindingContext is UserResponse source && !string.IsNullOrEmpty(PropertyName))
            {
                source.Validate(PropertyName);
                var errors = source.GetErrors(PropertyName).Cast<string>();
                if (errors != null && errors.Any())
                {
                    switch(PropertyName)
                    {
                        case "Email":
                            source.EmailHasError = true;
                            break;
                        case "LastName":
                            source.LastNameHasError = true;
                            break;
                        case "FirstName":
                            source.FirstNameHasError = true;
                            break;
                    }
                    //entry.HasError = true;
                    //entry.ErrorText = errors.First();
                }
                else
                {

                    switch (PropertyName)
                    {
                        case "Email":
                            source.EmailHasError = false;
                            break;
                        case "LastName":
                            source.LastNameHasError = false;
                            break;
                        case "FirstName":
                            source.FirstNameHasError = false;
                            break;
                    }
                }
                //switch(PropertyName)
                //{
                //    case "Email":
                //        source.EmailHasError = string.IsNullOrWhiteSpace(entry.Text);
                //        break;
                //    case "LastName":
                //        source.LastNameHasError = string.IsNullOrWhiteSpace(entry.Text);
                //        break;
                //    case "FirstName":
                //        source.FirstNameHasError = string.IsNullOrWhiteSpace(entry.Text);
                //        break;
                //}

            }
        }

        protected override void OnDetachingFrom(MaterialTextField entry)
        {
            base.OnDetachingFrom(entry);
            entry.Completed -= OnCompleted;
        }



        //public string PropertyName { get; set; }
        //protected override void OnAttachedTo(MaterialTextField entry)
        //{
        //    base.OnAttachedTo(entry);
        //    entry.TextChanged += OnTextChanged;
        //}


        //private void OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if(sender is MaterialTextField entry && entry.BindingContext is ValidationBase source && !string.IsNullOrEmpty(PropertyName))
        //    {
        //        var errors = source.GetErrors(PropertyName).Cast<string>();
        //        if(errors != null && errors.Any())
        //        {
        //            entry.HasError = true;
        //            entry.ErrorText = errors.First();
        //        }
        //        else
        //        {
        //            entry.HasError = false;
        //            entry.ErrorText = string.Empty;
        //        }
        //    }
        //}

        //protected override void OnDetachingFrom(MaterialTextField entry)
        //{
        //    base.OnDetachingFrom(entry);
        //    entry.TextChanged -= OnTextChanged;
        //}

    }
}
