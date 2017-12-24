﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutumnBox.GUI.UI
{
    public interface IAsyncRefreshable:IRefreshable
    {
        event EventHandler RefreshStart;
        event EventHandler RefreshFinished;
    }
}