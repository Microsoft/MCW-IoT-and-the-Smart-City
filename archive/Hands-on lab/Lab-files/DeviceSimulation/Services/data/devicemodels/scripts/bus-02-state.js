// Copyright (c) Microsoft. All rights reserved.

/*global log*/
/*global updateState*/
/*global updateProperty*/
/*jslint node: true*/

"use strict";

var center_latitude = 40.693935;
var center_longitude = -73.952279;

// Default state
var state = {
    online: true,
    latitude: center_latitude,
    longitude: center_longitude,
    fuellevel: 53.0,
    fuellevel_unit: "Gal",
    speed: 42.0,
    speed_unit: "mph",
    vin: "2K0H7PNZY0RSFQ033"
};

// Default properties
var properties = {};

/**
 * Restore the global state using data from the previous iteration.
 *
 * @param previousState device state from the previous iteration
 * @param previousProperties device properties from the previous iteration
 */
function restoreSimulation(previousState, previousProperties) {
    // If the previous state is null, force a default state
    if (previousState) {
        state = previousState;
    } else {
        log("Using default state");
    }

    if (previousProperties) {
        properties = previousProperties;
    } else {
        log("Using default properties");
    }
}

/**
 * Simple formula generating a random value around the average
 * in between min and max
 */
function vary(avg, percentage, min, max) {
    var value = avg * (1 + ((percentage / 100) * (2 * Math.random() - 1)));
    value = Math.max(value, min);
    value = Math.min(value, max);
    return value;
}

/**
 * Generate a random geolocation at some distance (in miles)
 * from a given location
 */
function varylocation(latitude, longitude, distance) {
    // Convert to meters, use Earth radius, convert to radians
    var radians = (distance * 1609.344 / 6378137) * (180 / Math.PI);
    return {
        latitude: latitude + radians,
        longitude: longitude + radians / Math.cos(latitude * Math.PI / 180)
    };
}

/**
 * Entry point function called by the simulation engine.
 * Returns updated simulation state.
 * Device property updates must call updateProperties() to persist.
 *
 * @param context             The context contains current time, device model and id
 * @param previousState       The device state since the last iteration
 * @param previousProperties  The device properties since the last iteration
 */
/*jslint unparam: true*/
function main(context, previousState, previousProperties) {

    // Restore the global device properties and the global state before
    // generating the new telemetry, so that the telemetry can apply changes
    // using the previous function state.
    restoreSimulation(previousState, previousProperties);

    // 0.1 miles around some location
    var coords = varylocation(center_latitude, center_longitude, 0.1);
    state.latitude = coords.latitude;
    state.longitude = coords.longitude;

    // 42 +/- 50%,  Min 0, Max 80
    // TODO: 3 - finish this line of code:
    //state.speed = vary(42, 50, 0, 80);

    // 53 +/- 25%,  Min 2, Max 80
    // TODO: 4 - finish this line of code:
    //state.fuellevel = vary(53, 25, 2, 80);

    updateState(state);
}
