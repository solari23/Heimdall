// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Microsoft.JSInterop;

namespace Heimdall.Web;

public class BootstrapHelper
{
    public BootstrapHelper(IJSRuntime jsRuntime)
    {
        this.JSRuntime = jsRuntime;
    }

    private IJSRuntime JSRuntime { get; }

    public void ToggleCollapse(string collapseElementId, string toggleButtonElementId = null)
    {
        if (toggleButtonElementId is not null)
        {
            this.JSRuntime.InvokeVoidAsync(
                "BootstrapHelper_ToggleClassOnElement",
                toggleButtonElementId,
                "collapsed");
        }
        this.JSRuntime.InvokeVoidAsync("BootstrapHelper_ToggleCollapse", collapseElementId);
    }
}
