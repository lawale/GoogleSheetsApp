using GoogleSheetsApp.Validations;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace GoogleSheetsApp.Models
{
    public class UserResponse : ValidationBase
    {
        
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string email;

        [RegularExpression(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
     + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$"), Required(ErrorMessage = "Email address is required.")]
        public string Email
        {
            get => email;

            set => SetValue(ref email, value);
        }

        private bool emailHasError;

        [Ignore]
        public bool EmailHasError
        {
            get => emailHasError;

            set => SetProperty(ref emailHasError, value);
        }

        private string lastName;

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName
        {
            get => lastName;

            set => SetValue(ref lastName, value);
        }

        private bool lastNameHasError;

        [Ignore]
        public bool LastNameHasError
        {
            get => lastNameHasError;

            set => SetProperty(ref lastNameHasError, value);
        }

        private string firstName;

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName
        {
            get => firstName;

            set => SetValue(ref firstName, value);
        }

        private bool firstNameHasError;

        [Ignore]
        public bool FirstNameHasError
        {
            get => lastNameHasError;

            set => SetProperty(ref firstNameHasError, value);
        }

        private string phoneNumber;

        [Phone]
        public string PhoneNumber
        {
            get => phoneNumber;

            set => SetValue(ref phoneNumber, value);
        }

        private bool phoneHasError;

        [Ignore]
        public bool PhoneHasError
        {
            get => phoneHasError;

            set => SetProperty(ref phoneHasError, value);
        }

        private string address;

        public string Address
        {
            get => address;

            set => SetValue(ref address, value);
        }

        private bool isSubscribed;

        public bool IsSubscribed
        {
            get => isSubscribed;

            set => SetValue(ref isSubscribed, value);
        }

        private string notes;

        public string Notes
        {
            get => notes;

            set => SetProperty(ref notes, value);
        }

        public bool HasBeenSubmitted { get; set; }


        public List<IList<object>> ToFormsData()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");

            IList<object> data = new List<object>
            {
                DateTime.Now.ToString("G", culture), Email, LastName, FirstName, PhoneNumber, Address, IsSubscribed, Notes
            };

            List<IList<object>> value = new List<IList<object>>
            {
                data
            };

            return value;
        }

        public IList<object> ToFormsSingleData()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");

            IList<object> data = new List<object>
            {
                DateTime.Now.ToString("G", culture), Email, LastName, FirstName, PhoneNumber, Address, IsSubscribed, Notes
            };
            return data;
        }

        private void SetValue<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null)
        {
            //ValidateProperty(value, propertyName);
            SetProperty(ref backingStore, value, propertyName);
        }

        
    }
}
