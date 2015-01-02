using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using Vsix.Common.Helpers;

namespace Vsix.Common
{
    public class MAPI
    {
        public bool AddRecipientTo(string email)
        {
            return AddRecipient(email, HowTo.MAPI_TO);
        }

        public bool AddRecipientCC(string email)
        {
            return AddRecipient(email, HowTo.MAPI_CC);
        }

        public bool AddRecipientBCC(string email)
        {
            return AddRecipient(email, HowTo.MAPI_BCC);
        }

        public void AddAttachment(string strAttachmentFileName)
        {
            _mAttachments.Add(strAttachmentFileName);
        }

        public int SendMailPopup(string strSubject, string strBody)
        {
            return SendMail(strSubject, strBody, MAPI_LOGON_UI | MAPI_DIALOG);
        }

        public int SendMailDirect(string strSubject, string strBody)
        {
            return SendMail(strSubject, strBody, MAPI_LOGON_UI);
        }

        [DllImport("MAPI32.DLL")]
        private static extern int MAPISendMail(IntPtr sess, IntPtr hwnd, MapiMessage message, int flg, int rsv);

        /// <summary>
        /// return status of the mail sent: 0-mail sent; 1-user abort, 
        /// for full list of codes see enum <seealso cref="Errors"/>.
        /// if code not 0 or 1 , error will be logged into log file.
        /// </summary>
        /// <param name="strSubject"></param>
        /// <param name="strBody"></param>
        /// <param name="how"></param>
        /// <returns></returns>
        private int SendMail(string strSubject, string strBody, int how)
        {
            var msg = new MapiMessage();
            try
            {
                msg.subject = strSubject;
                msg.noteText = strBody;
                msg.recips = GetRecipients(out msg.recipCount);
                msg.files = GetAttachments(out msg.fileCount);
                if (_mSender != null) msg.originator = GetSender();

                _mLastError = MAPISendMail(new IntPtr(0), new IntPtr(0), msg, how, 0);
                if (_mLastError > 1)
                {
                    LogHelper.LogError("MAPISendMail failed: " + GetLastError());
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            Cleanup(ref msg);
            return _mLastError;
        }

        private bool AddRecipient(string email, HowTo howTo)
        {
            _mRecipients.Add(new MapiRecipDesc {recipClass = (int) howTo, name = email});
            return true;
        }

        private IntPtr GetSender()
        {
            Type rtype = typeof (MapiRecipDesc);
            int rsize = Marshal.SizeOf(rtype);
            IntPtr ptrr = Marshal.AllocHGlobal(rsize);

            Marshal.StructureToPtr(_mSender, ptrr, false);
            return ptrr;
        }

        private IntPtr GetRecipients(out int recipCount)
        {
            recipCount = 0;
            if (_mRecipients.Count == 0)
                return IntPtr.Zero;

            int size = Marshal.SizeOf(typeof (MapiRecipDesc));
            IntPtr intPtr = Marshal.AllocHGlobal(_mRecipients.Count*size);

            int ptr = (int) intPtr;
            foreach (MapiRecipDesc mapiDesc in _mRecipients)
            {
                Marshal.StructureToPtr(mapiDesc, (IntPtr) ptr, false);
                ptr += size;
            }

            recipCount = _mRecipients.Count;
            return intPtr;
        }

        private IntPtr GetAttachments(out int fileCount)
        {
            fileCount = 0;
            if (_mAttachments == null)
                return IntPtr.Zero;

            if (_mAttachments.Count < 1 || _mAttachments.Count > MaxAttachments)
                return IntPtr.Zero;

            int size = Marshal.SizeOf(typeof (MapiFileDesc));
            IntPtr intPtr = Marshal.AllocHGlobal(_mAttachments.Count*size);

            var mapiFileDesc = new MapiFileDesc {position = -1};
            int ptr = (int) intPtr;

            foreach (string strAttachment in _mAttachments)
            {
                mapiFileDesc.name = Path.GetFileName(strAttachment);
                mapiFileDesc.path = strAttachment;
                Marshal.StructureToPtr(mapiFileDesc, (IntPtr) ptr, false);
                ptr += size;
            }

            fileCount = _mAttachments.Count;
            return intPtr;
        }

        private void Cleanup(ref MapiMessage msg)
        {
            int size = Marshal.SizeOf(typeof (MapiRecipDesc));
            int ptr = 0;

            if (msg.recips != IntPtr.Zero)
            {
                ptr = (int) msg.recips;
                for (int i = 0; i < msg.recipCount; i++)
                {
                    Marshal.DestroyStructure((IntPtr) ptr, typeof (MapiRecipDesc));
                    ptr += size;
                }
                Marshal.FreeHGlobal(msg.recips);
            }

            if (msg.files != IntPtr.Zero)
            {
                size = Marshal.SizeOf(typeof (MapiFileDesc));

                ptr = (int) msg.files;
                for (int i = 0; i < msg.fileCount; i++)
                {
                    Marshal.DestroyStructure((IntPtr) ptr, typeof (MapiFileDesc));
                    ptr += size;
                }
                Marshal.FreeHGlobal(msg.files);
            }

            _mRecipients.Clear();
            _mAttachments.Clear();
        }

        public string GetLastError()
        {
            return (_mLastError >= 0 && _mLastError < Errors.Length)
                       ? Errors[_mLastError]
                       : "MAPI error [" + _mLastError + "]";
        }

        private static string[] Errors
        {
            get
            {
                return new[]
                           {

                               "OK [0]",
                               "User abort [1]",
                               "General MAPI failure [2]",
                               "MAPI login failure [3]",
                               "Disk full [4]",
                               "Insufficient memory [5]",
                               "Access denied [6]",
                               "-unknown- [7]",
                               "Too many sessions [8]",
                               "Too many files were specified [9]",
                               "Too many recipients were specified [10]",
                               "A specified attachment was not found [11]",
                               "Attachment open failure [12]",
                               "Attachment write failure [13]",
                               "Unknown recipient [14]",
                               "Bad recipient type [15]",
                               "No messages [16]",
                               "Invalid message [17]",
                               "Text too large [18]",
                               "Invalid session [19]",
                               "Type not supported [20]",
                               "A recipient was specified ambiguously [21]",
                               "Message in use [22]",
                               "Network failure [23]",
                               "Invalid edit fields [24]",
                               "Invalid recipients [25]",
                               "Not supported [26]"

                           };

            }
        }

        private readonly List<MapiRecipDesc> _mRecipients = new List<MapiRecipDesc>();
        private readonly List<string> _mAttachments = new List<string>();
        private int _mLastError = 0;
        private MapiRecipDesc _mSender = null;

        private bool SetSenderInternal(string email)
        {
            _mSender = new MapiRecipDesc
                           {
                               recipClass = (int) HowTo.MAPI_ORIG,
                               name = email,
                               address = email,
                               eIDSize = 0,
                               entryID = IntPtr.Zero
                           };
            return true;
        }

        public bool SetSender(string email)
        {
            return SetSenderInternal(email);
        }

        /// <summary> 1 </summary>
        private const int MAPI_LOGON_UI = 0x00000001;

        /// <summary> 8 </summary>
        private const int MAPI_DIALOG = 0x00000008;

        /// <summary> 20 </summary>
        private const int MaxAttachments = 20;

        private enum HowTo
        {
            MAPI_ORIG = 0,
            MAPI_TO,
            MAPI_CC,
            MAPI_BCC
        };


        //private static bool _mailSent = false;
        //private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        //{
        //    // Get the unique identifier for this asynchronous operation.
        //    string token = (string) e.UserState;

        //    if (e.Cancelled)
        //        Console.WriteLine("[{0}] Send canceled.", token);

        //    if (e.Error != null)
        //        Console.WriteLine("[{0}] {1}", token, e.Error);
        //    else
        //        Console.WriteLine("Message sent.");

        //    _mailSent = true;
        //}

        /// <summary>
        /// send email using .Net API
        /// </summary>
        /// <param name="host">var client = new SmtpClient(host,port)</param>
        /// <param name="port">var client = new SmtpClient(host,port)</param>
        /// <param name="addressFrom"></param>
        /// <param name="addressTo">destination email(s)</param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachementPath"></param>
        /// <param name="enableSsl"></param>
        /// <param name="password"></param>
        /// <param name="user"></param>
        /// <param name="timeout"></param>
        public static void SendMailSMTP(string host, int port, string addressFrom, string addressTo,
                                 string subject, string body, string attachementPath,
                                 bool enableSsl, string user, string password, int timeout = 30000)
        {
            try
            {
                var message = new MailMessage(addressFrom, addressTo, subject, body)
                                  {
                                      IsBodyHtml = false,
                                      BodyEncoding = System.Text.Encoding.UTF8,
                                      SubjectEncoding = System.Text.Encoding.UTF8,
                                  };
                if (!string.IsNullOrEmpty(attachementPath))
                {
                    int k = 0;
                    foreach (string attch in attachementPath.Split(';'))
                    {
                        // limit number of attachments to 20
                        if (k++<20 && !string.IsNullOrEmpty( attch) &&  File.Exists(attch))
                            message.Attachments.Add(new Attachment(attch));
                    }
                }
                var client = new SmtpClient(host, port)
                                 {
                                     EnableSsl = enableSsl,
                                     UseDefaultCredentials = false,
                                     Credentials = new NetworkCredential(user, password),
                                     Timeout = timeout,
                                 };


                client.Send(message);

                // do not use async:
                //// to send mail async: SendAsync
                //client.SendCompleted += SendCompletedCallback;
                //string userState = "finished sending message";
                //client.SendAsync(message, userState);

                message.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }

    }


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MapiMessage
    {
        public int reserved;
        public string subject;
        public string noteText;
        public string messageType;
        public string dateReceived;
        public string conversationID;
        public int flags;
        public IntPtr originator;
        public int recipCount;
        public IntPtr recips;
        public int fileCount;
        public IntPtr files;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MapiFileDesc
    {
        public int reserved;
        public int flags;
        public int position;
        public string path;
        public string name;
        public IntPtr type;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MapiRecipDesc
    {
        public int reserved;
        public int recipClass;
        public string name;
        public string address;
        public int eIDSize;
        public IntPtr entryID;
    }
}