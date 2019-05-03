using System;

namespace MadsKristensen.AddAnyFile.Helpers
{
    [Flags]
    public enum SelectedInputType
    {
        None = 0,
        Interface = 1,
        Class = 2,
        Controller = 4,
        Enum = 8,
        Folder = 16,
        Base = 32,
    }
}
