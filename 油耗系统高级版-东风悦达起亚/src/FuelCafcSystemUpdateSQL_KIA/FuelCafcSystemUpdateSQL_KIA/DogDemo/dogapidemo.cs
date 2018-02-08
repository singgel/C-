////////////////////////////////////////////////////////////////////
// Copyright (C) 2012 SafeNet, Inc. All rights reserved.
//
// SuperDog(R) is a trademark of SafeNet, Inc.
//
//
////////////////////////////////////////////////////////////////////
using System;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Text;
using System.Runtime.InteropServices;
using SuperDog;


namespace DogDemo
{
    /// <summary>
    /// </summary>
    public class DogDemo
    {
        protected StringCollection stringCollection;
        protected System.Windows.Forms.TextBox textHistory;

        public const int FileId = 0xfff4;    // file id of default read/write file

        public const string defaultScope = "<dogscope />";

        private string scope;

        /// <summary>
        /// Constructor
        /// Intializes the object.
        /// </summary>
        public DogDemo(System.Windows.Forms.TextBox textHistory)
        {
            // keep a reference to the TextBox which 
            // shall dump the results of the operations.
            this.textHistory = textHistory;

            // next could be considered ugly.
            // build up a string collection holding
            // the status codes in a human readable manner.
            string[] stringRange = new string[] 
            {
                "Request successfully completed.", 
                "Request exceeds data file range.",
                "",
                "System is out of memory.", 
                "Too many open login sessions.", 
                "Access denied.",
                "",
                "Required SuperDog not found.", 
                "Encryption/decryption data length is too short.", 
                "Invalid input handle.", 
                "Specified File ID not recognized by API.", 
                "",
                "",
                "",
                "",
                "Invalid XML format.",
                "",
                "",
                "SuperDog to be updated not found.", 
                "Required XML tags not found; Contents in binary data are missing or invalid.", 
                "Update not supported by SuperDog.", 
                "Update counter is set incorrectly.", 
                "Invalid Vendor Code passed.",
                "",
                "Passed time value is outside supported value range.", 
                "",
                "Acknowledge data requested by the update, however the ack_data input parameter is NULL.", 
                "Program running on a terminal server.", 
                "",
                "Unknown algorithm used in V2C file.", 
                "Signature verification failed.", 
                "Requested Feature not available.", 
                "",
                "Communication error between API and local SuperDog License Manager.",
                "Vendor Code not recognized by API.", 
                "Invalid XML specification.", 
                "Invalid XML scope.", 
                "Too many SuperDog currently connected.", 
                "",
                "Session was interrupted.", 
                "",
                "Feature has expired.", 
                "SuperDog License Manager version too old.",
                "USB error occurred when communicating with a SuperDog.", 
                "",
                "System time has been tampered.", 
                "Communication error occurred in secure channel.", 
                "",
                "",
                "",
                "Unable to locate a Feature matching the scope.", 
                "",
                "",
                "",
                "Trying to install a V2C file with an update counter that is out" + 
                "of sequence with the update counter in the SuperDog." + 
                "The values of the update counter in the file are lower than" + 
                "those in the SuperDog.", 
                "Trying to install a V2C file with an update counter that is out" + 
                "of sequence with the update counter in the SuperDog." + 
                "The first value of the update counter in the file is greater than" + 
                "the value in the SuperDog."
            };

            stringCollection = new StringCollection();
            stringCollection.AddRange(stringRange);

            for (int n = stringCollection.Count; n < 400; n++)
            {
                stringCollection.Insert(n, "");
            }

            stringRange = new string[]  
            {
                "A required API dynamic library was not found.",
                "The found and assigned API dynamic library could not be verified.",
            };

            stringCollection.AddRange(stringRange);

            for (int n = stringCollection.Count; n < 500; n++)
            {
                stringCollection.Insert(n, "");
            }

            stringRange = new string[]  
            {
                "Object incorrectly initialized.",
                "A parameter is invalid.",
                "Already logged in.",
                "Already logged out."
            };

            stringCollection.AddRange(stringRange);

            for (int n = stringCollection.Count; n < 525; n++)
            {
                stringCollection.Insert(n, "");
            }

            stringCollection.Insert(525, "Incorrect use of system or platform.");

            for (int n = stringCollection.Count; n < 698; n++)
            {
                stringCollection.Insert(n, "");
            }

            stringCollection.Insert(698, "Capability is not available.");
            stringCollection.Insert(699, "Internal API error.");
        }


