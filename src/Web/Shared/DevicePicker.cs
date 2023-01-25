// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Heimdall.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Heimdall.Web.Shared;

public sealed class DevicePicker : InputSelect<string>
{
    [Parameter]
    public IReadOnlyCollection<DeviceType> DeviceTypeFilter { get; set; }

    [Inject]
    private DeviceRepository DeviceRepository { get; set; }

    private IReadOnlyCollection<Device> devices = null;

    protected override async Task OnInitializedAsync()
    {
        this.devices = await this.DeviceRepository.GetDevicesAsync(
            this.DeviceTypeFilter);
        await base.OnInitializedAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        this.ChildContent = null;

        if (this.devices is not null)
        {
            this.ChildContent = optionsBuilder =>
            {
                foreach (var device in this.devices)
                {
                    optionsBuilder.OpenElement(0, "option");
                    optionsBuilder.AddAttribute(1, "value", device.Id);
                    optionsBuilder.AddContent(2, device.Name);
                    optionsBuilder.CloseElement();
                }
            };
        }

        base.BuildRenderTree(builder);
    }
}
