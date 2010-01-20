using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace BoC.Web.Mvc.Binders
{
    public class DateTimeModelBinder : IModelBinder
    {

        public DateTimeModelBinder()
        {
            Date = "Date";
            Time = "Time";
            Month = "Month";
            Day = "Day";
            Year = "Year";
            Hour = "Hour";
            Minute = "Minute";
            Second = "Second";
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }
            if (!String.IsNullOrEmpty(bindingContext.ModelName) && !bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
            {
                return null;
            }

            //see if we received a number and assume it's a UNIX timestamp.... or should we parse tick-based also?
            string raw = GetRawString(bindingContext, this.Date) ?? GetRawString(bindingContext, "");
            if (raw != null)
            {
                long num;
                if (long.TryParse(raw, out num))
                {
                    return new System.DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(num);
                }
            }

            //try some dates
            DateTime? dateAttempt = 
                GetA<DateTime>(bindingContext, this.Date) ?? 
                GetA<DateTime>(bindingContext, "");
            
            DateTime? timeAttempt = GetA<DateTime>(bindingContext, this.Time);

            //Maybe they wanted the Time via parts
            if (timeAttempt == null)
            {
                var hour = GetA<int>(bindingContext, this.Hour);
                if (hour != null)
                {
                    timeAttempt = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day,
                                               hour ?? 0,
                                               GetA<int>(bindingContext, this.Minute) ?? 0,
                                               GetA<int>(bindingContext, this.Second) ?? 0);
                }
            }

            //Maybe they wanted the Date via parts
            if (dateAttempt == null)
            {
                var day = GetA<int>(bindingContext, this.Day);
                if (day != null)
                {
                    dateAttempt = new DateTime(GetA<int>(bindingContext, this.Year) ?? DateTime.Now.Year,
                                               GetA<int>(bindingContext, this.Month) ?? DateTime.Now.Month,
                                               day.Value,
                                               DateTime.MinValue.Hour, DateTime.MinValue.Minute,
                                               DateTime.MinValue.Second);
                }
            }

            if (timeAttempt != null && dateAttempt != null)
            {
                return new DateTime(dateAttempt.Value.Year, dateAttempt.Value.Month, dateAttempt.Value.Day,
                                    timeAttempt.Value.Hour,
                                    timeAttempt.Value.Minute,
                                    timeAttempt.Value.Second);
            }
            return dateAttempt ?? timeAttempt;
        }

        private static string GetRawString(ModelBindingContext bindingContext, string key)
        {
            var modelName = (bindingContext.ModelName == "entity") ? null : bindingContext.ModelName;
            var valueName = CreateSubPropertyName(modelName, key);

            //Try it with the prefix...
            var valueResult = bindingContext.ValueProvider.GetValue(valueName);
            //Didn't work? Try without the prefix if needed...
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix == true)
            {
                valueResult = bindingContext.ValueProvider.GetValue(key);
            }
            if (valueResult == null)
            {
                return null;
            }
            return valueResult.AttemptedValue;
            
        }

        private static T? GetA<T>(ModelBindingContext bindingContext, string key) where T : struct
        {
            var modelName = (bindingContext.ModelName == "entity") ? null : bindingContext.ModelName;
            var valueName = CreateSubPropertyName(modelName, key);

            //Try it with the prefix...
            var valueResult = bindingContext.ValueProvider.GetValue(valueName);
            //Didn't work? Try without the prefix if needed...
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix == true)
            {
                valueResult = bindingContext.ValueProvider.GetValue(key);
            }
            if (valueResult == null)
            {
                return null;
            }
            return (T?)valueResult.ConvertTo(typeof(T));
        }

        protected static string CreateSubPropertyName(string prefix, string propertyName)
        {
            if (String.IsNullOrEmpty(prefix))
            {
                return propertyName;
            }
            else if (String.IsNullOrEmpty(propertyName))
            {
                return prefix;
            }
            else
            {
                return prefix + "." + propertyName;
            }
        }


        public string Date { get; set; }
        public string Time { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Year { get; set; }
        public string Hour { get; set; }
        public string Minute { get; set; }
        public string Second { get; set; }
    }

    public class DateAndTimeAttribute : CustomModelBinderAttribute
    {

        private IModelBinder _binder;

        // The user cares about a full date structure and full time structure, or one or the other.
        public DateAndTimeAttribute(string date, string time)
        {
            _binder = new DateTimeModelBinder { Date = date, Time = time };
        }

        // The user wants to capture the date and time (or only one) as individual portions.
        public DateAndTimeAttribute(string year, string month, string day, string hour, string minute, string second)
        {
            _binder = new DateTimeModelBinder { Day = day, Month = month, Year = year, Hour = hour, Minute = minute, Second = second };
        }

        // The user wants to capture the date and time (or only one) as individual portions.
        public DateAndTimeAttribute(string date, string time, string year, string month, string day, string hour, string minute, string second)
        {
            _binder = new DateTimeModelBinder { Day = day, Month = month, Year = year, Hour = hour, Minute = minute, Second = second, Date = date, Time = time };
        }

        public override IModelBinder GetBinder() { return _binder; }
    }
}