        /// <summary>
        /// Dumps a bunch of bytes into the referenced TextBox.
        /// </summary>
        protected void DumpBytes(byte[] bytes)
        {
            Verbose("Dumping data (max. 64 Bytes):");

            for (int index = 0; index < bytes.Length; index++)
            {
                if (0 == (index % 8))
                    FormDogDemo.WriteToTextbox(textHistory,
                        (0 == index) ? "          " : "\r\n          ");

                FormDogDemo.WriteToTextbox(textHistory, "0x" + bytes[index].ToString("X2") + " ");

                // for performance reason we only dump 64 bytes
                if (63 <= index)
                {
                    FormDogDemo.WriteToTextbox(textHistory, "\r\n          ...");
                    break;
                }
            }

            Verbose("");
        }


        /// <summary>
        /// Demonstrates the usage of the AES encryption and
        /// decryption methods.
        /// </summary>
        protected void EncryptDecryptDemo(Dog dog)
        {
            if ((null == dog) || !dog.IsLoggedIn())
                return;

            Verbose("Encrypt/Decrypt Demo");

            // the string to be encryted/decrypted.
            string text = "SuperDog is great";
            Verbose("Encrypting \"" + text + "\"");

            // convert the string into a byte array.
            byte[] data = UTF8Encoding.Default.GetBytes(text);

            // encrypt the data.
            DogStatus status = dog.Encrypt(data);
            ReportStatus(status);

            if (DogStatus.StatusOk == status)
            {
                text = UTF8Encoding.Default.GetString(data);
                Verbose("Encrypted string: \"" + text + "\"");
                Verbose("");
                Verbose("Decrypting \"" + text + "\"");

                // decrypt the data.
                // on success we convert the data back into a 
                // human readable string.
                status = dog.Decrypt(data);
                ReportStatus(status);

                if (DogStatus.StatusOk == status)
                {
                    text = UTF8Encoding.Default.GetString(data);
                    Verbose("Decrypted string: \"" + text + "\"");
                }
            }

            Verbose("");

            // Second choice - encrypting a string using the 
            // native .net API
            text = "Encrypt/Decrypt String";
            Verbose("Encrypting \"" + text + "\"");

            status = dog.Encrypt(ref text);
            ReportStatus(status);

            if (DogStatus.StatusOk == status)
            {
                Verbose("Encrypted string: \"" + text + "\"");

                Verbose("");
                Verbose("Decrypting \"" + text + "\"");

                status = dog.Decrypt(ref text);
                ReportStatus(status);

                if (DogStatus.StatusOk == status)
                    Verbose("Decrypted string: \"" + text + "\"");
            }

            Verbose("");
        }

        /// <summary>
        /// Prints the footer into the referenced TextBox.
        /// </summary>
        protected virtual void Footer()
        {
            FormDogDemo.WriteToTextbox(textHistory, "API Demo completed\r\n");
        }


        /// <summary>
        /// Writes the Demo header into the referenced TextBox.
        /// </summary>
        protected virtual void Header()
        {
            if (textHistory != null && 0xFFFF < textHistory.TextLength)
                textHistory.Clear();

            FormDogDemo.WriteToTextbox(textHistory,
                "____________________________________________________________\r\n" +
                string.Format("API Demo started ({0})\r\n\r\n", DateTime.Now.ToString()));
        }


        /// <summary>
        /// Demonstrates how to perform a login and an automatic
        /// logout using C#'s scope clause.
        /// </summary>
        protected void LoginDefaultAutoDemo()
        {
            Verbose("Login Demo with Default Feature (DogFeature.Default)");

            DogFeature feature = DogFeature.Default;

            // this will perform a logout and object disposal
            // when the using scope is left.
            using (Dog dog = new Dog(feature))
            {
                DogStatus status = dog.Login(VendorCode.Code, scope);
                ReportStatus(status);

                Verbose("Object going out of scope - System will call Dispose");
            }

            Verbose("");
        }


