/* If you want default messages in English, you don't need to reference this file, because the messages are 
 * in English by default anyway. This file exists only for reference during translation.
 *
 * To get default messages in any other language (or a different dialect of English):
 *  (1) Make a copy of this file and call it anything else, for example: xVal.Messages.fr-FR.js
 *  (2) Edit your copy to specify messages in your chosen language
 *  (3) Reference your file in your project using a <script> tag.
 * Simply referencing your custom file will override the default messages with your messages.
 *
 * Note that at present this only works when using jQuery Validation (not ASP.NET native) but
 * I will add localisation support in ASP.NET native validation shortly.
*/
var xVal = xVal || {};
xVal.Messages = {
    "Required" : "This value is required.",
    "DataType_EmailAddress" : "Please enter a valid email address.",
    "DataType_Integer" : "Please enter a whole number.",
    "DataType_Decimal" : "Please enter a number.",
    "DataType_Date" : "Please enter a valid date.",
    "DataType_DateTime" : "Please enter a date and time.",
    "DataType_Currency" : "Please enter an amount of money.",
    "DataType_CreditCardLuhn" : "Please enter a valid credit card number.",
    "Regex" : "This value is invalid.",
    "Range_Numeric_Min" : "Please enter a value of at least {0}.",
    "Range_Numeric_Max" : "Please enter a value less than or equal to {0}.",
    "Range_Numeric_MinMax" : "Please enter a value between {0} and {1}.",
    "Range_String_Min" : "Please enter a value not alphabetically before '{0}'.",
    "Range_String_Max" : "Please enter a value not alphabetically after '{0}'.",
    "Range_String_MinMax" : "Please enter a value alphabetically between '{0}' and '{1}'.",
    "Range_DateTime_Min" : "Please enter a date no earlier than {0}.",
    "Range_DateTime_Max": "Please enter a date no later than {0}.",
    "Range_DateTime_MinMax": "Please enter a date between {0} and {1}.",
    "StringLength_Min": "Please enter at least {0} characters.",
    "StringLength_Max": "Please enter no more than {0} characters.",
    "StringLength_MinMax": "Please enter between {0} and {1} characters.",
    "Comparison_Equals" : "This value must be the same as {0}.",
    "Comparison_DoesNotEqual" : "This value must be different from {0}."
};