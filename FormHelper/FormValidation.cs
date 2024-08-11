using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FormHelper
{

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }

    public class Validator
    {
        public delegate void ValidationDelegate(object value, string regex = null);

        private static Dictionary<SettingType, ValidationDelegate> _validators = new Dictionary<SettingType, ValidationDelegate>()
        {
            { SettingType.String,       Validate_String},
            { SettingType.IPAddress,    Validate_IPAddress },
            { SettingType.NetworkPort,  Validate_NetworkPort },
            { SettingType.Number,       Validate_Number },
            { SettingType.Bool,         Validate_Bool  },
            { SettingType.File,         Validate_File },
            { SettingType.Directory,    Validate_Directory }
        };

        /// <summary>
        /// Validate a value based on setting type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException">if no validator is found</exception>
        public static void Validate<T>(UserSetting setting, T value)
        {
            if (value == null)
                throw new ValidationException($"The field {setting.Name} is required.");

            if (_validators.TryGetValue(setting.SettingType, out var validator))
            {
                validator(value, setting.ValidationRegex);
            }
            else
                throw new ValidationException($"No validator found for {setting.Name}:{setting.SettingType}" );
        }

        /// <summary>
        /// Check if the directory exists
        /// </summary>
        /// <param name="val"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        private static void Validate_Directory(object val, string regex = null)
        {
            ThrowIfInvalid(Directory.Exists(val + ""), "Folder does not exist");
        }

        /// <summary>
        /// Check if the file exists
        /// </summary>
        /// <param name="val"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        private static void Validate_File(object val, string regex = null)
        {
            ThrowIfInvalid(File.Exists(val + ""), "File does not exist");
        }

        /// <summary>
        /// Check if the value is true
        /// </summary>
        /// <param name="val"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        private static void Validate_Bool(object val, string regex = null)
        {

            if (bool.TryParse(val + "", out var isValid))
                ThrowIfInvalid(isValid, "Must be Checked");

            else
                ThrowIfInvalid(false, "Required");


        }



        /// <summary>
        /// Check if the value is a number
        /// </summary>
        /// <param name="val"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        private static void Validate_Number(object val, string regex = null)
        {
            ThrowIfInvalid(Regex.IsMatch(val + "", @"\d+"), "Not a Number");
        }
        

        /// <summary>
        /// Check if the value is a valid IP Address
        /// </summary>
        /// <param name="val"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        private static void Validate_IPAddress(object val, string regex = null)
        {
            ThrowIfInvalid(Regex.IsMatch(val + "", @"^(\d{1,3}\.){3}\d{1,3}$"), "Invalid IP Address");
        }

        /// <summary>
        /// If regex is null, will check if the string is not null or empty
        /// </summary>
        /// <param name="val"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static void Validate_String(object val, string regex = null)
        {
            
            if (regex == null && val != null)
            {
                ThrowIfInvalid(!string.IsNullOrWhiteSpace((string)val), "Required");
            }
            else if (regex != null && val != null)
            {
                ThrowIfInvalid(Regex.IsMatch((string)val, regex), "Invalid");
            }
            else
            {
                ThrowIfInvalid(false, "Required");
            }

        }

        /// <summary>
        /// Check is the value is a valid network port
        /// </summary>
        /// <param name="val"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        private static void Validate_NetworkPort(object val, string regex = null)
        {
            if (int.TryParse(val + "", out int port))
            {

                ThrowIfInvalid(0 <= port && port <= 65535, "Port must be between 0 - 65535");
            }
            else
            {
                ThrowIfInvalid(false, "Invalid Port");
            }
        }

        private static void ThrowIfInvalid(bool isValid, string message)
        {
            if (!isValid)
                throw new ValidationException(message);
        }
    }
    
}
