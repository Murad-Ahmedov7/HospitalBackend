using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Business.Enums.Auth
{
    public enum LoginStatus
    {
        Success,
        InvalidCredentials,
        AccountLocked,
    }
}
