using System.Linq;
using UnityEngine;

/// <summary>
/// Class centralizing extension methods for the GameObject class
/// around the multi-tags system.
/// </summary>
public static class GOTagsExtensions 
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
     *      - What about methods allowing to dynamically add or remove tags
     *  to a game object ? It would be cool !
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
     *	    - Added a method to access a game object tags with the GetTags method,
     *	and to check if the object contain one or multiple tags using the
     *	HasTag & HasTags methods.
	 *
	 *	-----------------------------------
	*/

    #region Methods
    /// <summary>
    /// Get all this game object tags.
    /// </summary>
    /// <param name="_go">Game object to get tags from.</param>
    /// <returns>Returns all this game object tags.</returns>
    public static string[] GetTags(this GameObject _go)
    {
        return _go.tag.Split(MultiTags.TagSeparator);
    }

    /// <summary>
    /// Does this game object has a specific tag ?
    /// </summary>
    /// <param name="_go">Game object to compare tags.</param>
    /// <param name="_tag">Tag to compare.</param>
    /// <returns>Returns true if the game object has the specified tag, false otherwise.</returns>
    public static bool HasTag(this GameObject _go, string _tag)
    {
        return _go.GetTags().Contains(_tag);
    }

    /// <summary>
    /// Does this game object has at least one of the specified tags ?
    /// </summary>
    /// <param name="_go">Game object to compare tags.</param>
    /// <param name="_tags">Tags to compare.</param>
    /// <returns>Returns true if the game object has at least one of the specified tags, false otherwise.</returns>
    public static bool HasTag(this GameObject _go, string[] _tags)
    {
        return _go.GetTags().Intersect(_tags).Any();
    }

    /// <summary>
    /// Does this game object has all the specified tags ?
    /// </summary>
    /// <param name="_go">Game object to compare tags.</param>
    /// <param name="_tags">All tags to compare.</param>
    /// <returns>Returns true if the game object has all the specified tags, false if it lacks event one.</returns>
    public static bool HasTags(this GameObject _go, string[] _tags)
    {
        return !_go.GetTags().Except(_tags).Any();
    }
    #endregion
}
