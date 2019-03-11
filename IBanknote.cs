using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace ICTL77F
{
    public interface IBanknote
    {
        bool Open();
        void Close();
        void Reset();
        void Reject();
        void Accept();
        void Hold();
        bool FireReadStatus();

    }
}
