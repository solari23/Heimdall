// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Heimdall.Web.Shared;

public sealed class EnumValuePicker<TEnum> : InputSelect<TEnum>
    where TEnum : struct, Enum
{
    [Parameter]
    public IReadOnlyCollection<TEnum> ExcludeValues { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var allowedEnumValues = Enum.GetValues<TEnum>();

        if (this.ExcludeValues is not null && this.ExcludeValues.Count > 0)
        {
            allowedEnumValues = allowedEnumValues.Where(e => !this.ExcludeValues.Contains(e)).ToArray();
        }

        this.ChildContent = optionsBuilder =>
        {
            foreach (var enumValue in allowedEnumValues)
            {
                var displayValue = EnumUtil<TEnum>.ToPrettyString(enumValue);

                optionsBuilder.OpenElement(0, "option");
                optionsBuilder.AddAttribute(1, "value", enumValue);
                optionsBuilder.AddContent(2, displayValue);
                optionsBuilder.CloseElement();
            }
        };

        if (this.ExcludeValues is not null
            && !allowedEnumValues.Contains(this.CurrentValue))
        {
            this.CurrentValue = allowedEnumValues[0];
        }

        base.BuildRenderTree(builder);
    }
}
