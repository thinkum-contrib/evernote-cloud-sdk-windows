using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using EvernoteSDK;
using EvernoteSDK.Advanced;
using Evernote.EDAM.Type;

namespace SampleAppAdvanced
{
    class Program
    {
        protected static ResourceManager rm = new ResourceManager("api", typeof(Program).Assembly);
        protected static readonly string enToken = rm.GetString("token");
        protected static readonly string enStore = rm.GetString("store");

        static void Main(string[] args)
        {

            Console.WriteLine("Using auth store: " + enStore);

            // Supply your key using ENSessionAdvanced instead of ENEsssion, to indicate your use of the Advanced interface.
            //
            // For purposes of testing, this example will use Evernote developer tokens rather than OAuth2
            //
            // Be sure to store your private developer token as the 'token' property value and the store URL 
            // as the 'store' property value in api.restext. An example of the property file syntax is 
            // available in api.restext.sample
            //
            // More information about developer tokens in the Evernote API:
            //  https://dev.evernote.com/doc/articles/dev_tokens.php
            //
            ENSessionAdvanced.SetSharedSessionDeveloperToken(enToken, enStore);

            if (ENSession.SharedSession.IsAuthenticated == false)
            {
                ENSession.SharedSession.AuthenticateToEvernote();
            }

            // Create a note (in the user's default notebook) with an attribute set (in this case, the ReminderOrder attribute to create a Reminder).
            ENNoteAdvanced myNoteAdv = new ENNoteAdvanced();
            myNoteAdv.Title = "Sample note with Reminder set";
            myNoteAdv.Content = ENNoteContent.NoteContentWithString("Hello, world - this note has a Reminder on it.");
            myNoteAdv.EdamAttributes["ReminderOrder"] = DateTime.Now.ToEdamTimestamp();
            ENNoteRef myRef = ENSession.SharedSession.UploadNote(myNoteAdv, null);

            // Now we'll create an EDAM Note.
            // First create the ENML content for the note.
            ENMLWriter writer = new ENMLWriter();
            writer.WriteStartDocument();
            writer.WriteString("Hello again, world.");
            writer.WriteEndDocument();
            // Create a note locally.
            Note myNote = new Note();
            myNote.Title = "Sample note from the Advanced world";
            myNote.Content = writer.Contents.ToString();
            // Create the note in the service, in the user's personal, default notebook.
            ENNoteStoreClient store = ENSessionAdvanced.SharedSession.PrimaryNoteStore;
            Note resultNote = store.CreateNote(myNote);
        }
    }
}
