using System;

namespace Zeus
{
    public class AppRelease : IAppRelease
    {
        private string _title, _description, _author;
        private Uri _downloadLink, _releaseNotesLink;
        private DateTime _date;
        private Version _version;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public Version AppVersion
        {
            get { return _version; }
            set { _version = value; }
        }

        public Uri DownloadLink
        {
            get { return _downloadLink; }
            set { _downloadLink = value; }
        }

        public Uri ReleaseNotesLink
        {
            get { return _releaseNotesLink; }
            set { _releaseNotesLink = value; }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
    }
}