        /// <summary>
        /// Demonstrates how to login into a dog using the
        /// default feature. The default feature is ALWAYS 
        /// available in every SuperDog.
        /// </summary>
        protected Dog LoginDefaultDemo()
        {
            Verbose("Login Demo with Default Feature (DogFeature.Default)");

            DogFeature feature = DogFeature.Default;

            Dog dog = new Dog(feature);

            DogStatus status = dog.Login(VendorCode.Code, scope);
            ReportStatus(status);
            Verbose("");

            // Please note that there is no need to call
            // a logout function explicitly - although it is
            // recommended. The garbage collector will perform
            // the logout when disposing the object.
            // If you need more control over the logout procedure
            // perform one of the more advanced tasks.
            return dog.IsLoggedIn() ? dog : null;
        }


        /// <summary>
        /// Demonstrates how to login using a feature id.
        /// </summary>
        protected Dog LoginDemo(DogFeature feature)
        {
            Verbose("Login Demo with Feature: " +
                    feature.FeatureId.ToString());

            // create a dog object using a feature
            // and perform a login using the vendor code.
            Dog dog = new Dog(feature);

            DogStatus status = dog.Login(VendorCode.Code, scope);
            ReportStatus(status);

            Verbose("");

            return dog.IsLoggedIn() ? dog : null;
        }


        /// <summary>
        /// Demonstrates how to perform a login using the default
        /// feature and how to perform an automatic logout
        /// using the SuperDog's Dispose method.
        /// </summary>
        protected void LoginDisposeDemo()
        {
            Verbose("Login/Dispose Demo with Default Feature (DogFeature.Default)");

            DogFeature feature = DogFeature.Default;

            Dog dog = new Dog(feature);

            DogStatus status = dog.Login(VendorCode.Code, scope);
            ReportStatus(status);

            Verbose("Disposing object - will perform an automatic logout");
            dog.Dispose();

            Verbose("");
        }


            /// <summary>
            /// Demonstrates how to perform a login and a logout
            /// using the default feature.
            /// </summary>
            protected void LoginLogoutDefaultDemo()
            {
                Verbose("Login/Logout Demo with Default Feature (DogFeature.Default)");

                DogFeature feature = DogFeature.Default;

                Verbose("Login:");
                Dog dog = new Dog(feature);

                DogStatus status = dog.Login(VendorCode.Code, scope);
                ReportStatus(status);

                if (DogStatus.StatusOk == status)
                {
                    Verbose("Logout:");
                    status = dog.Logout();

                    ReportStatus(status);
                }

                // recommended, but not mandatory
                // this call ensures that all resources to SuperDog
                // are freed immediately.
                dog.Dispose();
                Verbose("");
            }


        /// <summary>
        /// Performs a logout operation
        /// </summary>
        protected void LogoutDemo(ref Dog dog)
        {
            if ((null == dog) || !dog.IsLoggedIn())
                return;

            Verbose("Logout Demo");

            DogStatus status = dog.Logout();
            ReportStatus(status);

            // get rid of the dog immediately.
            dog.Dispose();
            dog = null;
            Verbose("");
        }


        /// <summary>
        /// Demonstrates how to perform read and write
        /// operations on a file in SuperDog
        /// </summary>
        protected void ReadWriteDemo(Dog dog, Int32 fileId)
        {
            if ((null == dog) || !dog.IsLoggedIn())
                return;

            Verbose("Read/Write Demo");

            // Get a file object to a file in SuperDog.
            // please note: the file object is tightly connected
            // to its dog object. logging out from a dog also
            // invalidates the file object.
            // doing the following will result in an invalid
            // file object:
            // dog.login(...)
            // DogFile file = dog.GetFile();
            // dog.logout();
            // Debug.Assert(file.IsValid()); will assert
            DogFile file = dog.GetFile(fileId);
            if (!file.IsLoggedIn())
            {
                // Not logged into a dog - nothing left to do.
                Verbose("Failed to get file object\r\n");
                return;
            }

            Verbose("Reading contents of file: " + file.FileId.ToString());

            Verbose("Retrieving the size of the file");

            // get the file size
            int size = 0;
            DogStatus status = file.FileSize(ref size);
            ReportStatus(status);

            if (DogStatus.StatusOk != status)
            {
                Verbose("");
                return;
            }

            Verbose("Size of the file is: " + size.ToString() + " Bytes");

            // read the contents of the file into a buffer
            byte[] bytes = new byte[size];

            Verbose("Reading data");
            status = file.Read(bytes, 0, bytes.Length);
            ReportStatus(status);

            if (DogStatus.StatusOk != status)
            {
                Verbose("");
                return;
            }

            DumpBytes(bytes);

            Verbose("Writing to file");

            // now let's write some data into the file
            byte[] newBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7 };

