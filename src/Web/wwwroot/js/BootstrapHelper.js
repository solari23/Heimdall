/* Copyright (c) Alexandre Kerametlian.
 * Licensed under the Apache License, Version 2.0.
 * 
 * A collection of functions to facilitate interop between Blazor and Bootstrap components.
 */

var collapseMap = {};

function BootstrapHelper_ToggleClassOnElement(elementId, classToToggle) {
    var element = document.getElementById(elementId);
    element.classList.toggle(classToToggle);
}

function BootstrapHelper_ToggleCollapse(elementId) {

    if (collapseMap[elementId] === undefined) {
        var element = document.getElementById(elementId);
        collapseMap[elementId] = new bootstrap.Collapse(element);
    }

    collapseMap[elementId].toggle();
}