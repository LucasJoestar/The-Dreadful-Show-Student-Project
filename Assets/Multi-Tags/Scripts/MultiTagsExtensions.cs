﻿using System.Linq;
using UnityEngine;

/// <summary>
/// Class centralizing extension methods around the multi-tags system.
/// </summary>
public static class MultiTagsExtensions 
{
    /* GOTagsExtensions :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Connect the Multi-tags system to the GameObject class with extension methods.
     *	    
     *	    Adds extension methods to the GameObject class to get all its tags from
     *	the Unity system, and check if it has a certain tag.
	 *
     *	#####################
	 *	####### TO DO #######
	 *	#####################
     * 
     *      Nothing to see here...
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[20 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Creation of the GameObjectTagsExtensions class.
     *	    
     *	    • Added a method to access a game object tags with the GetTags method,
     *	and to check if the object contain one or multiple tags using the
     *	HasTag & HasTags methods.
	 *
	 *	-----------------------------------
	*/

    #region GameObject
    /// <summary>
    /// Add a new tag to this GameObject.
    /// </summary>
    /// <param name="_go">Game object to add tag to.</param>
    /// <param name="_newTag">New tag to add.</param>
    public static void AddTag(this GameObject _go, string _newTag)
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) return;
        #endif

        MultiTags.TagsAsset.AddTagToObject(_go, _newTag);
    }

    /// <summary>
    /// Add a new tag to this GameObject.
    /// </summary>
    /// <param name="_go">Game object to add tag to.</param>
    /// <param name="_newTag">New tag to add.</param>
    public static void AddTag(this GameObject _go, Tag _newTag)
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) return;
        #endif

        MultiTags.TagsAsset.AddTagToObject(_go, _newTag);
    }


    /// <summary>
    /// Get all this game object tag names.
    /// </summary>
    /// <param name="_go">Game object to get tags from.</param>
    /// <returns>Returns all this game object tag names.</returns>
    public static string[] GetTagNames(this GameObject _go)
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) return _go.tag.Split(MultiTags.TAG_SEPARATOR);
        #endif

        return MultiTags.TagsAsset.GetObjectTags(_go).Select(t => t.Name).ToArray();
    }

    /// <summary>
    /// Get a Tag objects from all this game object tags.
    /// </summary>
    /// <param name="_go">Game object to get tags from.</param>
    /// <returns>Returns a Tags object from all this game object tags.</returns>
    public static Tag[] GetTags(this GameObject _go)
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) return MultiTags.GetTags(_go.GetTagNames());
        #endif

        return MultiTags.TagsAsset.GetObjectTags(_go);
    }


    /// <summary>
    /// Does this game object has a specific tag ?
    /// </summary>
    /// <param name="_go">Game object to compare tags.</param>
    /// <param name="_tag">Tag to compare.</param>
    /// <returns>Returns true if the game object has the specified tag, false otherwise.</returns>
    public static bool HasTag(this GameObject _go, string _tag)
    {
        return _go.GetTagNames().Contains(_tag);
    }

    /// <summary>
    /// Does this game object has at least one of the specified tags ?
    /// </summary>
    /// <param name="_go">Game object to compare tags.</param>
    /// <param name="_tags">Tags to compare.</param>
    /// <returns>Returns true if the game object has at least one of the specified tags, false otherwise.</returns>
    public static bool HasTag(this GameObject _go, string[] _tags)
    {
        return _go.GetTagNames().Intersect(_tags).Any();
    }


    /// <summary>
    /// Does this game object has all the specified tags ?
    /// </summary>
    /// <param name="_go">Game object to compare tags.</param>
    /// <param name="_tags">All tags to compare.</param>
    /// <returns>Returns true if the game object has all the specified tags, false if it lacks event one.</returns>
    public static bool HasTags(this GameObject _go, string[] _tags)
    {
        return !_go.GetTagNames().Except(_tags).Any();
    }

    /// <summary>
    /// Does this game object has a specific tag ?
    /// </summary>
    /// <param name="_go">Game object to compare tags.</param>
    /// <param name="_tag">Tag to compare.</param>
    /// <returns>Returns true if the game object has the specified tag, false otherwise.</returns>
    public static bool HasTag(this GameObject _go, Tag _tag)
    {
        return _go.GetTagNames().Contains(_tag.Name);
    }

    /// <summary>
    /// Does this game object has at least one of the specified tags ?
    /// </summary>
    /// <param name="_go">Game object to compare tags.</param>
    /// <param name="_tags">Tags to compare.</param>
    /// <returns>Returns true if the game object has at least one of the specified tags, false otherwise.</returns>
    public static bool HasTag(this GameObject _go, Tag[] _tags)
    {
        return _go.GetTagNames().Intersect(_tags.Select(t => t.Name)).Any();
    }

    /// <summary>
    /// Does this game object has all the specified tags ?
    /// </summary>
    /// <param name="_go">Game object to compare tags.</param>
    /// <param name="_tags">All tags to compare.</param>
    /// <returns>Returns true if the game object has all the specified tags, false if it lacks event one.</returns>
    public static bool HasTags(this GameObject _go, Tag[] _tags)
    {
        return !_go.GetTagNames().Except(_tags.Select(t => t.Name)).Any();
    }


    /// <summary>
    /// Removes a tag from a GameObject.
    /// </summary>
    /// <param name="_go">Game object to remove tag from.</param>
    /// <param name="_tag">Tag to remove.</param>
    public static void RemoveTag(this GameObject _go, string _tag)
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) return;
        #endif

        MultiTags.TagsAsset.RemoveTagFromObject(_go, _tag);
    }

    /// <summary>
    /// Removes a tag from a GameObject.
    /// </summary>
    /// <param name="_go">Game object to remove tag from.</param>
    /// <param name="_tag">Tag to remove.</param>
    public static void RemoveTag(this GameObject _go, Tag _tag)
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) return;
        #endif

        MultiTags.TagsAsset.RemoveTagFromObject(_go, _tag);
    }
    #endregion
}
