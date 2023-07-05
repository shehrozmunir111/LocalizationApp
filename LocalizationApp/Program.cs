using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using Google.Cloud.Translation.V2;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LocalizationApp
{
    class Program
    {
        // Import the required Win32 API functions
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool SetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref CONSOLE_FONT_INFO_EX consoleFontInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }

        static void Main(string[] args)
        {
            try
            {
                // Set the console font and encoding for Arabic characters
                SetConsoleFont("Consolas", 14, "Arial");
                Console.OutputEncoding = Encoding.UTF8;

                // Set console colors
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;

                // Clear the console screen
                Console.Clear();

                // Prompt the user to select a method
                Console.WriteLine("Which method do you want?");
                Console.WriteLine("1. Existing Resource files");
                Console.WriteLine("2. From Custom Folder");

                // Read the user's input
                string methodOption = Console.ReadLine();

                if (methodOption == "1")
                {
                    ExistingRecourceFiles();
                }
                else if (methodOption == "2")
                {
                    LocalizationMethod();
                }
                else
                {
                    Console.WriteLine("Invalid method option. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            // Wait for user input before closing the application
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void SetConsoleFont(string fontName, short fontSize, string fallbackFontName = "")
        {
            IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);
            if (hnd != IntPtr.Zero)
            {
                CONSOLE_FONT_INFO_EX consoleFontInfo = new CONSOLE_FONT_INFO_EX();
                consoleFontInfo.cbSize = (uint)Marshal.SizeOf(consoleFontInfo);
                consoleFontInfo.FontFamily = TMPF_TRUETYPE;
                consoleFontInfo.FaceName = fontName;
                if (fallbackFontName.Length > 0)
                    consoleFontInfo.FaceName += $", {fallbackFontName}";
                consoleFontInfo.dwFontSize = new COORD() { X = 0, Y = fontSize };

                SetCurrentConsoleFontEx(hnd, false, ref consoleFontInfo);
            }
        }

        // Constants for Win32 API calls
        private const int STD_OUTPUT_HANDLE = -11;
        private const int TMPF_TRUETYPE = 4;

        private static void ExistingRecourceFiles()
        {
            while (true)
            {
                Console.WriteLine("Enter a sentence in English (or type 'exit' to quit):");
                // Read the user's input
                string userInput = Console.ReadLine();

                // Check if the user wants to exit
                if (userInput.ToLower() == "exit")
                    break;

                // Prompt the user to select a language
                Console.WriteLine("Select a language for translation:");
                Console.WriteLine("1. Arabic");
                Console.WriteLine("2. Spanish");
                Console.WriteLine("3. German");
                Console.WriteLine("4. Urdu");

                // Read the user's language selection
                int languageOption;
                if (int.TryParse(Console.ReadLine(), out languageOption))
                {
                    // Check the selected language and set the culture accordingly
                    CultureInfo cultureInfo;
                    string resourceFileName;
                    switch (languageOption)
                    {
                        case 1:
                            cultureInfo = new CultureInfo("ar");
                            resourceFileName = "LocalizationApp.Resources";
                            break;
                        case 2:
                            cultureInfo = new CultureInfo("es");
                            resourceFileName = "LocalizationApp.ActiveDirectoryUser";
                            break;
                        case 3:
                            cultureInfo = new CultureInfo("de");
                            resourceFileName = "LocalizationApp.Resources";
                            break;
                        case 4:
                            cultureInfo = new CultureInfo("ur");
                            resourceFileName = "LocalizationApp.Resources";
                            break;
                        default:
                            Console.WriteLine("Invalid language option. Please try again.");
                            Console.WriteLine();
                            continue;
                    }

                    // Load the resource manager for the selected language
                    var resourceManager = new ResourceManager(resourceFileName, typeof(Program).Assembly);
                    CultureInfo.CurrentCulture = cultureInfo;
                    CultureInfo.CurrentUICulture = cultureInfo;
                    // Process the user's input
                    ProcessInput(userInput, resourceManager, cultureInfo);
                }
                else
                {
                    Console.WriteLine("Invalid language option. Please try again.");
                    Console.WriteLine();
                }
            }
        }

        private static void LocalizationMethod()
        {
            while (true)
            {
                Console.WriteLine("Enter a sentence in English (or type 'exit' to quit):");
                // Read the user's input
                string userInput = Console.ReadLine();

                // Check if the user wants to exit
                if (userInput.ToLower() == "exit")
                    break;

                var localizationObj = new Localization();
                var result = localizationObj.SearchUserInput(userInput);

                if (result["ErrorOccurred"] == "false")
                {
                    Console.WriteLine("Translation: " + result["ReturnedMessage"]);
                }
                else
                {
                    Console.WriteLine(result["ReturnedMessage"]);
                }
            }
        }

        private static void ProcessInput(string userInput, ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            // Look up the localized translation
            try
            {
                // Look up the localized translation
                string localizedText = resourceManager.GetString(userInput, cultureInfo);

                // Check if the translation is found
                if (localizedText != null)
                {
                    // Display the localized text
                    Console.WriteLine("Translation: " + localizedText);

                    // Retrieve the corresponding value from the resource file for verification
                    string expectedTranslation = resourceManager.GetString(userInput);

                    // Compare the translated text to the expected translation
                    if (localizedText == expectedTranslation)
                    {
                        Console.WriteLine("The translation is correct!");
                    }
                    else
                    {
                        Console.WriteLine("The translation is incorrect.");
                        Console.WriteLine("Expected translation: " + expectedTranslation);
                    }
                }
                else
                {
                    // Display a custom message for missing translation
                    Console.WriteLine("Translation not found for the entered sentence.");
                }
            }
            catch (Exception)
            {
                // Display a custom message for missing translation
                Console.WriteLine("Translation not found for the entered sentence.");
            }
            finally
            {
                Console.WriteLine();
            }
        }
    }
}
