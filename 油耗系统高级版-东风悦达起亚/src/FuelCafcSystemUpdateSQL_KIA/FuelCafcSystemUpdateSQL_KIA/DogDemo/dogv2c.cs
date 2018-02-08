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
    public class DogDemoV2c : DogDemo
    {
        /// <summary>
        /// Constructor.
        /// Initializes the object
        /// </summary>
        public DogDemoV2c(System.Windows.Forms.TextBox textHistory)
            : base(textHistory)
        {
        }


        /// <summary>
        /// Prints the footer message into the
        /// referenced TextBox
        /// </summary>
        protected override void Footer()
        {
            FormDogDemo.WriteToTextbox(textHistory, "SuperDog Update completed\r\n");
        }


        /// <summary>
        /// Prints the header message into the
        /// referenced TextBox
        /// </summary>
        protected override void Header()
        {
            if (textHistory != null && 0xFFFF < textHistory.TextLength)
                textHistory.Clear();

            FormDogDemo.WriteToTextbox(textHistory,
                "____________________________________________________________\r\n" +
                string.Format("SuperDog Update started ({0})\r\n\r\n",
                    DateTime.Now.ToString()));
        }


        /// <summary>
        /// Updates SuperDog using the passed update string
        /// and writes the returned acknowledge (if available)
        /// into the referenced TextBox.
        /// </summary>
        public new void RunDemo(string update)
        {
            try
            {
                Header();

                // print the update string
                Verbose("Update information:");
                Verbose(update.Replace("\n", "\r\n     "));
                Verbose("");

                string ack = null;

                // perform the update
                // please note that the SuperDog's Update method is
                // static.
                DogStatus status = Dog.Update(update, ref ack);
                ReportStatus(status);

                if (DogStatus.StatusOk == status)
                {
                    // print the acknowledgement
                    // the return of an ack. is controlled via the
                    // update package.
                    Verbose("Acknowledge information:");
                    Verbose(((null == ack) || (0 == ack.Length)) ? "Not available" :
                                                            ack.Replace("\n", "\r\n     "));
                }
                Verbose("");
                Footer();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message,
                                                     "Exception",
                                                      System.Windows.Forms.MessageBoxButtons.OK);
            }

        }
    }
}
