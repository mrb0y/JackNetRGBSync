﻿namespace RGBSyncPlus.UI.Tabs
{
    public class ModalModel
    {
        public string ModalText { get; set; }
        public bool ShowModalTextBox { get; set; }
        public bool ShowModalCloseButton { get; set; }
        public System.Action<string> modalSubmitAction { get; set; }
    }
}