            status = file.Write(newBytes, 0, newBytes.Length);
            ReportStatus(status);
            if (DogStatus.StatusOk != status)
            {
                Verbose("");
                return;
            }

            DumpBytes(newBytes);

            // and read them again
            Verbose("Reading written data");
            status = file.Read(newBytes, 0, newBytes.Length);
            ReportStatus(status);
            if (DogStatus.StatusOk == status)
                DumpBytes(newBytes);

            // restore the original contents
            file.Write(bytes, 0, bytes.Length);
            Verbose("");
        }


        /// <summary>
        /// Demonstrates how to read and write to/from a
        /// file at a certain file position
        /// </summary>
        protected void ReadWritePosDemo(Dog dog, Int32 fileId)
        {
            if ((null == dog) || !dog.IsLoggedIn())
                return;

            Verbose("GetFileSize/FilePos Demo");

            // firstly get a file object to a file.
            DogFile file = dog.GetFile(fileId);
            if (!file.IsLoggedIn())
            {
                // Not logged into dog - nothing left to do.
                Verbose("Failed to get file object\r\n");
                return;
            }

            Verbose("Reading contents of file: " + file.FileId.ToString());
            Verbose("Retrieving the size of the file");

            // we want to write an int at the end of the file.
            // therefore we are going to 
            // - get the file's size
            // - set the object's read and write position to
            //   the appropriate offset.
            int size = 0;
            DogStatus status = file.FileSize(ref size);
            ReportStatus(status);

            if (DogStatus.StatusOk != status)
            {
                Verbose("");
                return;
            }

            Verbose("Size of the file is: " + size.ToString() + " Bytes");
            Verbose("Setting file position to last int and reading value");

            // set the file pos to the end minus the size of int
            file.FilePos = size - DogFile.TypeSize(typeof(int));

            // now read what's there
            int aValue = 0;
            status = file.Read(ref aValue);
            ReportStatus(status);

            if (DogStatus.StatusOk != status)
            {
                Verbose("");
                return;
            }

            Verbose("Writing to file: 0x" + int.MaxValue.ToString("X2"));

            // write some data.
            status = file.Write(int.MaxValue);
            ReportStatus(status);

            if (DogStatus.StatusOk != status)
            {
                Verbose("");
                return;
            }

            // read back the written value.
            int newValue = 0;
            Verbose("Reading written data");
            status = file.Read(ref newValue);

            ReportStatus(status);
            if (DogStatus.StatusOk == status)
                Verbose("Data read: 0x" + newValue.ToString("X2"));

            // restore the original data.
            file.Write(aValue);
            Verbose("");
        }


        /// <summary>
        /// Dumps an operation status into the 
        /// referenced TextBox.
        /// </summary>
        protected void ReportStatus(DogStatus status)
        {
            FormDogDemo.WriteToTextbox(textHistory,
                string.Format("     Result: {0} (DogStatus::{1})\r\n",
                                    stringCollection[(int)status],
                                    status.ToString()));

            if (textHistory != null)
            {
                if (DogStatus.StatusOk == status)
                    textHistory.Refresh();
                else
                    textHistory.Parent.Refresh();
            }
        }


        /// <summary>
        /// Demonstrates how to get current time and date of 
        /// a SuperDog when available.
        /// </summary>
        protected void TestTimeDemo(Dog dog)
        {
            if ((null == dog) || !dog.IsLoggedIn())
                return;

            Verbose("Reading Time and Date Demo");

            DateTime time = DateTime.Now;
            DogStatus status = dog.GetTime(ref time);
            ReportStatus(status);

            if (DogStatus.StatusOk == status)
                Verbose("Time and date is " + time.ToString());

            Verbose("");
        }


