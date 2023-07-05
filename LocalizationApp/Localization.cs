using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationApp
{
    public class Localization
    {
        private string resourceFolderPath; // Variable to hold the folder path for .resx files
        private Dictionary<string, Dictionary<string, string>> resourceData; // Dictionary to store resource data

        public Localization()
        {
            string folderPath = @"C:\Users\shehroz.munir\Desktop\FolderComp\Forms";
            resourceFolderPath = folderPath; // Assign the provided folder path to the variable
            resourceData = new Dictionary<string, Dictionary<string, string>>(); // Initialize the dictionary

            // Load all .resx files from the specified folder
            var resourceFiles = Directory.GetFiles(resourceFolderPath, "*.resx");

            foreach (var file in resourceFiles)
            {
                // Extract the base name of the .resx file
                var baseName = Path.GetFileNameWithoutExtension(file);

                // Read the data from the resource file
                var data = ReadResourceData(file);

                // Add the resource data to the dictionary
                resourceData.Add(baseName, data);
            }
        }

        private Dictionary<string, string> ReadResourceData(string filePath)
        {
            var data = new Dictionary<string, string>();

            using (ResXResourceReader resourceReader = new ResXResourceReader(filePath))
            {
                foreach (DictionaryEntry entry in resourceReader)
                {
                    if (entry.Value != null)
                    {
                        string key = entry.Key.ToString();
                        string value = entry.Value.ToString();
                        data.Add(key, value);
                    }
                }
            }

            return data;
        }

        public Dictionary<string, string> SearchUserInput(string userInput)
        {
            CultureInfo cultureInfo = new CultureInfo("es");
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            var result = new Dictionary<string, string>();

            try
            {
                string expectedTranslation = null;

                // Loop through each resource data in the dictionary
                foreach (var data in resourceData.Values)
                {
                    if (data.TryGetValue(userInput, out expectedTranslation))
                        break; // Found the translation, exit the loop
                }

                if (expectedTranslation != null)
                {
                    // Store the expected translation in the userInput field
                    userInput = expectedTranslation;

                    result["ReturnedMessage"] = expectedTranslation;
                    result["ErrorOccurred"] = "false";
                }
                else
                {
                    result["ReturnedMessage"] = "Translation not found for the entered sentence.";
                    result["ErrorOccurred"] = "true";
                }
            }
            catch (Exception)
            {
                result["ReturnedMessage"] = "An error occurred while processing the input.";
                result["ErrorOccurred"] = "true";
            }

            return result;
        }
    }
}
