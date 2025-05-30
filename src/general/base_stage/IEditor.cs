﻿using System.Collections.Generic;
using AutoEvo;
using Godot;
using Newtonsoft.Json;

/// <summary>
///   Interface extracted to make GUI generic parameters work
/// </summary>
public interface IEditor : ISaveLoadedTracked
{
    /// <summary>
    ///   The number of mutation points left. This is a floating point number as a whole number of mutation points is
    ///   way too restrictive for many edits.
    /// </summary>
    public double MutationPoints { get; }

    /// <summary>
    ///   When true nothing costs MP
    /// </summary>
    public bool FreeBuilding { get; }

    /// <summary>
    ///   True when there is an action that can be canceled
    /// </summary>
    public bool CanCancelAction { get; }

    /// <summary>
    ///   True, once fade transition is finished when entering editor
    /// </summary>
    public bool TransitionFinished { get; }

    /// <summary>
    ///   True when the editor view is active and the user can perform an action (for example place an organelle)
    /// </summary>
    public bool ShowHover { get; set; }

    /// <summary>
    ///   Root node under which editor components should put their 3D space Nodes (placed things, editor controls etc.)
    /// </summary>
    public Node3D RootOfDynamicallySpawned { get; }

    /// <summary>
    ///   The main current game object holding various details
    /// </summary>
    public GameProperties CurrentGame { get; }

    /// <summary>
    ///   Access to the base class typed species that is edited, for use in various things in the editor systems
    ///   that don't need to care about the concrete type of species being edited
    /// </summary>
    public Species EditedBaseSpecies { get; }

    /// <summary>
    ///   Access to the auto-evo results of the previous run that ended before entering the current editor session.
    ///   May be null in unusual circumstances or before the editor has fully loaded.
    /// </summary>
    [JsonIgnore]
    public RunResults? PreviousAutoEvoResults { get; }

    /// <summary>
    ///   True once auto-evo (and possibly other stuff) the editor needs to wait for is ready
    /// </summary>
    public bool EditorReady { get; }

    public float DayLightFraction { get; set; }

    /// <summary>
    ///   Cancels the current editor action if possible
    /// </summary>
    /// <returns>True if canceled</returns>
    public bool CancelCurrentAction();

    public double WhatWouldActionsCost(IEnumerable<EditorCombinableActionData> actions);

    /// <summary>
    ///   Perform all actions through this to make undo and redo work
    /// </summary>
    /// <returns>True when the action was successful</returns>
    /// <remarks>
    ///   <para>
    ///     This takes in a base action type so that this interface doesn't need to depend on the specific action
    ///     type of the editor, which causes some pretty nasty generic constraint interdependencies
    ///   </para>
    /// </remarks>
    public bool EnqueueAction(ReversibleAction action);

    /// <summary>
    ///   Adds editor-state-specific context to the given sequence of actions. <see cref="EnqueueAction"/> and
    ///   <see cref="WhatWouldActionsCost"/> perform this automatically. Only adds the context if not missing to give
    ///   flexibility for editor components to add their custom action context that is not overridden.
    /// </summary>
    /// <param name="actions">The action data to add the context to</param>
    public void AddContextToActions(IEnumerable<CombinableActionData> actions);

    public void NotifyUndoRedoStateChanged();

    public bool CheckEnoughMPForAction(double cost);

    public void OnInsufficientMP(bool playSound = true);

    public void OnActionBlockedWhileMoving();

    public void OnInvalidAction();

    public void OnValidAction(IEnumerable<CombinableActionData> actions);

    /// <summary>
    ///   Request from the user to exit the editor anyway
    /// </summary>
    /// <param name="userOverrides">
    ///   The new user overrides to be used when exiting. Caller should add their own override that was just approved
    ///   to the overrides they were given through <see cref="EditorComponentBase{TEditor}.CanFinishEditing"/>.
    /// </param>
    /// <returns>True if editing was able to finish now, false if still some component is not ready to exit</returns>
    public bool RequestFinishEditingWithOverride(List<EditorUserOverride> userOverrides);

    /// <summary>
    ///   Request that GUI for showing extra info about a species is opened
    /// </summary>
    /// <param name="species">The species to open info for</param>
    public void OpenSpeciesInfoFor(Species species);

    /// <summary>
    ///   Calculates the next generation time. Used to precalculate timings for future events.
    /// </summary>
    /// <returns>Predicted next <see cref="GameWorld.TotalPassedTime"/></returns>
    public double CalculateNextGenerationTimePoint();
}
