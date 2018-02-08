////////////////////////////////////////////////////////////////////
// Copyright (C) 2012 SafeNet, Inc. All rights reserved.
//
// SuperDog(R) is a trademark of SafeNet, Inc. 
//
//
////////////////////////////////////////////////////////////////////
using System;
using SuperDog;


namespace DogDemo
{
    /// <summary>
    /// </summary>
    public class DogDemoC2v : DogDemo
    {
        /// <summary>
        /// Constructor.
        /// Initializes the object
        /// </summary>
        public DogDemoC2v(System.Windows.Forms.TextBox textHistory)
            : base(textHistory)
        {
        }


        /// <summary>
        /// Prints the footer message into the 
        /// referenced TextBox
        /// </summary>
        protected override void Footer()
        {
            FormDogDemo.WriteToTextbox(textHistory, "Generation of Status Information completed\r\n");
        }


        /// <summary>
        /// Prints the header into the 
        /// referenced TextBox
        /// </summary>
        protected override void Header()
        {
            if (textHistory != null && 0xFFFF < textHistory.TextLength)
                textHistory.Clear();

            FormDogDemo.WriteToTextbox(textHistory, 
                "____________________________________________________________\r\n"+
                string.Format("Generation of Status Information started ({0})\r\n\r\n",
                                              DateTime.Now.ToString()));
        }


        /// <summary>
        /// Retrieves the update information 
        /// without logging in using
        /// the SuperDog's GetInfo method.
        /// </summary>
        public string RunDemo()
        {
            string info = "";
            try
            {
                Header();

                Verbose("Retrieving Update Information");

                // now get the update information
                Verbose("Retrieving Information");
                DogStatus status = Dog.GetInfo(defaultScope, Dog.UpdateInfo, VendorCode.Code, ref info);
                ReportStatus(status);

                if (DogStatus.StatusOk == status)
                    Verbose(info.Replace("\n", "\r\n     "));
                else
                    Verbose("");

                Verbose("");
                Footer();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message,
                                                     "Exception",
                                                      System.Windows.Forms.MessageBoxButtons.OK);
            }
            return info;
        }
    }
}