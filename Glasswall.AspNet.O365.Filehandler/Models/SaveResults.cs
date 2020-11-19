﻿namespace Glasswall.AspNet.O365.Filehandler.Models
{
    public class SaveResults
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public string Filename { get; set; }
    }

    public class ShareLinkResults : SaveResults
    {
        public string SharingUrl { get; set; }
    }

    public class ItemContents
    {
        public string MarkdownText { get; set; }
    }
}