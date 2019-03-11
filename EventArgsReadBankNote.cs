using System;
using System.Collections.Generic;
using System.Text;

namespace ICTL77F
{
    public enum BILL_STATUS
    {
        ENABLED,
        DISABLED,
        ERROR,

    }

    public class EventArgsReadBankNote : EventArgs
    {
        private string strBankNote;
        public string BankNote
        {
            get
            {
                return this.strBankNote;
            }
            set
            {
                this.strBankNote = value;
            }
        }
    }
    public class EventArgsBankNoteStatus : EventArgs
    {
        public BILL_STATUS Status;
        public EventArgsBankNoteStatus (BILL_STATUS _status)
        {
            Status = _status;
        }
    }
}
