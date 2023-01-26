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

    public async Task ToggleCollapseAsync(string groupTag, string collapseElementId, string toggleButtonElementId = null)
    {
        if (toggleButtonElementId is not null)
        {
            await this.JSRuntime.InvokeVoidAsync(
                "BootstrapHelper_ToggleClassOnElement",
                toggleButtonElementId,
                "collapsed");
        }
        await this.JSRuntime.InvokeVoidAsync("BootstrapHelper_ToggleCollapse", groupTag, collapseElementId);
    }

    public async Task ReleaseCollapseAsync(string groupTag, string elementId)
    {
        await this.JSRuntime.InvokeVoidAsync("BootstrapHelper_ReleaseCollapse", groupTag, elementId);
    }

    public async Task ReleaseCollapseGroupAsync(string groupTag)
    {
        await this.JSRuntime.InvokeVoidAsync("BootstrapHelper_ReleaseCollapseGroup", groupTag);
    }
}
