// Copyright (c) Microsoft. All rights reserved.

/*global log*/
/*global updateState*/
/*global sleep*/
/*jslint node: true*/

"use strict";

// Default state
var state = {
    online: true
};


/**
 * Restore the global state using data from the previous iteration.
 *
 * @param previousState The output of main() from the previous iteration
 */
function restoreState(previousState) {
    // If the previous state is null, force a default state
    if (previousState !== undefined && previousState !== null) {
        state = previousState;
    } else {
        log("Using default state");
    }
}

/**
 * Entry point function called by the simulation engine.
 *
 * @param context        The context contains current time, device model and id, not used
 * @param previousState  The device state since the last iteration, not used
 */

/*jslint unparam: true*/
function main(context, previousState) {

    log("Executing 'Increase Timing' method on traffic light");

    restoreState(previousState);

    log("Traffic light is currently timed to: " + state.timing + " " + state.timing_unit);

    // Pause the simulation
    state.CalculateRandomizedTelemetry = false;
    updateState(state);

    // Increase
    state.timing += 15;
    updateState(state);
    log("Traffic light timing increased to " + state.timing + " " + state.timing_unit);
    sleep(1000);

    // Resume the simulation
    state.CalculateRandomizedTelemetry = true;
    updateState(state);

    log("'Increase Timing' method completed");
}