        /// <summary>
        /// Runs the API demo.
        /// </summary>
        public void RunDemo(string scope)
        {
            try
            {
                this.scope = scope;

                Header();

                // Demonstrate the different login methods
                LoginDefaultAutoDemo();
                LoginLogoutDefaultDemo();
                LoginDisposeDemo();

                // Demonstrates how to get a list of available features
                GetInfoDemo();

                // run the API demo using the default feature
                // (ALWAYS present in every SuperDog)
                Dog dog = LoginDefaultDemo();
                SessionInfoDemo(dog);
                ReadWriteDemo(dog, FileId);
                ReadWritePosDemo(dog, FileId); 
                EncryptDecryptDemo(dog);
                TestTimeDemo(dog);
                LogoutDemo(ref dog);

                // run the API demo using feature id 42
                dog = LoginDemo(new DogFeature(DogFeature.FromFeature(42).Feature));
                SessionInfoDemo(dog);
                ReadWriteDemo(dog, FileId);
                ReadWritePosDemo(dog, FileId);
                EncryptDecryptDemo(dog);
                TestTimeDemo(dog);
                LogoutDemo(ref dog);

                Footer();
            }
            catch (Exception ex)
            {
                if (textHistory == null)
                    Console.WriteLine(ex.Message);
                else
                    System.Windows.Forms.MessageBox.Show(ex.Message,
                                                     "Exception",
                                                      System.Windows.Forms.MessageBoxButtons.OK);
            }
        }


        /// <summary>
        /// Demonstrates how to use to retrieve a XML containing all available features.
        /// </summary>
        protected void GetInfoDemo()
        {
            string queryFormat = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                                                      "<dogformat root=\"dog_info\">" +
                                                        " <feature>" +
                                                        "  <attribute name=\"id\" />" +
                                                        "  <element name=\"license\" />" +
                                                        " </feature>" +
                                                        "</dogformat>";

            Verbose("Get Information Demo");

            Verbose("Retrieving Feature Information");

            string info = null;

            DogStatus status = Dog.GetInfo(scope, queryFormat, VendorCode.Code, ref info);

            ReportStatus(status);
            if (DogStatus.StatusOk == status)
            {
                Verbose("Fature Information:");
                Verbose(info.Replace("\n", "\r\n          "));
            }
            else
                Verbose("");
        }


        /// <summary>
        /// Demonstrates how to retrieve information from a dog.
        /// </summary>
        protected void SessionInfoDemo(Dog dog)
        {
            if ((null == dog) || !dog.IsLoggedIn())
                return;

            Verbose("Get Session Information Demo");

            Verbose("Retrieving SuperDog Information");

            // firstly we will retrieve the dog info.
            string info = null;
            DogStatus status = dog.GetSessionInfo(Dog.KeyInfo,
                                                    ref info);
            ReportStatus(status);
            if (DogStatus.StatusOk == status)
            {
                Verbose("SuperDog Information:");
                Verbose(info.Replace("\n", "\r\n          "));
            }
            else
                Verbose("");

            Verbose("Retrieving Session Information");

            // next the session info.
            status = dog.GetSessionInfo(Dog.SessionInfo, ref info);
            ReportStatus(status);
            if (DogStatus.StatusOk == status)
            {
                Verbose("Session Information:");
                Verbose(info.Replace("\n", "\r\n          "));
            }
            else
                Verbose("");

            Verbose("Retrieving Update Information");

            // last the update information.
            status = dog.GetSessionInfo(Dog.UpdateInfo, ref info);
            ReportStatus(status);
            if (DogStatus.StatusOk == status)
            {
                Verbose("Update Information:");
                Verbose(info.Replace("\n", "\r\n          "));
            }
            else
                Verbose("");
        }


        /// <summary>
        /// Writes some descriptive text into the 
        /// referenced TextBox.
        /// </summary>
        protected void Verbose(string text)
        {
            if (null == text)
                return;

            FormDogDemo.WriteToTextbox(textHistory, "     " + text + "\r\n");
        }
    }
}
