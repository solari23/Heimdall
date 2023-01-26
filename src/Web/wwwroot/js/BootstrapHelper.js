/* Copyright (c) Alexandre Kerametlian.
 * Licensed under the Apache License, Version 2.0.
 * 
 * A collection of functions to facilitate interop between Blazor and Bootstrap components.
 */

function BootstrapHelper_ToggleClassOnElement(elementId, classToToggle) {
    var element = document.getElementById(elementId);
    element.classList.toggle(classToToggle);
}

// Maintains a 2-level map: [groupTag -> elementId -> collapse object].
// Keeping the elements groupped by tag lets the caller release the objects 
// in one shot when they're no longer needed.
var collapseMap = {};

function BootstrapHelper_ToggleCollapse(groupTag, elementId) {
    if (collapseMap[groupTag] === undefined) {
        collapseMap[groupTag] = {};
    }

    if (collapseMap[groupTag][elementId] === undefined) {
        var element = document.getElementById(elementId);
        collapseMap[groupTag][elementId] = new bootstrap.Collapse(element);
    }

    collapseMap[groupTag][elementId].toggle();
}

function BootstrapHelper_ReleaseCollapse(groupTag, elementId) {
    if (collapseMap[groupTag] === undefined) {
        return;
    }

    if (collapseMap[groupTag][elementId] !== undefined) {
        collapseMap[groupTag][elementId].dispose();
        delete collapseMap[groupTag][elementId];
    }
}

function BootstrapHelper_ReleaseCollapseGroup(groupTag) {
    if (collapseMap[groupTag] === undefined) {
        return;
    }

    for (const key in collapseMap[groupTag]) {
        collapseMap[groupTag][key].dispose();
    }

    delete collapseMap[groupTag];
}